using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServerAspNetIdentity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources(IConfiguration configuration)
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApis(IConfiguration configuration)
        {
            return new[]
            {
                new ApiResource{
                    Enabled = true,
                    DisplayName = "VNR's API Resource",
                    Name = "VNR",
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "VNR.FullAccess",
                            DisplayName = "Full information access",
                            Required = true
                        },
                        new Scope
                        {
                            Name = "VNR.BasicAccess",
                            DisplayName = "Basic information access",
                            Required = true
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new[]
            {
                new Client
                {
                    ClientId = "VNR_API",
                    ClientName = "VNR's API Client",

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(configuration.GetValue<string>("VNR_API_ClientSecret").Sha256())
                    },

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    RequireConsent = false,

                    // scopes that client has access to
                    AllowedScopes = { "VNR.FullAccess" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "VNR_MVC",
                    ClientName = "VNR's MVC Client",

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = { new Secret(configuration.GetValue<string>("VNR_MVC_ClientSecret").Sha256()) },

                    RedirectUris = { configuration.GetValue<string>("VNR_MVC_LoginRedirectURI") },
                    PostLogoutRedirectUris = { configuration.GetValue<string>("VNR_MVC_LogoutRedirectURI") },

                    RequireConsent = false,

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", "VNR.FullAccess" }
                }
            };
        }
    }
}