using CommonServiceLocator;
using ShikuIM.Model;
using ShikuIM.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class AreaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ServiceLocator.Current.GetInstance<MainViewModel>().IsInDesignMode)
            {
                return "";
            }
            string area = "";
            if (value is int)
            {
                int areaId = (int)value;
                area = new Areas() { id = areaId }.GetModel().name ?? "";
            }
            return area;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
