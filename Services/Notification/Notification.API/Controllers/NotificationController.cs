using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Team5.BuildingBlocks.Core.Infrastructure.ViewModel;

namespace Notification.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IMapper _mapper;

        public NotificationController(ILogger<NotificationController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        // GET api/[controller]/items[?pageSize=3&pageIndex=10]
        // [HttpGet]
        // [Route("items")]
        // [ProducesResponseType(typeof(PaginatedItemsViewModel<Models.Notification>), (int) HttpStatusCode.OK)]
        // [ProducesResponseType(typeof(IEnumerable<Models.Notification>), (int) HttpStatusCode.OK)]
        // [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        // public async Task<IActionResult> NotificationsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0 )
        // {
        //     _logger.LogInformation("Getting from categories");
        //     // totalItems = await _topicContext.CategoryItems.LongCountAsync();
        //     // var items = await _topicContext.CategoryItems
        //     //     .OrderBy(c => c.Name)
        //     //     .Skip(pageIndex * pageSize)
        //     //     .Take(pageSize)
        //     //     .Include(c => c.CategoryTags)
        //     //     .ThenInclude(ct => ct.Tag)
        //     //     .ToListAsync();
        //     //
        //     // itemsOnPage = _mapper.Map<IEnumerable<CategoryViewModel>>(items);
        //     //
        //
        //     // var model = new PaginatedItemsViewModel<CategoryViewModel>(pageIndex, pageSize, totalItems, itemsOnPage);
        //
        //     // return Ok(model);
        // }
    }
}