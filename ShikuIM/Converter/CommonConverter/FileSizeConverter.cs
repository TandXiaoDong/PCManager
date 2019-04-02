using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string m_strSize = "";
            if (value is long)
            {

                long FactSize = long.Parse(value.ToString());
                if (FactSize < 1024.00)
                    m_strSize = FactSize.ToString("F2") + " Byte";
                else if (FactSize >= 1024.00 && FactSize < 1048576)
                    m_strSize = (FactSize / 1024.00).ToString("F2") + " K";
                else if (FactSize >= 1048576 && FactSize < 1073741824)
                    m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " M";
                else if (FactSize >= 1073741824)
                    m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
                return m_strSize;
            }
            return m_strSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw null;
        }
    }
}
