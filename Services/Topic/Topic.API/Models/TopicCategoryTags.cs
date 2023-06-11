using Newtonsoft.Json;

namespace Topic.API.Models
{
    public class TopicCategoryTags
    {
        [JsonIgnore]
        public string CategoryId { get; set; }
        public TopicCategory Category { get; set; }

        [JsonIgnore]
        public string TagId { get; set; }
        public TopicTag Tag { get; set; }
    }
}
