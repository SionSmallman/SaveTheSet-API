using sts_net.Models;

namespace sts_net.Services.Interfaces
{
    public interface ISpotifyClientCredentialsContext
    {
        SpotifyClientToken _clientAccessToken { get; set; }

        Task<SpotifyClientToken> GetTokenAsync();
    }
}