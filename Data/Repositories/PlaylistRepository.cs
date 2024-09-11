using Microsoft.EntityFrameworkCore;
using sts_net.Data.Repositories.Interfaces;
using sts_net.Models.DTO;

namespace sts_net.Data.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {

        private readonly SaveTheSetContext _context;
        public PlaylistRepository(SaveTheSetContext context)
        {
            _context = context;
        }


        public async Task<Playlist> CreateAsync(Playlist playlist)
        {
            await _context.Savedplaylists.AddAsync(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<Playlist?> DeleteAsync(string playlistId)
        {
            var playlist = await _context.Savedplaylists.SingleOrDefaultAsync(x => x.Playlistid.ToString() == playlistId);
            if (playlist == null)
            {
                return null;
            }
            _context.Savedplaylists.Remove(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<List<Playlist>> GetAllAsync()
        {
            return await _context.Savedplaylists.ToListAsync();
        }

        public async Task<Playlist?> GetByIdAsync(string playlistId)
        {
            var playlist = await _context.Savedplaylists.SingleOrDefaultAsync(x => x.Playlistid.ToString() == playlistId);
            if (playlist == null)
            {
                return null;
            }
            return playlist;
        }

        public Task<Playlist> UpdateAsync(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public List<RecentlySavedPlaylistDTO>? GetRecentPlaylists(int limit)
        {
            var playlists = _context.Savedplaylists.Join(_context.Artists,
               playlist => playlist.Spotifyartistid, artist => artist.SpotifyArtistId,
               (playlist, artist) => new RecentlySavedPlaylistDTO()
               {
                   PlaylistId = playlist.Playlistid,
                   ArtistName = playlist.Artistname,
                   SpotifyArtistId = playlist.Spotifyartistid,
                   ArtistImageUrl = artist.ImageUrl,
                   Venue = playlist.Venue,
                   City = playlist.City,
                   Date = playlist.Date
               }).OrderByDescending(x => x.PlaylistId).Take(limit);

            return playlists == null ? null : playlists.ToList();
        }
    }
}
