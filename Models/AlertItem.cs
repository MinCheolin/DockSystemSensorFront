
using Newtonsoft.Json;

namespace ShipyardDashboard.Models
{
    public class AlertItem
    {
        [JsonProperty("alertType")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("location")]
        public string Loc { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Value { get; set; } = string.Empty;
    }
}
