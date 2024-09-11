using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sts_net.Data.Repositories.Interfaces;
using sts_net.Models.DTO;
using sts_net.Services.Interfaces;

namespace sts_net.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISpotifyClientFactory _spotifyClientFactory;
        private readonly IStsTokenService _stsTokenService;
        
        public UserController(IUserRepository userRepository, ISpotifyClientFactory spotifyClientFactory, IStsTokenService stsTokenService) 
        { 
            _userRepository = userRepository;
            _spotifyClientFactory = spotifyClientFactory;
            _stsTokenService = stsTokenService;
        }

        [Route("api/user/profile")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfileDetails()
        {
            var stsToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
            var claims = _stsTokenService.GetTokenClaims(stsToken);

            // Get profile data from spotify
            var spotify = _spotifyClientFactory.CreateClient(claims.SpotifyUserAccessToken);
            var userProfileData = await spotify.UserProfile.Current();

            // Create return object
            var returnObj = new SpotifyUserProfileDTO()
            {
                UserId = userProfileData.Id,
                DisplayName = userProfileData.DisplayName,
                ProfileImageUrl = userProfileData.Images.Count != 0 ? userProfileData.Images[0].Url : null,
                ProfileUrl = userProfileData.ExternalUrls["spotify"],
            };
            return Ok(returnObj);
        }

        [Route("api/user/{spotifyId}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string spotifyId)
        {
            var stsToken = Request.Headers["Authorization"].ToString().Split(" ").Last();
            var claims = _stsTokenService.GetTokenClaims(stsToken);

            var jwtSpotifyId = claims.SpotifyUserId;

            // If verified JWT does not contain the same spotify ID as the ID in params, throw 403
            // This stops a user from being able to delete other peoples data using their own token and a 3rd party spotify ID 
            if (spotifyId != jwtSpotifyId) 
            { 
                return Forbid();
            }

            //Get user and remove from database
            var user = await _userRepository.DeleteAsync(jwtSpotifyId);

            if (user != null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
