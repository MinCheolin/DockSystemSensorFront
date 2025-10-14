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
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace ShipyardDashboard.ViewModels
{
    // Model for the critical alert popup
    public partial class CriticalAlertInfo : ObservableObject
    {
        [ObservableProperty] private string _alertType = string.Empty;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private string _message = string.Empty;
        [ObservableProperty] private DateTime _timestamp;
    }

    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly ApiService _apiService;
        private readonly DispatcherTimer _alertTimer;
        private readonly DispatcherTimer _kioskTimer;
        private readonly Dictionary<string, UserControl> _viewCache = new();
        private int _currentProcessIndex = 0;
        private int _kioskIntervalSeconds = 20; // Default interval
        private readonly HashSet<string> _shownCriticalAlerts = new HashSet<string>();

        public static event Action<string>? NavigateToProcessRequested;

        [ObservableProperty] private string _selectedProcess;
        public ObservableCollection<string> Processes { get; set; }
        [ObservableProperty] private UserControl _currentView;
        [ObservableProperty] private GlobalAlerts _globalAlerts;
        [ObservableProperty] private bool _isKioskModeActive = false;

        // New properties for improved kiosk mode
        [ObservableProperty] private int _kioskCountdown;
        [ObservableProperty] private bool _isCriticalAlertPopupOpen = false;
        [ObservableProperty] private CriticalAlertInfo? _currentCriticalAlert;

        public IRelayCommand ToggleKioskModeCommand { get; }
        public IRelayCommand CloseCriticalAlertCommand { get; }

        public MainViewModel()
        {
            _apiService = new ApiService();
            _globalAlerts = new GlobalAlerts();
            Processes = new ObservableCollection<string> { "종합 현황", "가공", "조립", "도장", "의장", "탑재", "시운전" };
            
            _selectedProcess = "종합 현황";
            _currentView = GetViewForProcess(_selectedProcess);

            _alertTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _alertTimer.Tick += async (s, e) => await UpdateGlobalAlertsAsync();
            
            // Kiosk timer now ticks every second for the countdown
            _kioskTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _kioskTimer.Tick += OnKioskTimerTick;

            ToggleKioskModeCommand = new RelayCommand(ToggleKioskMode);
            CloseCriticalAlertCommand = new RelayCommand(() => IsCriticalAlertPopupOpen = false);

            _alertTimer.Start();
            _ = UpdateGlobalAlertsAsync();

            NavigateToProcessRequested += OnNavigateToProcessRequested;
        }

        private bool _isKioskInitiatedChange = false;

        partial void OnSelectedProcessChanged(string value)
        {
            CurrentView = GetViewForProcess(value);
            
            // When user MANUALLY changes tab, stop kiosk mode.
            // The timer-initiated changes will set _isKioskInitiatedChange to true to bypass this.
            if (!_isKioskInitiatedChange && IsKioskModeActive)
            {
                ToggleKioskMode();
            }
        }

        private void ToggleKioskMode()
        {
            IsKioskModeActive = !IsKioskModeActive;
            if (IsKioskModeActive)
            {
                _currentProcessIndex = Processes.IndexOf(SelectedProcess);
                _kioskIntervalSeconds = GetKioskIntervalForProcess(SelectedProcess);
                KioskCountdown = _kioskIntervalSeconds;
                _kioskTimer.Start();
            }
            else
            {
                _kioskTimer.Stop();
                KioskCountdown = 0;
            }
        }

        private void OnKioskTimerTick(object? sender, EventArgs e)
        {
            // 1. Check for new critical alerts to show as a popup
            var criticalAlert = GlobalAlerts.Alerts.FirstOrDefault(a => a.Status == "위험");
            if (criticalAlert != null)
            {
                // Use a unique key for the alert to only show it once
                string alertKey = $"{criticalAlert.Location}-{criticalAlert.Message}";
                if (!_shownCriticalAlerts.Contains(alertKey))
                {
                    CurrentCriticalAlert = new CriticalAlertInfo
                    {
                        AlertType = criticalAlert.AlertType,
                        Location = criticalAlert.Location,
                        Message = criticalAlert.Message,
                        Timestamp = DateTime.Now
                    };
                    IsCriticalAlertPopupOpen = true;
                    _shownCriticalAlerts.Add(alertKey); // Mark as shown

                    // Automatically hide the toast after 7 seconds
                    Task.Delay(7000).ContinueWith(_ => IsCriticalAlertPopupOpen = false, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }

            // 2. Decrement countdown
            KioskCountdown--;

            // 3. If countdown reaches zero, switch to the next screen
            if (KioskCountdown <= 0)
            {
                _currentProcessIndex = (_currentProcessIndex + 1) % Processes.Count;
                
                _isKioskInitiatedChange = true;
                SelectedProcess = Processes[_currentProcessIndex];
                _isKioskInitiatedChange = false;

                // Reset interval and countdown for the new screen
                _kioskIntervalSeconds = GetKioskIntervalForProcess(SelectedProcess);
                KioskCountdown = _kioskIntervalSeconds;
            }
        }

        private int GetKioskIntervalForProcess(string processName)
        {
            return processName == "종합 현황" ? 40 : 20;
        }

        private UserControl GetViewForProcess(string processName)
        {
            if (_viewCache.TryGetValue(processName, out var view)) { return view; }

            UserControl newView = processName == "종합 현황"
                ? new OverviewView { DataContext = new OverviewViewModel(_apiService) }
                : new ProcessDashboardView { DataContext = new ProcessDashboardViewModel(processName, this) };

            _viewCache[processName] = newView;
            return newView;
        }

        private async Task UpdateGlobalAlertsAsync()
        { 
            var alerts = await _apiService.GetGlobalAlertsAsync();
            if (alerts != null)
            {
                if (alerts.Alerts != null)
                {
                    alerts.Alerts = alerts.Alerts.Where(a => a.Status != "정상").ToList();
                }
                GlobalAlerts = alerts;
            }
        }
        
        public static void RequestNavigation(string processName) => NavigateToProcessRequested?.Invoke(processName);
        private void OnNavigateToProcessRequested(string processName) => SelectedProcess = processName;

        public void Dispose()
        {
            _alertTimer.Stop();
            _kioskTimer.Stop();
            NavigateToProcessRequested -= OnNavigateToProcessRequested;
            foreach (var view in _viewCache.Values)
            {
                if (view.DataContext is IDisposable disposable) { disposable.Dispose(); }
            }
        }
    }
}
