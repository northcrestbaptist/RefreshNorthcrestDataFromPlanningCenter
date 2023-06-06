using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace NBC.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles",
                "Your role(s)",
                new []{ "role" })
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("nbctoolsapi", "NBC Tools API")
                {
                    Scopes = { "nbctoolsapi" },
                    UserClaims = new [] { "role" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
                {
                    new ApiScope("nbctoolsapi") { }
                };

        public static IEnumerable<Client> Clients =>
            new Client[]
                {
                    new Client()
                    {
                        ClientName = "NBC Tools",
                        ClientId = "nbcToolsClient",
                        AllowedGrantTypes = GrantTypes.Code,
                        RedirectUris =
                        {
                            "https://app.northcrestbaptist.com/NorthcrestData/"
                        },
                        PostLogoutRedirectUris =
                        {
                            "https://app.northcrestbaptist.com/NorthcrestData/"
                        },
                        RequireClientSecret = true,
                        //AllowAccessTokensViaBrowser = true,
                        //AlwaysIncludeUserClaimsInIdToken = true,
                        //AlwaysSendClientClaims = true,
                        //RequireConsent = false,
                        AccessTokenLifetime = 60 * 5,
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "roles",
                            "nbctoolsapi"
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        }
                    },
                    new Client()
                    {
                        ClientName = "NBC Tools Native",
                        ClientId = "nbcToolsClientNativeApp",
                        AllowedGrantTypes = GrantTypes.Code,
                        RedirectUris =
                        {
                            "ionic://com.tools.northcrestbaptistchurch"
                        },
                        PostLogoutRedirectUris =
                        {
                            "ionic://com.tools.northcrestbaptistchurch"
                        },
                        RequireClientSecret = true,
                        //RequirePkce = true,
                        //AllowAccessTokensViaBrowser = true,
                        //AlwaysIncludeUserClaimsInIdToken = true,
                        //AlwaysSendClientClaims = true,
                        //RequireConsent = false,
                        AccessTokenLifetime = 60 * 5,
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "roles",
                            "nbctoolsapi"
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        }
                    }
                };
    }
}