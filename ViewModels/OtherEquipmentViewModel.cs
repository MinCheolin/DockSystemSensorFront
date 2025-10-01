using CommunityToolkit.Mvvm.ComponentModel;

namespace ShipyardDashboard.ViewModels
{
    public partial class OtherEquipmentViewModel : ObservableObject
    {
        [ObservableProperty] private string _mainCompressorStatus = string.Empty;
        [ObservableProperty] private string _centralDustCollectorStatus = string.Empty;
    }
}
