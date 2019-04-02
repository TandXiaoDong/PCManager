using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 显示与隐藏转换器
    /// </summary>
    public class BoolToVisiblityConverter : IValueConverter
    {

        /// <summary>
        /// 传参相反控制
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int isvisib)
            {
                if (parameter == null)
                {
                    return isvisib == 1 ? Visibility.Visible : Visibility.Collapsed;//未传参正常转换
                }
                else
                {
                    return isvisib == 1 ? Visibility.Collapsed : Visibility.Visible;//如果传参相反控制
                }
            }
            else
            {
                if (parameter == null)
                {
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;//未传参正常转换
                }
                else
                {
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;//如果传参相反控制
                }
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
