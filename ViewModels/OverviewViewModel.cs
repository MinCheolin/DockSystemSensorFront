using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using ShipyardDashboard.Services;
using System.Collections.ObjectModel;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using OxyPlot.Annotations;
using System.Linq;

namespace ShipyardDashboard.ViewModels
{
    public partial class ShipBlockViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _blockId = "";

        [ObservableProperty]
        private string _status = "";

        [ObservableProperty]
        private int _progress;

        public string TooltipText => new StringBuilder()
            .AppendLine($"블록 ID: {BlockId}")
            .AppendLine($"상태: {Status}")
            .AppendLine($"진행률: {Progress}%")
            .ToString();
    }

    public partial class OverviewViewModel : ObservableObject, IDisposable
    {
        private readonly ApiService _apiService;
        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private ObservableCollection<ShipBlockViewModel> _shipBlocks = new();
        
        [ObservableProperty] 
        private PlotModel _monthlyProductionPlot = new();
        
        [ObservableProperty] 
        private PlotModel _operatingRateGauge = new();
        
        [ObservableProperty] 
        private OtherEquipmentViewModel _otherEquipment = new();

        public OverviewViewModel()
        {
            // This constructor is for the design-time preview in Visual Studio.
             _apiService = new ApiService(); // Use a dummy service
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                UpdateOperatingRateGauge(89.5);
                UpdateMonthlyProductionPlot(new List<Overview.ChartDataPoint>());
            }
        }

        public OverviewViewModel(ApiService apiService)
        {
            _apiService = apiService;
            
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _timer.Tick += async (s, e) => await LoadDataAsync();
            _timer.Start();
            
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var overviewData = await _apiService.GetOverviewAsync();
            if (overviewData != null)
            {
                UpdateOperatingRateGauge(overviewData.EquipmentOperatingRate);
                UpdateMonthlyProductionPlot(overviewData.MonthlyProduction);

                // This part is just for visualization as the backend doesn't provide this specific data yet.
                // In a real scenario, this would also come from the backend.
                UpdateShipBlocks(); 
                UpdateOtherEquipment(overviewData.OtherEquipmentStatus);
            }
        }

        private void UpdateShipBlocks()
        {
            // Keep the random block generation for visual flair, as it's not the focus of the bug.
            if (ShipBlocks.Any()) return; // Don't re-generate if already populated.

            var random = new System.Random();
            var statuses = new[] { "완료", "진행중", "미시작", "오류" };
            int blockNumber = 1;
            var tempBlocks = new ObservableCollection<ShipBlockViewModel>();
            for (int i = 0; i < 24; i++)
            {
                var status = statuses[random.Next(statuses.Length)];
                int progress = 0;
                if (status == "완료") progress = 100;
                else if (status == "진행중") progress = random.Next(20, 80);
                else if (status == "오류") progress = random.Next(30, 90);

                tempBlocks.Add(new ShipBlockViewModel
                {
                    BlockId = $"A-{blockNumber++}",
                    Status = status,
                    Progress = progress
                });
            }
            ShipBlocks = tempBlocks;
        }
        
        private void UpdateOtherEquipment(List<Overview.SimpleEquipmentStatus> equipmentStatuses)
        {
            var mainCompressor = equipmentStatuses?.FirstOrDefault(e => e.Name.Contains("공기 압축기"));
            var dustCollector = equipmentStatuses?.FirstOrDefault(e => e.Name.Contains("집진기"));

            OtherEquipment = new OtherEquipmentViewModel
            {
                MainCompressorStatus = mainCompressor?.Status ?? "N/A",
                CentralDustCollectorStatus = dustCollector?.Status ?? "N/A"
            };
        }

        private void UpdateMonthlyProductionPlot(List<Overview.ChartDataPoint> productionData)
        {
            var plotModel = new PlotModel { PlotAreaBorderColor = OxyColors.Transparent, TextColor = OxyColor.FromRgb(100, 100, 100) };
            var lineSeries = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerStroke = OxyColors.DodgerBlue, MarkerFill = OxyColors.White, MarkerStrokeThickness = 1.5, Color = OxyColors.DodgerBlue, StrokeThickness = 2 };
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, IsAxisVisible = true, MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None };
            var valueAxis = new LinearAxis { Position = AxisPosition.Left, IsAxisVisible = true, MinimumPadding = 0.1, MaximumPadding = 0.1, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.None, MajorGridlineColor = OxyColor.FromAColor(40,OxyColors.Black) };

            if (productionData != null)
            {
                for (int i = 0; i < productionData.Count; i++)
                {
                    lineSeries.Points.Add(new DataPoint(i, productionData[i].Value));
                    categoryAxis.Labels.Add(productionData[i].Date);
                }
            }
            categoryAxis.LabelFormatter = (index) => { int idx = (int)index; if (idx >= 0 && idx < categoryAxis.Labels.Count && idx % 3 == 0) { return categoryAxis.Labels[idx]; } return null; };

            plotModel.Series.Add(lineSeries);
            plotModel.Axes.Add(categoryAxis);
            plotModel.Axes.Add(valueAxis);
            MonthlyProductionPlot = plotModel;
        }

        private void UpdateOperatingRateGauge(double value)
        {
            var gaugeModel = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0), Background = OxyColors.Transparent };
            var series = new PieSeries { StartAngle = 270, AngleSpan = 360, InnerDiameter = 0.7, StrokeThickness = 0 };
            double normalizedValue = Math.Clamp(value, 0, 100);
            OxyColor valueColor = (normalizedValue < 70) ? OxyColor.FromRgb(231, 76, 60) : (normalizedValue < 85) ? OxyColor.FromRgb(241, 196, 15) : OxyColor.FromRgb(46, 204, 113);
            series.Slices.Add(new PieSlice("", normalizedValue) { Fill = valueColor });
            series.Slices.Add(new PieSlice("", 100 - normalizedValue) { Fill = OxyColor.FromRgb(236, 240, 241) });
            gaugeModel.Series.Add(series);
            gaugeModel.Annotations.Add(new TextAnnotation { Text = $"{value:N1}%", Font = "Segoe UI", FontSize = 24, FontWeight = FontWeights.Bold, TextColor = OxyColor.FromRgb(44, 62, 80), TextPosition = new DataPoint(0, 0), TextHorizontalAlignment = HorizontalAlignment.Center, TextVerticalAlignment = VerticalAlignment.Middle });
            OperatingRateGauge = gaugeModel;
        }

        public void Dispose()
        {
            _timer?.Stop();
        }
    }
}
