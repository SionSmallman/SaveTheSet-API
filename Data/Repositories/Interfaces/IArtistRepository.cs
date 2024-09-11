namespace sts_net.Data.Repositories.Interfaces
{
    public interface IArtistRepository
    {
        Task<Artist> CreateAsync(Artist artist);
        Task<List<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(string id);
        Task<Artist> UpdateImageAsync(string id, string imageUrl);
        Task<Artist?> DeleteAsync(string id);
    }
}
