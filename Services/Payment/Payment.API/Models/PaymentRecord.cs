using System;
using Payment.API.Models.Abstracts;
using Payment.API.Models.Enums;

namespace Payment.API.Models
{
    public class PaymentRecord : TransactionRecord
    {
        public TransactionType TransactionType { get; }
        public PaymentMethod PaymentMethod { get; } 
        public string UserId { get; }
        public UserPayment UserPayment { get; }

        public PaymentRecord(TransactionType transactionType, PaymentMethod paymentMethod,
            string userId, int amount) : base(amount)
        {
            TransactionType = transactionType;
            PaymentMethod = paymentMethod;
            UserId = userId;
        }
    }
}