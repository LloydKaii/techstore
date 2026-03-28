using TechStore.Models;

namespace TechStore.Services
{
    /// <summary>
    /// AI Service - Rule-based thông minh (không cần API key).
    /// Khi có Anthropic API key: uncomment phần USE_API_KEY và thêm key vào appsettings.json
    /// </summary>
    public class AIService
    {
        // ══════════════════════════════════════════════
        // 1. GỢI Ý SẢN PHẨM THÔNG MINH
        // ══════════════════════════════════════════════
        public List<Product> GetRecommendations(Product current, IEnumerable<Product> all)
        {
            var others = all.Where(p => p.Id != current.Id).ToList();

            // Rule 1: Cùng danh mục → điểm cao nhất
            var sameCat = others
                .Where(p => p.CategoryId == current.CategoryId)
                .OrderBy(p => Math.Abs(p.Price - current.Price))
                .Take(3).ToList();

            // Rule 2: Khoảng giá tương đương (±30%)
            var priceRange = others
                .Where(p => p.CategoryId != current.CategoryId
                    && p.Price >= current.Price * 0.7m
                    && p.Price <= current.Price * 1.3m)
                .OrderByDescending(p => p.Price)
                .Take(2).ToList();

            var result = sameCat.Concat(priceRange)
                .DistinctBy(p => p.Id)
                .Take(4).ToList();

            // Fallback: lấy ngẫu nhiên nếu không đủ
            if (result.Count < 2)
                result = others.OrderBy(_ => Guid.NewGuid()).Take(4).ToList();

            return result;
        }

        // ══════════════════════════════════════════════
        // 2. TÌM KIẾM THÔNG MINH (fuzzy search)
        // ══════════════════════════════════════════════
        public List<Product> SmartSearch(string query, IEnumerable<Product> all)
        {
            if (string.IsNullOrWhiteSpace(query)) return new();

            var q = query.ToLower().Trim();
            var words = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return all
                .Select(p => new
                {
                    Product = p,
                    Score   = CalcScore(p, q, words)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Product)
                .Take(20)
                .ToList();
        }

        private int CalcScore(Product p, string q, string[] words)
        {
            int score = 0;
            var name = p.Name.ToLower();
            var desc = (p.Description ?? "").ToLower();

            // Khớp chính xác → điểm cao nhất
            if (name.Contains(q)) score += 100;
            if (desc.Contains(q)) score += 30;

            // Khớp từng từ
            foreach (var w in words)
            {
                if (name.Contains(w))  score += 50;
                if (desc.Contains(w))  score += 15;
            }

            // Fuzzy: so sánh ký tự gần giống (xử lý typo)
            foreach (var w in words)
                if (w.Length >= 3 && FuzzyMatch(name, w)) score += 20;

            return score;
        }

        private bool FuzzyMatch(string text, string word)
        {
            // Tìm substring với tối đa 1 ký tự khác
            for (int i = 0; i <= text.Length - word.Length; i++)
            {
                int diff = 0;
                for (int j = 0; j < word.Length; j++)
                    if (text[i + j] != word[j]) diff++;
                if (diff <= 1) return true;
            }
            return false;
        }

        // ══════════════════════════════════════════════
        // 3. CHATBOT THÔNG MINH
        // ══════════════════════════════════════════════
        public string ChatResponse(string message, IEnumerable<Product> products, IEnumerable<Category> categories)
        {
            var msg = message.ToLower().Trim();

            // === Chào hỏi ===
            if (ContainsAny(msg, "xin chào", "hello", "hi", "chào", "hey"))
                return "Xin chào! 👋 Tôi là TechBot, trợ lý mua sắm của TechStore. Tôi có thể giúp bạn tìm sản phẩm, tư vấn cấu hình, hoặc trả lời câu hỏi về cửa hàng. Bạn cần gì ạ?";

            // === Hỏi về sản phẩm cụ thể ===
            foreach (var p in products)
            {
                if (msg.Contains(p.Name.ToLower()) || p.Name.ToLower().Split(' ').Any(w => w.Length > 3 && msg.Contains(w.ToLower())))
                {
                    return $"📦 **{p.Name}**\n" +
                           $"💰 Giá: {p.Price:N0}₫\n" +
                           $"📝 {p.Description ?? "Sản phẩm chất lượng cao"}\n" +
                           $"✅ Còn hàng | 🛡️ Bảo hành 12 tháng\n\n" +
                           $"Bạn muốn [xem chi tiết](/Product/Display/{p.Id}) hay thêm vào giỏ hàng không?";
                }
            }

            // === Hỏi về giá / ngân sách ===
            if (ContainsAny(msg, "giá rẻ", "rẻ nhất", "tiết kiệm", "budget"))
            {
                var cheap = products.OrderBy(p => p.Price).Take(3);
                var list  = string.Join("\n", cheap.Select(p => $"• {p.Name}: {p.Price:N0}₫"));
                return $"💡 Sản phẩm giá tốt nhất hiện tại:\n{list}\n\nBạn muốn tìm hiểu thêm sản phẩm nào?";
            }

            if (ContainsAny(msg, "cao cấp", "xịn nhất", "tốt nhất", "premium", "flagship"))
            {
                var top = products.OrderByDescending(p => p.Price).Take(3);
                var list = string.Join("\n", top.Select(p => $"• {p.Name}: {p.Price:N0}₫"));
                return $"⭐ Sản phẩm cao cấp nhất của chúng tôi:\n{list}\n\nBạn có muốn tôi tư vấn thêm không?";
            }

            // === Hỏi về danh mục ===
            foreach (var cat in categories)
            {
                if (msg.Contains(cat.Name.ToLower()))
                {
                    var inCat = products.Where(p => p.CategoryId == cat.Id).Take(3);
                    var list  = string.Join("\n", inCat.Select(p => $"• {p.Name}: {p.Price:N0}₫"));
                    return $"🏷️ Danh mục **{cat.Name}** có {products.Count(p => p.CategoryId == cat.Id)} sản phẩm:\n{list}\n\n[Xem tất cả](/Product)";
                }
            }

            // === Hỏi về laptop ===
            if (ContainsAny(msg, "laptop", "máy tính", "notebook"))
            {
                var laptops = products.Where(p =>
                    p.Name.ToLower().Contains("laptop") ||
                    p.Category?.Name.ToLower().Contains("laptop") == true).Take(4);
                if (laptops.Any())
                {
                    var list = string.Join("\n", laptops.Select(p => $"• {p.Name}: {p.Price:N0}₫"));
                    return $"💻 Các laptop đang có:\n{list}\n\nBạn dùng để làm gì? (học tập, văn phòng, gaming) để tôi tư vấn phù hợp hơn nhé!";
                }
            }

            // === Hỏi về gaming ===
            if (ContainsAny(msg, "gaming", "game", "chơi game", "chiến game"))
                return "🎮 Để chơi game tốt bạn cần:\n• Laptop/PC có card đồ họa rời (GTX/RTX)\n• RAM tối thiểu 16GB\n• SSD để load nhanh\n\nNgân sách bạn khoảng bao nhiêu để tôi tư vấn cụ thể?";

            // === Hỏi về bảo hành ===
            if (ContainsAny(msg, "bảo hành", "warranty", "hỏng", "lỗi"))
                return "🛡️ Chính sách bảo hành TechStore:\n• Bảo hành 12 tháng chính hãng\n• Đổi trả 30 ngày nếu lỗi nhà sản xuất\n• Hỗ trợ kỹ thuật 24/7\n\nBạn có vấn đề gì cần hỗ trợ không?";

            // === Hỏi về giao hàng ===
            if (ContainsAny(msg, "giao hàng", "ship", "vận chuyển", "nhận hàng"))
                return "🚚 Thông tin giao hàng:\n• Giao hàng toàn quốc trong 24-48h\n• Miễn phí vận chuyển cho đơn từ 500.000₫\n• Đóng gói cẩn thận, chống va đập\n\nBạn cần giao đến đâu?";

            // === Hỏi về thanh toán ===
            if (ContainsAny(msg, "thanh toán", "trả tiền", "payment", "cod", "qr"))
                return "💳 Phương thức thanh toán:\n• COD - Thanh toán khi nhận hàng\n• QR Code - Chuyển khoản nhanh\n• Momo, ZaloPay, VNPay\n\nBạn muốn dùng phương thức nào?";

            // === Đếm sản phẩm ===
            if (ContainsAny(msg, "có bao nhiêu", "tổng cộng", "bao nhiêu sản phẩm"))
                return $"📊 TechStore hiện có:\n• {products.Count()} sản phẩm\n• {categories.Count()} danh mục\n• Cập nhật liên tục mỗi ngày!\n\n[Xem tất cả sản phẩm](/Product)";

            // === Cảm ơn ===
            if (ContainsAny(msg, "cảm ơn", "thank", "thanks", "ok", "được rồi"))
                return "😊 Không có gì! Chúc bạn mua sắm vui vẻ tại TechStore. Nếu cần tư vấn thêm cứ hỏi tôi nhé!";

            // === Tạm biệt ===
            if (ContainsAny(msg, "tạm biệt", "bye", "goodbye", "thôi"))
                return "👋 Tạm biệt! Hẹn gặp lại bạn tại TechStore. Chúc bạn một ngày tốt lành!";

            // === Mặc định ===
            var randomProducts = products.OrderBy(_ => Guid.NewGuid()).Take(2);
            var suggest = string.Join(", ", randomProducts.Select(p => p.Name));
            return $"🤔 Tôi chưa hiểu rõ câu hỏi của bạn. Bạn có thể hỏi tôi về:\n" +
                   $"• Tìm sản phẩm theo tên hoặc danh mục\n" +
                   $"• Tư vấn theo ngân sách\n" +
                   $"• Thông tin bảo hành, giao hàng\n\n" +
                   $"Gợi ý hôm nay: {suggest}";
        }

        private bool ContainsAny(string text, params string[] keywords)
            => keywords.Any(k => text.Contains(k));
    }
}
