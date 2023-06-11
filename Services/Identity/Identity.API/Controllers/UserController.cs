using System.Net;
using Identity.API.Models;
using Identity.API.Models.DTO;
using Identity.API.Services.Interfaces;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    public class UserController : Controller
    {
        private readonly IUserFileService _profileService;
        private readonly IIdentityService _identityService;
        private readonly IExtendedProfileService _extendedProfileService;

        public UserController(IUserFileService profileService, IIdentityService identityService,
            IExtendedProfileService extendedProfileService)
        {
            _profileService = profileService;
            _identityService = identityService;
            _extendedProfileService = extendedProfileService;
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public ActionResult<ApplicationUserDto> GetUserInfo(string id)
        {
            var user = _extendedProfileService.GetUserById(_identityService.GetUserIdentity(), id);
            return user == null ? NotFound() : Ok(user);
        }

        [Route("search")]
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public ActionResult<ApplicationUserDto> SearchUserByName(string keyword = "", int pageSize = 2,
            int pageIndex = 0)
        {
            var users = _extendedProfileService.SearchUserByName(keyword, pageSize, pageIndex);
            return Ok(users);
        }

        [Route("{id}/following")]
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public ActionResult<ApplicationUserDto> GetFollowingUserIds([FromQuery] int pageSize = 5,
            [FromQuery] int pageIndex = 0)
        {
            var followingIds = _extendedProfileService.GetFollowingIds(_identityService.GetUserIdentity(), pageSize,
                pageIndex);
            return followingIds == null ? NotFound() : Ok(followingIds);
        }

        [Route("update-info")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public ActionResult UpdateUserInfo(UpdateUserInfoModel model)
        {
            _extendedProfileService.UpdateInfo(_identityService.GetUserIdentity(), model);
            return Ok();
        }

        [Route("follow")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public ActionResult FollowUser(FollowModel model)
        {
            _extendedProfileService.FollowUser(_identityService.GetUserIdentity(), model.TargetId);
            return Ok();
        }

        [Route("unfollow")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public ActionResult UnfollowUser(FollowModel model)
        {
            _extendedProfileService.UnfollowUser(_identityService.GetUserIdentity(), model.TargetId);
            return Ok();
        }

        [Route("profile-image")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public ActionResult<string> UploadProfileImage([FromForm] UploadImageModel file)
        {
            var imagePath =
                _profileService.UpdateProfileImage(_identityService.GetUserIdentity(),
                    file.Image.OpenReadStream());
            return Ok($"{imagePath}");
        }

        [Route("banner-image")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public ActionResult<string> UploadBannerImage([FromForm] UploadImageModel file)
        {
            var imagePath =
                _profileService.UpdateBannerImage(_identityService.GetUserIdentity(),
                    file.Image.OpenReadStream());
            return Ok($"{imagePath}");
        }
    }
}