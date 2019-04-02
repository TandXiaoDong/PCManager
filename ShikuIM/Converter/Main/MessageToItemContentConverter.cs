using ShikuIM.Model;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ShikuIM.Converter
{
    /// <summary>
    /// Message to ItemContent 
    /// </summary>
    public class MessageToItemContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Messageobject msg)
            {
                if (msg == null)
                {
                    return "";
                }

                string itemcontent = "";
                switch (msg.type)
                {
                    case kWCMessageType.Text:
                        itemcontent = msg.content;
                        break;
                    case kWCMessageType.Voice:
                        itemcontent = "[语音]";
                        break;
                    case kWCMessageType.Location:
                        itemcontent = "[位置]";
                        break;
                    case kWCMessageType.Video:
                        itemcontent = "[视频]";
                        break;
                    case kWCMessageType.Audio:
                        itemcontent = "[音频]";
                        break;
                    case kWCMessageType.Card:
                        itemcontent = "[名片]";
                        break;
                    case kWCMessageType.Image:
                        itemcontent = "[图片]";
                        break;
                    case kWCMessageType.Gif:
                        itemcontent = "[动画]";
                        break;
                    case kWCMessageType.File:
                        itemcontent = "[文件]";
                        break;
                    case kWCMessageType.Withdraw:
                        itemcontent = "撤回消息";
                        break;
                    case kWCMessageType.RoomNameChange:
                        itemcontent = "修改群名称 " + msg.content;
                        break;
                    case kWCMessageType.RoomDismiss:
                        itemcontent = "群组已被群主解散";
                        break;
                    case kWCMessageType.RoomExit:
                        if (msg.toUserId == Applicate.MyAccount.userId)//如果是自己退群
                        {
                            if (msg.toUserId == msg.fromUserId)//主动退群
                            {
                                itemcontent = "你已退出此群";
                            }
                            else//被踢
                            {
                                itemcontent = string.Format("你被 {0} 移出群组", msg.fromUserName);
                            }
                        }
                        else //群成员退出
                        {
                            if (msg.toUserId == msg.fromUserId)//主动退群
                            {
                                itemcontent = msg.fromUserName + "已退出群组";
                            }
                            else//被踢
                            {
                                itemcontent = msg.fromUserName + " 将 " + msg.toUserName + " 移出群组";
                            }
                        }
                        break;
                    case kWCMessageType.RoomNotice:
                        itemcontent = "群公告更改为: " + msg.content;
                        break;
                    case kWCMessageType.RoomInvite:
                        itemcontent = msg.toUserId == msg.fromUserId ? msg.toUserName + " 加入群组" : msg.fromUserName + " 邀请 " + msg.toUserName + " 加入群组";
                        break;
                    case kWCMessageType.RoomReadVisiblity:
                        itemcontent = msg.content == "0" ? "群组已关闭群消息已读人数" : "群组已开启群消息已读人数";
                        break;
                    case kWCMessageType.RoomIsVerify:
                        Regex reg = new Regex("^[A-F0-9]{8}([A-F0-9]{4}){3}[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                        if (reg.IsMatch(msg.objectId))
                        {
                            itemcontent = msg.content == "0" ? "群组已关闭进群验证" : "群组已开启进群验证";
                        }
                        else
                        {
                            itemcontent = "[验证消息]";
                        }
                        break;
                    case kWCMessageType.RoomIsPublic:
                        itemcontent = msg.content == "0" ? "群组已修改为公开群组" : "群组已修改为私密群组";
                        break;
                    case kWCMessageType.RoomInsideVisiblity:
                        itemcontent = msg.content == "0" ? "群组关闭了查看群成员功能" : "群组开启了查看群成员功能";
                        break;
                    case kWCMessageType.RoomUserRecommend:
                        itemcontent = msg.content == "0" ? "群组关闭了发送名片功能" : "群组开启了发送名片功能";
                        break;
                    case kWCMessageType.RoomFileUpload:
                        itemcontent = string.Format("{0} 共享一个群文件", msg.fromUserName);
                        break;
                    case kWCMessageType.RoomFileDelete:
                        itemcontent = string.Format("{0} 删除一个群文件", msg.fromUserName);
                        break;
                    case kWCMessageType.RoomAllBanned:
                        itemcontent = msg.content == "0" ? "群组已关闭全员禁言" : "群组已开启全员禁言";
                        break;
                    case kWCMessageType.RoomAllowMemberInvite:
                        itemcontent = msg.content == "0" ? "群组关闭全员禁言" : "群组开启全员禁言";
                        break;
                    case kWCMessageType.RoomAllowUploadFile:
                        itemcontent = msg.content == "0" ? "群组禁止上传文件" : "群组允许上传文件";
                        break;
                    case kWCMessageType.RoomAllowConference:
                        itemcontent = msg.content == "0" ? "群组禁止群成员发起会议" : "群组允许群成员发起会议";
                        break;
                    case kWCMessageType.RoomAllowSpeakCourse:
                        itemcontent = msg.content == "0" ? "群组禁止普通成员发送课程" : "群组允许普通成员发送课程";
                        break;
                    case kWCMessageType.RoomMemberBan:
                        if (msg.content == "0")
                        {
                            itemcontent = string.Format("{0} 被 {1} 取消禁言", msg.toUserName, msg.fromUserName);
                        }
                        else
                        {
                            itemcontent = string.Format("{0} 对 {1} 设置禁言", msg.fromUserName, msg.toUserName);
                        }
                        break;
                    case kWCMessageType.RoomAdmin:
                        if (msg.content == "1")
                        {
                            itemcontent = string.Format("{0} 设置 {1} 为管理员", msg.fromUserName, msg.toUserName);
                        }
                        else if (msg.content == "0")
                        {
                            itemcontent = string.Format("{0} 取消 {1} 为管理员", msg.fromUserName, msg.toUserName);
                        }
                        break;
                    case kWCMessageType.RoomManagerTransfer:
                        itemcontent = string.Format("{0} 已成为新群主", msg.toUserName);
                        break;
                    default:
                        itemcontent = "";// msg.content;
                        break;
                }
                return itemcontent;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

     /// <summary>
    /// Message to Fixed Message
    /// </summary>
    public class MessageToMsgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Messageobject)
            {
                var msg = ((Messageobject)value).Clone();
                switch (msg.type)
                {
                    case kWCMessageType.Text:
                        break;
                    case kWCMessageType.Voice:
                        msg.content = "[语音]";
                        break;
                    case kWCMessageType.Location:
                        msg.content = "[位置]";
                        break;
                    case kWCMessageType.Video:
                        msg.content = "[视频]";
                        break;
                    case kWCMessageType.Audio:
                        msg.content = "[音频]";
                        break;
                    case kWCMessageType.Card:
                        msg.content = "[名片]";
                        break;
                    case kWCMessageType.Image:
                        msg.content = "[图片]";
                        break;
                    case kWCMessageType.Gif:
                        msg.content = "[动画]";
                        break;
                    case kWCMessageType.File:
                        msg.content = "[文件]";
                        break;
                    case kWCMessageType.Withdraw:
                        msg.content = "撤回消息";
                        break;
                    case kWCMessageType.AudioChatCancel:
                    case kWCMessageType.VideoChatCancel:
                        break;
                    case kWCMessageType.RoomNameChange:
                        msg.content = "修改群名称 " + msg.content;
                        break;
                    case kWCMessageType.RoomDismiss:
                        msg.content = "群组已被群主解散";
                        break;
                    case kWCMessageType.RoomExit:
                        if (msg.toUserId == Applicate.MyAccount.userId)//如果是自己退群
                        {
                            if (msg.toUserId == msg.fromUserId)//主动退群
                            {
                                msg.content = "你已退出此群";
                            }
                            else//被踢
                            {
                                msg.content = string.Format("你被 {0} 移出群组", msg.fromUserName);
                            }
                        }
                        else //群成员退出
                        {
                            if (msg.toUserId == msg.fromUserId)//主动退群
                            {
                                msg.content = msg.fromUserName + "已退出群组";
                            }
                            else//被踢
                            {
                                msg.content = msg.fromUserName + " 将 " + msg.toUserName + " 移出群组";
                            }
                        }
                        break;
                    case kWCMessageType.RoomNotice:
                        msg.content = "群公告更改为: " + msg.content;
                        break;
                    case kWCMessageType.RoomInvite:
                        msg.content = msg.toUserId == msg.fromUserId ? msg.toUserName + " 加入群组" : msg.fromUserName + " 邀请 " + msg.toUserName + " 加入群组";
                        break;
                    case kWCMessageType.RoomReadVisiblity:
                        msg.content = msg.content == "0" ? "群组已关闭群消息已读人数" : "群组已开启群消息已读人数";
                        break;
                    case kWCMessageType.RoomIsVerify:
                        Regex reg = new Regex("^[A-F0-9]{8}([A-F0-9]{4}){3}[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                        if (reg.IsMatch(msg.objectId))
                        {
                            msg.content = msg.content == "0" ? "群组已关闭进群验证" : "群组已开启进群验证";
                        }
                        else
                        {
                            msg.content = "[验证消息]";
                        }
                        break;
                    case kWCMessageType.RoomIsPublic:
                        msg.content = msg.content == "0" ? "群组已修改为公开群组" : "群组已修改为私密群组";
                        break;
                    case kWCMessageType.RoomInsideVisiblity:
                        msg.content = msg.content == "0" ? "群组关闭了查看群成员功能" : "群组开启了查看群成员功能";
                        break;
                    case kWCMessageType.RoomUserRecommend:
                        msg.content = msg.content == "0" ? "群组关闭了发送名片功能" : "群组开启了发送名片功能";
                        break;
                    case kWCMessageType.RoomFileUpload:
                        msg.content = string.Format("{0} 共享一个群文件", msg.fromUserName);
                        break;
                    case kWCMessageType.RoomFileDelete:
                        msg.content = string.Format("{0} 删除一个群文件", msg.fromUserName);
                        break;
                    case kWCMessageType.RoomAllBanned:
                        msg.content = msg.content == "0" ? "群组已关闭全员禁言" : "群组已开启全员禁言";
                        break;
                    case kWCMessageType.RoomAllowMemberInvite:
                        msg.content = msg.content == "0" ? "群组关闭全员禁言" : "群组开启全员禁言";
                        break;
                    case kWCMessageType.RoomAllowUploadFile:
                        msg.content = msg.content == "0" ? "群组禁止上传文件" : "群组允许上传文件";
                        break;
                    case kWCMessageType.RoomAllowConference:
                        msg.content = msg.content == "0" ? "群组禁止群成员发起会议" : "群组允许群成员发起会议";
                        break;
                    case kWCMessageType.RoomAllowSpeakCourse:
                        msg.content = msg.content == "0" ? "群组禁止普通成员发送课程" : "群组允许普通成员发送课程";
                        break;
                    case kWCMessageType.RoomMemberBan:
                        if (msg.content == "0")
                        {
                            msg.content = string.Format("{0} 被 {1} 取消禁言", msg.toUserName, msg.fromUserName);
                        }
                        else
                        {
                            msg.content = string.Format("{0} 对 {1} 设置禁言", msg.fromUserName, msg.toUserName);
                        }
                        break;
                    case kWCMessageType.RoomAdmin:
                        if (msg.content == "1")
                        {
                            msg.content = string.Format("{0} 设置 {1} 为管理员", msg.fromUserName, msg.toUserName);
                        }
                        else if (msg.content == "0")
                        {
                            msg.content = string.Format("{0} 取消 {1} 为管理员", msg.fromUserName, msg.toUserName);
                        }
                        break;
                    case kWCMessageType.RoomManagerTransfer:
                        msg.content = string.Format("{0} 已成为新群主", msg.toUserName);
                        break;
                    default:
                        msg.content = "";// msg.content;
                        break;
                }
                return msg.content;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
