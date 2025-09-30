using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using ShipyardDashboard.Services;
using ShipyardDashboard.ViewModels.Base; // For RelayCommand
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System.Linq;
using OxyPlot.Axes;
using System;

namespace ShipyardDashboard.ViewModels
{
    public partial class OverviewViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private PlotModel _monthlyProductionPlot;

        [ObservableProperty]
        private PlotModel _operatingRateGauge;

        [ObservableProperty]
        private double _equipmentOperatingRate;

        [ObservableProperty]
        private Dictionary<string, double> _environmentData;

        [ObservableProperty]
        private ObservableCollection<Overview.SimpleEquipmentStatus> _otherEquipmentStatus;

        [ObservableProperty]
        private ObservableCollection<DigitalTwinObject> _digitalTwinObjects;

        public ICommand NavigateToProcessCommand { get; private set; }

        public OverviewViewModel()
        {
            _apiService = new ApiService();
            _monthlyProductionPlot = new PlotModel { Title = "월별 생산량" };
            _operatingRateGauge = new PlotModel();
            _environmentData = new Dictionary<string, double>();
            _otherEquipmentStatus = new ObservableCollection<Overview.SimpleEquipmentStatus>();
            _digitalTwinObjects = new ObservableCollection<DigitalTwinObject>();

            NavigateToProcessCommand = new RelayCommand<string>(NavigateToProcess);

            _ = LoadOverviewDataAsync();
        }

        private async Task LoadOverviewDataAsync()
        {
            try
            {
                var overviewData = await _apiService.GetOverviewAsync();
                if (overviewData != null)
                {
                    EquipmentOperatingRate = overviewData.EquipmentOperatingRate;
                    EnvironmentData = new Dictionary<string, double>(overviewData.EnvironmentData);
                    OtherEquipmentStatus = new ObservableCollection<Overview.SimpleEquipmentStatus>(overviewData.OtherEquipmentStatus);
                    DigitalTwinObjects = new ObservableCollection<DigitalTwinObject>(overviewData.DigitalTwinObjects);

                    UpdateMonthlyProductionPlot(overviewData.MonthlyProduction);
                    UpdateOperatingRateGauge(overviewData.EquipmentOperatingRate);
                }
            }
            catch (System.Exception ex)
            {
                // Log the exception or show a message to the user
                System.Diagnostics.Debug.WriteLine($"Failed to load overview data: {ex.Message}");
            }
        }

                private void UpdateMonthlyProductionPlot(List<Overview.ChartDataPoint> productionData)
                {
                    var plotModel = new PlotModel 
                    {
                        Title = "최근 30일 생산량",
                        PlotAreaBorderColor = OxyColors.Transparent,
                        TextColor = OxyColor.FromRgb(100, 100, 100)
                    };
        
                    var lineSeries = new LineSeries
                    {
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 3,
                        MarkerStroke = OxyColors.DodgerBlue,
                        MarkerFill = OxyColors.White,
                        MarkerStrokeThickness = 1.5,
                        Color = OxyColors.DodgerBlue,
                        StrokeThickness = 2,
                    };
        
                    var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, IsAxisVisible = true };
                    var valueAxis = new LinearAxis { Position = AxisPosition.Left, IsAxisVisible = true, MinimumPadding = 0.1, MaximumPadding = 0.1 };
        
                    if (productionData != null)
                    {
                        for (int i = 0; i < productionData.Count; i++)
                        {
                            lineSeries.Points.Add(new OxyPlot.DataPoint(i, productionData[i].Value));
                            categoryAxis.Labels.Add(productionData[i].Date);
                        }
                    }
        
                    categoryAxis.LabelFormatter = (index) =>
                    {
                        int idx = (int)index;
                        if (idx >= 0 && idx < categoryAxis.Labels.Count && idx % 7 == 0)
                        {
                            return categoryAxis.Labels[idx];
                        }
                        return null;
                    };
        
                    plotModel.Series.Add(lineSeries);
                    plotModel.Axes.Add(categoryAxis);
                    plotModel.Axes.Add(valueAxis);
        
                    MonthlyProductionPlot = plotModel;
                }
        private void UpdateOperatingRateGauge(double value)
        {
            var gaugeModel = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0), Background = OxyColors.Transparent };

            var series = new PieSeries
            {
                StartAngle = 270,
                AngleSpan = 360,
                InnerDiameter = 0.7,
                StrokeThickness = 0,
            };

            double normalizedValue = Math.Clamp(value, 0, 100);

            OxyColor valueColor;
            if (normalizedValue < 70) valueColor = OxyColor.FromRgb(231, 76, 60); // Red
            else if (normalizedValue < 85) valueColor = OxyColor.FromRgb(241, 196, 15); // Yellow
            else valueColor = OxyColor.FromRgb(46, 204, 113); // Green

            series.Slices.Add(new PieSlice("", normalizedValue) { Fill = valueColor });
            series.Slices.Add(new PieSlice("", 100 - normalizedValue) { Fill = OxyColor.FromRgb(236, 240, 241) });

            gaugeModel.Series.Add(series);

            // Add percentage text as an annotation in the center
            gaugeModel.Annotations.Add(new TextAnnotation
            {
                Text = $"{value:N1}%",
                Font = "Segoe UI",
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                TextColor = OxyColor.FromRgb(44, 62, 80), // Corresponds to LightThemePrimaryTextBrush
                TextPosition = new OxyPlot.DataPoint(0, 0),
                TextHorizontalAlignment = HorizontalAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Middle
            });

            OperatingRateGauge = gaugeModel;
        }

        private void NavigateToProcess(string? processName)
        {
            if (string.IsNullOrEmpty(processName)) return;
            // This event will be handled by MainViewModel
            MainViewModel.RequestNavigation(processName);
        }
    }
}
