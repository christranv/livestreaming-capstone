using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Infrastructure.Idempotency;

namespace Stream.API.Application.Commands
{
    // Regular CommandHandler
    public class CreateStreamerCommandHandler : IRequestHandler<CreateStreamerCommand, bool>
    {
        private readonly IStreamerRepository _streamerRepository;

        public CreateStreamerCommandHandler(IStreamerRepository streamerRepository)
        {
            _streamerRepository = streamerRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// new user created
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CreateStreamerCommand command, CancellationToken cancellationToken)
        {
            var streamer = new Streamer(command.IdentityGuid);
            _streamerRepository.Add(streamer);
            return await _streamerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }


    // Use for Idempotency in Command process
    public class CreateStreamerIdentifiedCommandHandler : IdentifiedCommandHandler<CreateStreamerCommand, bool>
    {
        public CreateStreamerIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateStreamerCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for processing streamer.
        }
    }
}
