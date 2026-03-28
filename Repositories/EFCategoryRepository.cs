// ============================================================
// Repositories/EFCategoryRepository.cs
// ============================================================
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _context.Categories.ToListAsync();

        public async Task<IEnumerable<Category>> GetPublicCategoriesAsync()
            => await _context.Categories.Where(c => !c.Name.Contains("Admin")).ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
