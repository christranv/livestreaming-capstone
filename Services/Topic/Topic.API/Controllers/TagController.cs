using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Topic.API.Infrastructure;
using Topic.API.ViewModel;

namespace Topic.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class TagController : ControllerBase
    {
        private readonly TopicContext _topicContext;
        private readonly IMapper _mapper;

        public TagController(TopicContext topicContext, IMapper mapper)
        {
            _topicContext = topicContext;
            _mapper = mapper;
        }

        // GET api/[controller]/search
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> TagNamesAsync([FromQuery] string keyword = "")
        {
            var result = await _topicContext.TagItems
                .Where(tag => string.IsNullOrEmpty(keyword) || tag.Name.Contains(keyword))
                .OrderBy(tag => tag.Name)
                .Take(10)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TagViewModel>>(result));
        }
    }
}