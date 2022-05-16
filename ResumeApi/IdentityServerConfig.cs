using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResumeApi
{
    public static class IdentityServerConfig
    {
        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource()
                {
                    Name = "resumeapi",
                    Scopes =
                    {
                        "resumeapi"
                    }
                }
            };
        }
        public static List<Client> GetApiClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "resumeapi",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = true,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,
                    AbsoluteRefreshTokenLifetime = 28800,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AllowedScopes = new List<string>
                    {
                        "resumeapi"
                    },
                    ClientSecrets = new List<Secret>
                    {
                        new Secret()
                        {
                            Value = "testpass".Sha256()
                        }
                    }

                }
            };
        }
        public static List<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                new ApiScope("resumeapi")
                {
                    UserClaims = {
                    "name",
                    "email",
                    "phone_number",
                    "role",
                    "permission"
                    }
                }
            };
        }
    }
}
