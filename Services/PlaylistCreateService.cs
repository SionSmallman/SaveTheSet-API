using SpotifyAPI.Web;
using sts_net.Services.Interfaces;

namespace sts_net.Services
{
    public class PlaylistCreateService : IPlaylistCreateService
    {
        public PlaylistCreateService()
        {

        }

        public PlaylistCreateRequest CreatePlaylistRequestObject(string title, string description, bool isPublic, bool isCollaborative)
        {
            var pcr = new PlaylistCreateRequest(title)
            {
                Description = description + " | Created at sts.sionsmallman.com",
                Public = isPublic,
                Collaborative = isCollaborative
            };
            return pcr;
        }
    }
}
