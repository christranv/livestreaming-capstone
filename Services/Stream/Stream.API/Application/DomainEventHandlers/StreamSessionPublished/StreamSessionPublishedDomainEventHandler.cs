using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Stream.API.Application.IntegrationEvents;
using Stream.API.Infrastructure.Services;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.Events;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Stream.API.Application.DomainEventHandlers.StreamSessionPublished
{
    public class StreamSessionPublishedDomainEventHandler
        : INotificationHandler<StreamSessionPublishedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IStreamerRepository _streamerRepository;
        private readonly IIdentityService _identityService;
        private readonly IStreamingIntegrationEventService _streamingIntegrationEventService;

        public StreamSessionPublishedDomainEventHandler(
            ILoggerFactory logger,
            IStreamerRepository streamerRepository,
            IIdentityService identityService,
            IStreamingIntegrationEventService streamingIntegrationEventService
        )
        {
            _streamerRepository = streamerRepository ?? throw new ArgumentNullException(nameof(streamerRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _streamingIntegrationEventService =
                streamingIntegrationEventService ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(StreamSessionPublishedDomainEvent domainEvent,
            CancellationToken cancellationToken)
        {
            var evtList = new List<IntegrationEvent>();
            if (!string.IsNullOrEmpty(domainEvent.StreamSession.SubEventId))
            {
                var subEventStreamSessionPublishedEvent = new SubEventStreamSessionPublishedIntegrationEvent(
                    domainEvent.StreamSessionId,
                    domainEvent.StreamSession.SubEventId
                );
                evtList.Add(subEventStreamSessionPublishedEvent);
            }

            var @event = new StreamSessionPublishedIntegrationEvent(domainEvent.StreamSessionId);
            evtList.Add(@event);
            await _streamingIntegrationEventService.AddAndSaveEventsAsync(evtList);
        }
    }
}