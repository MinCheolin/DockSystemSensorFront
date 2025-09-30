using System;
using System.Globalization;
using System.Windows.Data;

namespace ShipyardDashboard.Converters
{
    public class AlertStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null) return "";

            // Using Segoe MDL2 Assets font icons
            switch (status)
            {
                case "위험":
                    return "\uE7BA"; // CriticalError
                case "경고":
                    return "\uE783"; // Warning
                case "주의":
                    return "\uE76C"; // Info
                case "정상":
                    return "\uE73E"; // Completed
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
