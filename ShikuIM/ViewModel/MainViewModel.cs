using agsXMPP;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using ShikuIM.Resource;
using ShikuIM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>      
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region CommonMembers

        /// <summary>
        /// 对话框操作类型 
        /// </summary>
        public enum DialogFunction
        {
            /// <summary>
            /// 发送联系人
            /// </summary>
            SendContact,

            /// <summary>
            /// 转发消息
            /// </summary>
            ForwardMessage,

            /// <summary>
            /// @群成员
            /// </summary>
            AttentionGroupMember,

            /// <summary>
            /// 群音频聊天
            /// </summary>
            AudioChatWithGroupMember,

            /// <summary>
            /// 群视频聊天
            /// </summary>
            VideoChatWithGroupMember,

        }
        #endregion

        #region Private Member
        /// <summary>
        /// 正在输入消息的Timer
        /// </summary>
        private System.Timers.Timer TypingTimer = new System.Timers.Timer();
        private bool roomInfoVisible = false;
        private ObservableCollection<DataofMember> selectedGroupMember = new ObservableCollection<DataofMember>();
        private ObservableCollection<MessageListItem> messageItemList = new ObservableCollection<MessageListItem>();
        private ObservableCollection<MessageListItem> myGroupList = new ObservableCollection<MessageListItem>();
        private ObservableCollection<MessageListItem> friendList = new ObservableCollection<MessageListItem>();
        private ObservableCollection<MessageListItem> blackList = new ObservableCollection<MessageListItem>();
        private List<object> selectedContacts = new List<object>();
        private DataOfUserDetial selectedFriend = new DataOfUserDetial();
        private Session sess = new Session();
        private Room roomDetail = new Room();
        private DataOfUserDetial me = new DataOfUserDetial();
        private Room displayGroup = new Room();
        private SnackbarMessageQueue snackBar = new SnackbarMessageQueue();
        private SolidColorBrush accountStateFill;//用户状态颜色
        private GridLength windowLeftWidth = new GridLength(224);
        private bool groupdetailVisible;
        private int msgSelectedIndex = -1;
        private int tabSelectedIndex = 0;
        private int friendTypeIndex;
        private int unReadCount;
        private string accountStateText;
        private string hiddenMembers;
        private int myGroupSelectedIndex = -1;
        private int friendSelectedIndex = -1;
        private int blackSelectedIndex = -1;
        private bool selectedFriendProfileVisiblity;
        private string groupBtnContent;
        private FlowDocument textFieldDocument = new FlowDocument();
        private TextSelection sendBoxSelection;
        private bool isSendFile;
        private string sendAsFileTooltip = "作为文件发送";
        private string sencAsImageTooltip = "作为图片发送";
        private bool sendImageTooltipVisible;
        private bool sendFileTooltipVisible;
        private DialogFunction dialogFunctionType;
        #endregion

        #region Public Member

        /// <summary>
        /// 对话框操作类型
        /// </summary>
        public DialogFunction DialogFunctionType
        {
            get { return dialogFunctionType; }
            set { dialogFunctionType = value; RaisePropertyChanged(nameof(DialogFunctionType)); }
        }

        /// <summary>
        /// 提示音播放器
        /// </summary>
        public SoundPlayer soundPlayer { get; set; }

        /// <summary>
        /// 选中联系人(只能绑定List < Object >类型)
        /// </summary>
        public List<object> SelectedContacts
        {
            get { return selectedContacts; }
            set { selectedContacts = value; RaisePropertyChanged(nameof(SelectedContacts)); }
        }

        /// <summary>
        /// 对话框中显示的联系人 (一般用于转发和发送名片)
        /// </summary>
        public ObservableCollection<MessageListItem> DialogContacts { get; set; }

        /// <summary>
        /// 发送文本框TextSelection
        /// </summary>
        public TextSelection SendBoxSelection
        {
            get { return sendBoxSelection; }
            set
            {
                sendBoxSelection = value;
                RaisePropertyChanged(nameof(SendBoxSelection));
            }
        }

        /// <summary>
        /// SnackBarMessage 队列
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackBar; }
            set { snackBar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }

        /// <summary>
        /// 发送文件提示卡片显示
        /// </summary>
        public bool SendFileTooltipVisible
        {
            get { return sendFileTooltipVisible; }
            set
            {
                sendFileTooltipVisible = value;
                RaisePropertyChanged(nameof(SendFileTooltipVisible));
            }
        }

        /// <summary>
        /// 发送图片提示卡片显示
        /// </summary>
        public bool SendImageTooltipVisible
        {
            get { return sendImageTooltipVisible; }
            set
            {
                sendImageTooltipVisible = value;
                RaisePropertyChanged(nameof(SendImageTooltipVisible));
            }
        }

        /// <summary>
        /// 发送图片提示文本
        /// </summary>
        public string SendAsImageText
        {
            get { return sencAsImageTooltip; }
            set
            {
                sencAsImageTooltip = value;
                RaisePropertyChanged(nameof(SendAsImageText));
            }
        }

        /// <summary>
        /// 发送文件提示文本
        /// </summary>
        public string SendAsFileTooltip
        {
            get { return sendAsFileTooltip; }
            set
            {
                sendAsFileTooltip = value;
                RaisePropertyChanged(nameof(SendAsFileTooltip));
            }
        }

        /// <summary>
        /// 富文本框绑定
        /// </summary>
        public FlowDocument TextFieldDocument
        {
            get { return textFieldDocument; }
            set
            {
                textFieldDocument = value;
                RaisePropertyChanged(nameof(TextFieldDocument));
            }
        }

        /// <summary>
        /// 是否正在发送文件
        /// </summary>
        public bool IsSendFile
        {
            get { return isSendFile; }
            set
            {
                isSendFile = value;
                RaisePropertyChanged(nameof(IsSendFile));
            }
        }

        /// <summary>
        /// 窗口左侧宽度
        /// </summary>
        public GridLength WindowLeftWidth
        {
            get { return windowLeftWidth; }
            set
            {
                if (value == windowLeftWidth)
                {
                    return;
                }
                windowLeftWidth = value;
                RaisePropertyChanged(nameof(WindowLeftWidth));
            }
        }

        /// <summary>
        /// 选中好友详情是否显示
        /// </summary>
        public bool SelectedFriendProfileVisiblity
        {
            get { return selectedFriendProfileVisiblity; }
            set { selectedFriendProfileVisiblity = value; RaisePropertyChanged(nameof(SelectedFriendProfileVisiblity)); }
        }

        /// <summary>
        /// 黑名单选中索引
        /// </summary>
        public int BlackSelectedIndex
        {
            get { return blackSelectedIndex; }
            set
            {
                if (value == blackSelectedIndex)
                {
                    return;
                }
                blackSelectedIndex = value;
                RaisePropertyChanged(nameof(BlackSelectedIndex));
                FriendSelectionChanged();
            }
        }

        /// <summary>
        /// 好友选中的索引
        /// </summary>
        public int FriendSelectedIndex
        {
            get { return friendSelectedIndex; }
            set
            {
                if (value == friendSelectedIndex)
                {
                    return;
                }
                friendSelectedIndex = value;
                RaisePropertyChanged(nameof(FriendSelectedIndex));
                FriendSelectionChanged();//选中改变时
            }
        }

        /// <summary>
        /// 我的群组中选中索引
        /// </summary>
        public int MyGroupSelectedIndex
        {
            get { return myGroupSelectedIndex; }
            set
            {
                if (value == myGroupSelectedIndex)
                {
                    return;
                }
                myGroupSelectedIndex = value;
                RaisePropertyChanged(nameof(RaisePropertyChanged));
                MyGroupSelectionChanged();//选中项改变时
            }
        }

        /// <summary>
        /// 群详情显示
        /// </summary>
        public bool RoomInfoVisible
        {
            get { return roomInfoVisible; }
            set
            {
                if (value == roomInfoVisible)
                {
                    return;
                }
                roomInfoVisible = value;
                RaisePropertyChanged(nameof(RoomInfoVisible));
            }
        }

        /// <summary>
        /// 群组详情显示
        /// </summary>
        public bool GroupdetailVisible
        {
            get { return groupdetailVisible; }
            set
            {
                if (value == groupdetailVisible)
                {
                    return;
                }

                groupdetailVisible = value;
                RaisePropertyChanged(nameof(GroupdetailVisible));
            }
        }

        /// <summary>
        /// 用户在线状态(颜色)
        /// </summary>
        public SolidColorBrush AccountStateFill
        {
            get { return accountStateFill == null ? new SolidColorBrush() : accountStateFill; }
            set
            {
                if (value == accountStateFill)
                {
                    return;
                }
                accountStateFill = value;
                RaisePropertyChanged(nameof(AccountStateFill));
            }
        }

        /// <summary>
        /// 用户在线状态(文字提示)
        /// </summary>
        public string AccountStateText
        {
            get { return accountStateText; }
            set
            {
                accountStateText = value;
                RaisePropertyChanged(nameof(AccountStateText));
            }
        }

        /// <summary>
        /// 不显示群成员（文字提示）
        /// </summary>
        public string HiddenMembers
        {
            get { return hiddenMembers; }
            set
            {
                hiddenMembers = value;
                RaisePropertyChanged(nameof(HiddenMembers));
            }
        }


        /// <summary>
        /// 好友选中Tab(0为好友, 1为黑名单)
        /// </summary>
        public int FriendTypeIndex
        {
            get { return friendTypeIndex; }
            set
            {
                if (value == friendTypeIndex)
                {
                    return;
                }
                friendTypeIndex = value;
                RaisePropertyChanged(nameof(FriendTypeIndex));
                FriendTypeChanged();//选中改变时
            }
        }

        /// <summary>
        /// 消息选中索引
        /// </summary>
        public int MessageListSelectedIndex
        {
            get { return msgSelectedIndex; }
            set
            {
                if (msgSelectedIndex == value)
                {
                    return;
                }
                msgSelectedIndex = value;
                RaisePropertyChanged(nameof(MessageListSelectedIndex));
                OnMessageListSelectionChagned();
            }
        }

        /// <summary>
        /// 当前已登录对象
        /// </summary>
        public DataOfUserDetial Me
        {
            get { return me; }
            set
            {
                me = value;
                RaisePropertyChanged(nameof(Me));
            }
        }

        /// <summary>
        /// 当前聊天会话对象
        /// </summary>
        public Session Sess
        {
            get
            {
                return sess;
            }
            set
            {
                sess = value;
                RaisePropertyChanged(nameof(Sess));
                RaisePropertyChanged(nameof(MainTabSelectedIndex));//控制会话聊天标题显示
            }
        }

        /// <summary>
        /// 动图表情列表
        /// </summary>
        public ObservableCollection<string> GifEmotionList { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Emoji表情列表
        /// </summary>
        public ObservableCollection<string> EmojiEmotionList { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// 好友列表
        /// </summary>
        public ObservableCollection<MessageListItem> FriendList
        {
            get { return friendList; }
            set
            {
                friendList = value;
                RaisePropertyChanged(nameof(FriendList));
            }
        }

        /// <summary>
        /// 黑名单列表
        /// </summary>
        public ObservableCollection<MessageListItem> BlackList
        {
            get { return blackList; }
            set
            {
                blackList = value;
                RaisePropertyChanged(nameof(BlackList));
            }
        }

        /// <summary>
        /// 最近消息列表
        /// </summary>
        public ObservableCollection<MessageListItem> RecentMessageList
        {
            get
            {
                return messageItemList;
            }
            set
            {
                if (messageItemList == value)
                {
                    return;
                }
                messageItemList = value;
                RaisePropertyChanged(nameof(RecentMessageList));
            }
        }

        /// <summary>
        /// 我的房间列表
        /// </summary>
        public ObservableCollection<MessageListItem> MyGroupList
        {
            get { return myGroupList; }
            set
            {
                myGroupList = value;
                RaisePropertyChanged(nameof(MyGroupList));
            }
        }

        /// <summary>
        /// 选中的群成员信息
        /// </summary>
        public ObservableCollection<DataofMember> SelectedGroupMember
        {
            get { return selectedGroupMember; }
            set
            {
                selectedGroupMember = value;
                RaisePropertyChanged(nameof(SelectedGroupMember));
            }
        }

        /// <summary>
        /// 未读总数量
        /// </summary>
        public int TotalUnReadCount
        {
            get
            { return unReadCount; }
            set
            {
                if (value < 0)//未读数量不能为负
                {
                    unReadCount = 0;
                }
                else
                {
                    unReadCount = value;
                }

                RaisePropertyChanged(nameof(TotalUnReadCount));
            }
        }

        /// <summary>
        /// 当前选中的页面
        /// </summary>
        public int MainTabSelectedIndex
        {
            get { return tabSelectedIndex; }
            set
            {
                if (value == tabSelectedIndex)
                {
                    return;
                }
                tabSelectedIndex = value;

                RaisePropertyChanged(nameof(MainTabSelectedIndex));
            }
        }

        /// <summary>
        /// 选中的好友
        /// </summary>
        public DataOfUserDetial SelectedFriend
        {
            get { return selectedFriend; }
            set
            {
                selectedFriend = value;
                RaisePropertyChanged(nameof(SelectedFriend));
            }
        }

        /// <summary>
        /// 群组
        /// </summary>
        public Room DisplayGroup
        {
            get { return displayGroup; }
            set
            {
                displayGroup = value;
                RaisePropertyChanged(nameof(DisplayGroup));
            }
        }
        /// <summary>
        /// 右侧群聊的按钮内容
        /// </summary>
        public string GroupBtnContent
        {
            get { return groupBtnContent; }
            set
            {
                groupBtnContent = value;
                RaisePropertyChanged(nameof(GroupBtnContent));
            }
        }
        public Room RoomDetail
        {
            get { return roomDetail; }
            set
            {
                roomDetail = value;
                RaisePropertyChanged(nameof(RoomDetail));
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand => new RelayCommand(() => { Application.Current.MainWindow.Hide(); });

        /// <summary>
        /// 最大化
        /// </summary>
        public ICommand MaxCommand => new RelayCommand(() =>
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        });

        /// <summary>
        /// 最小化命令
        /// </summary>
        public ICommand MinCommand => new RelayCommand(() => { Application.Current.MainWindow.Hide(); });

        /// <summary>
        /// 消息删除点击
        /// </summary>
        public ICommand MsgItemDeleteCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RemoveMessageListItem();
                });
            }
        }
        /// <summary>
        /// 查看好友详情点击
        /// </summary>
        public ICommand FriendDetailCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    string UserId = "";
                    if (FriendTypeIndex == 0)
                    {
                        UserId = FriendList[FriendSelectedIndex].Jid;
                    }
                    else if (FriendTypeIndex == 1)
                    {
                        UserId = BlackList[BlackSelectedIndex].Jid;
                    }
                    Messenger.Default.Send(UserId, UserDetailNotifications.ShowUserDetial);
                    UserDetailView.GetWindow().Show();
                });
            }
        }
        /// <summary>
        /// 删除好友点击
        /// </summary>
        public ICommand FriendDeleteCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var item = new MessageListItem();
                    if (item.Jid == "10000")
                    {
                        Snackbar.Enqueue("不能删除客服公众号!");
                        return;
                    }
                    if (FriendTypeIndex == 0)
                    {
                        item = FriendList[FriendSelectedIndex];
                    }
                    else
                    {
                        item = BlackList[BlackSelectedIndex];
                    }
                    FriendDelete(item);
                });
            }
        }
        /// <summary>
        /// 黑名单点击
        /// </summary>
        public ICommand AddToBlackCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var friItem = FriendList[FriendSelectedIndex];
                    if (friItem.Jid == "10000")
                    {
                        Snackbar.Enqueue("不能拉黑系统账号!");
                        return;
                    }
                    BlackListAdd(friItem);
                });
            }
        }
        /// <summary>
        /// 黑名单点击
        /// </summary>
        public ICommand BlackListRemoveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var user = BlackList[BlackSelectedIndex];
                    DeBlack_Click(user);
                });
            }
        }
        /// <summary>
        /// 群聊发送消息
        /// </summary>
        public ICommand GroupSendMessageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MainTabSelectedIndex == 2)
                    {
                        var item = MyGroupList[MyGroupSelectedIndex];//获取项
                        MainTabSelectedIndex = 0;//转到消息界面
                        StartNewChatFromItem(item.Clone());//跳转聊天
                    }
                });
            }
        }
        /// <summary>
        /// 群聊详情点击
        /// </summary>
        public ICommand GroupDetailCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    string RoomId = MyGroupList[MyGroupSelectedIndex].Id;
                    GroupChatDetial.GetWindow().Show();
                    Messenger.Default.Send(RoomId, GroupDetialViewModel.InitialGroupDetial);//初始化群组
                    GroupChatDetial.GetWindow().Activate();
                });
            }
        }
        /// <summary>
        /// 退出群聊点击
        /// </summary>
        public ICommand GroupExitCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ExitGroup(MyGroupList[MyGroupSelectedIndex]);
                });
            }
        }
        /// <summary>
        /// 主窗口添加按钮点击
        /// </summary>
        public ICommand MainAddCommand
        {
            get
            {
                return new RelayCommand(() =>
                {

                    if (MainTabSelectedIndex == 1)
                    {
                        AccountQuery.GetWindow().Show();
                    }
                    else
                    {
                        ServiceLocator.Current.GetInstance<GroupCreateViewModel>().InitialProperties();//重置属性
                        GroupCreate.GetGroupCreate().Show();//显示添加群聊的窗口
                    }
                });
            }
        }

        /// <summary>
        /// 发送聊天消息命令
        /// </summary>
        public ICommand SendTextCommand => new RelayCommand(() =>
         {
             var main = Applicate.GetWindow<MainWindow>();
             main.rtb_sendMessage.ThisRichText.Focusable = true;
             string objectId = "";
             string inputText = "";
             //bool isFile = false;

             try
             {
                 foreach (var item in TextFieldDocument.Blocks)
                 {
                     if (item.GetType().Name == "BlockUIContainer")
                     {
                         inputText = GetObjectFromBlockUIContainer((BlockUIContainer)item).Tag.ToString();
                         OnSendMessage(inputText, "", true);
                         //Thread.Sleep(500);
                     }
                     else
                     {
                         inputText = ConvertParagraphToText((Paragraph)item, ref objectId);
                         OnSendMessage(inputText, objectId, false);
                     }
                 }

                 //清理文本框文本
                 TextFieldDocument.Blocks.Clear();
                 var emptypara = new Paragraph(new Run(""));
                 TextFieldDocument.Blocks.Add(emptypara);
                 main.rtb_sendMessage.ThisRichText.Focus();
             }
             catch (Exception ex)
             {
                 Snackbar.Enqueue("提示：图片和文字间请换行！");
             }             
         });

        private void OnSendMessage(string strMessage, string objectId, bool isFile)
        {
            var main = Applicate.GetWindow<MainWindow>();
            main.rtb_sendMessage.ThisRichText.Focusable = true;
            if (string.IsNullOrEmpty(strMessage))
            {
                main.rtb_sendMessage.ThisRichText.Focus();
                //Snackbar.Enqueue("不能发送空白消息");
                return;
            }
            else
            {
                if (Sess.Jid == null)
                {
                    Snackbar.Enqueue("请选择一位聊天对象");
                    return;
                }
                if (CheckIsBanned(Sess.Jid))//禁言
                {
                    return;
                }

                if (isFile)
                {
                    ShiKuManager.SendMessageFile(new MessageListItem
                    {
                        Jid = Sess.Jid,
                        ShowTitle = Sess.NickName,
                        MessageItemContent = Sess.MyMemberNickname,
                        Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Sess.Jid)
                    }, strMessage);
                }
                else
                {
                    ShiKuManager.SendText(new MessageListItem
                    {
                        Jid = Sess.Jid,
                        ShowTitle = Sess.NickName,
                        MessageItemContent = Sess.MyMemberNickname,
                        Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Sess.Jid)
                    }, strMessage, objectId);//发送
                }
            }
        }

        /// <summary>
        /// 富文本框 ctrl+V
        /// </summary>
        public ICommand KeyOfPasteCommand => new RelayCommand(() =>
        {
            System.Windows.MessageBox.Show("ctrl+v");
        });

        /// <summary>
        /// 发送聊天消息点击命令
        /// </summary>
        public ICommand SessionTitleCommand => new RelayCommand(() =>
        {
            if (Sess.Jid.Length > 15)
            {
                var tmpRoom = new Room().GetRoomIdByJid(Sess.Jid);//获取
                GroupChatDetial.GetWindow().Show();
                Messenger.Default.Send(tmpRoom, GroupDetialViewModel.InitialGroupDetial);//初始化群组
                GroupChatDetial.GetWindow().Activate();
            }
            else
            {
                if (Sess.Jid == "10001")
                {
                    //新的好友不显示个人资料
                }
                else
                {
                    Messenger.Default.Send(Sess.Jid, UserDetailNotifications.ShowUserDetial);
                    UserDetailView.GetWindow().Show();
                }
            }
        });

        /// <summary>
        /// 查看用户详情命令
        /// </summary>
        public ICommand UserDetailCommand => new RelayCommand<string>((userId) =>
        {
            if (userId == Applicate.MyAccount.userId)
            {
                DataOfUserDetial._avatarName = FileUtil.GetFileName(FileUtil.GetDirFiles(LocalConfig.userAvatorPath,userId),false);
                Messenger.Default.Send(true, UserDetailNotifications.ShowMyDetial);//显示个人详情
                Personal.GetPersonal().Show();
            }
            else
            {
                Messenger.Default.Send(userId, UserDetailNotifications.ShowUserDetial);
                UserDetailView.GetWindow().Show();
            }
        });

        /// <summary>
        /// 查看群验证命令
        /// </summary>
        public ICommand RoomVerifyCommand => new RelayCommand<Messageobject>((message) =>
        {
            var msg = new Messageobject()
            {
                messageId = message.messageId,
                fromUserId = message.fromUserId,
                FromId = message.FromId,
                toUserId = message.toUserId,
                ToId = message.ToId,
            }.GetModel();
            if (msg != null)
            {
                new RoomVerifyForm(msg).ShowDialog();
            }
        });

        /// <summary>
        /// 发送文件命令
        /// </summary>
        public ICommand SendFileCommand => new RelayCommand(() =>
        {
            if (CheckIsBanned(Sess.Jid))//禁言
            {
                return;
            }
            //文件按钮先选中并打开一个文件进行上传操作
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = true;//可多选
            fd.Filter = "文件|*.*|图片|*.jpg;*.png;*.jpeg;*.bmp";//文件筛选器
            fd.ShowDialog();
            //是否为
            if (fd.FileNames.Length > 0)
            {
                foreach (var fileName in fd.FileNames)
                {
                    ConsoleLog.Output("////******************************选中的文件为" + fileName);
                    //异步发送消息
                    ShiKuManager.SendMessageFile(new MessageListItem
                    {
                        Jid = Sess.Jid,
                        ShowTitle = Sess.NickName,
                        MessageItemContent = Sess.MyMemberNickname,
                        Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Sess.Jid)
                    }, fileName);
                }
            }
        });

        /// <summary>
        /// 对话框中发送按钮命令
        /// </summary>
        public ICommand DialogSendCommand => new RelayCommand<object>((para) =>
        {
            DialogHost.CloseDialogCommand.Execute(null, null);//首先关闭对话框
            var selectedtmps = new List<MessageListItem>();
            for (int i = 0; i < SelectedContacts.Count; i++)
            {
                selectedtmps.Add((MessageListItem)SelectedContacts[i]);
            }
            if (SelectedContacts.Count > 0)//非空验证
            {
                SelectedContacts.Clear();//恢复
                var msgtype = kWCMessageType.VideoMeetingInvite;//默认为视频聊天
                switch (DialogFunctionType)
                {
                    case DialogFunction.SendContact://发送联系人
                        ShiKuManager.SendContacts(new MessageListItem
                        {
                            Jid = Sess.Jid,
                            ShowTitle = Sess.NickName,
                            MessageItemContent = Sess.MyMemberNickname,
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Sess.Jid)
                        }, selectedtmps);
                        break;
                    case DialogFunction.ForwardMessage://转发消息
                        ShiKuManager.ForwardMessage(Applicate.ForwardMessageList, selectedtmps);
                        break;
                    case DialogFunction.AttentionGroupMember://@群成员
                        ShiKuManager.SendAttentionMsg(selectedtmps, new MessageListItem
                        {
                            Jid = Sess.Jid,
                            ShowTitle = Sess.NickName,
                            MessageItemContent = Sess.MyMemberNickname,
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Sess.Jid)
                        });
                        TextFieldDocument.Blocks.Clear();
                        var emptypara = new Paragraph(new Run(""));
                        TextFieldDocument.Blocks.Add(emptypara);
                        break;
                    case DialogFunction.AudioChatWithGroupMember://群组音频聊天
                    case DialogFunction.VideoChatWithGroupMember://群组视频聊天
                        var tmpRoom = new Room
                        {
                            jid = Sess.Jid,
                            id = Sess.RoomId,
                            name = Sess.NickName,
                            members = new ObservableCollection<DataofMember>(),
                        };
                        foreach (var item in selectedtmps)
                        {
                            tmpRoom.members.Add(new DataofMember { userId = item.Jid, nickname = item.ShowTitle });
                        }
                        if (DialogFunctionType == DialogFunction.AudioChatWithGroupMember)//根据对话框类型判断消息类型
                        {
                            //msgtype = kWCMessageType.AudioMeetingInvite;
                            //发起语音会议
                            for (int i = 0; i < selectedtmps.Count; i++)
                            {
                                ShiKuManager.SendAudioMeetingAsk(Sess.RoomId, Sess.Jid, selectedtmps[i].Jid, selectedtmps[i].MessageTitle);//发送邀请
                            }
                            selectedtmps[0].Id = Sess.Jid;//设置群聊JID
                            Messenger.Default.Send(tmpRoom, CommonNotifications.AudioChatRequest);
                        }
                        else
                        {
                            //发起视频会议
                            for (int i = 0; i < selectedtmps.Count; i++)
                            {
                                ShiKuManager.SendVideoMeetingAsk(Sess.RoomId, Sess.Jid, selectedtmps[i].Jid, selectedtmps[i].ShowTitle);//发送邀请
                            }
                            Messenger.Default.Send(tmpRoom, CommonNotifications.VideoChatRequest);
                        }
                        break;
                    default:
                        break;
                }
                DialogContacts.Clear();//清理临时对话框中好友
            }
            else
            {
                Snackbar.Enqueue("请至少选择一个用户");//异常提示
            }
        });

        /// <summary>
        /// 显示或隐藏联系人列表
        /// </summary>
        public ICommand ShowContactCommand => new RelayCommand(() =>
        {
            if (CheckIsBanned(Sess.Jid, true))//检查禁言
            {
                return;
            }
            var SendCards = App.Current.MainWindow.FindResource("DialogContactCard");//获取对话框
            DialogHost.OpenDialogCommand.Execute(SendCards, null);//打开发送联系人对话框
            DialogContacts.Clear();
            DialogFunctionType = DialogFunction.SendContact;//设置为发送名片
            DialogContacts.AddRange(FriendList);//添加好友至联系人预览集合
        });

        /// <summary>
        /// 历史记录命令
        /// </summary>
        public ICommand ChatHistoryCommand => new RelayCommand(() =>
        {
            if (Sess.Jid == null)
            {
                //Snackbar.Enqueue("必须选择一位聊天对象才能聊天");
                return;
            }
            ChatRecord.ShowChatRecord(Sess.Jid).Show();
        });

        /// <summary>
        /// 发红包
        /// </summary>
        public ICommand SendRedPacketCommand => new RelayCommand(() =>
        {
            if (CheckIsBanned(Sess.Jid, true))//检查禁言
            {
                return;
            }
            var SendCards = App.Current.MainWindow.FindResource("DialogContactCard");//获取对话框
            DialogHost.OpenDialogCommand.Execute(SendCards, null);//打开发送联系人对话框
            DialogContacts.Clear();
            DialogFunctionType = DialogFunction.SendContact;//设置为发送名片
            DialogContacts.AddRange(FriendList);//添加好友至联系人预览集合
        });

        /// <summary>
        /// 聊天框内拖入命令
        /// </summary>
        public ICommand ChatDragDropCommand => new RelayCommand(() =>
        {
            SendFileTooltipVisible = false;
            SendImageTooltipVisible = true;
        });

        /// <summary>
        /// 标记所有消息为已读
        /// </summary>
        public ICommand AllRecentUnReadCommand => new RelayCommand(() =>
        {
            TotalUnReadCount = 0;
            //RecentMessageList.
        });

        /// <summary>
        /// 聊天框内拖入并悬浮命令
        /// </summary>
        public ICommand ChatDragEnterCommand => new RelayCommand<System.Windows.DragEventArgs>((eve) =>
        {
            if (eve.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[])eve.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (paths != null && paths.Length != 0)
                {
                    if (!paths[0].Contains('.'))//如果不是文件不操作
                    {
                        return;
                    }
                    if (paths.Length > 1)//发送多个文件只能以文件方式发送
                    {
                        SendFileTooltipVisible = true;
                        SendImageTooltipVisible = false;
                        foreach (var item in paths)
                        {
                            ShiKuManager.SendMessageFile(new MessageListItem
                            {
                                Jid = Sess.Jid,
                                ShowTitle = Sess.NickName,
                                MessageItemContent = Sess.MyMemberNickname
                            }, item);
                        }
                    }
                    else//单个文件时
                    {
                        string FileType = paths[0].Substring(paths[0].LastIndexOf('.'), paths[0].Length - paths[0].LastIndexOf('.'));
                        switch (FileType)
                        {
                            case ".png":
                            case ".jpg":
                            case ".jpeg":
                            case ".bmp":
                            case ".gif":
                                SendImageTooltipVisible = true;
                                SendFileTooltipVisible = false;
                                break;
                            default:
                                SendImageTooltipVisible = false;
                                SendFileTooltipVisible = true;
                                break;
                        }

                    }
                }
            }
        });

        /// <summary>
        /// 拖入文件卡片提示放开后
        /// </summary>
        public ICommand FileToolTipCardDropCommand => new RelayCommand<System.Windows.DragEventArgs>((e) =>
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (!paths[0].Contains('.'))//如果不是文件不操作
                {
                    return;
                }
                SendFileTooltipVisible = false;
                SendImageTooltipVisible = false;
                ShiKuManager.SendMessageFile(new MessageListItem
                {
                    Jid = Sess.Jid,
                    ShowTitle = Sess.NickName,
                    MessageItemContent = Sess.MyMemberNickname
                }, paths[0]);//直接发送图片
            }
        });

        /// <summary>
        /// 拖入图片卡片提示放开后
        /// </summary>
        public ICommand ImageToolTipCardDropCommand => new RelayCommand<System.Windows.DragEventArgs>((e) =>
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (!paths[0].Contains('.'))//如果不是文件不操作
                {
                    return;
                }
                SendFileTooltipVisible = false;
                SendImageTooltipVisible = false;
                ShiKuManager.SendMessageFile(new MessageListItem
                {
                    Jid = Sess.Jid,
                    ShowTitle = Sess.NickName,
                    MessageItemContent = Sess.MyMemberNickname
                }, paths[0]);//直接发送图片
            }
        });


        /// <summary>
        /// 
        /// </summary>
        public ICommand SelectedMessageItemChangedCommand { get; set; }

        /// <summary>
        /// 设置命令-->打开设置窗口
        /// </summary>
        public ICommand SettingCommand => new RelayCommand(() => { SettingsWindow.getSetUp().Show(); });

        /// <summary>
        /// 聊天(双击/Enter键/发消息键进入)
        /// </summary>
        public ICommand GoToChatFromItemCommand => new RelayCommand(() =>
        {
            //声明选中项
            MessageListItem item = null;
            //判断
            if (MainTabSelectedIndex == 1)
            {
                if (FriendTypeIndex == 0)
                {
                    item = FriendList[FriendSelectedIndex];//获取好友信息
                }
                else
                {
                    ConsoleLog.Output("进入了错误的命令--不允许聊天");
                    return;
                }
            }
            else
            {
                item = MyGroupList[MyGroupSelectedIndex];//获取群聊信息
            }
            StartNewChatFromItem(item.Clone());//根据MsgItem新建聊天
        });

        /// <summary>
        /// 聊天(双击/Enter键/发消息键进入)
        /// </summary>
        public ICommand EmojiShowCommand => new RelayCommand<FrameworkElement>((emojiView) =>
        {
            emojiView.Focus();
        });

        /// <summary>
        /// 添加Emoji表情到文本框中
        /// </summary>
        public ICommand AddEmojiToTxtCommand { get; set; }

        /// <summary>
        /// 发送Gif表情命令
        /// </summary>
        public ICommand SendGifCommand => new RelayCommand<string>((emoji) =>
        {
            if (CheckIsBanned(Sess.Jid))//禁止会议
            {
                return;
            }
            var text = emoji.Substring(emoji.LastIndexOf('\\') + 1, emoji.Length - emoji.LastIndexOf('\\') - 1);
            ShiKuManager.SendGifMessage(new MessageListItem
            {
                Jid = Sess.Jid,
                ShowTitle = Sess.NickName,
                MessageItemContent = Sess.MyMemberNickname
            }, text);
        });

        /// <summary>
        /// 重新连接Xmpp
        /// </summary>
        public ICommand ReConnectionXmpp => new RelayCommand(() =>
        {
            Task.Run(() =>
            {
                if (ShiKuManager.xmpp.XmppCon.XmppConnectionState != XmppConnectionState.SessionStarted)
                {
                    ShiKuManager.xmpp.XmppCon.Open();
                }
            });
        });

        #endregion

        #region Contructor
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            InitialRecentList();//添加化最近的消息
            InitialEmotion();//初始化表情
            #region Initial Commands
            SelectedMessageItemChangedCommand = new RelayCommand(SelectedMessageItemChanged);
            AddEmojiToTxtCommand = new RelayCommand<string>(AddEmojiToTextbox);
            #endregion
            //SetTotalUnReadCount();//设置未读角标
            #region Initial MessageItemList
            if (RecentMessageList.Count(m => m.Jid == "10001") == 0)
            {
                var newFriendItem = new MessageListItem()
                {
                    ShowTitle = "新的朋友",
                    MessageTitle = "新的朋友",
                    Jid = "10001",
                    MessageItemType = ItemType.VerifyMsg,
                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath("10001")
                };
                RecentMessageList.Add(newFriendItem);
                newFriendItem.Insert();
            }
            #endregion
            RegisterMessengers();//注册通知
            ShiKuManager.InitialPlatformList();//初始化平台设备列表
            soundPlayer = new SoundPlayer();
            soundPlayer.Stream = ShikuRec.Windows_Notify_Messaging;//声音
            DialogContacts = new ObservableCollection<MessageListItem>();
            TypingTimer.Interval = 30000;//显示30秒正在输入
            TypingTimer.AutoReset = false;//只运行一次
            TypingTimer.Elapsed += (sen, eve) =>
            {
                Sess.IsOnlineText = "在线";//30
            };
        }
        #endregion

        #region 注册通知
        /// <summary>
        /// 注册通知  (注释可将鼠标移至Notifaction属性上查看)    
        /// </summary>
        private void RegisterMessengers()
        {
            //注册添加最近消息项
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.MainAddRecentItem, item => AddSingleMessageList(item));
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.MainInsertRecentItem, item => InsertOrTopSingleMessageList(item));
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.UpdateAccountName, remarkitem => OnUserNameChanged(remarkitem));
            Messenger.Default.Register<int>(this, MainViewNotifactions.MainChangeRecentListIndex, index => MessageListSelectedIndex = index);
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.MainAddFriendListItem, item => AddToFriendList(item));
            Messenger.Default.Register<int>(this, MainViewNotifactions.MainGoToPage, index => MainTabSelectedIndex = index);
            Messenger.Default.Register<string>(this, MainViewNotifactions.SnacbarMessage, smsg => Snackbar.Enqueue(smsg));//提示消息
            Messenger.Default.Register<Messageobject>(this, MainViewNotifactions.CreateOrUpdateRecentItem, msg => CreateOrUpdateMsgItem(msg));//创建或更新最近消息项目
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.UpdateRecentItemContent, (item) => { UpdateRecentItemContent(item); });
            Messenger.Default.Register<XmppConnectionState>(this, MainViewNotifactions.XmppConnectionStateChanged, (state) => { SetXmppState(state); });
            Messenger.Default.Register<bool>(this, MainViewNotifactions.InputTextChanged, (item) => { InputTextChanged(); });
            Messenger.Default.Register<DataOfUserDetial>(this, CommonNotifications.UpdateMyAccountDetail, (detial) => { UpdateMe(detial); });
            Messenger.Default.Register<Room>(this, CommonNotifications.AddGroupMemberSize, (room) =>
            {
                if (Sess.Jid == room.jid)//设置标题
                {
                    SetChatTitle(room.jid, room.name);
                }
                if (MyGroupSelectedIndex >= 0)
                {
                    if (MyGroupList[MyGroupSelectedIndex].Jid == room.jid)//更新选中群组的右侧详情
                    {
                        DisplayGroup = room;//更新页面数据
                        SelectedGroupMember.Clear();
                        SelectedGroupMember.AddRange(room.members);//重新添加
                    }
                }
            });
            Messenger.Default.Register<Dictionary<string, List<DataofMember>>>(this, CommonNotifications.RemoveGroupMember, (members) =>
            {
                var newitems = members.First();//获取对应的成员数据
                if (Sess.Jid == newitems.Key)//如果会话存在对应的
                {
                    SetChatTitle(newitems.Key, "");//更新标题
                }
                if (MyGroupSelectedIndex >= 0)
                {
                    if (MyGroupList[myGroupSelectedIndex].Jid == newitems.Key)//当前选中对象匹配
                    {
                        var tmpitem = SelectedGroupMember.FirstOrDefault(m => m.userId == newitems.Value[0].userId);//显示
                        if (SelectedGroupMember.Count(dm => dm.userId == tmpitem.userId) > 0)//如果存在对应成员的话
                        {
                            SelectedGroupMember.Remove(tmpitem);//删除群成员
                        }
                    }
                }
            });
            Messenger.Default.Register<string>(this, MainViewNotifactions.MainRemoveGroupItem, jid => RemoveFromGroupList(jid));
            Messenger.Default.Register<MessageListItem>(this, CommonNotifications.UpdateGroupMemberNickname, item =>
            {
                if (Sess.Jid.Length > 10)//如果当前是与群组会话
                {
                    if (item.Jid == Applicate.MyAccount.userId)
                    {
                        Sess.MyMemberNickname = item.ShowTitle;//更新自己在群组中的昵称
                    }
                }
            });
            ////加载好友/黑名单/群组列表通知
            Messenger.Default.Register<bool>(this, MainViewNotifactions.MainViewLoadFriendList, res =>
            {
                Task.Run(() =>
                {
                    /*
                    string[] platforms = new string[] { "android", "ios", "mac", "web" };
                    FriendList.Clear();
                    //插入数据库
                    for (int i = 0; i < platforms.Length; i++)
                    { 
                        new DataOfFriends { isDevice = true, toUserId = Applicate.MyAccount.userId + platforms[i] }.AutoInsert();
                    }
                    //Add platformList
                    App.Current.Dispatcher.Invoke(() =>
                    {

                    });*/
                    if (res)//if true, load from localdatabase
                    {
                        LoadFriendsByDb();
                    }
                    else
                    {
                        LoadFriendsByApi();
                    }
                });
            });
            Messenger.Default.Register<bool>(this, MainViewNotifactions.MainViewLoadBlockList, res =>
            {
                Task.Run(() =>
                {
                    if (res)
                    {
                        LoadBlockListByDb();
                    }
                    else
                    {
                        LoadBlockListByApi();
                    }
                });
            });
            Messenger.Default.Register<bool>(this, MainViewNotifactions.MainViewLoadGroupList, res =>
            {
                Task.Run(() =>
                {
                    if (res)
                    {
                        LoadJoinedRoomsByDb();
                    }
                    else
                    {
                        LoadJoinedRoomByApi();
                    }
                });
            });
            Messenger.Default.Register<Messageobject>(this, MainViewNotifactions.ForwardMessage, msg =>
            {
                DialogContacts.Clear();//以防万一清除数据
                //添加所有联系人
                DialogContacts.AddRange(FriendList);//添加好友列表
                DialogContacts.AddRange(MyGroupList);//添加群组列表
                ConsoleLog.Output(DialogContacts.Count);
                var contacts = App.Current.MainWindow.FindResource("DialogContactCard");//获取转发联系人列表控件
                DialogHost.OpenDialogCommand.Execute(contacts, null);
                Applicate.ForwardMessageList.Clear();
                Applicate.ForwardMessageList.Add(msg);//添加转发消息实体
                DialogFunctionType = DialogFunction.ForwardMessage;//设置为发送
            });
            Messenger.Default.Register<Messageobject>(this, MainViewNotifactions.WithDrawMessage, msg => { WithDrawMsg(msg); });
            Messenger.Default.Register<Room>(this, CommonNotifications.CreateNewGroupFinished, room => OnCreateNewGroup(room));//
            Messenger.Default.Register<string>(this, MainViewNotifactions.CancelBlockItem, item => OnCancelBlack(item));
            ////Xmpp通知注册
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgRecived, msg => { ProcessNewMessage(msg); });//收到消息
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgReceipt, (msg) => MsgReceipt(msg));//回执
        }
        #endregion

        #region 从群组中删除
        /// <summary>
        /// 从群组中删除
        /// </summary>
        /// <param name="jid">对应的Jid</param>
        private void RemoveFromGroupList(string jid)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                DeleteFromMessageList(jid);//删除消息列表项
                if (DisplayGroup != null && DisplayGroup.jid == jid)//
                {
                    RoomInfoVisible = false;
                }
                if (MyGroupList.Count(g => g.Jid == jid) > 0)
                {
                    var item = MyGroupList.FirstOrDefault(g => g.Jid == jid);
                    MyGroupList.Remove(item);
                }
            });
        }
        #endregion

        #region 更新
        private void UpdateMe(DataOfUserDetial detial)
        {
            if (Me == null)
            {
                Me = new DataOfUserDetial();
            }
            LogHelper.log.Debug("======================更新详情与下载保存");
            Me.nickname = detial.nickname;
            Me.userId = detial.userId;
            Me.active = detial.active;
            Me.areaCode = detial.areaCode;
            Me.birthday = detial.birthday;
            Me.areaId = detial.areaId;
            Me.attCount = detial.attCount;
            Me.name = detial.name;
            #region 下载当前用户头像
            var avator = new List<DownLoadFile>();
            avator.Add(new DownLoadFile
            {
                Jid = detial.userId,
                LocalUrl = Applicate.LocalConfigData.GetDownloadAvatorPath(detial.userId),
                ShouldDeleteWhileFileExists = true,
            });
            HttpDownloader.Download(avator, (complete) =>
            {
                Me.userId = complete.Jid;
            });
            #endregion
            Applicate.MyAccount = detial;
        }
        #endregion

        #region 聊天输入框内容改变
        /// <summary>
        /// 聊天输入框内容改变
        /// </summary>
        private void InputTextChanged()
        {

            #region @群成员
            if (Sess.isGroup)//如果不是群组则不做操作
            {
                string objectId = "";
                string texts = ConvertDocumentToText(TextFieldDocument, ref objectId);
                if (texts.Length == 0)//
                {
                    return;
                }
                Task.Run(() =>
                {
                    string tmp = texts.Substring(texts.Length - 1, 1);
                    if (tmp == "@")
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            var panel = App.Current.MainWindow.FindResource("DialogContactCard");//获取对话框
                            DataofMember member = new DataofMember();
                            string roomid = new Room().GetRoomIdByJid(Sess.Jid);
                            var lists = member.GetListByRoomId(roomid);
                            DialogContacts.Clear();//清除上次存在的联系人
                            for (int i = 0; i < lists.Count; i++)
                            {
                                DialogContacts.Add(lists[i].ToMsgItem());//添加群成员
                            }
                            DialogHost.OpenDialogCommand.Execute(panel, null);//显示对话框
                            DialogFunctionType = DialogFunction.AttentionGroupMember;//设置为@群成员
                        });
                    }
                });
            }
            #endregion
        }
        #endregion

        #region 更新最近消息项内容
        /// <summary>
        /// 更新最近消息项内容
        /// </summary>
        /// <param name="item">需要更新的项</param>
        private void UpdateRecentItemContent(MessageListItem item)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (RecentMessageList.Count(r => r.Jid == item.Jid) > 0)//确认项存在
                {
                    var tmpitem = RecentMessageList.FirstOrDefault(r => r.Jid == item.Jid);
                    tmpitem.Msg = new Messageobject { type = kWCMessageType.Text, content = item.MessageItemContent };//更新内容
                }
            });
        }
        #endregion

        #region 撤回消息
        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="msg">消息</param>
        private void WithDrawMsg(Messageobject msg)
        {
            var client = ShiKuManager.WithDrawMsg(msg.jid, msg.messageId, msg.isGroup);//撤回消息
            client.UploadDataCompleted += (send, eve) =>
            {
                if (eve.Error == null)
                {
                    msg.UpdateMessageType(msg.messageId, kWCMessageType.Withdraw);//更新数据库消息状态
                    string tcontent = "您撤回了一条消息";
                    msg.UpdateMessageContent(msg.messageId, tcontent);//更新为
                    msg.type = kWCMessageType.Withdraw;//设为撤回
                    Messenger.Default.Send(msg, ChatBubblesNotifications.WithDrawSingleMessage);//通知气泡列表更新
                    if (RecentMessageList.Any(r => r.Jid == msg.jid))
                    {
                        var item = RecentMessageList.FirstOrDefault(r => r.Jid == msg.jid);
                        item.Msg = msg;//更新左侧最近聊天消息
                        item.Message = msg.ToJson();//更新左侧最近聊天消息
                    }
                }
                else
                {
                    Snackbar.Enqueue("撤回失败：" + eve.Error.Message);
                }
            };
        }
        #endregion

        #region 插入最近联系人中第一项
        /// <summary>
        /// 插入最近联系人中第一项(置顶)
        /// </summary>
        /// <param name="item"></param>
        public void InsertOrTopSingleMessageList(MessageListItem item)
        {
            var exists = RecentMessageList.FirstOrDefault(r => r.Jid == item.Jid);

            if (exists != null)
            {
                if (RecentMessageList.IndexOf(exists) == 0)//如果已经置顶则不操作
                {
                    return;
                }
                RecentMessageList.Remove(exists);//移除UI
                Task.Run(() =>
                {
                    item.Update();//更新最近联系人至数据库
                });
            }
            else
            {
                Task.Run(() =>
                {
                    item.Insert();//存入最近联系人至数据库
                });
            }
            RecentMessageList.Insert(0, item);//插入第一条
        }
        #endregion

        #region Load RecentMsgItemList
        /// <summary>
        /// 加载最近的消息
        /// </summary>
        private void InitialRecentList()
        {
            var db = new MessageListItem();
            var list = db.GetAllList();
            if (list.Count > 0)
            {
                RecentMessageList.AddRange(list.ToObservableCollection());
            }
        }
        #endregion

        #region 初始化表情
        /// <summary>
        /// 初始化表情
        /// </summary>
        private void InitialEmotion()
        {
            //Load the Gif Emotion
            //Task.Run(() =>
            //{
            //Thread.Sleep(4000);
            var gifList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Resource\\Gif\\");
            var emojiList = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Resource\\Emoji\\");
            for (int i = 0; i < gifList.Length; i++)
            {
                lock (GifEmotionList)
                {
                    //Thread.Sleep(60);
                    //App.Current.Dispatcher.InvokeAsync(() =>
                    //{
                    GifEmotionList.Add(gifList[i]);
                    //});
                }
            }
            for (int i = 0; i < emojiList.Length; i++)
            {
                lock (EmojiEmotionList)
                {
                    //Thread.Sleep(60);
                    //App.Current.Dispatcher.InvokeAsync(() =>
                    //{
                    EmojiEmotionList.Add(emojiList[i]);
                    //});
                }
            }
            //});
        }
        #endregion

        #region 添加Emoji表情到文本框
        /// <summary>
        ///  添加Emoji到表情
        /// </summary>
        /// <param name="obj"></param>
        private void AddEmojiToTextbox(string obj)
        {
            var main = Applicate.GetWindow<MainWindow>();
            Image img = new Image();
            img.Width = 24;
            img.Height = 24;
            img.Source = new BitmapImage(new System.Uri(obj));
            int start = obj.LastIndexOf('\\') + 1;
            int end = obj.LastIndexOf('.') - start;
            img.Tag = $"[{obj.Substring(start, end)}]";
            App.Current.Dispatcher.Invoke(() =>
            {
                new InlineUIContainer(img, main.rtb_sendMessage.ThisRichText.Selection.Start);
                //将光标至于所有发送框末尾
                main.rtb_sendMessage.ThisRichText.CaretPosition = TextFieldDocument.Blocks.LastBlock.ContentEnd;
            });
            //TextFieldDocument.Blocks.Add(new Block())
        }
        #endregion

        #region 选中消息项改变时
        /// <summary>
        /// 选中消息项改变时
        /// </summary>
        private void SelectedMessageItemChanged()
        {
            if (MessageListSelectedIndex < 0)
            {
                return;
            }
            MessageListItem item = RecentMessageList[MessageListSelectedIndex];
            if (!string.IsNullOrWhiteSpace(item.Jid) && item.Jid == Sess.Jid)
            {
                return;
            }
            TotalUnReadCount -= RecentMessageList[MessageListSelectedIndex].UnReadCount;//减去对应未读消息数量
            RecentMessageList[MessageListSelectedIndex].UnReadCount = 0;//未读数量为0
                                                                        //清理块
            TextFieldDocument.Blocks.Clear();
            var emptypara = new Paragraph(new Run(""));
            TextFieldDocument.Blocks.Add(emptypara);
            //rtf_sendMessage.Focus();//聚焦到富文本框中
            if (MessageListSelectedIndex < 0)
            {
                return;
            }
            MsgItemMouseDown(item);
        }
        #endregion

        #region 播放媒体
        /// <summary>
        /// 播放媒体(建议音频)
        /// </summary>
        /// <param name="fileName">媒体路径</param>
        public void PlayMedia(string fileName)
        {
            Applicate.GetWindow<MainWindow>().VlcPlayer.SourceProvider.MediaPlayer.Play();
        }
        #endregion

        #region 暂停媒体
        /// <summary>
        /// 播放媒体(建议音频)
        /// </summary>
        public void PauseMedia()
        {
            Applicate.GetWindow<MainWindow>().VlcPlayer.SourceProvider.MediaPlayer.Pause();
        }
        #endregion

        #region 设置总未读数量
        public void SetTotalUnReadCount(int count = -1)
        {
            if (count == -1)
            {
                count = 0;
                foreach (var item in RecentMessageList)
                {
                    if (item.UnReadCount > 0)
                    {
                        ConsoleLog.Output("未读数：" + item.Jid + " " + item.UnReadCount);
                    }
                    count += item.UnReadCount;
                }
            }
            this.TotalUnReadCount = count;
        }
        #endregion

        #region 检查发起群聊消息是否被禁言
        /// <summary>
        /// 发起群聊消息前检查是否被禁言
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public bool CheckIsBanned(string jid, bool isSendCard = false)
        {
            if (jid.Length < 20)//普通用户不处理
            {
                return false;
            }
            bool banned = false;//默认为未被禁言用户
            var room = new Room() { jid = jid }.GetByJid();
            if (room != null)
            {
                var memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                if (memlist.Count == 0)
                {
                    APIHelper.GetRoomDetialByRoomId(room.id);
                    memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                }
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId);/*查询出自己的身份编号*/
                if (user != null)
                {
                    int access = (int)user.role;//获取权限
                    var talkTime = Helpers.StampToDatetime(user.talkTime);//获取禁言截至时间
                    if (DateTime.Now < talkTime && access == 3)//被禁言且不为管理层
                    {
                        Snackbar.Enqueue("你已被禁言至" + talkTime.ToString("MM-dd HH:mm:ss"));
                        banned = true;
                    }
                    else if (Helpers.DatetimeToStamp(DateTime.Now) < room.talkTime && access == 3)//全员禁言且不为管理层
                    {
                        Snackbar.Enqueue("群组已开启全体禁言");
                        banned = true;
                    }
                    else if (isSendCard && room.allowSendCard == 0 && access == 3)//不允许发送名片且不为管理层
                    {
                        Snackbar.Enqueue("群组已关闭普通成员私聊功能");
                        banned = true;
                    }
                }
                //else//查询不到用户则为已退群
                //{
                //    Snackbar.Enqueue("你已不在此群组");
                //    banned = true;
                //}
            }
            return banned;
        }
        #endregion

        #region 检查是否关闭群名片
        private bool CheckIsAllowSendCard(string jid)
        {
            if (jid.Length < 20)
            {
                return false;
            }

            bool banned = false;
            var room = new Room() { jid = jid }.GetByJid();
            if (room != null)
            {
                var memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                if (memlist.Count == 0)
                {
                    APIHelper.GetRoomDetialByRoomId(room.id);
                    memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                }
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*查询出自己的身份编号*/);
                if (user != null)
                {
                    int access = (int)user.role;
                    if (room.allowSendCard == 0 && access == 3)
                    {
                        Snackbar.Enqueue("群组已关闭普通成员私聊功能");
                        banned = true;
                    }
                }
                else
                {
                    Snackbar.Enqueue("你已不在此群组");
                    banned = true;

                }
            }

            return banned;
        }
        #endregion

        #region 检查是否关闭群会议
        private bool CheckIsAllowMeeting(string jid)
        {
            if (jid.Length < 20)
            {
                return false;
            }

            bool banned = false;
            var room = new Room() { jid = jid }.GetByJid();
            if (room != null)
            {
                var memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                if (memlist.Count == 0)
                {
                    APIHelper.GetRoomDetialByRoomId(room.id);
                    memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                }
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*查询出自己的身份编号*/);
                if (user != null)
                {
                    int access = (int)user.role;
                    if (room.allowConference == 0 && access == 3)
                    {
                        Snackbar.Enqueue("群组已关闭普通成员发起会议功能");
                        banned = true;
                    }
                }
                else
                {
                    Snackbar.Enqueue("你已不在此群组");
                    banned = true;

                }
            }

            return banned;
        }
        #endregion

        #region 群组列表选中项改变时(修改绑定的群聊详情)
        /// <summary>
        /// 群组列表选中项改变时(修改绑定的群聊详情)
        /// </summary>
        private void MyGroupSelectionChanged()
        {
            Task.Run(() =>
            {
                GC.Collect();
            });
            DisplayGroup = new Room();
            if (MyGroupSelectedIndex >= 0)
            {
                //获取选中的群聊
                var item = MyGroupList[MyGroupSelectedIndex];
                DisplayGroup = new Room() { id = item.Id }.GetByRoomId();
                if (DisplayGroup == null)
                {
                    ReloadRoomDetialBoard(item.Id);
                }
                else if (new JsonRoomMember().data.GetListByRoomId(item.Id).Count == 0)
                {
                    ReloadRoomDetialBoard(item.Id);
                }
                else if (DisplayGroup.showMember == 1 || DisplayGroup.userId.ToString() == Applicate.MyAccount.userId)
                {
                    HiddenMembers = "";
                    GetSelectedGroupMember(item.Id);
                }
                else
                {
                    HiddenMembers = "群主关闭群成员显示";
                    //群主关闭显示群成员
                    SelectedGroupMember.Clear();
                }
                GroupBtnContent = "发消息";
                RoomInfoVisible = true;
            }
        }
        #endregion

        #region ListBoxItem点击事件
        /// <summary>
        /// ListBoxItem点击事件
        /// </summary>
        private void OnListViewItemClick()
        {
            var item = new MessageListItem();
            bool isJoin = false;
            if (MainTabSelectedIndex == 1)
            {
                //根据下标查找到对应的好友或群组对象
                item = FriendList[FriendSelectedIndex].Clone();
                isJoin = true;
            }
            else if (MainTabSelectedIndex == 2)
            {
                item = MyGroupList[MyGroupSelectedIndex].Clone();
                isJoin = true;

            }
            if (isJoin)
            {
                MainTabSelectedIndex = 0;//转到消息界面
                StartNewChatFromItem(item.Clone());//跳转聊天
            }
        }
        #endregion

        #region Initial Lists
        #region 接口加载加入群聊
        /// <summary>
        /// 接口加载加入群聊
        /// </summary>
        public void LoadJoinedRoomByApi()
        {
            var myclient = APIHelper.GetMyRoomsAsync();//加载我加入过的群组
            myclient.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    Task.Run(() =>
                    {
                        string strs = Encoding.UTF8.GetString(e.Result);
                        var rooms = JsonConvert.DeserializeObject<RtnRooms>(strs);
                        if (rooms.data.Count > 0)
                        {
                            AddToGroupList(rooms.data);//添加接口返回数据
                        }
                        var tmproom = new Room();
                        tmproom.DeleteAllList();//删除
                        if (rooms.data != null && rooms.data.Count > 0)
                        {
                            for (int i = 0; i < rooms.data.Count; i++)
                            {
                                rooms.data[i].AutoInsert();
                            }
                        }
                    });
                }
                else
                {
                    Snackbar.Enqueue("获取群聊失败:" + e.Error.Message, "重试", () => { LoadJoinedRoomByApi(); });
                }
            };//存已加入群组
        }
        #endregion

        #region 从集合加载群组列表
        /// <summary>
        /// 从集合加载群组列表
        /// </summary>
        /// <param name="data">群组集合</param>
        public void AddToGroupList(List<Room> data)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < data.Count; i++)
                {
                    MyGroupList.Add(data[i].ToMsgItem());
                }
            });
        }
        #endregion

        #region 本地数据库加载加入群聊
        /// <summary>
        /// 从数据库中加载已进入群聊列表
        /// </summary>
        public void LoadJoinedRoomsByDb()
        {
            Task.Run(() =>
            {
                MyGroupList = new ObservableCollection<MessageListItem>();
                var rooms = new Room().GetJoinedList();
                //根据房间个数循环添加到集合
                if (rooms.Count > 0)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        lock (MyGroupList)
                        {
                            for (int i = 0; i < rooms.Count; i++)
                            {
                                AddToMyGroupList(rooms[i].ToMsgItem());//添加到集合中
                            }
                        }
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        lock (MyGroupList)
                        {
                            MyGroupList.Clear();
                        }
                    });
                }
            });
        }
        #endregion
        #region 本地加载好友列表
        /// <summary>
        /// 本地加载好友列表
        /// </summary>
        internal void LoadFriendsByDb()
        {
            try
            {
                var friends = new DataOfFriends().GetFriendsList();
                App.Current.Dispatcher.Invoke((() =>
                {
                    lock (FriendList)
                    {
                        foreach (var friendObj in friends)
                        {
                            AddToFriendList(friendObj.ToMsgListItem());
                        }
                    }
                }));

            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output("添加到好友时，" + ex.Message);
            }
        }
        #endregion
        #region 本地加载黑名单列表
        /// <summary>
        /// 本地加载黑名单列表
        /// </summary>
        internal void LoadBlockListByDb()
        {
            var blacks = new DataOfFriends().GetBlacksList();
            App.Current.Dispatcher.Invoke(() =>
            {
                lock (BlackList)
                {
                    BlackList.Clear();//先清空集合
                    foreach (var blackObj in blacks)
                    {
                        AddToBlackList(blackObj.ToMsgListItem());
                    }
                }
            });
        }
        #endregion

        #region 从黑名单列表中删除
        /// <summary>
        /// 从黑名单列表中删除
        /// </summary>
        /// <param name="item">消息项</param>
        public void RemoveFromBlackList(MessageListItem item)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (BlackList.Count(b => b.Jid == item.Jid) > 0)
                {
                    BlackList.Remove(item);
                }
            });
        }
        #endregion

        #region 接口加载好友列表
        /// <summary>
        /// 接口加载好友列表
        /// </summary>
        public void LoadFriendsByApi()
        {
            var client = APIHelper.GetFriendsAsync();//调取好友列表
            client.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    Task.Run(() =>
                    {
                        var restxt = Encoding.UTF8.GetString(e.Result);
                        var friends = JsonConvert.DeserializeObject<JsonFriends>(restxt);
                        if (friends.data.Count > 0)
                        {
                            Task.Run(() =>
                            {
                                var friend = new DataOfFriends() { status = 2 };
                                friend.DeleteByStatus();
                                foreach (var friObj in friends.data)
                                {
                                    friObj.AutoInsert();//存入数据库
                                }
                            });
                            #region 过滤系统账号
                            for (int i = 0; i < friends.data.Count; i++)
                            {
                                if (friends.data[i].toUserId.Length <= 6)
                                {
                                    friends.data.Remove(friends.data[i]);//如果为系统类型账号则不保存
                                }
                            }
                            //好友集合
                            List<MessageListItem> friendlist = new List<MessageListItem>();
                            #endregion
                            for (int i = 0; i < friends.data.Count; i++)
                            {
                                MessageListItem item = friends.data[i].ToMsgListItem();
                                friendlist.Add(item);
                            }
                            AddToFriendList(friendlist);//添加至好友列表项(主线程操作)
                            //需要缓存头像到本地的用户信息
                            List<DownLoadFile> downlist = new List<DownLoadFile>();
                            foreach (var item in friendlist)
                            {
                                string path = Applicate.LocalConfigData.GetDownloadAvatorPath(item.Jid);//尝试获取下载的缓存头像
                                if (!File.Exists(path))//当前用户本地无头像,添加到下载集合中下载
                                {
                                    downlist.Add(new DownLoadFile
                                    {
                                        Type = DownLoadFileType.Image,
                                        Token = item.Jid,
                                        Jid = item.Jid,
                                        LocalUrl = path
                                    });
                                }
                                else
                                {
                                    item.Avator = path;//头像存在就直接显示头像
                                }
                            }
                            //调用API下载头像
                            HttpDownloader.Download(downlist, (file) =>
                             {
                                 if (file.State == DownloadState.Error)//头像下载失败的时候设为初始头像
                                 {
                                     return;//初始化头像时已设置了显示头像(如果本地无头像就设置默认头像),故此处如果下载错误不更新UI
                                     //file.LocalUrl = Applicate.LocalConfigData.GetDisplayAvatorPath(file.Jid);
                                 }
                                 //这里根据回传的ID判断是哪一个用户的头像缓存好了。让UI部分去读取
                                 var uifriend = friendList.FirstOrDefault(f => f.Jid == file.Jid);
                                 if (uifriend != null)
                                 {
                                     App.Current.Dispatcher.Invoke(() =>
                                     {
                                         uifriend.Avator = file.LocalUrl;//刷新用户头像
                                     });
                                 }
                             });
                        }
                    });
                }
                else
                {
                    Snackbar.Enqueue("好友列表获取失败:" + e.Error.Message, "重试", () => { LoadFriendsByApi(); });
                }
            };
        }
        #endregion

        #region 接口加载黑名单列表
        /// <summary>
        /// 接口加载黑名单列表
        /// </summary>
        internal void LoadBlockListByApi()
        {
            //获取黑名单列表
            var bclient = APIHelper.GetBlackListAsync();//调取黑名单列表
            bclient.UploadDataCompleted += (sen, eve) =>
            {
                if (eve.Error == null)
                {
                    Task.Run(() =>
                    {
                        var blacks = JsonConvert.DeserializeObject<JsonFriends>(Encoding.UTF8.GetString(eve.Result));
                        if (blacks.data.Count > 0)
                        {
                            for (int i = 0; i < blacks.data.Count; i++)
                            {
                                AddToBlackList(blacks.data[i].ToMsgListItem());
                            }
                            //LoadBlackListByDb();
                            var black = new DataOfFriends() { status = -1 };
                            black.DeleteByStatus(-1);//删除黑名单好友
                            string tmp = Thread.CurrentThread.Name;
                            foreach (var blkObj in blacks.data)
                            {
                                blkObj.AutoInsert();//插入
                            }
                        }
                    });
                }
                else
                {
                    Snackbar.Enqueue("获取黑名单失败:" + eve.Error.Message, "重试", () => { LoadBlockListByApi(); });
                }
            };
        }
        #endregion

        #region 添加到黑名单列表
        /// <summary>
        /// 添加到黑名单列表
        /// </summary>
        /// <param name="item">要添加的项</param>
        private void AddToBlackList(MessageListItem item)
        {
            if (item != null)//空项不添加
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    BlackList.Add(item);
                });
            }
        }
        #endregion

        #endregion

        #region 添加到好友UI集合中
        /// <summary>
        /// 添加到好友UI集合中
        /// </summary>
        /// <param name="item">需要添加的项</param>
        public void AddToFriendList(MessageListItem item)
        {
            try
            {
                if (FriendList.Count(i => i.Jid == item.Jid) == 0)//如果不存在才添加
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        lock (FriendList)
                        {
                            FriendList.Add(item);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                ConsoleLog.Output(e.Message);
            }
        }

        public void AddToFriendList(List<MessageListItem> list)
        {
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    lock (FriendList)
                    {
                        foreach (var item in list)
                        {
                            AddToFriendList(item);//如果不存在才添加
                        }
                    }
                });
            }
            catch (Exception e)
            {
                ConsoleLog.Output(e.Message);//
            }
        }

        #endregion

        #region 好友Tab改变时修改绑定的值
        private void FriendSelectionChanged()
        {
            Task.Run(() =>
            {
                GC.Collect();
            });
            SelectedFriendProfileVisiblity = false;
            SelectedFriend = new DataOfUserDetial();
            //更新绑定的值
            if (FriendTypeIndex == 0)//好友
            {
                if (FriendList.Count == 0)
                {
                    return;
                }
                if (FriendSelectedIndex < 0)
                {
                    return;
                }
                var friend = FriendList[FriendSelectedIndex];
                if (friend != null)
                {
                    GetSelectUser(friend.Jid);//查询出对应UserId信息
                }
            }
            else if (FriendTypeIndex == 1)
            {
                if (BlackSelectedIndex > -1)
                {
                    //查询并赋值
                    var black = BlackList[BlackSelectedIndex];
                    if (black != null)
                    {
                        GetSelectUser(black.Jid);//查询出对应UserId信息
                    }
                }
                else
                {
                    //隐藏
                }
            }
        }
        #endregion

        #region 显示选中的用户
        /// <summary>
        /// 获取选中用户并显示
        /// </summary>
        /// <param name="jid">用户Jid</param>
        private void GetSelectUser(string jid)
        {
            var client = APIHelper.GetUserDetialAsync(jid);
            LogHelper.log.Info("选中用户GetUserDetial" + jid);
            var localuser = new DataOfFriends().GetByUserId(jid);//先显示本地
            SelectedFriend = localuser.ToDataOfUserDetial();
            SelectedFriend.sex = -1;
            SelectedFriend.birthday = 0;//初始化
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    string restext = Encoding.UTF8.GetString(res.Result);
                    var detial = JsonConvert.DeserializeObject<JsonuserDetial>(restext);
                    detial.data.remarkName = detial.data.friends.remarkName;
                    SelectedFriend = detial.data;
                    //如果昵称已更改, 且选中用户昵称不为空时更新
                    if (localuser.nickname != detial.data.nickname && !string.IsNullOrWhiteSpace(SelectedFriend.remarkName))
                    {
                        var fItem = new MessageListItem
                        {
                            Jid = detial.data.userId,
                            ShowTitle = detial.data.friends.remarkName,
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(detial.data.userId)
                        };
                        //通知页面刷新昵称
                        Messenger.Default.Send(fItem, MainViewNotifactions.UpdateAccountName);
                    }
                }
                else
                {
                    Snackbar.Enqueue("网络错误,获取用户详情失败:" + res.Error.Message, true);
                    var tmp = new DataOfFriends().GetByUserId(jid);//读取本地好友详情
                    SelectedFriend = new DataOfUserDetial()
                    {
                        userId = tmp.toUserId,
                        nickname = tmp.toNickname,
                        sex = tmp.sex,
                        birthday = tmp.birthday,
                        countryId = tmp.countryId,
                        provinceId = tmp.provinceId,
                        cityId = tmp.cityId,
                        areaId = tmp.areaId,
                    };
                }
                SelectedFriendProfileVisiblity = true;//显示选中用户详情
            };
        }
        #endregion

        #region 由MsgItem开启聊天
        /// <summary>
        /// 由MsgItem开启聊天
        /// </summary>
        internal void StartNewChatFromItem(MessageListItem targetItem)
        {
            try
            {
                MessageListSelectedIndex = -1;
                //如果会话不存在
                if (!RecentMessageList.Any(m => m.Jid == targetItem.Jid))
                {
                    SetChatTitle(targetItem.Jid, targetItem.ShowTitle);//设置会话标题
                    targetItem.MessageItemType = ItemType.Message;//设置消息类型为信息
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        InsertOrTopSingleMessageList(targetItem.Clone());//插入到第一个聊天控件中
                        MessageListSelectedIndex = 0;//跳转至第一项会话
                        targetItem.Insert();//保存至数据库
                    });
                }
                else//如果存在会话
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MessageListItem chatItem = RecentMessageList.First(m => m.Jid == targetItem.Jid);//获取对应的Message
                        //Sess.Jid = chatItem.Jid;
                        //Sess.NickName = chatItem.MessageTitle;
                        //Sess.RemarkName = chatItem.ShowTitle;
                        MessageListSelectedIndex = RecentMessageList.IndexOf(chatItem);//
                        chatItem.Update();
                    });
                }
                //跳转到对应的聊天界面
                Messenger.Default.Send(targetItem.Jid, ChatBubblesNotifications.ShowBubbleList);//显示消息记录
                MainTabSelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region 新建群聊时
        /// <summary>
        /// 新建群聊时(创建一个新的会话)
        /// </summary>
        /// <param name="room">创建好的群聊</param>
        internal void OnCreateNewGroup(Room room)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainTabSelectedIndex = 0;//跳转到消息页面
                AddToMyGroupList(room.ToMsgItem());
                var item = MyGroupList.FirstOrDefault(r => r.Jid == room.jid);
                if (item != null)
                {
                    StartNewChatFromItem(item.Clone());
                }
                Snackbar.Enqueue("创建群 “" + room.name + "”成功！");
            });
        }
        #endregion

        #region 添加单个Message项
        /// <summary>
        /// 添加单个Message项
        /// </summary>
        /// <param name="item"></param>
        public void AddSingleMessageList(MessageListItem item)
        {
            if (RecentMessageList.Count(m => m.Jid == item.Jid) == 0)
            {
                RecentMessageList.Add(item);
            }
        }
        #endregion

        #region 退出群聊
        /// <summary>
        /// 退出群聊    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExitGroup(MessageListItem room)
        {
            var tmp = new DataofMember().GetListByRoomId(room.Id);//使用
                                                                  //查询出自己的身份编号
            var member = tmp.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId);
            HttpClient client = null;
            if (member.role == MemberRole.Owner)
            {
                client = APIHelper.DismissRoomAsync(room);
            }
            else
            {
                client = ShiKuManager.LeaveRoom(room);
            }
            client.Tag2 = room.Jid;//指定Jid
            client.UploadDataCompleted += GroupDeleteComplete;//指定回调
        }
        private void GroupDeleteComplete(object sender, UploadDataCompletedEventArgs e)
        {
            var resstr = Encoding.UTF8.GetString(e.Result);//获取
            var result = JsonConvert.DeserializeObject<JsonBase>(resstr);
            var jid = ((HttpClient)sender).Tag2 as string;//获取Jid
            var rroom = ((HttpClient)sender).Tag as MessageListItem;//从Tag获取房间信息
            if (result.resultCode == 1)
            {
                DeleteFromMessageList(jid);//删除消息列表项
                Snackbar.Enqueue("退出群 " + rroom.MessageTitle + " 成功", true);
                RemoveMyGroupByJid(rroom.Jid);//从群聊列表中移除
                RoomInfoVisible = false;//隐藏群详情
                ConsoleLog.Output("退群成功");
            }
            else
            {
                RoomInfoVisible = true;
                Snackbar.Enqueue("退群失败:" + result.resultMsg);
                ConsoleLog.Output("退群失败");
            }
        }
        #endregion

        #region 删除好友
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        public void FriendDelete(MessageListItem item)
        {
            //提示确认信息
            if (System.Windows.MessageBox.Show("确定要删除\"" + item.MessageTitle + "\"吗 ？", "删好友", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                try
                {
                    var client = APIHelper.DeleteFriendAsync(item.Jid);
                    client.Tag = item;//赋值自定义对象
                    client.UploadDataCompleted += FriendDeleteComplete;
                }
                catch (Exception ex)
                {
                    LogHelper.log.Error(ex.Message, ex);
                    ConsoleLog.Output("删好友错误:" + ex.Message);
                }
            }
        }

        private void FriendDeleteComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)//网络正常
            {
                var tmp = Encoding.UTF8.GetString(e.Result);
                var result = JsonConvert.DeserializeObject<JsonBase>(tmp);
                var item = ((HttpClient)sender).Tag as MessageListItem;
                if (result.resultCode == 1)//正常情况
                {
                    //发送删除好友推送
                    ShiKuManager.SendDeleteFriend(item);
                    new DataOfFriends().UpdateFriendState(item.Jid, 0);//更新数据库
                    //删除消息列表好友
                    OnDeleteFriend(item.Jid);
                }
                else
                {
                    Snackbar.Enqueue("删除好友失败:" + result.resultMsg);
                }
            }
            else
            {
                Snackbar.Enqueue("网络错误, 删除好友失败:" + e.Error.Message);
            }
        }
        #endregion

        #region 从消息列表删除
        /// <summary>
        /// 从消息列表删除
        /// </summary>
        /// <param name="userId">需要删除的UserId</param>
        public void DeleteFromMessageList(string userId)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var item = RecentMessageList.FirstOrDefault(m => m.Jid == userId);
                int index = RecentMessageList.IndexOf(item);
                if (index >= 0)
                {
                    item.Delete();//删除数据库
                    if (Sess.Jid == userId)
                    {
                        Sess = new Session() { IsOnlineText = "" };
                    }
                    RemoveMessageListItem(index);
                    Sess.NickName = "";
                    Sess.RemarkName = "";
                }
            });
        }
        #endregion

        #region 移除消息列表中项
        /// <summary>
        /// 移除消息列表中项
        /// </summary>
        public void RemoveMessageListItem(int index = -1)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (index == -1)//If -1, just delete the Item you Selectd
                    {
                        index = MessageListSelectedIndex;
                    }
                    var msgItem = RecentMessageList[index];
                    if (RecentMessageList.Count > +1)// If the index of MessageList Count is larger than Selected Item Index 如果消息总数比选中值大
                    {
                        RecentMessageList.RemoveAt(index);// Remove it 移除
                        if (index == 0)
                        {
                            MessageListSelectedIndex = index;
                        }
                        else
                        {
                            MessageListSelectedIndex = index - 1;// Select Last item 选中上一级元素
                        }
                    }
                    else if (RecentMessageList.Count - 1 == 0)
                    {
                        MessageListSelectedIndex = -1;// Don't select any item 不选中任何 元素
                        Sess.Jid = null;
                        Sess.NickName = "";
                        Sess.RemarkName = "";
                    }
                    else if (RecentMessageList.Count == index + 1)
                    {
                        MessageListSelectedIndex = index;// Select Last item 选中上一级元素
                        RecentMessageList.RemoveAt(index);//Remove 移除
                    }
                    else
                    {
                        Sess.Jid = null;
                    }
                    msgItem.Delete();//更新数据库
                    SetTotalUnReadCount();//累加未读消息数量
                    //UpdateMessageList();
                }
                catch (Exception ex)
                {
                    ConsoleLog.Output(ex.Message);
                }
            });
        }
        #endregion

        #region 加入黑名单
        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="sender">控件源</param>
        /// <param name="e"></param>
        public void BlackListAdd(MessageListItem item)
        {
            if (System.Windows.MessageBox.Show("是否将 " + item.ShowTitle + " 加入黑名单？", "加入黑名单", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                try
                {
                    var client = APIHelper.BlockFriendAsync(item.Jid);
                    client.Tag = item;//设置临时变量
                    client.UploadDataCompleted += (sender, e) =>
                    {
                        var res = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                        var tmpClient = (HttpClient)sender;//获取事件源
                        if (tmpClient.Tag != null)
                        {
                            var tmpfriendItem = (MessageListItem)client.Tag;
                            if (res.resultCode == 1)
                            {
                                ShiKuManager.DefriendReq(tmpfriendItem);//发送拉黑好友推送
                                new DataOfFriends().UpdateFriendState(tmpfriendItem.Jid, -1);//更新数据库
                                OnBlackFriend(tmpfriendItem.Jid);//更新UI
                                //var toBlack = FriendList.FirstOrDefault(f => f.Jid == tmpFriend.toUserId);
                                //FriendList.Remove(toBlack);//删除好友列表中项
                                //AddToBlackList(toBlack);//添加黑名单项
                                Snackbar.Enqueue("将\"" + tmpfriendItem.ShowTitle + "\"拉黑成功");
                            }
                            else
                            {
                                Snackbar.Enqueue("加入黑名单失败:" + res.resultMsg);
                            }
                        }
                        else
                        {
                            Snackbar.Enqueue("加入黑名单失败，请重试");
                        }
                    };
                }
                catch (Exception ex)
                {
                    LogHelper.log.Error(ex.Message, ex);
                    ConsoleLog.Output("拉黑错误:" + ex.Message);
                }
            }
        }
        #endregion

        #region 移出黑名单
        /// <summary>
        /// 移出黑名单
        /// </summary>
        /// <param name="user"></param>
        internal void DeBlack_Click(MessageListItem target)
        {
            try
            {
                var useritem = target.Clone();
                var client = ShiKuManager.CancelBlockfriend(useritem.Jid);
                client.Tag = useritem;
                client.UploadDataCompleted += (sender, e) =>
                {
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                    var blackitem = ((HttpClient)sender).Tag as MessageListItem;
                    if (result.resultCode == 1)
                    {
                        OnCancelBlack(blackitem.Jid);//修改UI
                        Snackbar.Enqueue("将\"" + blackitem.ShowTitle + "\"移出黑名单成功");
                        var item = FriendList.FirstOrDefault(f => f.Jid == useritem.Jid);
                        if (item != null)
                        {
                            RecentMessageList.Add(item.Clone());//创建一个新的聊天项
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("将 \"" + blackitem.ShowTitle + "\" 移出黑名单失败，请重试");
                    }
                };
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                Snackbar.Enqueue("移出黑名单失败，请稍后重试");
                ConsoleLog.Output("移出黑名单错误:" + ex.Message);
            }
        }
        #endregion

        #region 设置好友昵称
        /// <summary>
        /// 设置好友名称
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="userName">好友昵称(群组时可为空)</param>
        internal void SetChatTitle(string userId, string userName)
        {
            var tmpsess = Sess;
            tmpsess.Jid = userId;
            Sess = tmpsess;
            if (Sess.Jid.Length < 20)//如果userid小于15就
            {
                if (Sess.Jid == "10001")
                {
                    Sess.NickName = "新的朋友";
                    Sess.RemarkName = "新的朋友";
                    Sess.IsOnlineText = "";
                }
                else if (Sess.Jid == "10000")
                {
                    Sess.NickName = "客服公众号";
                    Sess.RemarkName = "客服公众号";
                    Sess.IsOnlineText = "";
                }
                else
                {
                    var friends = new DataOfFriends().GetByUserId(userId);
                    var client = ShiKuManager.GetFriendState(userId);//回调方式赋值好友状态
                    client.UploadDataCompleted += (sender, e) =>
                    {
                        try
                        {
                            var tmpres = Encoding.UTF8.GetString(e.Result);
                            var tmp = JsonConvert.DeserializeObject<JsonUserState>(tmpres);
                            Sess.IsOnlineText = (tmp.data == 1) ? "在线" : "离线";
                        }
                        catch (Exception ex)
                        {
                            ConsoleLog.Output("获取好友在线状态出错:--" + ex.Message);
                        }
                    };
                    //获取Nickname
                    Sess.NickName = userName;
                    Sess.RemarkName = userName;
                }
            }
            else
            {
                var room = new Room().GetByJid(userId);
                if (room != null)
                {
                    Sess.RoomId = room.id;//设置群ID
                    var mem = new DataofMember { userId = Applicate.MyAccount.userId }.GetModelByJid(room.jid);
                    if (mem != null)
                    {
                        Sess.MyMemberNickname = mem.nickname;
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Sess.NickName = room.name;
                        Sess.RemarkName = room.name;
                        Sess.IsOnlineText = room.userSize + " 位成员";//群组为3, 不显示好友状态
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Sess.NickName = "";
                        Sess.RemarkName = "";
                        Sess.IsOnlineText = "";//群组为3, 不显示好友状态
                    });
                }
            }
        }
        #endregion

        #region 更新消息列表到数据库
        internal void UpdateMessageList()
        {
            if (MessageListSelectedIndex < 0)
            {
                return;
            }
            if (RecentMessageList.Count > 0)
            {
                RecentMessageList[MessageListSelectedIndex].Update();//消息列表更新到数据库
            }
        }
        #endregion

        #region 刷新所有图片对象
        /// <summary>
        /// 刷新所有图片对象
        /// </summary>
        internal void RefreshAllImg(string jid)
        {

            if (!string.IsNullOrWhiteSpace(jid))
            {
                if (jid == Applicate.MyAccount.userId)
                {
                    Me.userId = "";//以后再改
                    Me.userId = jid;
                }
                else
                {
                    var itemMsgObj = RecentMessageList.FirstOrDefault(d => d.Jid == jid);
                    if (itemMsgObj != null)
                    {
                        itemMsgObj.Jid = "";
                        itemMsgObj.Jid = jid;
                    }

                    var friendObj = FriendList.FirstOrDefault(d => d.Jid == jid);
                    if (friendObj != null)
                    {
                        friendObj.Jid = "";
                        friendObj.Jid = jid;
                    }

                    var blackObj = BlackList.FirstOrDefault(d => d.Jid == jid);
                    if (blackObj != null)
                    {
                        blackObj.Jid = "";
                        blackObj.Jid = jid;
                    }

                    var roomMemeberObj = SelectedGroupMember.FirstOrDefault(d => d.userId == jid);
                    if (roomMemeberObj != null)
                    {
                        roomMemeberObj.userId = "";
                        roomMemeberObj.userId = jid;
                    }

                    if (SelectedFriend != null)
                    {
                        SelectedFriend.userId = "";
                        SelectedFriend.userId = jid;
                    }
                }
            }
        }
        #endregion

        #region 新建聊天
        internal void NewChat(string userId)
        {
            if (userId.Length < 20)
            {//普通用户
                if (MyGroupList.Count(r => r.Jid == userId) > 0)
                {//检验存在
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MessageListItem ritem = new MessageListItem();
                        ritem = MyGroupList.FirstOrDefault(r => r.Jid == userId);
                        MessageListItem chatItem = RecentMessageList.First(m => m.Jid == userId);//获取对应的Message
                        MessageListSelectedIndex = RecentMessageList.IndexOf(chatItem);//
                    });
                }
            }
            else
            {
            }
        }
        #endregion

        #region 刷新群聊详情预览页
        /// <summary>
        /// 刷新群聊详情预览页
        /// </summary>
        /// <param name="roomId">群组Id</param>
        public void ReloadRoomDetialBoard(string roomId)
        {
            var tmpClient = APIHelper.GetRoomDetialByRoomIdAsync(roomId);
            DisplayGroup = new Room();//.GetByRoomId();
            tmpClient.UploadDataCompleted += (sen, res) =>
            {
                DisplayGroup = new Room().GetByRoomId(roomId);
                if (DisplayGroup == null)
                {
                    return;
                }
                updateRoomsItemTitle(DisplayGroup.jid, DisplayGroup.name);
                if (DisplayGroup.showMember == 1 || DisplayGroup.userId.ToString() == Applicate.MyAccount.userId)
                {
                    HiddenMembers = "";
                    GetSelectedGroupMember(roomId);
                }
                else
                {
                    HiddenMembers = "群主关闭群成员显示";
                    //群主关闭显示群成员
                    SelectedGroupMember.Clear();
                }
            };
        }
        #endregion

        #region 更新群聊列表内容
        public void updateRoomsItemTitle(string jid, string title)
        {
            MyGroupList.Where(d => d.Jid == jid).ToList().ForEach(d => d.MessageTitle = title);
            RecentMessageList.Where(d => d.Jid == jid).ToList().ForEach(d => d.MessageTitle = title);
            if (Sess.Jid == jid)
            {
                SetChatTitle(jid, title);
            }
        }
        #endregion

        #region 获取选中的群成员
        private void GetSelectedGroupMember(string roomId)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                IList<DataofMember> memberList = new List<DataofMember>();
                memberList = new DataofMember().GetListByRoomId(roomId);//从数据库中查询
                List<DownLoadFile> files = new List<DownLoadFile>();
                if (memberList.Count > 0)
                {
                    foreach (var item in memberList)
                    {
                        files.Add(new DownLoadFile
                        {
                            Jid = item.userId,
                            ShouldDeleteWhileFileExists = false,
                            LocalUrl = Applicate.LocalConfigData.GetDownloadAvatorPath(item.userId)
                        });
                    }
                }
                lock (memberList)
                {
                    SelectedGroupMember.Clear();
                    foreach (var member in memberList)
                    {
                        SelectedGroupMember.Add(member);
                    }
                }
                HttpDownloader.Download(files, (complete) =>
                {
                    var member = SelectedGroupMember.FirstOrDefault(m => m.userId == complete.Jid);
                    if (member != null)
                    {
                        member.userId = complete.Jid;//更新头像
                    }
                });
            });
        }
        #endregion

        #region 好友类型选中改变时
        /// <summary>
        /// 好友类型选中改变时
        /// </summary>
        public void FriendTypeChanged()
        {
            if (FriendTypeIndex == 0)
            {
                if (friendSelectedIndex >= 0)//有选中项的话
                {
                    GetSelectUser(FriendList[FriendSelectedIndex].Jid);//获取好友详情
                }
                else
                {
                    SelectedFriend = null;
                }
            }
            else if (FriendTypeIndex == 1)
            {
                if (BlackSelectedIndex >= 0)
                {
                    GetSelectUser(BlackList[BlackSelectedIndex].Jid);//获取黑名单详情
                }
                else
                {
                    SelectedFriend = null;
                }
            }
        }
        #endregion
        #region 设置状态
        /// <summary>
        /// 设置Xmpp状态
        /// </summary>
        /// <param name="state">Xmpp连接状态</param>
        internal void SetXmppState(XmppConnectionState state)
        {
            lock (AccountStateFill)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (Applicate.GetWindow<MainWindow>() == null)
                    {
                        return;
                    }
                    switch (state)
                    {
                        case XmppConnectionState.Disconnected:
                            AccountStateFill = new SolidColorBrush(Colors.Gray);
                            AccountStateText = "未连接";
                            break;
                        case XmppConnectionState.Connecting:
                            AccountStateFill = new SolidColorBrush(Colors.DimGray);
                            AccountStateText = "连接中...";
                            break;
                        case XmppConnectionState.Connected:
                            AccountStateFill = new SolidColorBrush(Colors.DeepSkyBlue);
                            AccountStateText = "Connected";
                            break;
                        case XmppConnectionState.Authenticating:
                            AccountStateFill = new SolidColorBrush(Colors.DeepPink);
                            AccountStateText = "验证中";
                            break;
                        case XmppConnectionState.Authenticated:
                            AccountStateFill = new SolidColorBrush(Colors.Tomato);
                            AccountStateText = "Authenticated";
                            break;
                        case XmppConnectionState.Binding:
                            AccountStateFill = new SolidColorBrush(Colors.Thistle);
                            AccountStateText = "绑定中...";
                            break;
                        case XmppConnectionState.Binded:
                            AccountStateFill = new SolidColorBrush(Colors.SlateGray);
                            AccountStateText = "绑定完成";
                            break;
                        case XmppConnectionState.StartSession:
                            AccountStateFill = new SolidColorBrush(Colors.SeaGreen);
                            AccountStateText = "连接中";
                            break;
                        case XmppConnectionState.StartCompression:
                            AccountStateFill = new SolidColorBrush(Colors.YellowGreen);
                            AccountStateText = "StartCompression";
                            break;
                        case XmppConnectionState.Compressed:
                            AccountStateFill = new SolidColorBrush(Colors.Yellow);
                            AccountStateText = "Compressed";
                            break;
                        case XmppConnectionState.SessionStarted:
                            AccountStateFill = new SolidColorBrush(Colors.LightGreen);
                            AccountStateText = "在线";
                            break;
                        case XmppConnectionState.Securing:
                            AccountStateFill = new SolidColorBrush(Colors.BlueViolet);
                            AccountStateText = "Securing...";
                            break;
                        case XmppConnectionState.Registering:
                            AccountStateFill = new SolidColorBrush(Colors.Aqua);
                            AccountStateText = "Registering";
                            break;
                        case XmppConnectionState.Registered:
                            AccountStateFill = new SolidColorBrush(Colors.AliceBlue);
                            AccountStateText = "Registered";
                            break;
                        default:
                            break;
                    }
                    //ConsoleLog.Output("-=-=-=-=状态颜色为：" + AccountStateFill.Color.ToString());
                });
            }
        }
        #endregion

        #region 消息列表选中改变
        /// <summary>
        /// 消息列表选中改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageListSelectionChagned()
        {
            Task.Run(() =>
            {
                GC.Collect();
            });
            if (MessageListSelectedIndex < 0)
            {
                return;
            }
            TypingTimer.Stop();//停止正在发送计时器以避免副标题被更改
            //Temp Media Player Stop(I'll change the mediaplayer into mainwindow as a control)
            var main = Applicate.GetWindow<MainWindow>();
            if (main.VlcPlayer.SourceProvider.MediaPlayer != null)
            {
                main.VlcPlayer.SourceProvider.MediaPlayer.Stop();
                //main.SelectedContact.SelectedItems.Clear();//删除//此处替换为清除数据集合
            }
            MessageListItem item = RecentMessageList[MessageListSelectedIndex];
            if (string.IsNullOrWhiteSpace(item.Jid) || (item.Jid == Sess.Jid))
            {
                return;
            }
            TotalUnReadCount -= item.UnReadCount;//减去对应未读消息数量
            item.UnReadCount = 0;//未读数量为0
            //清理文本 
            TextFieldDocument.Blocks.Clear();
            var emptypara = new Paragraph(new Run(""));
            TextFieldDocument.Blocks.Add(emptypara);
            MsgItemMouseDown(item);
        }
        #endregion

        #region 单击消息进行聊天
        /// <summary>
        /// 单击消息进行聊天
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="e"></param>
        internal void MsgItemMouseDown(MessageListItem Item)
        {
            SetChatTitle(Item.Jid, Item.ShowTitle);//设置聊天标题
            Sess.RoomId = Item.Id;//设置群Id
            if (Item.Jid.Length >= 7)
            {
                Messenger.Default.Send(Item.Jid, ChatBubblesNotifications.ShowBubbleList);//通知气泡列表显示消息               
            }
            if (Item.Jid.Length > 15)
            {
                Messenger.Default.Send(Item, ChatBubblesNotifications.SetMessageInfo);//通知到气泡列
            }
            //var lastMsg = ServiceLocator.Current.GetInstance<ChatBubbleListViewModel>().ShowDefaultMessage(Item.Jid);//批量显示消息
            //Item.Msg = lastMsg;//显示最后一条消息
            //保存最近消息列表
            Task.Run(() =>
            {
                UpdateMessageList();
            });
        }
        #endregion

        #region 拖动文件发送
        #region 拖动时悬浮
        /// <summary>
        /// 拖动文件到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eve"></param>
        private void ChatDragEnter(object sender, System.Windows.DragEventArgs eve)
        {
            eve.Handled = true;
        }
        #endregion

        #endregion

        #region 消息处理
        /// <summary>
        /// MainWindow消息处理(只做UI处理)
        /// MainWindow ChatMessage Processing (UI process only)
        /// </summary>
        /// <param name="msg">收到的消息</param>
        public void ProcessNewMessage(Messageobject msg)
        {
            CreateOrUpdateMsgItem(msg.Clone());
            switch (msg.type)
            {
                case kWCMessageType.PokeMessage://戳一戳
                    Messenger.Default.Send(true, MainWindow.ShowWindow);//显示
                    break;
                case kWCMessageType.Typing://正在输入
                    Sess.IsOnlineText = "对方正在输入...";
                    TypingTimer.Start();
                    break;
                case kWCMessageType.RoomExit://退群
                    var room = new Room { jid = msg.objectId }.GetByJid();
                    if (room != null)
                    {
                        ReloadRoomDetialBoard(room.id);
                    }
                    if (msg.toUserId == Applicate.MyAccount.userId)//如果是自己退群
                    {
                        RemoveMyGroupByJid(msg.objectId);//MyGroupList单个移除
                        DeleteFromMessageList(msg.objectId);//删除最近联系人
                    }
                    break;
                case kWCMessageType.RoomDismiss://解散群
                    RemoveMyGroupByJid(msg.objectId);//MyGroupList单个移除
                    //msg.content = "你已退出此群";
                    DeleteFromMessageList(msg.objectId);
                    //别人解散
                    //SetChatTitle(msg.objectId, null);
                    //}
                    break;
                case kWCMessageType.RoomNotice://群公告
                    break;
                case kWCMessageType.RoomMemberBan://成员禁言
                    break;
                case kWCMessageType.RoomInvite:
                    if (msg.toUserId == Applicate.MyAccount.userId)
                    {
                        var tmpRoom = new Room
                        {
                            jid = msg.objectId,
                            id = msg.fileName,
                            name = msg.content,
                            nickname = msg.fromUserName,
                            //IsJoined = true,//被邀请进入为已加入
                        };
                        //SetChatTitle(tmpRoom.jid, tmpRoom.name);//设置标题
                        AddToMyGroupList(tmpRoom.ToMsgItem());//添加群聊项
                    }
                    else
                    {
                        var rom = new Room { jid = msg.objectId }.GetByJid();
                        if (rom != null)
                        {
                            //SetChatTitle(rom.jid, rom.name);
                        }
                    }
                    break;
                case kWCMessageType.FriendRequest://打招呼
                    break;
                case kWCMessageType.RequestFriendDirectly://直接添加好友
                    var fItem = new DataOfFriends().GetByUserId(msg.fromUserId);
                    AddToFriendList(fItem.ToMsgListItem());//更新好友列表
                    StartNewChatFromItem(fItem.ToMsgListItem());//开始聊天
                    break;
                case kWCMessageType.RequestAgree://对方通过验证
                    var item = new DataOfFriends().GetByUserId(msg.fromUserId);
                    AddToFriendList(item.ToMsgListItem());//添加到好友列表
                    break;
                case kWCMessageType.DeleteFriend://被好友删除
                    OnDeleteFriend(msg.fromUserId);
                    break;
                case kWCMessageType.RoomNameChange://群名称修改
                    OnUserNameChanged(new MessageListItem { Jid = msg.jid, ShowTitle = msg.content });
                    break;
                case kWCMessageType.BlackFriend://被拉黑
                    OnBlackFriend(msg.fromUserId);//设置自己被拉黑
                    break;
                default:
                    break;
            }
        }

        #region 我的群组列表根据Jid移除
        /// <summary>
        /// 我的群组列表根据Jid移除
        /// </summary>
        /// <param name="jid">对应的Jid</param>
        private void RemoveMyGroupByJid(string jid)
        {
            var tmproom = MyGroupList.FirstOrDefault(m => m.Jid == jid);
            if (tmproom != null)
            {
                App.Current.Dispatcher.Invoke(() => { MyGroupList.Remove(tmproom); });
            }
        }
        #endregion

        #region 修改对应Jid昵称
        /// <summary>
        /// 修改对应Jid昵称
        /// </summary>
        /// <param name="nitem"></param>
        public void OnUserNameChanged(MessageListItem nitem)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (nitem.Jid.Length > 15)
                {
                    lock (MyGroupList)
                    {
                        if (MyGroupList.Count(m => m.Jid == nitem.Jid) > 0)
                        {
                            var item = MyGroupList.FirstOrDefault(m => m.Jid == nitem.Jid);//获取项
                            item.ShowTitle = nitem.ShowTitle;//更新群组列表项
                            if (DisplayGroup.jid == nitem.Jid)
                            {
                                DisplayGroup.name = nitem.ShowTitle;//更新预览面板的群名称
                            }
                        }
                    }
                }
                else
                {
                    if (BlackList.Count(i => i.Jid == nitem.Jid) > 0)
                    {
                        var item = BlackList.FirstOrDefault(m => m.Jid == nitem.Jid);
                        item.ShowTitle = nitem.ShowTitle;//更新群组列表项//更新黑名单
                    }
                    if (FriendList.Count(f => f.Jid == nitem.Jid) > 0)
                    {
                        var item = FriendList.FirstOrDefault(f => f.Jid == nitem.Jid);//获取对应好友项
                        item.ShowTitle = nitem.ShowTitle;//更新群组列表项/更新好友列表
                    }
                }
                if (RecentMessageList.Count(m => m.Jid == nitem.Jid) > 0)//更新最近消息项列表
                {
                    var item = RecentMessageList.FirstOrDefault(m => m.Jid == nitem.Jid);
                    item.ShowTitle = nitem.ShowTitle;//更新群组列表项
                }
                if (Sess.Jid == nitem.Jid)//更新会话标题
                {
                    Sess.NickName = nitem.MessageTitle;
                    Sess.RemarkName = nitem.ShowTitle;//更新
                }
                if (SelectedFriend.userId == nitem.Jid)//更新选中好友
                {
                    SelectedFriend.remarkName = nitem.ShowTitle;//更新选中好友类型
                }
            });
        }
        #endregion

        #region 添加到我的群组
        /// <summary>
        /// 添加到我的群组UI
        /// </summary>
        /// <param name="item">判断是否存在的项</param>
        private void AddToMyGroupList(MessageListItem item)
        {
            if (MyGroupList.Count(m => m.Jid == item.Jid) == 0)
            {
                lock (MyGroupList)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MyGroupList.Add(item.Clone());
                    });
                }
            }
        }
        #endregion

        #region 消息回执
        /// <summary>
        /// 消息回执
        /// </summary>
        /// <param name="msg">对应的消息</param>
        public void MsgReceipt(Messageobject msg)
        {
            switch (msg.type)
            {
                case kWCMessageType.kWCMessageTypeNone:
                    break;
                #region 消息回执处理(送达回执)
                case kWCMessageType.Text:
                case kWCMessageType.Image:
                case kWCMessageType.Voice:
                case kWCMessageType.Location:
                case kWCMessageType.Gif:
                case kWCMessageType.Video:
                case kWCMessageType.Audio:
                case kWCMessageType.Card:
                case kWCMessageType.File:
                case kWCMessageType.Remind:
                case kWCMessageType.SystemImage1:
                case kWCMessageType.SystemImage2:
                case kWCMessageType.Link:
                case kWCMessageType.AudioChatAsk:
                case kWCMessageType.AudioChatAccept:
                case kWCMessageType.AudioChatCancel:
                case kWCMessageType.AudioChatEnd:
                case kWCMessageType.VideoChatAsk:
                case kWCMessageType.VideoChatAccept:
                case kWCMessageType.VideoChatCancel:
                case kWCMessageType.VideoChatEnd:
                    //UpdateIsSend(msg.messageId);
                    break;
                #endregion
                case kWCMessageType.CancelBlackFriend:
                    OnCancelBlack(msg.ToId);//取消黑名单
                    break;
                case kWCMessageType.BlackFriend:
                    OnBlackFriend(msg.ToId);//设置对方被拉黑
                    break;
                case kWCMessageType.DeleteFriend:
                    Snackbar.Enqueue("已删除好友 " + msg.toUserName);//显示Snackbar.Enqueue提示
                    //OnDeleFriend(new Messageobject() { FromId = msg.ToId });
                    break;
                case kWCMessageType.FriendRequest://好友请求
                    break;
                case kWCMessageType.RequestFriendDirectly://直接添加好友
                    var item = new DataOfFriends().GetByUserId(msg.ToId);
                    AddToFriendList(item.ToMsgListItem());
                    if (RecentMessageList.Count(r => r.Jid == msg.ToId) == 0)
                    {
                        var tmp = FriendList.FirstOrDefault(m => m.Jid == msg.ToId);
                        StartNewChatFromItem(tmp.Clone());//聊天
                    }
                    break;
                case kWCMessageType.RequestAgree://同意对方好友请求
                    var friend = new DataOfFriends().GetByUserId(msg.toUserId);
                    AddToFriendList(friend.ToMsgListItem());
                    break;
                case kWCMessageType.Typing://正在输入
                    break;
                case kWCMessageType.Withdraw:
                    break;
                case kWCMessageType.IsRead:
                    break;
                case kWCMessageType.RedPacket:
                    break;
                case kWCMessageType.VideoMeetingInvite:
                    break;
                case kWCMessageType.VideoMeetingJoin:
                    break;
                case kWCMessageType.VideoMeetingQuit:
                    break;
                case kWCMessageType.VideoMeetingKick:
                    break;
                case kWCMessageType.AudioMeetingInvite:
                    break;
                case kWCMessageType.AudioMeetingJoin:
                    break;
                case kWCMessageType.AudioMeetingQuit:
                    break;
                case kWCMessageType.PhoneCalling:
                    break;
                case kWCMessageType.AudioMeetingSetSpeaker:
                    break;
                case kWCMessageType.AudioMeetingAllSpeaker:
                    break;
                case kWCMessageType.RoomNameChange:
                    break;
                case kWCMessageType.RoomDismiss://解散群
                case kWCMessageType.RoomExit://退群
                    var tmproom = MyGroupList.FirstOrDefault(r => r.Jid == msg.objectId);
                    if (tmproom != null)
                    {
                        MyGroupList.Remove(tmproom);//移除群组列表中对应项
                    }
                    var msgitem = RecentMessageList.FirstOrDefault(m => m.Jid == msg.objectId);
                    if (msgitem != null)
                    {
                        int rIndex = RecentMessageList.IndexOf(msgitem);//获取索引
                        RemoveMessageListItem(rIndex);//删除最近消息项
                        //Sess.Jid = "";//
                    }
                    break;
                case kWCMessageType.RoomNotice:
                    break;
                case kWCMessageType.RoomMemberBan:
                    break;
                case kWCMessageType.RoomInvite:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 删除好友
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="msg">通过FromUserId进行操作</param>
        public void OnDeleteFriend(string jid)
        {
            //删除好友
            DeleteFromMessageList(jid);//删除消息列表中的数据
            DeletefromFriendList(jid);//删除好友列表中的数据
            DeletefromBlockList(jid);
        }
        #endregion

        #region 从黑名单删除
        private void DeletefromBlockList(string jid)
        {
            var item = BlackList.FirstOrDefault(b => b.Jid == jid);
            if (item != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    BlackList.Remove(item);//移除对应项
                });
            }
        }
        #endregion

        #region 删除好友UI列表中数据
        private void DeletefromFriendList(string fromId)
        {
            var tmp = FriendList.FirstOrDefault(f => f.Jid == fromId);
            if (tmp != null)
            {
                int tmpindex = FriendList.IndexOf(tmp);
                App.Current.Dispatcher.Invoke(() =>
                {
                    lock (FriendList)
                    {
                        FriendList.RemoveAt(tmpindex);//移除对应的好友
                    }
                });
            }
        }
        #endregion

        #region 拉黑
        /// <summary>
        /// 拉黑(双方操作)
        /// </summary>
        /// <param name="msg"></param>
        public void OnBlackFriend(string userId)
        {
            DeleteFromMessageList(userId);//删除
            App.Current.Dispatcher.Invoke(() =>
            {
                var tmpfitem = FriendList.FirstOrDefault(f => f.Jid == userId);
                if (tmpfitem != null)
                {
                    AddToBlackList(tmpfitem);
                    FriendList.Remove(tmpfitem);//删除并添加到黑名单
                }
            });
        }
        #endregion

        #region 取消黑名单
        /// <summary>
        /// 取消黑名单
        /// </summary>
        /// <param name="userId">取消的用户</param>
        internal void OnCancelBlack(string userId)
        {
            var black = BlackList.FirstOrDefault(b => b.Jid == userId);
            if (black != null)
            {
                AddToFriendList(black);//添加
                RemoveFromBlackList(black);//移除
            }
        }
        #endregion

        #endregion

        #region 音频播放
        public string playingAudioId = "";

        public bool MainAudio(string msgId, string path = null)
        {
            bool isPlaying = false;
            try
            {
                if (playingAudioId != msgId)
                {
                    //执行结束播放
                    //Player.MediaPlayer.Stop();
                    //Player.SourceProvider.MediaPlayer.OnMediaPlayerEndReached();
                    Task.Factory.StartNew(() =>
                    {
                        //等待Main窗口的Player的EndReached执行完
                        while (true)
                        {
                            if (playingAudioId != true.ToString())
                            {
                                continue;
                            }

                            if (!string.IsNullOrWhiteSpace(path) && FileUtil.CheckFileExists(path))
                            {
                                /*
                                Player.SourceProvider.MediaPlayer.Play(new FileInfo(path));
                                Player.SourceProvider.MediaPlayer.Audio.Volume = 98;//音量
                                */
                                playingAudioId = msgId;
                            }
                            //break;
                            return;
                        }
                    });

                    isPlaying = true;
                }
                else
                {
                    //Player.SourceProvider.MediaPlayer.Pause();
                    isPlaying = false;
                }
                return isPlaying;
            }
            catch (Exception e)
            {
                LogHelper.log.Error(e.Message, e);
                return false;
            }
        }
        #endregion

        #region  ***处理所有消息
        /// <summary>
        /// 创建或者更新MessageListItem
        /// </summary>
        /// <param name="newMsg">需要处理的消息</param>
        internal void CreateOrUpdateMsgItem(Messageobject newMsg)
        {
            App.Current.Dispatcher.Invoke(() =>
         {
             MessageListItem item = new MessageListItem();
             if (RecentMessageList.Count(m => m.Jid == newMsg.jid) > 0)//不存在对应的消息项
             {
                 item = RecentMessageList.FirstOrDefault(m => m.Jid == newMsg.jid);//获取对应项
             }
             else
             {
                 item.Jid = newMsg.jid;
                 item.Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(newMsg.jid);//设置显示的头像路径
                 item.TimeSend = newMsg.timeSend;

                 if (newMsg.jid.Length < 15)
                 {
                     item.MessageTitle = newMsg.fromUserName;
                     
                 }
                 else
                 {
                     item.MessageTitle = newMsg.content;
                 }
                 
                 item.ShowTitle = newMsg.fromUserName;
                 //item.MessageItemContent = newMsg.content;
                 //item.UnReadCount = (newMsg.isMySend == 1 || newMsg.FromId == Applicate.MyAccount.userId) ? (0) : (1);//如果
             }
             InsertOrTopSingleMessageList(item);//置顶目前的消息
             if (Sess.Jid == newMsg.jid)//如果是当前选中会话则直接显示消息与气泡
             {
                 Messenger.Default.Send(newMsg, ChatBubblesNotifications.ShowSingleBubble);//显示消息
                 //MessageListSelectedIndex = 0;//选中第一条
             }
             else
             {
                 switch (newMsg.type)
                 {
                     case kWCMessageType.Text:
                     case kWCMessageType.Image:
                     case kWCMessageType.Voice:
                     case kWCMessageType.Location:
                     case kWCMessageType.Gif:
                     case kWCMessageType.Video:
                     case kWCMessageType.Audio:
                     case kWCMessageType.Card:
                     case kWCMessageType.File:
                     case kWCMessageType.Remind:
                     case kWCMessageType.RedPacket:
                     case kWCMessageType.SystemImage1:
                     case kWCMessageType.SystemImage2:
                     case kWCMessageType.Link:
                     case kWCMessageType.PokeMessage:
                     case kWCMessageType.SDKLink:
                     //break;
                     case kWCMessageType.FriendRequest:
                     case kWCMessageType.RequestAgree:
                     case kWCMessageType.RequestRefuse:
                     case kWCMessageType.NewSub:
                     case kWCMessageType.DeleteNotice:
                     case kWCMessageType.DeleteFriend:
                     case kWCMessageType.BlackFriend:
                     case kWCMessageType.RequestFriendDirectly:
                     case kWCMessageType.CancelBlackFriend:
                     //break;
                     case kWCMessageType.AudioChatCancel:
                     case kWCMessageType.AudioChatEnd:
                     //case kWCMessageType.VideoChatAsk://视频通话请求不显示角标
                     case kWCMessageType.VideoChatCancel:
                     case kWCMessageType.VideoChatEnd:
                         item.UnReadCount++;//累加未读角标
                         TotalUnReadCount++;
                         soundPlayer.Play();//播放提示音
                         Messenger.Default.Send(true, MainViewNotifactions.FlashWindow);
                         break;
                     case kWCMessageType.RoomNameChange:
                     case kWCMessageType.RoomDismiss:
                     case kWCMessageType.RoomExit:
                     case kWCMessageType.RoomNotice:
                     case kWCMessageType.RoomMemberBan:
                     case kWCMessageType.RoomInvite:
                     case kWCMessageType.RoomAdmin:
                     case kWCMessageType.RoomReadVisiblity:
                     case kWCMessageType.RoomIsVerify:
                     case kWCMessageType.RoomIsPublic:
                     case kWCMessageType.RoomInsideVisiblity:
                     case kWCMessageType.RoomUserRecommend:
                     case kWCMessageType.RoomAllBanned:
                     case kWCMessageType.RoomAllowMemberInvite:
                     case kWCMessageType.RoomAllowUploadFile:
                     case kWCMessageType.RoomAllowConference:
                     case kWCMessageType.RoomAllowSpeakCourse:
                     case kWCMessageType.RoomManagerTransfer:
                     //case kWCMessageType.AudioMeetingJoin:
                     case kWCMessageType.AudioMeetingQuit:
                     //case kWCMessageType.AudioMeetingSetSpeaker:
                     //case kWCMessageType.VideoMeetingJoin:
                     //case kWCMessageType.VideoMeetingQuit:
                     case kWCMessageType.VideoMeetingKick:
                     //case kWCMessageType.VideoMeetingInvite:
                     //case kWCMessageType.AudioMeetingInvite:
                     //case kWCMessageType.AudioMeetingAllSpeaker:
                     case kWCMessageType.RoomFileUpload:
                     case kWCMessageType.RoomFileDelete:
                         if (item.MessageItemType == ItemType.Mute)//如果为
                         {
                             break;
                         }
                         item.UnReadCount++;//累加未读角标
                         TotalUnReadCount++;
                         soundPlayer.Play();//播放提示音
                         Messenger.Default.Send(true, MainViewNotifactions.FlashWindow);
                         if (newMsg.jid.Length > 10)//如果Jid为群聊ID
                         {
                             var room = new Room().GetByJid(newMsg.jid);//获取群组信息
                             if (room != null)
                             {
                                 item.ShowTitle = room.name;//设置昵称
                                 if (room.offlineNoPushMsg == 1)//如果不推送的话
                                 {
                                     item.MessageItemType = ItemType.Mute;//如果群组为免打扰则设置项不自增未读角标
                                                                          //break;
                                 }
                             }
                         }
                         break;
                 }
             }
             #region 显示标题
             if (string.IsNullOrWhiteSpace(item.ShowTitle))
             {
                 if (newMsg.jid.Length > 10)//如果Jid为群聊ID
                 {
                     var room = new Room().GetByJid(newMsg.jid);//获取群组信息
                     if (room != null)
                     {
                         item.ShowTitle = room.name;//设置昵称
                         if (room.offlineNoPushMsg == 1)//如果不推送的话
                         {
                             item.MessageItemType = ItemType.Mute;//如果群组为免打扰则设置项不自增未读角标
                                                                  //break;
                         }
                     }
                 }
                 else//如果是单聊消息
                 {
                     if (newMsg.PlatformType > 0)//消息为其他登录设备转发
                     {
                         switch (newMsg.PlatformType)//设置标题和头像
                         {
                             case 1:
                                 item.ShowTitle = "我的Android";
                                 item.Tag = "android";
                                 break;
                             case 2:
                                 item.ShowTitle = "我的iPhone";
                                 item.Tag = "ios";
                                 break;
                             case 3:
                                 item.ShowTitle = "我的Web端";
                                 item.Tag = "web";
                                 break;
                             case 4:
                                 item.ShowTitle = "我的Mac电脑";
                                 item.Tag = "mac";
                                 break;
                             default:
                                 item.ShowTitle = "pc";
                                 item.Tag = "pc";
                                 break;
                         }
                     }
                     else//正常单聊消息类型
                     {
                         if (newMsg.jid == Applicate.MyAccount.userId)//我发出的消息
                         {
                             item.ShowTitle = Applicate.MyAccount.nickname;//使用接收用户昵称
                         }
                         else
                         {
                             item.ShowTitle = new DataOfFriends().GetUserNameByUserId(newMsg.toUserId);//使用发送用户昵称
                         }
                     }
                 }
             }
             #endregion
             if (string.IsNullOrWhiteSpace(item.ShowTitle))//如果项标题为空则移除整个项
             {
                 int tmpindex = RecentMessageList.IndexOf(item);
                 if (tmpindex >= 0)
                 {
                     RemoveMessageListItem(tmpindex);
                 }
             }
             else//标题不为空则更新项内容
             {
                 //显示消息预览内容
                 item.Msg = newMsg;
                 item.MessageItemContent = newMsg.content;
             }
         });
        }
        #endregion

        #region Public Helper

        #region 获取RichTextBox的文本
        /// <summary>
        /// 获取RichTextBox的文本
        /// </summary>
        /// <param name="richTextBox">需要获取文本的rtb对象</param>
        /// <returns>获取到的文本</returns>
        private string GetRTBText(System.Windows.Controls.RichTextBox richTextBox)
        {
            //获取所有的文本
            TextRange textRange = null;
            try
            {
                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output(ex.Message);
            }
            return textRange.Text.Trim();
        }
        #endregion

        #region 获取RichTextBox的文本转字符串
        /// <summary>
        /// 获取RichTextBox的文本转字符串
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="objectId">@群成员userId</param>
        /// <returns></returns>
        private string ConvertDocumentToText(FlowDocument documents, ref string objectId)
        {
            //获取所有的文本
            StringBuilder sb = new StringBuilder();
            int blockCount = documents.Blocks.Count;
            //以块的形式发送
            foreach (Paragraph items in documents.Blocks)
            {
                foreach (var item in items.Inlines)
                {
                    sb.Append(SetData(item, ref objectId));
                }
                sb.Append("\n");
            }
            return sb.ToString().TrimEnd('\n');
        }

        private string ConvertParagraphToText(Paragraph paragraph, ref string objectId)
        {
            //获取所有的文本
            StringBuilder sb = new StringBuilder();
            //以块的形式发送
            foreach (var item in paragraph.Inlines)
            {
                sb.Append(SetData(item, ref objectId));
            }
            return sb.ToString();
        }
        #endregion

        #region 从Document中获取媒体对象
        /// <summary>
        /// 从Document中获取媒体对象
        /// </summary>
        /// <param name="document"></param>
        static Random rand = new Random();
        private List<Image> GetObjectFromDocument(FlowDocument document)
        {
            List<Image> tmplist = new List<Image>();
            foreach (BlockUIContainer item in document.Blocks)
            {
                //var tmm = new BlockUIContainer();
                string strDateTime = DateTime.Now.GetDateTimeFormats('s')[0].ToString().Replace(":", "");
                if (item.Child is Image)
                {
                    var tmpimg = (Image)item.Child;
                    if (tmpimg.Tag == null)
                    {
                        Bitmap img = Helpers.ImageSourceToBitmap(tmpimg.Source);
                        //string path = Applicate.LocalConfigData.ChatDownloadPath + DateTime.Now.ToLongTimeString() + ".png";
                        string path = Applicate.LocalConfigData.ChatDownloadPath + "图片发送" +
                            strDateTime + ".png";
                        img.Save(path);
                        tmpimg.Tag = path;//设置路径
                    }

                    tmplist.Add(tmpimg);
                }
                else if (item.Child != null)
                {

                }
            }
            return tmplist;
        }
        private Image GetObjectFromBlockUIContainer(BlockUIContainer blockUIContainer)
        {
            Image tmpimg = new Image();
            string strDateTime = DateTime.Now.GetDateTimeFormats('s')[0].ToString().Replace(":", "");
            if (blockUIContainer.Child is Image)
            {
                tmpimg = (Image)blockUIContainer.Child;
                if (tmpimg.Tag == null)
                {
                    Bitmap img = Helpers.ImageSourceToBitmap(tmpimg.Source);
                    //string path = Applicate.LocalConfigData.ChatDownloadPath + DateTime.Now.ToLongTimeString() + ".png";
                    string path = Applicate.LocalConfigData.ChatDownloadPath + "图片发送" +
                        strDateTime + rand.Next(10000) + ".png";
                    img.Save(path);
                    tmpimg.Tag = path;//设置路径
                }
            }
            else if (blockUIContainer.Child != null)
            {

            }
            return tmpimg;
        }
        #endregion

        #region 设置行元素
        /// <summary>
        /// 设置行元素
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        private string SetData(Inline item, ref string objectId)
        {
            string v = "";
            if (item is Run)
            {
                var text = (item as Run).Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    return "";
                }
                if (text.StartsWith("@") && item.Tag != null)
                {
                    objectId += item.Tag.ToString() + ",";
                }
                v = text;
            }
            else if (item is Span)
            {
                var s = item as Span;
                v = " ";
            }
            else if (item is InlineUIContainer)
            {
                var child = item as InlineUIContainer;
                var image = child.Child as Image;
                if (image == null)
                {
                    return " ";
                }
                string tmp = (image.Tag == null) ? ("") : (image.Tag.ToString());
                v = (image.Tag == null) ? ("") : (image.Tag.ToString());
            }
            else
            {

            }
            return v;
        }
        #endregion


        #endregion
    }
}
