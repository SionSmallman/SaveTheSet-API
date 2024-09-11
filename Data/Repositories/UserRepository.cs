using Microsoft.EntityFrameworkCore;
using sts_net.Data.Repositories.Interfaces;

namespace sts_net.Data.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly SaveTheSetContext _context;
        public UserRepository(SaveTheSetContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteAsync(string spotifyUserId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Spotifyuserid == spotifyUserId);
            if (user == null)
            {
                return null;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(string spotifyUserId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Spotifyuserid == spotifyUserId);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<User?> UpdateLastLoginAsync(string spotifyUserId, DateTime lastLoginDateTime)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Spotifyuserid == spotifyUserId);
            if (user == null)
            {
                return null;
            };
            user.Lastlogin = lastLoginDateTime;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
