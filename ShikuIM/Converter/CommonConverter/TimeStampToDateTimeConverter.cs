using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class TimeStampToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long || value is int)
            {
                long time = System.Convert.ToInt64(value);
                if (time == 0)
                {
                    return "";
                }
                DateTime date = Helpers.StampToDatetime(time);
                if (parameter == null)
                {
                    return date.ToString("D");//转换为日期格式
                }
                else if (parameter is string para)
                {
                    if (para == "s")
                    {
                        return date;//转换为日期格式
                    }
                    else
                    {
                        return date.ToString("D");//转换为日期格式
                    }
                }
                else
                {
                    return date;//转换为日期格式
                }
            }
            else
            {
                if (parameter != null)
                {
                    return "未填写";
                }
                else
                {
                    return "";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long timeStamp = 0;
            if (value is DateTime time)
            {
                timeStamp = Helpers.DatetimeToStamp(time);

            }

            return timeStamp;
        }

    }
}
