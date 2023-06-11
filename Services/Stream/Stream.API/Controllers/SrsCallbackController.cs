using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stream.API.Application.Commands;
using Stream.API.Application.Models;
using Stream.API.Application.Queries;
using Stream.Domain.AggregatesModel.StreamerAggregate;

namespace Stream.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SrsCallbackController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStreamSessionQueries _streamSessionQueries;
        private readonly ILogger<SrsCallbackController> _logger;
        private readonly IStreamerRepository _streamerRepository;

        public SrsCallbackController(
            IMediator mediator,
            IStreamSessionQueries streamSessionQueries,
            ILogger<SrsCallbackController> logger,
            IStreamerRepository streamerRepository)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _streamSessionQueries = streamSessionQueries ?? throw new ArgumentNullException(nameof(streamSessionQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _streamerRepository = streamerRepository ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("on-connect")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> NewStreamSessionConnect([FromBody] SrsCallbackModel model)
        {
            _logger.LogInformation("New stream session connect");
            var commandResult = await _mediator.Send(new CreateStreamSessionCommand(model));
            return commandResult ? Ok(0) : BadRequest();
        }

        [Route("on-disconnect")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StreamSessionDisconnect([FromBody] SrsCallbackModel model)
        {
            _logger.LogInformation("Stream session disconnected");
            var token = HttpUtility.ParseQueryString(model.Param).Get("token");
            var commandResult = await _mediator.Send(new SetFinishedStreamSessionStatusCommand("", token));
            return commandResult ? Ok(0) : BadRequest();
        }
    }
}