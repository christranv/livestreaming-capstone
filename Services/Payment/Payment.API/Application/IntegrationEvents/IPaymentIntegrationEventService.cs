using System;
using System.Threading.Tasks;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Payment.API.Application.IntegrationEvents
{
    public interface IPaymentIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
        Task SaveEventAndPaymentContextChangesAsync(IntegrationEvent @event, Func<Task> action);
    }
}