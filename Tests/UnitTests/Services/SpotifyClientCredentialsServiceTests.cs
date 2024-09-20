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
using System.Threading.Tasks;

namespace sts_net.Tests.UnitTests.Services
{
    internal class SpotifyClientCredentialsServiceTests
    {
        private SpotifyClientToken clientToken;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private SpotifyClientCredentialsService service;
        private string searchForSongSampleResponse;
        private string searchForArtistSampleResponse;


        [SetUp]
        public void Setup()
        {
            clientToken = new SpotifyClientToken()
            {
                Token = "testtoken12345doesntmatterlol",
                Expiry = DateTime.UtcNow.AddDays(1),
            };
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            service = new SpotifyClientCredentialsService(clientToken, httpClientFactoryMock.Object);
            // Data for sample responses: Artist - Holding Absence, Song - Gravity
            searchForSongSampleResponse = File.ReadAllText("C:\\Users\\SionS\\source\\repos\\sts-net\\Tests\\UnitTests\\Services\\SampleApiResponses\\SpotifyApiSearchForSongResponse.json");
            searchForArtistSampleResponse = File.ReadAllText("C:\\Users\\SionS\\source\\repos\\sts-net\\Tests\\UnitTests\\Services\\SampleApiResponses\\SpotifyApiSearchForArtistResponse.json");
        }

        [Test]
        public async Task SearchForSong_ReturnsValidSongObject()
        {
            // Arrange
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
                    Content = new StringContent(searchForSongSampleResponse)
                });


            // Create a client which uses the message handler and setup factory to return this client.
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var song = await service.SearchForSong("Holding Absence", "Gravity");

            // Assert
            Assert.That(song, Is.TypeOf<Song>());
            Assert.That(song.SongTitle, Is.EqualTo("Gravity"));
            Assert.That(song.Artist, Is.EqualTo("Holding Absence"));
        }

        [Test]
        public async Task GetArtistDetails_ReturnsValidArtistObject()
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
                    Content = new StringContent(searchForArtistSampleResponse)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var artist = await service.GetArtistDetails("Holding Absence");

            // Assert
            Assert.That(artist, Is.TypeOf<Artist>());
            Assert.That(artist.Name, Is.EqualTo("Holding Absence"));
        }

    }
}
