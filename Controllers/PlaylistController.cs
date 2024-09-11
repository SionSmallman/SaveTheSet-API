using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SpotifyAPI.Web;
using sts_net.Data.Repositories.Interfaces;
using sts_net.Models.DTO;
using sts_net.Services;
using sts_net.Services.Interfaces;

namespace sts_net.Controllers
{
    public class PlaylistController : ControllerBase
    {
        // Get Dependencies
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IStsTokenService _stsTokenService;
        private readonly ISpotifyClientFactory _spotifyClientFactory;

        // Inject DB instance into controller
        public PlaylistController(IPlaylistRepository playlistRepository, IArtistRepository artistRepository, IStsTokenService stsTokenService, ISpotifyClientFactory spotifyClientFactory)
        {
            _playlistRepository = playlistRepository;
            _artistRepository = artistRepository;
            _stsTokenService = stsTokenService;
            _spotifyClientFactory = spotifyClientFactory;
        }
        [Route("api/playlists/recent")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetRecentlySavedSetlists([BindRequired] int limit = 3)
        {
            var playlists = _playlistRepository.GetRecentPlaylists(limit);
            return playlists == null ? NotFound() : Ok(playlists);
        }
        
        [Route("api/playlists")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistRequestDTO playlistFormData)
        {
            // If we've recieved a new STS Token from previous middleware, use that.
            // If not, use the token from the request header.
            string stsToken;
            if (Request.HttpContext.Items["newStsTokenString"] != null)
            {
                stsToken = (string)Request.HttpContext.Items["newStsTokenString"]!;
            }
            else
            {
                stsToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
            }


            var claims = _stsTokenService.GetTokenClaims(stsToken);
            var spotify = _spotifyClientFactory.CreateClient(claims.SpotifyUserAccessToken);

            // Create the playlist and fill selected songs.
            var playlistCreateService = new PlaylistCreateService(); // extract dependency
            var playlistCreateRequest = playlistCreateService.CreatePlaylistRequestObject(playlistFormData.Title, playlistFormData.Description, playlistFormData.IsPublic, false);
            try
            {

                var CreatedPlaylist = await spotify.Playlists.Create(claims.SpotifyUserId, playlistCreateRequest);
                
                // Get selected songs and add them to our created playlist
                List<string> songUriList = playlistFormData.SongList.Select(s => s.Uri).ToList(); 
                var addItemsRequest = new PlaylistAddItemsRequest(songUriList);
                await spotify.Playlists.AddItems(CreatedPlaylist.Id, addItemsRequest);

                // If artist hasn't been seen before, save all artist details to DB
                // Otherwise, check if stored artist image is out-of-date
                var existingArtist = await _artistRepository.GetByIdAsync(playlistFormData.Artist.SpotifyArtistId);
                if (existingArtist == null)
                {
                    await _artistRepository.CreateAsync(playlistFormData.Artist);
                } else
                {   
                    // Update artist image if stored one is out of date.
                    if (existingArtist.ImageUrl != playlistFormData.Artist.ImageUrl) 
                    {
                        await _artistRepository.UpdateImageAsync(existingArtist.SpotifyArtistId, playlistFormData.Artist.ImageUrl);
                    }
                    
                }

                // Initialise playlist data object, add data then save to DB
                var playlist = new Playlist() {   
                Spotifyuserid = claims.SpotifyUserId,
                Spotifyartistid = playlistFormData.Artist.SpotifyArtistId,
                Artistname = playlistFormData.Artist.Name,
                Venue = playlistFormData.Venue,
                City = playlistFormData.City,
                Date = playlistFormData.Date.ToString(),
                Setlistfmurl = playlistFormData.SetlistFmUrl.ToString(),
                CreatedAt = DateTime.Now,
                Spotifyplaylistlink = CreatedPlaylist.ExternalUrls["spotify"]
                };

                await _playlistRepository.CreateAsync(playlist);

                return Ok(new
                {
                    playlistURL = CreatedPlaylist.ExternalUrls["spotify"],
                    playlistID = CreatedPlaylist.Id,
                    newStsToken = Request.HttpContext.Items["newStsTokenString"] != null 
                    ? stsToken 
                    : null,
                    newSpotifyAccessTokenExpiry = Request.HttpContext.Items["newAccessTokenExpiry"] != null 
                    ? Request.HttpContext.Items["newAccessTokenExpiry"] 
                    : null,
                }) ;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/playlists")]
        [HttpGet]
        [Authorize]
        public IActionResult GetPlaylistsByUser([BindRequired] string id)
        {

            return Ok();
        }
    }
}
