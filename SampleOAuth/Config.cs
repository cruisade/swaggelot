using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace SampleOAuth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
                {
                    UserClaims = new List<string>()
                    {
                        "location",
                        "userId"
                    }
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedScopes = {"api1", "openid"}
                }
            };
    }
}