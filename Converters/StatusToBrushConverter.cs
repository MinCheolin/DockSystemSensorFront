using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShipyardDashboard.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (string.IsNullOrEmpty(status)) return Brushes.Gray;

            switch (status)
            {
                case "가동":
                case "정상":
                case "완료":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#2ECC71"); // Green
                case "주의":
                case "경고":
                case "대기":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#F1C40F"); // Yellow
                case "위험":
                case "오류":
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#E74C3C"); // Red
                default:
                    return Brushes.LightGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
