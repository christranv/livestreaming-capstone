using System;
using System.IO;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Identity.API.Services
{
    public class UserFileService : IUserFileService
    {
        private const string PROFILE_IMAGE_EXTERNAL_PART = "200x200.png";
        private const string BANNER_IMAGE_EXTERNAL_PART = "1020x570.png";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserFileService> _logger;
        private readonly IOptions<AppSettings> _appSettings;

        public UserFileService(UserManager<ApplicationUser> userManager, ILogger<UserFileService> logger,
            IOptions<AppSettings> settings)
        {
            _userManager = userManager;
            _logger = logger;
            _appSettings = settings;
        }

        /// <summary>
        /// Format and save profile image
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imgStream"></param>
        /// <returns></returns>
        /// <exception>
        ///     <cref>???</cref>
        /// </exception>
        public async Task<string> UpdateProfileImage(string userId, Stream imgStream)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new NotImplementedException("User not existed!");
            _logger.LogInformation("Updating profile image");
            var imgUrl = await SaveProfileImage(userId, imgStream, user.ProfileImage);
            user.ProfileImage = imgUrl;
            await _userManager.UpdateAsync(user);
            return imgUrl;
        }

        public async Task<string> SaveProfileImage(string userId, Stream imgStream, string oldFileName = "")
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"
                , $"images/profile/");
            // Delete old file
            if (!string.IsNullOrEmpty(oldFileName))
                File.Delete(Path.Combine(directoryPath,
                        oldFileName.Substring(_appSettings.Value.UserPicApiPath.Replace("[0]", "").Length)));
            _logger.LogInformation("Saving profile image");
            // Resize image
            using var image = Image.Load(imgStream);
            image.Mutate(x => x.Resize(200, 200));
            // Save image
            var fileName = $"{userId}-{DateTimeOffset.Now.ToUnixTimeSeconds()}-{PROFILE_IMAGE_EXTERNAL_PART}";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            await image.SaveAsPngAsync(Path.Combine(directoryPath, fileName));
            return _appSettings.Value.UserPicApiPath.Replace("[0]", fileName);
        }

        /// <summary>
        /// Format and save banner image
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imgStream"></param>
        /// <returns></returns>
        /// <exception>
        ///     <cref>???</cref>
        /// </exception>
        public async Task<string> UpdateBannerImage(string userId, Stream imgStream)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new NotImplementedException("User not existed!");
            _logger.LogInformation("Updating banner image");
            var imgUrl = await SaveBannerImage(userId, imgStream, user.BannerImage);
            user.BannerImage = imgUrl;
            await _userManager.UpdateAsync(user);
            return imgUrl;
        }

        public async Task<string> SaveBannerImage(string userId, Stream imgStream, string oldFileName = "")
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"
                , $"images/banner/");
            // Delete old file
            if (!string.IsNullOrEmpty(oldFileName))
                File.Delete(Path.Combine(directoryPath,
                        oldFileName.Substring(_appSettings.Value.BannerPicApiPath.Replace("[0]", "").Length)));
            _logger.LogInformation("Saving banner image");
            // Resize image
            using var image = Image.Load(imgStream);
            image.Mutate(x => x.Resize(1020, 570));
            // Save image
            var fileName = $"{userId}-{DateTimeOffset.Now.ToUnixTimeSeconds()}-{BANNER_IMAGE_EXTERNAL_PART}";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            await image.SaveAsPngAsync(Path.Combine(directoryPath, fileName));
            return _appSettings.Value.BannerPicApiPath.Replace("[0]", fileName);
        }
    }
}