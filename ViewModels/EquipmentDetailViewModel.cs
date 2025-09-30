using CommunityToolkit.Mvvm.ComponentModel;
using ShipyardDashboard.Models;
using ShipyardDashboard.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace ShipyardDashboard.ViewModels
{
    public partial class EquipmentDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private EquipmentGroup _equipmentGroup;

        [ObservableProperty]
        private ObservableCollection<Equipment> _individualEquipments;

        public ICommand CloseCommand { get; private set; }

        public EquipmentDetailViewModel(EquipmentGroup group)
        {
            _equipmentGroup = group;
            _individualEquipments = new ObservableCollection<Equipment>(group.Equipments);
            CloseCommand = new RelayCommand(CloseWindow);
        }

        private void CloseWindow(object? parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
