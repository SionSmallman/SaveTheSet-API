namespace sts_net.Data.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<Spotifytoken> CreateAsync(Spotifytoken token);
        Task<List<Spotifytoken>> GetAllAsync();
        Task<Spotifytoken?> GetByIdAsync(string id);
        Task<Spotifytoken> UpdateRefreshTokenAsync(string id, string newRefreshToken);
        Task<Spotifytoken?> DeleteAsync(string id);
    }
}
