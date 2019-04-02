using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class StampToDatetimeConerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long)
                return Helpers.StampToDatetime(long.Parse(value.ToString()));
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw null;
        }
    }
}
