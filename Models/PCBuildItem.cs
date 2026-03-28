
namespace TechStore.Models
{
    public class PCBuildItem
    {
        public int Id { get; set; }
        public int PCBuildId { get; set; }
        public int ComponentId { get; set; }

        public PCBuild? PCBuild { get; set; }
        public Component? Component { get; set; }
    }
}

