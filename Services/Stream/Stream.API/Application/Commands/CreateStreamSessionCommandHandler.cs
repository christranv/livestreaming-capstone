using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Stream.API.Application.Models;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.API.Application.Commands
{
    // Regular CommandHandler
    public class CreateStreamSessionCommandHandler
        : IRequestHandler<CreateStreamSessionCommand, bool>
    {
        private readonly IStreamSessionRepository _streamSessionRepository;
        private readonly IStreamerRepository _streamerRepository;

        // Using DI to inject infrastructure persistence Repositories
        public CreateStreamSessionCommandHandler(IStreamerRepository streamerRepository,
            IStreamSessionRepository streamSessionRepository)
        {
            _streamerRepository = streamerRepository;
            _streamSessionRepository = streamSessionRepository;
        }

        public async Task<bool> Handle(CreateStreamSessionCommand message, CancellationToken cancellationToken)
        {
            var token = HttpUtility.ParseQueryString(message.SrsCallbackCallbackModel.Param).Get("token");
            var streamer = await _streamerRepository.FindByAuthTokenAsync(token);
            // Check token && stream key valid
            if (streamer == null || token != streamer.AuthToken ||
                message.SrsCallbackCallbackModel.Stream != streamer.StreamKey)
                return false;
            // Check stream session created
            var streamSession = await _streamSessionRepository.GetStartedStreamSessionByStreamerIdAsync(streamer.Id);
            if (streamSession != null)
                return true;
            _streamSessionRepository.Add(new StreamSession(streamer.Id));

            return await _streamSessionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}