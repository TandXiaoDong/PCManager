using CommonServiceLocator;
using ShikuIM.Model;
using ShikuIM.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class RoomCreateDateConverter : IValueConverter
    {
        #region 转换
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ServiceLocator.Current.GetInstance<MainViewModel>().IsInDesignMode)//if it's design mode, don't do this
            {
                return "";
            }
            if (value != null && value is Room)
            {
                string text = "";
                var room = (Room)value;
                text = room.nickname + "创建此群于" + Helpers.StampToDatetime(room.createTime).ToShortDateString();
                return text;//返回转换好的字符
            }
            else
            {
                return "";
            }
        }
        #endregion

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
