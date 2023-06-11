using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Notification.API.Services.Interfaces;

namespace Notification.API
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IIdentityService _identityService;

        public NotificationHub(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _identityService.GetUserIdentity());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _identityService.GetUserIdentity());
            await base.OnDisconnectedAsync(ex);
        }
    }
}