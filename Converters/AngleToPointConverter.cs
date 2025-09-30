using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShipyardDashboard.Converters
{
    public class AngleToPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double value = (double)values[0];
                double maximum = (double)values[1];
                double width = (double)values[2];
                double height = (double)values[3];

                double percentage = (maximum > 0) ? (value / maximum) : 0;
                double angle = 180 * percentage;

                double radiusX = width / 2 - 10; // 10 is padding/stroke thickness related
                double radiusY = height - 10;

                double angleRad = (Math.PI / 180.0) * (angle + 180);

                double x = (width / 2) + Math.Sin(angleRad) * radiusX;
                double y = height + Math.Cos(angleRad) * radiusY;

                return new Point(x, y);
            }
            catch
            {
                return new Point(0, 0);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
