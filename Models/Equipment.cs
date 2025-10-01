using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public partial class Equipment : ObservableObject
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("status")]
        [ObservableProperty]
        private string _status = string.Empty;

        [JsonProperty("details")]
        public Dictionary<string, DetailValue> Details { get; set; } = new();

        // Property for Predictive Maintenance
        [ObservableProperty]
        private HealthStatus _health = new();
    }
}