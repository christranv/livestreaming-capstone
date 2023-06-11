using AutoMapper;
using Payment.API.Models;
using Payment.API.ViewModels;

namespace Payment.API.Configuration
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration Configure()
        {
            return new(cfg =>
            {
                cfg.CreateMap<DonateRecord, DonateRecordViewModel>();
                cfg.CreateMap<PaymentRecord, PaymentRecordViewModel>();
                cfg.CreateMap<UserPayment, UserPaymentViewModel>();
            });
        }
    }
}