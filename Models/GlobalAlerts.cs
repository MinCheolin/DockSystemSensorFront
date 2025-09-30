
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class GlobalAlerts
    {
        [JsonProperty("overallStatus")]
        public string OverallStatus { get; set; } = "Normal";

        [JsonProperty("alertSummary")]
        public Dictionary<string, long> AlertSummary { get; set; } = new();

        [JsonProperty("alerts")]
        public List<AlertItem> Alerts { get; set; } = new();
    }
}
