// ============================================================
// Repositories/IProductRepository.cs
// ============================================================
using TechStore.Models;

namespace TechStore.Repositories
{
    public interface IProductRepository
    {
        // Bài 3: async methods với EF Core
        Task<IEnumerable<Product>> GetAllAsync();
        Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int page = 1, int pageSize = 12, string? categoryName = null, string? search = null);
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>?> GetByCategoryAsync(string categoryName);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
