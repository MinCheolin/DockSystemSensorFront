using System;
using System.Globalization;
using System.Windows.Data;

namespace ShipyardDashboard.Services
{
    public class ValueGreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double numericValue && parameter is string thresholdString)
            {
                if (double.TryParse(thresholdString, out double threshold))
                {
                    return numericValue >= threshold;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
