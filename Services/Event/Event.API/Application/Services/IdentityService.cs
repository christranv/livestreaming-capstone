using System;
using Event.API.Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Http;

namespace Event.API.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;
        
        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public string GetUserIdentity()
        {
            return _context.HttpContext?.User.FindFirst("id")?.Value;
        }

        public string GetUserName()
        {
            return _context.HttpContext?.User.Identity?.Name;
        }
    }
}