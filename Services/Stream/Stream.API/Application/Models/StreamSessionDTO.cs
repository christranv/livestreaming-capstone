using System;
using System.Collections.Generic;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.API.Application.Models
{
    public class StreamSessionDTO
    {
        public string Id { get; set; }
        public string StreamerIdentityGuid { get; set; }
        public string StreamerName { get; set; }
        public string StreamerImageUrl { get; set; }
        public string Title { get; set; }
        public string Announcement { get; set; }
        public int ViewCount { get; set; }
        public string StreamUrlSource { get; set; }
        public string StreamUrl720P { get; set; }
        public string StreamUrl480P { get; set; }
        public string ThumbnailImage { get; set; }
        public Language Language { get; set; }
        public CategoryDTO Category { get; set; }
        public List<TagDTO> Tags { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}