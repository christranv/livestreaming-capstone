using System.Threading.Tasks;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
