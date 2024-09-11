using SpotifyAPI.Web;

namespace sts_net.Services.Interfaces
{
    public interface ISpotifyClientFactory
    {
        SpotifyClient CreateClient(string spotifyAccessToken);
    }
}