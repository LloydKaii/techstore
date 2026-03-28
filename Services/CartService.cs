using System.Text.Json;
using TechStore.Models;

namespace TechStore.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _http;
        private const string Key = "Cart";

        public CartService(IHttpContextAccessor http) => _http = http;

        public List<CartItem> GetCart()
        {
            var json = _http.HttpContext?.Session.GetString(Key);
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
        }

        public void SaveCart(List<CartItem> cart)
        {
            _http.HttpContext?.Session.SetString(Key, JsonSerializer.Serialize(cart));
        }

        public void AddItem(Product product, int qty = 1)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == product.Id);
            if (item != null)
                item.Quantity += qty;
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name      = product.Name,
                    Price     = product.Price,
                    Quantity  = qty,
                    ImageUrl  = product.ImageUrl
                });
            SaveCart(cart);
        }

        public void RemoveItem(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(cart);
        }

        public void UpdateQty(int productId, int qty)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (qty <= 0) cart.Remove(item);
                else item.Quantity = qty;
            }
            SaveCart(cart);
        }

        public void Clear() => _http.HttpContext?.Session.Remove(Key);

        public int Count => GetCart().Sum(c => c.Quantity);
        public decimal Total => GetCart().Sum(c => c.Total);
    }
}
