namespace sts_net;

public class Spotifytoken
{
    public string Spotifyuserid { get; set; } = null!;

    public string Spotifyrefreshtoken { get; set; } = null!;

    public virtual User Spotifyuser { get; set; } = null!;
}
