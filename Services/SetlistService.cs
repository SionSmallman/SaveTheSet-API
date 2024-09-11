using System.Text.Json.Nodes;
using sts_net.Services.Interfaces;

namespace sts_net.Services
{
    // Class used for interactions with SetlistFM API
    public class SetlistService : ISetlistService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public SetlistService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public JsonObject GetSetlist(string setlistId )
        {
            var httpRequestMessage = new HttpRequestMessage(
               HttpMethod.Get, $"https://api.setlist.fm/rest/1.0/setlist/{setlistId}"
               );
            httpRequestMessage.Headers.Add("x-api-key", _config["SetlistFmApiKey"]);
            httpRequestMessage.Headers.Add("Accept", "application/json");
            JsonObject responseObj;
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                var response = client.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var setlistJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                responseObj = JsonNode.Parse(setlistJsonString)!.AsObject();
                return responseObj;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }

        //Setlists on setlist.fm are broken up into "sections" e.g main setlist, encore etc, so loop to catch all song from all sections.
        public List<String> GetSongTitlesFromSetlistResponse(JsonObject setlistServiceResponse)
        {
            var songTitles = new List<String>();
            var sections = setlistServiceResponse["sets"]["set"].AsArray();
            foreach (var section in sections)
            {
                foreach (var song in section["song"].AsArray())
                {
                    var songObj = song.AsObject();

                    // Ignore tape songs
                    if (songObj.ContainsKey("tape") && songObj["tape"].ToString() == "true")
                    {
                        continue;
                    }

                    songTitles.Add(songObj["name"].ToString());
                }
            }

            return songTitles;
        }
    }
}
