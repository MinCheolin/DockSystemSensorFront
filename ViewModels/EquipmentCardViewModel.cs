using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ShipyardDashboard.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows;

namespace ShipyardDashboard.ViewModels
{
    public partial class EquipmentCardViewModel : ObservableObject
    {
        private Equipment _equipment;
        public Equipment Equipment
        {
            get => _equipment;
            set
            {
                if (SetProperty(ref _equipment, value))
                {
                    CreateMetrics();
                }
            }
        }

        [ObservableProperty]
        private int _gridRow;

        [ObservableProperty]
        private int _gridColumn;

        [ObservableProperty]
        private int _rowSpan;

        [ObservableProperty]
        private int _columnSpan;

        public int CardWidth { get; }
        public int CardHeight { get; }

        public ObservableCollection<MetricViewModel> Metrics { get; } = new();
        public ObservableCollection<MetricViewModel> Co2WelderSubMetrics { get; } = new();

        public EquipmentLayoutProvider.Position LayoutPosition { get; }

        public EquipmentCardViewModel(Equipment equipment, EquipmentLayoutProvider.Position position)
        {
            _equipment = equipment;
            LayoutPosition = position;
            _gridRow = position.Row;
            _gridColumn = position.Col;
            _rowSpan = position.RowSpan;
            _columnSpan = position.ColSpan;

            // Set card dimensions based on span to maintain aspect ratio in Viewbox
            if (RowSpan == 2 && ColumnSpan == 2) // Large
            {
                CardWidth = 400;
                CardHeight = 400;
            }
            else if (ColumnSpan == 4) // Super Wide
            {
                CardWidth = 800;
                CardHeight = 200;
            }
            else if (ColumnSpan == 2) // Wide
            {
                CardWidth = 400;
                CardHeight = 200;
            }
            else // Small
            {
                CardWidth = 200;
                CardHeight = 200;
            }

            CreateMetrics();

            // Populate sub-metrics for specific panels
            if (equipment.Name == "CO2 용접기" && Metrics.Count >= 3)
            {
                Co2WelderSubMetrics.Add(Metrics[1]);
                Co2WelderSubMetrics.Add(Metrics[2]);
            }
        }

        private void CreateMetrics()
        {
            Metrics.Clear();
            if (Equipment?.Details == null) return;

            var handledKeys = new HashSet<string>();

            // 1. Process visual metrics first based on the layout definition
            if (LayoutPosition.VisualMetrics != null)
            {
                foreach (var visualMetric in LayoutPosition.VisualMetrics)
                {
                    string metricName = visualMetric.Key;
                    var metricInfo = visualMetric.Value;

                    if (Equipment.Details.TryGetValue(metricName, out var detail))
                    {
                        PlotModel? plotModel = null;
                        double? numericValue = detail.Value;

                        if (metricInfo.VizType == VisualizationType.Gauge && numericValue.HasValue)
                        {
                            plotModel = CreateRadialGaugePlot(numericValue.Value, metricInfo.Min, metricInfo.Max, detail.Display, detail.Unit);
                        }
                        else if (metricInfo.VizType == VisualizationType.Sparkline && detail.SeriesData != null && detail.SeriesData.Any())
                        {
                            plotModel = CreateSparklinePlot(detail.SeriesData, detail.Display);
                        }
                        
                        Metrics.Add(new MetricViewModel(metricName, detail.Display, detail.Unit, metricInfo.VizType, numericValue, plotModel));
                        handledKeys.Add(metricName);
                    }
                }
            }

            // 2. Add remaining details as text metrics
            foreach (var detail in Equipment.Details)
            {
                if (!handledKeys.Contains(detail.Key))
                {
                    Metrics.Add(new MetricViewModel(detail.Key, detail.Value.Display, detail.Value.Unit, VisualizationType.Text, detail.Value.Value));
                }
            }
        }

        private PlotModel CreateRadialGaugePlot(double value, double min, double max, string displayValue, string unit)
        {
            var model = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0), Background = OxyColors.Transparent };

            var series = new PieSeries
            {
                StartAngle = 270,
                AngleSpan = 360,
                InnerDiameter = 0.7,
                StrokeThickness = 0,
                AreInsideLabelsAngled = false,
                InsideLabelFormat = null,
                OutsideLabelFormat = null,
            };

            double range = max - min;
            // Handle edge case where max and min are equal to avoid division by zero
            double normalizedValue = range > 0 ? Math.Clamp(((value - min) / range), 0, 1) : (value >= max ? 1 : 0);

            OxyColor valueColor;
            // Use percentage of range for color coding
            double percentage = normalizedValue * 100;
            if (percentage > 95) valueColor = OxyColor.FromRgb(231, 76, 60); // Red
            else if (percentage > 85) valueColor = OxyColor.FromRgb(241, 196, 15); // Yellow
            else valueColor = OxyColor.FromRgb(46, 204, 113); // Green

            series.Slices.Add(new PieSlice("", percentage) { Fill = valueColor });
            series.Slices.Add(new PieSlice("", 100 - percentage) { Fill = OxyColor.FromRgb(236, 240, 241) });

            model.Series.Add(series);

            // Main value text - Larger and Bolder
            model.Annotations.Add(new TextAnnotation
            {
                Text = displayValue,
                Font = "Segoe UI",
                FontSize = 42, // Adjusted font size
                FontWeight = OxyPlot.FontWeights.Bold,
                TextColor = OxyColor.FromRgb(44, 62, 80),
                TextPosition = new OxyPlot.DataPoint(0, 0), // Centered
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle
            });

            return model;
        }

        private PlotModel CreateSparklinePlot(List<double> values, string displayValue)
        {
            var pm = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0), Background = OxyColors.Transparent, Padding = new OxyThickness(5) };
            var series = new LineSeries
            {
                StrokeThickness = 4, // Thicker line
                Color = OxyColor.FromRgb(52, 152, 219) // Accent
            };

            for (int i = 0; i < values.Count; i++)
            {
                series.Points.Add(new OxyPlot.DataPoint(i, values[i]));
            }

            pm.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, IsAxisVisible = false });
            pm.Axes.Add(new LinearAxis { Position = AxisPosition.Left, IsAxisVisible = false });
            
            pm.Title = null;

            // Add value label at the last point of the sparkline
            if (values.Any())
            {
                pm.Annotations.Add(new PointAnnotation
                {
                    X = values.Count - 1,
                    Y = values.Last(),
                    Text = displayValue,
                    Font = "Segoe UI",
                    FontSize = 14,
                    FontWeight = OxyPlot.FontWeights.Bold,
                    TextColor = OxyColor.FromRgb(44, 62, 80),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle,
                    TextMargin = 10
                });
            }

            pm.Series.Add(series);
            return pm;
        }
    }
}
