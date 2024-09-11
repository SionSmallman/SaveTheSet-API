using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using sts_net.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace sts_net.Tests.UnitTests.Services
{
    internal class SetlistServiceTests
    {
        private Mock<IConfiguration> configMock;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private SetlistService setlistService;
        private JsonObject sampleSetlistJsonObject;

        [SetUp]
        public void Setup()
        {
            configMock = new Mock<IConfiguration>();
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            setlistService = new SetlistService(configMock.Object, httpClientFactoryMock.Object);
            // The sample setlist used is here: https://www.setlist.fm/setlist/iron-maiden/2024/rac-arena-perth-australia-43ab83cb.html
            var sampleSetlistFmApiResponse = File.ReadAllText("C:\\Users\\SionS\\source\\repos\\sts-net.Tests\\UnitTests\\Services\\SampleApiResponses\\SampleSetlistFmApiResponse.json");
            sampleSetlistJsonObject = JsonSerializer.Deserialize<JsonObject>(sampleSetlistFmApiResponse);

        }

        [Test]
        public void GetSetlist_ReturnsValidSetlistJson()
        {
            // Arrange

            // Mock the message handler to responsd with the sample SetlistFm API response
            var sampleSetlistFmApiResponse = File.ReadAllText("C:\\Users\\SionS\\source\\repos\\sts-net.Tests\\UnitTests\\Services\\SampleApiResponses\\SampleSetlistFmApiResponse.json");
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
                    Content = new StringContent(sampleSetlistFmApiResponse)
                });
            
            // Create a client which uses the message handler and setup factory to return this client.
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var setlistData = setlistService.GetSetlist("43ab83cb");

            // Assert
            var setlistDataString = JsonSerializer.Serialize<JsonObject>(setlistData);
            var sampleSetlistDataString = JsonSerializer.Serialize<JsonObject>(sampleSetlistJsonObject);
            Assert.That(setlistDataString, Is.EqualTo(sampleSetlistDataString));
        }

        [Test]
        public void GetSongTitlesFromSetlistResponse_ReturnsAllValidSongTitles()
        {
            //Arrange

            //Act
            List<String> songtitles = setlistService.GetSongTitlesFromSetlistResponse(sampleSetlistJsonObject);

            //Assert
            Assert.That(songtitles.Count, Is.EqualTo(15));
            Assert.That(songtitles[0], Is.EqualTo("Caught Somewhere in Time"));
        }
    }
}
