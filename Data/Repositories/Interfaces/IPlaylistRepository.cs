using sts_net.Models.DTO;

namespace sts_net.Data.Repositories.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<Playlist> CreateAsync(Playlist playlist);
        Task<List<Playlist>> GetAllAsync();
        Task<Playlist?> GetByIdAsync(string id);
        Task<Playlist> UpdateAsync(Playlist playlist);
        Task<Playlist?> DeleteAsync(string id);
        List<RecentlySavedPlaylistDTO>? GetRecentPlaylists(int limit);
        Task<List<UserPlaylistHistoryDTO>> GetUsersSavedPlaylists(string spotifyUserId);
    }
}
