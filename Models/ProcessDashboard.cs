
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class ProcessDashboard
    {
        [JsonProperty("processName")]
        public string ProcessName { get; set; } = string.Empty;

        [JsonProperty("equipmentGroups")]
        public List<EquipmentGroup> EquipmentGroups { get; set; } = new();
    }
}
