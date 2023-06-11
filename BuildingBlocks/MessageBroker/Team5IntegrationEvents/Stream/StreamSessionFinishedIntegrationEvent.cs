using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream
{
    public class StreamSessionFinishedIntegrationEvent : IntegrationEvent
    {
        public string StreamSessionId { get; }

        public StreamSessionFinishedIntegrationEvent(string streamSessionId)
        {
            StreamSessionId = streamSessionId;
        }
    }
}