namespace sts_net.Data.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> CreateAsync(T repoType);
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task<T> UpdateAsync(T repoType);
        Task<T?> DeleteAsync(string id);
    }
}
