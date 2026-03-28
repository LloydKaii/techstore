using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;
using TechStore.Services;

namespace TechStore.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cart;
        private readonly IProductRepository _productRepo;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public CartController(CartService cart, IProductRepository productRepo, ApplicationDbContext db, UserManager<ApplicationUser> um)
        { _cart=cart; _productRepo=productRepo; _db=db; _um=um; }

        public IActionResult Index(){ ViewBag.CartTotal=_cart.Total; return View(_cart.GetCart()); }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int qty=1)
        {
            var product=await _productRepo.GetByIdAsync(productId);
            if(product==null) return NotFound();
            _cart.AddItem(product,qty);
            TempData["Success"]=$"Đã thêm \"{product.Name}\" vào giỏ!";
            return RedirectToAction("Index");
        }

        [HttpPost] public IActionResult Update(int productId, int qty){ _cart.UpdateQty(productId,qty); return RedirectToAction("Index"); }
        [HttpPost] public IActionResult Remove(int productId){ _cart.RemoveItem(productId); TempData["Success"]="Đã xóa sản phẩm!"; return RedirectToAction("Index"); }

        [Authorize]
        public IActionResult Checkout()
        {
            var items=_cart.GetCart();
            if(!items.Any()){ TempData["Error"]="Giỏ hàng trống!"; return RedirectToAction("Index"); }
            ViewBag.CartItems=items; ViewBag.CartTotal=_cart.Total;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> PlaceOrder(
            string customerName,
            string address,
            string city,
            string phone,
            string? note,
            string? voucherCode,
            string paymentMethod = "COD")
        {
            var items = _cart.GetCart();
            if(!items.Any()) return RedirectToAction("Index");

            if(string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin giao hàng!";
                ViewBag.CartItems = items;
                ViewBag.CartTotal = _cart.Total;
                return View("Checkout");
            }

            var user = await _um.GetUserAsync(User);
            decimal originalTotal = _cart.Total;
            decimal total = originalTotal;
            int discountPercent = 0;

            // Áp dụng voucher (chỉ khi code hợp lệ)
            if(!string.IsNullOrWhiteSpace(voucherCode))
            {
                var voucher = await _db.Vouchers.FirstOrDefaultAsync(v =>
                    v.Code == voucherCode.Trim().ToUpper() &&
                    v.IsActive &&
                    v.ExpiryDate > DateTime.Now &&
                    v.UsedCount < v.UsageLimit);

                if(voucher != null)
                {
                    discountPercent = voucher.DiscountPercent;
                    total = Math.Round(originalTotal * (100 - discountPercent) / 100, 0);
                    voucher.UsedCount++;
                }
                else
                {
                    TempData["Error"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn!";
                    ViewBag.CartItems = items;
                    ViewBag.CartTotal = originalTotal;
                    ViewBag.VoucherCode = voucherCode;
                    return View("Checkout");
                }
            }

            string fullAddress = string.IsNullOrWhiteSpace(city)
                ? address.Trim()
                : $"{address.Trim()}, {city.Trim()}";

            var order = new Order
            {
                UserId       = user!.Id,
                CustomerName = customerName.Trim(),
                Address      = fullAddress,
                Phone        = phone.Trim(),
                Note         = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
                Total        = total,
                Status       = "Chờ xác nhận",
                OrderDate    = DateTime.Now
            };

            foreach(var item in items)
                order.Items.Add(new OrderItem
                {
                    ProductId   = item.ProductId,
                    ProductName = item.Name,
                    Price       = item.Price,
                    Quantity    = item.Quantity
                });

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            _cart.Clear();

            TempData["Success"]        = $"Đặt hàng thành công! Mã đơn: #TS{order.Id:D4}";
            TempData["OrderId"]        = order.Id.ToString();
            TempData["OrderTotal"]     = total.ToString("N0");
            TempData["OrderName"]      = customerName.Trim();
            TempData["OrderPhone"]     = phone.Trim();
            TempData["OrderAddress"]   = fullAddress;
            TempData["OrderCount"]     = items.Sum(i => i.Quantity).ToString();
            TempData["PaymentMethod"]  = paymentMethod;
            if(discountPercent > 0)
            {
                TempData["OrderDiscount"] = discountPercent.ToString();
                TempData["OrderOriginal"] = originalTotal.ToString("N0");
            }

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess() => View();
    }
}
