using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;

namespace TechStore.Controllers
{
    /// <summary>
    /// MANAGER CONTROLLER - Chỉ Manager có quyền
    /// Quản lý: Products, Orders, Inventory, Sales Reports
    /// KHÔNG CÓ: User Management, Roles, Vouchers, System Settings
    /// </summary>
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ApplicationDbContext _db;

        public ManagerController(IProductRepository productRepo, ICategoryRepository categoryRepo, ApplicationDbContext db)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _db = db;
        }

        /// <summary>
        /// Manager Dashboard - Hiển thị thống kê sản phẩm & đơn hàng
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            var products = await _productRepo.GetAllAsync();
            var orders = await _db.Orders.ToListAsync();
            var validOrders = orders.Where(o => o.Status != "Đã hủy").ToList();

            // Sản phẩm bán chạy nhất (lấy từ OrderItems)
            var topProducts = await _db.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var topProductsList = new List<Product>();
            foreach (var tp in topProducts)
            {
                var product = await _productRepo.GetByIdAsync(tp.ProductId);
                if (product != null)
                    topProductsList.Add(product);
            }

            ViewBag.TotalProducts = products.Count();
            ViewBag.TotalOrders = validOrders.Count;
            ViewBag.TotalRevenue = validOrders.Sum(o => o.Total);
            ViewBag.PendingOrders = orders.Count(o => o.Status == "Chờ xác nhận");
            ViewBag.ShippingOrders = orders.Count(o => o.Status == "Đang giao");
            ViewBag.CompletedOrders = orders.Count(o => o.Status == "Hoàn thành");
            ViewBag.TopProducts = topProductsList;
            ViewBag.RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10);

            // Doanh thu 7 ngày gần nhất (cho biểu đồ)
            var last7 = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-6 + i)).ToList();
            var revenueData = last7.Select(d =>
                (double)validOrders
                    .Where(o => o.OrderDate.Date == d)
                    .Sum(o => o.Total)
            ).ToList();
            var labels = last7.Select(d => d.ToString("dd/MM")).ToList();
            ViewBag.ChartLabels = System.Text.Json.JsonSerializer.Serialize(labels);
            ViewBag.ChartRevenue = System.Text.Json.JsonSerializer.Serialize(revenueData);

            return View("Dashboard");
        }

        /// <summary>
        /// Quản lý danh mục sản phẩm
        /// </summary>
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return View(categories);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Tên danh mục không được trống!";
                return RedirectToAction(nameof(Categories));
            }

            var category = new Category { Name = name.Trim(), Description = description?.Trim() };
            await _categoryRepo.AddAsync(category);
            TempData["Success"] = $"Đã thêm danh mục '{name}'!";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id, string name, string? description)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.Name = name.Trim();
            category.Description = description?.Trim();
            await _categoryRepo.UpdateAsync(category);
            TempData["Success"] = "Đã cập nhật danh mục!";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return NotFound();

            // Check if category has products
            var products = await _productRepo.GetAllAsync();
            if (products.Any(p => p.CategoryId == id))
            {
                TempData["Error"] = "Không thể xóa danh mục có sản phẩm!";
                return RedirectToAction(nameof(Categories));
            }

            await _categoryRepo.DeleteAsync(id);
            TempData["Success"] = "Đã xóa danh mục!";
            return RedirectToAction(nameof(Categories));
        }

        /// <summary>
        /// Quản lý sản phẩm
        /// </summary>
        public async Task<IActionResult> Products(int page = 1)
        {
            var (items, total) = await _productRepo.GetPagedAsync(page, 20, null, null);
            ViewBag.Page = page;
            ViewBag.TotalPages = (total + 19) / 20;
            ViewBag.Categories = await _categoryRepo.GetAllAsync();
            return View(items.ToList());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Images");

            if (ModelState.IsValid)
            {
                await _productRepo.AddAsync(product);
                TempData["Success"] = $"Đã thêm sản phẩm '{product.Name}'!";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _categoryRepo.GetAllAsync();
            return View("Products", await _productRepo.GetAllAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Description = product.Description;
            existing.CategoryId = product.CategoryId;
            if (!string.IsNullOrEmpty(product.ImageUrl))
                existing.ImageUrl = product.ImageUrl;

            await _productRepo.UpdateAsync(existing);
            TempData["Success"] = $"Đã cập nhật sản phẩm '{product.Name}'!";
            return RedirectToAction(nameof(Products));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            // Check if product in any active orders
            var hasActiveOrders = await _db.OrderItems
                .Where(oi => oi.ProductId == id)
                .AnyAsync(oi => !oi.Order.Status.Contains("Đã hủy"));

            if (hasActiveOrders)
            {
                TempData["Error"] = "Không thể xóa sản phẩm có đơn hàng đang xử lý!";
                return RedirectToAction(nameof(Products));
            }

            await _productRepo.DeleteAsync(id);
            TempData["Success"] = "Đã xóa sản phẩm!";
            return RedirectToAction(nameof(Products));
        }

        /// <summary>
        /// Quản lý đơn hàng - Manager chỉ xem và cập nhật trạng thái
        /// </summary>
        public async Task<IActionResult> Orders(string? status)
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật đơn #TS{id:D4} → {status}";
            return RedirectToAction(nameof(Orders));
        }

        /// <summary>
        /// Sales Report - Manager xem báo cáo bán hàng
        /// </summary>
        public async Task<IActionResult> SalesReport(DateTime? fromDate, DateTime? toDate)
        {
            var orders = await _db.Orders
                .Include(o => o.Items)
                .Where(o => o.Status != "Đã hủy")
                .ToListAsync();

            if (fromDate.HasValue)
                orders = orders.Where(o => o.OrderDate.Date >= fromDate.Value.Date).ToList();

            if (toDate.HasValue)
                orders = orders.Where(o => o.OrderDate.Date <= toDate.Value.Date).ToList();

            var reportData = new
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.Total),
                AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.Total) : 0,
                TopProducts = await _db.OrderItems
                    .Where(oi => !oi.Order.Status.Contains("Đã hủy"))
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                    .OrderByDescending(x => x.Quantity)
                    .Take(10)
                    .ToListAsync()
            };

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.ReportData = reportData;
            ViewBag.Orders = orders;

            return View();
        }
    }
}
