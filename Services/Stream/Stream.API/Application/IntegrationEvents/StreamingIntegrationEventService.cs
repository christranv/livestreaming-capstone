using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stream.Infrastructure;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;

namespace Stream.API.Application.IntegrationEvents
{
    public class StreamingIntegrationEventService : IStreamingIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly StreamContext _streamingContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<StreamingIntegrationEventService> _logger;

        public StreamingIntegrationEventService(IEventBus eventBus,
            StreamContext streamingContext,
            IntegrationEventLogContext eventLogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<StreamingIntegrationEventService> logger)
        {
            _streamingContext = streamingContext ?? throw new ArgumentNullException(nameof(streamingContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ??
                                                 throw new ArgumentNullException(
                                                     nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_streamingContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                    logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}",
                        logEvt.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation(
                "----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id,
                evt);

            await _eventLogService.SaveEventAsync(evt, _streamingContext.GetCurrentTransaction());
        }

        public async Task AddAndSaveEventsAsync(List<IntegrationEvent> evts)
        {
            foreach (var @event in evts)
            {
                _logger.LogInformation(
                    "----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})",
                    @event.Id,
                    @event);
            }

            await _eventLogService.SaveEventsAsync(evts, _streamingContext.GetCurrentTransaction());
        }
    }
}