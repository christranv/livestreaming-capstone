namespace Identity.API.Models.DTO
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUserSearchDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string Biography { get; set; }
        public int FollowedCount { get; set; }
    }
}