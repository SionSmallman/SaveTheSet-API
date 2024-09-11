namespace sts_net.Models
{
    // Model for Spotify Client Crentials flow
    public class SpotifyClientToken
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
