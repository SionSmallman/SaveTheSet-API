using sts_net.Models;
using Microsoft.IdentityModel.Tokens;

namespace sts_net.Services.Interfaces
{
    public interface IStsTokenService
    {
        public SecurityToken CreateToken(string spotifyUserId, string spotifyAccessToken, long spotifyAccessTokenExpiryTime);

        public StsTokenClaims GetTokenClaims(string token);

        public string GetJwtStringFromToken(SecurityToken token);
    }
}
