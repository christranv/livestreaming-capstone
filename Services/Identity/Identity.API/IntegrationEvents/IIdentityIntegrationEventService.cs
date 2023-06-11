using System.Threading.Tasks;
using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Identity.API.IntegrationEvents
{
    public interface IIdentityIntegrationEventService
    {
        Task SaveEventAndIdentityContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
