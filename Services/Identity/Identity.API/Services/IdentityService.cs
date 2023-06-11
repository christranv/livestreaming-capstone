using System;
using Identity.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Identity.API.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context; 
        

        public IdentityService(IHttpContextAccessor context, ILogger<IdentityService> logger)
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
