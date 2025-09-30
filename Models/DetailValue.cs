
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    // This class must match the structure sent by the backend inside the EquipmentDto's details map.
    public class DetailValue
    {
        [JsonProperty("display")]
        public string Display { get; set; } = string.Empty;

        [JsonProperty("value")]
        public double? Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; } = string.Empty;

        [JsonProperty("seriesData")]
        public List<double>? SeriesData { get; set; }
    }
}
