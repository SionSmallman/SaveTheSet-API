using sts_net.Models;
using System.Text.Json.Nodes;

namespace sts_net.Helpers.Interfaces
{
    public interface ISetlistFormatter
    {
        public Song BuildSongObjectFromSpotifyData(JsonNode spotifyApiReturnData);
        public Setlist BuildSetlist(Artist artistData, string venue, string city, string date, string setlistFmUrl, List<Song> songList);
    }
}