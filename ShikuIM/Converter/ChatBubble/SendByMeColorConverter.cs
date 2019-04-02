using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 根据是否是自己发送标志设置背景颜色
    /// </summary>
    public class SendByMeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int isMySend)
            {
                if (isMySend == 0)//Send by me
                {
                    return Application.Current.FindResource("PrimaryHueLightBrush");
                }
                else//Send not by
                {
                    return Application.Current.FindResource("MyBubbleBackgroundBrush");
                }
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
