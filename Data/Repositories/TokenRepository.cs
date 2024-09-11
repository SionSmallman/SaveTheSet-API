using Microsoft.EntityFrameworkCore;
using sts_net.Data.Repositories.Interfaces;

namespace sts_net.Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        private readonly SaveTheSetContext _context;
        public TokenRepository(SaveTheSetContext context)
        {
            _context = context;
        }


        public async Task<Spotifytoken> CreateAsync(Spotifytoken token)
        {
            await _context.Spotifytokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<Spotifytoken?> DeleteAsync(string spotifyUserId)
        {
            var token = await _context.Spotifytokens.SingleOrDefaultAsync(x => x.Spotifyuserid == spotifyUserId);
            if (token == null)
            {
                return null;
            }
            _context.Spotifytokens.Remove(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public Task<List<Spotifytoken>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Spotifytoken?> GetByIdAsync(string spotifyUserId)
        {
            var token = await _context.Spotifytokens.SingleOrDefaultAsync(x => x.Spotifyuserid == spotifyUserId);
            if (token == null)
            {
                return null;
            }
            return token;
        }

        public async Task<Spotifytoken> UpdateRefreshTokenAsync(string spotifyUserId, string newRefreshToken)
        {
            Spotifytoken token = await _context.Spotifytokens.SingleOrDefaultAsync(x => x.Spotifyuserid == spotifyUserId);
            if (token == null)
            {
                return null;
            };
            token.Spotifyrefreshtoken = newRefreshToken;
            await _context.SaveChangesAsync();
            return token;
        }
    }
}
