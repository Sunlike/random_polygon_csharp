using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace Random_Polygon.Converters
{
    public class StringToIntPercentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
             int result = 0;
             if (value == null || string.IsNullOrEmpty(value.ToString()) || !int.TryParse(value.ToString(), out result))
             {
                 result = 0;
             }

             return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return value.ToString();

            }
            return "";
        }

        #endregion
    }
}
