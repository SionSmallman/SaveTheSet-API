using SpotifyAPI.Web;
using sts_net.Helpers;
using sts_net.Services;

namespace sts_net.Tests.UnitTests.Helpers
{
    internal class PlaylistCreateServiceTests
    {
        private PlaylistCreateService playlistCreateService;
        [SetUp]
        public void Setup()
        {

            playlistCreateService = new PlaylistCreateService();
        }

        [Test]
        public void CreatePlaylistRequestObject_EqualTest()
        {
            

            //Arrange
            string title = "Title";
            string description = "Description";
            bool isPublic = true;
            bool isCollaborative = true;

            var testPcr = new PlaylistCreateRequest(title)
            {
                Description = description,
                Public = isPublic,
                Collaborative = isCollaborative
            };

            //Act
            PlaylistCreateRequest pcr = playlistCreateService.CreatePlaylistRequestObject(title, description, isPublic, isCollaborative);

            //Assert

            Assert.That(testPcr.Name, Is.EqualTo(pcr.Name));
            Assert.That(testPcr.Description + " | Created at sts.sionsmallman.com", Is.EqualTo(pcr.Description)); //check watermark is added
            Assert.That(testPcr.Public, Is.EqualTo(pcr.Public));
            Assert.That(testPcr.Collaborative, Is.EqualTo(pcr.Collaborative));
        }

    }
}
