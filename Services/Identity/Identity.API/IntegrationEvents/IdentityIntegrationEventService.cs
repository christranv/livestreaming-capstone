using System;
using System.Data.Common;
using System.Threading.Tasks;
using Identity.API.Data;
using Identity.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Utilities;

namespace Identity.API.IntegrationEvents
{
    public class IdentityIntegrationEventService : IIdentityIntegrationEventService, IDisposable
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly ApplicationDbContext _dbContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<IdentityIntegrationEventService> _logger;
        private volatile bool disposedValue;

        public IdentityIntegrationEventService(
            ILogger<IdentityIntegrationEventService> logger,
            IEventBus eventBus,
            ApplicationDbContext identityContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = identityContext ?? throw new ArgumentNullException(nameof(identityContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ??
                                                 throw new ArgumentNullException(
                                                     nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_dbContext.Database.GetDbConnection());
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

        public async Task SaveEventAndIdentityContextChangesAsync(IntegrationEvent evt)
        {
            _logger.LogInformation(
                "----- IdentityIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}",
                evt.Id);

            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            // await ResilientTransaction.New(_dbContext).ExecuteAsync(async () =>
            // {
            //     // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
            //     await _eventLogService.SaveEventAsync(evt, _dbContext.Database.CurrentTransaction);
            //     // await _dbContext.SaveChangesAsync();
            // });

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                using (var transaction = await _dbContext.BeginTransactionAsync())
                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                {
                    await _eventLogService.SaveEventAsync(evt, _dbContext.Database.CurrentTransaction);
                    await _dbContext.CommitTransactionAsync(transaction);

                    transactionId = transaction.TransactionId;
                }

                // await _streamingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    (_eventLogService as IDisposable)?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}