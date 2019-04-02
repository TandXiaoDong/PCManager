using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 在指定字符串前插入字符
    /// </summary>
    public class InsertTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            if (value is string)
            {
                string temp = value.ToString();
                return parameter.ToString() + temp;//累加字符串于字符尾部
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
