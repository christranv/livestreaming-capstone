using Newtonsoft.Json;
using Notification.API.Models.Enums;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Notification.API.Models
{
    public class Notification
    {
        public string Id { get; }
        public NotificationType Type { get; }
        public HubActionName ActionName { get; }
        public string UserId { get; }
        public string Content { get; }

        // For generate migrations
        private Notification() {}

        public Notification(NotificationType type, HubActionName actionName, string userId, object content) : this()
        {
            Type = type;
            ActionName = actionName;
            UserId = userId;
            Content = JsonConvert.SerializeObject(content);
        }
    }
}