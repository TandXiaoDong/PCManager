using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 在指定字符串后追加字符
    /// </summary>
    public class AppendTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            if (value is string || value is int)
            {
                string temp = value.ToString();
                return temp + parameter.ToString();//累加字符串于字符尾部
            }
            else
                return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
