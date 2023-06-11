using System.Threading.Tasks;

namespace Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
