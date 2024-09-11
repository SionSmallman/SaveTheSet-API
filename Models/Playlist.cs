namespace sts_net;

// A Playlist object is the final user saved playlist.
public class Playlist
{
    public int Playlistid { get; set; }

    public string Spotifyuserid { get; set; } = null!;

    public string Spotifyartistid { get; set; } = null!;

    public string Artistname { get; set; } = null!;

    public string Venue { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Date { get; set; } = null!;

    public string Setlistfmurl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? Spotifyplaylistlink { get; set; }

    public virtual Artist Spotifyartist { get; set; } = null!;

    public virtual User Spotifyuser { get; set; } = null!;
}
