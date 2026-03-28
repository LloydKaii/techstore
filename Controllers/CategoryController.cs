using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Repositories;

namespace TechStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository categoryRepo)
            => _categoryRepo = categoryRepo;

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");
            var categories = isAdmin
                ? await _categoryRepo.GetAllAsync()
                : await _categoryRepo.GetPublicCategoriesAsync();
            ViewBag.IsAdmin = isAdmin;
            // Pagination for admin list
            if (isAdmin && page > 1)
            {
                // Skip/Take logic if needed for large lists
                categories = categories.Skip((page - 1) * 10).Take(10);
                ViewBag.Page = page;
            }
            return View(categories);
        }

        public async Task<IActionResult> Display(int id)
        {
            var cat = await _categoryRepo.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add() => View();

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepo.AddAsync(category);
                TempData["Success"] = $"Đã thêm danh mục \"{category.Name}\"!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var cat = await _categoryRepo.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id) return NotFound();
            if (ModelState.IsValid)
            {
                await _categoryRepo.UpdateAsync(category);
                TempData["Success"] = $"Đã cập nhật danh mục \"{category.Name}\"!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _categoryRepo.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost, ActionName("DeleteConfirmed"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cat = await _categoryRepo.GetByIdAsync(id);
            if (cat != null)
            {
                await _categoryRepo.DeleteAsync(id);
                TempData["Success"] = $"Đã xóa danh mục \"{cat.Name}\"!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
