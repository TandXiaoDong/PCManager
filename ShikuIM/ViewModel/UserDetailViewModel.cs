using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// 用户详情
    /// </summary>
    public class UserDetailViewModel : ViewModelBase
    {
        #region Private and Static Members
        private DataOfUserDetial friend;
        private object btnContent;
        private bool btnIsEnable;
        private bool isRemarkNameVisibility;
        private string tempRemarkName;
        private string remarkName;
        private bool isEditing;
        private SnackbarMessageQueue snackbar;

        #endregion

        #region Binding/Public Member
        /// <summary>
        /// 按钮启用状态
        /// </summary>
        public bool BtnVisiblity
        {
            get { return btnIsEnable; }
            set { btnIsEnable = value; RaisePropertyChanged(nameof(BtnVisiblity)); }
        }

        /// <summary>
        /// 是否显示备注名
        /// </summary>
        public bool IsRemarkNameVisibility
        {
            get { return isRemarkNameVisibility; }
            set { isRemarkNameVisibility = value; RaisePropertyChanged(nameof(IsRemarkNameVisibility)); }
        }

        /// <summary>
        /// 按钮内容
        /// </summary>
        public object BtnContent
        {
            get { return btnContent; }
            set { btnContent = value; RaisePropertyChanged(nameof(BtnContent)); }
        }


        /// <summary>
        /// 好友详情
        /// </summary>
        public DataOfUserDetial Friend
        {
            get { return friend; }
            set { friend = value; RaisePropertyChanged(nameof(Friend)); }
        }
        /// <summary>
        /// 编辑状态备注名
        /// </summary>
        public string TempRemarkName
        {
            get { return tempRemarkName; }
            set { tempRemarkName = value; RaisePropertyChanged(nameof(TempRemarkName)); }
        }
        /// <summary>
        /// 真实的备注名
        /// </summary>
        public string RemarkName
        {
            get { return remarkName; }
            set { remarkName = value; RaisePropertyChanged(nameof(RemarkName)); }
        }
        /// <summary>
        /// 是否在编辑群资料(控制对话框)
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
            set { isEditing = value; RaisePropertyChanged(nameof(IsEditing)); }
        }

        /// <summary>
        /// 提示消息
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackbar; }
            set { snackbar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }
        #endregion

        #region Commands
        public ICommand SendOrAddCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    switch (Friend.status)//根据不同的好友类型判断
                    {
                        case 2://互为好友,,发消息
                            var item = new MessageListItem()
                            {
                                MessageTitle = Friend.nickname,
                                ShowTitle = Friend.remarkName,
                                Jid = Friend.userId,
                                //MessageItemType = (newMsg.type >= kWCMessageType.kWCMessageTypeFriReq && newMsg.type <= kWCMessageType.kWCMessageTypeDeblack) ? (ItemType.VerifyMsg) : (ItemType.Message)
                                MessageItemType = ItemType.Message,
                                Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Friend.userId)
                            };
                            Messenger.Default.Send(item, MainViewNotifactions.MainInsertRecentItem);//通知主窗口添加最近消息项
                            Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//通知主窗口开始新聊天
                            Messenger.Default.Send(0, MainViewNotifactions.MainGoToPage);//转至聊天页面
                            Messenger.Default.Send(true, UserDetailView.CloseWindow);//通知好友详情页面关闭
                            Messenger.Default.Send(true, MainWindow.ActiveWindow);//激活主窗口
                            break;
                        case 1://关注,,(大业说不为2都可以做添加好友操作)
                               //break;
                        case 0://陌生人,,给对方发送好友申请
                            var client = ShiKuManager.AddFriend(friend);
                            client.Tag = friend;//设置Tag为临时用户
                            client.UploadDataCompleted += (sen, res) =>
                            {
                                if (res.Error == null)//无网络错误时
                                {
                                    var restxt = Encoding.UTF8.GetString(res.Result);
                                    var result = JsonConvert.DeserializeObject<JsonAttention>(restxt);
                                    if (result.resultCode == 1)
                                    {
                                        var friend = ((HttpClient)sen).Tag as DataOfUserDetial;
                                        if (result.data.type == 2 || result.data.type == 4)
                                        {
                                            if (friend != null)
                                            {
                                                ShiKuManager.SendFriendRequest(friend.userId, true);//发送508
                                                Snackbar.Enqueue("你们已经是好友");
                                                BtnContent = "发送消息";
                                                var senditem = new MessageListItem
                                                {
                                                    Jid = friend.userId,
                                                    ShowTitle = friend.nickname,
                                                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(friend.userId)
                                                };
                                                Messenger.Default.Send(senditem, MainViewNotifactions.MainInsertRecentItem);//通知主窗口添加最近消息项
                                                Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//通知主窗口开始新聊天
                                                Messenger.Default.Send(0, MainViewNotifactions.MainGoToPage);//转至聊天页面
                                                Messenger.Default.Send(true, UserDetailView.CloseWindow);//通知好友详情页面关闭
                                                Messenger.Default.Send(true, MainWindow.ActiveWindow);//激活主窗口
                                                return;
                                            }
                                        }
                                        else //if (data.resultCode > 1 && data.resultCode < 5)//返回代码在1和5之间
                                        {
                                            if (friend != null)
                                            {
                                                //ShiKuManager.SendFriendRequest(friend.userId, false);//发送500
                                                BtnContent = "等待验证...";
                                                BtnVisiblity = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Snackbar.Enqueue("添加好友失败：" + result.resultMsg);
                                    }
                                }
                                else//网络错误
                                {
                                    Snackbar.Enqueue("网络错误：" + res.Error.Message);
                                }
                            };
                            break;
                        case -1:
                            //移出黑名单
                            //var theBlack = new DataOfFriends().GetBlackUser(Friend.userId);//查出黑名单对象
                            var resclient = ShiKuManager.CancelBlockfriend(Friend.userId);//接口取消拉黑
                            resclient.Tag = Friend.ToDataOfFriend().ToMsgListItem(); //theBlack.ToMsgListItem();
                            resclient.UploadDataCompleted += (sender, e) =>
                            {
                                var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                                var black = ((HttpClient)sender).Tag as MessageListItem;
                                if (result.resultCode == 1)
                                {
                                    var tmpItem = new MessageListItem
                                    {
                                        Jid = friend.userId,
                                        ShowTitle = friend.nickname,
                                        Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(friend.userId)
                                    };
                                    Messenger.Default.Send(tmpItem.Jid, MainViewNotifactions.CancelBlockItem);//通知主窗口刷新好友和黑名单列表
                                    Messenger.Default.Send(tmpItem, MainViewNotifactions.MainInsertRecentItem);//通知主窗口添加最近消息项
                                    Messenger.Default.Send(0, MainViewNotifactions.MainChangeRecentListIndex);//通知主窗口开始新聊天
                                    Messenger.Default.Send(0, MainViewNotifactions.MainGoToPage);//转至聊天页面
                                    Messenger.Default.Send(true, UserDetailView.CloseWindow);//通知好友详情页面关闭
                                    Messenger.Default.Send(true, MainWindow.ActiveWindow);//激活主窗口
                                    Snackbar.Enqueue("你们已成为是好友");
                                    BtnContent = "发送消息";
                                }
                                else
                                {
                                    Snackbar.Enqueue("取消拉黑失败:" + result.resultMsg);
                                }
                            };
                            break;
                        case -2:
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public ICommand EditRemarkNameCommand
        {
            get
            {
                return new RelayCommand(() => IsEditing = true);
            }
        }

        /// <summary>
        /// 好友信息编辑后确认
        /// </summary>
        public ICommand UserInfoConfirmCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var client = APIHelper.RemarkFriendAsync(Friend.userId, TempRemarkName);
                    client.UploadDataCompleted += UpdateRemarkComplete;
                });
            }
        }

        #region 更新昵称完成
        /// <summary>
        /// 更新昵称完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateRemarkComplete(object sender, UploadDataCompletedEventArgs res)
        {
            if (res.Error == null)//正常情况
            {
                var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                if (result.resultCode == 1)
                {
                    new DataOfFriends().UpdateRemarkName(Friend.userId, TempRemarkName);//更新数据库
                    RemarkName = TempRemarkName;//更新界面
                    Friend.remarkName = RemarkName;//更新好友昵称
                    TempRemarkName = "";//重置编辑文本框
                    IsEditing = false;//取消编辑状态(关闭对话框)
                    //通知主窗口更改备注
                    var item = new MessageListItem
                    {
                        Jid = friend.userId,
                        MessageTitle = Friend.remarkName,
                        Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(friend.userId)
                    };
                    Messenger.Default.Send(item, MainViewNotifactions.UpdateAccountName);//通知主窗口更新名称
                    Snackbar.Enqueue("修改昵称成功");
                }
            }
            else
            {
                Snackbar.Enqueue("修改昵称失败：" + res.Error.Message);
            }
        }
        #endregion

        /// <summary>
        /// 取消编辑Command
        /// </summary>
        public ICommand CancelEditCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TempRemarkName = "";
                    IsEditing = false;//关闭弹出框
                });
            }
        }
        #endregion

        #region Contructor
        public UserDetailViewModel()
        {
            Snackbar = new SnackbarMessageQueue();
            //注册显示
            Messenger.Default.Register<string>(this, UserDetailNotifications.ShowUserDetial, userid => ShowInfo(userid));
        }
        #endregion

        #region 显示数据
        public void ShowInfo(string userId)
        {
            Friend = new DataOfUserDetial();//清空
            //FileUtil.DeleteUserHeadImg(userId); //删除头像
            var dbfriend = new DataOfFriends() { toUserId = userId };//数据库查询对象
            if (dbfriend.Exists(userId))//如果本地存在此账号(无论以何种状态)
            {
                dbfriend = dbfriend.GetByUserId();//获取本地存储用户
                IsRemarkNameVisibility = (userId.Length <= 6) ? false : true;//系统号不显示备注
                switch (dbfriend.status)
                {
                    case 2://如果为好友的话
                        BtnContent = "发消息";
                        BtnVisiblity = true;
                        break;
                    case 1://我单方等待对方验证时
                        BtnContent = "等待验证..";
                        BtnVisiblity = true;
                        break;
                    case 0://如果在本地身份为陌生人时
                        BtnContent = "添加好友";
                        BtnVisiblity = true;
                        break;
                    case -1://如果在黑名单
                        BtnContent = "移出黑名单";
                        BtnVisiblity = true;
                        break;
                    case -2://我方被拉黑
                        BtnContent = "我已被对方拉黑";
                        BtnVisiblity = false;
                        break;
                    default:
                        break;
                }
                ShowLocalUserDetial(userId);//显示本地好友详情
            }
            else//如果不存在本地
            {
                BtnContent = "添加好友";
                BtnVisiblity = true;
                ShowStrangerDetial(userId);//显示陌生人详情
            }
        }
        #endregion

        #region 显示陌生人详情
        /// <summary>
        /// 显示陌生人详情
        /// </summary>
        /// <param name="userId">UserId</param>
        private void ShowStrangerDetial(string userId)
        {
            var dclient = APIHelper.GetUserDetialAsync(userId);//详情
            LogHelper.log.Info("陌生用户GetUserDetial" + userId);
            dclient.UploadDataCompleted += (s, res) =>
            {
                var user = JsonConvert.DeserializeObject<JsonuserDetial>(Encoding.UTF8.GetString(res.Result));
                var tmpFri = user.data.ConvertToDataFriend();
                if (tmpFri != null)
                {
                    Friend = user.data;
                    var downloads = new List<DownLoadFile>();
                    downloads.Add(new DownLoadFile()
                    {
                        Jid = userId,
                        Token = userId,
                        Name = user.data.nickname,
                        Type = DownLoadFileType.Image,
                        LocalUrl = Applicate.LocalConfigData.GetDownloadAvatorPath(userId),
                        ShouldDeleteWhileFileExists = true
                    });
                    //下载头像
                    HttpDownloader.Download(downloads, (file) =>
                    {
                        switch (file.State)
                        {
                            case DownloadState.Successed:
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Friend.userId = file.Jid;
                                });
                                break;
                            case DownloadState.Error:
                                break;
                            default:
                                break;
                        }
                    });
                    if (!string.IsNullOrWhiteSpace(user.data._remarkName))
                    {
                        RemarkName = user.data._remarkName;
                    }
                    else
                    {
                        RemarkName = Friend.remarkName;
                    }
                    var stranger = new DataOfFriends();
                    stranger = user.data.ToDataOfFriend();
                    stranger.status = 0;
                    stranger.AutoInsert();
                }
            };
        }
        #endregion

        #region 显示本地用户详情
        /// <summary>
        /// 显示本地用户详情
        /// </summary>
        /// <param name="userId">UserId</param>
        private void ShowLocalUserDetial(string userId)
        {
            var tmpFri = new DataOfFriends() { toUserId = userId }.GetByUserId();
            if (tmpFri != null)
            {
                Friend = tmpFri.ToDataOfUserDetial();
                Friend.sex = -1;//好友列表中无性别
                Friend.birthday = 0;//无生日数据
                RemarkName = Friend.remarkName;
            }
            var dclient = APIHelper.GetUserDetialAsync(userId);//详情
            //LogHelper.log.Info("用户详情GetUserDetial" + userId);
            dclient.UploadDataCompleted += (s, res) =>
            {
                if (res.Error == null)
                {
                    var user = JsonConvert.DeserializeObject<JsonuserDetial>(Encoding.UTF8.GetString(res.Result));
                    var tmpfriend = user.data.ConvertToDataFriend();
                    if (tmpfriend != null)
                    {
                        int status = Friend.status;//暂时保存好友关系
                        Friend = user.data;//显示数据
                        Friend.status = status;//重新赋值正确好友关系
                        if (!string.IsNullOrWhiteSpace(user.data._remarkName))
                        {
                            RemarkName = user.data._remarkName;
                        }
                        else
                        {
                            RemarkName = Friend.friends.remarkName;
                        }
                        var resfriend = new DataOfFriends();
                        resfriend = user.data.ToDataOfFriend();
                        //resfriend.status = 2;//设置为朋友
                        resfriend.AutoInsert();
                        if (Friend.remarkName != user.data.remarkName)
                        {
                            //刷新
                            Messenger.Default.Send(new MessageListItem
                            {
                                Jid = user.data.userId,
                                ShowTitle = user.data._remarkName,
                                Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(user.data.userId)
                            }, MainViewNotifactions.UpdateAccountName);
                        }
                    }
                }
                else
                {
                    Snackbar.Enqueue(res.Error.Message);
                }
            };

        }
        #endregion

        #region 获取好友完成后
        private void GetFriendDetailComplete(object sender, UploadDataCompletedEventArgs e)
        {
            var tmp = JsonConvert.DeserializeObject<DataOfFriends>(Encoding.UTF8.GetString(e.Result));
            Friend = new DataOfUserDetial()
            {
                userId = tmp.toUserId,
                nickname = tmp.toNickname,
                description = tmp.description,
                birthday = tmp.birthday,
                sex = tmp.sex
            };
        }
        #endregion

    }
}
