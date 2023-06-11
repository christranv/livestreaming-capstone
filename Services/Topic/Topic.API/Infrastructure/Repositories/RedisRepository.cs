using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Topic.API.Infrastructure.Repositories.Interfaces;
using Topic.API.ViewModel;

namespace Topic.API.Infrastructure.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private static readonly string CATEGORY_PAGINATED = "paginated_category";
        private static readonly string CATEGORY_TOTAL = "total_category";

        private readonly ILogger<RedisRepository> _logger;
        private readonly IDatabase _database;

        public RedisRepository(ILogger<RedisRepository> logger, ConnectionMultiplexer redis)
        {
            _logger = logger;
            _database = redis.GetDatabase();
        }

        public long GetTotalCategoriesCount()
        {
            var data = _database.StringGet(CATEGORY_TOTAL);
            return (data.IsNullOrEmpty) ? 0 : long.Parse(data);
        }

        public void SetTotalCategoriesCount(long value)
        {
            _database.StringSet(CATEGORY_TOTAL, value);
        }

        public IEnumerable<CategoryViewModel> GetCategoriesAsync(int pageIndex, int pageSize)
        {
            var data = _database.StringGet(GetPaginatedName(pageIndex, pageSize));
            _logger.LogInformation("Redis: Category item retrieved from redis");

            return data.IsNull ? null : JsonConvert.DeserializeObject<IEnumerable<CategoryViewModel>>(data);
        }

        public void UpdateCategories(IEnumerable<CategoryViewModel> data, int pageIndex, int pageSize)
        {
            _database.StringAppend(GetPaginatedName(pageIndex, pageSize), JsonConvert.SerializeObject(data));

            _logger.LogInformation("Redis: Category item persisted successfully");
        }

        private static string GetPaginatedName(int index, int size)
        {
            return string.Concat(CATEGORY_PAGINATED, "_", index, "_", size);
        }
    }
}
