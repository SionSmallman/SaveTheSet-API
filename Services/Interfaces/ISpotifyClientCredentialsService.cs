using sts_net.Models;

namespace sts_net.Services.Interfaces
{
    public interface ISpotifyClientCredentialsService

    {
        public Task<Song> SearchForSong(string songTitle, string artistName);
        public Task<Artist> GetArtistDetails(string artistName);
    }
}
