using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 根据控件和是否为自己发送属性来设置<see cref="Visibility"/>属性
    /// </summary>
    public class IsMySendToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string para = (string)parameter;
            int isMySend = (int)value;
            if (isMySend == 1)//是我发送的话
            {
                if (para == "0")//左边的控件
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else//不是我发送的话
            {
                if (para == "0")//如果是左边控件
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
