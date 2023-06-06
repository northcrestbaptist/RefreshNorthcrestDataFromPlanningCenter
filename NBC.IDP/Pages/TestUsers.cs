// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using IdentityModel;
using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Test;

namespace NBC.IDP;

public class TestUsers
{
    public static List<TestUser> Users
    {
        get
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };

            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "Richard",
                    Password = "password",
                    Claims =
                    {
                        new Claim("role", "mediaTeam"),
                        new Claim("role", "deacon"),
                        new Claim(JwtClaimTypes.Name, "Richard Johnson"),
                        new Claim(JwtClaimTypes.GivenName, "Richard"),
                        new Claim(JwtClaimTypes.FamilyName, "Johnson"),
                        new Claim(JwtClaimTypes.Email, "richard.johnson5@peraton.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "JD",
                    Password = "password",
                    Claims =
                    {
                        new Claim("roles", "staff"),
                        new Claim(JwtClaimTypes.Name, "JD Ainsworth"),
                        new Claim(JwtClaimTypes.GivenName, "JD"),
                        new Claim(JwtClaimTypes.FamilyName, "Ainsworth"),
                        new Claim(JwtClaimTypes.Email, "jdainsworth@northcrest.com")
                    }
                }
            };
        }
    }
}