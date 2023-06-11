using System;
using System.Collections.Generic;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.API.Application.Models
{
    public class StreamStatusDTO
    {
        public string Id { get; set; }
        public string StreamerIdentityGuid { get; set; }
        public string StreamerName { get; set; }
        public string StreamerImageUrl { get; set; }
        public int ViewCount { get; set; }
        public CategoryDTO Category { get; set; }
    }
}