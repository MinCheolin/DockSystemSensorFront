using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ShipyardDashboard.Services
{
    public class SubListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable list || parameter is not string paramString) return null;

            var listAsGeneric = list.Cast<object>().ToList();

            var parts = paramString.Split(':');
            if (parts.Length != 2) return null;

            if (int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int count))
            {
                return listAsGeneric.Skip(start).Take(count).ToList();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            throw new NotImplementedException();
        }
    }
}
