using System;
using Microsoft.AspNetCore.Http;
using Stream.SignalrHub.Services.Interfaces;

namespace Stream.SignalrHub.Services
{
    public class StreamSessionService : IStreamSessionService
    {
        private readonly IHttpContextAccessor _context;

        public StreamSessionService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetStreamSessionId()
        {
            return _context.HttpContext?.Request.Query["streamSessionId"];
        }
    }
}