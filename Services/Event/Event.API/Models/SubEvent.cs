using System;
using System.Collections.Generic;

namespace Event.API.Models
{
    public class SubEvent
    {
        public string Id { get; }
        public string Status { get; private set; }
        public Event Event { get; }
        public string StreamSessionId { get; private set; }
        public DateTime StartTime { get; }
        public IEnumerable<SubEventFollower> SubEventFollower { get; }

        private SubEvent()
        {
            SubEventFollower = new List<SubEventFollower>();
        }

        public SubEvent(string id, string status, Event @event, string streamSessionId, DateTime startTime) : this()
        {
            Id = id;
            Status = status;
            Event = @event;
            StreamSessionId = streamSessionId;
            StartTime = startTime;
        }

        public void SetStatusFinished(string status)
        {
            Status = status;
        }

        public void SetStatusHappening(string status, string streamSessionId)
        {
            Status = status;
            StreamSessionId = streamSessionId;
        }
    }
}