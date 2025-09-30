using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using ShipyardDashboard.Models;
using ShipyardDashboard.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using ShipyardDashboard.Views;

namespace ShipyardDashboard.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly DispatcherTimer _timer;
        private readonly Dictionary<string, UserControl> _viewCache = new();

        public static event Action<string>? NavigateToProcessRequested;

        [ObservableProperty]
        private string _selectedProcess;

        public ObservableCollection<string> Processes { get; set; }

        [ObservableProperty]
        private UserControl _currentView;

        [ObservableProperty]
        private GlobalAlerts _globalAlerts;

        public MainViewModel()
        {
            _apiService = new ApiService();
            _globalAlerts = new GlobalAlerts(); // Initialize to prevent binding errors on startup
            Processes = new ObservableCollection<string> { "종합 현황", "가공", "조립", "도장", "의장", "탑재", "시운전" };
            
            _selectedProcess = "종합 현황";
            _currentView = GetViewForProcess(_selectedProcess);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _timer.Tick += async (s, e) => await UpdateGlobalAlertsAsync();
            _timer.Start();
            
            _ = UpdateGlobalAlertsAsync();

            NavigateToProcessRequested += OnNavigateToProcessRequested;
        }

        partial void OnSelectedProcessChanged(string value)
        {
            CurrentView = GetViewForProcess(value);
        }

        private UserControl GetViewForProcess(string processName)
        {
            if (_viewCache.TryGetValue(processName, out var view))
            {
                return view;
            }

            UserControl newView;
            if (processName == "종합 현황")
            {
                newView = new OverviewView { DataContext = new OverviewViewModel() };
            }
            else
            {
                newView = new ProcessDashboardView { DataContext = new ProcessDashboardViewModel(processName) };
            }

            _viewCache[processName] = newView;
            return newView;
        }

        public static void RequestNavigation(string processName)
        {
            NavigateToProcessRequested?.Invoke(processName);
        }

        private void OnNavigateToProcessRequested(string processName)
        {
            SelectedProcess = processName;
        }

        private async Task UpdateGlobalAlertsAsync()
        {
            GlobalAlerts = await _apiService.GetGlobalAlertsAsync() ?? new GlobalAlerts();
        }
    }
}