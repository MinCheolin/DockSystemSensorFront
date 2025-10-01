using System.Windows;
using ShipyardDashboard.ViewModels;

namespace ShipyardDashboard
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); 
        }
    }
}