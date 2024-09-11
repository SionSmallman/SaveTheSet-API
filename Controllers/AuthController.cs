using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using Microsoft.AspNetCore.Authorization;
using sts_net.Services.Interfaces;
using sts_net.Data.Repositories.Interfaces;


namespace sts_net.Controllers
{
    public class AuthController : ControllerBase
    {
        // Inject services
        private readonly IConfiguration _config;
        private readonly IStsTokenService _stsTokenService;
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ISpotifyClientFactory _spotifyClientFactory;
        private readonly IOAuthClient _oAuthClient;
        public AuthController(IConfiguration config, IStsTokenService stsTokenService, IUserRepository userRepository, ITokenRepository tokenRepository, ISpotifyClientFactory spotifyClientFactory, IOAuthClient oAuthClient)
        {
            _config = config;
            _stsTokenService = stsTokenService;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _spotifyClientFactory = spotifyClientFactory;
            _oAuthClient = oAuthClient;
        }

        [Route("api/auth/login")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Build login request URL
            var loginRequest = new LoginRequest(
              new Uri(_config["ServerURL"] + "/api/auth/callback"),
              _config["Spotify:ClientId"],
              LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] { Scopes.PlaylistModifyPublic, Scopes.PlaylistModifyPrivate }
            };
            var uri = loginRequest.ToUri();
            // Return the auth url
            // Do this so the client can open a seperate pop up window for spotify auth
            // Instead of using main window and losing any progress
            return Ok(new { authUrl = uri.ToString()});
        }

        
        // After the user signs in to the Spotify popup, they are redirect back to this callback endpoint
        // with a query param code which is exchanged for the users access/refresh tokens
        [Route("api/auth/callback")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCallback(string code)
        {

            var response = await _oAuthClient.RequestToken(
              new AuthorizationCodeTokenRequest(_config["Spotify:ClientId"], _config["Spotify:ClientSecret"], code, new Uri(_config["ServerURL"] + "/api/auth/callback")
            ));

            // Get access and refresh tokens from response
            var accessToken = response.AccessToken;
            var refreshToken = response.RefreshToken;

            // Get user profile details
            var spotify = _spotifyClientFactory.CreateClient(accessToken);
            var userProfileData = await spotify.UserProfile.Current();
            var userId = userProfileData.Id;
            
            // Calculate spotify access token expiry time
            var accessTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds();

            // If returning user, just update last login date in Users table and refresh token in Spotifytokens table
            // Else, add new user to Users and new refresh token to Spotifytokens
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    await _userRepository.UpdateLastLoginAsync(userId, DateTime.Now);
                    await _tokenRepository.UpdateRefreshTokenAsync(userId, refreshToken);
                }
                else
                {
                    var newUser = new User()
                    {
                        Spotifyuserid = userId,
                        Lastlogin = DateTime.Now,
                        Createdat = DateTime.Now
                    };
                    await _userRepository.CreateAsync(newUser);
                    var newToken = new Spotifytoken()
                    {
                        Spotifyuserid = userId,
                        Spotifyrefreshtoken = refreshToken
                    };
                    await _tokenRepository.CreateAsync(newToken);
                }

                // Create the JWT
                var token = _stsTokenService.CreateToken(userId, accessToken, accessTokenExpiryTime);
                var jwtString = _stsTokenService.GetJwtStringFromToken(token);

                //Build URI and redirect
                UriBuilder uriBuild = new UriBuilder($"{_config["clientURL"]}/authredirect"!);
                uriBuild.Query = $"token={jwtString}&expiresIn={accessTokenExpiryTime}";
                return Redirect(uriBuild.Uri.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}