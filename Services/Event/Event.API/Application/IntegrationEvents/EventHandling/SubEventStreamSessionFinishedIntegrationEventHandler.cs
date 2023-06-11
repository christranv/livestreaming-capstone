using System;
using System.Threading.Tasks;
using Event.API.Application.Services.Interface;
using Event.API.Infrastructure.Services.Interface;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Event.API.Application.IntegrationEvents.EventHandling
{
    public class
        SubEventStreamSessionFinishedIntegrationEventHandler : IIntegrationEventHandler<
            SubEventStreamSessionFinishedIntegrationEvent>
    {
        private readonly IEventService _eventService;
        private readonly ILogger<SubEventStreamSessionFinishedIntegrationEventHandler> _logger;

        public SubEventStreamSessionFinishedIntegrationEventHandler(
            IEventService eventService,
            ILogger<SubEventStreamSessionFinishedIntegrationEventHandler> logger)
        {
            _eventService = eventService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SubEventStreamSessionFinishedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);

                await _eventService.SetSubEventStatusToFinishAsync(@event.SubEventId);
            }
        }
    }
}