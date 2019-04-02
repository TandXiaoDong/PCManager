using CommonServiceLocator;
using ShikuIM.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShikuIM.Converter
{
    public class GroupAccessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ServiceLocator.Current.GetInstance<MainViewModel>().IsInDesignMode)
            {
                return "";
            }
            if (parameter != null)//如果为false传
            {
                int access = System.Convert.ToInt16(value);//
                ImageSource accessImg = null;
                switch (access)
                {
                    case 1:
                        accessImg = new BitmapImage(new Uri(@"pack://application:,,,/icon/Owner.png", UriKind.Absolute));
                        break;
                    case 2:
                        accessImg = new BitmapImage(new Uri(@"pack://application:,,,/icon/Manager.png", UriKind.Absolute));
                        break;
                    case 3:
                        accessImg = new BitmapImage();
                        break;
                    default:
                        break;
                }
                return accessImg;
            }
            else//返回提示文字
            {
                int access = System.Convert.ToInt16(value);//
                string accessText = null;
                switch (access)
                {
                    case 1:
                        accessText = "群主";
                        break;
                    case 2:
                        accessText = "管理员";
                        break;
                    case 3:
                        accessText = "群员";
                        break;
                    default:
                        break;
                }
                return accessText;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
