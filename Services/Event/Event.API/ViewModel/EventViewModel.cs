using System;
using System.Collections.Generic;
using Event.API.Models;

namespace Event.API.ViewModel
{
    public class EventViewModel
    {
        public string Id { get; }
        public string Name { get; }
        public string LogoImageFilePath { get; }
        public string CategoryId { get; }
        public DateTime StartTime { get;  }
        public DateTime EndTime { get; }
        public IEnumerable<SubEventViewModel> SubEvents { get; }

        public EventViewModel(string id, string name, string logoImageFilePath, string categoryId, DateTime startTime, DateTime endTime, IEnumerable<SubEventViewModel> subEvents)
        {
            Id = id;
            Name = name;
            LogoImageFilePath = logoImageFilePath;
            CategoryId = categoryId;
            StartTime = startTime;
            EndTime = endTime;
            SubEvents = subEvents;
        }
    }
}
