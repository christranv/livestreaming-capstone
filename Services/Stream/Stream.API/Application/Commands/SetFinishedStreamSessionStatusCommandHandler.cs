using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.API.Application.Commands
{
    // Regular CommandHandler
    public class SetFinishedStreamSessionStatusCommandHandler
        : IRequestHandler<SetFinishedStreamSessionStatusCommand, bool>
    {
        private readonly IStreamSessionRepository _streamSessionRepository;
        private readonly IStreamerRepository _streamerRepository;

        // Using DI to inject infrastructure persistence Repositories
        public SetFinishedStreamSessionStatusCommandHandler(IMediator mediator, IStreamerRepository streamerRepository,
            IStreamSessionRepository streamSessionRepository)
        {
            _streamerRepository = streamerRepository;
            _streamSessionRepository = streamSessionRepository;
        }

        public async Task<bool> Handle(SetFinishedStreamSessionStatusCommand message,
            CancellationToken cancellationToken)
        {
            Streamer streamer;
            if (!string.IsNullOrEmpty(message.LiveToken))
            {
                streamer = await _streamerRepository.FindByAuthTokenAsync(message.LiveToken);
            }
            else
            {
                streamer = await _streamerRepository.FindByIdentityGuidAsync(message.StreamerIdentityGuid);
            }
            if (streamer == null) return false;
            var streamSession = await _streamSessionRepository.GetLatestStreamSessionByStreamerIdAsync(streamer.Id);
            if (streamSession == null || streamSession.StreamSessionStatusId==StreamSessionStatus.Finished.Id) return false;
            streamSession.SetFinishedStatus();

            return await _streamSessionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}