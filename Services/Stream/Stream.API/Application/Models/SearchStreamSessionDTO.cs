using System;
using System.Collections.Generic;

namespace Stream.API.Application.Models
{
    public class SearchStreamSessionDTO
    {
        public string Id { get; set; }
        public string StreamerIdentityGuid { get; set; }
        public string StreamerName { get; set; }
        public string StreamerImageUrl { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public string ThumbnailImage { get; set; }
        public List<TagDTO> Tags { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}