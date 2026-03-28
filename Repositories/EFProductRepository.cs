// ============================================================
// Repositories/EFProductRepository.cs
// Bài 3: Entity Framework Core implementation
// ============================================================
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<IEnumerable<Product>> GetAllAsync()
            => await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

        public async Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int page = 1, int pageSize = 12, string? categoryName = null, string? search = null)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(p => p.Category.Name.Contains(categoryName));

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Product?> GetByIdAsync(int id)
            => await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
