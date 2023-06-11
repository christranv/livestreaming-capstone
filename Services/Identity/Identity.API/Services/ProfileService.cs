using Identity.API.Services.Interfaces;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Identity.API.Infrastructure;
using Identity.API.Models;
using Identity.API.Models.DTO;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Services
{
    public class ProfileService : IProfileService, IExtendedProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ProfileService> _logger;
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ProfileService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ILogger<ProfileService> logger, ApplicationDbContext appDbContext, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            // var user = await _userManager.FindByIdAsync(subjectId);
            var user = _appDbContext.Users
                .Where(u => u.Id == subjectId)
                .Include(u => u.FollowingUsers)
                .Include(u => u.FollowedByUsers)
                .FirstOrDefault();

            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = GetClaimsFromUser(user);

            // Claims roles of user 
            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, roleName));
                    if (_roleManager.SupportsRoleClaims)
                    {
                        IdentityRole role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            claims.AddRange(await _roleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }

            context.IssuedClaims = claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var securityStamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value)
                        .SingleOrDefault();
                    if (securityStamp != null)
                    {
                        var dbSecurityStamp = await _userManager.GetSecurityStampAsync(user);
                        if (dbSecurityStamp != securityStamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        public ApplicationUserDto GetUserById(string currentUserId, string targetUserId)
        {
            var user = _appDbContext.Users
                .Where(u => u.Id == targetUserId)
                .Include(u => u.FollowingUsers)
                .Include(u => u.FollowedByUsers)
                .FirstOrDefault();
            if (user == null) return null;
            var userDto = _mapper.Map<ApplicationUserDto>(user);
            // Check IsFollowing target user
            userDto.IsFollowing = user.FollowedByUsers.Any(r => r.UserAId == currentUserId);
            return userDto;
        }

        public IDictionary<string, FollowerInfoDto> GetFollowingIds(string userId, int pageSize, int pageIndex)
        {
            var user = _appDbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.FollowingUsers)
                .FirstOrDefault();
            var followingUserIds = user?.FollowingUsers
                .Select(u => u.UserBId)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToList();
            var follower = _appDbContext.Users
                .Where(u => followingUserIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => new FollowerInfoDto(u.Name, u.ProfileImage));
            return follower;
        }
        
        public IEnumerable<ApplicationUserSearchDto> SearchUserByName(string keyword, int pageSize, int pageIndex)
        {
            var users = _appDbContext.Users
                .Where(u => keyword.IsNullOrEmpty() || u.Name.Contains(keyword))
                .Include(u => u.FollowingUsers)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToList();
            var userDtos = users.Select(u => _mapper.Map<ApplicationUserSearchDto>(u));
            return userDtos;
        }

        public bool UpdateInfo(string userId, UpdateUserInfoModel model)
        {
            var user = _appDbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null) throw new Exception("User not existed!");

            user.Name = model.DisplayName;
            user.Biography = model.Biography;
            user.Gender = model.Gender;

            _appDbContext.Update(user);
            _appDbContext.SaveChanges();
            return true;
        }

        public bool FollowUser(string currentUserId, string targetUserId)
        {
            if (currentUserId == targetUserId) throw new Exception("You can not follow yourself!");
            var currentUser = _appDbContext.Users
                .Where(user => user.Id == currentUserId)
                .Include(u => u.FollowingUsers)
                .FirstOrDefault();
            var targetUser = _appDbContext.Users.FirstOrDefault(user => user.Id == targetUserId);

            if (currentUser == null || targetUser == null) throw new Exception("User not existed!");

            if (currentUser.FollowingUsers.Any(r => r.UserAId == currentUser.Id && r.UserBId == targetUser.Id))
                return true;

            currentUser.FollowingUsers.Add(new ApplicationUserRelation {UserA = currentUser, UserB = targetUser});
            _appDbContext.SaveChanges();
            _logger.LogInformation("User {UserA} following user {UserB}", currentUserId, targetUserId);

            return true;
        }

        public bool UnfollowUser(string currentUserId, string targetUserId)
        {
            var currentUser = _appDbContext.Users
                .Where(user => user.Id == currentUserId)
                .Include(u => u.FollowingUsers)
                .FirstOrDefault();
            var targetUser = _appDbContext.Users.FirstOrDefault(user => user.Id == targetUserId);

            if (currentUser == null || targetUser == null) throw Exception("User not existed!");

            var relation = currentUser.FollowingUsers
                .FirstOrDefault(r => r.UserA == currentUser && r.UserB == targetUser);

            if (relation == null)
                throw new Exception($"{currentUserId} do not follow user {targetUserId}");

            currentUser.FollowingUsers.Remove(relation);
            _appDbContext.SaveChanges();
            _logger.LogInformation("User {UserA} unfollow user {UserB}", currentUserId, targetUserId);

            return true;
        }

        private List<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
                claims.Add(new Claim("name", user.Name));

            claims.Add(new Claim("gender", user.Gender.ToString()));
            claims.Add(new Claim("biography", user.Biography));

            claims.Add(new Claim("profile_image", user.ProfileImage));
            claims.Add(new Claim("banner_image", user.BannerImage));

            claims.Add(new Claim("followingCount", user.FollowingUsers.Count.ToString()));
            claims.Add(new Claim("followedCount", user.FollowedByUsers.Count.ToString()));

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false",
                        ClaimValueTypes.Boolean)
                });
            }

            return claims;
        }

        private Exception Exception(string v)
        {
            throw new NotImplementedException();
        }
    }
}