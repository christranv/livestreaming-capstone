using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Payment
{
    public class NewPaymentRecordAddedIntegrationEvent : IntegrationEvent
    {
        public string UserGuid { get; }
        public int Balance { get; }

        public NewPaymentRecordAddedIntegrationEvent(string userGuid, int balance)
        {
            UserGuid = userGuid;
            Balance = balance;
        }
    }
}