using ShikuIM.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// 将视频或音频消息转为可视化友好的文本
    /// </summary>
    public class CallMsgToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChatBubbleItemModel msg = (ChatBubbleItemModel)value;
            string content = "";
            switch (msg.type)
            {

                #region 语音通话
                case kWCMessageType.AudioChatAsk:
                    break;
                case kWCMessageType.AudioChatReady:
                    break;
                case kWCMessageType.AudioChatAccept:
                    break;
                case kWCMessageType.AudioChatCancel:
                    content = "取消了 语音通话";
                    break;
                case kWCMessageType.AudioMeetingInvite:
                    content = "邀请您 语音会议";
                    break;
                case kWCMessageType.AudioMeetingJoin:
                    break;
                case kWCMessageType.AudioChatEnd:
                case kWCMessageType.AudioMeetingQuit:
                    content = string.Format("结束了 语音通话，时长：{0} 秒", msg.timeLen);
                    break;
                case kWCMessageType.PhoneCalling:
                    break;
                #endregion

                #region 视频通话
                case kWCMessageType.VideoChatAsk:
                    break;
                case kWCMessageType.VideoChatReady:
                    break;
                case kWCMessageType.VideoChatAccept:
                    break;
                case kWCMessageType.VideoChatCancel:
                    content = "取消了 视频通话";
                    break;
                case kWCMessageType.VideoMeetingInvite:
                    content = "邀请您 视频会议";
                    break;
                case kWCMessageType.VideoMeetingJoin:
                    break;
                case kWCMessageType.VideoChatEnd:
                case kWCMessageType.VideoMeetingQuit:
                    content = string.Format("结束了 视频通话，时长：{0} 秒", msg.timeLen);
                    break;
                case kWCMessageType.VideoMeetingKick:
                    break;
                #endregion

                default:
                    break;
            }

            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
