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

namespace Stream.API.Application.DomainEventHandlers.StreamSessionFinished
{
    public class StreamSessionFinishedDomainEventHandler
        : INotificationHandler<StreamSessionFinishedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IStreamerRepository _streamerRepository;
        private readonly IIdentityService _identityService;
        private readonly IStreamingIntegrationEventService _streamingIntegrationEventService;

        public StreamSessionFinishedDomainEventHandler(
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

        public async Task Handle(StreamSessionFinishedDomainEvent domainEvent,
            CancellationToken cancellationToken)
        {
            var evtList = new List<IntegrationEvent>();
            if (!string.IsNullOrEmpty(domainEvent.StreamSession.SubEventId))
            {
                var subEventStreamSessionPublishedEvent = new SubEventStreamSessionFinishedIntegrationEvent(
                    domainEvent.StreamSession.SubEventId
                );
                evtList.Add(subEventStreamSessionPublishedEvent);
            }

            var @event = new StreamSessionFinishedIntegrationEvent(domainEvent.StreamSessionId);
            evtList.Add(@event);
            await _streamingIntegrationEventService.AddAndSaveEventsAsync(evtList);
        }
    }
}