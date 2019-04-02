using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 空或者为0控制显示与隐藏
    /// </summary>
    public class IsNullToVisibilityConverter : IValueConverter
    {

        /// <summary>
        /// 空或者为0控制显示与隐藏（Value为空时隐藏控件）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if (parameter == null)
                {
                    return text == null ? Visibility.Collapsed : Visibility.Visible;//如果传参相反控制
                }
                else
                {
                    return text == null ? Visibility.Visible : Visibility.Collapsed;//未传参正常转换 (null隐藏，非null显示) 
                }
            }
            else if (value is int num)
            {
                if (parameter == null)
                {
                    return num == 0 ? Visibility.Collapsed : Visibility.Visible;//如果传参相反控制
                }
                else
                {
                    return num == 0 ? Visibility.Visible : Visibility.Collapsed;//未传参正常转换 (null隐藏，非null显示) 
                }
            }
            else
            {
                if (parameter == null)
                {
                    return value == null ? Visibility.Collapsed : Visibility.Visible;//如果传参相反控制
                }
                else
                {
                    return value == null ? Visibility.Visible : Visibility.Collapsed;//未传参正常转换 (null隐藏，非null显示) 
                }
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
