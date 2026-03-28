using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Models;
using TechStore.Repositories;
using TechStore.Services;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly AIService _ai;

        public ProductController(IProductRepository p, ICategoryRepository c, AIService ai)
        {
            _productRepo = p;
            _categoryRepo = c;
            _ai = ai;
        }

        public async Task<IActionResult> Index(int page = 1, string? category = null, string? search = null)
        {
            var (items, total) = await _productRepo.GetPagedAsync(page, 12, category, search);
            var model = new PagedViewModel<Product>
            {
                Items = items,
                Page = page,
                PageSize = 12,
                TotalItems = total,
                CategoryFilter = category,
                Search = search
            };
            ViewBag.Categories = await _categoryRepo.GetAllAsync();
            return View(model);
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();
            var all = await _productRepo.GetAllAsync();
            ViewBag.Recommendations = _ai.GetRecommendations(product, all);
            return View(product);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add()
        {
            await LoadCategoriesAsync();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Add(Product product)
        {
            ModelState.Remove("Category"); ModelState.Remove("Images");
            if (ModelState.IsValid)
            {
                await _productRepo.AddAsync(product);
                TempData["Success"] = $"Đã thêm \"{product.Name}\" thành công!";
                return RedirectToAction(nameof(Index));
            }
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p == null) return NotFound();
            await LoadCategoriesAsync(p.CategoryId);
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id) return NotFound();
            ModelState.Remove("Category"); ModelState.Remove("Images");
            if (ModelState.IsValid)
            {
                var ex = await _productRepo.GetByIdAsync(id);
                if (ex == null) return NotFound();
                ex.Name = product.Name; ex.Price = product.Price;
                ex.Description = product.Description; ex.CategoryId = product.CategoryId;
                if (!string.IsNullOrEmpty(product.ImageUrl)) ex.ImageUrl = product.ImageUrl;
                await _productRepo.UpdateAsync(ex);
                TempData["Success"] = $"Đã cập nhật \"{product.Name}\"!";
                return RedirectToAction(nameof(Index));
            }
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("DeleteConfirmed"), ValidateAntiForgeryToken, Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p != null) { await _productRepo.DeleteAsync(id); TempData["Success"] = $"Đã xóa \"{p.Name}\"!"; }
            return RedirectToAction(nameof(Index));
        }


        private async Task LoadCategoriesAsync(int sel = 0)
        {
            var cats = await _categoryRepo.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name", sel);
        }
    }
}
