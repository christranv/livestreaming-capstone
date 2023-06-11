using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.IntegrationEvents;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Identity;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityIntegrationEventService _identityIntegrationEventService;
        private readonly IEventBus _eventBus;

        public AccountController(UserManager<ApplicationUser> userManager,
            IIdentityIntegrationEventService identityIdentityIntegrationEventService, IEventBus eventBus)
        {
            _userManager = userManager;
            _identityIntegrationEventService = identityIdentityIntegrationEventService;
            _eventBus = eventBus;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.User.Name,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Errors.Any())
                {
                    throw new Exception(result.Errors.First().Description);
                }

                await _userManager.AddToRoleAsync(user, "admin");
                var @event = new NewUserCreatedIntegrationEvent(user.Id);
                await _identityIntegrationEventService.SaveEventAndIdentityContextChangesAsync(@event);
                // await _identityIntegrationEventService.PublishThroughEventBusAsync(@event);
                _eventBus.Publish(@event);
            }

            return Ok();
        }
    }
}