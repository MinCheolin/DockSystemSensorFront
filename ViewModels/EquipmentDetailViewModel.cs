using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using ShipyardDashboard.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System;
using OxyPlot;
using OxyPlot.Series;

namespace ShipyardDashboard.ViewModels
{
    public partial class EquipmentDetailViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private EquipmentGroup _equipmentGroup;

        [ObservableProperty]
        private ObservableCollection<Equipment> _individualEquipments;

        public ICommand CloseCommand { get; private set; }

        private readonly DispatcherTimer _timer;
        private readonly Random _random = new();
        private int _tickCount = 0;

        public EquipmentDetailViewModel(EquipmentGroup group)
        {
            _equipmentGroup = group;
            _individualEquipments = new ObservableCollection<Equipment>(group.Equipments);
            CloseCommand = new RelayCommand(CloseWindow);

            // Initialize Health Status for each equipment
            foreach (var eq in _individualEquipments)
            {
                InitializeSensorData(eq.Health);
            }

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void InitializeSensorData(HealthStatus health)
        {
            // Add normal range bands to the plots
            health.VibrationPlotModel.Annotations.Add(new OxyPlot.Annotations.RectangleAnnotation 
            { 
                MinimumY = 0.5, MaximumY = 3.0, 
                Fill = OxyColor.FromAColor(50, OxyColors.Green), 
                Text = "정상 범위"
            });
            health.CurrentPlotModel.Annotations.Add(new OxyPlot.Annotations.RectangleAnnotation 
            { 
                MinimumY = 48, MaximumY = 57, 
                Fill = OxyColor.FromAColor(50, OxyColors.Green), 
                Text = "정상 범위"
            });

            for (int i = 0; i < 50; i++)
            {
                health.VibrationData.Add(new OxyPlot.DataPoint(i, _random.NextDouble() * 2 + 1)); // Base vibration
                health.CurrentData.Add(new OxyPlot.DataPoint(i, _random.NextDouble() * 5 + 50)); // Base current
            }
            UpdatePlot(health.VibrationPlotModel, health.VibrationData);
            UpdatePlot(health.CurrentPlotModel, health.CurrentData);
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            _tickCount++;
            foreach (var equipment in _individualEquipments)
            {
                var health = equipment.Health;

                // Simulate new data point
                double newVibration = _random.NextDouble() * 2 + 1;
                double newCurrent = _random.NextDouble() * 5 + 50;

                // Occasionally spike the data to show 'Warning' or 'Danger'
                if (_random.NextDouble() < 0.1) newVibration += _random.NextDouble() * 5;
                if (_random.NextDouble() < 0.05) newCurrent += _random.NextDouble() * 20;

                // Update data collections
                health.VibrationData.RemoveAt(0);
                health.VibrationData.Add(new OxyPlot.DataPoint(_tickCount + 50, newVibration));

                health.CurrentData.RemoveAt(0);
                health.CurrentData.Add(new OxyPlot.DataPoint(_tickCount + 50, newCurrent));

                // Update health status based on new data
                if (newVibration > 5 || newCurrent > 65)
                {
                    health.Status = "주의";
                }
                else if (newVibration > 7 || newCurrent > 80)
                {
                    health.Status = "위험";
                }
                else
                {
                    health.Status = "정상";
                }

                // Update the plots
                UpdatePlot(health.VibrationPlotModel, health.VibrationData);
                UpdatePlot(health.CurrentPlotModel, health.CurrentData);
            }
        }

        private void UpdatePlot(PlotModel plotModel, ObservableCollection<OxyPlot.DataPoint> data)
        {
            plotModel.Series.Clear();
            plotModel.Series.Add(new LineSeries { ItemsSource = data, Color = OxyColors.DodgerBlue });
            plotModel.InvalidatePlot(true);
        }

        private void CloseWindow(object? parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
        }
    }
}