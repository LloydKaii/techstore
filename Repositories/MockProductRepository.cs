// ============================================================
// Repositories/MockProductRepository.cs
// Bài 2: Mock data không cần database
// ============================================================
using TechStore.Models;

namespace TechStore.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public MockProductRepository()
        {
            _products = new List<Product>
            {
                new Product { Id=1, Name="MacBook Pro M3 14\"", Price=42990000,
                    Description="Chip M3, màn Liquid Retina XDR, pin 22h.", CategoryId=1,
                    ImageUrl="https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=400&q=80" },
                new Product { Id=2, Name="Dell XPS 15 OLED", Price=52900000,
                    Description="Core i9 Gen 13, RAM 32GB, màn OLED 3.5K.", CategoryId=2,
                    ImageUrl="https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400&q=80" },
                new Product { Id=3, Name="ASUS ROG Strix G16", Price=38500000,
                    Description="RTX 4070, Core i9, màn 240Hz.", CategoryId=3,
                    ImageUrl="https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400&q=80" },
                new Product { Id=4, Name="ThinkPad X1 Carbon", Price=35990000,
                    Description="Siêu mỏng nhẹ, Core i7, bảo mật vân tay.", CategoryId=4,
                    ImageUrl="https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=400&q=80" },
            };
        }

        public Task<IEnumerable<Product>> GetAllAsync()
            => Task.FromResult<IEnumerable<Product>>(_products);

        public Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int page = 1, int pageSize = 12, string? categoryName = null, string? search = null)
        {
            var query = _products.AsQueryable();
            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(p => p.CategoryId == 1); // Mock categories
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            var total = query.Count();
            var itemsList = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var items = (IEnumerable<Product>)itemsList;
            return Task.FromResult((items, total));
        }


        public Task<Product?> GetByIdAsync(int id)
            => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<IEnumerable<Product>?> GetByCategoryAsync(string categoryName)
            => Task.FromResult<IEnumerable<Product>?>(_products.Where(p => p.Category?.Name == categoryName).ToList());

        public Task AddAsync(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product product)
        {
            var idx = _products.FindIndex(p => p.Id == product.Id);
            if (idx >= 0) _products[idx] = product;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var p = _products.FirstOrDefault(p => p.Id == id);
            if (p != null) _products.Remove(p);
            return Task.CompletedTask;
        }
    }
}
