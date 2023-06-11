using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models
{
    public class FollowModel
    {
        [Required] public string TargetId { get; set; }
    }
}