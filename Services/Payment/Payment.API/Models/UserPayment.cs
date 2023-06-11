using System;
using System.Collections.Generic;
using System.Linq;

namespace Payment.API.Models
{
    public class UserPayment
    {
        // public string Id { get; set; }
        public string UserId { get; }
        public int Balance { get; private set; }
        public IEnumerable<DonateRecord> DonateHistory { get; } = new List<DonateRecord>();
        public IEnumerable<PaymentRecord> PaymentHistory { get; } = new List<PaymentRecord>();
        public DateTime CreatedDate { get; }

        public UserPayment(string userId, int balance)
        {
            UserId = userId;
            Balance = balance;
            CreatedDate = DateTime.Now;
        }

        public void AddBalance(int amount)
        {
            Balance += amount;
        }

        public void MinusBalance(int amount)
        {
            if(Balance < amount) throw new Exception("Exceed balance amount");
            Balance -= amount;
        }
        
        public void AddDonateRecord(DonateRecord donateRecord)
        {
            ((IList<DonateRecord>)DonateHistory).Add(donateRecord);
        }
                
        public void AddPaymentRecord(PaymentRecord paymentRecord)
        {
            ((IList<PaymentRecord>)PaymentHistory).Add(paymentRecord);
        }
    }
}