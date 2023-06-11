using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stream.API.Application.Commands;
using Stream.API.Application.Models;
using Stream.API.Application.Queries;
using Stream.API.Infrastructure.Services;
using Team5.BuildingBlocks.Core.Infrastructure.ViewModel;

namespace Stream.API.Controllers
{
    [Route("api/v1/stream-session")]
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class StreamSessionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStreamSessionQueries _streamSessionQueries;
        private readonly IStreamerQueries _streamerQueries;
        private readonly IIdentityService _identityService;
        private readonly StreamSettings _settings;
        private readonly ILogger<StreamSessionController> _logger;

        public StreamSessionController(
            IMediator mediator,
            IStreamSessionQueries streamSessionQueries,
            IStreamerQueries streamerQueries,
            IIdentityService identityService,
            IOptionsSnapshot<StreamSettings> settings,
            ILogger<StreamSessionController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _streamSessionQueries =
                streamSessionQueries ?? throw new ArgumentNullException(nameof(streamSessionQueries));
            _streamerQueries = streamerQueries ?? throw new ArgumentNullException(nameof(streamerQueries));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _settings = settings.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<StreamSessionDTO>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StreamSessionsAsync([FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0, [FromQuery] string categoryGuid = "")
        {
            _logger.LogInformation("Getting from stream sessions");
            var result = await _streamSessionQueries.GetStreamSessionsAsync(pageSize, pageIndex, categoryGuid);
            return Ok(result);
        }

        // GET api/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<StreamSessionDTO>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchStreamSessionsAsync([FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0, [FromQuery] string keyword = "")
        {
            _logger.LogInformation("Searching from stream sessions");
            var result =
                await _streamSessionQueries.SearchStreamSessionsAsync(keyword, pageSize, pageIndex);
            return Ok(result);
        }

        [HttpGet]
        [Route("stream-status")]
        [ProducesResponseType(typeof(Dictionary<string,StreamSessionDTO>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult> CheckStreamingStatusByIdentityGuidsAsync([FromQuery] string identityGuids)
        {
            _logger.LogInformation("Check streaming status by IdentityGuids");

            if (identityGuids == null) return Ok();
            var checkedInputs = identityGuids.Split(',').Select(name => (Ok: !string.IsNullOrEmpty(name), Value: name))
                .ToList();
            if (!checkedInputs.All(nid => nid.Ok))
                throw new Exception("identityGuids value invalid. Must be comma-separated list of strings");
            var selectedIdentityGuids = checkedInputs.Select(id => id.Value.ToString()).ToList();

            var streamerIds = await _streamerQueries.GetStreamerIdsFromIdentityGuidsAsync(selectedIdentityGuids);
            var streamStatuses =
                await _streamSessionQueries.CheckStreamStatusesByStreamerIds(selectedIdentityGuids, streamerIds);
            return Ok(streamStatuses);
        }

        [HttpGet]
        [Route("recommends")]
        [ProducesResponseType(typeof(IEnumerable<StreamSessionDTO>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetRecommendStreamSessionsStatusAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            _logger.LogInformation("Getting recommend from stream sessions");
            var result = await _streamSessionQueries.GetRecommendStreamSessionsStatus(pageSize, pageIndex);
            return Ok(result);
        }
        

        /// <summary>
        /// STREAMER SIDE
        /// Get stream session url (not related to stream session record)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("streamer-media-source")]
        [ProducesResponseType(typeof(StreamerMediaSourceDTO), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetStreamMediaUrlAsync()
        {
            _logger.LogInformation("Getting streamer's media source");
            return Ok(await _streamerQueries.GetStreamMediaSourceFromIdentityGuidAsync(
                _identityService.GetUserIdentity()));
        }

        [HttpGet]
        [Route("{streamSessionId}")]
        [ProducesResponseType(typeof(StreamSessionDTO), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetStreamAsync(string streamSessionId)
        {
            _logger.LogInformation("Getting from stream session by id");
            try
            {
                var streamSession = await _streamSessionQueries.GetStreamSessionAsync(streamSessionId);
                return Ok(streamSession);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("stream-config")]
        [ProducesResponseType(typeof(StreamSessionDTO), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ConnectStreamConfigDTO> GetStreamConfig()
        {
            var streamer = await _streamerQueries.GetStreamerByIdentityGuidAsync(_identityService.GetUserIdentity());
            return new ConnectStreamConfigDTO(
                $"{_settings.SrsRtmpServerUrl}?token={streamer.AuthToken}",
                streamer.StreamKey);
        }

        [HttpPut]
        [Route("finish")]
        [ProducesResponseType(typeof(StreamSessionDTO), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult> FinishStreamSession([FromHeader(Name = "x-requestid")] string requestId)
        {
            var streamerIdentityId = _identityService.GetUserIdentity();
            var commandResult = await _mediator.Send(new SetFinishedStreamSessionStatusCommand(streamerIdentityId));
            return commandResult ? Ok() : BadRequest();
        }

        [HttpPut]
        [Route("publish")]
        [ProducesResponseType(typeof(StreamSessionDTO), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult> PublishStreamSession([FromBody] PublishStreamSessionStatusCommand command,
            [FromHeader(Name = "x-requestid")] string requestId)
        {
            command.StreamerIdentityGuid = _identityService.GetUserIdentity();
            var commandResult = await _mediator.Send(command);
            return commandResult ? Ok() : BadRequest();
        }
    }
}