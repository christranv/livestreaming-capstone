using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Stream.Domain.Seedwork;

namespace Stream.Infrastructure.Repositories
{
    public class StreamSessionRepository : IStreamSessionRepository
    {
        private readonly StreamContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public StreamSessionRepository(StreamContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public StreamSession Add(StreamSession streamSession)
        {
            return _context.StreamSessions.Add(streamSession).Entity;
        }

        public async Task<StreamSession> GetAsync(string streamSessionId)
        {
            var streamSession = await _context
                .StreamSessions
                .FirstOrDefaultAsync(o => o.Id == streamSessionId);

            if (streamSession == null)
            {
                streamSession = _context
                    .StreamSessions
                    .Local
                    .FirstOrDefault(o => o.Id == streamSessionId);
            }

            if (streamSession == null) return null;
            await _context.Entry(streamSession)
                .Collection(i => i.Tags).LoadAsync();
            // await _context.Entry(streamSession)
            //     .Reference(i => i.SessionCategory).LoadAsync();

            return streamSession;
        }

        public async Task<StreamSession> GetStartedStreamSessionByStreamerIdAsync(string streamerId)
        {
            var streamSession = await _context
                .StreamSessions
                .FirstOrDefaultAsync(o =>
                    o.StreamerId == streamerId && o.StreamSessionStatusId == StreamSessionStatus.Created.Id);

            if (streamSession == null)
            {
                streamSession = _context
                    .StreamSessions
                    .Local
                    .FirstOrDefault(o =>
                        o.StreamerId == streamerId && o.StreamSessionStatusId == StreamSessionStatus.Created.Id);
            }

            return streamSession == null ? null : streamSession;
        }

        public async Task<StreamSession> GetLatestStreamSessionByStreamerIdAsync(string streamerId)
        {
            var streamSession = await _context
                .StreamSessions
                .OrderByDescending(ss => ss.CreatedDate)
                .FirstOrDefaultAsync(o =>
                    o.StreamerId == streamerId);


            if (streamSession == null)
            {
                streamSession = _context
                    .StreamSessions
                    .Local
                    .OrderByDescending(ss => ss.CreatedDate)
                    .FirstOrDefault(o =>
                        o.StreamerId == streamerId);
            }

            return streamSession;
        }

        public async Task<StreamSessionCategory> GetCategoryById(string categoryGuid)
        {
            return await _context.StreamSessionCategories.FirstOrDefaultAsync(c => c.Id == categoryGuid);
        }

        public void Update(StreamSession streamSession)
        {
            _context.Entry(streamSession).State = EntityState.Modified;
        }
    }
}