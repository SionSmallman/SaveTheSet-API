using Microsoft.IdentityModel.Tokens;
using sts_net.Models;
using sts_net.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace sts_net.Services
{

    // STS Token is the JWT used for validating with the API.
    // The token is NOT the token used for accessing the Spotify User API.
    public class StsTokenService: IStsTokenService
    {
        private readonly IConfiguration _config;

        public StsTokenService( IConfiguration config)
        {
            _config = config;
        }

        public StsTokenClaims GetTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonTokenContents = handler.ReadToken(token) as JwtSecurityToken;
            var claims = new StsTokenClaims();
            claims.SpotifyUserAccessToken = jsonTokenContents.Claims.First(claim => claim.Type == "spotifyToken").Value;
            claims.SpotifyUserId = jsonTokenContents.Claims.First(claim => claim.Type == "spotifyUserId").Value;
            return claims;
        }

        
        // Creates a STS Token for authentication with this API.
        // Token contains the users Spotify id, access token and token expiry time.
        
        public SecurityToken CreateToken(string spotifyUserId, string spotifyAccessToken, long spotifyAccessTokenExpiryTime)
        {

            // Get token params from config file
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            
            // Set token claims and create descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("spotifyUserId", spotifyUserId),
                        new Claim("spotifyToken", spotifyAccessToken),
                        new Claim("expiresIn", spotifyAccessTokenExpiryTime.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }),
                Expires = DateTime.UtcNow.AddDays(14),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            
            // Create token from descriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return token;
        }

        // Refresh the token if expired
        public SecurityToken RefreshExpiredStsToken(SecurityToken expiredToken)
        {
            return null;
        }

        // Gets Jwt string from Token object
        public string GetJwtStringFromToken(SecurityToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtString = tokenHandler.WriteToken(token);
            return jwtString;
        }

    }
}
