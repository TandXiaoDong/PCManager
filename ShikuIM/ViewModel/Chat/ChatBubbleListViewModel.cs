using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using ShikuIM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{

    /// <summary>
    /// 消息气泡ViewModel
    /// </summary>
    public class ChatBubbleListViewModel : ViewModelBase
    {
        #region Private Member
        private HttpClient HttpService = null;
        private bool isInLoading;
        #endregion

        #region Public Member

        /// <summary>
        /// 是否在加载
        /// </summary>
        public bool IsLoading
        {
            get { return isInLoading; }
            set { isInLoading = value; RaisePropertyChanged(nameof(IsLoading)); }
        }

        /// <summary>
        /// 消息气泡项
        /// </summary>
        public ObservableCollection<ChatBubbleItemModel> ChatMessageList { get; set; }
        #endregion

        #region Comands

        /// <summary>
        /// 滚动时命令
        /// </summary>
        public ICommand ScrollChangedCommand { get; set; }

        #endregion

        #region Contructor
        public ChatBubbleListViewModel()
        {
            #region Initials Member
            ChatMessageList = new ObservableCollection<ChatBubbleItemModel>();
            #endregion
            #region Initial Commands
            ScrollChangedCommand = new RelayCommand<ScrollChangedEventArgs>(ChatListScrollChanged);
            #endregion
            InitialMessengers();
        }
        #endregion

        #region 注册通知
        /// <summary>
        /// 注册通知
        /// </summary>
        private void InitialMessengers()
        {
            //批量显示消息
            Messenger.Default.Register<string>(this, ChatBubblesNotifications.ShowBubbleList, (jid) => { ShowDefaultMessage(jid); });
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgRecived, msg => { ProcessNewMessage(msg); });
            Messenger.Default.Register<Messageobject>(this, ChatBubblesNotifications.WithDrawSingleMessage, msg => { WithDrawMessage(msg); });
            Messenger.Default.Register<Messageobject>(this, ChatBubblesNotifications.ShowSingleBubble, (msg) => AddSingleChatMessage(msg));
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgReceipt, (msg) => MsgReceipt(msg));
            Messenger.Default.Register<string>(this, CommonNotifications.XmppMsgReceipt, (msgId) => UpdateMessageSended(msgId));
            Messenger.Default.Register<MessageListItem>(this, CommonNotifications.UpdateGroupMemberNickname, (item) => UpdateGroupMemberRemark(item));
            Messenger.Default.Register<Messageobject>(this, ChatBubblesNotifications.DeleteMessage, (msg) => { RemoveMessage(msg); });
        }
        #endregion

        /// <summary>
        /// 根据MessageId设置消息的
        /// </summary>
        /// <param name="msgId"></param>
        private void UpdateMessageSended(string msgId)
        {
            var existsItem = ChatMessageList.FirstOrDefault(m => m.messageId == msgId);
            if (existsItem != null)
            {
                existsItem.messageState = 1;//设为送达
                existsItem.StateIcon = PackIconKind.Check;//设为送达
                existsItem.MsgStateText = "送达";
                existsItem.ReadToolTip = "送达";
            }
        }

        #region 删除消息
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msg">需要删除的消息</param>
        private void RemoveMessage(Messageobject msg)
        {
            var msgItem = ChatMessageList.FirstOrDefault(m => m.messageId == msg.messageId);//获取
            if (msgItem != null)
            {
                ChatMessageList.Remove(msgItem);//删除
            }
        }
        #endregion

        #region 查看更多消息
        /// <summary>
        /// 查看更多消息
        /// </summary>
        private void GetMoreMessage()
        {
            if (ChatMessageList != null && ChatMessageList.Count > 0)
            {
                if (ChatMessageList[0].type == kWCMessageType.LocalTypeNotice && ChatMessageList[0].content.Contains("无更多聊天记录"))
                {
                    return;
                }
            }
            long lastSendTime;
            string sessJid = ServiceLocator.Current.GetInstance<MainViewModel>().Sess.Jid;//显示
            if (ChatMessageList.Count == 0)
            {
                lastSendTime = Helpers.DatetimeToStamp(DateTime.Now, true);//如果无消息传入当前时间
            }
            else
            {
                lastSendTime = Helpers.DatetimeToStamp(Helpers.StampToDatetime(ChatMessageList[0].timeSend), true);//获取毫秒时间戳
            }
            //
            if (HttpService == null)
            {
                //消息数量为0 或者 第一条消息不为"无更多消息"的提示消息  ，就调用历史消息
                if (ChatMessageList.Count == 0 ||
                    (!ChatMessageList[0].content.Contains("无更多聊天记录") && ChatMessageList[0].type != kWCMessageType.LocalTypeNotice))
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        IsLoading = true;
                    });
                    HttpService = APIHelper.GetMessageAsync(sessJid, lastSendTime);
                    HttpService.UploadDataCompleted += GetMessagesComplete;
                }
            }
        }

        /// <summary>
        /// 查看消息完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMessagesComplete(object sender, UploadDataCompletedEventArgs e)
        {
            try
            {
                var resStr = Encoding.UTF8.GetString(e.Result);//使用
                var afterHtmlTxt = Helpers.ClearHTML(resStr);
                var jsonRtn = JsonConvert.DeserializeObject<JsonParentMessage>(afterHtmlTxt);
                List<Messageobject> msgList = new List<Messageobject>();
                #region 获取返回消息
                foreach (var body in jsonRtn.data)
                {
                    //body.Body = System.Web.HttpUtility.HtmlDecode(body.Body);//解码
                    Messageobject reciveJsonMsg = JsonConvert.DeserializeObject<Messageobject>(body.Body);
                    if (reciveJsonMsg.isEncrypt == 1)
                    {
                        //string kyes = (Applicate.ApiKey + JsonMsg.timeSend + JsonMsg.messageId).MD5create();
                        reciveJsonMsg.content = AES.TDecrypt3Des(reciveJsonMsg.content, null);//解密
                    }
                    //reciveJsonMsg.jid = roomId;
                    #region 拼接完整消息
                    if (!string.IsNullOrWhiteSpace(body.room_jid_id))//群聊
                    {
                        reciveJsonMsg.ToId = body.room_jid_id;//群聊Jid
                        reciveJsonMsg.FromId = body.sender;//发送者
                    }
                    else//单聊
                    {
                        if (string.IsNullOrWhiteSpace(reciveJsonMsg.ToId))
                        {
                            reciveJsonMsg.ToId = body.direction == 1 ? body.sender : body.receiver;//设置接收者
                        }
                        if (string.IsNullOrWhiteSpace(reciveJsonMsg.FromId))
                        {
                            reciveJsonMsg.FromId = body.direction == 1 ? body.sender : body.receiver;//设置发送者
                        }
                    }
                    reciveJsonMsg.messageState = body.isRead == 1 ? 2 : 1;//设置已读或送达
                    #endregion
                    reciveJsonMsg.isRead = body.isRead;
                    msgList.Add(reciveJsonMsg);
                }
                #endregion
                Task.Run(() =>
                {
                    for (int i = 0; i < msgList.Count; i++)//插入数据库
                    {
                        msgList[i].Insert();
                    }
                });
                if (msgList.Count < 100)//如果返回数量小于100则为无更多消息
                {
                    //如果滚动页面中没有消息，或  滚动页面中最顶部一条消息位为 普通消息
                    if (ChatMessageList.Count == 0 ||
                        (ChatMessageList[0].type != kWCMessageType.LocalTypeNotice
                        && !ChatMessageList[0].content.Contains("无更多聊天记录")))
                    {
                        var tipmsg = new Messageobject()
                        {
                            content = "已无更多聊天记录",
                            type = kWCMessageType.LocalTypeNotice,
                            FromId = Applicate.MyAccount.userId,
                            ToId = ServiceLocator.Current.GetInstance<MainViewModel>().Sess.Jid,
                            timeSend = 126227520//设置发送时间为最早
                        };
                        Task.Run(() =>
                        {
                            tipmsg.Insert();
                        });
                        InsertSingleChatMessage(tipmsg);//显示至页面
                    }
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    int historyCount = 15;
                    if (msgList.Count < 15)//计算最大显示15条消息
                    {
                        historyCount = msgList.Count;
                    }
                    #region 只显示15条消息
                    var tmplst = new List<Messageobject>();
                    for (int i = historyCount - 1; i >= 0; i--)//获取
                    {
                        tmplst.Add(msgList[i]);
                    }
                    if (ChatMessageList[0].type == kWCMessageType.LocalTypeNotice && ChatMessageList[0].content == "已无更多聊天记录")
                    {
                        InsertChatMessage(tmplst, 1);//插入顶部的消息
                    }
                    else
                    {
                        InsertChatMessage(tmplst);//插入顶部的消息
                    }
                    #endregion
                    Task.Run(() =>
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {

                                var previousOffset = 0.0;//Get old messages' height
                                for (int i = 0; i < 5; i++)//获取5条消息记录的高度
                                {
                                    previousOffset += ChatMessageList[i].BubbleHeight;
                                }
                                for (int i = 0; i < tmplst.Count; i++)//Plus new messages' height
                                {
                                    previousOffset += ChatMessageList[i].BubbleHeight;
                                }
                                Messenger.Default.Send(previousOffset, ChatBubbleListControl.ScrollChatMessageListVerticalOffset);//通知界面滚动至原始位置

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMessagesCompleteError" + ex.Message);
                LogHelper.log.Error("-_-_-_MessagesCompleteError" + ex.Message, ex);
            }
            finally
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    IsLoading = false;
                });
                ((HttpClient)sender).Dispose();//完成后释放资源
                HttpService = null;//初始化Http访问
            }
        }
        #endregion

        #region 插入单条消息至顶部
        private void InsertSingleChatMessage(Messageobject msg)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ChatMessageList.Insert(0, msg.MessageObjectToBubbleItem());
            });
        }
        #endregion

        #region 撤回消息
        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="msg">需要撤回的消息</param>
        private void WithDrawMessage(Messageobject msg)
        {
            if (ChatMessageList.Count(c => c.messageId == msg.messageId) > 0)
            {
                var cmsg = ChatMessageList.FirstOrDefault(c => c.messageId == msg.messageId);
                cmsg.content = msg.content;//内容
                cmsg.type = msg.type;//更新为撤回消息
                #region 移除并添加
                int index = ChatMessageList.IndexOf(cmsg);
                ChatMessageList.Remove(cmsg);
                ChatMessageList.Insert(index, cmsg);
                #endregion
            }
        }
        #endregion

        #region 滚动事件
        /// <summary>
        /// 滚动时事件
        /// </summary>
        /// <param name="eve">滚动相关数据</param>
        private void ChatListScrollChanged(ScrollChangedEventArgs eve)
        {
            ShowOrHideFAB(eve);
            if (eve.VerticalChange < 0)//滚动长度小于0 (往上滚动)
            {
                //Messenger.Default.Send(false, ChatBubbleListControl.ScrollChatBubbleMessageToBottom);//自动滚动
                if (ChatMessageList.Count > 1)//如果显示消息不少于十条时显示
                {
                    var lastMsgOffset = 0.0;
                    for (int i = 0; i < ChatMessageList.Count; i++)
                    {
                        lastMsgOffset += ChatMessageList[i].BubbleHeight;
                    }
                    lastMsgOffset = lastMsgOffset * 0.1;
                    if (eve.VerticalOffset <= lastMsgOffset)//当只剩10%的长度时
                    {
                        Task.Run(() =>
                        {
                            Messageobject tmpMsg = new Messageobject()
                            {
                                FromId = Applicate.MyAccount.userId,
                                ToId = ChatMessageList[0].jid
                            };
                            List<Messageobject> msgList = tmpMsg.GetPageList(ChatMessageList.Count);//获取第二页消息
                            if (msgList.Count > 0)//如果查出消息才显示
                            {
                                App.Current.Dispatcher.Invoke(() =>
                               {
                                   InsertChatMessage(msgList);
                                   Task.Run(() =>
                                   {
                                       App.Current.Dispatcher.Invoke(() =>
                                       {
                                           var scroll = eve.Source as ScrollViewer;//Get ScrollViewer
                                           var previousOffset = lastMsgOffset;//Get old messages' height
                                           for (int i = 0; i < msgList.Count; i++)//Plus new messages' height
                                           {
                                               previousOffset += ChatMessageList[i].BubbleHeight;
                                           }
                                           scroll.ScrollToVerticalOffset(previousOffset);//Do scroll
                                       });
                                   });
                               });//插入和显示消息
                            }
                            else//未查出消息
                            {
                                GetMoreMessage();//从服务器获取消息
                            }
                        });
                    }
                }
            }
            else//往下滚动
            {
                //Messenger.Default.Send(true, ChatBubbleListControl.ScrollChatBubbleMessageToBottom);//自动滚动
            }
        }
        #endregion

        #region 显示或隐藏对应的FloatingActionButton
        /// <summary>
        /// 显示或隐藏对应的FloatingActionButton
        /// </summary>
        /// <param name="eve">滚动参数</param>
        private void ShowOrHideFAB(ScrollChangedEventArgs eve)
        {
            Task.Run(() =>
            {
                if (ChatMessageList.Count > 2)//如果消息大于
                {
                    var control = eve.Source as ScrollViewer;
                    var totalHeight = control.ScrollableHeight;
                    var lastHeight = ChatMessageList[ChatMessageList.Count - 1].BubbleHeight;
                    if (totalHeight - eve.VerticalOffset < lastHeight)//高于最后一条消息气泡显示返回到底部按钮
                    {
                        Messenger.Default.Send(true, ChatBubbleListControl.FloatingActionButtonShowUp);
                    }
                    else//隐藏
                    {
                        Messenger.Default.Send(true, ChatBubbleListControl.FloatingActionButtonHideOff);
                    }
                }
            });
        }
        #endregion

        #region 插入顶部消息
        /// <summary>
        /// 插入顶部消息
        /// </summary>
        /// <param name="msgList">需要插入的消息集合</param>
        /// <param name="offset">排除的偏移量</param>
        public void InsertChatMessage(List<Messageobject> msgList, int offset = 0)
        {
            ChatMessageList.InsertRange(msgList.MsgListToObservableMsgList());//倒叙插入
            SendReadMsgAsync(msgList);//发送已读
        }
        #endregion

        #region 显示消息集合
        /// <summary>
        /// 显示消息气泡
        /// </summary>
        /// <param name="messageList"></param>
        public void ShowChatMessage(List<Messageobject> messageList)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                //First Clean the entire ChatMessageList
                ChatMessageList.Clear();
                //Add to the ChatMessageList
                ChatMessageList.AddRange(messageList.MsgListToObservableMsgList());
                Task.Run(() =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Messenger.Default.Send(true, ChatBubbleListControl.ScrollChatBubbleMessageToBottom);//自动滚动
                    });
                });
            });
        }
        #endregion

        #region 显示单个消息气泡
        /// <summary>
        /// 显示单个消息气泡
        /// </summary>
        /// <param name="message"></param>
        public void AddSingleChatMessage(Messageobject message)
        {
            if (ServiceLocator.Current.GetInstance<MainViewModel>().Sess.Jid == message.jid)
            {
                //Add the single to ChatMessageList
                ChatMessageList.Add(message.MessageObjectToBubbleItem());
                Messenger.Default.Send(true, ChatBubbleListControl.ScrollChatBubbleMessageToBottom);//自动滚动
                SendReadMsgAsync(new List<Messageobject> { message });//发送已读
            }
        }
        #endregion

        #region 默认显示消息
        /// <summary>
        /// 显示默认聊天信息(15条)
        /// </summary>
        public Messageobject ShowDefaultMessage()
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            ChatMessageList.Clear();
            var tmpMsg = new Messageobject()
            {
                FromId = Applicate.MyAccount.userId,
                ToId = main.Sess.Jid
            };
            List<Messageobject> msgList = tmpMsg.GetPageList(0);
            ShowChatMessage(msgList);//显示消息气泡
            SendReadMsgAsync(msgList);//发送已读
            return msgList.LastOrDefault();
        }
        #endregion

        #region 显示默认消息
        /// <summary>
        /// 显示默认消息
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public void ShowDefaultMessage(string jid)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ChatMessageList.Clear();//
            });
            var tmpMsg = new Messageobject()
            {
                FromId = Applicate.MyAccount.userId,
                ToId = jid
            };
            List<Messageobject> msgList = tmpMsg.GetPageList(0);
            ShowChatMessage(msgList);//显示消息气泡
            SendReadMsgAsync(msgList);//发送已读
            if (msgList.Count < 15)//如果消息数量不足15条则拉取100条存入数据库并显示15条
            {
                GetMoreMessage();
            }
        }
        #endregion

        #region 收到消息
        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="msg">Message</param>
        public void ProcessNewMessage(Messageobject msg)
        {
            //处理撤回消息
            switch (msg.type)
            {
                case kWCMessageType.RoomMemberNameChange:
                    UpdateGroupMemberRemark(new MessageListItem { Jid = msg.fromUserId, ShowTitle = msg.content, Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(msg.fromUserId) });
                    break;
                case kWCMessageType.Withdraw:
                    var withdraw = ChatMessageList.FirstOrDefault(m => m.messageId == msg.content);
                    if (withdraw != null)
                    {
                        ChatMessageList.Remove(withdraw);//移除对应消息
                    }
                    break;
                case kWCMessageType.PokeMessage://戳一戳
                    break;
                case kWCMessageType.RoomFileDelete://文件删除
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 更新群昵称
        /// <summary>
        /// 更新群昵称
        /// </summary>
        /// <param name="item">更新项</param>
        private void UpdateGroupMemberRemark(MessageListItem item)
        {
            var lists = ChatMessageList.Where(c => c.fromUserId == item.Jid).ToList();
            if (lists.Count > 0)
            {
                for (int i = 0; i < lists.Count; i++)
                {
                    lists[i].fromUserName = item.ShowTitle;//设置修改后的昵称
                }
                Task.Run(() =>
                {
                    //for (int i = 0; i < lists.Count; i++)
                    //{
                    lists[0].UpdateNicknameByUserId(lists[0].jid, item.Jid, item.ShowTitle);//更新数据库
                                                                                            //}
                });
            }
        }
        #endregion

        #region 送达回执
        public void MsgReceipt(Messageobject msg)
        {
            var existsItem = ChatMessageList.FirstOrDefault(m => m.messageId == msg.messageId);//获取对应消息
            if (existsItem != null)
            {
                existsItem.messageState = 1;//设为送达
                existsItem.StateIcon = PackIconKind.Check;//设为送达
                existsItem.MsgStateText = "送达";
                existsItem.ReadToolTip = "送达";
            }
        }
        #endregion


        #region 消息发送超时
        public void SendTimeout(Messageobject msg)
        {
            var target = ChatMessageList.FirstOrDefault(m => m.messageId == msg.messageId);
            if (target != null)
            {
                target.messageState = -1;
                //target.StateIcon = PackIconKind.AlertCircle;

                target.UpdateMessageState();
                //target = target;
            }
        }
        #endregion

        #region Private Helper

        #region 发送已读消息
        /// <summary>
        /// 发送已读消息
        /// </summary>
        /// <param name="msgList">消息列表</param>
        private void SendReadMsgAsync(IList<Messageobject> msgList)
        {
            if (msgList != null && msgList.Count > 0)
            {
                Task.Run(() =>
                {
                    /*foreach (var msg in msgList)
                    {
                        if (msg.isMySend == 0 && msg.isRead == 0)//非自己发送 And 未读消息
                        {
                            ShiKuManager.SendRead(msg.messageId, msg.jid);
                        }
                    }*/
                    for (int a = 0; a < msgList.Count; a++)
                    {
                        if (msgList[a].isMySend == 0 && msgList[a].isRead == 0 && msgList[a].type > kWCMessageType.kWCMessageTypeNone)//非自己发送 And 未读消息
                        {
                            ShiKuManager.SendRead(msgList[a].messageId, msgList[a].jid);
                            msgList[a].UpdateIsRead(msgList[a].messageId);
                        }
                    }
                });
            }
        }
        #endregion

        #endregion

    }
}
