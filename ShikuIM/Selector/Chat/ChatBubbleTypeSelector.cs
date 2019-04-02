using Newtonsoft.Json;
using ShikuIM.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM.Selector
{

    /// <summary>
    /// 消息类型选择器
    /// </summary>
    public class ChatBubbleTypeSelector : DataTemplateSelector
    {
        #region 返回对应消息类型的气泡
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate bubbleTemplate = null;
            ChatBubbleItemModel msg = (ChatBubbleItemModel)item;
            switch (msg.type)
            {
                case kWCMessageType.LocalTypeNotice://提示消息
                    bubbleTemplate = (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                    break;
                case kWCMessageType.kWCMessageTypeNone:
                    break;
                case kWCMessageType.Text://文本
                    //return (DataTemplate) Application.Current.Resources. //("TextMessageTemplate");
                    bubbleTemplate = (DataTemplate)Application.Current.FindResource("TextMessageTemplate");
                    break;
                case kWCMessageType.Image://图片
                    bubbleTemplate = (DataTemplate)Application.Current.FindResource("ImageMessageTemplate");
                    break;
                case kWCMessageType.Voice://语音
                    return (DataTemplate)Application.Current.FindResource("AudioMessage");
                case kWCMessageType.Location://位置
                    return (DataTemplate)Application.Current.FindResource("LocationMessageTemplate");
                case kWCMessageType.Gif://动图
                    return (DataTemplate)Application.Current.FindResource("GifMessage");
                case kWCMessageType.Video://视频
                    return (DataTemplate)Application.Current.FindResource("VideoMessage");
                case kWCMessageType.Audio://音频
                    return (DataTemplate)Application.Current.FindResource(" ");
                case kWCMessageType.Card://联系人
                    return (DataTemplate)Application.Current.FindResource("ContactMessage");
                case kWCMessageType.File://文件
                    return (DataTemplate)Application.Current.FindResource("FileMessage");
                case kWCMessageType.Remind:
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.IsRead:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.RedPacket:
                    msg.content = "[红包]";
                    return (DataTemplate)Application.Current.FindResource("TextMessageTemplate");
                case kWCMessageType.SystemImage1:
                    break;
                case kWCMessageType.SystemImage2:
                    break;
                case kWCMessageType.Link:
                    break;
                case kWCMessageType.PokeMessage://戳一戳
                    return (DataTemplate)Application.Current.FindResource("PokeMessage");
                case kWCMessageType.FriendRequest:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.RequestAgree:
                    RequestAgree(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RequestRefuse:
                    break;
                case kWCMessageType.DeleteFriend:
                    DeleteFriend(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.BlackFriend:
                    BlackFriend(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RequestFriendDirectly:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.CancelBlackFriend:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.RoomFileUpload:
                    RoomFileUpload(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomFileDelete:
                    RoomFileDelete(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomFileDownload:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.Typing:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.Withdraw:
                    WithDrawMsg(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                //case kWCMessageType.AudioChatAsk:
                //case kWCMessageType.AudioChatAccept:
                case kWCMessageType.AudioChatCancel:
                case kWCMessageType.AudioChatEnd:
                case kWCMessageType.VideoChatCancel:
                case kWCMessageType.VideoChatEnd:
                    CallMsgChange(msg);
                    return (DataTemplate)Application.Current.FindResource("CallTipMessage");
                //case kWCMessageType.VideoChatAsk:
                //case kWCMessageType.VideoChatAccept:
                case kWCMessageType.VideoMeetingInvite:
                case kWCMessageType.VideoMeetingJoin:
                case kWCMessageType.VideoMeetingQuit:
                case kWCMessageType.VideoMeetingKick:
                case kWCMessageType.AudioMeetingJoin:
                case kWCMessageType.AudioMeetingQuit:
                case kWCMessageType.AudioChatReady:
                case kWCMessageType.VideoChatReady:
                case kWCMessageType.AudioMeetingInvite:
                case kWCMessageType.PhoneCalling:
                case kWCMessageType.AudioMeetingSetSpeaker:
                case kWCMessageType.AudioMeetingAllSpeaker://音视频电话消息
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.WeiboPraise:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.WeiboComment:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
                case kWCMessageType.RoomMemberNameChange:
                    RoomMemberNameChange(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomNameChange:
                    RoomNameChange(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomDismiss:
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomExit:
                    RoomExit(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomNotice:
                    RoomNotice(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomMemberBan:
                    RoomMemberBan(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomInvite:
                    RoomInvite(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomAdmin:
                    RoomAdmin(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomReadVisiblity:
                    RoomReadVisiblity(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomIsVerify:
                    RoomIsVerify(msg);
                    Regex reg = new Regex("^[A-F0-9]{8}([A-F0-9]{4}){3}[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                    if (reg.IsMatch(msg.objectId))
                    {
                        return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                    }
                    else
                    {
                        return (DataTemplate)Application.Current.FindResource("RoomVerifyMessage");
                    }
                case kWCMessageType.RoomIsPublic:
                    RoomIsPublic(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomInsideVisiblity:
                    RoomInsideVisiblity(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomUserRecommend:
                    RoomUserRecommend(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomAllBanned:
                    RoomAllBanned(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomAllowMemberInvite:
                    RoomAllowMemberInvite(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomAllowUploadFile:
                    RoomAllowUploadFile(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomAllowConference:
                    RoomAllowConference(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomAllowSpeakCourse:
                    RoomAllowSpeakCourse(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                case kWCMessageType.RoomManagerTransfer:
                    RoomManagerTransfer(msg);
                    return (DataTemplate)Application.Current.FindResource("ToolTipMessage");
                default:
                    return (DataTemplate)Application.Current.FindResource("EmptyMessageTemplate");
            }
            return bubbleTemplate;
        }
        #endregion

        private void CallMsgChange(ChatBubbleItemModel msg)
        {
            switch (msg.type)
            {
                case kWCMessageType.AudioChatEnd:
                case kWCMessageType.VideoChatEnd:
                    msg.content = msg.content + "，时长:" + msg.timeLen + "秒";
                    break;
                default:
                    break;
            }
        }

        #region 消息文本处理
        #region 普通成员讲课
        private void RoomAllowSpeakCourse(ChatBubbleItemModel msg)
        {
            if (msg.content == "0")
            {
                msg.content = string.Format("群组禁止普通成员讲课({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else if (msg.content == "1")
            {
                msg.content = string.Format("群组允许普通成员讲课({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }
        #endregion

        private void RoomManagerTransfer(ChatBubbleItemModel msg)
        {
            msg.content = string.Format("{0} 成为新群主({1})", msg.toUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }

        private void RoomAllowConference(ChatBubbleItemModel msg)
        {
            msg.content = msg.content == "0" ? "群组禁止群成员发起会议" : "群组允许群成员发起会议";
        }

        private void RoomAllowUploadFile(ChatBubbleItemModel msg)
        {
            msg.content = msg.content == "0" ? "群组禁止上传文件" : "群组允许上传文件";
        }

        private void RoomAllowMemberInvite(ChatBubbleItemModel msg)
        {
            msg.content = msg.content == "0" ? "群组禁止普通成员邀请好友" : "群组允许普通成员邀请好友";
        }

        private void RoomAllBanned(ChatBubbleItemModel msg)
        {
            if (msg.content == "0")
            {
                msg.content = "已关闭全体禁言";
            }
            else
            {
                msg.content = "已开启全体禁言";
            }
        }

        private void RoomUserRecommend(ChatBubbleItemModel msg)
        {
            if (msg.content == "0")
            {
                msg.content = string.Format("群主关闭了群内私聊功能({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else if (msg.content == "1")
            {
                msg.content = string.Format("群组开启了群内私聊功能({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void RoomInsideVisiblity(ChatBubbleItemModel msg)
        {
            if (msg.content == "0")
            {
                msg.content = string.Format("群组关闭了查看群成员功能({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else if (msg.content == "1")
            {
                msg.content = string.Format("群组开启了查看群成员功能({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void RoomIsPublic(ChatBubbleItemModel msg)
        {
            msg.content = msg.content == "0" ? "群组已修改为公开群组" : "群组已修改为私密群组";
        }

        private void RoomIsVerify(ChatBubbleItemModel msg)
        {
            msg.content = msg.content == "0" ? "群组已开启进群验证" : "群组已关闭进群验证";
            Regex reg = new Regex("^[A-F0-9]{8}([A-F0-9]{4}){3}[A-F0-9]{12}$", RegexOptions.IgnoreCase);
            if (reg.IsMatch(msg.objectId))
            {
                return;
            }
            var roomVerify = JsonConvert.DeserializeObject<RoomVerify>(msg.objectId);
            if (roomVerify.isInvite == "0")//邀请进群
            {
                msg.content = string.Format("{0} 邀请 {1} 位朋友加入群聊（{2}）", msg.fromUserName, roomVerify.userIds.Split(',').Length, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else
            {
                msg.content = string.Format("{0} 申请加入群聊（{1}）", msg.fromUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void RoomReadVisiblity(ChatBubbleItemModel msg)
        {
            if (msg.content == "0")
            {
                msg.content = string.Format("群组关闭了群消息已读人数功能({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else if (msg.content == "1")
            {
                msg.content = string.Format("群组开启了群消息已读人数功能({0})", Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void RoomAdmin(ChatBubbleItemModel msg)
        {
            if (msg.content == "1")
            {
                msg.content = string.Format("{0} 设置 {1} 为管理员({2})", msg.fromUserName, msg.toUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else if (msg.content == "0")
            {
                msg.content = string.Format("{0} 取消了 {1} 为管理员({2})", msg.fromUserName, msg.toUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void RoomInvite(ChatBubbleItemModel msg)
        {
            msg.content = msg.toUserId == msg.fromUserId ? msg.toUserName + " 加入群组" :
                msg.fromUserName + " 邀请 " + msg.toUserName + " 加入群组";
        }

        private void RoomMemberBan(ChatBubbleItemModel msg)
        {
            if (msg.content == "0")
            {
                msg.content = string.Format("{0} 被 {1} 取消禁言({2})", msg.toUserName, msg.fromUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
            else
            {
                msg.content = string.Format("{0} 对 {1} 设置了禁言，截止至 {2}", msg.fromUserName, msg.toUserName, Helpers.StampToDatetime(long.Parse(msg.content)).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void RoomNotice(ChatBubbleItemModel msg)
        {
            msg.content = string.Format("新公告 {0}({1})", msg.content, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }

        private void RoomNameChange(ChatBubbleItemModel msg)
        {
            msg.content = string.Format("{0} 修改群名称: {1}({2})", msg.fromUserName, msg.content, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }

        private void RoomMemberNameChange(ChatBubbleItemModel msg)
        {
            msg.content = string.Format("{0} 修改昵称为: {1}({2})", msg.fromUserName, msg.content, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }

        #region 拉黑
        private void BlackFriend(ChatBubbleItemModel msg)
        {
            msg.content = string.Format("您已被 {0} 拉入黑名单({1})",
                msg.fromUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }
        #endregion

        #region 删除好友
        private void DeleteFriend(ChatBubbleItemModel msg)
        {
            msg.content = string.Format("您已被 {0} 删除({1})", msg.fromUserName,
                Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }
        #endregion

        #region 同意好友请求
        private void RequestAgree(ChatBubbleItemModel msg)
        {
            msg.content =
                string.Format("你们已经是朋友，开始聊天吧({0})",
                Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }
        #endregion

        #region 文件上传
        private void RoomFileUpload(ChatBubbleItemModel msg)
        {
            msg.content =
                string.Format("{0} 上传了群文件 {1}({2})",
                msg.fromUserName,
                msg.fileName,
                Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }
        #endregion

        #region 文件上传
        private void RoomFileDelete(ChatBubbleItemModel msg)
        {
            msg.content =
                string.Format("{0} 删除了群文件 {1}({2})",
                msg.fromUserName,
                msg.fileName,
                Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }
        #endregion

        #region 消息撤回
        private void WithDrawMsg(ChatBubbleItemModel msg)
        {
            msg.content =
                string.Format("{0} 撤回一条消息({1})",
                msg.fromUserName,
                Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
        }
        #endregion

        #region 退群消息处理
        public void RoomExit(ChatBubbleItemModel msg)
        {
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
        }
        #endregion

        #endregion




    }
}
