using CommonServiceLocator;
using ShikuIM.ViewModel;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class TotalNoReadCountConverter : IValueConverter
    {

        #region 转换至前台
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //if (ServiceLocator.Current.GetInstance<MainViewModel>().IsInDesignMode)//if it's design mode, don't do this
                //{
                //    return "";
                //}
                if (value is int)
                {
                    int unread = System.Convert.ToInt32(value);//获取未读计数
                    return (unread == 0) ? ("") : (unread.ToString());
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                ConsoleLog.Output("转换未读角标时出错" + ex.Message);
                return "";
            }
        }
        #endregion

        #region 转换至后台
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
