using System.Threading.Tasks;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Event.API.Application.IntegrationEvents
{
    public interface IEventIntegrationEventService
    {
        Task SaveEventAndEventContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
