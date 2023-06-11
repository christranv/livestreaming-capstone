using System.Collections.Generic;
using System.Threading.Tasks;
using Payment.API.Models;
using Payment.API.ViewModels;

namespace Payment.API.Infrastructure.Services.Interface
{
    public interface IPaymentService
    {
        string MomoRequest(string orderInfo, string amount, string userId);
        void AddUserPayment(string userGuid);
        Task AddDonateRecord(DonateRecord donateRecord);
        IEnumerable<UserPaymentViewModel> GetAllUserPayment();
        UserPaymentViewModel GetUserPaymentById(string userId);
    }
}