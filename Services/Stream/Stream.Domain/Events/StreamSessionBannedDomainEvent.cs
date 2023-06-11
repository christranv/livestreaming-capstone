using MediatR;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Domain.Events
{
    /// <summary>
    /// Event.API used when an stream is banned
    /// </summary>
    public class StreamSessionBannedDomainEvent : INotification
    {
        public string StreamSessionId { get; }
        public StreamSession Stream { get; }

        public StreamSessionBannedDomainEvent(string streamSessionId, StreamSession stream)
        {
            StreamSessionId = streamSessionId;
            Stream = stream;
        }
    }
}