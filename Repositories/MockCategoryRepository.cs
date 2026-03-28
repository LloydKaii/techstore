// ============================================================
// Repositories/MockCategoryRepository.cs
// Bài 2: Mock category data
// ============================================================
using TechStore.Models;

namespace TechStore.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new()
        {
            new Category { Id=1, Name="Laptop Apple" },
            new Category { Id=2, Name="Laptop Dell" },
            new Category { Id=3, Name="Laptop ASUS" },
            new Category { Id=4, Name="Laptop Lenovo" },
            new Category { Id=5, Name="Laptop HP" },
        };

        public Task<IEnumerable<Category>> GetAllAsync()
            => Task.FromResult<IEnumerable<Category>>(_categories);

        public Task<IEnumerable<Category>> GetPublicCategoriesAsync()
            => Task.FromResult<IEnumerable<Category>>(_categories.Where(c => !c.Name.Contains("Admin")));

        public Task<Category?> GetByIdAsync(int id)
            => Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

        public Task AddAsync(Category category)
        {
            category.Id = _categories.Any() ? _categories.Max(c => c.Id) + 1 : 1;
            _categories.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Category category)
        {
            var idx = _categories.FindIndex(c => c.Id == category.Id);
            if (idx >= 0) _categories[idx] = category;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var c = _categories.FirstOrDefault(c => c.Id == id);
            if (c != null) _categories.Remove(c);
            return Task.CompletedTask;
        }
    }
}
