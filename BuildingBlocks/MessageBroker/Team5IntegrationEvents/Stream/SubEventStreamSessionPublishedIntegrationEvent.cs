using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream
{
    public class SubEventStreamSessionPublishedIntegrationEvent : IntegrationEvent
    {
        public string StreamSessionId { get; }
        public string SubEventId { get; }

        public SubEventStreamSessionPublishedIntegrationEvent(string streamSessionId, string subEventId)
        {
            StreamSessionId = streamSessionId;
            SubEventId = subEventId;
        }
    }
}