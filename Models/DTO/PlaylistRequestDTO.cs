namespace sts_net.Models.DTO
{
    public class PlaylistRequestDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public Artist Artist { get; set; }

        public string Venue { get; set; }

        public string City { get; set; }

        public DateOnly Date {  get; set; }

        public Uri SetlistFmUrl { get; set; }

        public List<Song> SongList { get; set; }


    }
}
