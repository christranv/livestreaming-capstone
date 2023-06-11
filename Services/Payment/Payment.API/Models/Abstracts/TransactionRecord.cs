using System;

namespace Payment.API.Models.Abstracts
{
    public abstract class TransactionRecord
    {
        public string Id { get; }
        public int Amount { get; }
        public DateTime CreateDate { get; }

        protected TransactionRecord(int amount)
        {
            Id = Guid.NewGuid().ToString();
            Amount = amount;
            CreateDate = DateTime.UtcNow;
        }
    }
}