using agsXMPP;
using agsXMPP.Net;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.agent;
using agsXMPP.protocol.iq.roster;
using agsXMPP.Sasl;
using agsXMPP.Xml.Dom;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using ShikuIM.Model;
using ShikuIM.UserControls;
using ShikuIM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace ShikuIM
{
    /// <summary>
    /// Xmpp控制类
    /// </summary>
    internal class XmppManager
    {
        /// <summary>
        /// XMPP连接对象
        /// </summary>
        internal XmppClientConnection XmppCon;

        /// <summary>
        /// 用于间隔一定时间重连的计时器
        /// </summary>
        private Timer ReConnectionTimer = new Timer();
        private int ConnectionTimes = 0;

        #region 构造函数(初始化XmppClientConnection对象)
        /// <summary>
        /// 构造函数：初始化XmppClientConnection对象
        /// </summary>
        internal XmppManager()
        {
            //初始化XMPPConnection
            XmppCon = new XmppClientConnection();
            ReConnectionTimer.Elapsed += (sen, eve) =>
            {
                if (XmppCon.XmppConnectionState == XmppConnectionState.SessionStarted)
                {
                    ReConnectionTimer.Stop();//如果6秒内    
                    ConsoleLog.Output("停止重连计时器...");
                }
                Task.Run(() =>
                {
                    XmppCon.Open();//到达时间重连
                });

                //ConnectionTimes++;
                //if (ConnectionTimes == 3)
                //{
                //    Messenger.Default.Send("正在重新连接...", MainViewNotifactions.SnacbarMessage);//主窗口提示
                //}               
                ConsoleLog.Output("正在重新连接...");
            };
            XmppCon.KeepAlive = true;//保持在线
            double xTimeout = Applicate.URLDATA.data.xmppPingTime;
            if (Applicate.URLDATA.data.xmppPingTime <= 0)
            {
                xTimeout = 7;
            }
            ReConnectionTimer.Interval = xTimeout * 1000;//设置断线重连时间为6
            XmppCon.KeepAliveInterval = Convert.ToInt32(xTimeout);//心跳包间隔时间
            //设置套接字连接类型(SocketConnectionType)为 Direct
            XmppCon.SocketConnectionType = SocketConnectionType.Direct;//设置套接字连接类型
            //设置事件
            XmppCon.OnReadXml += new XmlHandler(XmppCon_OnReadXml);
            XmppCon.OnWriteXml += new XmlHandler(XmppCon_OnWriteXml);
            //Xmpp连接状态改变时
            XmppCon.OnXmppConnectionStateChanged += OnXmppStateChanged;
            XmppCon.OnRosterStart += XmppCon_OnRosterStart;
            XmppCon.OnRosterEnd += XmppCon_OnRosterEnd;
            XmppCon.OnRosterItem += XmppCon_OnRosterItem;
            XmppCon.OnAgentStart += XmppCon_OnAgentStart;
            XmppCon.OnAgentEnd += XmppCon_OnAgentEnd;
            XmppCon.OnAgentItem += XmppCon_OnAgentItem;
            //登录  和  关闭
            XmppCon.OnLogin += XmppCon_OnLogin;
            XmppCon.OnClose += XmppCon_OnClose;
            XmppCon.OnPresence += XmppCon_OnPresence;
            //此处接收信息
            XmppCon.OnMessage += XmppCon_OnMessage;
            //错误时  保持时
            XmppCon.OnError += XmppCon_OnError;
            XmppCon.OnIq += XmppCon_OnIq;
            XmppCon.OnAuthError += XmppCon_OnAuthError;
            XmppCon.OnSocketError += XmppCon_OnSocketError;
            XmppCon.OnStreamError += XmppCon_OnStreamError;
            /*
            XmppCon.OnReadSocketData += new BaseSocket.OnSocketDataHandler(ClientSockext_OnReceive);
            XmppCon.OnWriteSocketData += new BaseSocket.OnSocketDataHandler(ClientSocket_OnSend);
            */
            XmppCon.ClientSocket.OnValidateCertificate += new RemoteCertificateValidationCallback(ClientSocket_OnValidateCertificate);
            XmppCon.OnSaslStart += new SaslEventHandler(XmppCon_OnSaslStart);
        }
        #endregion
        #region StreamError
        /// <summary>
        /// 流错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XmppCon_OnStreamError(object sender, Element e)
        {
            agsXMPP.protocol.Stream stream = new agsXMPP.protocol.Stream();
            //stream.SetData(Encoding.Default.GetString(data));
            if (e.ToString().Contains("conflict"))
            {
                //XmppCon = new XmppClientConnection();
                //XmppCon.Close();//首先关闭连接
                MessageBoxResult res = MessageBoxResult.None;
                App.Current.Dispatcher.Invoke(() =>
                {
                    res = MessageBox.Show(Application.Current.MainWindow, "您已在其他设备登录,请重新登录！", "其他设备登录提示", MessageBoxButton.OK);
                    ShiKuManager.ApplicationExit();
                    System.Windows.Forms.Application.Restart();
                    Application.Current.Shutdown();
                });
                XmppCon.Close();//关闭
            }
        }

        #endregion
        #region XmppCon_OnSaslStart
        private void XmppCon_OnSaslStart(object sender, SaslEventArgs args)
        {

        }
        #endregion
        #region --------------OnXmppStateChanged----------------
        /// <summary>
        /// Xmpp连接状态改变时 
        /// <param name="sender"></param>
        /// <param name="state"></param>
        private void OnXmppStateChanged(object sender, XmppConnectionState state)
        {
            LogHelper.log.Info("------Xmpp状态：" + state.ToString());//输出日志
            ConsoleLog.Output("------Xmpp状态变为：" + state.ToString());
            //如果是已登录状态
            if (Applicate.IsAccountVerified)
            {
                Messenger.Default.Send(state, MainViewNotifactions.XmppConnectionStateChanged);
                Task.Run(() =>
                {
                    //通知
                    #region 如果连接状态为'会话打开'
                    if (state == XmppConnectionState.SessionStarted && Applicate.IsAccountVerified == true)
                    {
                        LogHelper.log.Info("Logined and Xmpp Connected successful");
                        JoinXmppGroups();//Xmpp加入群聊
                        //SendCheckPlatform();//发送设备验证在线消息类型
                        ReConnectionTimer.Stop();//停止重连计时
                    }
                    #endregion
                    else if (state == XmppConnectionState.Disconnected)//如果连接断开就重新连接
                    {
                        ReConnectionTimer.Start();//开始重连计时
                        Task.Run(() =>
                        {
                            //保存离线时间
                            new LocalUser().UpdateLastExitTime(Applicate.MyAccount.userId, Helpers.DatetimeToStamp(DateTime.Now));
                        });
                        XmppCon.ConnectServer = Applicate.URLDATA.data.XMPPHost;
                        XmppCon.Open();
                    }
                });
            }
            else//如果当前用户没有通过验证
            {
                //不做操作
            }
        }
        #endregion

        #region ClientSocket_OnValidateCertificate
        private bool ClientSocket_OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion
        #region ClientSocket_OnSend
        private void ClientSocket_OnSend(object sender, byte[] data, int count)
        {
            ConsoleLog.Output("ClientSocket_OnSend：" + Encoding.UTF8.GetString(data));
        }
        #endregion
        #region ClientSocket_OnReceive
        private void ClientSocket_OnReceive(object sender, byte[] data, int count)
        {
            ConsoleLog.Output("---ClientSocket_OnReceive：" + Encoding.UTF8.GetString(data));
            return;
        }
        #endregion
        #region XmppCon_OnSocketError
        private void XmppCon_OnSocketError(object sender, Exception ex)
        {
            LogHelper.log.Error("--XmppCon_OnSocketError" + ex.Message, ex);
            ConsoleLog.Output("XmppCon_OnSocketError：" + ex.Message);
        }
        #endregion
        #region XmppCon_OnAuthError
        private void XmppCon_OnAuthError(object sender, Element e)
        {
            ConsoleLog.Output("XmppCon_OnAuthError：" + e.InnerXml);
            Applicate.IsAccountVerified = false;
            if (e.InnerXml.Contains("Password not verified"))
            {
                var loginview = ServiceLocator.Current.GetInstance<LoginAndRegisterViewModel>();
                loginview.SnackBar.Enqueue("密码错误，请重新输入");
                loginview.InitialRegisterProperties();
                this.XmppCon.Close();//关闭Xmpp连接
                App.Current.Dispatcher.Invoke(() =>
                {
                    var mainwindow = Applicate.GetWindow<MainWindow>();
                    var amain = Application.Current.MainWindow;
                    if (amain is MainWindow)
                    {
                        mainwindow.Hide();
                        Application.Current.MainWindow = new Login();//隐藏主窗口
                        Application.Current.MainWindow.Show();
                    }
                });
            }
        }
        #endregion
        #region XmppCon_OnIq
        private void XmppCon_OnIq(object sender, IQ iq)
        {
            ConsoleLog.Output("-----XmppCon_OnIq：" + iq.ToString());
        }
        #endregion
        #region XmppCon_OnError
        private void XmppCon_OnError(object sender, Exception ex)
        {
            ConsoleLog.Output("\n\nXmppCon_OnError：" + ex.Message);
        }
        #endregion
        #region XmppCon_OnPresence
        /// <summary>
        /// 出席信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pres"></param>
        private void XmppCon_OnPresence(object sender, Presence pres)
        {
            LogHelper.log.Info("On___Presence\n" + pres.ToString() + "\n");
        }
        #endregion
        #region XMPP连接关闭时
        private void XmppCon_OnClose(object sender)
        {

        }
        #endregion
        #region Xmpp连接登录时
        private void XmppCon_OnLogin(object sender)
        {

        }
        #endregion
        #region XmppCon_OnAgentItem
        private void XmppCon_OnAgentItem(object sender, Agent agent)
        {

        }
        #endregion
        #region XmppCon_OnAgentEnd
        private void XmppCon_OnAgentEnd(object sender)
        {

        }
        #endregion
        #region XmppCon_OnAgentStart
        private void XmppCon_OnAgentStart(object sender)
        {

        }
        #endregion
        #region XmppCon_OnRosterItem
        private void XmppCon_OnRosterItem(object sender, RosterItem item)
        {

        }
        #endregion
        #region XmppCon_OnRosterEnd
        private void XmppCon_OnRosterEnd(object sender)
        {

        }
        #endregion
        #region XmppCon_OnRosterStart
        private void XmppCon_OnRosterStart(object sender)
        {

        }
        #endregion
        #region 发送XML时
        private void XmppCon_OnWriteXml(object sender, string xml)
        {
            ConsoleLog.Output("XmppCon_OnWriteXml" + xml);
        }
        #endregion
        #region 收到XML时
        private void XmppCon_OnReadXml(object sender, string xml)
        {
            ConsoleLog.Output("XmppCon_OnReadXml" + xml);
            /*
            if (xml.Contains("Please configure"))
            {
                ConfigRoom();
            }
            */
        }
        #endregion
        #region XmppCon_OnMessage接收到消息---------------------------
        /// <summary>
        /// Xmpp接收到信息时  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="xmppMsg"></param>
        private void XmppCon_OnMessage(object sender, Message xmppMsg)
        {
            ConsoleLog.Output("-------------XmppCon_OnMessage:" + xmppMsg);
            LogHelper.log.Info("***OnMessage--:" + xmppMsg.ToString());
            #region 如果Xmpp消息中body为null并且没有送达回执, 跳出此方法
            if (xmppMsg.Body == null && !xmppMsg.HasTag("received"))
            {
                return;
            }
            #endregion
            var JsonMsg = new Messageobject();
            #region 消息回执(更新本地的发送中标志等~)
            if (xmppMsg.HasTag("received"))//如果XML节点中含有recived节点则为送达回执
            {
                ProcessXmppReceived(xmppMsg);//处理消息送达回执
                return;
            }
            #endregion
            #region 不知道什么类型的消息
            else if (xmppMsg.HasTag(typeof(agsXMPP.protocol.extensions.ibb.Data)))
            {
                // ignore IBB messages
                MessageBox.Show("------agsXMPP.protocol.extensions.ibb.Data------XmppCon_OnMessage：" + xmppMsg.Body);
                return;
            }
            #endregion
            #region 单聊消息
            else if (xmppMsg.Type == MessageType.chat)
            {
                #region 强行删除部分消息类型content中的Json消息
                //(因部分消息类型content属性为Json，而MessageObject设定为content为string,JsonConverter遇到符合的Json时不能转为string,故需删除content内的字符。)
                //string flush = "";
                //if (xmppMsg.Body.Contains("machineIdentifier"))//确定为群文件 删除消息
                //{
                //    int start = xmppMsg.Body.IndexOf("content\":{");
                //    int end = xmppMsg.Body.IndexOf("}");
                //    flush = xmppMsg.Body.Substring(start, end - 1);
                //    xmppMsg.Body = xmppMsg.Body.Replace(flush, "content\":\"\"");
                //}
                #endregion
                JsonMsg = JsonConvert.DeserializeObject<Messageobject>(xmppMsg.Body);//反序列化
                #region 拼接完善json消息
                if (JsonMsg.messageId == null || JsonMsg.messageId == "")
                {
                    JsonMsg.messageId = xmppMsg.Id;
                }
                JsonMsg.isGroup = 0;//非群聊消息
                JsonMsg.isSend = 1;//已送达
                bool isGrroupMsg =
                    (JsonMsg.type >= kWCMessageType.RoomMemberNameChange && JsonMsg.type <= kWCMessageType.RoomManagerTransfer)
                    || (JsonMsg.type >= kWCMessageType.RoomFileUpload && JsonMsg.type <= kWCMessageType.RoomFileDownload);
                //处理群组设置类消息
                if (isGrroupMsg)
                {
                    JsonMsg.FromId = JsonMsg.objectId;//发送者
                    if (JsonMsg.type == kWCMessageType.RoomInvite || JsonMsg.type == kWCMessageType.RoomExit)
                    {
                        //进群邀请和退群的fromUserId为邀请者
                        JsonMsg.FromId = JsonMsg.objectId;
                    }
                    else
                    {
                        JsonMsg.fromUserId = JsonMsg.objectId;//发送者
                    }
                    JsonMsg.isGroup = 1;//群组类型消息
                }
                else
                {
                    if (JsonMsg.ToId == "" || JsonMsg.ToId == null)
                    {
                        JsonMsg.ToId = xmppMsg.To.User;
                    }
                    JsonMsg.isGroup = 0;//非群聊消息
                    if (xmppMsg.From != null)
                    {
                        JsonMsg.FromId = xmppMsg.From.User;
                    }
                    else
                    {
                        JsonMsg.FromId = JsonMsg.fromUserId;
                    }
                    if (xmppMsg.From.User == Applicate.MyAccount.userId && JsonMsg.ToId == Applicate.MyAccount.userId)//如果是自己在其他端发送的消息
                    { //设置消息内部
                        switch (xmppMsg.From.Resource)
                        {
                            case "android":
                                JsonMsg.PlatformType = 1;
                                break;
                            case "ios":
                                JsonMsg.PlatformType = 2;
                                break;
                            case "web":
                                JsonMsg.PlatformType = 3;
                                break;
                            case "mac":
                                JsonMsg.PlatformType = 4;
                                break;
                            default:
                                JsonMsg.PlatformType = 5;
                                break;
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region 群聊消息
            else if (xmppMsg.Type == MessageType.groupchat)
            {
                ConsoleLog.Output("-----------------------------------------------XmppCon_OnMessage收到一条群组消息：" + xmppMsg);
                #region 判断是否包含json数据
                if (xmppMsg.Body != null)
                {

                    if (!xmppMsg.Body.Contains("{"))
                    {
                        //如果消息Body中不包含 { 就跳出方法
                        return;
                    }
                }
                #endregion
                //JsonSerializerSettings sets = new JsonSerializerSettings();
                #region 强行删除content中的Json消息
                string flush = "";
                if (xmppMsg.Body.Contains("machineIdentifier"))//确定为群删除消息
                {
                    int start = xmppMsg.Body.IndexOf("content\":{");
                    int end = xmppMsg.Body.IndexOf("}");
                    flush = xmppMsg.Body.Substring(start, end - 1);
                    xmppMsg.Body = xmppMsg.Body.Replace(flush, "content\":\"\"");
                }
                #endregion
                //反序列化为MessageObject对象
                JsonMsg = JsonConvert.DeserializeObject<Messageobject>(xmppMsg.Body);

                #region 拼接json消息
                if (JsonMsg.messageId == null || JsonMsg.messageId == "")
                {
                    JsonMsg.messageId = xmppMsg.Id;
                }
                //设置为群聊消息
                JsonMsg.isGroup = 1;
                //设置房间JID
                //recivemsg.fromUserId = xmppMsg.From.User;
                //设置UserID
                if (JsonMsg.toUserId == "")
                {
                    JsonMsg.toUserId = Applicate.MyAccount.userId;//接收者UserId
                    JsonMsg.toUserName = Applicate.MyAccount.nickname;//接收者UserName
                }
                JsonMsg.FromId = xmppMsg.From.User;//发送者
                JsonMsg.readPersons = "1";//设置1人已读
                #endregion
                //if (JsonMsg.fromUserId == Applicate.MyAccount.userId && JsonMsg.type < kWCMessageType.Remind)
                //{
                //    return;
                //}
            }
            #endregion
            #region 解密
            if (JsonMsg.isEncrypt == 1)
            {
                //string kyes = (Applicate.ApiKey + JsonMsg.timeSend + JsonMsg.messageId).MD5create();
                JsonMsg.content = AES.TDecrypt3Des(JsonMsg.content, null);//此处需要改参数
            }
            #endregion
            if (xmppMsg.To == null)//如果为空设置为自己
            {
                JsonMsg.ToId = Applicate.MyAccount.userId;
            }
            else
            {
                if (!string.IsNullOrEmpty(JsonMsg.toUserId))
                {
                    JsonMsg.ToId = JsonMsg.toUserId;
                }
                else
                {
                    JsonMsg.ToId = xmppMsg.To.User;
                }
            }
            SendReceiptXmlId(xmppMsg.Id, xmppMsg.From, xmppMsg.To, (xmppMsg.Type == MessageType.groupchat));//发送送达回执(群聊不发送)
            DoMessage(JsonMsg);
        }
        #endregion

        #region 处理消息回执
        /// <summary>
        /// 处理消息回执
        /// </summary>
        /// <param name="xmppMsg"></param>
        public void ProcessXmppReceived(Message xmppMsg)
        {
            if (xmppMsg.From.User == Applicate.MyAccount.userId)//自己的
            {
                if (xmppMsg.From.Resource == "pc")//PC发送的消息不处理
                {
                    return;
                }
                PlatformService.GetInstance().UpdatePlatformOnlineStatus(xmppMsg.From.Resource, true);
                //PlatformService.GetInstance().CreateNewPlatform(xmppMsg.From.Resource);
            }
            xmppMsg.ChildNodes.ToString();
            string messageID = xmppMsg.FirstChild.GetAttribute("id");//获取首个子项的id属性值
            //普通回执
            var tMsg = Applicate.SendingList.FirstOrDefault(m => m.MessageId == messageID);
            if (tMsg != null)
            {
                DoMsgReceipt(tMsg.TmpMsg);//接收到消息回执时(通知所有带有MessageIdUI)
                tMsg.TmpMsg.UpdateReceived();//更新消息状态为已送达
                ConsoleLog.Output("//接收到消息回执 --  " + messageID);
            }
            //撤回回执
            var withdrawMsg = Applicate.WithDrawMessageList.FirstOrDefault(m => m.messageId == messageID);
            if (withdrawMsg != null)
            {
                //DoRecallMsgReceipt(withdrawMsg);//接收到消息回执时(通知所有带有MessageIdUI)
                Applicate.WithDrawMessageList.RemoveAll(d => d.messageId == withdrawMsg.messageId);//移除对应消息
                /*
                new Messageobject
                {
                    ToId = withdrawMsg.ToId,
                    FromId = withdrawMsg.FromId,
                    messageId = withdrawMsg.content
                }.Delete();*/
                //删除对应MessageId消息
                ConsoleLog.Output("你撤回一条消息");
            }
            Messenger.Default.Send(xmppMsg.Id, CommonNotifications.XmppMsgReceipt);//通知每个已注册此Token的ViewModel
        }
        #endregion

        #region 普通消息通知
        /// <summary>
        /// 普通消息通知
        /// </summary>
        /// <param name="msg">需要处理的消息</param>
        private void DoMessage(Messageobject msg)
        {
            bool IsProcessed = false;//消息是否已处理
            bool isGroupMsg = (msg.type >= kWCMessageType.RoomMemberNameChange && msg.type <= kWCMessageType.RoomManagerTransfer)
                                            || (msg.type >= kWCMessageType.RoomFileUpload && msg.type <= kWCMessageType.RoomFileDownload);
            if (isGroupMsg)//群聊
            {
                HandleGroupManageMessage(msg);//处理群聊消息
                IsProcessed = true;
            }
            else if (msg.type >= kWCMessageType.FriendRequest
                &&
                msg.type <= kWCMessageType.PhoneContactToFriend)//验证ur消息
            {
                msg.FromId = "10001";
                ProcessVerifingMsgBegin(msg);//10001处理
                IsProcessed = true;
            }
            else if (msg.type == kWCMessageType.OnlineStatus)//在线情况不处理
            {

                return;
            }
            else
            {
                switch (msg.type)
                {
                    case kWCMessageType.IsRead:
                        DoMsgRead(msg);//接收者已读消息
                        IsProcessed = true;
                        break;
                    case kWCMessageType.Withdraw:
                        new Messageobject()
                        {
                            fromUserId = msg.fromUserId,
                            FromId = msg.FromId,
                            toUserId = msg.toUserId,
                            ToId = msg.ToId,
                            messageId = msg.content
                        }.Delete();
                        break;
                    case kWCMessageType.AudioChatCancel:
                        msg.content = "取消了语音通话";
                        break;
                    case kWCMessageType.VideoChatCancel:
                        msg.content = "取消了视频通话";
                        break;
                    case kWCMessageType.kWCMessageTypeNone://空消息不处理
                        return;
                    case kWCMessageType.RoomMemberBan:
                        msg.FromId = msg.objectId;
                        break;
                    default:
                        break;
                }
            }
            if (IsProcessed)
            {
                return;
            }
            int count = msg.Insert();//存储消息
            if (count != 0)//数据库成功插入
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Messenger.Default.Send(msg, CommonNotifications.XmppMsgRecived);//通知各页面收到消息
                });                
            }
        }
        #endregion

        #region 好友验证处理
        /// <summary>
        /// 好友验证处理
        /// </summary>
        /// <param name="msg">消息</param>
        private void ProcessVerifingMsgBegin(Messageobject msg)
        {
            if (msg != null)
            {
                msg.FromId = "10001";
                //msg.Insert();
                var tmpFriend = new DataOfFriends();
                if (!tmpFriend.ExistsFriend(msg.fromUserId))//是否存在
                {
                    var client = APIHelper.GetUserDetialAsync(msg.fromUserId);
                    client.Tag = msg;
                    client.UploadDataCompleted += ProcessVerifingMsgEnd;
                }
                else
                {
                    ProcessVerifingMsgEnd(new HttpClient() { Tag = msg });
                }
            }
        }

        /// <summary>
        /// 处理收到的验证消息结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessVerifingMsgEnd(object sender = null, UploadDataCompletedEventArgs e = null)
        {
            var msg = ((HttpClient)sender).Tag as Messageobject;//获取信息
            var tmpFriend = new DataOfFriends().GetByUserId(msg.fromUserId);
            if (tmpFriend == null || string.IsNullOrWhiteSpace(tmpFriend.toNickname))//如果事件值为空从本地获取好友
            {
                var client = APIHelper.GetUserDetialAsync(msg.fromUserId);
                client.Tag = msg;
                client.UploadDataCompleted += ProcessVerifingMsgEnd;//递归一次使用
                return;
            }
            switch (msg.type)
            {
                case kWCMessageType.DeleteFriend://被删除
                    tmpFriend.UpdateFriendState(tmpFriend.toUserId, 0);//设置为陌生人
                    break;
                case kWCMessageType.BlackFriend://被拉黑
                    tmpFriend.UpdateFriendState(msg.fromUserId, -2);//设置为被拉黑
                    break;
                case kWCMessageType.RequestFriendDirectly://直接添加好友
                case kWCMessageType.RequestAgree://被同意
                    var toast = new Messageobject
                    {
                        type = kWCMessageType.LocalTypeNotice,
                        FromId = msg.fromUserId,
                        ToId = msg.ToId,
                        fromUserId = msg.fromUserId,
                        toUserId = msg.toUserId,
                        messageId = Guid.NewGuid().ToString("N"),
                        timeSend = msg.timeSend,
                        content = "你们已经成为好友，快来聊天吧"
                    };
                    toast.Insert();//保存提示消息
                    tmpFriend.UpdateFriendState(tmpFriend.toUserId, 2);//设置为好友
                    break;
                case kWCMessageType.PhoneContactToFriend://通讯录添加好友
                    return;
                default:
                    break;
            }
            Messenger.Default.Send(msg, CommonNotifications.XmppMsgRecived);//通知各页面收到消息
        }
        #endregion

        #region 群组消息处理
        /// <summary>
        /// 群组消息处理
        /// <para>
        /// RoomMemberNameChange和RoomAllowSpeakCourse之间的群设置消息
        /// 主要为数据库操作
        /// </para>
        /// </summary>
        /// <param name="msg">对应的处理消息</param>
        private void HandleGroupManageMessage(Messageobject msg)
        {
            var room = new Room();
            room.jid = msg.objectId;//RoomJid
            msg.FromId = msg.objectId;
            room.id = msg.fileName ?? new Room().GetRoomIdByJid(msg.objectId);//RoomId if type is roominvite
            switch (msg.type)
            {
                case kWCMessageType.RoomFileUpload://上传群文件

                    break;
                case kWCMessageType.RoomFileDelete://删除群文件

                    break;
                case kWCMessageType.RoomFileDownload://下载群文件

                    break;
                case kWCMessageType.RoomMemberNameChange://改群内昵称
                    var mem = new DataofMember();
                    mem.userId = msg.toUserId;
                    mem.UpdateMemberNickname(msg.objectId, msg.FromId, msg.content);
                    break;
                case kWCMessageType.RoomNameChange://改名
                    room.UpdateNameByJid(msg.content);
                    break;
                case kWCMessageType.RoomDismiss://解散
                    room.DeleteByJid();
                    Messenger.Default.Send(true, GroupChatDetial.CloseGroupDetialWindow);
                    //return;
                    break;
                case kWCMessageType.RoomExit://退出
                    if (msg.toUserId == Applicate.MyAccount.userId)//我被踢
                    {
                        ExitGroup(room.jid);
                        new DataofMember().DeleteByRoomJid(room.jid);//我
                        room.DeleteByJid();
                    }
                    else//成员退群
                    {
                        new DataofMember() { userId = msg.toUserId }.DeleteTheMemberByRoomJid(room.jid);//删除退出好友的Userid(群成员退群)
                        //return;//土白菜不显示
                    }
                    break;
                case kWCMessageType.RoomNotice://公告
                    room.UpdateDescByJid(msg.content);//更新公告
                    break;
                case kWCMessageType.RoomInvite://进群
                    if (msg.toUserId == Applicate.MyAccount.userId)
                    {
                        //如果我已经在群里了
                        var isExists = room.Exists();
                        if (!isExists)
                        {
                            APIHelper.GetRoomDetialByRoomId(room.id);
                            var lastExitTime = new LocalUser().GetLastExitTime(Applicate.MyAccount.userId);
                            this.JoinGroup(msg.objectId, lastExitTime, msg.content);//Xmpp加入群聊(处理完群组后加入)
                        }
                    }
                    APIHelper.GetRoomMemberAsync(room.id);//更新群成员
                    break;
                case kWCMessageType.RoomReadVisiblity://显示阅读人数
                    room.UpdateShowRead(int.Parse(msg.content));
                    break;
                case kWCMessageType.RoomIsVerify://群验证
                    Regex reg = new Regex("^[A-F0-9]{8}([A-F0-9]{4}){3}[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                    if (reg.IsMatch(msg.objectId))
                    {
                        //开启群验证回执
                        room.UpdateNeedVerify(int.Parse(msg.content));
                    }
                    else
                    {
                        //收到进群验证请求
                        var roomVerify = JsonConvert.DeserializeObject<RoomVerify>(msg.objectId);
                        msg.FromId = roomVerify.roomJid;
                    }
                    break;
                case kWCMessageType.RoomAdmin://管理员
                    if (msg.content == "0")//取消管理员
                    {
                        new DataofMember() { userId = msg.toUserId, role = MemberRole.Member }.UpdateRoleByJid(room.jid);
                    }
                    else if (msg.content == "1")//设置管理员
                    {
                        new DataofMember() { userId = msg.toUserId, role = MemberRole.Admin }.UpdateRoleByJid(room.jid);
                    }
                    break;
                case kWCMessageType.RoomIsPublic://公开群
                    room.UpdateIsPublic(int.Parse(msg.content));
                    /* var allGroupObj = mControl.AllGroupList.FirstOrDefault(d => d.Jid == room.jid);
                    if (allGroupObj != null)
                        allGroupObj.IsVisibility = msg.content == "0";//0公开
                    */
                    break;
                case kWCMessageType.RoomInsideVisiblity://显示群内成员
                    room.UpdateShowMember(int.Parse(msg.content));
                    break;
                case kWCMessageType.RoomUserRecommend://是否允许发送名片
                    room.UpdateAllowSendCard(int.Parse(msg.content));
                    break;
                case kWCMessageType.RoomMemberBan://禁言成员
                    long talkTime = 0;
                    if (long.TryParse(msg.content, out talkTime))
                    {
                        new DataofMember() { userId = msg.toUserId }.UpdateTalkTime(room.jid, talkTime);
                    }
                    break;
                case kWCMessageType.RoomAllBanned://群组全员禁言
                    long talkTime2 = 0;
                    if (long.TryParse(msg.content, out talkTime2))
                    {
                        room.UpdateTalkTime(talkTime2);
                    }
                    break;
                case kWCMessageType.RoomAllowMemberInvite://是否允许群内普通成员邀请陌生人
                    room.UpdateAllowInviteFriend(int.Parse(msg.content));
                    break;
                case kWCMessageType.RoomManagerTransfer://是否转让群主
                    new DataofMember() { userId = msg.fromUserId, role = MemberRole.Member }.UpdateRoleByJid(room.jid);
                    new DataofMember() { userId = msg.toUserId, role = MemberRole.Owner }.UpdateRoleByJid(room.jid);
                    room.UpdateOwner(msg.toUserId);
                    break;
                case kWCMessageType.RoomAllowUploadFile://是否允许成员上传群文件
                    room.UpdateAllowUploadFile(int.Parse(msg.content));
                    break;
                case kWCMessageType.RoomAllowConference://是否允许群会议
                    room.UpdateAllowConference(int.Parse(msg.content));
                    break;
                case kWCMessageType.RoomAllowSpeakCourse://是否允许群成员开课
                    room.UpdateAllowSpeakCourse(int.Parse(msg.content));
                    break;
                default:
                    return;
            }
            int count = msg.Insert();//存储消息
            if (count != 0)//数据库成功插入
            {
                Messenger.Default.Send(msg, CommonNotifications.XmppMsgRecived);//通知各页面收到消息
            }
        }
        #endregion

        #region 退出群组
        /// <summary>
        /// Xmpp退出群组以不接收
        /// </summary>
        /// <param name="jid"></param>
        private void ExitGroup(string jid)
        {
            Presence pres = new Presence();
            pres.From = new Jid(Applicate.MyAccount.userId, XmppCon.MyJID.Server, "pc");
            pres.To = new Jid(jid, "muc." + XmppCon.MyJID.Server, "thirdwitch");
            pres.Type = PresenceType.unavailable;
            XmppCon.Send(pres);
        }
        #endregion

        #region 回执消息通知
        /// <summary>
        /// 回执消息通知
        /// </summary>
        /// <param name="msg">消息体</param>
        private void DoMsgReceipt(Messageobject msg)
        {
            RemoveFromSendingList(msg.messageId);//从正在发送消息队列中移除
            #region 好友验证消息
            if (msg.type >= kWCMessageType.FriendRequest &&
                 msg.type <= kWCMessageType.CancelBlackFriend)////好友验证消息
            {
                ProcessVerifingMsgRecBegin(msg);
                return;
            }
            #endregion

            #region 群聊设置消息
            if (msg.type >= kWCMessageType.RoomNameChange &&
                 msg.type <= kWCMessageType.RoomUserRecommend)//群组消息
            {
                var tmproom = new Room().GetByJid(msg.FromId);

                switch (msg.type)
                {
                    case kWCMessageType.RoomNameChange:
                        break;
                    case kWCMessageType.RoomDismiss:
                    case kWCMessageType.RoomExit:
                        tmproom.DeleteByJid(msg.FromId);//删除对应Jid的群组
                        break;
                    case kWCMessageType.RoomNotice:
                        break;
                    case kWCMessageType.RoomMemberBan:
                        break;
                    case kWCMessageType.RoomInvite:
                        break;
                    case kWCMessageType.RoomReadVisiblity:
                        break;
                    case kWCMessageType.RoomIsVerify:
                        break;
                    case kWCMessageType.RoomIsPublic:
                        break;
                    case kWCMessageType.RoomInsideVisiblity:
                        break;
                    case kWCMessageType.RoomUserRecommend:
                        break;
                    default:
                        break;
                }
            }
            #endregion

            Messenger.Default.Send(msg, CommonNotifications.XmppMsgReceipt);//通知每个已注册此Token的ViewModel
        }
        #endregion

        #region 开始处理
        /// <summary>
        /// 开始处理验证消息回执
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessVerifingMsgRecBegin(Messageobject msg)
        {
            var tmpFriend = new DataOfFriends();
            if (!tmpFriend.ExistsFriend(msg.ToId))//是否存在ProcessVerifingMsgEnd
            {
                var client = APIHelper.GetUserDetialAsync(msg.ToId);
                client.Tag = msg;
                client.UploadDataCompleted += ProcessVerifingMsgRecEnd;//处理
            }
            else
            {
                ProcessVerifingMsgRecEnd(new HttpClient() { Tag = msg });
            }
        }

        /// <summary>
        /// 处理验证消息回执完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessVerifingMsgRecEnd(object sender, UploadDataCompletedEventArgs e = null)
        {
            var msg = ((HttpClient)sender).Tag as Messageobject;//获取信息
            var tmpFriend = new DataOfFriends().GetByUserId(msg.ToId);
            if (tmpFriend == null || string.IsNullOrWhiteSpace(tmpFriend.toNickname))//如果事件值为空从本地获取好友
            {
                var client = APIHelper.GetUserDetialAsync(msg.ToId);
                client.Tag = msg;
                client.UploadDataCompleted += ProcessVerifingMsgRecEnd;//递归一次使用
                return;
            }
            switch (msg.type)
            {
                case kWCMessageType.DeleteFriend://被删除
                    tmpFriend.UpdateFriendState(0);//设置为陌生人
                    break;
                case kWCMessageType.BlackFriend://被拉黑
                    tmpFriend.UpdateFriendState(-2);//设置为被拉黑
                    break;
                case kWCMessageType.RequestFriendDirectly://直接添加好友
                case kWCMessageType.RequestAgree://被同意
                    var toast = new Messageobject
                    {
                        type = kWCMessageType.LocalTypeNotice,
                        ToId = msg.FromId,
                        FromId = msg.ToId,
                        toUserId = msg.fromUserId,
                        fromUserId = msg.toUserId,
                        messageId = Guid.NewGuid().ToString("N"),
                        timeSend = msg.timeSend,
                        content = "你们已经成为好友，快来聊天吧"
                    };
                    toast.Insert();//保存提示消息
                    tmpFriend.UpdateFriendState(2);//设置为好友
                    break;
                case kWCMessageType.CancelBlackFriend:
                    tmpFriend.UpdateFriendState(2);//设置为好友
                    break;
                default:
                    break;
            }
            Messenger.Default.Send(msg, CommonNotifications.XmppMsgReceipt);//通知每个已注册此Token的ViewModel
        }
        #endregion

        #region 接收者已读消息
        /// <summary>
        /// 接收者已读消息
        /// </summary>
        /// <param name="msg">消息体</param>
        private void DoMsgRead(Messageobject msg)
        {
            if (msg == null)
            {
                return;
            }

            msg.UpdateIsReadPersons(msg.content);
        }
        #endregion

        #region 添加至重发消息列表中
        /// <summary>
        /// 添加至重发消息列表中
        /// </summary>
        /// <param name="msg">可能需要重发的消息</param>
        internal void AddToSendingList(Messageobject msg)
        {
            //如果存在
            if (Applicate.SendingList.Exists(c => c.MessageId == msg.messageId))
            {
                return;
            }
            //不存在就添加
            MessageTimer timer = new MessageTimer(10000);//间隔10秒重发(10000毫秒)
            timer.MessageId = msg.messageId;
            timer.TmpMsg = msg;
            timer.Elapsed += new System.Timers.ElapsedEventHandler((sen, e) =>
            {
                var MsgTmer = (MessageTimer)sen;//获取
                /*到达间隔时 更新UI为感叹号 */
                MsgTmer.TmpMsg.messageState = 0;//设为发送中
                if (MsgTmer.ReSendCount == 3)
                {
                    MsgTmer.Stop();//停止
                    MsgTmer.ReSendCount = 0;//重置发送次数
                    Applicate.SendingList.Remove(MsgTmer);//移除此计时器
                    //通知超时
                }
                else
                {
                    //累加发送次数
                    ConsoleLog.Output("重发消息" + MsgTmer.TmpMsg.messageId + "第" + MsgTmer.ReSendCount + "次");
                    MsgTmer.ReSendCount++;
                    SendJsonMsg(MsgTmer.TmpMsg);
                }
            });
            timer.Start();//启动计时器
            //添加至集合
            Applicate.SendingList.Add(timer);
        }
        #endregion
        #region 删除对应MessageId的消息计时器
        /// <summary>
        /// 根据MessageId删除对应的消息计时器
        /// </summary>
        /// <param name="MessageId">对应的消息Id</param>
        public void RemoveFromSendingList(string MessageId)
        {
            try
            {
                if (Applicate.SendingList.Exists(m => m.MessageId == MessageId))//存在则操作
                {
                    Applicate.SendingList.FirstOrDefault(msg => msg.MessageId == MessageId).Stop();//停止计时器
                    //Applicate.SendingList.Remove(Applicate.SendingList.FirstOrDefault(msg => msg.MessageId == MessageId));//移除
                    Applicate.SendingList.RemoveAll(d => d.MessageId == MessageId);//移除
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Output("移除临时消息时出错" + ex.Message);
            }
        }
        #endregion

        #region 发送送达回执
        /// <summary>
        /// 发送消息回执(送达)
        /// </summary>
        /// <param name="recivemsg">信息的UID</param>
        /// <param name="From">接收回执者</param>
        /// <param name="to">发送回执者</param>
        private void SendReceiptXmlId(string MessageId, Jid From, Jid To, bool IsGroup = false)
        {
            try
            {
                if (IsGroup)
                {
                    return;
                }

                Message msg = new Message();
                msg.To = new Jid(From.User + "@" + XmppCon.Server);//设置接收者
                if (To == null)
                {
                    msg.From = XmppCon.MyJID;
                }
                else
                {
                    msg.From = new Jid(To);//设置发送者
                }
                msg.Type = (IsGroup) ? (MessageType.groupchat) : (MessageType.chat);//设置群组或单聊消息
                Element msgChild = new Element("received", null);//实例化一个新元素
                msgChild.SetAttribute("xmlns", "urn:xmpp:receipts");
                msgChild.SetAttribute("id", MessageId);//设置id为信息ID
                msg.AddChild(msgChild);//添加元素进Message中
                //发送消息回执
                this.XmppCon.Send(msg);
            }
            catch (Exception ex)
            {
                LogHelper.log.Error("----在发送消息回执时出错 : " + ex.Message, ex);
                ConsoleLog.Output("---------------------------------------------在发送消息回执时出错 : " + ex.Message, ex);
            }
        }
        #endregion
        #region 发送已读标志
        /// <summary>
        /// 发送已读标志
        /// </summary>
        /// <param name="readMsg">已读消息</param>
        /// <param name="to">接收者</param>
        public void SendIsRead(Messageobject readMsg, string to)
        {
            //新Message
            var message = new Message();
            try
            {
                #region 自定义属性
                //设置协议属性
                message.Type = (to.Length > 11) ? (MessageType.groupchat) : (MessageType.chat);//设置Xmpp消息类型
                message.Body = readMsg.ToJson(); //JsonConvert.SerializeObject(isRead);//body为已读回执
                message.To = (to.Length > 11) ? (new Jid(to, "muc." + Applicate.URLDATA.data.xMPPHost, "")) : (new Jid(to, XmppCon.Server, "youjob"));//接收者
                message.From = new Jid(Applicate.MyAccount.userId + "@" + XmppCon.Server);//设置发送者
                message.Id = readMsg.messageId;//设置信息的Uid
                #endregion
                if (to.Length < 20)
                {
                    #region 已读消息消息回执
                    var reqRec = new Element("request");//请求接送方发送消息回执
                    reqRec.SetAttribute("xmlns", "urn:xmpp:receipts");//设置回执属性
                    message.AddChild(reqRec);//添加请求回执的节点
                    message.SetAttribute("xmlns", "jabber:client");
                    #endregion
                }
                XmppCon.Send(message);//发送
            }
            catch (Exception ex)
            {
                LogHelper.log.Error("发送已读回执时出错" + ex.Message, ex);
                ConsoleLog.Output("---------------------------------------------收到消息后，发送已读回执时出错 : " + ex.Message);
            }
        }
        #endregion
        #region 发送正常消息
        /// <summary>
        /// 发送正常消息
        /// </summary>
        /// <param name="jsonMsg"></param>
        public void SendJsonMsg(Messageobject jsonMsg)
        {
            //生成XML信息
            Message xMsg = new Message();
            if (jsonMsg.messageId == null)
            {
                jsonMsg.messageId = Guid.NewGuid().ToString("N");
            }
            if (jsonMsg.isGroup >= 1)
            {
                xMsg.To = new Jid(jsonMsg.toUserId + "@muc." + XmppCon.Server);//设置消息的接收人
                xMsg.Type = MessageType.groupchat;//设置为群消息类型
            }
            else
            {
                xMsg.To = new Jid(jsonMsg.toUserId + "@" + XmppCon.Server);//设置消息的接收人
                xMsg.Type = MessageType.chat;//设置为单聊类型
            }

            #region 请求发送送达回执到本机
            //如果非群聊需要请求对方发送回执
            Element reqRec = new Element("request");//请求接送方发送消息回执
            reqRec.SetAttribute("xmlns", "urn:xmpp:receipts");//设置回执属性
            xMsg.AddChild(reqRec);//添加请求回执的节点e
            xMsg.SetAttribute("xmlns", "jabber:client");
            #endregion

            #region 加密
            if (jsonMsg.isEncrypt == 1)
            {
                //string kyes = (Applicate.ApiKey + JsonMsg.timeSend + JsonMsg.messageId).MD5create();
                jsonMsg.content = AES.Encrypt3Des(jsonMsg.content, null);//此处需要改参数
            }
            #endregion

            xMsg.From = jsonMsg.fromUserId + "@" + XmppCon.Server;//自己的
            xMsg.Body = jsonMsg.ToJson(); //JsonConvert.SerializeObject(jsonMsg);//
            xMsg.Id = jsonMsg.messageId;//设置协议中Id为body中的ui
            if (xMsg.Type == MessageType.chat)
            {
                for (int i = 0; i < Applicate.PlatformNames.Length; i++)
                {
                    var plats = PlatformService.GetInstance().GetPlatformByName(Applicate.PlatformNames[i]);
                    if (plats.IsOnline)//如果设备在线时，转发给对应的设备
                    {
                        var tmp = xMsg.Clone();
                        tmp.To = new Jid(Applicate.MyAccount.userId, XmppCon.MyJID.Server, Applicate.PlatformNames[i]);
                        XmppCon.Send(tmp);
                    }
                    AddToSendingList(jsonMsg);//添加到临时重发消息列表中 
                }
            }
            AddToSendingList(jsonMsg.Clone());//添加到临时重发消息列表中
            XmppCon.Send(xMsg);
        }
        #endregion
        #region 发送指定接收方的消息
        /// <summary>
        /// 发送原MessageObject中不带有UserId的消息出去
        /// </summary>
        /// <param name="jsonMsg">MessageObject对象</param>
        /// <param name="to">接收者</param>
        public void SendJsonMsg(Messageobject jsonMsg, string to)
        {
            var msg = new Message
            {
                Body = jsonMsg.ToJson(),//消息转Json
                Id = (jsonMsg.messageId == null) ? (Guid.NewGuid().ToString()) : (jsonMsg.messageId),//协议Id
                Type = (jsonMsg.isGroup == 1) ? (MessageType.groupchat) : (MessageType.chat),//接收者类型
                To = (jsonMsg.isGroup == 1) ? (new Jid(to, "muc." + Applicate.URLDATA.data.XMPPHost, "")) : (new Jid(to, XmppCon.Server, "")),//接收者
                From = new Jid(Applicate.MyAccount.userId, XmppCon.Server, "pc")//由自己发出
            };
            if (jsonMsg.ToId.Length > 15)//群聊
            {
                //var mymember = new DataofMember().GetModelByJid(jsonMsg.ToId);
                //jsonMsg.fromUserName = mymember.nickname;//设置自己的群成员昵称
                jsonMsg.fromUserName = jsonMsg.ToId;
            }
            msg.Id = jsonMsg.messageId;//Xmpp信息Message
            Element reqRec = new Element("request");//请求接送方发送消息回执
            reqRec.SetAttribute("xmlns", "urn:xmpp:receipts");//设置回执属性
            msg.AddChild(reqRec);//添加请求回执的节点
            msg.SetAttribute("xmlns", "jabber:client");
            AddToSendingList(jsonMsg);//添加至发送队列

            XmppCon.Send(msg);
        }
        #endregion

        #region 关闭Xmpp连接
        /// <summary>
        /// 关闭Xmpp连接
        /// </summary>
        internal void CloseAll()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                XmppCon.Close();
            });
        }
        #endregion
        #region 加入已存于列表的群聊
        /// <summary>
        /// 加入已存于列表的群聊
        /// </summary>
        internal void JoinXmppGroups()
        {
            Task.Run(() =>
            {
                var lastExitTime = new LocalUser().GetLastExitTime(Applicate.MyAccount.userId);
                var rooms = new Room().GetJoinedList();
                for (int i = 0; i < rooms.Count; i++)
                {
                    //Xmpp加入群聊
                    Jid jid = new Jid(rooms[i].jid, "muc." + XmppCon.Server, rooms[i].nickname);
                    JoinGroup(rooms[i].jid, lastExitTime, rooms[i].name);
                }
            });
        }
        #endregion
        #region 加入群聊
        /// <summary>
        /// 加入群聊
        /// </summary>
        /// <param name="roomJid">RoomId</param>
        /// <param name="lastExitTime"></param>
        /// <param name="resource">群昵称(非必需)</param>
        internal void JoinGroup(string roomJid, long lastExitTime, string resource = "")
        {
            Task.Factory.StartNew(() =>
            {
                Presence pres = new Presence();
                //设置JID接收
                Jid to = new Jid(roomJid + "@muc." + Applicate.URLDATA.data.XMPPDomain) + "/" + Applicate.MyAccount.userId;
                //to.Resource = resource;//需告诉服务器房间名称
                pres.To = to;
                #region 阻止显示历史消息的x节点
                Element x = new Element();
                Element history = new Element();
                history.TagName = "history";
                #region 获取上次离线与现在时差
                long tmp = Helpers.DatetimeToStamp(DateTime.Now) - lastExitTime;
                history.SetAttribute("seconds", tmp);//请求群组离线消息(总之就是阻止显示历史消息)
                #endregion
                /*
                var time = Utils.StampToDatetime(Convert.ToInt64(ConfigurationUtil.GetValue("lastExitStamp")));
                string since = time.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
                history.SetAttribute("since", since);
                */
                x.TagName = "x";
                x.SetAttribute("xmlns", "http://jabber.org/protocol/muc");
                x.AddChild(history);
                pres.AddChild(x);
                #endregion
                XmppCon.Send(pres);
                ConsoleLog.Output("加入群聊发送的消息为：-----" + pres.ToString());
            });
        }
        #endregion
        #region 退出房间
        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="roomJid">需要退出的房间ID</para   m>
        public void exitRoom(string roomJid)
        {
            Task.Run(() =>
            {
                Presence pres = new Presence();
                Jid jid = new Jid(roomJid + "@muc." + Applicate.URLDATA.data.xMPPHost);
                pres.To = jid;
                pres.Type = PresenceType.unavailable;
                XmppCon.Send(pres);
            });
        }
        #endregion

        #region 创建群组
        /// <summary>
        /// Xmpp建群
        /// </summary>
        /// <param name="room">群实例</param>
        /// <param name="members">群成员</param>
        internal void xCreateGroup(Room room, List<string> members)
        {
            Jid romJid = new Jid(room.jid, "muc." + XmppCon.Server, Applicate.MyAccount.userId);
            SendPresence(room, romJid);//第一步，发送Presence(声明多人聊天室)
            Task.Delay(600);
            ConfigRoomByIQ(room, romJid);//发送配置
        }
        #endregion

        #region Xmpp建群
        /// <summary>
        /// <presence
        ///   from='crone1@shakespeare.lit/desktop'
        ///   to='darkcave@chat.shakespeare.lit/firstwitch'>
        ///   <x xmlns = 'http://jabber.org/protocol/muc' />
        /// </presence>
        /// </summary>
        /// <param name="room"></param>
        private void SendPresence(Room room, Jid roomJid)
        {
            #region presence报文
            Presence presence = new Presence();
            //presence.Id = Guid.NewGuid().ToString("N");//报文Id
            //<presence id="1517651565" to="819350df88a94bbcb2c4329e7fa1a9f2@muc.im.shiku.co/10007134" xmlns="jabber:client">
            presence.From = XmppCon.MyJID;//发送者
            presence.To = roomJid;
            Element x = new Element();
            x.TagName = "x";
            x.SetAttribute("xmlns", "'http://jabber.org/protocol/muc'");//{<presence xmlns="jabber:client" from="10009349@im.shiku.co/youjob" to="2402950a7a7349a697a6bff7002cf725@muc.im.shiku.co/10009349"><x xmlns="'http://jabber.org/protocol/muc'" /></presence>}
            presence.AddChild(x);
            #endregion
            XmppCon.Send(presence);
        }
        #endregion

        #region 初始配置房间
        /// <summary>
        /// 初始配置房间
        /// </summary>
        /// <param name="roomJid"></param>
        private void ConfigRoomByIQ(Room room, Jid roomJid)
        {
            #region 原来的配置
            /*
            ////配置
            Field type = new Field();
            type.Var = "FORM_TYPE";
            type.Value = "http://jabber.org/protocol/muc#roomconfig";
            //名称
            Field name = new Field(FieldType.Text_Single);
            name.Var = "muc#roomconfig_roomname";
            name.Value = room.name;
            //描述
            Field desc = new Field(FieldType.Text_Single);
            desc.Var = "muc#roomconfig_roomdesc";
            desc.Value = room.desc;
            //是否永久
            Field persistent = new Field(FieldType.Boolean);
            persistent.Var = "muc#roomconfig_persistentroom";
            persistent.Value = "1";
            //是否为公开群
            Field ispublic = new Field(FieldType.Boolean);
            ispublic.Var = "muc#roomconfig_publicroom";
            ispublic.Value = "1";

            agsXMPP.protocol.x.muc.iq.owner.OwnerIq configIq = new agsXMPP.protocol.x.muc.iq.owner.OwnerIq(IqType.set, roomJid);//发送RoomJid
            configIq.Query.AddChild(new Data(XDataFormType.submit));
            //configIq.Query.AddChild(type);//表单类型
            configIq.Query.AddChild(name);//房间名称
            configIq.Query.AddChild(desc);//房间描述
            configIq.Query.AddChild(persistent);//是否为永久群聊
            configIq.Query.AddChild(ispublic);//是否为公开群聊
            */
            #endregion
            string tmpConfig = "<iq id=\"" + Guid.NewGuid().ToString("N") + "\"" +
                                            " to=\"" + roomJid.User + "@muc." + Applicate.URLDATA.data.XMPPDomain + "\" " +
                                            "type=\"set\">" +
                                                "<query xmlns=\"http://jabber.org/protocol/muc#owner\">" +
                                                "<x xmlns='jabber:x:data' type=\"submit\">" +
                                                "<field var=\"muc#roomconfig_roomname\" type=\"text-single\">" +
                                                    "<value>" + room.name + "</value>" +
                                                "</field>" +
                                                "<field var=\"muc#roomconfig_roomdesc\" type=\"text-single\">" +
                                                    "<value>" + room.desc + "</value>" +
                                                "</field><field var=\"muc#roomconfig_persistentroom\" type=\"boolean\">" +
                                                    "<value>1</value>" +
                                                "</field>" +
                                                "<field var=\"muc#roomconfig_publicroom\" type=\"boolean\">" +
                                                    "<value>1</value>" +
                                                "</field>" +
                                                "</x>" +
                                                "</query>" +
                                            "</iq>";
            XmppCon.Send(tmpConfig);
        }
        #endregion
    }

}