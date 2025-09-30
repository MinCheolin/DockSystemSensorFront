using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class Overview
    {
        [JsonProperty("monthlyProduction")]
        public List<ChartDataPoint> MonthlyProduction { get; set; } = new();

        [JsonProperty("equipmentOperatingRate")]
        public double EquipmentOperatingRate { get; set; }

        [JsonProperty("environmentData")]
        public Dictionary<string, double> EnvironmentData { get; set; } = new();

        [JsonProperty("otherEquipmentStatus")]
        public List<SimpleEquipmentStatus> OtherEquipmentStatus { get; set; } = new();

        [JsonProperty("digitalTwinObjects")]
        public List<DigitalTwinObject> DigitalTwinObjects { get; set; } = new();

        public class ChartDataPoint
        {
            [JsonProperty("date")]
            public string Date { get; set; } = string.Empty;

            [JsonProperty("value")]
            public double Value { get; set; }
        }

        public class SimpleEquipmentStatus
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("status")]
            public string Status { get; set; } = string.Empty;
        }
    }
}
