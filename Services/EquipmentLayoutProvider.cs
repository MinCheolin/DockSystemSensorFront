
using ShipyardDashboard.Models;
using System.Collections.Generic;

namespace ShipyardDashboard.Services
{
    public static class EquipmentLayoutProvider
    {
        public class VisualMetricInfo
        {
            public VisualizationType VizType { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
        }

        public class Position
        {
            public string EquipmentName { get; set; } = string.Empty;
            public int Row { get; set; }
            public int Col { get; set; }
            public int RowSpan { get; set; } = 1;
            public int ColSpan { get; set; } = 1;
            public Dictionary<string, VisualMetricInfo>? VisualMetrics { get; set; }
        }

        private static readonly Dictionary<string, List<Position>> _layouts = new()
        {
            {
                "가공", new List<Position>
                {
                    new Position { EquipmentName = "CNC 플라즈마 절단기 #14", Row = 0, Col = 0, RowSpan = 2, ColSpan = 2, VisualMetrics = new() {
                        { "가동률", new VisualMetricInfo { VizType = VisualizationType.Gauge, Min = 0, Max = 100 } },
                        { "절단 속도", new VisualMetricInfo { VizType = VisualizationType.Sparkline } }
                    }},
                    new Position { EquipmentName = "CNC 플라즈마 절단기 #15", Row = 0, Col = 2, ColSpan = 2, VisualMetrics = new() {
                        { "가동률", new VisualMetricInfo { VizType = VisualizationType.Gauge, Min = 0, Max = 100 } }
                    }},
                    new Position { EquipmentName = "강판 벤딩기 #3", Row = 1, Col = 2 },
                    new Position { EquipmentName = "강판 벤딩기 #4", Row = 1, Col = 3 }
                }
            },
            {
                "조립", new List<Position>
                {
                    new Position { EquipmentName = "CO2 용접기", Row = 0, Col = 0, ColSpan = 4, VisualMetrics = new() {
                        { "전류", new VisualMetricInfo { VizType = VisualizationType.Sparkline } },
                        { "전압", new VisualMetricInfo { VizType = VisualizationType.Text } },
                        { "가스 유량", new VisualMetricInfo { VizType = VisualizationType.Text } }
                    }},
                    new Position { EquipmentName = "자동 용접 로봇 #7", Row = 1, Col = 0, RowSpan = 2, ColSpan = 2, VisualMetrics = new() {
                        { "용접 길이", new VisualMetricInfo { VizType = VisualizationType.Gauge, Min = 0, Max = 5000 } },
                        { "진행률", new VisualMetricInfo { VizType = VisualizationType.ProgressBar, Min = 0, Max = 100 } }
                    }},
                    new Position { EquipmentName = "자동 용접 로봇 #8", Row = 1, Col = 2 },
                    new Position { EquipmentName = "자동 용접 로봇 #9", Row = 1, Col = 3 }
                }
            }
        };

        public static List<Position> GetLayoutForProcess(string processName)
        {
            if (_layouts.TryGetValue(processName, out var layout))
            {
                return layout;
            }
            return new List<Position>(); // Return empty list if no specific layout is defined
        }
    }
}
