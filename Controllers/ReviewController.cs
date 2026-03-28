using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        public ReviewController(ApplicationDbContext db, UserManager<ApplicationUser> um){ _db=db; _um=um; }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int rating, string comment)
        {
            var user = await _um.GetUserAsync(User);
            if(user==null) return Unauthorized();
            var existing = await _db.Reviews.FirstOrDefaultAsync(r=>r.ProductId==productId&&r.UserId==user.Id);
            if(existing!=null){ existing.Rating=rating; existing.Comment=comment; existing.CreatedAt=DateTime.Now; TempData["Success"]="Đã cập nhật đánh giá!"; }
            else { _db.Reviews.Add(new Review{ProductId=productId,UserId=user.Id,Rating=rating,Comment=comment}); TempData["Success"]="Cảm ơn bạn đã đánh giá!"; }
            await _db.SaveChangesAsync();
            return RedirectToAction("Display","Product",new{id=productId});
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user=await _um.GetUserAsync(User);
            var review=await _db.Reviews.FindAsync(id);
            if(review==null) return NotFound();
            if(review.UserId!=user?.Id&&!User.IsInRole("Admin")) return Forbid();
            var pid=review.ProductId;
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            TempData["Success"]="Đã xóa đánh giá!";
            return RedirectToAction("Display","Product",new{id=pid});
        }
    }
}
