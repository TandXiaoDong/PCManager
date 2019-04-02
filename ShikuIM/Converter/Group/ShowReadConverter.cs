using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class ShowReadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = true;
            if (value is int)
            {
                int isTrue = (int)value;
                if (isTrue == 0)
                {
                    b = false;
                }
                else if (isTrue == 1)
                {
                    b = true;
                }
            }
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return value;
            }
        }
    }
}
