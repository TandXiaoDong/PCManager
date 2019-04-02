using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 将文件名转为绝对路径
    /// </summary>
    public class GifMsgToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string)value;
            if (name.StartsWith("gif_"))
            {
                name = name.Replace("gif_", "");
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "Resource\\Gif\\" + name;
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
