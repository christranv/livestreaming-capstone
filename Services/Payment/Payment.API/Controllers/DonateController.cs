using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payment.API.Infrastructure.Services.Interface;
using Payment.API.Models;

namespace Payment.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class DonateController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<DonateController> _logger;
        private readonly IIdentityService _identityService;

        public DonateController(IPaymentService paymentService, ILogger<DonateController> logger,
            IIdentityService identityService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _identityService = identityService;
        }

        [HttpPost]
        public async Task<ActionResult> AddDonateRecord(DonateRecord donateRecord)
        {
            _logger.LogInformation("Add Donate Record");
            var donateRecordAdd = new DonateRecord( 
                donateRecord.Amount,
                _identityService.GetUserIdentity(),
                donateRecord.StreamSessionId,
                donateRecord.ReceiverIdentityGuid, 
                donateRecord.UserName,
                donateRecord.Message);
            await _paymentService.AddDonateRecord(donateRecordAdd);
            return Ok();
        }
    }
}