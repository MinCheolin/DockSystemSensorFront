using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class EquipmentGroup
    {
        [JsonProperty("groupName")]
        public string GroupName { get; set; } = string.Empty;

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("statusSummary")]
        public Dictionary<string, int> StatusSummary { get; set; } = new();

        [JsonProperty("keyDataSummary")]
        public Dictionary<string, string> KeyDataSummary { get; set; } = new();

        [JsonProperty("equipments")]
        public List<Equipment> Equipments { get; set; } = new();
    }
}
