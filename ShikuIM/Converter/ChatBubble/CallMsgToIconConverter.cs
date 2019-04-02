using MaterialDesignThemes.Wpf;
using ShikuIM.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 将视频或音频消息转为友好的图标
    /// </summary>
    public class CallMsgToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChatBubbleItemModel msg = (ChatBubbleItemModel)value;
            switch (msg.type)
            {
                case kWCMessageType.AudioChatAsk:
                case kWCMessageType.AudioChatAccept:
                case kWCMessageType.AudioChatCancel:
                case kWCMessageType.AudioChatEnd:
                case kWCMessageType.AudioMeetingInvite:
                    return PackIconKind.Phone;
                case kWCMessageType.VideoChatAsk:
                case kWCMessageType.VideoChatAccept:
                case kWCMessageType.VideoChatCancel:
                case kWCMessageType.VideoChatEnd:
                case kWCMessageType.VideoMeetingInvite:
                    return PackIconKind.Video;
                default:
                    return PackIconKind.Phone;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
