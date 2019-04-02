using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{

    /// <summary>
    /// 性别转文本转换器
    /// </summary>
    public class GenderToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is int)
            {
                int gender = (int)value;
                return (gender == 1) ? ("男") : ("女");
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
