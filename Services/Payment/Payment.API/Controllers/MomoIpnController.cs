using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Payment.API.Infrastructure;
using Payment.API.Models;
using Payment.API.Models.Enums;

namespace Payment.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MomoIpnController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly PaymentContext _paymentContext;
        private readonly ILogger<MomoIpnController> _logger;

        public MomoIpnController(IConfiguration configuration, PaymentContext paymentContext,
            ILogger<MomoIpnController> logger)
        {
            _configuration = configuration;
            _paymentContext = paymentContext;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult NotifyUrl([FromForm] MomoIpnModel data)
        {
            _logger.LogInformation("NotifyURL Momo, Save to Payment Record");
            var stringArr = data.ExtraData.Split("=");
            var userId = stringArr[1];
            var amount = int.Parse(data.Amount) / 100;
            if (data.ErrorCode.Equals("0"))
            {
                var userPayment = _paymentContext.UserPayments.SingleOrDefault(u => u.UserId == userId);
                var paymentRecord = new PaymentRecord(TransactionType.Add, PaymentMethod.Momo, userId, amount);

                _paymentContext.PaymentRecords.Add(paymentRecord);
                userPayment?.AddBalance(amount);

                _paymentContext.SaveChanges();
            }

            return Ok();
        }
    }
}