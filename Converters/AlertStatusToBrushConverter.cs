using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShipyardDashboard.Converters
{
    public class AlertStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null) return Brushes.Gray;

            switch (status)
            {
                case "위험":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#E53935"); // Red
                case "경고":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA000"); // Amber
                case "점검 필요":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#039BE5"); // Light Blue
                case "주의":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#FDD835"); // Yellow
                case "정상":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#43A047"); // Green
                default:
                    return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
