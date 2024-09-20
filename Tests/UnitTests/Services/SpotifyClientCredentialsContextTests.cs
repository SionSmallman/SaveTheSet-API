using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using sts_net.Models;
using sts_net.Services;
using sts_net.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace sts_net.Tests.UnitTests.Services
{
    internal class SpotifyClientCredentialsContextTests
    {
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private SpotifyClientCredentialsContext spotifyClientCredentialsContext;
        private string clientCredentialSampleResponse;

        [SetUp]
        public void Setup()
        {
            // Generate config variables needed for methods
            var configVariables = new Dictionary<string, string>
            {
                {"Spotify:ClientId","test" },
                {"Spotify:ClientSecret","test" },
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configVariables)
                .Build();

            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            spotifyClientCredentialsContext = new SpotifyClientCredentialsContext(config, httpClientFactoryMock.Object);
            clientCredentialSampleResponse = File.ReadAllText("C:\\Users\\SionS\\source\\repos\\sts-net\\Tests\\UnitTests\\Services\\SampleApiResponses\\SpotifyApiClientCredentialsResponse.json");
        }

        [Test]
        public async Task GetTokenAsync_GeneratesNewTokenIfNoneExist()
        {
            // Arrange

            // Mock the message handler to responsd with the sample SetlistFm API response
            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(clientCredentialSampleResponse)
                });

            // Create a client which uses the message handler and setup factory to return this client.
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var clientCredentialsToken = await spotifyClientCredentialsContext.GetTokenAsync();

            // Assert
            Assert.That(clientCredentialsToken, Is.TypeOf<SpotifyClientToken>());
            Assert.That(String.IsNullOrEmpty(clientCredentialsToken.Token), Is.False);

        }

        [Test]
        public async Task GetTokenAsync_ReturnsCurrentTokenIfOneExists()
        {
            // Arrange

            var existingToken = new SpotifyClientToken()
            {
                Token = "kjsdhfaskdljfha.asdfasfasdfa.asdfgerg3eegr",
                Expiry = DateTime.UtcNow.AddDays(1),
            };

            spotifyClientCredentialsContext._clientAccessToken = existingToken;

            // Act
            var returnedToken = await spotifyClientCredentialsContext.GetTokenAsync();

            // Assert
            Assert.That(returnedToken, Is.TypeOf<SpotifyClientToken>());
            Assert.That(returnedToken.Token, Is.EqualTo(existingToken.Token));
            Assert.That(returnedToken.Expiry, Is.EqualTo(existingToken.Expiry));
        }
    }
}
