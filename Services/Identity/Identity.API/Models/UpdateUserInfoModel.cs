using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models
{
    public class UpdateUserInfoModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Biography { get; set; }
        [Required]
        public int Gender { get; set; }
    }
}
