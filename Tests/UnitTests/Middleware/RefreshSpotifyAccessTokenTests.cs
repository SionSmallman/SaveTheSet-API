using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using SpotifyAPI.Web;
using sts_net.Data.Repositories.Interfaces;
using sts_net.Middleware;
using sts_net.Models;
using sts_net.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace sts_net.Tests.UnitTests.Middleware
{
    internal class RefreshSpotifyAccessTokenTests
    {

        private DefaultHttpContext context;
        private RefreshSpotifyAccessToken middleware;
        private Mock<ITokenRepository> tokenRepositoryMock;
        private Mock<IStsTokenService> stsTokenServiceMock;
        private Mock<IOAuthClient> oauthClientMock;
        private IConfigurationRoot config;
        [SetUp]
        public void Setup()
        {
            middleware = new RefreshSpotifyAccessToken(
                next: (innerHttpContext) =>
                {
                    return Task.CompletedTask;
                }
            );

            // Generate config variables needed for methods
            var configVariables = new Dictionary<string, string>
            {
                {"Spotify:ClientId","test" },
                {"Spotify:ClientSecret","test" },
            };

            config = new ConfigurationBuilder()
                .AddInMemoryCollection(configVariables)
                .Build();

            context = new DefaultHttpContext();

            // JWTs for test
            string exampleStsToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzcG90aWZ5VXNlcklkIjoic2lvbnNtYWxsbWFuOTgiLCJzcG90aWZ5VG9rZW4iOiJCUUJJS2xBLUMxRFpJVHQ3WEo1VHVCY1g3VlBDOS1tUzluR1c0M05jbFhyUGFtTTFpYmlRMjU0WHlBazVDeV9mM0dremo2TFpNeG1VWDc1aXNrb2VUeVVTaEdKd3BuSVY5OWZHby1zMUhvbW90dlBpSUFlbk1hbVBIdm1hQ1pFelI3UmpJUG01R0hlRGxnN2xKbEhxeEJvWms1ZFYtOEF3WmV6N081RDNKY0Y5Wi11SklwcnQ0WkdwWHZUNE5wWm1YRnJTcXlGY0pPc3FhMmtvSGU2cXB6enZ3ZVZKV2h6Mm9IWGg5dE1ZZjZxZzE2c2JDV3VWbXBnaXRLRnpJRHcyRnhDdjFuMHFSUSIsImV4cGlyZXNJbiI6IjE3MjU4MTI1OTIiLCJqdGkiOiI5YTU4YTIwMi0zYzIwLTQ5MjgtOTM0NS1hZjAxMjQzZTg1MTEiLCJuYmYiOjE3MjU4MDg5OTIsImV4cCI6MTcyNzAxODU5MiwiaWF0IjoxNzI1ODA4OTkyLCJpc3MiOiJzaW9uc21hbGxtYW4uY29tIiwiYXVkIjoic2lvbnNtYWxsbWFuLmNvbSJ9.Heom3b1tCK-43rfgvVuqy_ti-u0O8Er4dlg7o9f4cFU";

            context.Request.Headers["Authorization"] = "Bearer " + exampleStsToken;

            // Mock Setups
            tokenRepositoryMock = new Mock<ITokenRepository>();
            stsTokenServiceMock = new Mock<IStsTokenService>();
            oauthClientMock = new Mock<IOAuthClient>();

            var mockTokenClaims = new StsTokenClaims()
            {
                SpotifyUserAccessToken = "khaasklfdhaskjdfhadf.asjdfasbdfjakshdgfaskjdhfgasdf.jhasdgf7asd6fasdfasfd",
                SpotifyUserId = "savethesetdummy"
            };

            stsTokenServiceMock.Setup(x => x.GetTokenClaims(It.IsAny<string>())).Returns(mockTokenClaims);
            stsTokenServiceMock.Setup(x => x.CreateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).Returns(new JwtSecurityToken());
            stsTokenServiceMock.Setup(x => x.GetJwtStringFromToken(It.IsAny<SecurityToken>())).Returns("dshfberivnaerfv");

            var mockToken = new Spotifytoken()
            {
                Spotifyrefreshtoken = "kljhbfklajshdfaskfj"          
            };
            tokenRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(mockToken);

            var mockOauthResponse = new AuthorizationCodeRefreshResponse()
            {
                AccessToken = "jkhfbasfbasfdkas"
            };

            oauthClientMock.Setup(x => x.RequestToken(It.IsAny<AuthorizationCodeRefreshRequest>(), default)).ReturnsAsync(mockOauthResponse);

        }

        [Test]
        public async Task RefreshSpotifyAccessToken_DoesNothingIfIncorrectEndpoint() 
        {
            // Arrange
            context.Request.Method = "GET";
            context.Request.Headers.Append("Client-Access-Token-Expiry", "1");

            // Act
            await middleware.InvokeAsync(context, tokenRepositoryMock.Object, stsTokenServiceMock.Object, config, oauthClientMock.Object);

            // Assert
            Assert.That(context.Items["newStsTokenString"], Is.Null);
        }

        [Test]
        public async Task RefreshSpotifyAccessToken_RefreshesTokenIfExpired()
        {
            // Arrange
            context.Request.Method = "POST";

            //Add expired timestamp to headers
            context.Request.Headers.Append("Client-Access-Token-Expiry", "1");

            // Act
            await middleware.InvokeAsync(context, tokenRepositoryMock.Object, stsTokenServiceMock.Object, config, oauthClientMock.Object);

            // Assert
            Assert.That(context.Items["newStsTokenString"], Is.Not.Null);
            Assert.That(context.Items["newAccessTokenExpiry"], Is.Not.Null);

        }

        [Test]
        public async Task RefreshSpotifyAccessToken_DoesNothingIfNotExpired()
        {
            // Arrange
            context.Request.Method = "POST";

            // Add in-date timestamp to headers
            context.Request.Headers.Append("Client-Access-Token-Expiry", "2147483647");

            // Act
            await middleware.InvokeAsync(context, tokenRepositoryMock.Object, stsTokenServiceMock.Object, config, oauthClientMock.Object);

            // Assert
            Assert.That(context.Items["newStsTokenString"], Is.Null);
        }

    }
}
