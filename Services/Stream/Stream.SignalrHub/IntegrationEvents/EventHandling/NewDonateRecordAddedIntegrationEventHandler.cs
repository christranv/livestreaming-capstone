using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Stream.SignalrHub.Models;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Payment;

namespace Stream.SignalrHub.IntegrationEvents.EventHandling
{
    public class NewDonateRecordAddedIntegrationEventHandler :
        IIntegrationEventHandler<NewDonateRecordAddedIntegrationEvent>
    {
        private readonly ILogger<NewDonateRecordAddedIntegrationEventHandler> _logger;
        private readonly IHubContext<StreamHub> _hubContext;

        public NewDonateRecordAddedIntegrationEventHandler(
            ILogger<NewDonateRecordAddedIntegrationEventHandler> logger,
            IHubContext<StreamHub> hubContext
        )
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _hubContext = hubContext;
        }

        public async Task Handle(NewDonateRecordAddedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);

                await _hubContext.Clients.Group(@event.StreamSessionId)
                    .SendAsync("SendDonate",
                        new Message(MessageType.Donate, @event.UserName, @event.Message, @event.Amount));
            }
        }
    }
}