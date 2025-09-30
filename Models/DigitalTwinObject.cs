using Newtonsoft.Json;

namespace ShipyardDashboard.Models
{
    public class DigitalTwinObject
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}
