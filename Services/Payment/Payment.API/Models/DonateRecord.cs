using Payment.API.Models.Abstracts;

namespace Payment.API.Models
{
    public class DonateRecord : TransactionRecord
    {
        public string StreamSessionId { get; }
        public string ReceiverIdentityGuid { get; }
        public string UserName { get; }
        public string Message { get; }
        public string UserId { get; }
        public UserPayment UserPayment { get; }

        public DonateRecord(int amount, string userId, string streamSessionId, string receiverIdentityGuid, string userName, string message) : base(amount)
        {
            UserId = userId;
            StreamSessionId = streamSessionId;
            ReceiverIdentityGuid = receiverIdentityGuid;
            UserName = userName;
            Message = message;
        }
    }
}