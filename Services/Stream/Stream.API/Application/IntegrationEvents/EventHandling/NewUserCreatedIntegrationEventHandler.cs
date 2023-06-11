using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Stream.API.Application.Commands;
using Stream.Infrastructure;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBus.Extensions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Identity;

namespace Stream.API.Application.IntegrationEvents.EventHandling
{
    public class NewUserCreatedIntegrationEventHandler :
        IIntegrationEventHandler<NewUserCreatedIntegrationEvent>
    {
        private readonly StreamContext _streamContext;
        private readonly ILogger<NewUserCreatedIntegrationEventHandler> _logger;
        private readonly IMediator _mediator;

        public NewUserCreatedIntegrationEventHandler(
            StreamContext streamContext,
            ILogger<NewUserCreatedIntegrationEventHandler> logger,
            IMediator mediator)
        {
            _streamContext = streamContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _mediator = mediator;
        }

        public async Task Handle(NewUserCreatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);
                var command = new CreateStreamerCommand(@event.IdentityGuid);
                
                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    command.GetGenericTypeName(),
                    nameof(command.IdentityGuid),
                    command.IdentityGuid,
                    command);
                await _mediator.Send(command);
            }
        }
    }
}