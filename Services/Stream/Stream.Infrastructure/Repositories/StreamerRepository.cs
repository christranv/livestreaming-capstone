using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.Seedwork;

namespace Stream.Infrastructure.Repositories
{
    public class StreamerRepository : IStreamerRepository
    {
        private readonly StreamContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public StreamerRepository(StreamContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Streamer Add(Streamer streamer)
        {
            return _context.Streamers
                .Add(streamer)
                .Entity;
        }

        public Streamer Update(Streamer streamer)
        {
            return _context.Streamers
                .Update(streamer)
                .Entity;
        }

        public Streamer Find(string streamerId)
        {
            var streamer = _context.Streamers
                .SingleOrDefault(b => b.Id == streamerId);
            return streamer;
        }

        public async Task<Streamer> FindByAuthTokenAsync(string authToken)
        {
            var streamer = await _context.Streamers
                .SingleOrDefaultAsync(b => b.AuthToken == authToken);
            return streamer;
        }
        
        public async Task<Streamer> FindByIdentityGuidAsync(string identityGuid)
        {
            var streamer = await _context.Streamers
                .SingleOrDefaultAsync(b => b.IdentityGuid == identityGuid);
            return streamer;
        }
        
        public async Task<string> GetStreamerIdFromIdentityGuid(string identityGuid)
        {
            return await _context.Streamers
                .Where(b => b.IdentityGuid == identityGuid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();;
        }
    }
}