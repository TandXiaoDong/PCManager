using ShikuIM.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 根据<see cref="Messageobject"/>显示撤回按钮
    /// </summary>
    public class WithDrawVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Messageobject msg)
            {
                if (msg.isMySend == 1)//是我发送的话
                {
                    var parix = Helpers.DatetimeToStamp(DateTime.Now) - msg.timeSend;
                    if (parix <= 300)//如果5分钟内
                    {
                        return Visibility.Visible;//我发送的消息5分钟内允许撤回
                    }
                    else
                    {
                        return Visibility.Collapsed;//5分钟后不允许撤回
                    }
                }
                else
                {
                    return Visibility.Collapsed;//是对方发送的话就隐藏
                }
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
