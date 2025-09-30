using CommunityToolkit.Mvvm.ComponentModel;

namespace ShipyardDashboard.ViewModels
{
    public partial class ProcessStepViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private string _icon = "";

        [ObservableProperty]
        private bool _isLast = false;
    }
}
