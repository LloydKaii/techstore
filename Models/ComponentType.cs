
using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class ComponentType
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public List<Component> Components { get; set; } = new();
        public List<PCBuildItem> PCBuildItems { get; set; } = new();
    }
}

