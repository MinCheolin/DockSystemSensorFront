using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace ShipyardDashboard.Models
{
    public partial class DigitalTwinObject : ObservableObject
    {
        [JsonProperty("id")]
        [ObservableProperty]
        private string _id = string.Empty;

        [JsonProperty("name")]
        [ObservableProperty]
        private string _name = string.Empty;

        [JsonProperty("type")]
        [ObservableProperty]
        private string _type = string.Empty;

        [JsonProperty("x")]
        [ObservableProperty]
        private double _x;

        [JsonProperty("y")]
        [ObservableProperty]
        private double _y;

        [JsonProperty("status")]
        [ObservableProperty]
        private string _status = string.Empty;

        // Properties for animation, not from API
        [JsonIgnore]
        public double TargetX { get; set; }

        [JsonIgnore]
        public double TargetY { get; set; }

        [JsonIgnore]
        public double Speed { get; set; } = 1.0; // Default speed
    }
}
