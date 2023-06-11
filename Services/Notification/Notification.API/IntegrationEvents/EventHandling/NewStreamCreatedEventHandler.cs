using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notification.API.Models.Enums;
using Notification.API.Services.Interfaces;
using Serilog.Context;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Notification.API.IntegrationEvents.EventHandling
{
    public class NewStreamCreatedEventHandler : IIntegrationEventHandler<StreamSessionCreatedIntegrationEvent>
    {
        private readonly ILogger<NewStreamCreatedEventHandler> _logger;
        private readonly INotificationService _notificationService;

        public NewStreamCreatedEventHandler(
            INotificationService notificationService,
            ILogger<NewStreamCreatedEventHandler> logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(StreamSessionCreatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);
                var data = new Models.Notification(NotificationType.System, HubActionName.NewStreamConnected, @event.StreamerIdentityGuid, @event);
                _notificationService.AddNotification(data);
            }
        }
    }
}