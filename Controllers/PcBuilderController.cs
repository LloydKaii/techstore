using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PcBuilderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _repo;

        // Thời gian lâu nhất để coi là hàng "tươi" (6 tháng)
        private const int FRESH_COMPONENT_DAYS = 180;

        public PcBuilderController(ApplicationDbContext context, IProductRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        /// <summary>
        /// Lấy danh sách linh kiện theo loại
        /// </summary>
        [HttpGet("components")]
        public async Task<ActionResult<object[]>> GetComponents(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest("Loại linh kiện không được để trống");

            var (items, _) = await _repo.GetPagedAsync(1, 30, type, null);

            var result = items
                .Where(p => p.Price > 0)
                .OrderByDescending(p => p.Name.Contains("Latest") || p.Name.Contains("2025"))
                .ThenByDescending(p => p.Price)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = (long)p.Price,
                    imageUrl = p.ImageUrl ?? "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&q=80",
                    category = p.Category?.Name ?? "Unknown",
                    description = (p.Description ?? "").Substring(0, Math.Min(100, p.Description?.Length ?? 0))
                })
                .ToArray();

            return Ok(result.Take(25).ToArray());
        }

        /// <summary>
        /// Auto-build PC dựa trên ngân sách
        /// Budget allocation:
        /// - Gaming (50%+ GPU): GPU:40%, CPU:25%, RAM:15%, Others:20%
        /// - Office/Content (50% CPU): CPU:35%, GPU:20%, RAM:20%, Others:25%
        /// - Balanced (equal): CPU:25%, GPU:30%, RAM:15%, Storage:15%, PSU:10%, Mainboard:5%
        /// </summary>
        [HttpGet("auto-build")]
        public async Task<ActionResult<object>> AutoBuild(
            decimal budget,
            string purpose = "balanced") // gaming, office, balanced, content
        {
            if (budget <= 0 || budget > 100_000_000) // Max 100 triệu VND
                return BadRequest(new { error = "Budget phải nằm trong khoảng 0 - 100 triệu VND" });

            // Budget allocation theo mục đích sử dụng  
            var budgetAllocation = purpose?.ToLower() switch
            {
                "gaming" => new Dictionary<string, decimal>
            {
                { "CPU", 0.22m }, { "GPU", 0.42m }, { "RAM", 0.12m }, { "SSD", 0.10m },
                { "MAINBOARD", 0.08m }, { "PSU", 0.06m }
            },
                "office" => new Dictionary<string, decimal>
            {
                { "CPU", 0.30m }, { "GPU", 0.15m }, { "RAM", 0.20m }, { "SSD", 0.15m },
                { "MAINBOARD", 0.12m }, { "PSU", 0.08m }
            },
                "content" => new Dictionary<string, decimal>
            {
                { "CPU", 0.35m }, { "GPU", 0.28m }, { "RAM", 0.18m }, { "SSD", 0.10m },
                { "MAINBOARD", 0.05m }, { "PSU", 0.04m }
            },
                _ => new Dictionary<string, decimal>
            {
                { "CPU", 0.25m }, { "GPU", 0.30m }, { "RAM", 0.15m }, { "SSD", 0.12m },
                { "MAINBOARD", 0.10m }, { "PSU", 0.08m }
            }
            };

            var build = new Dictionary<string, object>();
            decimal totalPrice = 0;
            var warnings = new List<string>();

            // Tìm component cho từng loại
            foreach (var (componentType, budgetPercentage) in budgetAllocation)
            {
                var maxPrice = budget * budgetPercentage;
                var components = await GetComponentsByType(componentType);

                if (!components.Any())
                {
                    warnings.Add($"Không tìm thấy linh kiện {componentType}");
                    continue;
                }

                // Ưu tiên: giá gần ngân sách -> mới nhất -> cao rating
                var selectedComponent = components
                    .Where(c => c.Price <= maxPrice)
                    .OrderByDescending(c => Math.Abs(c.Price - (budget * budgetPercentage) / 2))
                    .FirstOrDefault()
                    ?? components.OrderByDescending(c => c.Price).First();

                build[componentType.ToLower()] = new
                {
                    id = selectedComponent.Id,
                    name = selectedComponent.Name,
                    price = (long)selectedComponent.Price,
                    category = selectedComponent.Category?.Name
                };

                totalPrice += selectedComponent.Price;
            }

            // Kiểm tra xem tổng giá có vượt ngân sách không
            if (totalPrice > budget * 1.1m) // cho phép vượt 10%
                warnings.Add($"Tổng giá ({totalPrice:N0}₫) vượt ngân sách {(totalPrice - budget):N0}₫");

            return Ok(new
            {
                purpose,
                budget = (long)budget,
                build,
                total = (long)totalPrice,
                savings = (long)(budget - totalPrice),
                utilization = Math.Round((totalPrice / budget) * 100, 1),
                warnings = warnings.Any() ? warnings : null
            });
        }

        /// <summary>
        /// Lấy component với giá cao nhất trong ngân sách
        /// (hỗ trợ select manual)
        /// </summary>
        [HttpGet("best-in-budget")]
        public async Task<ActionResult<object>> BestInBudget(string componentType, decimal maxPrice)
        {
            if (string.IsNullOrWhiteSpace(componentType) || maxPrice <= 0)
                return BadRequest("Component type hoặc price không hợp lệ");

            var components = await GetComponentsByType(componentType);
            var best = components
                .Where(c => c.Price <= maxPrice)
                .OrderByDescending(c => c.Price)
                .FirstOrDefault();

            if (best == null)
                return NotFound($"Không tìm thấy {componentType} trong ngân sách {maxPrice:N0}₫");

            return Ok(new
            {
                id = best.Id,
                name = best.Name,
                price = (long)best.Price,
                description = best.Description,
                imageUrl = best.ImageUrl
            });
        }

        /// <summary>
        /// Tính toán giá tổng của build
        /// </summary>
        [HttpPost("calculate")]
        public async Task<ActionResult<object>> Calculate([FromBody] Dictionary<string, int> components)
        {
            if (components == null || !components.Any())
                return BadRequest("Danh sách component không được trống");

            decimal total = 0;
            var invalidComponents = new List<string>();

            foreach (var (key, productId) in components)
            {
                var product = await _repo.GetByIdAsync(productId);
                if (product == null)
                {
                    invalidComponents.Add($"{key}(ID:{productId})");
                }
                else
                {
                    total += product.Price;
                }
            }

            return Ok(new
            {
                totalPrice = (long)total,
                componentCount = components.Count,
                invalidComponents = invalidComponents.Any() ? invalidComponents : null
            });
        }

        // ═══ PRIVATE HELPERS ═══════════════════════════════════════

        private async Task<List<Product>> GetComponentsByType(string type)
        {
            var normalizedType = type.ToUpper().Trim();

            var components = await _context.Products
                .Where(p => p.Price > 0 && p.Category != null)
                .ToListAsync();

            var filtered = components
                .Where(p => MatchComponentType(p.Name, normalizedType))
                .OrderByDescending(c => IsNewComponent(c.Id))
                .ThenByDescending(c => c.Price)
                .ToList();

            return filtered;
        }

        private bool MatchComponentType(string productName, string componentType)
        {
            if (string.IsNullOrEmpty(productName)) return false;

            var name = productName.ToUpper();

            return componentType switch
            {
                "CPU" => name.Contains("CPU") || name.Contains("PROCESSOR") || name.Contains("RYZEN") || name.Contains("INTEL CORE"),
                "GPU" => name.Contains("GPU") || name.Contains("VGA") || name.Contains("RTX") || name.Contains("RX") || name.Contains("GTX"),
                "RAM" => name.Contains("RAM") || name.Contains("MEMORY") || name.Contains("DDR"),
                "SSD" => name.Contains("SSD") || name.Contains("NVME") || (name.Contains("STORAGE") && !name.Contains("HDD")),
                "MAINBOARD" => name.Contains("MAIN") || name.Contains("MOTHERBOARD") || name.Contains("MOBO"),
                "PSU" => name.Contains("PSU") || name.Contains("POWER SUPPLY"),
                "CASE" => name.Contains("CASE") || name.Contains("CHASSIS"),
                _ => false
            };
        }

        private bool IsNewComponent(int productId)
        {
            // Giả sử newer products có ID cao hơn (crude heuristic)
            return productId > 100; // Tuỳ chỉnh theo data thực tế
        }
    }
}


