// ============================================================
// Controllers/HomeController.cs
// ============================================================
using Microsoft.AspNetCore.Mvc;
using TechStore.Repositories;
using TechStore.Models;
using TechStore.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index(int page = 1, string? category = null, string? search = null)
        {
            var (items, total) = await _productRepo.GetPagedAsync(page, 8, category, search);
            var model = new PagedViewModel<Product>
            {
                Items = items,
                Page = page,
                PageSize = 8,
                TotalItems = total,
                CategoryFilter = category,
                Search = search
            };
            ViewBag.Categories = await _categoryRepo.GetAllAsync();
            return View(model);
        }
    }
}
