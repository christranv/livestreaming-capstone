using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream
{
    public class StreamSessionCreatedIntegrationEvent : IntegrationEvent
    {
        public string StreamSessionId { get; }
        public string StreamerIdentityGuid { get; }
        public string StreamUrlSource { get; }
        public string StreamUrl480p { get; }
        public string StreamUrl720p { get; }

        public StreamSessionCreatedIntegrationEvent(string streamSessionId, string streamerIdentityGuid, string streamUrlSource, string streamUrl480P, string streamUrl720P)
        {
            StreamSessionId = streamSessionId;
            StreamerIdentityGuid = streamerIdentityGuid;
            StreamUrlSource = streamUrlSource;
            StreamUrl480p = streamUrl480P;
            StreamUrl720p = streamUrl720P;
        }
    }
}