using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0.01, 999999999, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá (VNĐ)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh đại diện")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public List<ProductImage>? Images { get; set; }

        // Helper property for views
        public string CategoryName => Category?.Name ?? "Khác";
    }
}

