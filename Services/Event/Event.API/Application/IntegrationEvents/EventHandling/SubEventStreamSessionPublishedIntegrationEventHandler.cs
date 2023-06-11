using System.Threading.Tasks;
using Event.API.Application.Services.Interface;
using Event.API.Infrastructure.Services.Interface;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Event.API.Application.IntegrationEvents.EventHandling
{
    public class SubEventStreamSessionPublishedIntegrationEventHandler : IIntegrationEventHandler<SubEventStreamSessionPublishedIntegrationEvent>
    {
        private readonly IEventService _eventService;
        private readonly ILogger<SubEventStreamSessionPublishedIntegrationEventHandler> _logger;
        
        public SubEventStreamSessionPublishedIntegrationEventHandler(
            IEventService eventService,
            ILogger<SubEventStreamSessionPublishedIntegrationEventHandler> logger
            )
        {
            _eventService = eventService;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SubEventStreamSessionPublishedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);
                
                await _eventService.SetSubEventStatusToHappeningAsync(@event.SubEventId, @event.StreamSessionId);
            }
        }
    }
}