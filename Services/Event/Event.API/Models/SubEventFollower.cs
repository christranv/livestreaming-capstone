using System;

namespace Event.API.Models
{
    public class SubEventFollower
    {
        public string Id { get; }
        public string UserId { get; }
        public SubEvent SubEvent { get; }
        public DateTime CreateDate { get; }

        public SubEventFollower()
        {
            CreateDate = DateTime.UtcNow;
        }

        public SubEventFollower(string id, string userId, SubEvent subEvent) : this()
        {
            Id = id;
            UserId = userId;
            SubEvent = subEvent;
        }
    }
}