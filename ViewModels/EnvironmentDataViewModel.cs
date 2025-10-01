using CommunityToolkit.Mvvm.ComponentModel;

namespace ShipyardDashboard.ViewModels
{
    public partial class EnvironmentDataViewModel : ObservableObject
    {
        [ObservableProperty] private double _temperature;
        [ObservableProperty] private double _humidity;
        [ObservableProperty] private int _illuminance;
        [ObservableProperty] private double _fineDust;
    }
}
