using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Payment.API.Application.IntegrationEvents;
using Payment.API.Infrastructure;
using Payment.API.Infrastructure.Services;
using Payment.API.Infrastructure.Services.Interface;
using Payment.API.Models;
using Payment.API.ViewModels;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;
using Team5.BuildingBlocks.MessageBroker.Team5IntegrationEvents.Payment;

namespace Payment.API.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentContext _paymentContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPaymentIntegrationEventService _paymentIntegrationEventService;
        private readonly IIntegrationEventLogService _eventLogService;

        public PaymentService(PaymentContext paymentContext, IMapper mapper, IConfiguration configuration,
            IPaymentIntegrationEventService paymentIntegrationEventService,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory
        )
        {
            _paymentContext = paymentContext;
            _mapper = mapper;
            _configuration = configuration;
            _paymentIntegrationEventService = paymentIntegrationEventService;
            var integrationEventLogServiceFactory1 = integrationEventLogServiceFactory ??
                                                     throw new ArgumentNullException(
                                                         nameof(integrationEventLogServiceFactory));
            _eventLogService = integrationEventLogServiceFactory1(_paymentContext.Database.GetDbConnection());
        }

        //Create User Payment Record from Id in Identity
        public void AddUserPayment(string userGuid)
        {
            var userPayment = new UserPayment(userGuid, 0);
            _paymentContext.UserPayments.Add(userPayment);
            _paymentContext.SaveChanges();
        }

        public string MomoRequest(string orderInfo, string amount, string userId)
        {
            // var existingUserPaymentCount = _paymentContext.UserPayments.Count(u => u.UserId == userId);
            // if (existingUserPaymentCount == 0)
            // {
            //     CreateUserPaymentRecord(userId);
            // }
            var endPoint = _configuration.GetValue<string>("endPoint");
            var partnerCode = _configuration.GetValue<string>("partnerCode");
            var accessKey = _configuration.GetValue<string>("accessKey");
            var secretKey = _configuration.GetValue<string>("secretKey");
            var orderInfor = orderInfo;
            var returnUrl = _configuration.GetValue<string>("returnUrl");
            var notifyUrl = _configuration.GetValue<string>("notifyUrl");

            var amountMoney = amount;
            var orderId = Guid.NewGuid().ToString();
            var requestId = Guid.NewGuid().ToString();
            var extraData = "userId=" + userId;

            var rawHash = "partnerCode=" +
                          partnerCode + "&accessKey=" +
                          accessKey + "&requestId=" +
                          requestId + "&amount=" +
                          amountMoney + "&orderId=" +
                          orderId + "&orderInfo=" +
                          orderInfor + "&returnUrl=" +
                          returnUrl + "&notifyUrl=" +
                          notifyUrl + "&extraData=" +
                          extraData;

            MomoSecurity crypto = new MomoSecurity();
            string signature = crypto.signSHA256(rawHash, secretKey);

            JObject message = new JObject
            {
                {"partnerCode", partnerCode},
                {"accessKey", accessKey},
                {"requestId", requestId},
                {"amount", amountMoney},
                {"orderId", orderId},
                {"orderInfo", orderInfor},
                {"returnUrl", returnUrl},
                {"notifyUrl", notifyUrl},
                {"extraData", extraData},
                {"requestType", "captureMoMoWallet"},
                {"signature", signature}
            };

            string responseFromMomo = MomoPaymentRequest.sendPaymentRequest(endPoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);

            return jmessage.GetValue("payUrl").ToString();
        }

        public IEnumerable<UserPaymentViewModel> GetAllUserPayment()
        {
            var userPayments = _paymentContext.UserPayments.ToList();
            return userPayments.Select(userPayment => new UserPaymentViewModel
                (
                    userPayment.UserId,
                    userPayment.Balance,
                    _mapper.Map<IEnumerable<DonateRecordViewModel>>(_paymentContext.DonateRecords
                        .Where(d => d.UserId == userPayment.UserId)
                        .OrderByDescending(d => d.CreateDate)
                        .ToList()),
                    _mapper.Map<IEnumerable<PaymentRecordViewModel>>(_paymentContext.PaymentRecords
                        .Where(p => p.UserId == userPayment.UserId)
                        .OrderByDescending(p => p.CreateDate)
                        .ToList()),
                    userPayment.CreatedDate
                )
            ).ToList();
            // return _mapper.Map<IEnumerable<UserPaymentViewModel>>(_paymentContext.UserPayments.ToList());
        }

        public UserPaymentViewModel GetUserPaymentById(string userId)
        {
            var userPaymentById =
                _mapper.Map<UserPaymentViewModel>(_paymentContext.UserPayments.FirstOrDefault(u => u.UserId == userId));
            if (userPaymentById == null) throw new Exception("Not Found User Payment");
            var nested = new UserPaymentViewModel
            (
                userPaymentById.UserId,
                userPaymentById.Balance,
                _mapper.Map<IEnumerable<DonateRecordViewModel>>(_paymentContext.DonateRecords
                    .Where(d => d.UserId == userPaymentById.UserId)
                    .OrderByDescending(d => d.CreateDate)
                    .ToList()),
                _mapper.Map<IEnumerable<PaymentRecordViewModel>>(_paymentContext.PaymentRecords
                    .Where(p => p.UserId == userPaymentById.UserId)
                    .OrderByDescending(p => p.CreateDate)
                    .ToList()),
                userPaymentById.CreatedDate
            );
            userPaymentById = nested;
            return userPaymentById;
        }

        public async Task AddDonateRecord(DonateRecord donateRecord)
        {
            var sender = _paymentContext.UserPayments.SingleOrDefault(u => u.UserId == donateRecord.UserId);
            var receiver =
                _paymentContext.UserPayments.SingleOrDefault(u => u.UserId == donateRecord.ReceiverIdentityGuid);
            if (sender == null || receiver == null) throw new Exception("User not exist!");
            if (sender.Balance < donateRecord.Amount) throw new Exception("Exceed balance!");

            var @event = new NewDonateRecordAddedIntegrationEvent(donateRecord.StreamSessionId,
                donateRecord.UserName, donateRecord.Amount, donateRecord.Message);

            // await _paymentIntegrationEventService.SaveEventAndPaymentContextChangesAsync(@event, async () =>
            // {
            //     sender.AddDonateRecord(donateRecord);
            //     sender.MinusBalance(donateRecord.Amount);
            //     receiver.AddBalance(donateRecord.Amount);
            // });
            
            var strategy = _paymentContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _paymentContext.Database.BeginTransactionAsync())
                {
                    sender.AddDonateRecord(donateRecord);
                    sender.MinusBalance(donateRecord.Amount);
                    receiver.AddBalance(donateRecord.Amount);
                    await _paymentContext.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(@event, _paymentContext.Database.CurrentTransaction);
                    await transaction.CommitAsync();
                }
            });
            await _paymentIntegrationEventService.PublishThroughEventBusAsync(@event);
        }
    }
}