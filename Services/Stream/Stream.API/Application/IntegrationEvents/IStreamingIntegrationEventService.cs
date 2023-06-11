using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Stream.API.Application.IntegrationEvents
{
    public interface IStreamingIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
        Task AddAndSaveEventsAsync(List<IntegrationEvent> evts);
    }
}