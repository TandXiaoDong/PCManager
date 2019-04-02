using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// Bool转换为Int
    /// </summary>
    public class BoolReverseConverter : IValueConverter
    {
        /// <summary>
        /// Bool转换为Int
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">传参相反控制</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolvalue)
            {
                return boolvalue ? false : true;//未传参正常转换
            }
            else
            {
                return false;//
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
