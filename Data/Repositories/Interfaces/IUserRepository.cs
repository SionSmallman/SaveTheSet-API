namespace sts_net.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> UpdateLastLoginAsync(string id, DateTime lastLoginDate);
        Task<User?> DeleteAsync(string id);
    }
}
