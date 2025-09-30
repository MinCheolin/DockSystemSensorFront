
namespace ShipyardDashboard.Models
{
    public class DataPoint
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty; // Using string to be flexible (e.g., "Online", "75%", "1,200 RPM")
        public VisualizationType Visualization { get; set; }
    }
}
