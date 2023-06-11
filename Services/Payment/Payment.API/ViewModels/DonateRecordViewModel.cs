using System;

namespace Payment.API.ViewModels
{
    public class DonateRecordViewModel
    {
        public string Id { get; }
        public int Amount { get; }
        public DateTime CreateDate { get; }
        public string StreamSessionId { get; }
        public string ReceiverIdentityGuid { get; }
        public string UserName { get; }
        public string Message { get; }

        public DonateRecordViewModel(string id, int amount, DateTime createDate, string receiverIdentityGuid, string streamSessionId, 
            string username, string message)
        {
            Id = id;
            Amount = amount;
            CreateDate = createDate;
            ReceiverIdentityGuid = receiverIdentityGuid;
            StreamSessionId = streamSessionId;
            UserName = username;
            Message = message;
        }
    }
}