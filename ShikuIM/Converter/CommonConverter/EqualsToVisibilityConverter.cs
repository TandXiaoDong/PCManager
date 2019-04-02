using CommonServiceLocator;
using MaterialDesignThemes.Wpf.Converters;
using ShikuIM.ViewModel;
using System;
using System.Globalization;
using System.Windows;

namespace ShikuIM.Converter
{
    public class EqualsToVisibilityConverter : EqualityToVisibilityConverter
    {

        public new object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ServiceLocator.Current.GetInstance<MainViewModel>().IsInDesignMode)
            {
                return Visibility.Collapsed;
            }

            return base.Convert(value, targetType, parameter, culture);
        }

        public new object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
