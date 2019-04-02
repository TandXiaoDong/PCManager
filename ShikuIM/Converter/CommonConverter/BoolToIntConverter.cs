using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// Bool转换为Int
    /// </summary>
    public class BoolToIntConverter : IValueConverter
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
            if (value is bool isvisib)
            {
                if (parameter == null)
                {
                    int temp = isvisib ? 1 : 0;
                    return temp;//未传参正常转换
                }
                else
                {
                    return isvisib ? 0 : 1;//如果传参相反控制
                }
            }
            else
            {
                return 0;//不为
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
