
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStore.Models
{
    public class Component
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Specs { get; set; } = string.Empty; // JSON socket/form-factor

        [Required]
        public int ComponentTypeId { get; set; }

        public int StockQuantity { get; set; } = 100;

        // Navigation
        public ComponentType? ComponentType { get; set; }
        public List<PCBuildItem> PCBuildItems { get; set; } = new();
    }
}

