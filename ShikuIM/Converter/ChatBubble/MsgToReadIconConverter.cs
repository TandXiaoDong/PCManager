using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{

    /// <summary>
    /// 将文件类型转换为文字
    /// </summary>
    public class MsgToReadIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int state)
            {
                if (parameter is int type)//判断显示类型
                {
                    if (type == 1)
                    {

                        switch (state)
                        {
                            case -1:
                                return PackIconKind.AlertCircle;//发送失败
                            case 0:
                                return PackIconKind.Timer;//正在发送
                            case 1:
                                return PackIconKind.Check;//送达
                            case 2:
                                return PackIconKind.CheckAll;//已读
                            default:
                                return PackIconKind.Timer;//已读
                        }
                    }
                    else if (type == 2)
                    {
                        switch (state)
                        {
                            case -1:
                                return "失败";//发送失败
                            case 0:
                                return "发送中";//正在发送
                            case 1:
                                return "";//送达
                            case 2:
                                return "已读";//已读
                            default:
                                return PackIconKind.Timer;//已读
                        }

                    }
                    else//返回
                    {
                        return parameter;
                    }
                    

                }
                return PackIconKind.Timer;//正在发送
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
