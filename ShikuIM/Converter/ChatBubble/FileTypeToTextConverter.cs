using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{

    /// <summary>
    /// 将文件类型转换为文字
    /// </summary>
    public class FileTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string temp = (string)value;
            string text = temp.Substring(temp.LastIndexOf('.') + 1, temp.Length - temp.LastIndexOf('.') - 1);
            string extend = text.ToUpper();//转小写
            return extend + " " + "文件";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
