using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Random_Polygon.Converters
{
    public class DoubleToPercentConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double ratio  = double.Parse(value.ToString());
            string format = parameter == null ? "0" : parameter.ToString();
            string result = (ratio * 100).ToString(format);
            return result + "%";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = value.ToString();
            result = result.Remove(result.Length - 1);
            return double.Parse(result)/100;
        }

        #endregion
    }

   

}
