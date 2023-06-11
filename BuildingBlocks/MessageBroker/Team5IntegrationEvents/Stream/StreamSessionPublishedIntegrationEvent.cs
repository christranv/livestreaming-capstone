using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream
{
    public class StreamSessionPublishedIntegrationEvent : IntegrationEvent
    {
        public string StreamSessionId { get; }

        public StreamSessionPublishedIntegrationEvent(string streamSessionId)
        {
            StreamSessionId = streamSessionId;
        }
    }
}
