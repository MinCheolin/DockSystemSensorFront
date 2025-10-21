
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class GlobalAlerts
    {
        [JsonProperty("overallStatus")]
        public string OverallStatus { get; set; } = "Normal";


        [JsonProperty("alerts")]
        public List<AlertItem> Alerts { get; set; } = new();
    }
}
