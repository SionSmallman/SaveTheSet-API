using SpotifyAPI.Web;
using sts_net.Services.Interfaces;

namespace sts_net.Services
{
    public class SpotifyClientFactory : ISpotifyClientFactory
    {
        public SpotifyClientFactory() { }

        public SpotifyClient CreateClient(string spotifyAccessToken)
        {
            return new SpotifyClient(spotifyAccessToken);
        }
    }
}
