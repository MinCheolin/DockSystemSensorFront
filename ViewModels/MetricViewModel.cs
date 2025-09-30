
using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using ShipyardDashboard.Models;

namespace ShipyardDashboard.ViewModels
{
    public partial class MetricViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _displayValue;

        [ObservableProperty]
        private PlotModel? _plotModel;

        [ObservableProperty]
        private string _unit;

        public VisualizationType VizType { get; }
        public double? NumericValue { get; }

        public MetricViewModel(string name, string displayValue, string unit, VisualizationType vizType, double? numericValue = null, PlotModel? plotModel = null)
        {
            _name = name;
            _displayValue = displayValue;
            _unit = unit;
            VizType = vizType;
            NumericValue = numericValue;
            _plotModel = plotModel;
        }
    }
}
