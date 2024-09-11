using sts_net.Models;

namespace sts_net.Services.Interfaces
{
    public interface ISpotifyClientCredentialsServiceFactory
    {
        SpotifyClientCredentialsService CreateService(SpotifyClientToken clientToken, IHttpClientFactory httpClientFactory);
    }
}