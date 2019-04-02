using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CommonServiceLocator;
using ShikuIM.ViewModel;

namespace ShikuIM.Converter
{
    public class QRCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ServiceLocator.Current.GetInstance<MainViewModel>().IsInDesignMode)
            {
                return  new ImageBrush();
            }
            ImageBrush imgbrush = new ImageBrush();
            //Bean bean = value as Bean;
            //imgbrush.ImageSource = QRCodeHandler.CreateQRCode(bean.code,"Byte", 10, 10, "M", false, bean.logo);
            if(value !=null)
            imgbrush.ImageSource = QRCodeHandler.CreateQRCode(value.ToString(), "Byte", 10, 10, "M", false, "");
            return imgbrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
