
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class Equipment
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("details")]
        public Dictionary<string, DetailValue> Details { get; set; } = new();
    }
}
