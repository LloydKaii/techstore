using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required, StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        public string Status { get; set; } = "Chờ xác nhận";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string? Note { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id         { get; set; }
        public int OrderId    { get; set; }
        public Order? Order   { get; set; }

        public int ProductId  { get; set; }
        public Product? Product { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity  { get; set; }

        public decimal Total => Price * Quantity;
    }
}
