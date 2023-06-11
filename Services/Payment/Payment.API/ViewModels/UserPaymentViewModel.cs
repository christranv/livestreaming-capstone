using System;
using System.Collections;
using System.Collections.Generic;

namespace Payment.API.ViewModels
{
    public class UserPaymentViewModel
    {
        // public string Id { get; set; }
        public string UserId { get; set; }
        public int Balance { get; set; }
        public IEnumerable<DonateRecordViewModel> DonateHistory { get; set; } = new List<DonateRecordViewModel>();
        public IEnumerable<PaymentRecordViewModel> PaymentHistory { get; set; } = new List<PaymentRecordViewModel>();
        public DateTime CreatedDate { get; set; }

        public UserPaymentViewModel(string userId, int balance,
            IEnumerable<DonateRecordViewModel> donateHistory, 
            IEnumerable<PaymentRecordViewModel> paymentHistory, DateTime createdDate)
        {
            UserId = userId;
            Balance = balance;
            DonateHistory = donateHistory;
            PaymentHistory = paymentHistory;
            CreatedDate = createdDate;
        }

        public UserPaymentViewModel()
        {
        }
        
    }
}