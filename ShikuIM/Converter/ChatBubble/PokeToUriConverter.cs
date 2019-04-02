using ShikuIM.Resource;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 根据isMySend属性判断水平位置
    /// </summary>
    public class PokeToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int isMysend)
            {
                if (isMysend == 1)//是自己发送的话显示
                {
                    return Helpers.ConvertBitmapToBitmapSource(ShikuRec.PokeRight);
                }
                else
                {
                    return Helpers.ConvertBitmapToBitmapSource(ShikuRec.PokeLeft);
                }
            }
            else
            {
                return new ImageBrush(Helpers.ConvertBitmapToBitmapSource(ShikuRec.PokeLeft));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
