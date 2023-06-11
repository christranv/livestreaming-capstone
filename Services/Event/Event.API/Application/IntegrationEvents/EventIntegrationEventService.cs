using System;
using System.Data.Common;
using System.Threading.Tasks;
using Event.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;

namespace Event.API.Application.IntegrationEvents
{
    public class EventIntegrationEventService : IEventIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly EventContext _eventContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<EventIntegrationEventService> _logger;
        private volatile bool _disposedValue;

        public EventIntegrationEventService(
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            IEventBus eventBus,
            EventContext eventContext, ILogger<EventIntegrationEventService> logger)
        {
            var integrationEventLogServiceFactory1 = integrationEventLogServiceFactory ??
                                                     throw new ArgumentNullException(
                                                         nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventContext = eventContext ?? throw new ArgumentNullException(nameof(eventContext));
            _eventLogService = integrationEventLogServiceFactory1(_eventContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})",
                    evt.Id, Program.AppName, evt);

                await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
                _eventBus.Publish(evt);
                await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                    evt.Id, Program.AppName, evt);
                await _eventLogService.MarkEventAsFailedAsync(evt.Id);
            }
        }

        public async Task SaveEventAndEventContextChangesAsync(IntegrationEvent evt)
        {
            _logger.LogInformation(
                "----- EventIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}",
                evt.Id);

            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            

            // var strategy = _eventContext.Database.CreateExecutionStrategy();
            // await strategy.ExecuteAsync(
            //     async () =>
            //     {
            //         using (var transaction = await _eventContext.Database.BeginTransactionAsync())
            //         {
            //             await _eventContext.SaveChangesAsync();
            //             await _eventLogService.SaveEventAsync(evt, _eventContext.Database.CurrentTransaction);
            //             await transaction.CommitAsync();
            //         }
            //     });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                (_eventLogService as IDisposable)?.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}