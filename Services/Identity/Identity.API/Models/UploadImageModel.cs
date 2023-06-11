using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Identity.API.Models
{
    public class UploadImageModel
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
