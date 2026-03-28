using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TechStore.Models
{
    public class PCBuild
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Display(Name = "Tên Build")]
        [StringLength(100)]
        public string Name { get; set; } = "My PC Build";

        [Display(Name = "CPU")]
        public int? CPUId { get; set; }
        public Product? CPU { get; set; }

        [Display(Name = "Mainboard")]
        public int? MainboardId { get; set; }
        public Product? Mainboard { get; set; }

        [Display(Name = "RAM")]
        public int? RAMId { get; set; }
        public Product? RAM { get; set; }

        [Display(Name = "VGA")]
        public int? VGAId { get; set; }
        public Product? VGA { get; set; }

        [Display(Name = "PSU")]
        public int? PSUId { get; set; }
        public Product? PSU { get; set; }

        [Display(Name = "Case")]
        public int? CaseId { get; set; }
        public Product? Case { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng giá")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
    }
}

