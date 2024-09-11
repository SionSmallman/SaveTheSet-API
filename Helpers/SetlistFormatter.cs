using sts_net.Helpers.Interfaces;
using sts_net.Models;
using System.Globalization;
using System.Text.Json.Nodes;

namespace sts_net.Helpers
{

    // This is a helper class for reshaping and formatting the JSON responses from setlistfm/spotify

    public class SetlistFormatter : ISetlistFormatter
    {

        public SetlistFormatter()
        {
        }

        public Song BuildSongObjectFromSpotifyData(JsonNode spotifyApiReturnData)
        {
            // Some songs have multple artists (features etc)
            List<string> artistNameArray = new List<string>();
            foreach (var artist in spotifyApiReturnData["tracks"]["items"][0]["artists"].AsArray())
            {
                artistNameArray.Add(artist["name"].ToString());
            }
            var artistString = String.Join(" & ", artistNameArray);

            Song song = new Song();
            song.Uri = (string)spotifyApiReturnData["tracks"]["items"][0]["uri"];
            song.SongTitle = (string)spotifyApiReturnData["tracks"]["items"][0]["name"];
            song.Artist = artistString;
            song.Album = (string)spotifyApiReturnData["tracks"]["items"][0]["album"]["name"];
            song.AlbumArtUrl = (string)spotifyApiReturnData["tracks"]["items"][0]["album"]["images"][0]["url"];

            return song;
        }

        public Setlist BuildSetlist(Artist artistData, string venue, string city, string date, string setlistFmUrl, List<Song> songList)
        {
            var setlist = new Setlist();
            setlist.Artist = artistData;
            setlist.Venue = venue;
            setlist.City = city;
            setlist.Date = DateOnly.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            setlist.SetlistFMUrl = setlistFmUrl;
            setlist.SongList = songList;
            return setlist;
        }


    }
}
