﻿using System;
using Microsoft.AspNetCore.Http;
using Stream.SignalrHub.Services.Interfaces;

namespace Stream.SignalrHub.Services
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
            return _context.HttpContext?.User.FindFirst("name")?.Value;
        }
    }
}