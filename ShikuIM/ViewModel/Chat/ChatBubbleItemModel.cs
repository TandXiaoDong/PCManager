using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using ShikuIM.ViewModel;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Linq;

namespace ShikuIM.Model
{

    /// <summary>
    /// Chat Bubble Model(Inherited from <see cref="Messageobject"/>)
    /// </summary>
    public class ChatBubbleItemModel : Messageobject
    {

        #region Private Member
        private bool isSelected;
        private PackIconKind stateIcon;
        private string readToolTip;
        private string msgStateText;
        private MessageListItem GroupItem;
        private Room displayRoom = new Room();

        #endregion

        #region Public Member 

        /// <summary>
        /// 气泡高度
        /// </summary>
        public double BubbleHeight { get; set; }

        /// <summary>
        /// 消息显示状态
        /// </summary>
        public string MsgStateText
        {
            get
            {
                switch (messageState)
                {
                    case -1:
                        msgStateText = "发送失败";//发送失败
                        break;
                    case 0:
                        msgStateText = "发送中";//正在发送
                        break;
                    case 1:
                        msgStateText = "送达";//送达
                        break;
                    case 2:
                        msgStateText = "已读";//已读
                        break;
                    default:
                        msgStateText = "发送中";//发送中
                        break;
                }
                return msgStateText;
            }
            set
            {
                msgStateText = value;
                RaisePropertyChanged(nameof(MsgStateText));
            }
        }
        
        /// <summary>
        /// 消息状态提示文本
        /// </summary>
        public string ReadToolTip
        {
            get
            {
                switch (messageState)
                {
                    case -1:
                        readToolTip = "发送失败";//发送失败
                        break;
                    case 0:
                        readToolTip = "发送中";//正在发送
                        break;
                    case 1:
                        readToolTip = "送达";//送达
                        break;
                    case 2:
                        readToolTip = readPersons == "0" ? "" : readPersons + "已读";//已读
                        break;
                    default:
                        readToolTip = "发送中";//已读
                        break;
                }
                return readToolTip;
            }
            set
            {
                readToolTip = value;
                RaisePropertyChanged(nameof(ReadToolTip));
            }
        }

        /// <summary>
        /// 状态图标
        /// </summary>
        public PackIconKind StateIcon
        {
            get
            {
                switch (messageState)
                {
                    case -1:
                        stateIcon = PackIconKind.AlertCircle;//发送失败
                        break;
                    case 0:
                        stateIcon = PackIconKind.Timer;//正在发送
                        break;
                    case 1:
                        stateIcon = PackIconKind.Check;//送达
                        break;
                    case 2:
                        stateIcon = PackIconKind.CheckAll;//已读
                        break;
                    default:
                        stateIcon = PackIconKind.Timer;//已读
                        break;
                }
                return stateIcon;
            }
            set { stateIcon = value; RaisePropertyChanged(nameof(StateIcon)); }
        }

        /// <summary>
        /// Chat Item Selected state
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// 当前群组详情
        /// </summary>
        public Room DisplayRoom
        {
            get { return displayRoom; }
            set { displayRoom = value; RaisePropertyChanged(nameof(DisplayRoom)); }
        }
        #endregion

        #region Commands

        public ICommand BubbleLoadCommand { get; set; }


        /// <summary>
        /// 头像点击命令
        /// </summary>
        public ICommand AvatorCommand { get; set; }

        /// <summary>
        /// 复制消息
        /// </summary>
        public ICommand CopyCommand { get; set; }

        /// <summary>
        ///转发消息
        /// </summary>
        public ICommand ForwardCommand { get; set; }

        /// <summary>
        /// 撤销消息
        /// </summary>
        public ICommand WithDrawCommand { get; set; }

        /// <summary>
        /// 删除消息
        /// </summary>
        public ICommand MultiSelectCommand { get; set; }

        /// <summary>
        /// 删除消息
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        #endregion

        #region Contructor
        public ChatBubbleItemModel()
        {
            InitialMessengers();
            #region Initial Commands
            CopyCommand = new RelayCommand(CopyMessage);
            ForwardCommand = new RelayCommand(ForWardMessage);
            WithDrawCommand = new RelayCommand(WithDrawMessage);
            DeleteCommand = new RelayCommand(DeleteMessage);
            AvatorCommand = new RelayCommand<string>(AvatorDetial);
            BubbleLoadCommand = new RelayCommand<RoutedEventArgs>(BubbleLoaded);
            MultiSelectCommand = new RelayCommand(MultiSelectMessage);
            #endregion
        }
        #endregion

        private void BubbleLoaded(RoutedEventArgs obj)
        {
            var tmp = (FrameworkElement)obj.Source;
            BubbleHeight = tmp.ActualHeight;
        }

        #region 注册通知
        /// <summary>
        /// 注册通知
        /// </summary>
        private void InitialMessengers()
        {
            Messenger.Default.Register<MessageListItem>(this, ChatBubblesNotifications.SetMessageInfo, (item) => { SetMsgInfoMessage(item); });
        }

        /// <summary>
        /// 加载群详情
        /// </summary>
        /// <param name="msgItem"></param>
        public void SetMsgInfoMessage(MessageListItem msgItem)
        {
            GroupItem = msgItem;

            var tempRoom = new Room().GetByRoomId(GroupItem.Id);//从数据库获取
            if (tempRoom != null)
            {
                DisplayRoom = tempRoom;
            }
            Messenger.Default.Send(tempRoom, GroupDetialViewModel.InitialGroupDetial);//初始化群组
        }

        #endregion

        #region 头像点击详情
        /// <summary>
        /// 头像点击详情
        /// </summary>
        /// <param name="para">参数</param>
        private void AvatorDetial(string para)
        {
            DataofMember dm = DisplayRoom.members.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId);
            if (GroupItem.Jid.Length > 15 && DisplayRoom.allowSendCard == 0)
            {
                
                if (DisplayRoom.userId.ToString() != Applicate.MyAccount.userId)
                {
                    return;
                }
            }

            if (para.Length > 15)
            {
                var tmpRoom = new Room().GetRoomIdByJid(para);//获取
                GroupChatDetial.GetWindow().Show();
                Messenger.Default.Send(tmpRoom, GroupDetialViewModel.InitialGroupDetial);//初始化群组
                GroupChatDetial.GetWindow().Activate();
            }
            else
            {
                if (Applicate.MyAccount.userId != para) // 点自己的头像不做操作
                {
                    if (para == "10001")
                    {
                        //新的好友不显示个人资料
                    }
                    else
                    {
                        Messenger.Default.Send(para, UserDetailNotifications.ShowUserDetial);
                        UserDetailView.GetWindow().Show();
                    }
                }
            }
        }
        #endregion

        #region 删除消息
        /// <summary>
        /// 删除消息
        /// </summary>
        private void DeleteMessage()
        {
            Messenger.Default.Send((this as Messageobject), ChatBubblesNotifications.DeleteMessage);
            this.Delete();//数据库删除
        }
        #endregion

        #region 撤回消息
        /// <summary>
        /// 撤回消息
        /// </summary>
        private void WithDrawMessage()
        {
            var now = Helpers.DatetimeToStamp(DateTime.Now);
            var lack = now - timeSend;//获取时间差
            if (lack <= 300)//五分钟内允许撤回
            {
                Messenger.Default.Send((this as Messageobject), MainViewNotifactions.WithDrawMessage);//通知撤回消息
            }
            else
            {
                Messenger.Default.Send("消息已超过5分钟,不允许撤回", MainViewNotifactions.SnacbarMessage);//通知主页面显示提示信息
            }
        }
        #endregion

        #region 转发消息
        private void ForWardMessage()
        {
            //转发消息
            Messenger.Default.Send(this as Messageobject, MainViewNotifactions.ForwardMessage);//转发单条消息
        }
        #endregion

        #region 复制信息
        /// <summary>
        /// 复制信息
        /// </summary>
        private void CopyMessage()
        {
            switch (this.type)
            {
                case kWCMessageType.Text:
                    Clipboard.SetDataObject(content, true);
                    break;
                case kWCMessageType.Image:
                    string localpath = Applicate.LocalConfigData.ChatDownloadPath;
                    string filename = fileName.Substring(
                        fileName.LastIndexOf('/') + 1,
                        fileName.Length - fileName.LastIndexOf('/') - 1
                        );
                    var localfile = localpath + filename;
                    Clipboard.SetDataObject(new Bitmap(localfile), true);
                    break;
                case kWCMessageType.Video:
                    string localpath_video = Applicate.LocalConfigData.ChatDownloadPath;
                    string filename_video = fileName.Substring(
                        fileName.LastIndexOf('/') + 1,
                        fileName.Length - fileName.LastIndexOf('/') - 1
                        );
                    var localfile_video = localpath_video + filename_video;
                    IDataObject data = new DataObject(DataFormats.FileDrop, new string[] { localfile_video });
                    MemoryStream memo = new MemoryStream(4);
                    byte[] bytes = new byte[] { (byte)(5), 0, 0, 0 };
                    memo.Write(bytes, 0, bytes.Length);
                    data.SetData("copy_video", memo);
                    Clipboard.SetDataObject(data, true);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 多选
        /// <summary>
        /// 信息多选
        /// </summary>
        private void MultiSelectMessage()
        {

        }
        #endregion
    }
}
