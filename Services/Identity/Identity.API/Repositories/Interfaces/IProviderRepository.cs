using System.Collections.Generic;
using Identity.API.Models;

namespace Identity.API.Repositories.Interfaces
{
    public interface IProviderRepository
    {
        IEnumerable<Provider> Get();
    }
}
