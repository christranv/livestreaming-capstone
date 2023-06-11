using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Team5.BuildingBlocks.Core.Infrastructure.ViewModel;
using Topic.API.Extensions;
using Topic.API.Infrastructure;
using Topic.API.Infrastructure.Repositories.Interfaces;
using Topic.API.Models;
using Topic.API.ViewModel;

namespace Topic.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly TopicContext _topicContext;
        private readonly IRedisRepository _redisRepository;
        private readonly IMapper _mapper;

        public CategoryController(ILogger<CategoryController> logger, TopicContext topicContext,
            IRedisRepository redisRepository, IMapper mapper)
        {
            _logger = logger;
            _topicContext = topicContext;
            _redisRepository = redisRepository;
            _mapper = mapper;
        }

        // GET api/[controller]/search
        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TopicCategory>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<TopicCategory>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> SearchCategoryByNameAsync([FromQuery] string keyword)
        {
            var result = await _topicContext.CategoryItems
                .Where(category => string.IsNullOrEmpty(keyword) || category.Name.Contains(keyword))
                .Include(c => c.CategoryTags)
                .ThenInclude(ct => ct.Tag)
                .OrderBy(category => category.DisplayOrder)
                .Take(10)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryViewModel>>(result));
        }

        // GET api/[controller]/categoryId
        [HttpGet]
        [Route("{categoryId}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TopicCategory>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<TopicCategory>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> CategoryByIdAsync(string categoryId)
        {
            var result = await _topicContext.CategoryItems
                .Where(category => category.Id == categoryId)
                .Include(c => c.CategoryTags)
                .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync();
            return result != null ? Ok(_mapper.Map<CategoryViewModel>(result)) : NotFound();
        }

        // GET api/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TopicCategory>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<TopicCategory>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CategoriesAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0,
            string tagIds = null)
        {
            _logger.LogInformation("Getting from categories");
            if (!string.IsNullOrEmpty(tagIds))
            {
                var result = await GetCategoriesByTagIdsAsync(tagIds, pageIndex, pageSize);

                if (result == null)
                {
                    return NotFound("No Result");
                }

                return Ok(result);
            }

            // Retrieve from redis
            var totalItems = _redisRepository.GetTotalCategoriesCount();
            var itemsOnPage = _redisRepository.GetCategoriesAsync(pageIndex, pageSize);

            // If redis empty then get from db
            if (itemsOnPage == null)
            {
                totalItems = await _topicContext.CategoryItems.LongCountAsync();
                var items = await _topicContext.CategoryItems
                    .OrderBy(c => c.DisplayOrder)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .Include(c => c.CategoryTags)
                    .ThenInclude(ct => ct.Tag)
                    .ToListAsync();

                itemsOnPage = _mapper.Map<IEnumerable<CategoryViewModel>>(items);

                _redisRepository.SetTotalCategoriesCount(totalItems);
                _redisRepository.UpdateCategories(itemsOnPage, pageIndex, pageSize);
            }

            var model = new PaginatedItemsViewModel<CategoryViewModel>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        private async Task<PaginatedItemsViewModel<CategoryViewModel>> GetCategoriesByTagIdsAsync(string tagIds,
            int pageIndex,
            int pageSize)
        {
            var tagIdsCheck = tagIds.Split(',').Select(name => (Ok: !string.IsNullOrEmpty(name), Value: name)).ToList();

            if (!tagIdsCheck.All(nid => nid.Ok))
            {
                throw new Exception("tagIds value invalid. Must be comma-separated list of strings");
            }

            var tagIdsToSelect = tagIdsCheck.Select(id => id.Value.ToString()).ToList();

            // Get list of categoryId matching condition
            var categoryIdTagIds = await _topicContext.CategoryItems
                .Include(c => c.CategoryTags)
                .ToDictionaryAsync(c => c.Id, c => c.CategoryTags.Select(ct => ct.TagId));
            var matchingCategoryIds =
                categoryIdTagIds.Where(d => !tagIdsToSelect.Except(d.Value).Any()).Select(t => t.Key);

            var totalItems = await _topicContext.CategoryItems
                .Where(c =>
                    matchingCategoryIds.Contains(c.Id)
                ).LongCountAsync();

            if (totalItems == 0) return null;

            var items = await _topicContext.CategoryItems
                .Include(c => c.CategoryTags)
                .ThenInclude(ct => ct.Tag)
                .Where(c =>
                    matchingCategoryIds.Contains(c.Id)
                )
                .OrderBy(t => t.DisplayOrder)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedResult = _mapper.Map<IEnumerable<CategoryViewModel>>(items);
            return new PaginatedItemsViewModel<CategoryViewModel>(pageIndex, pageSize, totalItems, mappedResult);
        }
    }
}