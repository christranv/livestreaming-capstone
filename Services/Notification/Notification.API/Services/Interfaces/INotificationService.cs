using System.Collections.Generic;

namespace Notification.API.Services.Interfaces
{
    public interface INotificationService
    {
        public void AddNotification(Models.Notification data);
        public void AddNotifications(IList<Models.Notification> data);
    }
}