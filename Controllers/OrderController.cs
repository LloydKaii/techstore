using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public OrderController(ApplicationDbContext db, UserManager<ApplicationUser> um)
        { _db=db; _um=um; }

        // ── CUSTOMER: lịch sử đơn hàng của chính mình ──────────────────────────
        public async Task<IActionResult> MyOrders()
        {
            var user = await _um.GetUserAsync(User);
            var orders = await _db.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == user!.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // ── CHI TIẾT đơn hàng (Customer xem đơn của mình, Admin/Manager xem hết) ─
        public async Task<IActionResult> Detail(int id)
        {
            var user  = await _um.GetUserAsync(User);
            var order = await _db.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            bool isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");
            if (order.UserId != user?.Id && !isAdminOrManager) return Forbid();

            return View(order);
        }

        // ── ADMIN/MANAGER: quản lý tất cả đơn hàng (có filter theo trạng thái) ──
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage(string? status)
        {
            var statuses = new[] { "Chờ xác nhận", "Đã xác nhận", "Đang giao", "Hoàn thành", "Đã hủy" };
            ViewBag.Statuses = statuses;
            ViewBag.CurrentStatus = status;

            var query = _db.Orders.Include(o => o.Items).AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }

        // ── ADMIN/MANAGER: cập nhật trạng thái đơn ─────────────────────────────
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? returnStatus)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật đơn #TS{id:D4} → {status}";

            if (!string.IsNullOrEmpty(returnStatus))
                return RedirectToAction(nameof(Manage), new { status = returnStatus });
            return RedirectToAction(nameof(Manage));
        }

        // ── LEGACY route /Order/All → redirect sang Manage ──────────────────────
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult All() => RedirectToAction(nameof(Manage));
    }
}
