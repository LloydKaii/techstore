using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;
using TechStore.Services;

namespace TechStore.Controllers
{
    [AllowAnonymous]
    public class BuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly PCBuildService _buildService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BuildController> _logger;

        public BuildController(
            ApplicationDbContext context,
            IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            PCBuildService buildService,
            UserManager<ApplicationUser> userManager,
            ILogger<BuildController> logger)
        {
            _context = context;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _buildService = buildService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Build
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryRepo.GetAllAsync();
                var cpus = await _productRepo.GetByCategoryAsync("CPU") ?? new List<Product>();
                var gpus = await _productRepo.GetByCategoryAsync("GPU") ?? new List<Product>();
                var rams = await _productRepo.GetByCategoryAsync("RAM") ?? new List<Product>();
                var storages = await _productRepo.GetByCategoryAsync("Storage") ?? new List<Product>();
                var motherboards = await _productRepo.GetByCategoryAsync("Motherboard") ?? new List<Product>();
                var psus = await _productRepo.GetByCategoryAsync("PSU") ?? new List<Product>();
                var cases = await _productRepo.GetByCategoryAsync("Case") ?? new List<Product>();

                ViewBag.CPUs = cpus;
                ViewBag.GPUs = gpus;
                ViewBag.RAMs = rams;
                ViewBag.Storages = storages;
                ViewBag.Motherboards = motherboards;
                ViewBag.PSUs = psus;
                ViewBag.Cases = cases;
                ViewBag.Categories = categories;

                if (User.Identity.IsAuthenticated)
                    ViewBag.Builds = await GetUserBuildsAsync();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Build page");
                TempData["Error"] = "Lỗi tải trang Build PC. Vui lòng thử lại!";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Build/Recommend - AI Generate Build
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Recommend([FromBody] PCBuildService.PCBuildRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { error = "Dữ liệu không hợp lệ" });

                var result = await _buildService.RecommendBuild(request);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        cpu = CreateComponentResponse(result.CPU),
                        gpu = CreateComponentResponse(result.GPU),
                        ram = CreateComponentResponse(result.RAM),
                        storage = CreateComponentResponse(result.Storage),
                        motherboard = CreateComponentResponse(result.Motherboard),
                        psu = CreateComponentResponse(result.PSU),
                        case_ = CreateComponentResponse(result.Case),
                        totalPrice = result.TotalPrice,
                        warning = result.Warning
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recommending build");
                return BadRequest(new { error = "Lỗi tạo build: " + ex.Message });
            }
        }

        // GET: /Build/MyBuilds
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyBuilds()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var builds = await _context.PCBuilds
                    .Include(b => b.CPU)
                    .Include(b => b.RAM)
                    .Include(b => b.VGA)
                    .Include(b => b.Mainboard)
                    .Include(b => b.PSU)
                    .Include(b => b.Case)
                    .Where(b => b.UserId == user.Id)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return View(builds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading MyBuilds");
                return RedirectToAction("Index");
            }
        }

        // POST: /Build/Save - Lưu Build
        [Authorize(Roles = "Customer"), HttpPost]
        public async Task<IActionResult> Save([FromBody] dynamic buildData)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var buildName = buildData?.name ?? "My Build";
                var components = buildData?.components as dynamic;

                var build = new PCBuild
                {
                    UserId = user.Id,
                    Name = buildName,
                    CreatedAt = DateTime.Now,
                    TotalPrice = 0
                };

                if (components != null)
                {
                    int cpuId, gpuId, ramId, motherId, psuId, caseId;
                    
                    if (int.TryParse(components.cpuId?.ToString() ?? "", out cpuId))
                        build.CPUId = cpuId;
                    if (int.TryParse(components.gpuId?.ToString() ?? "", out gpuId))
                        build.VGAId = gpuId;
                    if (int.TryParse(components.ramId?.ToString() ?? "", out ramId))
                        build.RAMId = ramId;
                    if (int.TryParse(components.motherId?.ToString() ?? "", out motherId))
                        build.MainboardId = motherId;
                    if (int.TryParse(components.psuId?.ToString() ?? "", out psuId))
                        build.PSUId = psuId;
                    if (int.TryParse(components.caseId?.ToString() ?? "", out caseId))
                        build.CaseId = caseId;
                }

                build.TotalPrice = await CalculateBuildPrice(build);

                _context.PCBuilds.Add(build);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Build saved successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving build");
                return BadRequest(new { error = "Lỗi lưu build: " + ex.Message });
            }
        }

        private async Task<decimal> CalculateBuildPrice(PCBuild build)
        {
            decimal total = 0;

            if (build.CPUId.HasValue)
            {
                var cpu = await _context.Products.FindAsync(build.CPUId.Value);
                if (cpu != null) total += cpu.Price;
            }
            if (build.RAMId.HasValue)
            {
                var ram = await _context.Products.FindAsync(build.RAMId.Value);
                if (ram != null) total += ram.Price;
            }
            if (build.VGAId.HasValue)
            {
                var vga = await _context.Products.FindAsync(build.VGAId.Value);
                if (vga != null) total += vga.Price;
            }
            if (build.MainboardId.HasValue)
            {
                var mb = await _context.Products.FindAsync(build.MainboardId.Value);
                if (mb != null) total += mb.Price;
            }
            if (build.PSUId.HasValue)
            {
                var psu = await _context.Products.FindAsync(build.PSUId.Value);
                if (psu != null) total += psu.Price;
            }
            if (build.CaseId.HasValue)
            {
                var @case = await _context.Products.FindAsync(build.CaseId.Value);
                if (@case != null) total += @case.Price;
            }

            return total;
        }

        private object CreateComponentResponse(Product product)
        {
            if (product == null || product.Id == 0)
                return new { id = 0, name = "N/A", price = 0, image = "" };

            return new
            {
                id = product.Id,
                name = product.Name,
                price = product.Price,
                image = product.ImageUrl ?? ""
            };
        }

        private async Task<List<PCBuild>> GetUserBuildsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return new List<PCBuild>();

            return await _context.PCBuilds
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}

