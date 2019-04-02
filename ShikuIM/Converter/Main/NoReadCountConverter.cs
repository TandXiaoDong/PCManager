using CommonServiceLocator;
using ShikuIM.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    public class NoReadCountConverter : IValueConverter
    {
        #region 转换未读数量
        /// <summary>
        /// 此处传入参数为UserId值的Tag,用于判断标签是否是Session中的东西
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                //string UserId = System.Convert.ToString(parameter);
                //var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                //if (mControl.IsInDesignMode)//if it's design mode, don't do this
                //{
                //    return "";
                //}
                //如果当前聊天对象是该消息对象时,,返回空
                int count = (int)value;
                if (count > 0)
                {
                    return count;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        #endregion

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
