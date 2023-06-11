using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Stream.Infrastructure.Idempotency;

namespace Stream.API.Application.Commands
{
    // Regular CommandHandler
    public class
        PublishStreamSessionStatusCommandHandler : IRequestHandler<PublishStreamSessionStatusCommand, bool>
    {
        private readonly IStreamSessionRepository _streamSessionRepository;
        private readonly IStreamerRepository _streamerRepository;
        private readonly IMapper _mapper;

        public PublishStreamSessionStatusCommandHandler(IStreamSessionRepository streamSessionRepository,
            IStreamerRepository streamerRepository, IMapper mapper)
        {
            _streamSessionRepository = streamSessionRepository;
            _streamerRepository = streamerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handler which processes the command when
        /// streamer disconnect stream
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(PublishStreamSessionStatusCommand command,
            CancellationToken cancellationToken)
        {
            var streamer = await _streamerRepository.FindByIdentityGuidAsync(command.StreamerIdentityGuid);
            var streamSession = await _streamSessionRepository.GetStartedStreamSessionByStreamerIdAsync(streamer.Id);

            if (streamSession == null) return false;

            var lan = Language.FromName(command.LanguageName);
            // var category = await _streamSessionRepository.GetCategoryById(command.Category.Id);
            // if (category == null)
            var category = new StreamSessionCategory(command.Category.Id, command.Category.Name);
            
            // Create List<StreamSessionTag> from TagDTO
            var tags = command.Tags.Select(tag => new StreamSessionTag(tag.Id, tag.Name)).ToList();

            streamSession.PublishStreamSession(command.StreamerName, command.StreamerImageUrl,streamer.StreamKey, command.Title,
                command.Announcement, lan.Id,
                category, tags,
                command.SubEventId);
            _streamSessionRepository.Update(streamSession);

            return await _streamSessionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }


    // Use for Idempotency in Command process
    public class
        PublishStreamSessionStatusIdentifiedOrderStatusCommandHandler : IdentifiedCommandHandler<
            PublishStreamSessionStatusCommand, bool>
    {
        public PublishStreamSessionStatusIdentifiedOrderStatusCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<PublishStreamSessionStatusCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true; // Ignore duplicate requests for processing order.
        }
    }
}