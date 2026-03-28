using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ApplicationDbContext _context;

        public ProductsController(IProductRepository productRepo, ApplicationDbContext context)
        {
            _productRepo = productRepo;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts(string? type = null)
        {
            var query = _context.Products.Where(p => p.CategoryId == 5) // Linh kiện
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.ImageUrl
                }).AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                query = type.ToUpper() switch
                {
                    "CPU" => query.Where(p => p.Name.Contains("CPU", StringComparison.OrdinalIgnoreCase)),
                    "RAM" => query.Where(p => p.Name.Contains("RAM", StringComparison.OrdinalIgnoreCase)),
                    "VGA" => query.Where(p => p.Name.Contains("VGA", StringComparison.OrdinalIgnoreCase)),
                    "MAINBOARD" => query.Where(p => p.Name.Contains("Mainboard", StringComparison.OrdinalIgnoreCase)),
                    "PSU" => query.Where(p => p.Name.Contains("PSU", StringComparison.OrdinalIgnoreCase)),
                    "CASE" => query.Where(p => p.Name.Contains("Case", StringComparison.OrdinalIgnoreCase)),
                    _ => query
                };
            }

            var products = await query.Take(20).ToListAsync();
            return Ok(products);
        }
    }
}

