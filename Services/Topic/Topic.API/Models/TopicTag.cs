using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Topic.API.Models
{
    public class TopicTag
    {
        public string Id { get; }
        public string Name { get; }
        [JsonIgnore]
        public ICollection<TopicCategoryTags> CategoryTags { get; } = new List<TopicCategoryTags>();

        public TopicTag(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
