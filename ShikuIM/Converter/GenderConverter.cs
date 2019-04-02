using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{

    /// <summary>
    /// 性别转换器
    /// </summary>
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is int)
            {
                //(user.data.sex == 0) ? ("女") : (user.data.sex == 1) ? ("男") : ("");//性别
                int gender = System.Convert.ToInt32(value);
                if (gender < 0)//如果传值小于0不显示
                {
                    return PackIconKind.HumanMaleFemale;
                }
                var para = (string)parameter;
                if (para == "" || para == null)
                {
                    return (gender == 1) ? (PackIconKind.GenderMale) : (PackIconKind.GenderFemale);
                }
                else
                {
                    return (gender == 1) ? ("男") : ("女");
                }
            }
            else
            {
                return PackIconKind.GenderMale;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
