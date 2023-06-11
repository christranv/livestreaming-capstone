using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Stream.Infrastructure.Idempotency;
using Team5.BuildingBlocks.MessageBroker.EventBus.Extensions;

namespace Stream.API.Application.Commands
{
    /// <summary>
    /// Provides a base implementation for handling duplicate request and ensuring idempotent updates, in the cases where
    /// a requestid sent by client is used to detect duplicate requests.
    /// </summary>
    /// <typeparam name="T">Type of the command handler that performs the operation if request is not duplicated</typeparam>
    /// <typeparam name="TR">Return value of the inner command handler</typeparam>
    public class IdentifiedCommandHandler<T, TR> : IRequestHandler<IdentifiedCommand<T, TR>, TR>
        where T : IRequest<TR>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdentifiedCommandHandler<T, TR>> _logger;

        protected IdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<T, TR>> logger)
        {
            _mediator = mediator;
            _requestManager = requestManager;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected virtual TR CreateResultForDuplicateRequest()
        {
            return default;
        }

        /// <summary>
        /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
        /// just enqueues the original inner command.
        /// </summary>
        /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<TR> Handle(IdentifiedCommand<T, TR> message, CancellationToken cancellationToken)
        {
            var alreadyExists = await _requestManager.ExistAsync(message.Id);
            if (alreadyExists)
            {
                return CreateResultForDuplicateRequest();
            }

            await _requestManager.CreateRequestForCommandAsync<T>(message.Id);
            try
            {
                var command = message.Command;
                var commandName = command.GetGenericTypeName();
                string idProperty;
                string commandId;

                switch (command)
                {
                    case CreateStreamerCommand createStreamerCommand:
                        idProperty = nameof(createStreamerCommand.IdentityGuid);
                        commandId = $"{createStreamerCommand.IdentityGuid}";
                        break;

                    default:
                        idProperty = "Id?";
                        commandId = "n/a";
                        break;
                }

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    commandName,
                    idProperty,
                    commandId,
                    command);

                // Send the embedded business command to mediator so it runs its related CommandHandler 
                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "----- Command result: {@Result} - {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    result,
                    commandName,
                    idProperty,
                    commandId,
                    command);

                return result;
            }
            catch
            {
                return default;
            }
        }
    }
}