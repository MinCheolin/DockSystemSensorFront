using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using System.Collections.ObjectModel;

namespace ShipyardDashboard.Models
{
    public partial class HealthStatus : ObservableObject
    {
        [ObservableProperty]
        private string _status = "정상"; // e.g., 정상, 주의, 위험

        [ObservableProperty]
        private PlotModel _vibrationPlotModel;

        [ObservableProperty]
        private PlotModel _currentPlotModel;

        public ObservableCollection<OxyPlot.DataPoint> VibrationData { get; } = new();
        public ObservableCollection<OxyPlot.DataPoint> CurrentData { get; } = new();

        public HealthStatus()
        {
            _vibrationPlotModel = CreateSparklinePlotModel();
            _currentPlotModel = CreateSparklinePlotModel();
        }

        private PlotModel CreateSparklinePlotModel()
        {
            return new PlotModel
            {
                Axes = {
                    new OxyPlot.Axes.LinearAxis { IsAxisVisible = false, Position = OxyPlot.Axes.AxisPosition.Bottom },
                    new OxyPlot.Axes.LinearAxis { IsAxisVisible = false, Position = OxyPlot.Axes.AxisPosition.Left }
                },
                PlotAreaBorderThickness = new OxyThickness(0),
                Padding = new OxyThickness(0),
                PlotMargins = new OxyThickness(0)
            };
        }
    }
}
