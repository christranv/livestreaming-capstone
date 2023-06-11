using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream
{
    public class SubEventStreamSessionFinishedIntegrationEvent : IntegrationEvent
    {
        public string SubEventId { get; }

        public SubEventStreamSessionFinishedIntegrationEvent(string subEventId)
        {
            SubEventId = subEventId;
        }
    }
}