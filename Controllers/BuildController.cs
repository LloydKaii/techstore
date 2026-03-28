using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;

namespace TechStore.Controllers
{
    public class BuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuildController(ApplicationDbContext context, IProductRepository productRepo, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _productRepo = productRepo;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allLinhKienQuery = _context.Products
                            .Where(p => p.Category.Name == "Linh kiện");

            var cpuList = await allLinhKienQuery.Where(p => p.Name.Contains("CPU")).OrderBy(p => p.Price).Take(10).ToListAsync();
            var ramList = await allLinhKienQuery.Where(p => p.Name.Contains("RAM")).OrderBy(p => p.Price).Take(10).ToListAsync();
            var vgaList = await allLinhKienQuery.Where(p => p.Name.Contains("VGA") || p.Name.Contains("RTX") || p.Name.Contains("RX")).OrderBy(p => p.Price).Take(10).ToListAsync();
            var mainList = await allLinhKienQuery.Where(p => p.Name.Contains("Main") || p.Name.Contains("Mainboard")).OrderBy(p => p.Price).Take(10).ToListAsync();
            var psuList = await allLinhKienQuery.Where(p => p.Name.Contains("PSU")).OrderBy(p => p.Price).Take(10).ToListAsync();
            var ssdList = await allLinhKienQuery.Where(p => p.Name.Contains("SSD")).OrderBy(p => p.Price).Take(10).ToListAsync();

            ViewBag.Components = new Dictionary<string, object>
            {
                ["CPU"] = cpuList,
                ["RAM"] = ramList,
                ["VGA"] = vgaList,
                ["MAINBOARD"] = mainList,
                ["PSU"] = psuList,
                ["SSD"] = ssdList
            };

            if (User.Identity.IsAuthenticated)
                ViewBag.Builds = await GetUserBuildsAsync();

            return View();
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Save()
        {
            var user = await _userManager.GetUserAsync(User);
            var build = new PCBuild
            {
                UserId = user.Id,
                Name = !string.IsNullOrEmpty(Request.Form["name"]) ? Request.Form["name"].ToString() : "My PC Build",
                CPUId = int.TryParse(Request.Form["CPUId"], out int cpuId) ? cpuId : null,
                RAMId = int.TryParse(Request.Form["RAMId"], out int ramId) ? ramId : null,
                VGAId = int.TryParse(Request.Form["VGAId"], out int vgaId) ? vgaId : null,
                MainboardId = int.TryParse(Request.Form["MainboardId"], out int mainId) ? mainId : null,
                PSUId = int.TryParse(Request.Form["PSUId"], out int psuId) ? psuId : null,
                CaseId = int.TryParse(Request.Form["CaseId"], out int caseId) ? caseId : null,
                Notes = Request.Form["notes"].ToString(),
                CreatedAt = DateTime.Now,
                TotalPrice = 0
            };

            if (build.CPUId.HasValue)
            {
                var cpu = await _context.Products.FindAsync(build.CPUId.Value);
                if (cpu != null) build.TotalPrice += cpu.Price;
            }
            if (build.RAMId.HasValue)
            {
                var ram = await _context.Products.FindAsync(build.RAMId.Value);
                if (ram != null) build.TotalPrice += ram.Price;
            }
            if (build.VGAId.HasValue)
            {
                var vga = await _context.Products.FindAsync(build.VGAId.Value);
                if (vga != null) build.TotalPrice += vga.Price;
            }
            if (build.MainboardId.HasValue)
            {
                var main = await _context.Products.FindAsync(build.MainboardId.Value);
                if (main != null) build.TotalPrice += main.Price;
            }
            if (build.PSUId.HasValue)
            {
                var psu = await _context.Products.FindAsync(build.PSUId.Value);
                if (psu != null) build.TotalPrice += psu.Price;
            }
            if (build.CaseId.HasValue)
            {
                var @case = await _context.Products.FindAsync(build.CaseId.Value);
                if (@case != null) build.TotalPrice += @case.Price;
            }

            _context.PCBuilds.Add(build);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã lưu build thành công!";
            return RedirectToAction(nameof(MyBuilds));
        }

        [Authorize]
        public async Task<IActionResult> MyBuilds()
        {
            var user = await _userManager.GetUserAsync(User);
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

        [AllowAnonymous]
        public IActionResult AutoBuild(decimal budget)
        {
            ViewBag.Budget = budget;
            return PartialView("_AutoBuild");
        }

        private async Task<List<PCBuild>> GetUserBuildsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return new List<PCBuild>();
            return await _context.PCBuilds.Where(b => b.UserId == user.Id).OrderByDescending(b => b.CreatedAt).Take(5).ToListAsync();
        }
    }
}

