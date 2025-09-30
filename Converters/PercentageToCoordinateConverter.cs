using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ShipyardDashboard.Converters
{
    public class PercentageToCoordinateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values.Any(v => v == null || v == System.Windows.DependencyProperty.UnsetValue))
            {
                return 0.0;
            }

            try
            {
                double percentage = System.Convert.ToDouble(values[0]);
                double totalSize = System.Convert.ToDouble(values[1]);
                
                // Assume a fixed size for the element for now, e.g., 30 pixels
                double elementSize = 30; 

                // Calculate coordinate and center the element
                return (percentage / 100.0 * totalSize) - (elementSize / 2.0);
            }
            catch
            {
                return 0.0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
