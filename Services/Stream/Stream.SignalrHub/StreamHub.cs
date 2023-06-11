using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Stream.SignalrHub.Models;
using Stream.SignalrHub.Services.Interfaces;

namespace Stream.SignalrHub
{
    [Authorize]
    public class StreamHub : Hub
    {
        private readonly IIdentityService _identityService;
        private readonly IStreamSessionService _streamSessionService;
        private readonly IGroupCounterService _groupCounterService;

        public StreamHub(IIdentityService identityService, IStreamSessionService streamSessionService,
            IGroupCounterService groupCounterService)
        {
            _identityService = identityService;
            _streamSessionService = streamSessionService;
            _groupCounterService = groupCounterService;
        }

        public override async Task OnConnectedAsync()
        {
            var groupId = _streamSessionService.GetStreamSessionId();
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(_streamSessionService.GetStreamSessionId())
                .SendAsync("UpdateCounter", _groupCounterService.NewUserConnected(groupId));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var groupId = _streamSessionService.GetStreamSessionId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _streamSessionService.GetStreamSessionId());
            await Clients.Group(_streamSessionService.GetStreamSessionId())
                .SendAsync("UpdateCounter", _groupCounterService.UserDisconnected(groupId));
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SelfUpdateCounter()
        {
            var groupId = _streamSessionService.GetStreamSessionId();
            await Clients.Client(Context.ConnectionId).SendAsync("SelfUpdateCounter", _groupCounterService.GetGroupUserCount(groupId));
        }
        
        public async Task SendMessage(Message message)
        {
            message.UserName = _identityService.GetUserName();
            await Clients.Group(_streamSessionService.GetStreamSessionId()).SendAsync("SendMessage", message);
        }
    }
}