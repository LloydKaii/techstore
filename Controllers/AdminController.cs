using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;

namespace TechStore.Controllers
{
    [Authorize(Roles="Admin,Manager")]
    public class AdminController : Controller
    {
        private readonly IProductRepository  _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly UserManager<ApplicationUser>  _um;
        private readonly RoleManager<IdentityRole>     _rm;
        private readonly ApplicationDbContext _db;

        public AdminController(IProductRepository p, ICategoryRepository c, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, ApplicationDbContext db)
        { _productRepo=p; _categoryRepo=c; _um=um; _rm=rm; _db=db; }

        public async Task<IActionResult> Dashboard()
        {
            var products   = await _productRepo.GetAllAsync();
            var categories = await _categoryRepo.GetAllAsync();
            var users      = _um.Users.ToList();
            var orders     = await _db.Orders.ToListAsync();

            int adminCount=0;
            foreach(var u in users) if(await _um.IsInRoleAsync(u,"Admin")) adminCount++;

            ViewBag.TotalProducts   = products.Count();
            ViewBag.TotalCategories = categories.Count();
            ViewBag.TotalUsers      = users.Count;
            ViewBag.TotalAdmins     = adminCount;
            ViewBag.TotalOrders     = orders.Count;
            ViewBag.TotalRevenue    = orders.Where(o=>o.Status!="Đã hủy").Sum(o=>o.Total);
            ViewBag.PendingOrders   = orders.Count(o=>o.Status=="Chờ xác nhận");
            ViewBag.RecentProducts  = products.TakeLast(5).Reverse();
            ViewBag.RecentOrders    = orders.OrderByDescending(o=>o.OrderDate).Take(5);
            ViewBag.Users           = users.Take(6);

            // Doanh thu 7 ngày gần nhất (cho biểu đồ)
            var last7 = Enumerable.Range(0,7).Select(i=>DateTime.Today.AddDays(-6+i)).ToList();
            var revenueData = last7.Select(d=> orders.Where(o=>o.OrderDate.Date==d&&o.Status!="Đã hủy").Sum(o=>(double)o.Total)).ToList();
            var labels = last7.Select(d=>d.ToString("dd/MM")).ToList();
            ViewBag.ChartLabels  = System.Text.Json.JsonSerializer.Serialize(labels);
            ViewBag.ChartRevenue = System.Text.Json.JsonSerializer.Serialize(revenueData);

            return View();
        }

        // Quản lý Users - chỉ Admin
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Users()
        {
            var users = _um.Users.ToList();
            var list  = new List<(ApplicationUser User, IList<string> Roles)>();
            foreach(var u in users) list.Add((u, await _um.GetRolesAsync(u)));
            ViewBag.AllRoles = _rm.Roles.Select(r=>r.Name).ToList();
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles="Admin")]
        public async Task<IActionResult> SetRole(string userId, string role)
        {
            var user = await _um.FindByIdAsync(userId);
            if(user==null) return NotFound();
            var current = await _um.GetRolesAsync(user);
            await _um.RemoveFromRolesAsync(user,current);
            await _um.AddToRoleAsync(user,role);
            TempData["Success"]=$"Đã cấp quyền {role} cho {user.Email}!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles="Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _um.FindByIdAsync(userId);
            if(user==null) return NotFound();
            var me = await _um.GetUserAsync(User);
            if(me?.Id==userId){ TempData["Error"]="Không thể xóa tài khoản đang đăng nhập!"; return RedirectToAction(nameof(Users)); }
            await _um.DeleteAsync(user);
            TempData["Success"]=$"Đã xóa tài khoản {user.Email}!";
            return RedirectToAction(nameof(Users));
        }

        // Quản lý Voucher
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Vouchers() => View(await _db.Vouchers.OrderByDescending(v=>v.Id).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles="Admin")]
        public async Task<IActionResult> AddVoucher(string code, int discountPercent, int usageLimit, int days)
        {
            _db.Vouchers.Add(new Voucher{ Code=code.ToUpper(), DiscountPercent=discountPercent, UsageLimit=usageLimit, ExpiryDate=DateTime.Now.AddDays(days), IsActive=true });
            await _db.SaveChangesAsync();
            TempData["Success"]=$"Đã tạo voucher {code.ToUpper()}!";
            return RedirectToAction(nameof(Vouchers));
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles="Admin")]
        public async Task<IActionResult> ToggleVoucher(int id)
        {
            var v = await _db.Vouchers.FindAsync(id);
            if(v!=null){ v.IsActive=!v.IsActive; await _db.SaveChangesAsync(); TempData["Success"]=$"Đã {(v.IsActive?"kích hoạt":"tắt")} voucher!"; }
            return RedirectToAction(nameof(Vouchers));
        }
    }
}
