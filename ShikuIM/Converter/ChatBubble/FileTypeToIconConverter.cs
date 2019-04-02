using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 将文件类型转换为图标
    /// </summary>
    public class FileTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string temp = ((string)value);
            int start = temp.LastIndexOf('.') - 1;
            int end = temp.Length - temp.LastIndexOf('.') + 1;
            string text = temp.Substring(start, end);
            string extend = text.ToLower();//获取扩展名并转小写
            switch (extend)
            {
                case "apk":
                    return PackIconKind.Android;
                case "pdf":
                    return PackIconKind.FilePdf;

                case "mp3":
                case "flac":
                case "ape":
                case "wav":
                case "ogg":
                    return PackIconKind.FileMusic;
                case "rar":
                case "zip":
                    return PackIconKind.ZipBox;
                case "txt":
                    return PackIconKind.FileDocument;
                case "doc":
                case "docx":
                    return PackIconKind.FileWord;
                case "ppt":
                case "pptx":
                    return PackIconKind.FilePowerpoint;
                case "jpg":
                case "jpeg":
                case "bmp":
                case "ico":
                case "png":
                case "gif":
                    return PackIconKind.FileImage;
                case "mp4":
                case "mov":
                case "mkv":
                case "avi":
                    return PackIconKind.FileVideo;
                default:
                    return PackIconKind.File;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
