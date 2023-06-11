using System.Collections.Generic;
using System.Threading.Tasks;
using Stream.API.Application.Models;
using Stream.Domain.AggregatesModel.StreamerAggregate;

namespace Stream.API.Application.Queries
{
    public interface IStreamerQueries
    {
        Task<Streamer> GetStreamerByIdentityGuidAsync(string identityGuid);
        Task<IEnumerable<string>> GetStreamerIdsFromIdentityGuidsAsync(IEnumerable<string> identityGuids);
        Task<StreamerMediaSourceDTO> GetStreamMediaSourceFromIdentityGuidAsync(string identityGuid);
    }
}
