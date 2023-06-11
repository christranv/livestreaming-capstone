using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.API.Infrastructure.Services.Interface;
using Serilog.Context;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Identity;

namespace Payment.API.Application.IntegrationEvents.EventHandling
{
    public class NewUserCreatedIntegrationEventHandler :
        IIntegrationEventHandler<NewUserCreatedIntegrationEvent>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<NewUserCreatedIntegrationEventHandler> _logger;

        public NewUserCreatedIntegrationEventHandler(
            IPaymentService paymentService,
            ILogger<NewUserCreatedIntegrationEventHandler> logger )
        {
            _paymentService = paymentService;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(NewUserCreatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);
                _paymentService.AddUserPayment(@event.IdentityGuid);
            }
        }
    }
}