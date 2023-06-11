using System;
using System.Collections.Generic;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.Events;
using Stream.Domain.Exceptions;
using Stream.Domain.Seedwork;
using Stream.Domain.SeedWork;

namespace Stream.Domain.AggregatesModel.StreamSessionAggregate
{
    public class StreamSession
        : Entity, IAggregateRoot
    {
        public string StreamerId { get; }
        public Streamer Streamer { get; private set; }
        public string StreamerName { get; private set; }
        public string StreamerImageUrl { get; private set; }
        public StreamSessionStatus StreamSessionStatus { get; private set; }
        public int StreamSessionStatusId { get; private set; }
        public string SubEventId { get; private set; }

        public string Title { get; private set; }
        public string Announcement { get; private set; }
        public int ViewCount { get; private set; }
        public string ThumbnailImage { get; private set; }
        public Language Language { get; private set; }
        public int LanguageId { get; private set; }
        public StreamSessionCategory Category { get; private set; }
        public List<StreamSessionTag> Tags { get; }
        public DateTime CreatedDate { get; }

        private StreamSession()
        {
            Tags = new List<StreamSessionTag>();
            ViewCount = 0;
            CreatedDate = DateTime.UtcNow;
            StreamSessionStatusId = StreamSessionStatus.Created.Id;
            LanguageId = Language.Eng.Id; // Default value
        }

        public StreamSession(string streamerId) : this()
        {
            StreamerId = streamerId;

            // Add the StreamSessionStartedDomainEvent to the domain events collection 
            // to be raised/dispatched when committing changes into the Database [ After DbContext.SaveChanges() ]
            AddStreamSessionCreatedDomainEvent();
        }

        public void PublishStreamSession(string streamerName, string streamerImageUrl, string streamingKey,
            string title, string announcement, int languageId,
            StreamSessionCategory category,
            IEnumerable<StreamSessionTag> tags, string subEventId = null)
        {
            if (StreamSessionStatusId != StreamSessionStatus.Created.Id)
                StatusChangeException(StreamSessionStatus.Published);
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(announcement))
                throw new StreamDomainException($"Title and Announcement must not be empty");

            StreamerName = streamerName;
            StreamerImageUrl = streamerImageUrl;
            Title = title;
            Announcement = announcement;
            LanguageId = languageId;
            Category = category;
            Tags.AddRange(tags);
            StreamSessionStatusId = StreamSessionStatus.Published.Id;
            SubEventId = subEventId;
            ThumbnailImage = $"{streamingKey}-001.png";

            AddDomainEvent(new StreamSessionPublishedDomainEvent(base.Id, this));
        }

        public void SetFinishedStatus()
        {
            if (StreamSessionStatusId != StreamSessionStatus.Created.Id &&
                StreamSessionStatusId != StreamSessionStatus.Published.Id)
                StatusChangeException(StreamSessionStatus.Finished);
            AddDomainEvent(new StreamSessionFinishedDomainEvent(base.Id, this));
            StreamSessionStatusId = StreamSessionStatus.Finished.Id;
        }

        public void SetBannedStatus()
        {
            if (StreamSessionStatusId != StreamSessionStatus.Published.Id)
                StatusChangeException(StreamSessionStatus.Banned);
            AddDomainEvent(new StreamSessionBannedDomainEvent(base.Id, this));
            StreamSessionStatusId = StreamSessionStatus.Banned.Id;
        }
        
        public void UpdateViewCount(int viewCount)
        {
            ViewCount = viewCount;
        }

        private void AddStreamSessionCreatedDomainEvent()
        {
            var orderStartedDomainEvent = new StreamSessionCreatedDomainEvent(base.Id, this);
            AddDomainEvent(orderStartedDomainEvent);
        }

        private void StatusChangeException(Enumeration streamStatusToChange)
        {
            throw new StreamDomainException(
                $"Is not possible to change the stream status from {StreamSessionStatus.From(StreamSessionStatusId)} to {streamStatusToChange.Name}.");
        }
    }
}