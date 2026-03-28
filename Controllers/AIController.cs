using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;
using TechStore.Services;

namespace TechStore.Controllers
{
    public class AIController : Controller
    {
        private readonly AIService _ai;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ApplicationDbContext _db;

        public AIController(AIService ai, IProductRepository p, ICategoryRepository c, ApplicationDbContext db)
        { _ai=ai; _productRepo=p; _categoryRepo=c; _db=db; }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if(string.IsNullOrWhiteSpace(q)) return Json(new{results=new List<object>()});
            var all = await _productRepo.GetAllAsync();
            var results = _ai.SmartSearch(q,all).Select(p=>new{id=p.Id,name=p.Name,price=p.Price.ToString("N0")+"₫",image=p.ImageUrl,category=p.Category?.Name??"",url=$"/Product/Display/{p.Id}"});
            return Json(new{results});
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            if(string.IsNullOrWhiteSpace(req?.Message)) return Json(new{reply="Bạn muốn hỏi gì ạ?"});
            var products   = await _productRepo.GetAllAsync();
            var categories = await _categoryRepo.GetAllAsync();
            var reply      = _ai.ChatResponse(req.Message,products,categories);
            return Json(new{reply});
        }

        [HttpGet]
        public async Task<IActionResult> CheckVoucher(string code)
        {
            if(string.IsNullOrWhiteSpace(code)) return Json(new{valid=false,message="Vui lòng nhập mã voucher"});
            var v = await _db.Vouchers.FirstOrDefaultAsync(x=>x.Code==code.ToUpper()&&x.IsActive&&x.ExpiryDate>DateTime.Now&&x.UsedCount<x.UsageLimit);
            if(v==null) return Json(new{valid=false,message="Mã không hợp lệ hoặc đã hết hạn"});
            return Json(new{valid=true,percent=v.DiscountPercent,message=$"Áp dụng thành công! Giảm {v.DiscountPercent}%"});
        }
    }

    public class ChatRequest { public string? Message { get; set; } }
}
