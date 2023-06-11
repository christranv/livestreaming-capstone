namespace Identity.API.Models.DTO
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public string ProfileImage { get; set; }
        public string BannerImage { get; set; }
        public string Biography { get; set; }
        public int FollowingCount { get; set; }
        public int FollowedCount { get; set; }
        // Is current user following
        public bool IsFollowing { get; set; }
    }
}