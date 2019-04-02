using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class ItemContentVisibleConverter : IValueConverter
    {
        #region 转换方法
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string tmp = System.Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(tmp))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
        #endregion

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
