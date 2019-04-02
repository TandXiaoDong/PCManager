
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// Converter for display Empty page or chat page
    /// </summary>
    public class ChatTypeDisplayConverter : IValueConverter
    {
        #region 后台转前台
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var jid = (string)value;
                //如果好友验证消息才显示
                if (jid == "10001")
                {
                    return 1;//好友验证页面
                }
                else if (jid != null && jid != "")
                {
                    //return 2;//返回空白页面的索引
                    return 0;//返回聊天页面
                }
                else
                {
                    return 2;//返回空白页面的索引
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
