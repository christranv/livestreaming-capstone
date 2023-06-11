using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Event.API.Application.IntegrationEvents;
using Event.API.Application.Services.Interface;
using Event.API.Infrastructure;
using Event.API.Infrastructure.Services.Interface;
using Event.API.Models;
using Event.API.ViewModel;
using Microsoft.EntityFrameworkCore;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Event;

namespace Event.API.Application.Services
{
    public class EventService : IEventService
    {
        private readonly EventContext _context;
        private readonly IMapper _mapper;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly IEventIntegrationEventService _eventIntegrationEventService;

        public EventService(EventContext context, IMapper mapper,
            IEventIntegrationEventService eventIntegrationEventService,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _context = context;
            _mapper = mapper;
            _eventIntegrationEventService = eventIntegrationEventService;
            var integrationEventLogServiceFactory1 = integrationEventLogServiceFactory ??
                                                     throw new ArgumentNullException(
                                                         nameof(integrationEventLogServiceFactory));
            _eventLogService = integrationEventLogServiceFactory1(_context.Database.GetDbConnection());
        }

        public void CreateEvent(Models.Event eventItem)
        {
            _context.Events.Add(eventItem);
            _context.SaveChanges();
        }

        public IEnumerable<EventViewModel> GetAllEvent()
        {
            var eventItems = _context.Events.ToList();

            return eventItems.Select(eventItem => new EventViewModel
                (
                    eventItem.Id,
                    eventItem.Name,
                    eventItem.LogoImageFilePath,
                    eventItem.CategoryId,
                    eventItem.StartTime,
                    eventItem.EndTime,
                    GetSubEventByEventId(eventItem.Id)
                ))
                .ToList();
        }

        public EventViewModel GetEventById(string id)
        {
            var eventItemById = _mapper.Map<EventViewModel>(_context.Events.FirstOrDefault(e => e.Id == id));
            if (eventItemById == null) throw new Exception("Not Found Event");
            var nestedEvent = new EventViewModel
            (
                eventItemById.Id,
                eventItemById.Name,
                eventItemById.LogoImageFilePath,
                eventItemById.CategoryId,
                eventItemById.StartTime,
                eventItemById.EndTime,
                GetSubEventByEventId(eventItemById.Id)
            );
            eventItemById = nestedEvent;
            return eventItemById;
        }

        IEnumerable<SubEventViewModel> GetSubEventByEventId(string eventId)
        {
            var subEventsByEventId = _context.SubEvents.Where(se => se.Event.Id == eventId).ToList();
            return subEventsByEventId.Select(subEvent => new SubEventViewModel(
                    subEvent.Id,
                    subEvent.Status,
                    subEvent.Event.Name,
                    subEvent.StreamSessionId,
                    subEvent.StartTime,
                    _mapper.Map<IEnumerable<SubEventFollowerViewModel>>(_context.SubEventFollowers
                        .Where(sef => sef.SubEvent.Id == subEvent.Id).ToList())
                ))
                .ToList();
        }

        public void AddSubEvent(string eventId, string status, string streamSessionId, DateTime startTime)
        {
            var eventItem = _context.Events.Find(eventId);
            if (eventItem == null) throw new Exception("Not Found Event");
            var subEventItem = new SubEvent(Guid.NewGuid().ToString(), status, eventItem, streamSessionId, startTime);
            _context.SubEvents.Add(subEventItem);
            _context.SaveChanges();
        }

        public void DeleteSubEvent(string id)
        {
            var subEventDelete = _context.SubEvents.Find(id);
            if (subEventDelete == null) throw new Exception("SubEvent not found!");
            _context.SubEvents.Remove(subEventDelete);
            _context.SaveChanges();
        }

        public SubEventViewModel GetSubEventById(string id)
        {
            var subEventsById = _context.SubEvents.Where(se => se.Id == id);
            var subEvents = subEventsById.Select(subEvent => new SubEventViewModel(
                subEvent.Id,
                subEvent.Status,
                subEvent.Event.Name,
                subEvent.StreamSessionId,
                subEvent.StartTime,
                _mapper.Map<IEnumerable<SubEventFollowerViewModel>>(_context.SubEventFollowers
                    .Where(sef => sef.SubEvent.Id == subEvent.Id).ToList())
            )).FirstOrDefault();
            return subEvents;
        }

        public void EditSubEvent(string subEventId, string status, string streamSessionId, DateTime startTime)
        {
            var subEventById = _context.SubEvents.FirstOrDefault(se => se.Id == subEventId);
            if (subEventById == null) throw new Exception("SubEvent not found!");
            var subEventUpdate = new SubEvent(subEventId, status, subEventById.Event, streamSessionId, startTime);
            _context.Entry(subEventById).CurrentValues.SetValues(subEventUpdate);
            _context.SaveChanges();
        }

        public async Task SetSubEventStatusToHappeningAsync(string subEventId, string streamSessionId)
        {
            var subEvent = _context.SubEvents
                .Include(se => se.Event)
                .Include(se => se.SubEventFollower)
                .FirstOrDefault(se => se.Id == subEventId);
            if (subEvent == null) throw new Exception("SubEvent not found!");

            var @event = new SubEventChangedToHappeningIntegrationEvent(subEvent.Event.Name, streamSessionId,
                subEvent.Event.LogoImageFilePath, subEvent.SubEventFollower.Select(f => f.UserId).ToList());

            // await _eventIntegrationEventService.SaveEventAndPaymentContextChangesAsync(@event);

            // Excuse in transaction
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    subEvent.SetStatusHappening("Happening", streamSessionId);
                    await _context.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(@event, _context.Database.CurrentTransaction);
                    await transaction.CommitAsync();
                }
            });
            await _eventIntegrationEventService.PublishThroughEventBusAsync(@event);
        }

        public async Task SetSubEventStatusToFinishAsync(string subEventId)
        {
            var subEvent = _context.SubEvents.FirstOrDefault(se => se.Id == subEventId);
            if (subEvent == null) throw new Exception("SubEvent not found!");
            subEvent.SetStatusFinished("Finished");
            await _context.SaveChangesAsync();
        }

        public IEnumerable<EventViewModel> SearchEventWithClosestSubEvent(string keyword)
        {
            List<DateTime> dates = null;
            var matchingEvents = _context.Events.Where(e => e.Name.Contains(keyword)).ToList();
            if (matchingEvents == null) throw new Exception("Not Found Event");
            foreach (var eventItem in matchingEvents)
            {
                dates = _context.SubEvents.Where(se => se.Event == eventItem && se.Status == "NotHappenedYet")
                    .Select(se => se.StartTime).ToList();
            }

            var closestDates = dates?.OrderBy(date => Math.Abs((date - DateTime.Now).Ticks)).Take(1).FirstOrDefault();
            return matchingEvents.Select(eventItem => new EventViewModel
                (
                    eventItem.Id,
                    eventItem.Name,
                    eventItem.LogoImageFilePath,
                    eventItem.CategoryId,
                    eventItem.StartTime,
                    eventItem.EndTime,
                    _mapper.Map<IEnumerable<SubEventViewModel>>(_context.SubEvents
                        .Where(se => se.StartTime == closestDates && se.Status == "NotHappenedYet").ToList())
                ))
                .ToList();
        }

        public IEnumerable<EventViewModel> SearchEvent(string keyword)
        {
            var resultSearchEvent = _context.Events.Where(e => e.Name.Contains(keyword)).ToList();
            if (resultSearchEvent == null) throw new Exception("Not Found Event");
            return resultSearchEvent.Select(eventItem => new EventViewModel
                (
                    eventItem.Id,
                    eventItem.Name,
                    eventItem.LogoImageFilePath,
                    eventItem.CategoryId,
                    eventItem.StartTime,
                    eventItem.EndTime,
                    GetSubEventByEventId(eventItem.Id)
                )
            ).ToList();
        }

        public IEnumerable<ScheduleEvent> GetScheduleEvents(DateTime fromDate, string categoryId, string type,
            int pageSize, string userId)
        {
            List<DateTime> dates;
            if (!string.IsNullOrEmpty(categoryId))
            {
                dates = _context.SubEvents
                    .Join(_context.Events, se => se.Event, e => e, (se, e) => new
                    {
                        se.StartTime,
                        categoryId = e.CategoryId,
                    })
                    .Where(e => e.categoryId == categoryId)
                    .Select(se => se.StartTime.Date)
                    .OrderBy(date => date)
                    .Distinct()
                    .ToList();
            }
            else
            {
                dates = _context.SubEvents
                    .Select(se => se.StartTime.Date)
                    .Distinct()
                    .ToList();
            }

            List<ScheduleEvent> scheduleList = new List<ScheduleEvent>();

            var closestDate = dates.OrderBy(date => Math.Abs((date - fromDate.Date).Ticks)).Take(1).FirstOrDefault();
            var afterClosestDate = dates.OrderBy(se => se).Where(date => date > closestDate).Take(3).ToList();
            var beforeClosestDate =
                dates.OrderByDescending(se => se).Where(date => date < closestDate).Take(3).ToList();
            beforeClosestDate.Reverse();

            switch (type)
            {
                case "middle":
                {
                    List<DateTime> listDateMiddle = new List<DateTime>();
                    listDateMiddle.AddRange(beforeClosestDate);
                    listDateMiddle.Add(closestDate);
                    listDateMiddle.AddRange(afterClosestDate);
                    scheduleList.AddRange(string.IsNullOrEmpty(categoryId)
                        ? listDateMiddle.Select(date => GetSchedule(date, userId))
                        : listDateMiddle.Select(date => GetScheduleWithCategoryId(date, categoryId, userId)));
                    break;
                }
                case "behind":
                {
                    var datePoint = beforeClosestDate[0];
                    var listDateBehind = dates.Where(date => date < datePoint).OrderByDescending(date => date)
                        .Take(pageSize).ToList();
                    listDateBehind.Reverse();

                    scheduleList.AddRange(string.IsNullOrEmpty(categoryId)
                        ? listDateBehind.Select(date => GetSchedule(date, userId))
                        : listDateBehind.Select(date => GetScheduleWithCategoryId(date, categoryId, userId)));
                    break;
                }
                case "forward":
                {
                    var datePoint = afterClosestDate[2];
                    var listDateForward =
                        dates.Where(date => date > datePoint).OrderBy(se => se).Take(pageSize).ToList();

                    scheduleList.AddRange(string.IsNullOrEmpty(categoryId)
                        ? listDateForward.Select(date => GetSchedule(date, userId))
                        : listDateForward.Select(date => GetScheduleWithCategoryId(date, categoryId, userId)));
                    break;
                }
                default:
                {
                    throw new Exception("Invalid type");
                }
            }

            return scheduleList;
        }

        ScheduleEvent GetSchedule(DateTime date, string userId)
        {
            var subEvents = _context.SubEvents.Where(se => se.StartTime.Date == date).ToList();
            var schedule = new ScheduleEvent(
                date,
                subEvents.Join(_context.Events, se => se.Event.Id, e => e.Id, (se, e) => new
                {
                    scheduleId = se.Id,
                    scheduleStatus = se.Status,
                    se.StartTime,
                    se.StreamSessionId,
                    eventId = se.Event.Id,
                    eventTitle = e.Name,
                    eventLogo = e.LogoImageFilePath,
                    e.CategoryId,
                    isFollowing =
                        _context.SubEventFollowers.Any(sef => sef.UserId == userId && sef.SubEvent.Id == se.Id)
                })
            );
            return schedule;
        }

        ScheduleEvent GetScheduleWithCategoryId(DateTime date, string categoryId, string userId)
        {
            var schedule = new ScheduleEvent(
                date,
                _context.SubEvents
                    .Join(_context.Events, se => se.Event.Id, e => e.Id, (se, e) => new
                    {
                        scheduleId = se.Id,
                        scheduleStatus = se.Status,
                        se.StartTime,
                        se.StreamSessionId,
                        EventId = se.Event.Id,
                        eventTitle = e.Name,
                        eventLogo = e.LogoImageFilePath,
                        e.CategoryId,
                        isFollowing =
                            _context.SubEventFollowers.Any(sef => sef.UserId == userId && sef.SubEvent.Id == se.Id)
                    })
                    .Where(se => se.StartTime.Date == date)
                    .Where(se => se.CategoryId == categoryId)
                    .ToList()
            );
            return schedule;
        }

        public void FollowSubEvent(string subEventId, string userId)
        {
            var subEvent = _context.SubEvents.Find(subEventId);
            if (subEvent == null) throw new Exception("Not Found SubEvent");
            var subEventFollower = new SubEventFollower(Guid.NewGuid().ToString(), userId, subEvent);
            _context.SubEventFollowers.Add(subEventFollower);
            _context.SaveChanges();
        }

        public void UnFollowSubEvent(string subEventId, string userId)
        {
            var subEventFollower =
                _context.SubEventFollowers.FirstOrDefault(sef => sef.SubEvent.Id == subEventId && sef.UserId == userId);
            if (subEventFollower == null) throw new Exception("Not Found SubEventFollower");
            _context.Remove(subEventFollower);
            _context.SaveChanges();
        }
    }
}