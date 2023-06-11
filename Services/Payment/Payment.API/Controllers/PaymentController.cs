using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Payment.API.Infrastructure.Services;
using Payment.API.Infrastructure.Services.Interface;
using Payment.API.ViewModels;

namespace Payment.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly IIdentityService _identityService;
        
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IConfiguration configuration, IIdentityService identityService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _configuration = configuration;
            _identityService = identityService;
        }

        [HttpPost]
        [Route("momopayment")]
        public ActionResult MomoRequest(string orderInfo, string amount)
        {
            _logger.LogInformation("Send request to Momo Payment");
            var id = _identityService.GetUserIdentity();
            
            var forwardUrl = new ForwardUrlDto(_paymentService.MomoRequest(orderInfo, amount, id));
            return Ok(forwardUrl);
        }

        [HttpGet]
        [Route("momopayment/ReturnUrl")]
        public ActionResult ReturnUrlMoMo()
        {
            _logger.LogInformation("Return URL Momo !!");
            var param = "partnerCode="+ 
                        Request.Query["partnerCode"] + "&accessKey="+
                        Request.Query["accessKey"] + "&requestId=" +
                        Request.Query["requestId"] + "&amount=" + 
                        Request.Query["amount"] + "&orderId="+
                        Request.Query["orderId"] + "&orderInfo="+ 
                        Request.Query["orderInfo"] + "&orderType=" +
                        Request.Query["orderType"] + "&transId="+ 
                        Request.Query["transId"] + "&message=" +
                        Request.Query["message"] + "&localMessage=" +
                        Request.Query["localMessage"] + "&responseTime="+ 
                        Request.Query["responseTime"] + "&errorCode=" +
                        Request.Query["errorCode"] + "&payType="+ 
                        Request.Query["payType"] + "&extraData=" +
                        Request.Query["extraData"];
            MomoSecurity crypto = new MomoSecurity();
            var serectKey = _configuration.GetValue<string>("secretKey");
            var signature = crypto.signSHA256(param, serectKey);
            return signature != Request.Query["signature"].ToString() ?
                Ok("Invalid Request !!") : 
                Ok(!Request.Query["errorCode"].Equals("0") ? "Payment Failed !!" : "Payment Succeed !!");
        }
        
        [HttpGet]
        public ActionResult<UserPaymentViewModel> GetAllUserPayment()
        {
            _logger.LogInformation("Get all user payment.");
            return Ok(_paymentService.GetAllUserPayment());
        }

        [HttpGet]
        [Route("{userId}")]
        public ActionResult<UserPaymentViewModel> GetUserPaymentById(string userId)
        {
            var userPaymentById = _paymentService.GetUserPaymentById(userId);
            if (userPaymentById == null)
            {
                _logger.LogWarning($"User payment with Id {userId} not found !!");
                return NotFound();
            }

            return Ok(userPaymentById);
        }

        [HttpGet]
        [Route("currentUser")]
        public ActionResult<UserPaymentViewModel> GetCurrentUserPayment()
        {
            var id = _identityService.GetUserIdentity();
            var currentUserPayment = _paymentService.GetUserPaymentById(id);
            if (currentUserPayment == null)
            {
                _logger.LogWarning($"User payment with Id {id} not found !!");
                return NotFound();
            }
            
            return Ok(currentUserPayment);
        }

    }
    
}