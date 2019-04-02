using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class GetFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.GetFileName(value.ToString());
            //Uri filePath = new Uri(value.ToString());
            //return filePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw null;
        }
    }
}
