// ============================================================
// Models/Category.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(50, ErrorMessage = "Tên tối đa 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        // Navigation
        public List<Product>? Products { get; set; }
    }
}

// ============================================================
// Models/ProductImage.cs
// ============================================================
// using System.ComponentModel.DataAnnotations;
// namespace TechStore.Models
// {
//     public class ProductImage
//     {
//         public int Id { get; set; }
//         public string Url { get; set; } = string.Empty;
//         public int ProductId { get; set; }
//         public Product? Product { get; set; }
//     }
// }
