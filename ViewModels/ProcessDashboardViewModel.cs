using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using ShipyardDashboard.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System;
using System.Windows.Input;
using ShipyardDashboard.ViewModels.Base;
using System.Windows;
using ShipyardDashboard.Views;

namespace ShipyardDashboard.ViewModels
{
    public partial class ProcessDashboardViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string _processName;

        [ObservableProperty]
        private ObservableCollection<EquipmentGroupViewModel> _equipmentGroups;

        // Reference to the MainViewModel for communication
        public MainViewModel MainVm { get; }

        private readonly ApiService _apiService;
        private readonly DispatcherTimer _timer;

        public ICommand ShowEquipmentDetailsCommand { get; private set; }

        // Updated constructor
        public ProcessDashboardViewModel(string processName, MainViewModel mainViewModel)
        {
            _processName = processName;
            MainVm = mainViewModel; // Store reference
            _equipmentGroups = new ObservableCollection<EquipmentGroupViewModel>();
            _apiService = new ApiService();
            
            ShowEquipmentDetailsCommand = new RelayCommand<EquipmentGroupViewModel>(ShowEquipmentDetails);

            _ = InitializeDashboardAsync();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _timer.Tick += async (s, e) => await UpdateDataAsync();
            _timer.Start();
        }

        private async Task InitializeDashboardAsync()
        {
            try
            {
                var dashboardData = await _apiService.GetProcessDashboardAsync(ProcessName);
                if (dashboardData?.EquipmentGroups == null) return;

                foreach (var group in dashboardData.EquipmentGroups)
                {
                    EquipmentGroups.Add(new EquipmentGroupViewModel(group));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize dashboard for {ProcessName}: {ex.Message}");
            }
        }

        private async Task UpdateDataAsync()
        {
            try
            {
                var dashboardData = await _apiService.GetProcessDashboardAsync(ProcessName);
                if (dashboardData?.EquipmentGroups == null) return;

                foreach (var updatedGroup in dashboardData.EquipmentGroups)
                {
                    var existingGroupVm = EquipmentGroups.FirstOrDefault(g => g.GroupName == updatedGroup.GroupName);
                    if (existingGroupVm != null)
                    {
                        existingGroupVm.UpdateGroup(updatedGroup);
                    }
                    else
                    {
                        EquipmentGroups.Add(new EquipmentGroupViewModel(updatedGroup));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update dashboard for {ProcessName}: {ex.Message}");
            }
        }

        private void ShowEquipmentDetails(EquipmentGroupViewModel? groupVm)
        {
            if (groupVm == null) return;

            var detailViewModel = new EquipmentDetailViewModel(groupVm.EquipmentGroup);
            var detailWindow = new EquipmentDetailView { DataContext = detailViewModel };
            detailWindow.Owner = Application.Current.MainWindow;
            detailWindow.ShowDialog();
            
            // Dispose the ViewModel when the window is closed
            if(detailViewModel is IDisposable disposable) disposable.Dispose();
        }

        public void Dispose()
        {
            _timer.Stop();
        }
    }
}