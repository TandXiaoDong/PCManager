using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class IsDownloadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility isDownload = Visibility.Visible;
            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                isDownload = Visibility.Hidden;
            return isDownload;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
