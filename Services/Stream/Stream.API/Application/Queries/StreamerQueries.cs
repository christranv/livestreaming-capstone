using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stream.API.Application.Models;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Infrastructure;

namespace Stream.API.Application.Queries
{
    public class StreamerQueries
        : IStreamerQueries
    {
        private readonly StreamContext _context;
        private readonly IMapper _mapper;

        public StreamerQueries(StreamContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Streamer> GetStreamerByIdentityGuidAsync(string identityGuid)
        {
            var result = await _context.Streamers.FirstOrDefaultAsync(ss => ss.IdentityGuid == identityGuid);
            if (result == null) throw new KeyNotFoundException();
            return result;
        }

        public async Task<IEnumerable<string>> GetStreamerIdsFromIdentityGuidsAsync(
            IEnumerable<string> identityGuids)
        {
            return await _context.Streamers.Where(s => identityGuids.Contains(s.IdentityGuid))
                .Select(s=>s.Id).ToListAsync();
        }

        public async Task<StreamerMediaSourceDTO> GetStreamMediaSourceFromIdentityGuidAsync(string identityGuid)
        {
            var result = await _context.Streamers.FirstOrDefaultAsync(ss => ss.IdentityGuid == identityGuid);
            if (result == null) throw new KeyNotFoundException();
            return _mapper.Map<StreamerMediaSourceDTO>(result);
        }
    }
}