using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 根据isMySend属性判断水平位置
    /// </summary>
    public class IsMySendToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int isMysend)
            {
                //int isLeft = (int)parameter;
                if (parameter is int isLeft)
                {
                    if (isMysend == 1)//如果是自己发送的
                    {
                        if (isLeft == 0) // 对方控件
                        {
                            return HorizontalAlignment.Left;
                        }
                        else
                        {
                            return HorizontalAlignment.Right;
                        }
                    }
                    else
                    {
                        return HorizontalAlignment.Right;
                    }
                }
                else//默认情况下,, 自己发送的消息在右,,对方发送的消息在左
                    if (isMysend == 1)//
                {
                    return HorizontalAlignment.Right;
                }
                else
                {
                    return HorizontalAlignment.Left;
                }
            }
            else
            {
                return HorizontalAlignment.Left;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
