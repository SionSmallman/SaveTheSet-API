using sts_net.Helpers;
using sts_net.Models;
using sts_net.Services.Interfaces;
using System.Text.Json.Nodes;

namespace sts_net.Services
{
    // Class used to call API using Client Credentials flow
    // This is used for the search functionality and means we can search Spotify for matching songs before the user authenticates
    // More info: https://developer.spotify.com/documentation/web-api/tutorials/client-credentials-flow
    public class SpotifyClientCredentialsService : ISpotifyClientCredentialsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SpotifyClientToken _clientToken;
        public SpotifyClientCredentialsService(SpotifyClientToken clientToken, IHttpClientFactory httpClientFactory) 
        { 
            _clientToken = clientToken;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Song> SearchForSong(string songTitle, string artistName)
        {
            // Instantiate Song formatter for later
            var setlistFormatter = new SetlistFormatter();

            var httpMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://api.spotify.com/v1/search?q=artist%3A{artistName}+track%3A{songTitle}&type=track&limit=1");
            httpMessage.Headers.Add("Authorization", "Bearer " + _clientToken.Token);
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(httpMessage);
                response.EnsureSuccessStatusCode();
                var songJsonString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonNode.Parse(songJsonString)!.AsObject();
                var itemArray = responseObject["tracks"]["items"].AsArray();
                // If song cant be found, add to error list
                if (itemArray.Count != 0)
                {
                    // Song is valid, take response data and build Song object, then add to array
                    var song = setlistFormatter.BuildSongObjectFromSpotifyData(responseObject);
                    return song;
                }
                return null;

            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }

        // Searches for artist on Spotify API.
        // If artist found, returns new Artist object containing SpotifyID, Name and ImageUrl.
        // If not found, return null
        public async Task<Artist> GetArtistDetails(string artistName)
        {
            var httpMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://api.spotify.com/v1/search?q=artist%3A{artistName}&type=artist&limit=1");
            httpMessage.Headers.Add("Authorization", "Bearer " + _clientToken.Token);
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(httpMessage);
                response.EnsureSuccessStatusCode();
                var artistJsonString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonNode.Parse(artistJsonString)!.AsObject();
                if (responseObject["artists"]["items"].AsArray().Count > 0)
                {
                    var artist = new Artist();
                    artist.SpotifyArtistId = responseObject["artists"]["items"][0]["id"].ToString();
                    artist.Name = artistName;
                    artist.ImageUrl = responseObject["artists"]["items"][0]["images"][0]["url"].ToString();
                    return artist;
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }

        public async Task<(List<Song> foundSongs, List<string> errorSongs)> GetSongsAvailableOnSpotify(List<string> songTitles, string artistName)
        {
            var foundSongs = new List<Song>();
            var errorSongs = new List<string>();
            foreach (var songTitle in songTitles)
            {
                var song = await SearchForSong(songTitle, artistName);
                if (song == null)
                {
                    errorSongs.Add(songTitle);
                }
                else
                {
                    foundSongs.Add(song);
                }
            }
            return (foundSongs,errorSongs);
        }
    }
}
