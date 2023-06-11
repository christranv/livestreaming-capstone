using System;
using Payment.API.Models.Enums;

namespace Payment.API.ViewModels
{
    public class PaymentRecordViewModel
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentRecordViewModel(string id, int amount, DateTime createDate, PaymentMethod paymentMethod)
        {
            Id = id;
            Amount = amount;
            CreateDate = createDate;
            PaymentMethod = paymentMethod;
        }
    }
}