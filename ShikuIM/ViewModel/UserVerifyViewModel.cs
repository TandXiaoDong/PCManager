using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{

    /// <summary>
    /// 用户验证ViewModel
    /// </summary>
    public class UserVerifyListViewModel : ViewModelBase
    {
        #region Private Members

        private string answerContnt;//回话文本
        private SnackbarMessageQueue toast = new SnackbarMessageQueue();//提示消息
        private ObservableCollection<VerifingFriend> verifyUserList = new ObservableCollection<VerifingFriend>();//验证好友列表
        private bool dialogShow;//弹出框显示控
        private bool answerBtnEnable;//回话按钮启用
        private bool inProgress = false;//是否处于发送状态
        private bool answerProgressbarVisible = false;//回话进度条显示
        private bool answerTxtVisible = true;//回话按钮文字显示
        private bool inVerifingProgress;//同意验证中
        private bool agreeTxtVisible = true;//"通过验证"文字显示
        private bool agreeProgressVisible = false;//"通过验证"进度条显示
        #endregion

        #region Public Members
        /// <summary>
        /// "通过验证"进度条显示
        /// </summary>
        public bool AgreeProgressVisible
        {
            get { return agreeProgressVisible; }
            set { agreeProgressVisible = value; RaisePropertyChanged(nameof(AgreeProgressVisible)); }
        }

        /// <summary>
        /// "通过验证"文字显示
        /// </summary>
        public bool AgreeTxtVisible
        {
            get { return agreeTxtVisible; }
            set { agreeTxtVisible = value; RaisePropertyChanged(nameof(AgreeTxtVisible)); }
        }

        /// <summary>
        /// 是否处于通过验证消息等待回执状态
        /// </summary>
        public bool InVerifingWaiting
        {
            get { return inVerifingProgress; }
            set
            {
                inVerifingProgress = value;
                if (inVerifingProgress)
                {
                    AgreeTxtVisible = false;
                    AgreeProgressVisible = true;
                }
                else
                {
                    AgreeTxtVisible = true;
                    AgreeProgressVisible = false;
                }
            }
        }


        /// <summary>
        /// 回话发送文字显示
        /// </summary>
        public bool AnswerTxtVisible
        {
            get { return answerTxtVisible; }
            set { answerTxtVisible = value; RaisePropertyChanged(nameof(AnswerTxtVisible)); }
        }

        /// <summary>
        /// 进度条显示
        /// </summary>
        public bool ProgressbarVisible
        {
            get { return answerProgressbarVisible; }
            set { answerProgressbarVisible = value; RaisePropertyChanged(nameof(ProgressbarVisible)); }
        }

        /// <summary>
        /// 回话按钮是否启用
        /// </summary>
        public bool AnswerBtnEnable
        {
            get { return answerBtnEnable; }
            set { answerBtnEnable = value; RaisePropertyChanged(nameof(AnswerBtnEnable)); }
        }

        /// <summary>
        /// 运行状态
        /// </summary>
        public bool InProgress
        {
            get
            {
                if (inProgress)
                {
                    AnswerBtnEnable = false;//禁用按钮
                    ProgressbarVisible = true;//显示进度条
                    AnswerTxtVisible = false;//隐藏文字
                }
                else
                {
                    AnswerBtnEnable = true;//启用按钮
                    ProgressbarVisible = false;//隐藏进度条
                    AnswerTxtVisible = true;//显示文字
                }
                return inProgress;
            }
            set
            {
                inProgress = value;
                if (inProgress)
                {
                    AnswerBtnEnable = false;//禁用按钮
                    ProgressbarVisible = true;//显示进度条
                    AnswerTxtVisible = false;//隐藏文字
                }
                else
                {
                    AnswerBtnEnable = true;//启用按钮
                    ProgressbarVisible = false;//隐藏进度条
                    AnswerTxtVisible = true;//显示文字
                }
            }
        }

        /// <summary>
        /// 验证好友列表
        /// </summary>
        public ObservableCollection<VerifingFriend> VerifyUserList
        {
            get { return verifyUserList; }
            set { verifyUserList = value; RaisePropertyChanged(nameof(VerifyUserList)); }
        }

        /// <summary>
        /// 提示消息
        /// </summary>
        public SnackbarMessageQueue SnackBar
        {
            get { return toast; }
            set { toast = value; RaisePropertyChanged(nameof(SnackBar)); }
        }

        /// <summary>
        /// 回话文本
        /// </summary>
        public string AnswerContnt
        {
            get { return answerContnt; }
            set { answerContnt = value; RaisePropertyChanged(nameof(AnswerContnt)); }
        }


        /// <summary>
        /// 弹出框显示控制
        /// </summary>
        public bool DialogShow
        {
            get { return dialogShow; }
            set { dialogShow = value; RaisePropertyChanged(nameof(DialogShow)); }
        }
        #endregion

        #region Commands


        /// <summary>
        /// 同意Command
        /// </summary>
        public ICommand AgreeCommand
        {
            get
            {
                return new RelayCommand<VerifingFriend>((friend) =>
                {
                    InVerifingWaiting = true;
                    if (friend.toUserId.Length < 20)//好友验证
                    {
                        var frienddetail = new DataOfFriends()
                        {
                            toUserId = friend.toUserId,
                            toNickname = friend.toNickname
                        };
                        var client = ShiKuManager.AgreeFriendReq(frienddetail);//同意点击事件
                        client.Tag = frienddetail;
                        client.UploadDataCompleted += AgreeComplete;//回调
                    }
                    else//进群验证
                    {
                        var room = new Room() { jid = friend.toUserId }.GetByJid();
                        if (room != null)
                        {
                            string[] memArray = friend.userKey.Split(',');
                            var client = APIHelper.UpdateCreateGroupAsync(room.id, memArray.ToList());
                            client.UploadDataCompleted += (sen, res) =>
                            {
                                var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                                if (rtn.resultCode == 1)
                                {
                                    friend.Delete();
                                    VerifyUserList.Remove(VerifyUserList.FirstOrDefault(d => d.id == friend.id));
                                }
                            };
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 查看好友详情
        /// </summary>
        public ICommand CheckDetailCommand
        {
            get
            {
                return new RelayCommand<string>((para) =>
                {
                    var userid = para.ToString();
                    if (Applicate.MyAccount.userId == userid)
                    {
                        Personal.GetPersonal();
                    }
                    else
                    {
                        Messenger.Default.Send(userid, UserDetailNotifications.ShowUserDetial);
                        UserDetailView.GetWindow().Show();
                    }
                });
            }
        }

        /// <summary>
        /// 回话框打开Command
        /// </summary>
        public ICommand AnswerOpenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DialogShow = true;
                });
            }
        }

        /// <summary>
        /// 回话发送Command
        /// </summary>
        public ICommand AnswerActionCommand
        {
            get
            {
                return new RelayCommand<VerifingFriend>((obj) =>
                {
                    var friend = obj;
                    InProgress = true;//启用等待5
                    ShiKuManager.ReAnswerBack(AnswerContnt, new MessageListItem
                    {
                        Jid = friend.toUserId,
                        ShowTitle = friend.toNickname,
                        Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(friend.toUserId)
                    });
                    VerifyUserList.FirstOrDefault(v => v.toUserId == friend.toUserId).Content = "我：" + AnswerContnt;
                });
            }
        }
        #endregion

        #region Construstor
        /// <summary>
        /// Initial Data from Database and sth. else
        /// </summary>
        public UserVerifyListViewModel()
        {
            if (IsInDesignMode)
            {
                return;
            }
            var lists = new VerifingFriend().GetVerifingsList();//读取数据库
            for (int i = 0; i < lists.Count; i++)
            {
                VerifyUserList.Add(lists[i]);
            }
            InitialMessanger();
        }
        #endregion

        private void InitialMessanger()
        {
            ////Xmpp通知注册
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgRecived, msg => { ProcessNewMessage(msg); });
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgReceipt, (msg) => MsgReceipt(msg));
            Messenger.Default.Register<VerifingFriend>(this, VerifyFriendLIstToken.DeleteVerifyItem, (item) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    VerifyUserList.Remove(item);
                });
            });


        }

        #region 同意完成后
        /// <summary>
        /// 同意完成后
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e"></param>
        private void AgreeComplete(object sender, UploadDataCompletedEventArgs e)
        {
            var source = (HttpClient)sender;//事件源
            var resstr = Encoding.UTF8.GetString(e.Result);//获取字符
            var resjson = JsonConvert.DeserializeObject<JsonBase>(resstr);//反序列化
            //过滤状态码
            switch (resjson.resultCode)
            {
                case 1:
                case 2:
                    ShiKuManager.SendVerifyAgreeXmpp((DataOfFriends)source.Tag);//发送Xmpp消息(通过验证)
                    break;
                case 3:
                    MessageBox.Show("已经添加该好友, 不能重复添加", "提示", MessageBoxButton.OK);
                    break;
                case 1100801:
                    MessageBox.Show("对方拒绝被添加", "提示", MessageBoxButton.OK);
                    break;
                case 1100802:
                    MessageBox.Show("你已经被对方拉黑, 不能添加对方", "提示", MessageBoxButton.OK);
                    break;
                default:
                    break;
            }
            InVerifingWaiting = false;
        }
        #endregion

        #region 添加或更新集合
        /// <summary>
        /// 添加或更新集合
        /// </summary>
        /// <param name="item">对应的项</param>
        public void AddOrUpdateToList(VerifingFriend item)
        {
            #region 修改集合
            if (VerifyUserList.Count(v => v.toUserId == item.toUserId) > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var verifyItem = VerifyUserList.FirstOrDefault(v => v.toUserId == item.toUserId);
                    verifyItem.StatusTag = item.StatusTag;
                    verifyItem.VerifyStatus = item.VerifyStatus;
                });
            }
            else//添加未存在的消息
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    VerifyUserList.Add(item);
                });
            }
            #endregion
        }
        #endregion

        #region 收到消息时
        /// <summary>
        /// 收到消息时
        /// </summary>
        /// <param name="msg">收到的新消息</param>
        public void ProcessNewMessage(Messageobject msg)
        {
            if (msg.type >= kWCMessageType.FriendRequest && msg.type <= kWCMessageType.PhoneContactToFriend)
            {
                var vItem = new VerifingFriend();
                var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                if (VerifyUserList.FirstOrDefault(v => v.toUserId == msg.fromUserId) != null)
                {
                    vItem = VerifyUserList.FirstOrDefault(v => v.toUserId == msg.fromUserId);
                }

                switch (msg.type)
                {
                    case kWCMessageType.FriendRequest:
                        vItem.VerifyStatus = -4;//对方添加己方
                        vItem.StatusTag = "通过验证";
                        vItem.Content = msg.content;//消息内容
                        break;
                    case kWCMessageType.RequestAgree:
                        vItem.VerifyStatus = 1;
                        vItem.StatusTag = "已通过验证";
                        vItem.Content = "验证被" + msg.fromUserName /*((vItem.sex == 0) ? ("他") : ("她"))*/ + "通过了";
                        //mControl.AddToFriendList(msg.FromId);//添加到好友列表
                        var tmp = new MessageListItem()
                        {
                            Jid = msg.FromId,
                            ShowTitle = msg.fromUserName,
                            MessageTitle = msg.fromUserName,
                            MessageItemType = ItemType.Message,
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(msg.FromId)
                        };
                        mControl.MsgItemMouseDown(tmp);
                        mControl.StartNewChatFromItem(tmp.Clone());
                        break;
                    case kWCMessageType.RequestRefuse://对方回话
                        //item.StatusTag = "";
                        vItem.CanAnswer = true;
                        vItem.Content = msg.content;//回话内容
                        DialogShow = false;
                        AnswerContnt = "";//清空文本
                        break;
                    case kWCMessageType.DeleteFriend:
                        //System.Windows.Forms.MessageBox.Show("删除成功");
                        vItem.Content = msg.fromUserName + " 已删除了我";
                        vItem.VerifyStatus = 0;
                        break;
                    case kWCMessageType.BlackFriend:
                        vItem.VerifyStatus = 0;
                        vItem.Content = msg.fromUserName + " 拉黑了我";
                        break;
                    case kWCMessageType.RequestFriendDirectly:
                        vItem.VerifyStatus = 1;//互为好友
                        vItem.Content = msg.fromUserName + " 添加你为好友";//消息内容
                        break;
                    case kWCMessageType.CancelBlackFriend:
                        vItem.Content = msg.fromUserName + " 取消了黑名单";
                        Messenger.Default.Send(vItem.toUserId, MainViewNotifactions.CancelBlockItem);
                        var tmpFriend = new DataOfFriends();
                        break;
                    default:
                        break;
                }
                //更新最近用户中的内容
                Messenger.Default.Send(new MessageListItem
                {
                    Jid = "10001",
                    MessageItemContent = vItem.Content,
                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath("10001")
                }, MainViewNotifactions.UpdateRecentItemContent);
                vItem.toUserId = msg.fromUserId;//UserId
                vItem.toNickname = msg.fromUserName;//昵称
                vItem.Type = Convert.ToInt32(msg.type);//消息类型
                vItem.InsertOrUpdate();//存入数据库
                AddOrUpdateToList(vItem);
            }

        }
        #endregion


        #region 消息回执
        /// <summary>
        /// 消息回执
        /// </summary>
        /// <param name="msg"></param>
        public void MsgReceipt(Messageobject msg)
        {
            if (msg.type >= kWCMessageType.FriendRequest && msg.type <= kWCMessageType.CancelBlackFriend)
            {
                InVerifingWaiting = false;
                var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                var verifyItem = VerifyUserList.FirstOrDefault(v => v.toUserId == msg.toUserId);
                if (verifyItem == null)
                {
                    verifyItem = new VerifingFriend()
                    {
                        toNickname = msg.toUserName,
                        toUserId = msg.ToId
                    };
                }

                switch (msg.type)
                {
                    case kWCMessageType.FriendRequest://打招呼回执
                        verifyItem.VerifyStatus = -3;
                        verifyItem.Content = "等待验证中...";
                        break;
                    case kWCMessageType.RequestAgree://通过验证回执
                        verifyItem.VerifyStatus = 1;
                        verifyItem.Content = "我已通过对" + msg.toUserName /*((verifyItem.sex == 0) ? ("他") : ("她")) */+ "的验证";
                        var tmp = mControl.FriendList.FirstOrDefault(m => m.Jid == msg.toUserId);
                        //mControl.MsgItemMouseDown(tmp);
                        mControl.StartNewChatFromItem(tmp.Clone());
                        mControl.Snackbar.Enqueue(verifyItem.Content);//
                        //ServiceLocator.Current.GetInstance<MainViewModel>().LoadAllFriendsByApi();
                        break;
                    case kWCMessageType.RequestRefuse://回话回执
                        InProgress = false;//不处于发送状态
                        DialogShow = false;//关闭对话框
                        AnswerContnt = "";//清空文本
                        SnackBar.Enqueue("回话给" + msg.toUserName + "成功");
                        break;
                    case kWCMessageType.DeleteFriend:
                        verifyItem.Content = "已删除好友 " + msg.toUserName;
                        verifyItem.toNickname = msg.toUserName;//此处为接收者Name
                        verifyItem.VerifyStatus = -1;
                        break;
                    case kWCMessageType.BlackFriend:
                        verifyItem.Content = "已将好友 " + msg.toUserName + " 拉黑";
                        verifyItem.toNickname = msg.toUserName;//此处为接收者Name
                        break;
                    case kWCMessageType.RequestFriendDirectly://直接添加好友
                        verifyItem.VerifyStatus = 1;
                        verifyItem.Content = "已添加好友" + msg.toUserName /*((verifyItem.sex == 0) ? ("他") : ("她"))*/ ;
                        if (new DataOfFriends().ExistsLocal(msg.ToId))//本地数据库有用户
                        {
                            var dbFri = new DataOfFriends().GetByUserId(msg.ToId);
                            dbFri.UpdateFriendState(msg.ToId, 2);//互为好友
                        }
                        else
                        {

                        }
                        Messenger.Default.Send(verifyItem.Content, MainViewNotifactions.SnacbarMessage);//提示
                        break;
                    case kWCMessageType.CancelBlackFriend:
                        var tuser = new DataOfFriends().GetByUserId(msg.toUserId);
                        verifyItem.Content = "已将 " + tuser.toNickname + " 移出黑名单";
                        verifyItem.VerifyStatus = 1;//互为好友
                        break;
                    default:
                        break;
                }
                //更新最近用户中的内容
                Messenger.Default.Send(new MessageListItem
                {
                    Jid = "10001",
                    MessageItemContent = verifyItem.Content,
                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath("10001")
                }, MainViewNotifactions.UpdateRecentItemContent);
                AddOrUpdateToList(verifyItem);//更新UI
                verifyItem.InsertOrUpdate();//存入数据库
                verifyItem.Update();//更新至数据库
            }
        }
        #endregion


        #region 刷新头像
        public void RefreshAllImg(string userId)
        {
            foreach (var verifyItem in VerifyUserList)
            {
                if (verifyItem.toUserId == userId)
                {
                    verifyItem.toUserId = userId;
                }
            }
        }
        #endregion

        #region 更新用户名
        public void OnUserNameChanged(string jid, string nickname)
        {
            if (jid.Length > 10)
            {
                return;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                var item = VerifyUserList.FirstOrDefault(v => v.toUserId == jid);
                if (item != null)
                {
                    item.toNickname = nickname;//更新昵称
                }
            });
        }
        #endregion

    }
}
