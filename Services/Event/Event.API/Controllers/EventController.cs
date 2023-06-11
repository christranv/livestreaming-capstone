using System;
using System.Collections.Generic;
using System.IO;
using BrunoZell.ModelBinding;
using Event.API.Application.Services.Interface;
using Event.API.Infrastructure.Services.Interface;
using Event.API.Models;
using Event.API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Event.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IEventService _eventService;
        private readonly IConfiguration _configuration;
        private readonly IIdentityService _identityService;
        public EventController(ILogger<EventController> logger, IEventService eventService, IConfiguration configuration, IIdentityService identityService)
        {
            _logger = logger;
            _eventService = eventService;
            _configuration = configuration;
            _identityService = identityService;
        }

        //Create new event
        [HttpPost]
        public IActionResult CreateEvent([ModelBinder(BinderType = typeof(JsonModelBinder))]Models.Event events, IFormFile file)
        {
            _logger.LogInformation("Create Event");
            string dbPathImage = null;
            string eventId = Guid.NewGuid().ToString();
            var folderName = Path.Combine("wwwroot");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (file.Length > 0)
            {
                using var image = Image.Load(file.OpenReadStream());
                image.Mutate(x => x.Resize(112, 112));
                var fileName = eventId + ".png";
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPathImageConfig = _configuration.GetValue<string>("EventPicApiPath");
                dbPathImage = dbPathImageConfig.Replace("[0]", fileName);
                image.SaveAsPng(fullPath);
            }
            
            var eventItem = new Models.Event(eventId,events.Name,dbPathImage,events.CategoryId,events.StartTime,events.EndTime);
            _eventService.CreateEvent(eventItem);
            return Ok(eventItem);
        }
        
        //Get All Event
        [HttpGet]
        public ActionResult<SubEventViewModel> GetAllEvents()
        {
            _logger.LogInformation("Get All Event");
            return Ok(_eventService.GetAllEvent());
        }

        //Get Event by Id
        [HttpGet]
        [Route("{id}")]
        public ActionResult<EventViewModel> GetEventById(string id)
        {
            var eventItems = _eventService.GetEventById(id);
            if (eventItems != null) return Ok(eventItems);
            _logger.LogError($"Event with id {id} not found");
            return NotFound();
        }

        //Add subEvent
        [HttpPost]
        [Route("{eventId}/subEvent")]
        public ActionResult<SubEvent> AddSubEvent(string eventId, string status, string streamSessionId, DateTime startTime)
        {
            _logger.LogInformation("Add Sub Event");
            _eventService.AddSubEvent(eventId, status, streamSessionId, startTime);
            return Ok();
        }

        //Delete subEvent
        [HttpDelete]
        [Route("subEvent/{id}")]
        public ActionResult<SubEvent> DeleteSubEvent(string id)
        {
            _logger.LogInformation("Delete Sub Event");
            _eventService.DeleteSubEvent(id);
            return Ok();
        }

        //Edit subEvent
        [HttpPut]
        [Route("subEvent/{id}")]
        public ActionResult<SubEvent> EditSubEvent(string id, string status, string streamSessionId, DateTime startTime)
        {
            _logger.LogInformation("Update Sub Event");
            _eventService.EditSubEvent(id, status, streamSessionId, startTime);
            return Ok();
        }
        
        [HttpPut]
        [Route("subEvent/{subEventId}/statusToHappening")]
        public ActionResult<SubEvent> UpdateSubEventStatusToHappening(string subEventId, string streamSessionId)
        {
            _logger.LogInformation("Update sub event status to happening");
            _eventService.SetSubEventStatusToHappeningAsync(subEventId, streamSessionId);
            return Ok();
        }

        [HttpPut]
        [Route("subEvent/{subEventId}/statusToFinished")]
        public ActionResult<SubEvent> UpdateSubEventStatusToFinish(string subEventId)
        {
            _logger.LogInformation("Update sub event status to finished");
            _eventService.SetSubEventStatusToFinishAsync(subEventId);
            return Ok();
        }

        //Search Event
        [HttpGet]
        [Route("event/search")]
        public ActionResult<EventViewModel> SearchEvent(string keyword)
        {
            _logger.LogInformation("Search Event By Name");
            var searchResultEvent = _eventService.SearchEvent(keyword);
            return Ok(searchResultEvent);
        }

        //Search Event and closest date from current date
        [HttpGet]
        [Route("event/search/closestSubEvent")]
        public ActionResult<EventViewModel> SearchEventWithClosestSubEvent(string keyword)
        {
            _logger.LogInformation("Search Event By Name");
            var searchResultEvent = _eventService.SearchEventWithClosestSubEvent(keyword); 
            return Ok(searchResultEvent);
        }

        //Get Schedule
        [HttpGet]
        [Route("schedule/{fromDate}")]
        public ActionResult<IEnumerable<ScheduleEvent>> GetScheduleEvent(DateTime fromDate, string categoryId, string type = "middle", [FromQuery]int pageSize = 7)
        {
            _logger.LogInformation("Get Schedule");
            var userId = _identityService.GetUserIdentity();
            // type = behind || middle || forward
            var scheduleList = _eventService.GetScheduleEvents(fromDate, categoryId, type, pageSize, userId);
            return Ok(scheduleList);
        }

        //Follow SubEvent
        [HttpPost]
        [Route("subEvent/{subEventId}/Follow")]
        public ActionResult<SubEventFollower> FollowSubEvent(string subEventId)
        {
            _logger.LogInformation($"Follow SubEvent with id {subEventId}");
            var userId = _identityService.GetUserIdentity();
            _eventService.FollowSubEvent(subEventId, userId);
            return Ok();
        }

        //UnFollow SubEvent
        [HttpDelete]
        [Route("subEvent/{subEventId}/UnFollow")]
        public ActionResult<SubEventFollower> UnFollowSubEvent(string subEventId)
        {
            _logger.LogInformation($"UnFollow SubEvent with id {subEventId}");
            var userId = _identityService.GetUserIdentity();
            _eventService.UnFollowSubEvent(subEventId, userId);
            return Ok();
        }

        //Get SubEvent by Id
        [HttpGet]
        [Route("subEvent/{subEventId}")]
        public ActionResult<SubEventViewModel> GetSubEventById(string subEventId)
        {
            _logger.LogInformation($"Get SubEvent id {subEventId}");
            var subEvent = _eventService.GetSubEventById(subEventId);
            return Ok(subEvent);
        }
    }
}