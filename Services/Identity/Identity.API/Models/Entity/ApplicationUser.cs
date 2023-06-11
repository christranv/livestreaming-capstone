using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public int Gender { get; set; }
        public String ProfileImage { get; set; }
        public String BannerImage { get; set; }
        public String Biography { get; set; }
        public ICollection<ApplicationUserRelation> FollowingUsers { get; } = new List<ApplicationUserRelation>();
        public ICollection<ApplicationUserRelation> FollowedByUsers { get; } = new List<ApplicationUserRelation>();
    }
}
