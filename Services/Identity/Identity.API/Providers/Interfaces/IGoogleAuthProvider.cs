using System;
using Identity.API.Models;
using Identity.API.Providers.Interfaces;

namespace Identity.API.Providers
{
    public interface IGoogleAuthProvider : IExternalAuthProvider
    {
        Provider Provider { get; }
    }
}
