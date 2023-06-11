using Newtonsoft.Json;

namespace Identity.API.Models
{
    public class ApplicationUserRelation
    {
        /// <summary>
        /// Follow user
        /// </summary>
        [JsonIgnore]
        public string UserAId { get; set; }
        public ApplicationUser UserA { get; set; }

        /// <summary>
        /// Followed user
        /// </summary>
        [JsonIgnore]
        public string UserBId { get; set; }
        public ApplicationUser UserB { get; set; }
    }
}
