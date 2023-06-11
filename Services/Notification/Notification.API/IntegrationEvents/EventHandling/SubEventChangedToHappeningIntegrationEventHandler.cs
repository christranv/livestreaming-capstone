using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notification.API.Models;
using Notification.API.Models.Enums;
using Notification.API.Services.Interfaces;
using Serilog.Context;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Event;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Notification.API.IntegrationEvents.EventHandling
{
    public class
        SubEventChangedToHappeningIntegrationEventHandler : IIntegrationEventHandler<
            SubEventChangedToHappeningIntegrationEvent>
    {
        private readonly ILogger<SubEventChangedToHappeningIntegrationEventHandler> _logger;
        private readonly INotificationService _notificationService;

        public SubEventChangedToHappeningIntegrationEventHandler(
            INotificationService notificationService,
            ILogger<SubEventChangedToHappeningIntegrationEventHandler> logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(SubEventChangedToHappeningIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);
                var notifications = @event.FollowedUserGuids
                    .Select(userId => 
                        new Models.Notification(NotificationType.User, HubActionName.NewSubEventHappening, userId,
                        new SubEventContentModel(@event.EventName, @event.StreamSessionId, @event.ThumbnailImageUrl))).ToList();
                _notificationService.AddNotifications(notifications);
            }
        }
    }
}