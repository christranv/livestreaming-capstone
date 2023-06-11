using System.Collections.Generic;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Event
{
    public class SubEventChangedToHappeningIntegrationEvent : IntegrationEvent
    {
        public string EventName { get; }
        public string StreamSessionId { get; }
        public string ThumbnailImageUrl { get; }
        public IEnumerable<string> FollowedUserGuids { get; }

        public SubEventChangedToHappeningIntegrationEvent(string eventName, string streamSessionId, string thumbnailImageUrl, IEnumerable<string> followedUserGuids)
        {
            EventName = eventName;
            StreamSessionId = streamSessionId;
            ThumbnailImageUrl = thumbnailImageUrl;
            FollowedUserGuids = followedUserGuids;
        }
    }
}