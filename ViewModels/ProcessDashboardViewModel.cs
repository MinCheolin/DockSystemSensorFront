
using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using ShipyardDashboard.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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

        private readonly ApiService _apiService;
        private readonly DispatcherTimer _timer;

        public ICommand ShowEquipmentDetailsCommand { get; private set; }

        public ProcessDashboardViewModel(string processName)
        {
            _processName = processName;
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
            catch (System.Exception ex)
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
                        // If a new group appears, add it (though unlikely in this simulation)
                        EquipmentGroups.Add(new EquipmentGroupViewModel(updatedGroup));
                    }
                }
            }
            catch (System.Exception ex)
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
        }

        public void Dispose()
        {
            _timer.Stop();
        }
    }

    // EquipmentLayoutProvider is no longer directly used by ProcessDashboardViewModel
    // but might be useful for other layout purposes or can be removed if not needed.
    public class EquipmentLayoutProvider
    {
        public record VisualMetricInfo(VisualizationType VizType, double Min = 0, double Max = 100);
        public record Position(int Row, int Col, int RowSpan = 1, int ColSpan = 1, Dictionary<string, VisualMetricInfo>? VisualMetrics = null);

        public Dictionary<string, Position> GetLayoutForProcess(string processName)
        {
            return processName switch
            {
                "가공" => GetCuttingLayout(),
                "조립" => GetAssemblyLayout(),
                "도장" => GetPaintingLayout(),
                "의장" => GetOutfittingLayout(),
                "탑재" => GetErectionLayout(),
                "시운전" => GetSeaTrialLayout(),
                _ => new Dictionary<string, Position>()
            };
        }

        private Dictionary<string, Position> GetCuttingLayout() => new()
        {
            { "CNC 플라즈마 절단기", new(0, 0, 1, 2, new() {{ "절단 속도", new(VisualizationType.Gauge, 0, 5.0) }, { "작업 ID", new(VisualizationType.Text) }, { "일일 생산량", new(VisualizationType.Text) }}) },
            { "CNC 레이저 절단기",   new(0, 2, 1, 2) },
            { "강판 벤딩 롤러",     new(1, 0, 1, 2, new() {{ "현재 각도", new(VisualizationType.Gauge, 0, 180) }, { "압력", new(VisualizationType.Gauge, 0, 5000) }}) },
            { "쇼트 블라스팅기",     new(1, 2, 1, 1) },
            { "유압 프레스",         new(1, 3, 1, 1) }
        };

        private Dictionary<string, Position> GetAssemblyLayout() => new()
        {
            { "CO2 용접기",      new(0, 0, 1, 2, new() {{ "전류", new(VisualizationType.Gauge, 0, 500) }, { "전압", new(VisualizationType.Text) }, { "가스 유량", new(VisualizationType.Text) }}) },
            { "용접 로봇",         new(0, 2, 1, 1) },
            { "지브 크레인",       new(0, 3, 1, 1) },
            { "갠트리 크레인",     new(1, 0, 1, 4, new() {{ "정격 대비", new(VisualizationType.ProgressBar, 0, 100) }, { "현재 하중", new(VisualizationType.Text) }}) }
        };

        private Dictionary<string, Position> GetPaintingLayout() => new()
        {
            { "온/습도 모니터링 시스템", new(0, 0, 1, 2, new() {{ "현재 온도", new(VisualizationType.Gauge, -10, 50) }, { "현재 습도", new(VisualizationType.Gauge, 0, 100) }}) },
            { "환기 시스템",           new(0, 2, 1, 2) },
            { "에어리스 스프레이",       new(1, 0, 1, 2) },
            { "도장 부스 제어 시스템",   new(1, 2, 1, 2) }
        };

        private Dictionary<string, Position> GetOutfittingLayout() => new()
        {
            { "파이프 자동 벤딩기", new(0, 0, 1, 2, new() {{ "가공 각도", new(VisualizationType.Gauge, 0, 360) }, { "일일 생산량", new(VisualizationType.Text) }}) },
            { "오버헤드 크레인",     new(0, 2, 1, 2) },
            { "케이블 포설기",       new(1, 0, 1, 2) },
            { "유압 펌프",           new(1, 2, 1, 2) }
        };

        private Dictionary<string, Position> GetErectionLayout() => new()
        {
            { "골리앗 크레인",   new(0, 0, 2, 2, new() {
                { "현재 하중", new(VisualizationType.Text) },
                { "정격 대비", new(VisualizationType.ProgressBar, 0, 100) },
                { "풍속", new(VisualizationType.Gauge, 0, 50) }
            }) },
            { "트랜스포터",      new(0, 2, 1, 2, new() {{ "주행 속도", new(VisualizationType.Gauge, 0, 10.0) }, { "연료/배터리", new(VisualizationType.ProgressBar, 0, 100) }}) },
            { "타워 크레인",     new(1, 2, 1, 2) }
        };

        private Dictionary<string, Position> GetSeaTrialLayout() => new()
        {
            { "주 엔진",         new(0, 0, 2, 2, new() {
                { "RPM", new(VisualizationType.Gauge, 0, 3000) },
                { "엔진 부하", new(VisualizationType.ProgressBar, 0, 100) },
                { "연료 소모율", new(VisualizationType.Sparkline) }
            }) },
            { "선박 발전기",     new(0, 2, 1, 2, new() {{ "전력 생산량", new(VisualizationType.Gauge, -1000, 1000) }}) },
            { "항해 통신 시스템",new(2, 0, 1, 2) },
            { "밸러스트 펌프",   new(2, 2, 1, 1) },
            { "GPS 수신기",      new(2, 3, 1, 1) }
        };
    }
}
