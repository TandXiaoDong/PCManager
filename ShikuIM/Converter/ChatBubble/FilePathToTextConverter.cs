using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{

    /// <summary>
    /// 将文件路径(如果存在)转换为文件名
    /// </summary>
    public class FilePathToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string temp = (string)value;
            if (temp.Contains("\\"))
            {
                return temp.Substring(temp.IndexOf('\\') + 1, temp.Length - temp.IndexOf('\\'));
            }
            else if (temp.Contains("/"))
            {
                int start = temp.LastIndexOf('/') + 1;
                int end = temp.Length - temp.LastIndexOf('/')-1;
                return temp.Substring(start, end);
            }
            else
            {
                return temp;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
