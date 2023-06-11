using System.Collections.Generic;
using Identity.API.Helpers;
using Identity.API.Models;
using Identity.API.Repositories.Interfaces;

namespace Identity.API.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        public IEnumerable<Provider> Get()
        {
            return ProviderDataSource.GetProviders();
        }
    }
}
