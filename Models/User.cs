namespace sts_net;

public class User
{
    public string Spotifyuserid { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public DateTime Lastlogin { get; set; }

    public virtual ICollection<Playlist> Savedplaylists { get; set; } = new List<Playlist>();

    public virtual Spotifytoken? Spotifytoken { get; set; }
}
