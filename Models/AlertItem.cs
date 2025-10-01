
using Newtonsoft.Json;

namespace ShipyardDashboard.Models
{
    public class AlertItem
    {
        [JsonProperty("alertType")]
        public string AlertType { get; set; } = string.Empty;

        [JsonProperty("location")]
        public string Location { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
