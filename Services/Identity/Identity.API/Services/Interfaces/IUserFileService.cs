using System.IO;
using System.Threading.Tasks;

namespace Identity.API.Services.Interfaces
{
    public interface IUserFileService
    {
        public Task<string> UpdateProfileImage(string userId, Stream imgStream);
        public Task<string> SaveProfileImage(string userId, Stream imgStream, string oldFileName);
        public Task<string> UpdateBannerImage(string userId, Stream imgStream);
        public Task<string> SaveBannerImage(string userId, Stream imgStream, string oldFileName);
    }
}
