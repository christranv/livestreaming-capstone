using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payment.API.Infrastructure;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Utilities;

namespace Payment.API.Application.IntegrationEvents
{
    public class PaymentIntegrationEventService : IPaymentIntegrationEventService, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly PaymentContext _paymentContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<PaymentIntegrationEventService> _logger;
        private volatile bool _disposedValue;

        public PaymentIntegrationEventService(
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            IEventBus eventBus,
            PaymentContext paymentContext,
            ILogger<PaymentIntegrationEventService> logger)
        {
            var integrationEventLogServiceFactory1 = integrationEventLogServiceFactory ??
                                                     throw new ArgumentNullException(
                                                         nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _paymentContext = paymentContext ?? throw new ArgumentNullException(nameof(paymentContext));
            _eventLogService = integrationEventLogServiceFactory1(_paymentContext.Database.GetDbConnection());
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

        public async Task SaveEventAndPaymentContextChangesAsync(IntegrationEvent @event, Func<Task> action)
        {
            _logger.LogInformation(
                "----- PaymentIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}",
                @event.Id);

            // //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            // await ResilientTransaction.New(_paymentContext).ExecuteAsync(async () =>
            // {
            //     // Achieving atomicity between original payment database operation and the IntegrationEventLog thanks to a local transaction
            //     await action();
            //     await _paymentContext.SaveChangesAsync();
            //     await _eventLogService.SaveEventAsync(@event, _paymentContext.Database.CurrentTransaction);
            // });

            // var strategy = _paymentContext.Database.CreateExecutionStrategy();
            // await strategy.ExecuteAsync(async () =>
            // {
            //     using (var transaction = await _paymentContext.Database.BeginTransactionAsync())
            //     {
            //         await action();
            //         await _paymentContext.SaveChangesAsync();
            //         await _eventLogService.SaveEventAsync(@event, _paymentContext.Database.CurrentTransaction);
            //     }
            // });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    (_eventLogService as IDisposable)?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}