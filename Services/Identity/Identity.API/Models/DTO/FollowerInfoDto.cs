namespace Identity.API.Models.DTO
{
    public class FollowerInfoDto
    {
        public string Name { get; }
        public string ProfileImage { get; }

        public FollowerInfoDto(string name, string profileImage)
        {
            Name = name;
            ProfileImage = profileImage;
        }
    }
}