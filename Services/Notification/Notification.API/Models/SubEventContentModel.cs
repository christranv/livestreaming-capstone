namespace Notification.API.Models
{
    public class SubEventContentModel
    {
        public string EventName { get; }
        public string StreamSessionId { get; }
        public string ThumbnailImageUrl { get; }

        public SubEventContentModel(string eventName, string streamSessionId, string thumbnailImageUrl)
        {
            EventName = eventName;
            StreamSessionId = streamSessionId;
            ThumbnailImageUrl = thumbnailImageUrl;
        }
    }
}