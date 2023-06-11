using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Identity
{
    public class NewUserCreatedIntegrationEvent : IntegrationEvent
    {
        public string IdentityGuid { get; }

        public NewUserCreatedIntegrationEvent(string identityGuid)
        {
            IdentityGuid = identityGuid;
        }
    }
}