using Microsoft.Extensions.Logging;
using TechStore.Models;
using TechStore.Repositories;

namespace TechStore.Services
{
    public class PCBuildService
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<PCBuildService> _logger;

        public PCBuildService(IProductRepository productRepo, ILogger<PCBuildService> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        public class PCBuildRequest
        {
            public string Purpose { get; set; } = "gaming";
            public decimal Budget { get; set; } = 20000000;
            public string Priority { get; set; } = "balanced";
            public int? LockedCPUId { get; set; }
            public int? LockedGPUId { get; set; }
        }

        public class PCBuildResult
        {
            public Product? CPU { get; set; }
            public Product? GPU { get; set; }
            public Product? RAM { get; set; }
            public Product? Storage { get; set; }
            public Product? Motherboard { get; set; }
            public Product? PSU { get; set; }
            public Product? Case { get; set; }
            public decimal TotalPrice { get; set; }
            public string? Warning { get; set; }

            public decimal CalculateTotalPrice()
            {
                decimal total = 0;
                if (CPU != null) total += CPU.Price;
                if (GPU != null) total += GPU.Price;
                if (RAM != null) total += RAM.Price;
                if (Storage != null) total += Storage.Price;
                if (Motherboard != null) total += Motherboard.Price;
                if (PSU != null) total += PSU.Price;
                if (Case != null) total += Case.Price;
                return total;
            }
        }

        public async Task<PCBuildResult> RecommendBuild(PCBuildRequest request)
        {
            try
            {
                var result = new PCBuildResult();

                if (request.Budget <= 0 || request.Budget > 500000000)
                    throw new ArgumentException("Budget phải từ 5M đến 100M VNĐ");

                // Tính budget cho từng component theo use case
                var budgets = GetBudgetAllocation(request.Budget, request.Purpose, request.Priority);

                // Recommend hoặc sử dụng locked components
                result.CPU = request.LockedCPUId.HasValue
                    ? await _productRepo.GetByIdAsync(request.LockedCPUId.Value)
                    : await RecommendComponent("CPU", request.Purpose, budgets["CPU"]);

                result.GPU = request.LockedGPUId.HasValue
                    ? await _productRepo.GetByIdAsync(request.LockedGPUId.Value)
                    : await RecommendComponent("GPU", request.Purpose, budgets["GPU"]);

                result.RAM = await RecommendComponent("RAM", request.Purpose, budgets["RAM"]);
                result.Storage = await RecommendComponent("Storage", request.Purpose, budgets["Storage"]);
                result.Motherboard = await RecommendComponent("Motherboard", request.Purpose, budgets["Motherboard"]);
                result.PSU = await RecommendComponent("PSU", request.Purpose, budgets["PSU"]);
                result.Case = await RecommendComponent("Case", request.Purpose, budgets["Case"]);

                result.TotalPrice = result.CalculateTotalPrice();

                if (result.TotalPrice > request.Budget * 1.1m) // Allow 10% over
                    result.Warning = $"⚠️ Build vượt budget {(result.TotalPrice - request.Budget):N0} đ. Gợi ý: giảm GPU hoặc CPU.";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RecommendBuild");
                throw;
            }
        }

        private Dictionary<string, decimal> GetBudgetAllocation(decimal totalBudget, string purpose, string priority)
        {
            return purpose switch
            {
                "gaming" => AllocateGamingBudget(totalBudget, priority),
                "office" => AllocateOfficeBudget(totalBudget),
                "design" => AllocateDesignBudget(totalBudget),
                "streaming" => AllocateStreamingBudget(totalBudget),
                _ => AllocateBalancedBudget(totalBudget)
            };
        }

        private Dictionary<string, decimal> AllocateGamingBudget(decimal budget, string priority)
        {
            return priority switch
            {
                "performance" => new Dictionary<string, decimal>
                {
                    { "CPU", budget * 0.22m },
                    { "GPU", budget * 0.42m },
                    { "RAM", budget * 0.12m },
                    { "Storage", budget * 0.10m },
                    { "Motherboard", budget * 0.08m },
                    { "PSU", budget * 0.04m },
                    { "Case", budget * 0.02m }
                },
                "value" => new Dictionary<string, decimal>
                {
                    { "CPU", budget * 0.20m },
                    { "GPU", budget * 0.35m },
                    { "RAM", budget * 0.12m },
                    { "Storage", budget * 0.10m },
                    { "Motherboard", budget * 0.08m },
                    { "PSU", budget * 0.10m },
                    { "Case", budget * 0.05m }
                },
                _ => new Dictionary<string, decimal> // balanced
                {
                    { "CPU", budget * 0.25m },
                    { "GPU", budget * 0.35m },
                    { "RAM", budget * 0.12m },
                    { "Storage", budget * 0.10m },
                    { "Motherboard", budget * 0.08m },
                    { "PSU", budget * 0.06m },
                    { "Case", budget * 0.04m }
                }
            };
        }

        private Dictionary<string, decimal> AllocateOfficeBudget(decimal budget)
        {
            return new Dictionary<string, decimal>
            {
                { "CPU", budget * 0.20m },
                { "GPU", budget * 0.10m },
                { "RAM", budget * 0.15m },
                { "Storage", budget * 0.25m },
                { "Motherboard", budget * 0.15m },
                { "PSU", budget * 0.10m },
                { "Case", budget * 0.05m }
            };
        }

        private Dictionary<string, decimal> AllocateDesignBudget(decimal budget)
        {
            return new Dictionary<string, decimal>
            {
                { "CPU", budget * 0.25m },
                { "GPU", budget * 0.30m },
                { "RAM", budget * 0.20m },
                { "Storage", budget * 0.15m },
                { "Motherboard", budget * 0.05m },
                { "PSU", budget * 0.04m },
                { "Case", budget * 0.01m }
            };
        }

        private Dictionary<string, decimal> AllocateStreamingBudget(decimal budget)
        {
            return new Dictionary<string, decimal>
            {
                { "CPU", budget * 0.30m },
                { "GPU", budget * 0.35m },
                { "RAM", budget * 0.18m },
                { "Storage", budget * 0.10m },
                { "Motherboard", budget * 0.04m },
                { "PSU", budget * 0.02m },
                { "Case", budget * 0.01m }
            };
        }

        private Dictionary<string, decimal> AllocateBalancedBudget(decimal budget)
        {
            return new Dictionary<string, decimal>
            {
                { "CPU", budget * 0.25m },
                { "GPU", budget * 0.35m },
                { "RAM", budget * 0.12m },
                { "Storage", budget * 0.10m },
                { "Motherboard", budget * 0.08m },
                { "PSU", budget * 0.06m },
                { "Case", budget * 0.04m }
            };
        }

        private async Task<Product> RecommendComponent(string category, string purpose, decimal budget)
        {
            try
            {
                var products = await _productRepo.GetByCategoryAsync(category);

                if (products == null || products.Count() == 0)
                    return new Product { Name = "N/A", Price = 0, Id = 0 };

                var filtered = products.Where(p => p.Price <= budget).ToList();

                if (!filtered.Any())
                    filtered = products.OrderBy(p => p.Price).Take(1).ToList();

                return purpose switch
                {
                    "gaming" => filtered.OrderByDescending(p => p.Name.Contains("High") ? 1 : 0)
                        .ThenByDescending(p => p.Price).FirstOrDefault() ?? products.FirstOrDefault(),

                    "office" => filtered.OrderBy(p => p.Price).FirstOrDefault() ?? products.FirstOrDefault(),

                    "design" => filtered.OrderByDescending(p => p.Price).FirstOrDefault() ?? products.FirstOrDefault(),

                    "streaming" => filtered.OrderByDescending(p => p.Price).FirstOrDefault() ?? products.FirstOrDefault(),

                    _ => filtered.OrderByDescending(p => p.Price).FirstOrDefault() ?? products.FirstOrDefault()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recommending {category}");
                return new Product { Name = "N/A", Price = 0, Id = 0 };
            }
        }
    }
}
