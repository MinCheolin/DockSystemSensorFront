using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using ShipyardDashboard.ViewModels;

namespace ShipyardDashboard.Services
{
    public class MetricFinderConverter : IValueConverter
    {
        // Trivial comment to force re-compilation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<MetricViewModel> metrics && parameter is string key)
            {
                return metrics.FirstOrDefault(m => m.Name == key);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
