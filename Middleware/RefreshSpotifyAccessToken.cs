using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Web;
using sts_net.Data.Repositories.Interfaces;
using sts_net.Middleware;
using sts_net.Services.Interfaces;
using System.Diagnostics;

namespace sts_net.Middleware
{
    public class RefreshSpotifyAccessToken
    {
   
        private readonly RequestDelegate _next;

        public RefreshSpotifyAccessToken(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenRepository tokenRepository, IStsTokenService stsTokenService, IConfiguration config, IOAuthClient oAuthClient)
        {
            // Get token expiry time from request headers, then check to see if token has expired.
            // If YES, grab refresh token from database and create new token, attaching it to the request.
            // If NO, continue request.

            // If the request isn't a post request on the playlists endpoint (i.e the request isn't a save playlist request), ignore it.
            if (context.Request.Method != "POST")
            {
                await _next(context);
                return;
            }
            Debug.WriteLine("Refresh Middleware fired");
            context.Request.Headers.TryGetValue("Client-Access-Token-Expiry", out StringValues spotifyAccessTokenExpiryTime);
            int accessTokenExpiryTime = Int32.Parse(spotifyAccessTokenExpiryTime[0]);


            // Get current time in unix timestamp
            DateTime utcNow = DateTime.UtcNow;
            DateTimeOffset utcOffset = new DateTimeOffset(utcNow);
            long unixTimestampNow = utcOffset.ToUnixTimeSeconds();

            if (accessTokenExpiryTime < unixTimestampNow ) 
            {
                Debug.WriteLine("Refreshing Spotify Access Token");

                var jwt = context.Request.Headers["Authorization"].ToString().Split(" ").Last();
                var claims = stsTokenService.GetTokenClaims(jwt);
                var spotifyUserId = claims.SpotifyUserId;

                // Get refresh token from DB
                var dbTokenObject = await tokenRepository.GetByIdAsync(spotifyUserId);
                string refreshToken = dbTokenObject.Spotifyrefreshtoken;

                // Get new token
                AuthorizationCodeRefreshResponse response = await oAuthClient.RequestToken(
                new AuthorizationCodeRefreshRequest(config["Spotify:ClientId"], config["Spotify:ClientSecret"], refreshToken), default);

                // Create new StsToken
                var newAccessTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds();
                SecurityToken newStsToken = stsTokenService.CreateToken(spotifyUserId, response.AccessToken, newAccessTokenExpiryTime);
                string newTokenJwtString = stsTokenService.GetJwtStringFromToken(newStsToken);

                // Attach new JWT to request
                context.Items["newStsTokenString"] = newTokenJwtString;
                context.Items["newAccessTokenExpiry"] = newAccessTokenExpiryTime;

                // If response contained new refresh token, update database
                if (response.RefreshToken != null)
                {
                    await tokenRepository.UpdateRefreshTokenAsync(claims.SpotifyUserId, response.RefreshToken);
                }
            }
            await _next(context);
        }
    }
}

// Expose middleware
public static class RefreshSpotifyTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseRefreshSpotifyToken(this IApplicationBuilder builder)
    {

        return builder.UseMiddleware<RefreshSpotifyAccessToken>();
    }
}