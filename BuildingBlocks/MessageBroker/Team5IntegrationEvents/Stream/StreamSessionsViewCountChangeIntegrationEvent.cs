using System.Collections.Generic;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream
{
    public class StreamSessionsViewCountChangeIntegrationEvent : IntegrationEvent
    {
        public Dictionary<string, int> ViewCountPerStreamSession;

        public StreamSessionsViewCountChangeIntegrationEvent(Dictionary<string, int> viewCountPerStreamSession)
        {
            ViewCountPerStreamSession = viewCountPerStreamSession;
        }
    }
}