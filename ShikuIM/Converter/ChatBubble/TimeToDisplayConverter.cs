using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 时间转为友好的文本
    /// </summary>
    public class TimeToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long timeStamp)
            {
                var time = timeStamp.StampToDatetime().ToDateTimeOffset();//获取时间
                return ConvertTime(time);
            }
            else if (value is DateTime sTime)
            {
                return ConvertTime(sTime.ToDateTimeOffset());
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

        #region Private Helper

        /// <summary>
        /// 转时间为友好文字的核心代码
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string ConvertTime(DateTimeOffset time)
        {
            //if it is today..
            if (time.Date == DateTimeOffset.UtcNow.Date)
            {
                return time.ToLocalTime().ToString("HH:mm:ss");
            }
            //Otherwise, return a full date that include year and month
            return time.ToLocalTime().ToString("MM dd, HH:mm");//
        }
        #endregion

    }
}
