namespace sts_net.Models;

public class Setlist {
    public Artist Artist { get; set; }
    public string Venue { get; set; }
    public string City { get; set; }
    public DateOnly Date { get; set; }
    public string SetlistFMUrl {  get; set; }
    public List<Song> SongList { get; set; }
}
