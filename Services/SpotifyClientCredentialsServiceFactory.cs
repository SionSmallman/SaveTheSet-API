using sts_net.Models;
using sts_net.Services.Interfaces;

namespace sts_net.Services
{
    //Horrible filename
    public class SpotifyClientCredentialsServiceFactory : ISpotifyClientCredentialsServiceFactory
    {
        public SpotifyClientCredentialsServiceFactory() { }

        public SpotifyClientCredentialsService CreateService(SpotifyClientToken clientToken, IHttpClientFactory httpClientFactory)
        {
            return new SpotifyClientCredentialsService(clientToken, httpClientFactory);
        }
    }
}
