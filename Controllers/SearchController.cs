using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using sts_net.Helpers;
using sts_net.Models;
using sts_net.Services;
using sts_net.Services.Interfaces;
using System.Text.Json.Nodes;

namespace sts_net.Controllers
{
    public class SearchController : ControllerBase
    {
        // Get Spotify Client instance
        private readonly IConfiguration _config;
        private readonly ISpotifyClientCredentialsContext _spotifyClientCredentialsContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISpotifyClientCredentialsServiceFactory _spotifyClientCredentialsServiceFactory;
        private readonly ISetlistService _setlistService;
        

        // Inject instance into controller
        public SearchController(IConfiguration config, IHttpClientFactory httpClientFactory, ISpotifyClientCredentialsContext spotifyClientCredentialsContext, ISpotifyClientCredentialsServiceFactory spotifyClientCredentialsServiceFactory, ISetlistService setlistService)
        {
            _config = config;
            _spotifyClientCredentialsContext = spotifyClientCredentialsContext;
            _httpClientFactory = httpClientFactory;
            _spotifyClientCredentialsServiceFactory = spotifyClientCredentialsServiceFactory;
            _setlistService = setlistService;
        }

        [Route("api/search")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchForSetlistSongs([BindRequired] string id)
        {
            SpotifyClientToken clientToken = await _spotifyClientCredentialsContext.GetTokenAsync();
            var spotifyClientCredentialsService = _spotifyClientCredentialsServiceFactory.CreateService(clientToken, _httpClientFactory);

            JsonObject setlistServiceResponse;
            try
            {
                setlistServiceResponse = _setlistService.GetSetlist(id);
            }
            catch (HttpRequestException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return NotFound(new { error = "ID does not match any setlist" });
                }
                return StatusCode(502, new { error = "Error fetching setlist data. SetlistFM API may be unavailable"});
            }

            // Get metadata from setlistfm
            // Todo: extract to POCO
            var artistName = setlistServiceResponse["artist"]["name"].ToString();
            var venueName = setlistServiceResponse["venue"]["name"].ToString();
            var cityName = setlistServiceResponse["venue"]["city"]["name"].ToString();
            var date = setlistServiceResponse["eventDate"].ToString();
            var setlistFmUrl = setlistServiceResponse["url"].ToString();

            //Setlists on setlist.fm are broken up into "sections" e.g main setlist, encore etc, so loop to catch all song from all sections.
            var songTitles = _setlistService.GetSongTitlesFromSetlistResponse(setlistServiceResponse);

            // Get artist details, return NotFound if we cant find artist on Spotify
            var artistData = await spotifyClientCredentialsService.GetArtistDetails(artistName);
            if (artistData == null)
            {
                return NotFound(new { error = "Cannot find artist on Spotify" });
            }

            // Get songs that are available on spotify
            var (foundSongs, errorSongs) = await spotifyClientCredentialsService.GetSongsAvailableOnSpotify(songTitles, artistName);
            if (foundSongs.Count == 0)
            {
                return NotFound(new { error = "Cannot find any songs from this setlist on Spotify" });
            }

            var setlistFormatter = new SetlistFormatter(); // extract dependency
            var returnSetlist = setlistFormatter.BuildSetlist(artistData, venueName, cityName, date, setlistFmUrl, foundSongs);
            return Ok(returnSetlist);



        }
    }
}