using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ShikuIM.Model;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 将UserId转为图片
    /// </summary>
    public class UserIdToPathConverter : IValueConverter
    {

        /// <summary>
        /// 将UserId转为路径
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object para, CultureInfo culture)
        {
            try
            {
                //string 类型 并且 不是空白 路径
                if (value is string avatarId)
                {
                    string path = Applicate.LocalConfigData.GetDisplayAvatorPath(avatarId);//获取显示的头像

                    LogHelper.log.Debug("======================获取本地路径显示的头像  \r\n"+path+" avatarID:"+avatarId);
                    if (!string.IsNullOrEmpty(path))
                    {
                        var thumbnail = new BitmapImage();
                        thumbnail.BeginInit();
                        thumbnail.DecodePixelWidth = 80;//图像压缩
                        thumbnail.CreateOptions = BitmapCreateOptions.DelayCreation;//需要时加载
                        thumbnail.CacheOption = BitmapCacheOption.OnLoad;//缓存模式
                        thumbnail.UriSource = new Uri(path, UriKind.Absolute);//现在可以改代码
                        thumbnail.EndInit();
                        thumbnail.Freeze();
                        return thumbnail;
                    }
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}

