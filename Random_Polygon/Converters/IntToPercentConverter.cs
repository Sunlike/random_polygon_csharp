using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace Random_Polygon.Converters
{
    public class IntToPercentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double ratio = double.Parse(value.ToString());
            return (int)ratio * 100.0;


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double ratio = double.Parse(value.ToString());
            return (ratio / 100.0).ToString("0.00");
        }

        #endregion
    }
}
