using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stream.API.Application.Models;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Team5.BuildingBlocks.Core.Infrastructure.ViewModel;

namespace Stream.API.Application.Queries
{
    public interface IStreamSessionQueries
    {
        public Task<StreamSessionDTO> GetStreamSessionAsync(string id);
        public Task<PaginatedItemsViewModel<StreamSessionDTO>> GetStreamSessionsAsync(int pageSize, int pageIndex, string categoryGuid);
        public Task<PaginatedItemsViewModel<SearchStreamSessionDTO>> SearchStreamSessionsAsync(string keyword, int pageSize, int pageIndex);

        public Task<IEnumerable<StreamStatusDTO>> GetRecommendStreamSessionsStatus(int pageSize, int pageIndex);
        public Task<IDictionary<string, StreamStatusDTO>> CheckStreamStatusesByStreamerIds(
            IEnumerable<string> identityGuids, IEnumerable<string> streamerIds);
    }
}
