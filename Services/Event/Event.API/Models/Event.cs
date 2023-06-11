using System;
using System.Collections.Generic;

namespace Event.API.Models
{
    public class Event
    {
        public string Id { get; }
        public string Name { get; }
        public string LogoImageFilePath { get; }
        public string CategoryId { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public IEnumerable<SubEvent> SubEvents { get; }

        private Event()
        {
            SubEvents = new List<SubEvent>();
        }

        public Event(string id, string name, string logoImageFilePath, string categoryId, DateTime startTime, DateTime endTime) : this()
        {
            Id = id;
            Name = name;
            LogoImageFilePath = logoImageFilePath;
            CategoryId = categoryId;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}