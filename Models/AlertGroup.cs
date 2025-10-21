
using ShipyardDashboard.Models;
using System.Collections.Generic;

namespace ShipyardDashboard.Models
{
    public class AlertGroup
    {
        public string Category { get; set; } = string.Empty;
        public int AlertCount { get; set; }
        public List<AlertItem> Alerts { get; set; } = new();
    }
}
