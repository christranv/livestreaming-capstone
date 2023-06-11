using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Stream.API.Application.Commands;
using Stream.Infrastructure;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBus.Extensions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Identity;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Stream.API.Application.IntegrationEvents.EventHandling
{
    public class StreamSessionsViewCountChangeIntegrationEventHandler :
        IIntegrationEventHandler<StreamSessionsViewCountChangeIntegrationEvent>
    {
        private readonly StreamContext _streamContext;
        private readonly ILogger<StreamSessionsViewCountChangeIntegrationEventHandler> _logger;
        private readonly IMediator _mediator;

        public StreamSessionsViewCountChangeIntegrationEventHandler(
            StreamContext streamContext,
            ILogger<StreamSessionsViewCountChangeIntegrationEventHandler> logger,
            IMediator mediator)
        {
            _streamContext = streamContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _mediator = mediator;
        }

        public async Task Handle(StreamSessionsViewCountChangeIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                    @event.Id, Program.AppName, @event);
                var command = new UpdateStreamSessionsViewCountCommand(@event.ViewCountPerStreamSession);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    command.GetGenericTypeName(), 0, 0, command);
                await _mediator.Send(command);
            }
        }
    }
}