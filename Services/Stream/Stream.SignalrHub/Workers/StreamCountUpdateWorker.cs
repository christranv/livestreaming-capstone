using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stream.SignalrHub.Services.Interfaces;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Stream;

namespace Stream.SignalrHub.Workers
{
    public class StreamCountUpdateWorker : BackgroundService
    {
        private const int UpdateInterval = 10000;

        private readonly ILogger<StreamCountUpdateWorker> _logger;
        private readonly IEventBus _eventBus;
        private readonly IGroupCounterService _groupCounterService;

        public StreamCountUpdateWorker(ILogger<StreamCountUpdateWorker> logger, IEventBus eventBus, IGroupCounterService groupCounterService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _groupCounterService = groupCounterService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                _logger.LogInformation("Updating user count");
                var @event =
                    new StreamSessionsViewCountChangeIntegrationEvent(_groupCounterService.GetAllGroupsUserCount());
                _eventBus.Publish(@event);
                await Task.Delay(UpdateInterval, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested);
        }
    }
}