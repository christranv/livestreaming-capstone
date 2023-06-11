using MediatR;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Domain.Events
{
    /// <summary>
    /// Event.API used when an stream is published
    /// </summary>
    public class StreamSessionPublishedDomainEvent : INotification
    {
        public string StreamSessionId { get; }
        public StreamSession StreamSession { get; }

        public StreamSessionPublishedDomainEvent(string streamSessionId, StreamSession streamSession)
        {
            StreamSessionId = streamSessionId;
            StreamSession = streamSession;
        }
    }
}