using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Notification.API.Infrastructure;
using Notification.API.Services.Interfaces;

namespace Notification.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            NotificationContext context)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddNotification(Models.Notification data)
        {
            _hubContext.Clients
                .Group(data.UserId)
                .SendAsync(data.ActionName.ToString(), data.Content);
            _context.Notifications.Add(data);
            _context.SaveChanges();
        }

        public void AddNotifications(IList<Models.Notification> data)
        {
            foreach (var notification in data)
            {
                _hubContext.Clients
                    .Group(notification.UserId)
                    .SendAsync(notification.ActionName.ToString(), notification.Content);
            }
            _context.Notifications.AddRange(data);
            _context.SaveChanges();
        }
    }
}