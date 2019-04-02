using ShikuIM.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 根据控件和是否为自己发送属性来设置<see cref="Visibility"/>属性
    /// </summary>
    public class MsgToGroupControlVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string para = (string)parameter;
            if (value == null)
            {
                return "";
            }
            var isMySend = (ChatBubbleItemModel)value;
            if (isMySend.isGroup == 1)//群聊
            {
                return Visibility.Visible;
            }
            else//单聊
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
