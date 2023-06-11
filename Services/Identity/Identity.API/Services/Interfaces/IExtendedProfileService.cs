using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Models.DTO;

namespace Identity.API.Services.Interfaces
{
    public interface IExtendedProfileService
    {
        public bool FollowUser(string currentUserId, string targetUserId);
        public bool UnfollowUser(string currentUserId, string targetUserId);
        public bool UpdateInfo(string userId, UpdateUserInfoModel model);
        public ApplicationUserDto GetUserById(string currentUserId, string targetUserId);
        public IDictionary<string, FollowerInfoDto> GetFollowingIds(string userId, int pageSize, int pageIndex);
        public IEnumerable<ApplicationUserSearchDto> SearchUserByName(string keyword, int pageSize, int pageIndex);
    }
}