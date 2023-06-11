using MediatR;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.Domain.Events
{
    /// <summary>
    /// Event.API used when an stream is finished
    /// </summary>
    public class StreamSessionFinishedDomainEvent : INotification
    {
        public string StreamSessionId { get; }
        public StreamSession StreamSession { get; }

        public StreamSessionFinishedDomainEvent(string streamSessionId, StreamSession streamSession)
        {
            StreamSessionId = streamSessionId;
            StreamSession = streamSession;
        }
    }
}