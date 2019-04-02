using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = false;
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
            return value;
        }
    }
}
