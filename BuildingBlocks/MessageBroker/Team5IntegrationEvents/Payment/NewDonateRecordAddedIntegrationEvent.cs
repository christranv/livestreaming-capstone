using Team5.BuildingBlocks.MessageBroker.EventBus.Events;

namespace Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Payment
{
    public class NewDonateRecordAddedIntegrationEvent : IntegrationEvent
    {
        public string StreamSessionId { get; }
        public string UserName { get; }
        public int Amount { get; }
        public string Message { get; }

        public NewDonateRecordAddedIntegrationEvent(string streamSessionId, string userName, int amount, string message)
        {
            StreamSessionId = streamSessionId;
            UserName = userName;
            Amount = amount;
            Message = message;
        }
    }
}