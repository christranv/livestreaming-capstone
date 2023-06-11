using System;
using System.Collections.Generic;
using Event.API.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Event.API.ViewModel
{
    public class SubEventViewModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string StreamSessionId { get; set; }
        public DateTime StartTime { get; set; }
        public IEnumerable<SubEventFollowerViewModel> SubEventFollower { get; }

        public SubEventViewModel(string id, string status, string name, string streamSessionId, DateTime startTime, IEnumerable<SubEventFollowerViewModel> subEventFollower)
        {
            Id = id;
            Status = status;
            Name = name;
            StreamSessionId = streamSessionId;
            StartTime = startTime;
            SubEventFollower = subEventFollower;
        }

        public SubEventViewModel()
        {
        }
    }
}