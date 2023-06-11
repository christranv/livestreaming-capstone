using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stream.API.Application.IntegrationEvents;
using Stream.API.Infrastructure.Services;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.Events;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Stream.API.Application.DomainEventHandlers.StreamSessionCreated
{
    public class StreamSessionCreatedDomainEventHandler
        : INotificationHandler<StreamSessionCreatedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IStreamerRepository _streamerRepository;
        private readonly IIdentityService _identityService;
        private readonly IStreamingIntegrationEventService _streamingIntegrationEventService;
        private readonly IOptions<StreamSettings> _settings;

        public StreamSessionCreatedDomainEventHandler(
            ILoggerFactory logger,
            IStreamerRepository streamerRepository,
            IIdentityService identityService,
            IStreamingIntegrationEventService streamingIntegrationEventService,
            IOptions<StreamSettings> settings
        )
        {
            _streamerRepository = streamerRepository ?? throw new ArgumentNullException(nameof(streamerRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _streamingIntegrationEventService =
                streamingIntegrationEventService ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings;
        }

        public async Task Handle(StreamSessionCreatedDomainEvent streamSessionCreatedDomainEvent,
            CancellationToken cancellationToken)
        {
            _logger.CreateLogger<StreamSessionCreatedDomainEvent>()
                .LogTrace("New stream session created {Id}",
                    streamSessionCreatedDomainEvent.StreamSessionId);

            var streamer = _streamerRepository.Find(streamSessionCreatedDomainEvent.Stream.StreamerId);
            var streamSessionCreatedIntegrationEvent = new StreamSessionCreatedIntegrationEvent(
                streamSessionCreatedDomainEvent.StreamSessionId, streamer.IdentityGuid,
                $"{_settings.Value.SrsApiServerUrl}/{streamer.StreamKey}/source.m3u8",
                $"{_settings.Value.SrsApiServerUrl}/{streamer.StreamKey}/480.m3u8",
                $"{_settings.Value.SrsApiServerUrl}/{streamer.StreamKey}/720.m3u8"
            );

            await _streamingIntegrationEventService.AddAndSaveEventAsync(streamSessionCreatedIntegrationEvent);
        }
    }
}