using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Series;

namespace ShipyardDashboard.ViewModels
{
    public partial class EquipmentGroupViewModel : ObservableObject
    {
        public EquipmentGroup EquipmentGroup { get; private set; }

        [ObservableProperty]
        private string _groupName;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private Dictionary<string, int> _statusSummary;

        [ObservableProperty]
        private Dictionary<string, string> _keyDataSummary;

        [ObservableProperty]
        private PlotModel _statusPlotModel;

        public EquipmentGroupViewModel(EquipmentGroup equipmentGroup)
        {
            EquipmentGroup = equipmentGroup;
            _groupName = equipmentGroup.GroupName;
            _statusPlotModel = new PlotModel();
            UpdateGroup(equipmentGroup); // Use UpdateGroup to initialize all properties
        }

        public void UpdateGroup(EquipmentGroup newGroup)
        {
            EquipmentGroup = newGroup;
            GroupName = newGroup.GroupName;
            TotalCount = newGroup.TotalCount;
            StatusSummary = newGroup.StatusSummary;
            KeyDataSummary = newGroup.KeyDataSummary;
            
            UpdateStatusPlotModel();
        }

        private void UpdateStatusPlotModel()
        {
            var plotModel = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0), Background = OxyColors.Transparent };
            var series = new PieSeries
            {
                StrokeThickness = 1,
                Stroke = OxyColors.White,
                InnerDiameter = 0.6,
                StartAngle = -90,
                AngleSpan = 360,
                FontSize = 12,
                OutsideLabelFormat = "{1}", // Show status name
                InsideLabelFormat = null
            };

            if (StatusSummary != null)
            {
                foreach (var entry in StatusSummary.Where(s => s.Value > 0))
                {
                    series.Slices.Add(new PieSlice(entry.Key, entry.Value) { Fill = OxyColor.FromRgb(GetStatusColor(entry.Key).R, GetStatusColor(entry.Key).G, GetStatusColor(entry.Key).B) });
                }
            }

            plotModel.Series.Add(series);
            StatusPlotModel = plotModel;
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "가동" or "작업 중" or "정상" or "분사 중" or "이동 중" => (Color)ColorConverter.ConvertFromString("#4CAF50"), // Green
                "대기" => (Color)ColorConverter.ConvertFromString("#FFC107"), // Amber
                "정지" => (Color)ColorConverter.ConvertFromString("#9E9E9E"), // Grey
                "오류" or "위험" => (Color)ColorConverter.ConvertFromString("#F44336"), // Red
                _ => Colors.Gray,
            };
        }
    }
}