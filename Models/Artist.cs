namespace sts_net;

public class Artist
{
    public string SpotifyArtistId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public virtual ICollection<Playlist> Savedplaylists { get; set; } = new List<Playlist>();
}

