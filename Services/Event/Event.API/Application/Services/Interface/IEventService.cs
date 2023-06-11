#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Event.API.ViewModel;

namespace Event.API.Application.Services.Interface
{
    public interface IEventService
    {
        //Admin API
        void CreateEvent(Models.Event eventItem);
        IEnumerable<EventViewModel> GetAllEvent();
        void AddSubEvent(string eventId, string status, string streamSessionId, DateTime startTime);
        EventViewModel GetEventById(string id);
        void DeleteSubEvent(string id);
        SubEventViewModel? GetSubEventById(string id);
        IEnumerable<EventViewModel> SearchEvent(string keyword);
        IEnumerable<EventViewModel> SearchEventWithClosestSubEvent(string keyword);
        void EditSubEvent(string subEventId, string status, string streamSessionId, DateTime startTime);
        Task SetSubEventStatusToHappeningAsync(string subEventId, string streamSessionId);
        Task SetSubEventStatusToFinishAsync(string subEventId);
        IEnumerable<ScheduleEvent> GetScheduleEvents(DateTime fromDate, string categoryId, string type, int pageSize, string userId);
        void FollowSubEvent(string subEventId, string userId);
        void UnFollowSubEvent(string subEventId, string userId);
    }
}