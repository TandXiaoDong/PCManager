using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{

    /// <summary>
    /// 账号昵称转换器
    /// </summary>
    public class AccountNicknameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string remarkname)
            {
                if (!string.IsNullOrWhiteSpace(remarkname))
                {
                    return remarkname;
                }
                else
                {
                    if (parameter is string para)
                    {
                        string nickname = Applicate.GetNicknameByJid(para);
                        return nickname;
                    }
                    else
                    {
                        return "未命名";
                    }
                }
            }
            else
            {
                return "未命名";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
}
