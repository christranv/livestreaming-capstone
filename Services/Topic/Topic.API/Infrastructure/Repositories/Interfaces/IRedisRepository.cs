using System;
using System.Collections.Generic;
using Topic.API.ViewModel;

namespace Topic.API.Infrastructure.Repositories.Interfaces
{
    public interface IRedisRepository
    {
        void SetTotalCategoriesCount(long value);
        long GetTotalCategoriesCount();
        void UpdateCategories(IEnumerable<CategoryViewModel> data, int start, int end);
        IEnumerable<CategoryViewModel> GetCategoriesAsync(int start, int end);
    }
}
