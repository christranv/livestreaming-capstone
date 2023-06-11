using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.API.Application.Commands
{
    // Regular CommandHandler
    public class UpdateStreamSessionsViewCountCommandHandler
        : IRequestHandler<UpdateStreamSessionsViewCountCommand, bool>
    {
        private readonly IStreamSessionRepository _streamSessionRepository;

        // Using DI to inject infrastructure persistence Repositories
        public UpdateStreamSessionsViewCountCommandHandler(IStreamSessionRepository streamSessionRepository)
        {
            _streamSessionRepository = streamSessionRepository;
        }

        public async Task<bool> Handle(UpdateStreamSessionsViewCountCommand message,
            CancellationToken cancellationToken)
        {
            foreach (var (key, value) in message.ViewCountPerStreamSession)
            {
                var streamSession = await _streamSessionRepository.GetAsync(key);
                streamSession?.UpdateViewCount(value);
            }

            return await _streamSessionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}