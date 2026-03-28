using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        public WishlistController(ApplicationDbContext db, UserManager<ApplicationUser> um){ _db=db; _um=um; }

        public async Task<IActionResult> Index()
        {
            var user = await _um.GetUserAsync(User);
            var items = await _db.WishlistItems.Include(w=>w.Product).ThenInclude(p=>p!.Category)
                .Where(w=>w.UserId==user!.Id).OrderByDescending(w=>w.AddedAt).ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int productId)
        {
            var user = await _um.GetUserAsync(User);
            var existing = await _db.WishlistItems.FirstOrDefaultAsync(w=>w.UserId==user!.Id&&w.ProductId==productId);
            if(existing!=null){ _db.WishlistItems.Remove(existing); TempData["Success"]="Đã xóa khỏi wishlist!"; }
            else { _db.WishlistItems.Add(new WishlistItem{UserId=user!.Id,ProductId=productId}); TempData["Success"]="Đã thêm vào wishlist!"; }
            await _db.SaveChangesAsync();
            return RedirectToAction("Display","Product",new{id=productId});
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var user = await _um.GetUserAsync(User);
            var item = await _db.WishlistItems.FirstOrDefaultAsync(w=>w.Id==id&&w.UserId==user!.Id);
            if(item!=null){ _db.WishlistItems.Remove(item); await _db.SaveChangesAsync(); TempData["Success"]="Đã xóa!"; }
            return RedirectToAction(nameof(Index));
        }
    }
}
