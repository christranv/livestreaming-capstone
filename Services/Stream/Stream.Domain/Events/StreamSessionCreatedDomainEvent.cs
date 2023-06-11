using MediatR;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Domain.Events
{
    /// <summary>
    /// Event.API used when an stream is created
    /// </summary>
    public class StreamSessionCreatedDomainEvent : INotification
    {
        public string StreamSessionId { get; }
        public StreamSession Stream { get; }

        public StreamSessionCreatedDomainEvent(string streamSessionId, StreamSession stream)
        {
            StreamSessionId = streamSessionId;
            Stream = stream;
        }
    }
}