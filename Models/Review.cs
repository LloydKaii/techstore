using System.ComponentModel.DataAnnotations;
namespace TechStore.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required] public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required] public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        [Range(1,5)] public int Rating { get; set; } = 5;
        [Required, StringLength(500)] public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class WishlistItem
    {
        public int Id { get; set; }
        [Required] public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        [Required] public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }

    public class Voucher
    {
        public int Id { get; set; }
        [Required, StringLength(20)] public string Code { get; set; } = string.Empty;
        public int DiscountPercent { get; set; } = 10;
        public bool IsActive { get; set; } = true;
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(30);
        public int UsageLimit { get; set; } = 100;
        public int UsedCount  { get; set; } = 0;
    }
}
