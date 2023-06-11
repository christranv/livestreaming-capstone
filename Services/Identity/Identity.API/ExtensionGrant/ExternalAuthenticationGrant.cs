using System;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.API.Providers;
using Identity.API.Helpers;
using Newtonsoft.Json.Linq;
using Identity.API.Models;
using System.Net.Mail;
using System.Net.Http;
using System.IO;
using Identity.API.IntegrationEvents;
using Identity.API.Providers.Interfaces;
using Identity.API.Services.Interfaces;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Identity;

namespace Identity.API.ExtensionGrant
{
    public class ExternalAuthenticationGrant : IExtensionGrantValidator
    {
        private static readonly List<string> AdminEmailWhitelist = new()
        {
            "tnhlong@gmail.com",
            "chuongtran1999dn@gmail.com",
            "chuongtvde130001@fpt.edu.vn",
            "truongngochailong219@gmail.com",
        };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGoogleAuthProvider _googleAuthProvider;
        private readonly IUserFileService _profileService;
        private readonly IIdentityIntegrationEventService _identityIntegrationEventService;
        private readonly IEventBus _eventBus;

        public ExternalAuthenticationGrant(
            UserManager<ApplicationUser> userManager,
            IGoogleAuthProvider googleAuthProvider,
            IUserFileService profileService,
            IIdentityIntegrationEventService identityIntegrationEventService,
            IEventBus eventBus
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _googleAuthProvider = googleAuthProvider ?? throw new ArgumentNullException(nameof(googleAuthProvider));
            _providers = new Dictionary<ProviderType, IExternalAuthProvider>
            {
                {ProviderType.Google, _googleAuthProvider},
            };
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));

            _identityIntegrationEventService = identityIntegrationEventService ??
                                               throw new ArgumentNullException(
                                                   nameof(identityIntegrationEventService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(profileService));
        }


        private Dictionary<ProviderType, IExternalAuthProvider> _providers;

        public string GrantType => "external";


        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var provider = context.Request.Raw.Get("provider");
            if (string.IsNullOrWhiteSpace(provider))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "invalid provider");
                return;
            }


            var token = context.Request.Raw.Get("external_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "invalid external token");
                return;
            }

            var providerType = (ProviderType) Enum.Parse(typeof(ProviderType), provider, true);

            if (!Enum.IsDefined(typeof(ProviderType), providerType))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "invalid provider");
                return;
            }

            var userInfo = _providers[providerType].GetUserInfo(token);

            var requestEmail = userInfo.Value<string>("email");

            var externalId = userInfo.Value<string>("id");
            if (!string.IsNullOrWhiteSpace(externalId))
            {
                var user = await _userManager.FindByLoginAsync(provider, externalId);
                if (null != user)
                {
                    user = await _userManager.FindByIdAsync(user.Id);
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    context.Result = new GrantValidationResult(user.Id, provider, userClaims, provider, null);
                    return;
                }
            }

            context.Result = await this.ProcessAsync(userInfo, requestEmail, provider);
        }

        public async Task<GrantValidationResult> ProcessAsync(JObject userInfo, string userEmail, string provider)
        {
            var userExternalId = userInfo.Value<string>("id");

            if (string.IsNullOrWhiteSpace(userExternalId))
            {
                return new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    "could not retrieve user Id from the token provided");
            }

            var existingUser = _userManager.FindByEmailAsync(userEmail).Result;

            if (existingUser != null)
            {
                return new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    "User with specified email already exists");
            }

            var userId = Guid.NewGuid().ToString();

            // Download and save image from url
            var imgUrl = userInfo.Value<string>("picture");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(imgUrl);
            Stream imgStream = await response.Content.ReadAsStreamAsync();
            string filename = await _profileService.SaveProfileImage(userId, imgStream, null);

            var newUser = new ApplicationUser
            {
                Id = userId,
                Email = userEmail,
                UserName = new MailAddress(userEmail).User,
                Name = userInfo.Value<string>("name").Trim('"').Trim(),
                NormalizedEmail = userEmail.Trim('"').Trim(),
                NormalizedUserName = userEmail.Trim('"').Trim(),
                ProfileImage = filename
            };
            var result = _userManager.CreateAsync(newUser).Result;
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AdminEmailWhitelist.Contains(userEmail) ? "admin" : "user");
                await _userManager.AddLoginAsync(newUser, new UserLoginInfo(provider, userExternalId, provider));
                var userClaims = _userManager.GetClaimsAsync(newUser).Result;
                // Publish newUserCreated event 
                var @event = new NewUserCreatedIntegrationEvent(userId);
                await _identityIntegrationEventService.SaveEventAndIdentityContextChangesAsync(@event);
                // await _identityIntegrationEventService.PublishThroughEventBusAsync(@event);
                _eventBus.Publish(@event);
                return new GrantValidationResult(newUser.Id, provider, userClaims, provider, null);
            }

            return new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                "could not create user , please try again.");
        }
    }
}