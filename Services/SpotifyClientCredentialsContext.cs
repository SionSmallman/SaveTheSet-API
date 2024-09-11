using Microsoft.Net.Http.Headers;
using sts_net.Models;
using sts_net.Services.Interfaces;
using System.Text.Json.Nodes;

namespace sts_net.Services;
public class SpotifyClientCredentialsResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}
// Context created on startup to generate an initial access token for the client credentials flow
// This is used for the search functionality and means we can search Spotify for matching songs before the user authenticates
// More info: https://developer.spotify.com/documentation/web-api/tutorials/client-credentials-flow
public class SpotifyClientCredentialsContext : ISpotifyClientCredentialsContext
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public SpotifyClientToken _clientAccessToken { get; set; }

    // Create Client, get initial spotify clearence
    public SpotifyClientCredentialsContext(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    private Task<SpotifyClientToken> FetchNewTokenAsync()
        => Task.Run(() =>
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{_config["Spotify:ClientId"]}:{_config["Spotify:ClientSecret"]}");
            var clientBase64 = Convert.ToBase64String(plainTextBytes);

            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Post, "https://accounts.spotify.com/api/token"
                )
            {
                Headers =
                        {
                            { HeaderNames.Authorization, "Basic " + clientBase64 },
                        }
            };
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            };
            httpRequestMessage.Content = new FormUrlEncodedContent(formData);

            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                var response = client.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var readAsStringAsync = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var responseObj = JsonNode.Parse(readAsStringAsync)!;

                // Configuring token manually instead of deserialising the reponse object
                // so we can keep track of expiry time
                var token = new SpotifyClientToken();
                token.Token = (string)responseObj["access_token"]!;
                DateTime currentDateTime = DateTime.Now;
                token.Expiry = currentDateTime.Add(new TimeSpan(1, 0, 0));
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        });

    public async Task<SpotifyClientToken> GetTokenAsync()
    {
        // If a token already exists and is not expired, return our existing token
        if (_clientAccessToken != null && _clientAccessToken.Expiry > DateTime.Now)
        {
            return _clientAccessToken;
        }
        _clientAccessToken = await FetchNewTokenAsync();
        return _clientAccessToken;
    }


}
