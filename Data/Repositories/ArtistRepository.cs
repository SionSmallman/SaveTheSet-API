using Microsoft.EntityFrameworkCore;
using sts_net.Data.Repositories.Interfaces;

namespace sts_net.Data.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly SaveTheSetContext _context;
        public ArtistRepository(SaveTheSetContext context)
        {
            _context = context;
        }

        public async Task<Artist> CreateAsync(Artist artist)
        {
            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
            return artist;
        }

        public async Task<Artist?> DeleteAsync(string spotifyArtistId)
        {
            var artist = await _context.Artists.SingleOrDefaultAsync(x => x.SpotifyArtistId == spotifyArtistId);
            if (artist == null)
            {
                return null;
            }
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return artist;
        }

        public async Task<List<Artist>> GetAllAsync()
        {
            return await _context.Artists.ToListAsync();
        }

        public async Task<Artist?> GetByIdAsync(string spotifyArtistId)
        {
            var artist = await _context.Artists.SingleOrDefaultAsync(x => x.SpotifyArtistId == spotifyArtistId);
            if (artist == null)
            {
                return null;
            };
            return artist;
        }

        public async Task<Artist> UpdateImageAsync(string spotifyArtistId, string imageUrl)
        {
            var artist = await _context.Artists.SingleOrDefaultAsync(x => x.SpotifyArtistId == spotifyArtistId);
            if (artist == null)
            {
                return null;
            };
            artist.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
            return artist;
        }
    }
}
