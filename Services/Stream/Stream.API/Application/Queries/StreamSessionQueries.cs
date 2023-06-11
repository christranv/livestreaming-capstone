using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stream.API.Application.Models;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Stream.Infrastructure;
using Team5.BuildingBlocks.Core.Infrastructure.ViewModel;

namespace Stream.API.Application.Queries
{
    public class StreamSessionQueries
        : IStreamSessionQueries
    {
        private readonly StreamContext _context;
        private readonly IMapper _mapper;

        public StreamSessionQueries(StreamContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<StreamSessionDTO> GetStreamSessionByIdentityGuidsAsync(string streamerId)
        {
            var result = await _context.StreamSessions
                .Where(ss =>
                    ss.StreamerId == streamerId && ss.StreamSessionStatusId == StreamSessionStatus.Published.Id)
                .Include(ss => ss.Tags)
                .Include(ss => ss.Category)
                .Include(ss => ss.Language)
                .Include(ss => ss.Streamer)
                .FirstOrDefaultAsync();
            if (result == null) throw new KeyNotFoundException();
            return _mapper.Map<StreamSessionDTO>(result);
        }

        public async Task<StreamSessionDTO> GetStreamSessionAsync(string id)
        {
            var result = await _context.StreamSessions
                .Where(ss => ss.Id == id && ss.StreamSessionStatusId == StreamSessionStatus.Published.Id)
                .Include(ss => ss.Tags)
                .Include(ss => ss.Category)
                .Include(ss => ss.Language)
                .Include(ss => ss.Streamer)
                .FirstOrDefaultAsync();
            if (result == null) throw new KeyNotFoundException();
            return _mapper.Map<StreamSessionDTO>(result);
        }

        public async Task<PaginatedItemsViewModel<StreamSessionDTO>> GetStreamSessionsAsync(int pageSize, int pageIndex,
            string categoryGuid = "")
        {
            var totalItems =
                await _context.StreamSessions.LongCountAsync(ss =>
                    ss.StreamSessionStatusId == StreamSessionStatus.Published.Id &&
                    (string.IsNullOrEmpty(categoryGuid) || ss.Category.CategoryGuid == categoryGuid));
            var itemsOnPage = await _context.StreamSessions
                .Where(ss =>
                    ss.StreamSessionStatusId == StreamSessionStatus.Published.Id &&
                    (string.IsNullOrEmpty(categoryGuid) || ss.Category.CategoryGuid == categoryGuid))
                .OrderByDescending(ss => ss.CreatedDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Include(ss => ss.Tags)
                .Include(ss => ss.Category)
                .Include(ss => ss.Language)
                .Include(ss => ss.Streamer)
                .ToListAsync();
            return new PaginatedItemsViewModel<StreamSessionDTO>(pageIndex, pageSize, totalItems,
                _mapper.Map<IEnumerable<StreamSessionDTO>>(itemsOnPage));
        }

        public async Task<PaginatedItemsViewModel<SearchStreamSessionDTO>> SearchStreamSessionsAsync(string keyword,
            int pageSize, int pageIndex)
        {
            var totalItems =
                await _context.StreamSessions.LongCountAsync(ss =>
                    ss.StreamSessionStatusId == StreamSessionStatus.Published.Id &&
                    ss.Title.Contains(keyword));
            var itemsOnPage = await _context.StreamSessions
                .Where(ss =>
                    ss.StreamSessionStatusId == StreamSessionStatus.Published.Id &&
                    ss.Title.Contains(keyword))
                .OrderByDescending(ss => ss.CreatedDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Include(ss => ss.Tags)
                .Include(ss => ss.Streamer)
                .ToListAsync();
            return new PaginatedItemsViewModel<SearchStreamSessionDTO>(pageIndex, pageSize, totalItems,
                _mapper.Map<IEnumerable<SearchStreamSessionDTO>>(itemsOnPage));
        }

        public async Task<IDictionary<string, StreamStatusDTO>> CheckStreamStatusesByStreamerIds(
            IEnumerable<string> identityGuids, IEnumerable<string> streamerIds)
        {
            var itemsOnPage = await _context.StreamSessions
                .Where(ss =>
                    streamerIds.Contains(ss.StreamerId) &&
                    ss.StreamSessionStatusId == StreamSessionStatus.Published.Id)
                .OrderBy(ss => ss.CreatedDate)
                .Include(ss => ss.Category)
                .Include(ss => ss.Streamer)
                .ToDictionaryAsync(ss => ss.Streamer.IdentityGuid, ss => ss);
            return identityGuids.ToDictionary(k => k,
                k => itemsOnPage.ContainsKey(k) ? _mapper.Map<StreamStatusDTO>(itemsOnPage[k]) : null);
        }

        public async Task<IEnumerable<StreamStatusDTO>> GetRecommendStreamSessionsStatus(int pageSize, int pageIndex)
        {
            var itemsOnPage = await _context.StreamSessions
                .Where(ss => ss.StreamSessionStatusId == StreamSessionStatus.Published.Id)
                .OrderByDescending(ss => ss.CreatedDate)
                .Include(ss => ss.Category)
                .Include(ss => ss.Streamer)
                .ToListAsync();
            return _mapper.Map<IEnumerable<StreamStatusDTO>>(itemsOnPage);
        }
    }
}