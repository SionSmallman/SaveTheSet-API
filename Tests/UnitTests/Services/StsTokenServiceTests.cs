using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using sts_net.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace sts_net.Tests.UnitTests.Services
{
    internal class StsTokenServiceTests
    {
        private StsTokenService stsTokenService;
        private string jwtRegex;

        [SetUp]
        public void Setup()
        {
            var configForTokenService = new Dictionary<string, string>
            {
                {"Jwt:Issuer","http://test.com/" },
                {"Jwt:Audience","http://test.com/" },
                {"Jwt:Key","AAEDD6CAA547B2EF7F3298D1D2DC12EF7F3298D1D2DC1" },
            };
            var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configForTokenService)
            .Build();

            jwtRegex = "^\\S+\\.\\S+\\.\\S+$";

            stsTokenService = new StsTokenService(config);
        }

        [Test]
        public void GetTokenClaims_ValidTokenReturnsClaims()
        {
            //Arrange
            var sampleJwtString = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzcG90aWZ5VXNlcklkIjoiMzFyaGdxYjZtdHQycHBraG41NGJ0cWU2aW1mNCIsInNwb3RpZnlUb2tlbiI6IkJRQ0xkZFM5SkM1N1hYcWFRSnpBTW81dzBqckdrN0N2UEFFZEhMRE1CT0RreE4zdWZCUFJzdnBRelJ2bEJ1S3BaZ3YtTWdzUUtNa245Q2VYS3dEc1Y0TkkwRGZyaFFQTlM5cFA1OWlBaVhtdWNYNl9CLWMzZ3R1aElhdVZTSFZ4UmUtUTdWajVvS2hncGpwSGFySkg3Y19wQVhkRGQ0ZDFlU1dzZ1lLdUtZb1AtUjlOWkFwQ3ZNS09WeUk1bGdRLVpWSFgtUG9RRnZfcVlCNlBqVnVha2x4RGN4RGd4M1lUaTJhUXZHMGVDWGJPZTRVWWxKMGd0WDhfNU9VM0hMSWYwMDdHaXdmeFhqRSIsImV4cGlyZXNJbiI6IjE3MjUyODg2NjMiLCJqdGkiOiI2Mzk3NGVjMy05MjE2LTQzYzYtYjg3OC0wYjZlZGIxNDFhZDMiLCJuYmYiOjE3MjUyODUwNjMsImV4cCI6MTcyNjQ5NDY2MywiaWF0IjoxNzI1Mjg1MDYzLCJpc3MiOiJzaW9uc21hbGxtYW4uY29tIiwiYXVkIjoic2lvbnNtYWxsbWFuLmNvbSJ9.2H4bfRR6gkOtGq8cXWXXy8WT_baHVoLPcYpQH7h6IM8";

            //Act
            var claims = stsTokenService.GetTokenClaims(sampleJwtString);

            // Assert
            // Sample token should always give same values back
            Assert.That(claims, Is.Not.Null);
            Assert.That(claims.SpotifyUserId, Is.EqualTo("31rhgqb6mtt2ppkhn54btqe6imf4"));
            Assert.That(claims.SpotifyUserAccessToken, Is.EqualTo("BQCLddS9JC57XXqaQJzAMo5w0jrGk7CvPAEdHLDMBODkxN3ufBPRsvpQzRvlBuKpZgv-MgsQKMkn9CeXKwDsV4NI0DfrhQPNS9pP59iAiXmucX6_B-c3gtuhIauVSHVxRe-Q7Vj5oKhgpjpHarJH7c_pAXdDd4d1eSWsgYKuKYoP-R9NZApCvMKOVyI5lgQ-ZVHX-PoQFv_qYB6PjVuaklxDcxDgx3YTi2aQvG0eCXbOe4UYlJ0gtX8_5OU3HLIf007GiwfxXjE"));
        }

        // Given same inputs, output token should always be the same
        [Test]
        public void CreateToken_CreatesValidToken()
        {
            //Arrange
            string spotifyUserId = "test_id";
            string spotifyUserAccessToken = "BQCLddS9JC57XXqaQJzAMo5w0jrGk7CvPAEdHLDMBODkxN3ufBPRsvpQzRvlBuKpZgv-MgsQKMkn9CeXKwDsV4NI0DfrhQPNS9pP59iAiXmucX6_B-c3gtuhIauVSHVxRe-Q7Vj5oKhgpjpHarJH7c_pAXdDd4d1eSWsgYKuKYoP-R9NZApCvMKOVyI5lgQ-ZVHX-PoQFv_qYB6PjVuaklxDcxDgx3YTi2aQvG0eCXbOe4UYlJ0gtX8_5OU3HLIf007GiwfxXjE";
            long spotifyUserAccessTokenExpiry = 12345;

            //Act
            var token = stsTokenService.CreateToken(spotifyUserId, spotifyUserAccessToken, spotifyUserAccessTokenExpiry);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtString = tokenHandler.WriteToken(token);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token.Issuer, Is.EqualTo("http://test.com/"));
            Assert.That(jwtString, Does.Match(jwtRegex));
        }

        [Test]
        public void GetJwtStringFromToken_ReturnsJwtString()
        {
            //Arrange
            JwtSecurityToken token = new JwtSecurityToken(signingCredentials: new SigningCredentials
                (new SymmetricSecurityKey(Encoding.ASCII.GetBytes("AAEDD6CAA547B2EF7F3298D1D2DC12EF7F3298D1D2DC1")),
                SecurityAlgorithms.HmacSha256Signature));

            //Act
            string jwtString = stsTokenService.GetJwtStringFromToken(token);

            //Assert
            Assert.That(string.IsNullOrEmpty(jwtString), Is.False);
            Assert.That(jwtString, Does.Match(jwtRegex));
            
        }
    }
}
