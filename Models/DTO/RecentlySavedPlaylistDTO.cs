namespace sts_net.Models.DTO
{
    public class RecentlySavedPlaylistDTO
    {
        public required int PlaylistId { get; set; }
        public required string ArtistName { get; set; }
        public required string SpotifyArtistId { get; set; }
        public string? ArtistImageUrl { get; set; }
        public required string Venue { get; set; }
        public required string City { get; set; }
        public required string Date { get; set; }
    }
}
