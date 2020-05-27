using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Test;

namespace SampleOAuth
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "A847CCF8-5EC5-4DB5-A652-6C192FC434A9", 
                Username = "alice", 
                Password = "alice",
                Claims =
                {
                    new Claim("userId", "A847CCF8-5EC5-4DB5-A652-6C192FC434A9"),
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Address,
                        @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                        IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "Heidelberg")
                }
            },
            new TestUser
            {
                SubjectId = "2C5DE5E8-644D-4533-ABB8-58C5B4F87A78", 
                Username = "bob", 
                Password = "bob",
                Claims =
                {
                    new Claim("userId", "2C5DE5E8-644D-4533-ABB8-58C5B4F87A78"),
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Address,
                        @"{ 'street_address': 'One Hacker Way', 'locality': 'Amsterdam', 'postal_code': 69118, 'country': 'Netherlands' }",
                        IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "Amsterdam")
                }
            }
        };
    }
}