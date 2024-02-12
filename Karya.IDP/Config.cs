using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Karya.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource()
            {
                Name = "verification",
                UserClaims = new List<string> 
                { 
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified
                }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        { 
            new ApiScope("weatherapi.read"),
            new ApiScope("weatherapi.write")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("weatherapi")
              {
                Scopes = new List<string> {"weatherapi.read", "weatherapi.write"},
                ApiSecrets = new List<Secret> {new Secret("secret".Sha256())},
                UserClaims = new List<string> {"role"}
              }
        };

    public static IEnumerable<Client> Clients =>
        new List<Client> 
        {
            // machine-to-machine client (from quickstart 1)
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // scopes that client has access to
                AllowedScopes = { "weatherapi.read", "weatherapi.write" }
            },
            // interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = {"https://localhost:7113/signin-oidc"},
                FrontChannelLogoutUri = "https://localhost:7113/signout-oidc",
                PostLogoutRedirectUris = {"https://localhost:7113/signout-callback-oidc"},

                AllowOfflineAccess = true,
                AllowedScopes = {"openid", "profile", "weatherapi.read"},
                RequirePkce = true,
                RequireConsent = true,
                AllowPlainTextPkce = false
            },
            // interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "angular",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = {"http://localhost:4200"},
                FrontChannelLogoutUri = "http://localhost:4200",
                PostLogoutRedirectUris = {"http://localhost:4200"},
                AllowedCorsOrigins = {"http://localhost:4200"},

                AllowOfflineAccess = true,
                AllowedScopes = {"openid", "profile", "weatherapi.read"},
                RequirePkce = true,
                RequireConsent = false,
                RequireClientSecret = false,
                AllowPlainTextPkce = false,
                AllowAccessTokensViaBrowser = true,
                CoordinateLifetimeWithUserSession = true,
            }
        };
}