using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace Identity.API.Configuration
{
    public static class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("topic", "Topic Service")
                {
                    Scopes = {"topic"}
                },
                new ApiResource("stream", "Stream Management Service")
                {
                    Scopes = {"stream"}
                },
                new ApiResource("stream.hub", "Stream Management Service")
                {
                    Scopes = {"stream.hub"}
                },
                new ApiResource("notification", "Notification Service")
                {
                    Scopes = {"notification"}
                },
                new ApiResource("event", "Event Service")
                {
                    Scopes =
                    {
                        "event"
                    }
                },
                new ApiResource("payment", "Payment Service")
                {
                    Scopes =
                    {
                        "payment"
                    }
                },
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName),
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("topic", "Topic Service"),
                new ApiScope("stream", "Stream Management Service"),
                new ApiScope("stream.hub", "Stream Realtime Service"),
                new ApiScope("notification", "Notification Service"),
                new ApiScope("event", "Event Service"),
                new ApiScope("payment", "Payment Service"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "Roles", new[] {JwtClaimTypes.Role})
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "SPA OpenId Client",
                    AllowedGrantTypes = new[] {GrantType.ResourceOwnerPassword, "external"},
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        "topic",
                        "stream",
                        "stream.hub",
                        "notification",
                        "event",
                        "payment",
                        "roles"
                    },
                    AccessTokenLifetime = 31622400,
                },
            };
        }
    }
}