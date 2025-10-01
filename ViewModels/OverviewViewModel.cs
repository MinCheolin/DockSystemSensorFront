using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using System.Collections.ObjectModel;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using OxyPlot.Annotations;

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

    public partial class OverviewViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ShipBlockViewModel> _shipBlocks = new();
        
        // Restored properties for KPIs
        [ObservableProperty] private OxyPlot.PlotModel _monthlyProductionPlot = new();
        [ObservableProperty] private OxyPlot.PlotModel _operatingRateGauge = new();
        [ObservableProperty] private OtherEquipmentViewModel _otherEquipment = new();

        public OverviewViewModel()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            var random = new System.Random();

            // 1. Ship Blocks Data
            var statuses = new[] { "완료", "진행중", "미시작", "오류" };
            int blockNumber = 1;
            for (int i = 0; i < 50; i++)
            {
                var status = statuses[random.Next(statuses.Length)];
                int progress = 0;
                if (status == "완료") progress = 100;
                else if (status == "진행중") progress = random.Next(20, 80);
                else if (status == "오류") progress = random.Next(30, 90);

                ShipBlocks.Add(new ShipBlockViewModel
                {
                    BlockId = $"A-{blockNumber++}",
                    Status = status,
                    Progress = progress
                });
            }

            // 2. Other Equipment Data
            OtherEquipment = new OtherEquipmentViewModel { MainCompressorStatus = "가동", CentralDustCollectorStatus = "경고" };

            // 3. KPI Chart Data
            var productionData = new List<Models.Overview.ChartDataPoint>();
            for (int i = 0; i < 30; i++) { productionData.Add(new Models.Overview.ChartDataPoint { Date = DateTime.Now.AddDays(-29 + i).ToString("MM-dd"), Value = random.Next(80, 120) }); }
            UpdateMonthlyProductionPlot(productionData);
            UpdateOperatingRateGauge(89.5);
        }

        private void UpdateMonthlyProductionPlot(List<Models.Overview.ChartDataPoint> productionData)
        {
            var plotModel = new PlotModel { PlotAreaBorderColor = OxyColors.Transparent, TextColor = OxyColor.FromRgb(100, 100, 100) };
            var lineSeries = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerStroke = OxyColors.DodgerBlue, MarkerFill = OxyColors.White, MarkerStrokeThickness = 1.5, Color = OxyColors.DodgerBlue, StrokeThickness = 2 };
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, IsAxisVisible = true, MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None };
            var valueAxis = new LinearAxis { Position = AxisPosition.Left, IsAxisVisible = true, MinimumPadding = 0.1, MaximumPadding = 0.1, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.None, MajorGridlineColor = OxyColor.FromAColor(40,OxyColors.Black) };

            if (productionData != null)
            {
                for (int i = 0; i < productionData.Count; i++) { lineSeries.Points.Add(new DataPoint(i, productionData[i].Value)); categoryAxis.Labels.Add(productionData[i].Date); }
            }
            categoryAxis.LabelFormatter = (index) => { int idx = (int)index; if (idx >= 0 && idx < categoryAxis.Labels.Count && idx % 7 == 0) { return categoryAxis.Labels[idx]; } return null; };

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
    }
}