using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using ShikuIM.Model;
using ShikuIM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShikuIM
{
    /// <summary>
    /// 此类是关于视酷IM的一系列业务操作
    /// </summary>
    internal static class ShiKuManager
    {

        /// <summary>
        /// Xmpp管理类
        /// </summary>
        internal static XmppManager xmpp;

        #region 初始化XMPP连接
        /// <summary>
        /// 初始化XMPP连接对象
        /// </summary>
        /// <param name="curruser">当前的用户对象</param>
        /// <param name="Pwd">用户密码</param>
        private static void InitialXmpp(Jsonuser curruser, string Pwd)
        {
            try
            {
                //实例化XmppManager类
                xmpp = new XmppManager();
                #region 设置Xmpp连接属性
                xmpp.XmppCon.ConnectServer = Applicate.URLDATA.data.XMPPHost;
                xmpp.XmppCon.Server = Applicate.URLDATA.data.XMPPDomain;//XMPPDomain;
                xmpp.XmppCon.Username = curruser.data.userId;
                xmpp.XmppCon.Password = Pwd;
                xmpp.XmppCon.Resource = "pc";
                xmpp.XmppCon.UseCompression = false;//禁用压缩
                //xmpp.XmppCon.UseSso = true;//设置单点登录
                //xmpp.XmppCon.UseSSL = false;//不使用SSL加密
                //xmpp.XmppCon.AutoRoster = false;//无名册功能
                //xmpp.XmppCon.AutoPresence = true;//自动发送在线
                xmpp.XmppCon.AutoResolveConnectServer = true;//设置自动解析
                xmpp.XmppCon.EnableCapabilities = true;
                xmpp.XmppCon.ClientVersion = "1.0";
                xmpp.XmppCon.RegisterAccount = false;
                xmpp.XmppCon.UseStartTLS = false;
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("接口登录完成后，打开Xmpp时出现错误：" + ex.Message);
            }
        }
        #endregion

        #region Http接口访问

        #region 获取服务器配置
        /// <summary>
        /// 获取服务器配置
        /// </summary>
        internal static void GetConfigAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    var client = APIHelper.GetFullConfig();//获取所有配置信息
                    if (client != null)
                    {
                        client.DownloadDataCompleted += (sen, eve) =>
                        {
                            if (eve.Error != null)
                            {
                                Messenger.Default.Send(eve.Error.Message, LoginAndRegisterViewModel.ErrorMessage);
                            }
                        };
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send(ex.Message, LoginAndRegisterViewModel.ErrorMessage);
                }
            });
        }
        #endregion

        #region 视酷用户登录
        /// <summary>
        /// 用户登录 (这里是http接口登录，接口登录完成之后我们再走xmpp登录)
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="password"></param>
        /// <param name="latitude">维度</param>
        /// <param name="longitude">经度</param>
        /// <param name="isSavepassword">是否保存密码</param>
        internal static HttpClient ShiKuLogin(string phonenumber, string password, string latitude, string longitude, string areaCode)
        {
            //接收登录返回的User对象
            HttpClient client;
            client = APIHelper.UserLogin(phonenumber, password, latitude, longitude, areaCode);
            client.Tag = Helpers.MD5create(password);//指定自定义变量(密码)
            client.Tag2 = phonenumber;//(电话号码)
            client.Tag3 = password;//密码
            client.UploadDataCompleted += LoginComplete;//这个是回调函数，打开xmpp就在回调里面执行
            return client;
        }
        #endregion

        #region 登录完成
        /// <summary>
        /// 登录完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void LoginComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)//正常清空
            {
                string res = Encoding.UTF8.GetString(e.Result);
                var curruser = JsonConvert.DeserializeObject<Jsonuser>(res);
                var client = ((HttpClient)sender);
                string password = client.Tag as string;//获取密码
                string telephone = client.Tag2 as string;//获取手机号
                FileUtil.DeleteUserHeadImg(curruser.data.userId);//异步删除头像
                //如果ResultCode为1的话，就登录Xmpp，并显示主窗口
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (curruser != null && curruser.resultCode == 1)
                    {
                        Applicate.Access_Token = curruser.data.access_token;//赋值令牌
                        Applicate.IsAccountVerified = true;//记录为已登录
                        Applicate.MyAccount = new DataOfUserDetial()
                        {
                            userId = curruser.data.userId,
                            nickname = curruser.data.nickname,
                            Telephone = telephone,
                        };//赋值全局变量
                        Applicate.MyAccount.Telephone = Applicate.MyAccount.Telephone.Substring(2);
                        Applicate.LoginTime = Helpers.DatetimeToStamp(DateTime.Now);//记录登录时间
                        Application.Current.MainWindow = new MainWindow();//初始化主窗口
                        Application.Current.MainWindow.Show();//显示主窗口
                        Messenger.Default.Send(Applicate.MyAccount, CommonNotifications.UpdateMyAccountDetail);
                        //根据返回值加载联系人列表
                        if (curruser.data.login.isupdate == 1)//如果有更新则覆盖本地获取
                        {
                            Messenger.Default.Send(true, MainViewNotifactions.MainViewLoadFriendList);
                            Messenger.Default.Send(true, MainViewNotifactions.MainViewLoadBlockList);
                            Messenger.Default.Send(true, MainViewNotifactions.MainViewLoadGroupList);
                        }
                        else
                        {
                            Messenger.Default.Send(false, MainViewNotifactions.MainViewLoadFriendList);
                            Messenger.Default.Send(false, MainViewNotifactions.MainViewLoadBlockList);
                            Messenger.Default.Send(false, MainViewNotifactions.MainViewLoadGroupList);
                        }
                        Task.Run(() =>
                        {
                            new LocalUser().UpdateLastExitTime(curruser.data.userId, curruser.data.login.offlineTime);//保存离线时间
                            InitialXmpp(curruser, password);//初始化Xmpp连接对象
                            xmpp.XmppCon.Open();//打开Xmpp连接
                            ShiKuManager.GetLastChatList(curruser.data.login.offlineTime.ToString());//
                            ServiceLocator.Current.GetInstance<UserDetailViewModel>();//n
                            ServiceLocator.Current.GetInstance<MyDetialViewModel>();
                            ServiceLocator.Current.GetInstance<GroupCreateViewModel>();
                            ServiceLocator.Current.GetInstance<GroupDetialViewModel>();
                            ServiceLocator.Current.GetInstance<UserVerifyListViewModel>();
                            ServiceLocator.Current.GetInstance<AccountQueryViewModel>();
                            ServiceLocator.Current.GetInstance<ChatHistoryViewModel>();
                            ServiceLocator.Current.GetInstance<RoomVerifyViewModel>();
                            ServiceLocator.Current.GetInstance<SettingViewModel>();
                            ServiceLocator.Current.GetInstance<ChatBubbleListViewModel>();
                            //ServiceLocator.Current.GetInstance<ViewModel>();
                        });
                    }
                });
            }
            else
            {
                //登录失败
            }
        }
        #endregion

        #region 视酷用户登录
        /// <summary>
        /// 用户登录 (这里是http接口登录，接口登录完成之后我们再走xmpp登录)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="latitude">维度</param>
        /// <param name="longitude">经度</param>
        internal static HttpClient ShiKuVisitorLogin(string key, string latitude, string longitude)
        {
            //接收登录返回的User对象
            HttpClient client;
            client = APIHelper.VisitorLogin(key, latitude, longitude);
            client.Tag = "";//
            client.Tag2 = "";//
            client.Tag3 = "";//
            client.UploadDataCompleted += VisitorLoginComplete;//这个是回调函数，打开xmpp就在回调里面执行
            return client;
        }
        #endregion

        #region 游客登录完成
        /// <summary>
        /// 游客登录完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void VisitorLoginComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)//正常清空
            {
                string res = Encoding.UTF8.GetString(e.Result);
                var curruser = JsonConvert.DeserializeObject<Jsonuser>(res);
                var client = ((HttpClient)sender);
                //string password = client.Tag as string;//获取密码
                string password = curruser.data.passWord;
                string telephone = client.Tag2 as string;//获取手机号
                FileUtil.DeleteUserHeadImg(curruser.data.userId);//异步删除头像
                //如果ResultCode为1的话，就登录Xmpp，并显示主窗口
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (curruser != null && curruser.resultCode == 1)
                    {
                        Applicate.Access_Token = curruser.data.access_token;//赋值令牌
                        Applicate.IsAccountVerified = true;//记录为已登录
                        Applicate.MyAccount = new DataOfUserDetial()
                        {
                            userId = curruser.data.userId,
                            nickname = curruser.data.nickname,
                            Telephone = telephone,
                        };//赋值全局变量
                        Applicate.MyAccount.Telephone = "";
                        Applicate.LoginTime = Helpers.DatetimeToStamp(DateTime.Now);//记录登录时间
                        Application.Current.MainWindow = new MainWindow();//初始化主窗口
                        Application.Current.MainWindow.Show();//显示主窗口
                        Messenger.Default.Send(Applicate.MyAccount, CommonNotifications.UpdateMyAccountDetail);
                        //根据返回值加载联系人列表
                        if (curruser.data.login.isupdate == 1)//如果有更新则覆盖本地获取
                        {
                            Messenger.Default.Send(true, MainViewNotifactions.MainViewLoadFriendList);
                            Messenger.Default.Send(true, MainViewNotifactions.MainViewLoadBlockList);
                            Messenger.Default.Send(true, MainViewNotifactions.MainViewLoadGroupList);
                        }
                        else
                        {
                            Messenger.Default.Send(false, MainViewNotifactions.MainViewLoadFriendList);
                            Messenger.Default.Send(false, MainViewNotifactions.MainViewLoadBlockList);
                            Messenger.Default.Send(false, MainViewNotifactions.MainViewLoadGroupList);
                        }
                        Task.Run(() =>
                        {
                            new LocalUser().UpdateLastExitTime(curruser.data.userId, curruser.data.login.offlineTime);//保存离线时间
                            InitialXmpp(curruser, password);//初始化Xmpp连接对象
                            xmpp.XmppCon.Open();//打开Xmpp连接
                            ShiKuManager.GetLastChatList(curruser.data.login.offlineTime.ToString());//
                            ServiceLocator.Current.GetInstance<UserDetailViewModel>();//n
                            ServiceLocator.Current.GetInstance<MyDetialViewModel>();
                            ServiceLocator.Current.GetInstance<GroupCreateViewModel>();
                            ServiceLocator.Current.GetInstance<GroupDetialViewModel>();
                            ServiceLocator.Current.GetInstance<UserVerifyListViewModel>();
                            ServiceLocator.Current.GetInstance<AccountQueryViewModel>();
                            ServiceLocator.Current.GetInstance<ChatHistoryViewModel>();
                            ServiceLocator.Current.GetInstance<RoomVerifyViewModel>();
                            ServiceLocator.Current.GetInstance<SettingViewModel>();
                            ServiceLocator.Current.GetInstance<ChatBubbleListViewModel>();
                            //ServiceLocator.Current.GetInstance<ViewModel>();
                        });
                    }
                });
            }
            else
            {
                //登录失败
            }
        }
        #endregion

        #region 获取上次的聊天列表
        /// <summary>
        /// 获取上次的聊天列表
        /// </summary>
        /// <param name="lastTimeLen"></param>
        /// <returns></returns>
        public static HttpClient GetLastChatList(string lastTimeLen)
        {
            var client = APIHelper.GetLastChatListAsync(lastTimeLen);
            client.UploadDataCompleted += (sen, eve) =>
            {
                if (eve.Error != null)
                {
                    var restxt = Encoding.UTF8.GetString(eve.Result);//
                }
            };
            return client;
        }
        #endregion

        #region 上传头像
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="filePath">头像路径</param>
        /// <returns></returns>
        internal static WebClient UploadAvator(string userId, string filePath)
        {
            string url = Applicate.URLDATA.data.uploadUrl + "upload/UploadifyAvatarServlet";
            // 读文件流
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //这部分需要完善
            string ContentType = "application/octet-stream";
            byte[] fileBytes = new byte[fs.Length];
            fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));
            // 生成需要上传的二进制数组
            CreateBytes cb = new CreateBytes();
            // 所有表单数据
            ArrayList bytesArray = new ArrayList
            {
                // 普通表单
                cb.CreateFieldData("userId", userId),
                // 文件表单
                cb.CreateFieldData(userId, filePath, ContentType, fileBytes)
            };
            // 合成所有表单并生成二进制数组
            byte[] bytes = cb.JoinBytes(bytesArray);
            // 上传到指定Url
            var client = APIHelper.UploadAvatorAsync(url, bytes);
            fs.Close();//释放资源
            fs.Dispose();
            return client;
        }
        #endregion

        #region 撤回消息
        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="userId">对应的用户</param>
        /// <param name="messageId">需要撤回的消息Id</param>
        internal static HttpClient WithDrawMsg(string userId, string messageId, int isGroup)
        {
            var client = APIHelper.DeleteMessageAsync(messageId, isGroup, 2);
            client.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    //先使用接口
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                    #region 发送消息
                    Messageobject msg = new Messageobject();
                    msg.fromUserId = Applicate.MyAccount.userId;
                    msg.FromId = Applicate.MyAccount.userId;
                    msg.fromUserName = Applicate.MyAccount.nickname;//这里放自己的昵称
                    msg.toUserId = userId;
                    msg.ToId = userId;
                    msg.isGroup = (userId.Length > 15) ? (1) : (0);//是否为群消息
                    msg.content = messageId;//撤回的消息ID
                    msg.type = kWCMessageType.Withdraw;//消息类型为撤回消息
                    msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//当前的时间戳
                    msg.messageId = Guid.NewGuid().ToString("N");//协议信息MessageId
                    Applicate.WithDrawMessageList.Add(msg);
                    xmpp.SendJsonMsg(msg, userId);//指定发送的UserId
                    #endregion
                }
                else
                {


                }
            };
            return client;
        }
        #endregion

        #region 同意好友聊天请求
        /// <summary>
        /// 1=关注成功，已关注目标用户
        /// 2=关注成功，已互为好友
        /// 3=关注失败，已关注目标用户
        /// 4=关注失败，已互为好友
        /// 1100801=关注失败，目标用户拒绝关注
        /// 1100802=关注失败，目标用户将你拉黑
        /// </summary>
        /// <param name="reqUser">对应的用户</param>
        /// <returns>返回的结果</returns>
        internal static HttpClient AgreeFriendReq(DataOfFriends reqUser)
        {
            //接口关注
            return APIHelper.AttentionAddAsync(reqUser.toUserId);
        }
        #endregion

        #region 发送"通过验证"消息
        /// <summary>
        /// "通过验证"Xmpp消息
        /// </summary>
        /// <param name="target" >目标用户</param>
        public static void SendVerifyAgreeXmpp(DataOfFriends target)
        {
            //Xmpp发送消息
            Messageobject agreeMsg = new Messageobject()
            {
                fromUserId = Applicate.MyAccount.userId,
                toUserId = target.toUserId
            };
            agreeMsg.messageId = Guid.NewGuid().ToString("N");//MessageId
            agreeMsg.toUserName = target.toNickname;//对方名称
            agreeMsg.fromUserName = Applicate.MyAccount.nickname;//发送方Id
            agreeMsg.type = kWCMessageType.RequestAgree;//同意添加好友
            agreeMsg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            xmpp.SendJsonMsg(agreeMsg, target.toUserId);//发送
            return;
        }
        #endregion

        #region 删除好友消息推送
        /// <summary>
        /// 删除好友消息推送
        /// </summary>
        /// <param name="item"></param>
        internal static void SendDeleteFriend(MessageListItem item)
        {
            Messageobject agreeMsg = new Messageobject();
            agreeMsg.messageId = Guid.NewGuid().ToString("N");
            agreeMsg.ToId = item.Jid;
            agreeMsg.toUserId = item.Jid;//对方UserId
            agreeMsg.FromId = Applicate.MyAccount.userId;
            agreeMsg.fromUserId = Applicate.MyAccount.userId;
            agreeMsg.fromUserName = Applicate.MyAccount.nickname;//这里放自己的昵称
            agreeMsg.type = kWCMessageType.DeleteFriend;//消息类型为删除好友
            agreeMsg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//当前的时间戳
            agreeMsg.toUserName = item.ShowTitle;//对方昵称
            xmpp.SendJsonMsg(agreeMsg, item.Jid);//指定发送的UserId
        }
        #endregion

        #region 主动加入群聊(接口)
        /// <summary>
        /// 主动加入群聊(接口)
        /// </summary>
        /// <param name="roomId">对应的RoomId,非Jid</param>
        /// <returns></returns>
        internal static void JoinGroup(string roomId, string roomJid)
        {
            var lastExitTime = new LocalUser().GetLastExitTime(Applicate.MyAccount.userId);
            var client = APIHelper.GroupJoinAsync(roomId);
            client.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                    if (result != null && result.resultCode == 1)
                    {
                        //Send XmppMsg to Join the target group
                        xmpp.JoinGroup(roomJid, lastExitTime);
                        //When join the target group successed, Reload RoomList by API
                        ServiceLocator.Current.GetInstance<MainViewModel>().LoadJoinedRoomByApi();
                    }
                }
            };
        }
        #endregion

        #region 无验证添加好友
        /// <summary>
        /// 无验证添加好友
        /// <para>
        /// 已添加开始聊天等处理
        /// </para>
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>接口添加好友返回的数据</returns>
        internal static HttpClient AddFriend(DataOfUserDetial user)
        {
            //接口添加好友
            var client = APIHelper.AddFriendAsync(user.userId);
            client.Tag = user;//自定义变量
            client.UploadDataCompleted += AddFriendComplete;//回调
            return client;
        }
        #endregion

        #region 删除群聊
        /// <summary>
        /// 删除群聊
        /// </summary>
        /// <param name="id">群聊ID</param>
        /// <returns>是否成功</returns>
        internal static HttpClient deleteRoom(string id)
        {
            return APIHelper.DeleteRoomAsync(id);
        }
        #endregion

        #region 获取好友在线状态
        /// <summary>
        /// 获取好友在线状态
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>返回</returns>
        public static HttpClient GetFriendState(string userId)
        {
            try
            {
                //声明接口URL
                string url = Applicate.URLDATA.data.apiUrl + "user/getOnLine";
                //定义参数
                string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId;
                var parabyte = Encoding.UTF8.GetBytes(para);
                var client = new HttpClient();
                client.Headers.Add("ContentLength", parabyte.Length.ToString());
                client.UploadDataAsync(new Uri(url), parabyte);
                //JsonConvert.DeserializeObject<rtnUserState>(tmp, typeof(rtnUserState));
                return client;
            }
            catch (Exception e)
            {
                LogHelper.log.Error(e.Message, e);
                ConsoleLog.Output(e.Message);
                return null;
            }
        }
        #endregion

        #region 退出群聊
        /// <summary>
        /// 退出群聊
        /// </summary> 
        /// <param name="room"></param>
        /// <returns></returns>
        internal static HttpClient leaveRoom(Room room)
        {
            //接口退群
            var client = APIHelper.ExitRoomAsync(room.id);
            client.Tag = room;
            client.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    var res = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                    var rdetail = ((HttpClient)sender).Tag as Room;//
                    if (res.resultCode == 1)
                    {
                        //Xmpp退群(在接口退群成功之后)
                        xmpp.exitRoom(rdetail.jid);
                    }
                    //("退出群 " + detial.data.nickname + " 成功", "系统提示");
                    else
                    {
                        ConsoleLog.Output("退群失败");
                    }
                    //("退出群 " + detial.data.nickname + " 失败,请重试", "系统提示");
                    //return 0;
                }
            };//回调发送退群
            return client;
        }
        #endregion

        #region 退出群聊
        /// <summary>
        /// 退出群聊
        /// </summary> 
        /// <param name="room">id、nickname、jid</param>
        /// <returns></returns>
        internal static HttpClient LeaveRoom(MessageListItem room)
        {
            //接口退群
            var client = APIHelper.ExitRoomAsync(room.Id);
            client.Tag = room;
            client.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    string restext = Encoding.UTF8.GetString(e.Result);
                    var res = JsonConvert.DeserializeObject<JsonBase>(restext);
                    var rroom = ((HttpClient)sender).Tag as MessageListItem;//从Tag获取房间信息
                    if (res.resultCode == 1)
                    {
                        //Xmpp退群(在接口退群成功之后)
                        xmpp.exitRoom(rroom.Jid);
                        //new Room().UpdateJoinState(rroom.id, false);//更新为已退出群
                        //var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                        //mControl.LoadMyRoomsByDb();
                        //ServiceLocator.Current.GetInstance<MainViewModel>().Toast("退出群 " + rroom.data.name + " 成功");
                        //("退出群 " + detial.data.nickname + " 成功", "系统提示");
                    }
                    else
                    {
                        ConsoleLog.Output("退群失败");
                        //ServiceLocator.Current.GetInstance<MainViewModel>().Snackbar.Enqueue("退出群 " + rroom.name + " 失败，请重试");
                        //return 0;
                    }
                }
            };//回调发送退群
            return client;
        }
        #endregion

        #region 创建群组
        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="room">房间</param>
        /// <param name="members">成员(包括创建者)</param>
        /// <returns>返回处理结果</returns>
        internal static HttpClient CreateGroup(Room room, List<string> members, Panel parent = null)
        {
            xmpp.xCreateGroup(room, members);//Xmpp创建
            HttpClient client = null;
            client = APIHelper.CreateGroupWithMembersAsync(room, members);
            client.UploadDataCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    var res = Encoding.UTF8.GetString(e.Result);
                    var jsonres = JsonConvert.DeserializeObject<JsonRoom>(res);
                    if (jsonres.data.members == null || jsonres.data.members.Count == 0)
                    {
                        var troom = ((HttpClient)s).Tag as Room;////获取群组
                        troom.members[0].AutoInsertRange(troom.members, jsonres.data.id);
                        jsonres.data.userSize = troom.userSize;//成员数量
                    }
                    var group = jsonres.data;
                    group.AutoInsert();
                    new DataofMember().AutoInsertRange(group.members, group.id);
                }
            };
            return client;
        }
        #endregion

        #region 建群成功后
        /// <summary>
        /// 建群成功后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CreateSingleMemberGroupComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var tmpstr = Encoding.UTF8.GetString(e.Result);
                var tmpRes = JsonConvert.DeserializeObject<JsonBase>(tmpstr);
                if (tmpRes.resultCode == 1)
                {
                    var newRoom = JsonConvert.DeserializeObject<Room>(tmpstr);
                    var group = ((HttpClient)sender).Tag as Room;//获取群详情
                    Task.Run(() =>
                    {
                        group.AutoInsert();
                        new DataofMember().AutoInsertRange(group.members, group.id);
                        //mControl.LoadAllRoomsByDb();
                    });
                    var member = ((HttpClient)sender).Tag2 as List<string>;//获取群成员
                                                                           //Task.Delay(300);
                    var client = APIHelper.UpdateCreateGroupAsync(group.id, member);//邀请成员
                    var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                    client.Tag = group;//设置群详情
                    client.UploadDataCompleted += (s, res) =>
                    {
                        string rtnString = Encoding.UTF8.GetString(res.Result);
                        var room = ((HttpClient)s).Tag as Room;
                        var result = JsonConvert.DeserializeObject<JsonBase>(rtnString);
                        //建群成功后刷新群列表
                        if (result.resultCode == 1)
                        {
                            //此处应该点击群聊项然后进群聊天
                            mControl.StartNewChatFromItem(new MessageListItem
                            {
                                Jid = group.jid,
                                MessageTitle = group.name,
                                ShowTitle = group.name,
                                MessageItemType = ItemType.Message,
                                Id = group.id,
                                Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(group.jid)
                            });//跳转到群聊聊天界面
                            mControl.Snackbar.Enqueue("创建群聊成功！");
                            //var item = mControl.MyGroupList.FirstOrDefault(m => m.Id == room.id);
                        }
                    };
                }
            }
        }
        #endregion

        #region 邀请群成员
        /// <summary>
        /// 邀请群成员
        /// </summary>
        /// <param name="roomJid"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        internal static HttpClient InviteGroup(string roomid, List<string> members)
        {
            var client = APIHelper.UpdateCreateGroupAsync(roomid, members);//二次邀请成员
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var restxt = Encoding.UTF8.GetString(res.Result);
                    var result = JsonConvert.DeserializeObject<JsonBase>(restxt);
                    if (result.resultCode == 1)
                    {

                    }
                }
            };
            return client;
        }
        #endregion

        #region 邀请群成员
        /// <summary>
        /// 邀请群成员
        /// </summary>
        /// <param name="roomJid"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        internal static HttpClient InviteGroup(string roomid, List<string> members, Panel parent)
        {
            JsonRoom newRoom = new JsonRoom();
            var friends = new DataOfFriends().GetFriendsList();
            var rooms = new Room().GetJoinedList();
            var rom = rooms.FirstOrDefault(d => d.id == roomid);
            if (rom != null)
            {
                newRoom.data.id = rom.id;
                newRoom.data.name = rom.name;
                var client = APIHelper.UpdateCreateGroupAsync(rom.id, members);//二次邀请成员
                client.UploadDataCompleted += (sen, res) =>
                {
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                    if (result.resultCode == 1)
                    {
                        mControl.Snackbar.Enqueue("邀请进群成功");
                    }
                    else
                    {
                        mControl.Snackbar.Enqueue("邀请进群失败!\n" + result.resultMsg ?? "");
                    }
                };
                return client;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 添加好友完成时
        /// <summary>
        /// 添加好友完成时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AddFriendComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string resstr = Encoding.UTF8.GetString(e.Result);
                var result = JsonConvert.DeserializeObject<JsonAttention>(resstr);//获取返回值
                var user = ((HttpClient)sender).Tag as DataOfUserDetial;
                if (result.resultCode == 1)
                {
                    var verifyModel = ServiceLocator.Current.GetInstance<UserVerifyListViewModel>();
                    var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                    if (mControl.RecentMessageList.Count(m => m.Jid == "10001") == 0)
                    {
                        mControl.RecentMessageList.Add(new MessageListItem()
                        {
                            MessageItemType = ItemType.VerifyMsg,
                            MessageItemContent = "",
                            Jid = "10001",
                            MessageTitle = "新的朋友",
                            ShowTitle = "新的朋友",
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath("10001")
                        });
                    }

                    if (result.data.type == 2 || result.data.type == 4)//对方并没有验证直接聊天
                    {
                        ShiKuManager.SendFriendRequest(user.userId, true);//发送添加好友消息
                        verifyModel.AddOrUpdateToList(new VerifingFriend()
                        {
                            toNickname = user.nickname,
                            toUserId = user.userId,
                            VerifyStatus = 1,
                            Content = "你们已成为好友"
                        });
                        var dbfriend = new DataOfFriends();
                        bool isExists = dbfriend.ExistsFriend(user.userId);
                        if (isExists)//是否存在本地好友
                        {
                            var item = new MessageListItem { Jid = user.userId, ShowTitle = user.nickname, Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(user.userId) };
                            Messenger.Default.Send(item, MainViewNotifactions.MainInsertRecentItem);//添加聊天项
                            Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//聊天
                            var fItem = dbfriend.GetByUserId(user.userId).ToMsgListItem();
                            Messenger.Default.Send(fItem, MainViewNotifactions.MainAddFriendListItem);//添加好友项
                        }
                        else//本地好友不存在
                        {
                            APIHelper.GetUserDetialAsync(user.userId);
                        }
                    }
                    else
                    {
                        ShiKuManager.SendFriendRequest(user.userId, false);//发送打招呼消息
                        verifyModel.AddOrUpdateToList(new VerifingFriend()
                        {
                            toNickname = user.nickname,
                            toUserId = user.userId,
                            VerifyStatus = -3,
                            Content = "等待验证中..."
                        });
                    }
                }
            }
            else
            {
                //失败
            }
        }
        #endregion

        #endregion

        #region Xmpp消息发送


        #region 初始化平台设备列表
        /// <summary>
        /// 初始化平台设备列表
        /// </summary>
        public static void InitialPlatformList()
        {
            if (Applicate.PlatformNames.Length != 0)//如果全局无设备列表不操作
            {
                var tmpplats = PlatformService.GetInstance();//获取设备
                for (int i = 0; i < Applicate.PlatformNames.Length; i++)
                {
                    var tmptmer = new PlatformTimer { Interval = 10000, PlatformName = Applicate.PlatformNames[i] };//5分钟重发在线消息确认(暂设5秒)
                    tmptmer.Elapsed += (sen, eve) =>
                    {
                        var tmp = (PlatformTimer)sen;
                        SendCheckPlatformOnline(tmp.PlatformName);
                    };
                    tmptmer.Start();//开始检测
                    //SendCheckPlatformOnline(Applicate.PlatformNames[i]);
                    tmpplats.CreateNewPlatform(Applicate.PlatformNames[i], tmptmer);
                }
            }
        }
        #endregion

        #region 发送检查多平台设备在线消息
        /// <summary>
        /// 发送检查多平台设备在线消息
        /// </summary>
        /// <param name="resource">平台名称</param>
        /// <param name="isOnline">是否在线</param>
        public static void SendCheckPlatformOnline(string resource, string isOnline = "1")
        {
            var msg = new Messageobject
            {
                content = isOnline,
                fromUserId = Applicate.MyAccount.userId,
                toUserId = Applicate.MyAccount.userId,
                type = kWCMessageType.OnlineStatus,
            };
            if (xmpp == null)
            {
                return;
            }
            var xmsg = new agsXMPP.protocol.client.Message
            {
                Body = msg.ToJson(),
                To = new agsXMPP.Jid(xmpp.XmppCon.MyJID.User, xmpp.XmppCon.MyJID.Server, resource),
                From = new agsXMPP.Jid(xmpp.XmppCon.MyJID.User, xmpp.XmppCon.MyJID.Server, "pc"),
                Id = Guid.NewGuid().ToString("N"),
                Type = agsXMPP.protocol.client.MessageType.chat,
            };
            #region 请求发送送达回执到本机
            //如果非群聊需要请求对方发送回执
            agsXMPP.Xml.Dom.Element reqRec = new agsXMPP.Xml.Dom.Element("request");//请求接送方发送消息回执
            reqRec.SetAttribute("xmlns", "urn:xmpp:receipts");//设置回执属性
            xmsg.AddChild(reqRec);//添加请求回执的节点
            xmsg.SetAttribute("xmlns", "jabber:client");
            #endregion
            xmpp.XmppCon.Send(xmsg);
        }
        #endregion

        #region 发送正在通话消息类型(通话心跳包)
        /// <summary>
        /// 发送正在通话消息类型(通话心跳包)
        /// </summary>
        /// <param name="jid">对方UserId</param>
        /// <param name="toNickname">对方昵称</param>
        public static void SendPhoneCallingAlive(string jid, string toNickname)
        {
            var msg = new Messageobject
            {
                ToId = jid,
                FromId = Applicate.MyAccount.userId,
                fromUserId = Applicate.MyAccount.userId,
                toUserId = jid,
                fromUserName = Applicate.MyAccount.nickname,
                toUserName = toNickname,
                timeSend = Helpers.DatetimeToStamp(DateTime.Now),
                messageId = Guid.NewGuid().ToString("N"),
                type = kWCMessageType.PhoneCalling
            };
            xmpp.SendJsonMsg(msg);
        }
        #endregion

        #region 好友验证时回话
        /// <summary>
        /// 好友验证时回话
        /// </summary>
        /// <param name="target">需回话的UserId</param>
        internal static void ReAnswerBack(string content, MessageListItem target)
        {
            if (target == null || target.Jid == "")
            {
                return;
            }
            Messageobject jsonMsg = new Messageobject();
            jsonMsg.FromId = jsonMsg.fromUserId;
            jsonMsg.ToId = jsonMsg.toUserId;
            jsonMsg.fromUserId = Applicate.MyAccount.userId;//UserId
            jsonMsg.fromUserName = Applicate.MyAccount.nickname;//自己的昵称
            jsonMsg.toUserId = target.Jid;//指定接收者
            jsonMsg.toUserName = target.ShowTitle;//接收者名称
            jsonMsg.content = content;//内容
            jsonMsg.type = kWCMessageType.RequestRefuse;//回话消息
            jsonMsg.messageId = Guid.NewGuid().ToString("N");//MessageId
            jsonMsg.isDelay = 0;//是否延期
            jsonMsg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送消息时间
            jsonMsg.Insert();//存入数据库
            xmpp.SendJsonMsg(jsonMsg);
        }
        #endregion

        #region 发送打招呼信息
        /// <summary>
        /// 发送打招呼信息
        /// </summary>
        /// <param name="userId">接收者Id</param>
        /// <param name="isDirectly">ResultCode为2和4时, 发送508(直接添加好友)，1和3时, 发500()</param>
        internal static void SendFriendRequest(string userId, bool isDirectly)
        {
            Messageobject message = new Messageobject();
            message.FromId = message.fromUserId;
            message.ToId = userId;
            message.content = "你好";//打招呼内容
            message.fromUserId = Applicate.MyAccount.userId;//打招呼者
            message.fromUserName = Applicate.MyAccount.nickname;//打招呼者名称
            message.toUserId = userId;//接收者
            message.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            message.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            if (isDirectly)
            {
                message.type = kWCMessageType.RequestFriendDirectly;//对方关闭好友验证,直接添加好友
            }
            else
            {
                message.type = kWCMessageType.FriendRequest;//对方开启好友验证, 为打招呼消息
            }
            message.Insert();//存入数据库
            xmpp.SendJsonMsg(message);//发送Xmpp消息
        }
        #endregion

        #region 拉黑好友消息推送
        /// <summary>x
        /// 拉黑好友消息推送
        /// </summary>
        /// <param name="friendItem"></param>
        internal static void DefriendReq(MessageListItem friendItem)
        {
            Messageobject agreeMsg = new Messageobject();
            agreeMsg.ToId = friendItem.Jid;
            agreeMsg.FromId = Applicate.MyAccount.userId;
            agreeMsg.toUserId = friendItem.Jid;//接收者
            agreeMsg.toUserName = friendItem.ShowTitle;
            agreeMsg.fromUserId = Applicate.MyAccount.userId;
            agreeMsg.fromUserName = Applicate.MyAccount.nickname;//这里放自己的昵称
            agreeMsg.type = kWCMessageType.BlackFriend;//消息类型为删除好友
            agreeMsg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//当前的时间戳
            agreeMsg.messageId = Guid.NewGuid().ToString("N");//生成Guid
            xmpp.SendJsonMsg(agreeMsg, friendItem.Jid);//指定发送的UserId

        }
        #endregion

        #region 移出黑名单消息推送
        /// <summary>
        /// 取消黑名单Xmpp消息
        /// </summary>
        /// <param name="userId"></param>
        internal static void DeblackReq(string userId)
        {
            Messageobject msg = new Messageobject();
            msg.toUserId = userId;
            msg.messageId = Guid.NewGuid().ToString("N");//消息ID
            msg.ToId = userId;
            msg.FromId = Applicate.MyAccount.userId;
            msg.fromUserId = Applicate.MyAccount.userId;
            msg.fromUserName = Applicate.MyAccount.nickname;//这里放自己的昵称
            msg.type = kWCMessageType.CancelBlackFriend;//消息类型为取消黑名单
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//当前的时间戳
            xmpp.SendJsonMsg(msg, userId);//指定发送的UserId
        }
        #endregion

        #region 取消黑名单
        /// <summary>
        /// 取消黑名单
        /// </summary>
        public static HttpClient CancelBlockfriend(string userId)
        {
            var client = APIHelper.CancelBlockfriendAsync(userId);
            client.UploadDataCompleted += (sen, eve) =>
            {
                if (eve.Error == null)
                {
                    var restxt = Encoding.UTF8.GetString(eve.Result);
                    var resobj = JsonConvert.DeserializeObject<JsonBase>(restxt);
                    if (resobj.resultCode == 1)
                    {
                        var blackitem = ((HttpClient)sen).Tag as MessageListItem;
                        DeblackReq(blackitem.Jid);//发送消息
                        new DataOfFriends().UpdateFriendState(blackitem.Jid, 2);//数据库更新为好友
                    }
                }
            };
            return client;
        }
        #endregion

        #region 邀请进入群组消息推送
        /// <summary>
        /// 邀请进入群组消息推送
        /// </summary>
        /// <param name="reqUser"></param>
        /// <param name="room"></param>
        internal static void RoomInviteReq(JsonuserDetial reqUser, JsonRoom room)
        {
            Messageobject msg = new Messageobject();
            msg.messageId = Guid.NewGuid().ToString("N");
            msg.fromUserId = Applicate.MyAccount.userId;
            msg.fromUserName = Applicate.MyAccount.nickname;//这里放自己的昵称
            msg.type = kWCMessageType.RoomInvite;//消息类型为删除好友
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//当前的时间戳
            msg.fileSize = 0;//是否显示阅读人数，1:开启，0:关闭
            msg.objectId = room.data.jid;//房间ID
            msg.content = room.data.name;//房间名
            msg.toUserId = reqUser.data.userId;//接收人ID
            msg.toUserName = reqUser.data.nickname;//接收人昵称
            xmpp.SendJsonMsg(msg, reqUser.data.userId.ToString());//指定发送的UserId
        }
        #endregion

        #region 发送名片
        /// <summary>
        /// 发送名片
        /// </summary>
        /// <param name="target"></param>
        /// <param name="userArray"></param>
        internal static void SendContacts(MessageListItem target, List<MessageListItem> userArray)
        {
            Task.Run(() =>
            {
                var msg = new Messageobject();
                msg.type = kWCMessageType.Card;
                msg.toUserId = target.Jid;//接收者Id
                msg.ToId = target.Jid;
                msg.toUserName = target.ShowTitle;//接收者昵称
                msg.isGroup = (target.Jid.Length > 20) ? 1 : 0;//判断是否为群
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.FromId = Applicate.MyAccount.userId;
                msg.fromUserId = Applicate.MyAccount.userId;//发送者Id
                for (int i = 0; i < userArray.Count; i++)
                {
                    msg.messageId = Guid.NewGuid().ToString("N"); //MessageId
                    msg.content = userArray[i].MessageTitle;//内容为名片人昵称
                    msg.objectId = userArray[i].Jid;//名片人ID
                    msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//设置当前的时间
                    Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                    msg.Insert();//保存到数据库
                    xmpp.SendJsonMsg(msg);//发送消息
                }
            });
        }
        #endregion

        #region 发送@群成员消息
        /// <summary>
        /// 发送@群成员消息
        /// </summary>
        /// <param name="selectedtmps">选中需要@的群成员</param>
        internal static void SendAttentionMsg(List<MessageListItem> selectedtmps, MessageListItem target)
        {
            Task.Run(() =>
            {
                //封装Message对象
                Messageobject msg = new Messageobject
                {
                    //content = text, 
                    FromId = Applicate.MyAccount.userId,//发送方UserId
                    fromUserId = Applicate.MyAccount.userId,//发送方UserId
                    ToId = target.Jid,
                    toUserId = target.Jid,//接收方UserID
                    toUserName = target.ShowTitle,//接收方昵称
                };
                msg.isGroup = (target.Jid.Length > 10) ? (1) : (0);
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间(使用时间戳)
                msg.messageId = Guid.NewGuid().ToString("N");//设置Message唯一ID
                msg.type = kWCMessageType.Text;//发送信息1，文本类型
                for (int i = 0; i < selectedtmps.Count; i++)
                {
                    msg.objectId += selectedtmps[i].Jid + " ";
                    msg.content += "@" + selectedtmps[i].ShowTitle + " ";
                }
                Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                msg.Insert();//存库
                xmpp.SendJsonMsg(msg);
            });
        }
        #endregion

        #region 发送进群验证
        internal static void SendRoomVerify(string toUserId, RoomVerify roomVerify)
        {
            Messageobject msg = new Messageobject();
            msg.ToId = toUserId;
            msg.toUserId = toUserId;//接收者
            msg.FromId = Applicate.MyAccount.userId;
            msg.fromUserId = Applicate.MyAccount.userId;//发送者
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.RoomIsVerify;//
            msg.objectId = roomVerify.toJson();
            //msg.Insert();//存入数据库
            xmpp.SendJsonMsg(msg);
        }
        #endregion

        #region 发送询问是否准备好语音通话
        /// <summary>
        /// 发送询问是否准备好语音通话
        /// </summary>
        /// <param name="toUserId"></param>
        internal static void SendAudioChatAsk(string toUserId)
        {
            Messageobject msg = new Messageobject();
            //message.myUserId = Applicate.MyAccount.userId; 
            msg.ToId = toUserId;
            msg.toUserId = toUserId;//接收者
            msg.FromId = Applicate.MyAccount.userId;
            msg.fromUserId = Applicate.MyAccount.userId;//发送者
            msg.content = "邀请您语音通话";//打招呼内容
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.AudioChatAsk;//询问是否准备好语音通话
            msg.Insert();//存入数据库
            xmpp.SendJsonMsg(msg);
        }
        #endregion

        #region 发送接受语音通话
        /// <summary>
        /// 发送接受语音通话
        /// </summary>
        /// <param name="toUserId"></param>
        internal static void SendAudioChatAccept(string toUserId)
        {
            Messageobject message = new Messageobject();
            message.toUserId = toUserId;//接收者
            message.ToId = toUserId;
            message.fromUserId = Applicate.MyAccount.userId;//
            message.FromId = Applicate.MyAccount.userId;
            message.content = "接受语音通话";//打招呼内容
            message.fromUserName = Applicate.MyAccount.nickname;//
            message.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            message.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            message.type = kWCMessageType.AudioChatAccept;//接受语音通话
            message.Insert();
            xmpp.SendJsonMsg(message);
        }
        #endregion

        #region 发送拒绝语音通话 或 取消拔号
        /// <summary>
        /// 发送拒绝语音通话 或 取消拔号
        /// </summary>
        /// <param name="ToUserId"></param>
        internal static void SendAudioChatCancel(string ToUserId)
        {
            Messageobject msg = new Messageobject();
            msg.content = "取消语音通话";//打招呼内容
            msg.fromUserId = Applicate.MyAccount.userId;//
            msg.FromId = Applicate.MyAccount.userId;//
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.toUserId = ToUserId;//接收者
            msg.ToId = ToUserId;
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.AudioChatCancel;//拒绝语音通话 或 取消拔号
            msg.Insert();
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新

        }
        #endregion

        #region 发送结束语音通话
        /// <summary>
        /// 发送结束语音通话
        /// </summary>
        /// <param name="ToUserId"></param>
        internal static void SendAudioChatEnd(string ToUserId, long timeLen)
        {
            Messageobject msg = new Messageobject();
            msg.content = "结束语音通话";//打招呼内容
            msg.fromUserId = Applicate.MyAccount.userId;//
            msg.FromId = Applicate.MyAccount.userId;
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.toUserId = ToUserId;//接收者
            msg.ToId = ToUserId;
            msg.timeLen = (int)timeLen;//时长
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.AudioChatEnd;//结束语音通话
            msg.Insert();
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 发送询问是否准备好视频通话
        /// <summary>
        /// 发送询问是否准备好视频通话
        /// </summary>
        /// <param name="ToUserId"></param>
        internal static void SendVideoChatAsk(string ToUserId)
        {
            Messageobject message = new Messageobject();
            //message.myUserId = Applicate.MyAccount.userId;
            message.content = "邀请您视频通话";//打招呼内容
            message.toUserId = ToUserId;//接收者
            message.ToId = ToUserId;
            message.FromId = Applicate.MyAccount.userId;
            message.fromUserId = Applicate.MyAccount.userId;//
            message.fromUserName = Applicate.MyAccount.nickname;//
            message.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            message.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            message.type = kWCMessageType.VideoChatAsk;//询问是否准备好视频通话
            message.Insert();
            xmpp.SendJsonMsg(message);
        }
        #endregion

        #region 发送接受视频通话
        /// <summary>
        /// 发送接受视频通话
        /// </summary>
        /// <param name="toUserId"></param>
        internal static void SendVideoChatAccept(string toUserId)
        {
            Messageobject message = new Messageobject();
            message.content = "接受视频通话";//打招呼内容
            message.ToId = toUserId;
            message.toUserId = toUserId;//接收者
            message.FromId = Applicate.MyAccount.userId;//
            message.fromUserId = Applicate.MyAccount.userId;//
            message.fromUserName = Applicate.MyAccount.nickname;//
            message.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            message.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            message.type = kWCMessageType.VideoChatAccept;//接受视频通话
            message.Insert();
            xmpp.SendJsonMsg(message);
        }
        #endregion

        #region 发送拒绝视频通话 或 取消拔号
        /// <summary>
        /// 发送拒绝视频通话 或 取消拔号
        /// </summary>
        /// <param name="ToUserId"></param>
        internal static void SendVideoChatCancel(string ToUserId)
        {
            Messageobject msg = new Messageobject();
            msg.content = "取消视频通话";//打招呼内容
            msg.ToId = ToUserId;
            msg.toUserId = ToUserId;//接收者
            msg.FromId = Applicate.MyAccount.userId;//
            msg.fromUserId = Applicate.MyAccount.userId;//
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.VideoChatCancel;//拒绝视频通话 或 取消拔号
            msg.Insert();//存入数据库
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 发送结束视频通话
        /// <summary>
        /// 发送结束视频通话
        /// </summary>
        /// <param name="ToUserId"></param>
        internal static void SendVideoChatEnd(string ToUserId, long timeLen)
        {
            Messageobject msg = new Messageobject();
            //message.myUserId = Applicate.MyAccount.userId;
            msg.content = "结束视频通话";//打招呼内容
            msg.ToId = ToUserId;
            msg.FromId = Applicate.MyAccount.userId;//
            msg.fromUserId = Applicate.MyAccount.userId;//
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.toUserId = ToUserId;//接收者
            msg.timeLen = (int)timeLen;//时长
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.VideoChatEnd;//结束视频通话 
            msg.Insert();//存入数据库
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 发送询问视频会议
        /// <summary>
        /// 发送询问视频会议
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="roomJid"></param>
        /// <param name="ToUserId"></param>
        /// <param name="toNickname"></param>
        internal static void SendVideoMeetingAsk(string roomId, string roomJid, string ToUserId, string toNickname)
        {
            Messageobject msg = new Messageobject();
            msg.ToId = ToUserId;//接收者
            msg.toUserId = ToUserId;//接收者
            msg.FromId = Applicate.MyAccount.userId;//发起者
            msg.fromUserId = Applicate.MyAccount.userId;//接收者
            msg.fileName = roomJid;//呼叫号码
            msg.content = "邀请您视频会议";//打招呼内容
            msg.fromUserName = Applicate.MyAccount.nickname;//发送者
            msg.objectId = roomJid;//房间jid
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.VideoMeetingInvite;//询问是否准备好视频通话
            msg.toUserName = toNickname;
            msg.Insert();
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 发送询问音频会议
        /// <summary>
        /// 发送询问音频会议
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="ToUserId"></param>
        internal static void SendAudioMeetingAsk(string roomId, string roomJid, string ToUserId, string toNickname)
        {
            Messageobject msg = new Messageobject();
            msg.ToId = ToUserId;//接收者
            msg.toUserId = ToUserId;//接收者
            msg.FromId = Applicate.MyAccount.userId;//发送者
            msg.fromUserId = Applicate.MyAccount.userId;//发送者
            msg.content = "邀请您语音会议";//打招呼内容
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.objectId = roomJid;//对方jid
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.AudioMeetingInvite;//询问是否准备好音频通话
            msg.toUserName = toNickname;
            msg.Insert();
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 发送加入视频会议
        /// <summary>
        /// 发送加入视频会议
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="ToUserId"></param>
        internal static void SendVideoMeetingJoin(string roomId, string ToUserId)
        {
            Messageobject message = new Messageobject();
            message.ToId = roomId;
            message.toUserId = ToUserId;//接收者
            message.FromId = Applicate.MyAccount.userId;//
            message.fromUserId = Applicate.MyAccount.userId;//
            message.content = "加入视频会议";//打招呼内容
            message.fromUserName = Applicate.MyAccount.nickname;//
            message.objectId = roomId;//房间jid
            message.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            message.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            message.type = kWCMessageType.VideoMeetingJoin;//加入视频通话
            var mem = new DataofMember() { userId = ToUserId }.GetModelByJid(roomId);
            if (mem != null)
            {
                message.toUserName = mem.nickname;
            }
            message.Insert();
            xmpp.SendJsonMsg(message);
        }
        #endregion

        #region 发送退出视频会议
        /// <summary>
        /// 发送退出视频会议
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="ToUserId"></param>
        internal static void SendVideoMeetingQuit(string roomId, string ToUserId, int timeLen)
        {
            Messageobject msg = new Messageobject();
            msg.ToId = roomId;
            msg.FromId = Applicate.MyAccount.userId;//
            msg.content = "退出视频会议";//打招呼内容
            msg.fromUserId = Applicate.MyAccount.userId;//
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.toUserId = ToUserId;//接收者
            msg.timeLen = timeLen;//时长
            msg.objectId = roomId;//房间jid
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.VideoMeetingQuit;//退出视频通话
            var mem = new DataofMember() { userId = ToUserId }.GetModelByJid(roomId);
            if (mem != null)
            {
                msg.toUserName = mem.nickname;
            }
            msg.Insert();
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 发送加入音频会议
        /// <summary>
        /// 发送加入音频会议
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="ToUserId"></param>
        internal static void SendAudioMeetingJoin(string RoomId, string ToUserId)
        {
            Messageobject message = new Messageobject();
            message.ToId = RoomId;
            message.FromId = Applicate.MyAccount.userId;//
            message.fromUserId = Applicate.MyAccount.userId;//
            message.content = "加入音频会议";//打招呼内容
            message.fromUserName = Applicate.MyAccount.nickname;//
            message.toUserId = ToUserId;//接收者
            message.objectId = RoomId;//房间jid
            message.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            message.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            message.type = kWCMessageType.AudioMeetingJoin;//加入音频通话
            var mem = new DataofMember() { userId = ToUserId }.GetModelByJid(RoomId);
            if (mem != null)
            {
                message.toUserName = mem.nickname;
            }

            message.Insert();
            xmpp.SendJsonMsg(message);
        }
        #endregion

        #region 发送退出音频会议
        /// <summary>
        /// 发送退出音频会议
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="ToUserId"></param>
        internal static void SendAudioMeetingQuit(string RoomId, string ToUserId, int timeLen)
        {
            Messageobject msg = new Messageobject();
            msg.ToId = RoomId;
            msg.toUserId = ToUserId;//接收者
            msg.FromId = Applicate.MyAccount.userId;//
            msg.fromUserId = Applicate.MyAccount.userId;//
            msg.content = "退出音频会议";//打招呼内容
            msg.fromUserName = Applicate.MyAccount.nickname;//
            msg.objectId = RoomId;//房间jid
            msg.timeLen = timeLen;//时长
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间
            msg.messageId = Guid.NewGuid().ToString("N");//生成MessageId
            msg.type = kWCMessageType.AudioMeetingQuit;//退出音频通话
            var mem = new DataofMember() { userId = ToUserId }.GetModelByJid(RoomId);
            if (mem != null)
            {
                msg.toUserName = mem.nickname;
            }

            msg.Insert();
            xmpp.SendJsonMsg(msg);
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
        }
        #endregion

        #region 转发消息(支持多条)
        /// <summary>
        /// 转发消息(支持多条)
        /// </summary>
        /// <param name="msgs">消息</param>
        /// <param name="toUsers">接收者</param>
        public static void ForwardMessage(List<Messageobject> msgs, List<MessageListItem> toUsers)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < toUsers.Count; i++)
                {
                    for (int j = 0; j < msgs.Count; j++)
                    {
                        msgs[j].ToId = msgs[j].toUserId = toUsers[i].Jid;//设置接收者
                        msgs[j].fromUserId = msgs[j].FromId = Applicate.MyAccount.userId;//设置我的ID
                        msgs[j].toUserName = toUsers[i].MessageTitle;//设置昵称
                        msgs[j].messageId = Guid.NewGuid().ToString("N");//修改MessageId
                        msgs[j].isGroup = (toUsers[i].Jid.Length > 8) ? (0) : (1);//m
                        msgs[j].timeSend = Helpers.DatetimeToStamp(DateTime.Now);//设置发送时间
                        xmpp.SendJsonMsg(msgs[j], toUsers[i].Jid);//发送
                        msgs[j].Insert();//存入数据库
                        Messenger.Default.Send(msgs[j], MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                    }
                }
            });
        }
        #endregion

        #region 发送文件信息
        /// <summary>
        /// 发送文件信息
        /// </summary>
        /// <param name="localpath">本地文件路径(包括文件名)</param>
        internal static void SendMessageFile(MessageListItem target, string localpath)
        {
            string guid = Guid.NewGuid().ToString("N");//定义一个Guid
            Messageobject msg = new Messageobject();
            //如果为
            BitmapImage bit = null;
            #region 判断是否为图片文件
            //bool isImage = false;//判断是否为图片文件
            //bool isAudio = false;//判断是否音频文件
            //bool isVideo = false;//判断是否为视频文件
            //string strTest = localpath.Substring(localpath.LastIndexOf('.') + 1).ToLower();
            switch (localpath.Substring(localpath.LastIndexOf('.') + 1).ToLower())
            {
                case "png":
                case "jpg":
                case "jpeg":
                case "bmp":
                case "gif":
                    //isImage = true;
                    bit = FileUtil.ReadFileByteToBitmap(localpath);//获取图片
                                                                   //bit = new Bitmap(localpath);
                    msg.location_x = bit.Width;
                    msg.location_y = bit.Height;//设置图片的长宽
                    msg.fileName = localpath;//路径
                    msg.type = kWCMessageType.Image;//设置为图片类型
                    break;
                case "avi":
                case "wmv3":
                case "asf2":
                case "rm4":
                case "mp4":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "flv":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "3gp":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "mkv":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "mov":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "mpg":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "ogg":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "swf":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                case "vob":
                    msg.type = kWCMessageType.Video;
                    msg.fileName = localpath;
                    break;
                default:
                    msg.type = kWCMessageType.File;//设置为文件类型
                    msg.fileName = localpath.Substring(localpath.LastIndexOf('\\'));//设置文件名
                    break;
            }
            #endregion
            #region 封装messageObject
            //msg.type = kWCMessageType.kWCMessageTypeImage;//图片类型消息(*********此处需要改*********)
            msg.ToId = target.Jid;
            msg.FromId = Applicate.MyAccount.userId;
            msg.toUserId = target.Jid;//接受者Id
            msg.toUserName = target.ShowTitle;//接收者昵称
            msg.isGroup = target.Jid.Length > 10 ? 1 : 0;//判断是否为群
            msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
            msg.content = localpath;
            int start = localpath.LastIndexOf('\\') + 1, end = localpath.Length - localpath.LastIndexOf('\\') - 1;
            msg.fileName = localpath.Substring(start, end);
            msg.filePath = localpath;//文件位置
            msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//设置当前的时间
            msg.fromUserId = Applicate.MyAccount.userId;//发送者Id
            msg.fileSize = Convert.ToInt32(new FileInfo(localpath).Length);//文件大小
            msg.messageId = guid;//设置唯一uid
            #endregion

            #region 上传文件和获取返回值
            var client = new WebClient();
            client.Headers.Add("Content-Type", "multipart/form-data; boundary=---------------------------7d5b915500cee");
            //client.Headers.Add("","");
            //client.Tag = msg;//使用赋值临时变量
            client.UploadDataCompleted += (s, res) =>
            {
                if (res.Error == null)
                {
                    string result = Encoding.UTF8.GetString(res.Result);
                    var rtnInfo = JsonConvert.DeserializeObject<JsonFileinfo>(result);//获取返回值
                                                                                      //var msg = ((HttpClient)s).Tag as Messageobject;
                    if (rtnInfo.data.images.Count > 0)
                    {
                        msg.content = rtnInfo.data.images[0].oUrl;//消息内容为返回的图片原始Url
                    }
                    else if (rtnInfo.data.videos.Count > 0)
                    {
                        msg.content = rtnInfo.data.videos[0].oUrl;
                    }
                    else if (rtnInfo.data.audios.Count > 0)
                    {
                        msg.content = rtnInfo.data.audios[0].oUrl;
                    }
                    else if (rtnInfo.data.others.Count > 0)
                    {
                        msg.content = rtnInfo.data.others[0].oUrl;//消息内容为返回的文件原始Url
                    }
                    msg.Insert();//存入数据库
                    xmpp.SendJsonMsg(msg);
                }
                else
                {
                    ConsoleLog.Output("上传文件失败" + res.Error.Message);
                }
            };
            #endregion

            #region 获取文件
            var uri = new Uri(Applicate.URLDATA.data.uploadUrl + "upload/UploadServlet");
            // 读文件流
            FileStream fs = new FileStream(localpath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //这部分需要完善
            string ContentType = "application/octet-stream";
            byte[] fileBytes = new byte[fs.Length];
            fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));
            // 生成需要上传的二进制数组
            CreateBytes cb = new CreateBytes();
            // 所有表单数据
            ArrayList bytesArray = new ArrayList
            {
                // 普通表单
                cb.CreateFieldData("validTime", "-1"),
                // 文件表单
                cb.CreateFieldData("-1", localpath, ContentType, fileBytes)
            };
            // 合成所有表单并生成二进制数组
            byte[] bytes = cb.JoinBytes(bytesArray);
            #endregion

            client.UploadDataAsync(uri, bytes);//异步上传文件
            Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
            Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//发送后选中第一个好友
        }
        #endregion

        #region 发送动画表情
        /// <summary>
        /// 发送动画表情
        /// </summary>
        /// <param name="localpath">本地文件路径(包括文件名)</param>
        internal static void SendGifMessage(MessageListItem target, string text)
        {
            try
            {
                //封装Message对象
                Messageobject msg = new Messageobject
                {
                    content = text, //(是否加密) ? (AES.EncryptDES(text, "1234567")) : (text),//设置信息内容
                                    //isEncrypt = Convert.ToInt16(是否加密),
                    FromId = Applicate.MyAccount.userId,//发送方UserId
                    fromUserId = Applicate.MyAccount.userId,//发送方UserId
                    ToId = target.Jid,
                    toUserId = target.Jid,//接收方UserID
                    toUserName = target.ShowTitle,//接收方昵称
                };
                msg.isGroup = (target.Jid.Length > 10) ? (1) : (0);//判断是否为群组
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间(使用时间戳)
                msg.messageId = Guid.NewGuid().ToString("N");//设置Message唯一ID
                msg.type = kWCMessageType.Gif;//指定为Gif类型
                                              //更新消息列表中内容
                Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                                                                                           //Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//选中第一项
                msg.Insert();//存库
                xmpp.SendJsonMsg(msg);
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("Send Gif Message Error" + ex.Message);
            }
        }
        #endregion

        #region 发送语音信息
        /// <summary>
        /// 发送语音信息
        /// </summary>
        /// <param name="target"></param>
        /// <param name="audio"></param>
        /// <param name="timeLen"></param>
        /// <param name="fileSize"></param>
        internal static void SendVoice(MessageListItem target, string audio, int timeLen = 0, int fileSize = 0)
        {
            try
            {
                //封装Message对象 
                Messageobject msg = new Messageobject();
                msg.ToId = target.Jid;
                msg.FromId = Applicate.MyAccount.userId;//发送方UserId
                msg.fromUserId = Applicate.MyAccount.userId;//发送方UserId
                msg.content = audio;//设置信息内容
                msg.toUserId = target.Jid; ;//接收方UserID
                msg.toUserName = target.ShowTitle;
                msg.isGroup = target.Jid.Length > 10 ? 1 : 0;//设置是否为群组消息类型
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间(使用时间戳)
                msg.messageId = Guid.NewGuid().ToString("N");//设置Message唯一ID
                msg.type = kWCMessageType.Voice;//发送信息
                msg.timeLen = timeLen;
                msg.fileSize = fileSize;
                msg.readPersons = 0.ToString();
                Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                msg.Insert();
                xmpp.SendJsonMsg(msg);
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("Send Audio Message Error：" + ex.Message);
            }
        }
        #endregion

        #region 发送视频信息
        /// <summary>
        /// 发送视频信息
        /// </summary>
        /// <param name="target"></param>
        /// <param name="audio"></param>
        /// <param name="fileSize"></param>
        internal static void SendVideo(MessageListItem target, string audio, int fileSize = 0)
        {
            try
            {
                //封装Message对象
                Messageobject msg = new Messageobject();
                msg.ToId = target.Jid;//接收方
                msg.toUserId = target.Jid;//接收方UserID
                msg.FromId = Applicate.MyAccount.userId;//发送方UserId
                msg.fromUserId = Applicate.MyAccount.userId;//发送方UserId
                msg.content = audio;//设置信息内容
                msg.toUserName = target.ShowTitle;//接收者名称
                msg.isGroup = target.Jid.Length > 20 ? 1 : 0;//判断是否为
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间(使用时间戳)
                msg.messageId = Guid.NewGuid().ToString("N");//设置Message唯一ID
                msg.type = kWCMessageType.Video;//指定为视频消息   
                msg.fileSize = fileSize;
                Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                msg.Insert();//存入数据库
                xmpp.SendJsonMsg(msg);
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("Send Video Message Error：" + ex.Message);
            }
        }
        #endregion

        #region 发送文本信息
        /// <summary>
        /// 发送文本信息(包含@消息)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="objectId"></param>
        internal static void SendText(MessageListItem target, string text, string objectId = "")
        {
            try
            {
                //封装Message对象
                Messageobject msg = new Messageobject
                {
                    content = text,
                    FromId = Applicate.MyAccount.userId,//发送方UserId
                    fromUserId = Applicate.MyAccount.userId,//发送方UserId
                    ToId = target.Jid,
                    toUserId = target.Jid,//接收方UserID
                    toUserName = target.ShowTitle,//接收方昵称
                };
                msg.isGroup = (target.Jid.Length > 10) ? (1) : (0);//判断是否为群
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间(使用时间戳)
                msg.messageId = Guid.NewGuid().ToString("N");//设置Message唯一ID
                msg.type = kWCMessageType.Text;//发送信息1，文本类型
                if (!string.IsNullOrWhiteSpace(objectId))
                {
                    msg.objectId = objectId;
                }
                //更新消息列表中内容
                Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                                                                                           //Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//发送后选中第一项
                msg.Insert();//存库
                xmpp.SendJsonMsg(msg);
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("Send Text Message Error：" + ex.Message);
            }
        }
        #endregion

        #region 发送位置信息
        /// <summary>
        /// 发送位置信息
        /// </summary>
        /// <param name="msgPara">需要发送的信息对象</param>
        internal static void SendMessageTypeLocation(MessageListItem target, string content, string location_x, string location_y, string objectId)
        {
            try
            {
                //封装Message对象
                Messageobject msg = new Messageobject
                {
                    content = content, //(ConfigurationUtil.GetValue("isEncrypt") == "1") ? (AES.EncryptDES(content, "1234567")) : (content),//设置信息内容
                    objectId = objectId,
                    location_x = double.Parse(location_x),
                    location_y = double.Parse(location_y),
                    FromId = Applicate.MyAccount.userId,//发送方UserId
                    fromUserId = Applicate.MyAccount.userId,//发送方UserId
                    fromUserName = Applicate.MyAccount.nickname,//接收方(好友)的好友名字
                    ToId = target.Jid,
                    toUserId = target.Jid,//接收方UserID
                    toUserName = target.ShowTitle,//接收者名称
                };
                msg.isGroup = target.Jid.Length > 10 ? 1 : 0;//设置是否为群聊
                msg.fromUserName = (msg.isGroup == 1) ? target.MessageItemContent : Applicate.MyAccount.nickname;//发送者群昵称或用户名
                msg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);//发送时间(使用时间戳)
                msg.messageId = Guid.NewGuid().ToString("N");//设置Message唯一ID
                msg.type = kWCMessageType.Location;//指定为位置类型
                Messenger.Default.Send(msg, MainViewNotifactions.CreateOrUpdateRecentItem);//通知主窗口更新
                msg.Insert();//存库
                xmpp.SendJsonMsg(msg);//发送消息
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("Send Location Message Error：" + ex.Message);
            }
        }
        #endregion

        #region 发送已读消息
        /// <summary>
        /// 发送已读消息
        /// </summary>
        /// <param name="messageId">消息Id</param>
        /// <param name="to">接收者</param>
        internal static void SendRead(string messageId, string to)
        {
            string target = to.Clone() as string;
            //使用实体类发送
            string uid = Guid.NewGuid().ToString("N");//生成一个新的Guid
            Messageobject readmsg = new Messageobject();
            readmsg.messageId = uid;
            readmsg.content = messageId;
            readmsg.isGroup = (target.Length > 11) ? (1) : (0);//可能为群聊消息
            readmsg.type = kWCMessageType.IsRead;//设置消息为已读回执
            readmsg.timeSend = Helpers.DatetimeToStamp(DateTime.Now);
            readmsg.fromUserId = Applicate.MyAccount.userId;
            readmsg.fromUserName = Applicate.MyAccount.nickname;
            xmpp.SendIsRead(readmsg, target);//发送
        }
        #endregion

        #endregion


        #region 退出程序
        /// <summary>
        /// 退出程序
        /// </summary>
        internal static void ApplicationExit()
        {
            try
            {
                //接口退出
                Applicate.IsAccountVerified = false;
                APIHelper.UserLogout();
                Applicate.GetWindow<MainWindow>().VlcPlayer.Dispose();//释放播放器
                new LocalUser().UpdateLastExitTime(Applicate.MyAccount.userId, Helpers.DatetimeToStamp(DateTime.Now));
                xmpp.XmppCon.Close();//关闭Xmpp连接
            }
            catch (Exception ex)
            {
                ConsoleLog.Output("退出时出错" + ex.Message);
            }
        }
        #endregion
    }
}
