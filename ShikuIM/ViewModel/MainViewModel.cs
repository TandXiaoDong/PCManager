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
        /// �Ի���������� 
        /// </summary>
        public enum DialogFunction
        {
            /// <summary>
            /// ������ϵ��
            /// </summary>
            SendContact,

            /// <summary>
            /// ת����Ϣ
            /// </summary>
            ForwardMessage,

            /// <summary>
            /// @Ⱥ��Ա
            /// </summary>
            AttentionGroupMember,

            /// <summary>
            /// Ⱥ��Ƶ����
            /// </summary>
            AudioChatWithGroupMember,

            /// <summary>
            /// Ⱥ��Ƶ����
            /// </summary>
            VideoChatWithGroupMember,

        }
        #endregion

        #region Private Member
        /// <summary>
        /// ����������Ϣ��Timer
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
        private SolidColorBrush accountStateFill;//�û�״̬��ɫ
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
        private string sendAsFileTooltip = "��Ϊ�ļ�����";
        private string sencAsImageTooltip = "��ΪͼƬ����";
        private bool sendImageTooltipVisible;
        private bool sendFileTooltipVisible;
        private DialogFunction dialogFunctionType;
        #endregion

        #region Public Member

        /// <summary>
        /// �Ի����������
        /// </summary>
        public DialogFunction DialogFunctionType
        {
            get { return dialogFunctionType; }
            set { dialogFunctionType = value; RaisePropertyChanged(nameof(DialogFunctionType)); }
        }

        /// <summary>
        /// ��ʾ��������
        /// </summary>
        public SoundPlayer soundPlayer { get; set; }

        /// <summary>
        /// ѡ����ϵ��(ֻ�ܰ�List < Object >����)
        /// </summary>
        public List<object> SelectedContacts
        {
            get { return selectedContacts; }
            set { selectedContacts = value; RaisePropertyChanged(nameof(SelectedContacts)); }
        }

        /// <summary>
        /// �Ի�������ʾ����ϵ�� (һ������ת���ͷ�����Ƭ)
        /// </summary>
        public ObservableCollection<MessageListItem> DialogContacts { get; set; }

        /// <summary>
        /// �����ı���TextSelection
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
        /// SnackBarMessage ����
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackBar; }
            set { snackBar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }

        /// <summary>
        /// �����ļ���ʾ��Ƭ��ʾ
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
        /// ����ͼƬ��ʾ��Ƭ��ʾ
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
        /// ����ͼƬ��ʾ�ı�
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
        /// �����ļ���ʾ�ı�
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
        /// ���ı����
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
        /// �Ƿ����ڷ����ļ�
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
        /// ���������
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
        /// ѡ�к��������Ƿ���ʾ
        /// </summary>
        public bool SelectedFriendProfileVisiblity
        {
            get { return selectedFriendProfileVisiblity; }
            set { selectedFriendProfileVisiblity = value; RaisePropertyChanged(nameof(SelectedFriendProfileVisiblity)); }
        }

        /// <summary>
        /// ������ѡ������
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
        /// ����ѡ�е�����
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
                FriendSelectionChanged();//ѡ�иı�ʱ
            }
        }

        /// <summary>
        /// �ҵ�Ⱥ����ѡ������
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
                MyGroupSelectionChanged();//ѡ����ı�ʱ
            }
        }

        /// <summary>
        /// Ⱥ������ʾ
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
        /// Ⱥ��������ʾ
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
        /// �û�����״̬(��ɫ)
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
        /// �û�����״̬(������ʾ)
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
        /// ����ʾȺ��Ա��������ʾ��
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
        /// ����ѡ��Tab(0Ϊ����, 1Ϊ������)
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
                FriendTypeChanged();//ѡ�иı�ʱ
            }
        }

        /// <summary>
        /// ��Ϣѡ������
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
        /// ��ǰ�ѵ�¼����
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
        /// ��ǰ����Ự����
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
                RaisePropertyChanged(nameof(MainTabSelectedIndex));//���ƻỰ���������ʾ
            }
        }

        /// <summary>
        /// ��ͼ�����б�
        /// </summary>
        public ObservableCollection<string> GifEmotionList { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Emoji�����б�
        /// </summary>
        public ObservableCollection<string> EmojiEmotionList { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// �����б�
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
        /// �������б�
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
        /// �����Ϣ�б�
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
        /// �ҵķ����б�
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
        /// ѡ�е�Ⱥ��Ա��Ϣ
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
        /// δ��������
        /// </summary>
        public int TotalUnReadCount
        {
            get
            { return unReadCount; }
            set
            {
                if (value < 0)//δ����������Ϊ��
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
        /// ��ǰѡ�е�ҳ��
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
        /// ѡ�еĺ���
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
        /// Ⱥ��
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
        /// �Ҳ�Ⱥ�ĵİ�ť����
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
        /// �ر�����
        /// </summary>
        public ICommand CloseCommand => new RelayCommand(() => { Application.Current.MainWindow.Hide(); });

        /// <summary>
        /// ���
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
        /// ��С������
        /// </summary>
        public ICommand MinCommand => new RelayCommand(() => { Application.Current.MainWindow.Hide(); });

        /// <summary>
        /// ��Ϣɾ�����
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
        /// �鿴����������
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
        /// ɾ�����ѵ��
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
                        Snackbar.Enqueue("����ɾ���ͷ����ں�!");
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
        /// ���������
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
                        Snackbar.Enqueue("��������ϵͳ�˺�!");
                        return;
                    }
                    BlackListAdd(friItem);
                });
            }
        }
        /// <summary>
        /// ���������
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
        /// Ⱥ�ķ�����Ϣ
        /// </summary>
        public ICommand GroupSendMessageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MainTabSelectedIndex == 2)
                    {
                        var item = MyGroupList[MyGroupSelectedIndex];//��ȡ��
                        MainTabSelectedIndex = 0;//ת����Ϣ����
                        StartNewChatFromItem(item.Clone());//��ת����
                    }
                });
            }
        }
        /// <summary>
        /// Ⱥ��������
        /// </summary>
        public ICommand GroupDetailCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    string RoomId = MyGroupList[MyGroupSelectedIndex].Id;
                    GroupChatDetial.GetWindow().Show();
                    Messenger.Default.Send(RoomId, GroupDetialViewModel.InitialGroupDetial);//��ʼ��Ⱥ��
                    GroupChatDetial.GetWindow().Activate();
                });
            }
        }
        /// <summary>
        /// �˳�Ⱥ�ĵ��
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
        /// ��������Ӱ�ť���
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
                        ServiceLocator.Current.GetInstance<GroupCreateViewModel>().InitialProperties();//��������
                        GroupCreate.GetGroupCreate().Show();//��ʾ���Ⱥ�ĵĴ���
                    }
                });
            }
        }

        /// <summary>
        /// ����������Ϣ����
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

                 //�����ı����ı�
                 TextFieldDocument.Blocks.Clear();
                 var emptypara = new Paragraph(new Run(""));
                 TextFieldDocument.Blocks.Add(emptypara);
                 main.rtb_sendMessage.ThisRichText.Focus();
             }
             catch (Exception ex)
             {
                 Snackbar.Enqueue("��ʾ��ͼƬ�����ּ��뻻�У�");
             }             
         });

        private void OnSendMessage(string strMessage, string objectId, bool isFile)
        {
            var main = Applicate.GetWindow<MainWindow>();
            main.rtb_sendMessage.ThisRichText.Focusable = true;
            if (string.IsNullOrEmpty(strMessage))
            {
                main.rtb_sendMessage.ThisRichText.Focus();
                //Snackbar.Enqueue("���ܷ��Ϳհ���Ϣ");
                return;
            }
            else
            {
                if (Sess.Jid == null)
                {
                    Snackbar.Enqueue("��ѡ��һλ�������");
                    return;
                }
                if (CheckIsBanned(Sess.Jid))//����
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
                    }, strMessage, objectId);//����
                }
            }
        }

        /// <summary>
        /// ���ı��� ctrl+V
        /// </summary>
        public ICommand KeyOfPasteCommand => new RelayCommand(() =>
        {
            System.Windows.MessageBox.Show("ctrl+v");
        });

        /// <summary>
        /// ����������Ϣ�������
        /// </summary>
        public ICommand SessionTitleCommand => new RelayCommand(() =>
        {
            if (Sess.Jid.Length > 15)
            {
                var tmpRoom = new Room().GetRoomIdByJid(Sess.Jid);//��ȡ
                GroupChatDetial.GetWindow().Show();
                Messenger.Default.Send(tmpRoom, GroupDetialViewModel.InitialGroupDetial);//��ʼ��Ⱥ��
                GroupChatDetial.GetWindow().Activate();
            }
            else
            {
                if (Sess.Jid == "10001")
                {
                    //�µĺ��Ѳ���ʾ��������
                }
                else
                {
                    Messenger.Default.Send(Sess.Jid, UserDetailNotifications.ShowUserDetial);
                    UserDetailView.GetWindow().Show();
                }
            }
        });

        /// <summary>
        /// �鿴�û���������
        /// </summary>
        public ICommand UserDetailCommand => new RelayCommand<string>((userId) =>
        {
            if (userId == Applicate.MyAccount.userId)
            {
                DataOfUserDetial._avatarName = FileUtil.GetFileName(FileUtil.GetDirFiles(LocalConfig.userAvatorPath,userId),false);
                Messenger.Default.Send(true, UserDetailNotifications.ShowMyDetial);//��ʾ��������
                Personal.GetPersonal().Show();
            }
            else
            {
                Messenger.Default.Send(userId, UserDetailNotifications.ShowUserDetial);
                UserDetailView.GetWindow().Show();
            }
        });

        /// <summary>
        /// �鿴Ⱥ��֤����
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
        /// �����ļ�����
        /// </summary>
        public ICommand SendFileCommand => new RelayCommand(() =>
        {
            if (CheckIsBanned(Sess.Jid))//����
            {
                return;
            }
            //�ļ���ť��ѡ�в���һ���ļ������ϴ�����
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = true;//�ɶ�ѡ
            fd.Filter = "�ļ�|*.*|ͼƬ|*.jpg;*.png;*.jpeg;*.bmp";//�ļ�ɸѡ��
            fd.ShowDialog();
            //�Ƿ�Ϊ
            if (fd.FileNames.Length > 0)
            {
                foreach (var fileName in fd.FileNames)
                {
                    ConsoleLog.Output("////******************************ѡ�е��ļ�Ϊ" + fileName);
                    //�첽������Ϣ
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
        /// �Ի����з��Ͱ�ť����
        /// </summary>
        public ICommand DialogSendCommand => new RelayCommand<object>((para) =>
        {
            DialogHost.CloseDialogCommand.Execute(null, null);//���ȹرնԻ���
            var selectedtmps = new List<MessageListItem>();
            for (int i = 0; i < SelectedContacts.Count; i++)
            {
                selectedtmps.Add((MessageListItem)SelectedContacts[i]);
            }
            if (SelectedContacts.Count > 0)//�ǿ���֤
            {
                SelectedContacts.Clear();//�ָ�
                var msgtype = kWCMessageType.VideoMeetingInvite;//Ĭ��Ϊ��Ƶ����
                switch (DialogFunctionType)
                {
                    case DialogFunction.SendContact://������ϵ��
                        ShiKuManager.SendContacts(new MessageListItem
                        {
                            Jid = Sess.Jid,
                            ShowTitle = Sess.NickName,
                            MessageItemContent = Sess.MyMemberNickname,
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Sess.Jid)
                        }, selectedtmps);
                        break;
                    case DialogFunction.ForwardMessage://ת����Ϣ
                        ShiKuManager.ForwardMessage(Applicate.ForwardMessageList, selectedtmps);
                        break;
                    case DialogFunction.AttentionGroupMember://@Ⱥ��Ա
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
                    case DialogFunction.AudioChatWithGroupMember://Ⱥ����Ƶ����
                    case DialogFunction.VideoChatWithGroupMember://Ⱥ����Ƶ����
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
                        if (DialogFunctionType == DialogFunction.AudioChatWithGroupMember)//���ݶԻ��������ж���Ϣ����
                        {
                            //msgtype = kWCMessageType.AudioMeetingInvite;
                            //������������
                            for (int i = 0; i < selectedtmps.Count; i++)
                            {
                                ShiKuManager.SendAudioMeetingAsk(Sess.RoomId, Sess.Jid, selectedtmps[i].Jid, selectedtmps[i].MessageTitle);//��������
                            }
                            selectedtmps[0].Id = Sess.Jid;//����Ⱥ��JID
                            Messenger.Default.Send(tmpRoom, CommonNotifications.AudioChatRequest);
                        }
                        else
                        {
                            //������Ƶ����
                            for (int i = 0; i < selectedtmps.Count; i++)
                            {
                                ShiKuManager.SendVideoMeetingAsk(Sess.RoomId, Sess.Jid, selectedtmps[i].Jid, selectedtmps[i].ShowTitle);//��������
                            }
                            Messenger.Default.Send(tmpRoom, CommonNotifications.VideoChatRequest);
                        }
                        break;
                    default:
                        break;
                }
                DialogContacts.Clear();//������ʱ�Ի����к���
            }
            else
            {
                Snackbar.Enqueue("������ѡ��һ���û�");//�쳣��ʾ
            }
        });

        /// <summary>
        /// ��ʾ��������ϵ���б�
        /// </summary>
        public ICommand ShowContactCommand => new RelayCommand(() =>
        {
            if (CheckIsBanned(Sess.Jid, true))//������
            {
                return;
            }
            var SendCards = App.Current.MainWindow.FindResource("DialogContactCard");//��ȡ�Ի���
            DialogHost.OpenDialogCommand.Execute(SendCards, null);//�򿪷�����ϵ�˶Ի���
            DialogContacts.Clear();
            DialogFunctionType = DialogFunction.SendContact;//����Ϊ������Ƭ
            DialogContacts.AddRange(FriendList);//��Ӻ�������ϵ��Ԥ������
        });

        /// <summary>
        /// ��ʷ��¼����
        /// </summary>
        public ICommand ChatHistoryCommand => new RelayCommand(() =>
        {
            if (Sess.Jid == null)
            {
                //Snackbar.Enqueue("����ѡ��һλ��������������");
                return;
            }
            ChatRecord.ShowChatRecord(Sess.Jid).Show();
        });

        /// <summary>
        /// �����
        /// </summary>
        public ICommand SendRedPacketCommand => new RelayCommand(() =>
        {
            if (CheckIsBanned(Sess.Jid, true))//������
            {
                return;
            }
            var SendCards = App.Current.MainWindow.FindResource("DialogContactCard");//��ȡ�Ի���
            DialogHost.OpenDialogCommand.Execute(SendCards, null);//�򿪷�����ϵ�˶Ի���
            DialogContacts.Clear();
            DialogFunctionType = DialogFunction.SendContact;//����Ϊ������Ƭ
            DialogContacts.AddRange(FriendList);//��Ӻ�������ϵ��Ԥ������
        });

        /// <summary>
        /// ���������������
        /// </summary>
        public ICommand ChatDragDropCommand => new RelayCommand(() =>
        {
            SendFileTooltipVisible = false;
            SendImageTooltipVisible = true;
        });

        /// <summary>
        /// ���������ϢΪ�Ѷ�
        /// </summary>
        public ICommand AllRecentUnReadCommand => new RelayCommand(() =>
        {
            TotalUnReadCount = 0;
            //RecentMessageList.
        });

        /// <summary>
        /// ����������벢��������
        /// </summary>
        public ICommand ChatDragEnterCommand => new RelayCommand<System.Windows.DragEventArgs>((eve) =>
        {
            if (eve.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[])eve.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (paths != null && paths.Length != 0)
                {
                    if (!paths[0].Contains('.'))//��������ļ�������
                    {
                        return;
                    }
                    if (paths.Length > 1)//���Ͷ���ļ�ֻ�����ļ���ʽ����
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
                    else//�����ļ�ʱ
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
        /// �����ļ���Ƭ��ʾ�ſ���
        /// </summary>
        public ICommand FileToolTipCardDropCommand => new RelayCommand<System.Windows.DragEventArgs>((e) =>
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (!paths[0].Contains('.'))//��������ļ�������
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
                }, paths[0]);//ֱ�ӷ���ͼƬ
            }
        });

        /// <summary>
        /// ����ͼƬ��Ƭ��ʾ�ſ���
        /// </summary>
        public ICommand ImageToolTipCardDropCommand => new RelayCommand<System.Windows.DragEventArgs>((e) =>
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (!paths[0].Contains('.'))//��������ļ�������
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
                }, paths[0]);//ֱ�ӷ���ͼƬ
            }
        });


        /// <summary>
        /// 
        /// </summary>
        public ICommand SelectedMessageItemChangedCommand { get; set; }

        /// <summary>
        /// ��������-->�����ô���
        /// </summary>
        public ICommand SettingCommand => new RelayCommand(() => { SettingsWindow.getSetUp().Show(); });

        /// <summary>
        /// ����(˫��/Enter��/����Ϣ������)
        /// </summary>
        public ICommand GoToChatFromItemCommand => new RelayCommand(() =>
        {
            //����ѡ����
            MessageListItem item = null;
            //�ж�
            if (MainTabSelectedIndex == 1)
            {
                if (FriendTypeIndex == 0)
                {
                    item = FriendList[FriendSelectedIndex];//��ȡ������Ϣ
                }
                else
                {
                    ConsoleLog.Output("�����˴��������--����������");
                    return;
                }
            }
            else
            {
                item = MyGroupList[MyGroupSelectedIndex];//��ȡȺ����Ϣ
            }
            StartNewChatFromItem(item.Clone());//����MsgItem�½�����
        });

        /// <summary>
        /// ����(˫��/Enter��/����Ϣ������)
        /// </summary>
        public ICommand EmojiShowCommand => new RelayCommand<FrameworkElement>((emojiView) =>
        {
            emojiView.Focus();
        });

        /// <summary>
        /// ���Emoji���鵽�ı�����
        /// </summary>
        public ICommand AddEmojiToTxtCommand { get; set; }

        /// <summary>
        /// ����Gif��������
        /// </summary>
        public ICommand SendGifCommand => new RelayCommand<string>((emoji) =>
        {
            if (CheckIsBanned(Sess.Jid))//��ֹ����
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
        /// ��������Xmpp
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
            InitialRecentList();//��ӻ��������Ϣ
            InitialEmotion();//��ʼ������
            #region Initial Commands
            SelectedMessageItemChangedCommand = new RelayCommand(SelectedMessageItemChanged);
            AddEmojiToTxtCommand = new RelayCommand<string>(AddEmojiToTextbox);
            #endregion
            //SetTotalUnReadCount();//����δ���Ǳ�
            #region Initial MessageItemList
            if (RecentMessageList.Count(m => m.Jid == "10001") == 0)
            {
                var newFriendItem = new MessageListItem()
                {
                    ShowTitle = "�µ�����",
                    MessageTitle = "�µ�����",
                    Jid = "10001",
                    MessageItemType = ItemType.VerifyMsg,
                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath("10001")
                };
                RecentMessageList.Add(newFriendItem);
                newFriendItem.Insert();
            }
            #endregion
            RegisterMessengers();//ע��֪ͨ
            ShiKuManager.InitialPlatformList();//��ʼ��ƽ̨�豸�б�
            soundPlayer = new SoundPlayer();
            soundPlayer.Stream = ShikuRec.Windows_Notify_Messaging;//����
            DialogContacts = new ObservableCollection<MessageListItem>();
            TypingTimer.Interval = 30000;//��ʾ30����������
            TypingTimer.AutoReset = false;//ֻ����һ��
            TypingTimer.Elapsed += (sen, eve) =>
            {
                Sess.IsOnlineText = "����";//30
            };
        }
        #endregion

        #region ע��֪ͨ
        /// <summary>
        /// ע��֪ͨ  (ע�Ϳɽ��������Notifaction�����ϲ鿴)    
        /// </summary>
        private void RegisterMessengers()
        {
            //ע����������Ϣ��
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.MainAddRecentItem, item => AddSingleMessageList(item));
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.MainInsertRecentItem, item => InsertOrTopSingleMessageList(item));
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.UpdateAccountName, remarkitem => OnUserNameChanged(remarkitem));
            Messenger.Default.Register<int>(this, MainViewNotifactions.MainChangeRecentListIndex, index => MessageListSelectedIndex = index);
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.MainAddFriendListItem, item => AddToFriendList(item));
            Messenger.Default.Register<int>(this, MainViewNotifactions.MainGoToPage, index => MainTabSelectedIndex = index);
            Messenger.Default.Register<string>(this, MainViewNotifactions.SnacbarMessage, smsg => Snackbar.Enqueue(smsg));//��ʾ��Ϣ
            Messenger.Default.Register<Messageobject>(this, MainViewNotifactions.CreateOrUpdateRecentItem, msg => CreateOrUpdateMsgItem(msg));//��������������Ϣ��Ŀ
            Messenger.Default.Register<MessageListItem>(this, MainViewNotifactions.UpdateRecentItemContent, (item) => { UpdateRecentItemContent(item); });
            Messenger.Default.Register<XmppConnectionState>(this, MainViewNotifactions.XmppConnectionStateChanged, (state) => { SetXmppState(state); });
            Messenger.Default.Register<bool>(this, MainViewNotifactions.InputTextChanged, (item) => { InputTextChanged(); });
            Messenger.Default.Register<DataOfUserDetial>(this, CommonNotifications.UpdateMyAccountDetail, (detial) => { UpdateMe(detial); });
            Messenger.Default.Register<Room>(this, CommonNotifications.AddGroupMemberSize, (room) =>
            {
                if (Sess.Jid == room.jid)//���ñ���
                {
                    SetChatTitle(room.jid, room.name);
                }
                if (MyGroupSelectedIndex >= 0)
                {
                    if (MyGroupList[MyGroupSelectedIndex].Jid == room.jid)//����ѡ��Ⱥ����Ҳ�����
                    {
                        DisplayGroup = room;//����ҳ������
                        SelectedGroupMember.Clear();
                        SelectedGroupMember.AddRange(room.members);//�������
                    }
                }
            });
            Messenger.Default.Register<Dictionary<string, List<DataofMember>>>(this, CommonNotifications.RemoveGroupMember, (members) =>
            {
                var newitems = members.First();//��ȡ��Ӧ�ĳ�Ա����
                if (Sess.Jid == newitems.Key)//����Ự���ڶ�Ӧ��
                {
                    SetChatTitle(newitems.Key, "");//���±���
                }
                if (MyGroupSelectedIndex >= 0)
                {
                    if (MyGroupList[myGroupSelectedIndex].Jid == newitems.Key)//��ǰѡ�ж���ƥ��
                    {
                        var tmpitem = SelectedGroupMember.FirstOrDefault(m => m.userId == newitems.Value[0].userId);//��ʾ
                        if (SelectedGroupMember.Count(dm => dm.userId == tmpitem.userId) > 0)//������ڶ�Ӧ��Ա�Ļ�
                        {
                            SelectedGroupMember.Remove(tmpitem);//ɾ��Ⱥ��Ա
                        }
                    }
                }
            });
            Messenger.Default.Register<string>(this, MainViewNotifactions.MainRemoveGroupItem, jid => RemoveFromGroupList(jid));
            Messenger.Default.Register<MessageListItem>(this, CommonNotifications.UpdateGroupMemberNickname, item =>
            {
                if (Sess.Jid.Length > 10)//�����ǰ����Ⱥ��Ự
                {
                    if (item.Jid == Applicate.MyAccount.userId)
                    {
                        Sess.MyMemberNickname = item.ShowTitle;//�����Լ���Ⱥ���е��ǳ�
                    }
                }
            });
            ////���غ���/������/Ⱥ���б�֪ͨ
            Messenger.Default.Register<bool>(this, MainViewNotifactions.MainViewLoadFriendList, res =>
            {
                Task.Run(() =>
                {
                    /*
                    string[] platforms = new string[] { "android", "ios", "mac", "web" };
                    FriendList.Clear();
                    //�������ݿ�
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
                DialogContacts.Clear();//�Է���һ�������
                //���������ϵ��
                DialogContacts.AddRange(FriendList);//��Ӻ����б�
                DialogContacts.AddRange(MyGroupList);//���Ⱥ���б�
                ConsoleLog.Output(DialogContacts.Count);
                var contacts = App.Current.MainWindow.FindResource("DialogContactCard");//��ȡת����ϵ���б�ؼ�
                DialogHost.OpenDialogCommand.Execute(contacts, null);
                Applicate.ForwardMessageList.Clear();
                Applicate.ForwardMessageList.Add(msg);//���ת����Ϣʵ��
                DialogFunctionType = DialogFunction.ForwardMessage;//����Ϊ����
            });
            Messenger.Default.Register<Messageobject>(this, MainViewNotifactions.WithDrawMessage, msg => { WithDrawMsg(msg); });
            Messenger.Default.Register<Room>(this, CommonNotifications.CreateNewGroupFinished, room => OnCreateNewGroup(room));//
            Messenger.Default.Register<string>(this, MainViewNotifactions.CancelBlockItem, item => OnCancelBlack(item));
            ////Xmpp֪ͨע��
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgRecived, msg => { ProcessNewMessage(msg); });//�յ���Ϣ
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgReceipt, (msg) => MsgReceipt(msg));//��ִ
        }
        #endregion

        #region ��Ⱥ����ɾ��
        /// <summary>
        /// ��Ⱥ����ɾ��
        /// </summary>
        /// <param name="jid">��Ӧ��Jid</param>
        private void RemoveFromGroupList(string jid)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                DeleteFromMessageList(jid);//ɾ����Ϣ�б���
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

        #region ����
        private void UpdateMe(DataOfUserDetial detial)
        {
            if (Me == null)
            {
                Me = new DataOfUserDetial();
            }
            LogHelper.log.Debug("======================�������������ر���");
            Me.nickname = detial.nickname;
            Me.userId = detial.userId;
            Me.active = detial.active;
            Me.areaCode = detial.areaCode;
            Me.birthday = detial.birthday;
            Me.areaId = detial.areaId;
            Me.attCount = detial.attCount;
            Me.name = detial.name;
            #region ���ص�ǰ�û�ͷ��
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

        #region ������������ݸı�
        /// <summary>
        /// ������������ݸı�
        /// </summary>
        private void InputTextChanged()
        {

            #region @Ⱥ��Ա
            if (Sess.isGroup)//�������Ⱥ����������
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
                            var panel = App.Current.MainWindow.FindResource("DialogContactCard");//��ȡ�Ի���
                            DataofMember member = new DataofMember();
                            string roomid = new Room().GetRoomIdByJid(Sess.Jid);
                            var lists = member.GetListByRoomId(roomid);
                            DialogContacts.Clear();//����ϴδ��ڵ���ϵ��
                            for (int i = 0; i < lists.Count; i++)
                            {
                                DialogContacts.Add(lists[i].ToMsgItem());//���Ⱥ��Ա
                            }
                            DialogHost.OpenDialogCommand.Execute(panel, null);//��ʾ�Ի���
                            DialogFunctionType = DialogFunction.AttentionGroupMember;//����Ϊ@Ⱥ��Ա
                        });
                    }
                });
            }
            #endregion
        }
        #endregion

        #region ���������Ϣ������
        /// <summary>
        /// ���������Ϣ������
        /// </summary>
        /// <param name="item">��Ҫ���µ���</param>
        private void UpdateRecentItemContent(MessageListItem item)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (RecentMessageList.Count(r => r.Jid == item.Jid) > 0)//ȷ�������
                {
                    var tmpitem = RecentMessageList.FirstOrDefault(r => r.Jid == item.Jid);
                    tmpitem.Msg = new Messageobject { type = kWCMessageType.Text, content = item.MessageItemContent };//��������
                }
            });
        }
        #endregion

        #region ������Ϣ
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        private void WithDrawMsg(Messageobject msg)
        {
            var client = ShiKuManager.WithDrawMsg(msg.jid, msg.messageId, msg.isGroup);//������Ϣ
            client.UploadDataCompleted += (send, eve) =>
            {
                if (eve.Error == null)
                {
                    msg.UpdateMessageType(msg.messageId, kWCMessageType.Withdraw);//�������ݿ���Ϣ״̬
                    string tcontent = "��������һ����Ϣ";
                    msg.UpdateMessageContent(msg.messageId, tcontent);//����Ϊ
                    msg.type = kWCMessageType.Withdraw;//��Ϊ����
                    Messenger.Default.Send(msg, ChatBubblesNotifications.WithDrawSingleMessage);//֪ͨ�����б����
                    if (RecentMessageList.Any(r => r.Jid == msg.jid))
                    {
                        var item = RecentMessageList.FirstOrDefault(r => r.Jid == msg.jid);
                        item.Msg = msg;//����������������Ϣ
                        item.Message = msg.ToJson();//����������������Ϣ
                    }
                }
                else
                {
                    Snackbar.Enqueue("����ʧ�ܣ�" + eve.Error.Message);
                }
            };
        }
        #endregion

        #region ���������ϵ���е�һ��
        /// <summary>
        /// ���������ϵ���е�һ��(�ö�)
        /// </summary>
        /// <param name="item"></param>
        public void InsertOrTopSingleMessageList(MessageListItem item)
        {
            var exists = RecentMessageList.FirstOrDefault(r => r.Jid == item.Jid);

            if (exists != null)
            {
                if (RecentMessageList.IndexOf(exists) == 0)//����Ѿ��ö��򲻲���
                {
                    return;
                }
                RecentMessageList.Remove(exists);//�Ƴ�UI
                Task.Run(() =>
                {
                    item.Update();//���������ϵ�������ݿ�
                });
            }
            else
            {
                Task.Run(() =>
                {
                    item.Insert();//���������ϵ�������ݿ�
                });
            }
            RecentMessageList.Insert(0, item);//�����һ��
        }
        #endregion

        #region Load RecentMsgItemList
        /// <summary>
        /// �����������Ϣ
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

        #region ��ʼ������
        /// <summary>
        /// ��ʼ������
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

        #region ���Emoji���鵽�ı���
        /// <summary>
        ///  ���Emoji������
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
                //������������з��Ϳ�ĩβ
                main.rtb_sendMessage.ThisRichText.CaretPosition = TextFieldDocument.Blocks.LastBlock.ContentEnd;
            });
            //TextFieldDocument.Blocks.Add(new Block())
        }
        #endregion

        #region ѡ����Ϣ��ı�ʱ
        /// <summary>
        /// ѡ����Ϣ��ı�ʱ
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
            TotalUnReadCount -= RecentMessageList[MessageListSelectedIndex].UnReadCount;//��ȥ��Ӧδ����Ϣ����
            RecentMessageList[MessageListSelectedIndex].UnReadCount = 0;//δ������Ϊ0
                                                                        //�����
            TextFieldDocument.Blocks.Clear();
            var emptypara = new Paragraph(new Run(""));
            TextFieldDocument.Blocks.Add(emptypara);
            //rtf_sendMessage.Focus();//�۽������ı�����
            if (MessageListSelectedIndex < 0)
            {
                return;
            }
            MsgItemMouseDown(item);
        }
        #endregion

        #region ����ý��
        /// <summary>
        /// ����ý��(������Ƶ)
        /// </summary>
        /// <param name="fileName">ý��·��</param>
        public void PlayMedia(string fileName)
        {
            Applicate.GetWindow<MainWindow>().VlcPlayer.SourceProvider.MediaPlayer.Play();
        }
        #endregion

        #region ��ͣý��
        /// <summary>
        /// ����ý��(������Ƶ)
        /// </summary>
        public void PauseMedia()
        {
            Applicate.GetWindow<MainWindow>().VlcPlayer.SourceProvider.MediaPlayer.Pause();
        }
        #endregion

        #region ������δ������
        public void SetTotalUnReadCount(int count = -1)
        {
            if (count == -1)
            {
                count = 0;
                foreach (var item in RecentMessageList)
                {
                    if (item.UnReadCount > 0)
                    {
                        ConsoleLog.Output("δ������" + item.Jid + " " + item.UnReadCount);
                    }
                    count += item.UnReadCount;
                }
            }
            this.TotalUnReadCount = count;
        }
        #endregion

        #region ��鷢��Ⱥ����Ϣ�Ƿ񱻽���
        /// <summary>
        /// ����Ⱥ����Ϣǰ����Ƿ񱻽���
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public bool CheckIsBanned(string jid, bool isSendCard = false)
        {
            if (jid.Length < 20)//��ͨ�û�������
            {
                return false;
            }
            bool banned = false;//Ĭ��Ϊδ�������û�
            var room = new Room() { jid = jid }.GetByJid();
            if (room != null)
            {
                var memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                if (memlist.Count == 0)
                {
                    APIHelper.GetRoomDetialByRoomId(room.id);
                    memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                }
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId);/*��ѯ���Լ�����ݱ��*/
                if (user != null)
                {
                    int access = (int)user.role;//��ȡȨ��
                    var talkTime = Helpers.StampToDatetime(user.talkTime);//��ȡ���Խ���ʱ��
                    if (DateTime.Now < talkTime && access == 3)//�������Ҳ�Ϊ�����
                    {
                        Snackbar.Enqueue("���ѱ�������" + talkTime.ToString("MM-dd HH:mm:ss"));
                        banned = true;
                    }
                    else if (Helpers.DatetimeToStamp(DateTime.Now) < room.talkTime && access == 3)//ȫԱ�����Ҳ�Ϊ�����
                    {
                        Snackbar.Enqueue("Ⱥ���ѿ���ȫ�����");
                        banned = true;
                    }
                    else if (isSendCard && room.allowSendCard == 0 && access == 3)//����������Ƭ�Ҳ�Ϊ�����
                    {
                        Snackbar.Enqueue("Ⱥ���ѹر���ͨ��Ա˽�Ĺ���");
                        banned = true;
                    }
                }
                //else//��ѯ�����û���Ϊ����Ⱥ
                //{
                //    Snackbar.Enqueue("���Ѳ��ڴ�Ⱥ��");
                //    banned = true;
                //}
            }
            return banned;
        }
        #endregion

        #region ����Ƿ�ر�Ⱥ��Ƭ
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
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*��ѯ���Լ�����ݱ��*/);
                if (user != null)
                {
                    int access = (int)user.role;
                    if (room.allowSendCard == 0 && access == 3)
                    {
                        Snackbar.Enqueue("Ⱥ���ѹر���ͨ��Ա˽�Ĺ���");
                        banned = true;
                    }
                }
                else
                {
                    Snackbar.Enqueue("���Ѳ��ڴ�Ⱥ��");
                    banned = true;

                }
            }

            return banned;
        }
        #endregion

        #region ����Ƿ�ر�Ⱥ����
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
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*��ѯ���Լ�����ݱ��*/);
                if (user != null)
                {
                    int access = (int)user.role;
                    if (room.allowConference == 0 && access == 3)
                    {
                        Snackbar.Enqueue("Ⱥ���ѹر���ͨ��Ա������鹦��");
                        banned = true;
                    }
                }
                else
                {
                    Snackbar.Enqueue("���Ѳ��ڴ�Ⱥ��");
                    banned = true;

                }
            }

            return banned;
        }
        #endregion

        #region Ⱥ���б�ѡ����ı�ʱ(�޸İ󶨵�Ⱥ������)
        /// <summary>
        /// Ⱥ���б�ѡ����ı�ʱ(�޸İ󶨵�Ⱥ������)
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
                //��ȡѡ�е�Ⱥ��
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
                    HiddenMembers = "Ⱥ���ر�Ⱥ��Ա��ʾ";
                    //Ⱥ���ر���ʾȺ��Ա
                    SelectedGroupMember.Clear();
                }
                GroupBtnContent = "����Ϣ";
                RoomInfoVisible = true;
            }
        }
        #endregion

        #region ListBoxItem����¼�
        /// <summary>
        /// ListBoxItem����¼�
        /// </summary>
        private void OnListViewItemClick()
        {
            var item = new MessageListItem();
            bool isJoin = false;
            if (MainTabSelectedIndex == 1)
            {
                //�����±���ҵ���Ӧ�ĺ��ѻ�Ⱥ�����
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
                MainTabSelectedIndex = 0;//ת����Ϣ����
                StartNewChatFromItem(item.Clone());//��ת����
            }
        }
        #endregion

        #region Initial Lists
        #region �ӿڼ��ؼ���Ⱥ��
        /// <summary>
        /// �ӿڼ��ؼ���Ⱥ��
        /// </summary>
        public void LoadJoinedRoomByApi()
        {
            var myclient = APIHelper.GetMyRoomsAsync();//�����Ҽ������Ⱥ��
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
                            AddToGroupList(rooms.data);//��ӽӿڷ�������
                        }
                        var tmproom = new Room();
                        tmproom.DeleteAllList();//ɾ��
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
                    Snackbar.Enqueue("��ȡȺ��ʧ��:" + e.Error.Message, "����", () => { LoadJoinedRoomByApi(); });
                }
            };//���Ѽ���Ⱥ��
        }
        #endregion

        #region �Ӽ��ϼ���Ⱥ���б�
        /// <summary>
        /// �Ӽ��ϼ���Ⱥ���б�
        /// </summary>
        /// <param name="data">Ⱥ�鼯��</param>
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

        #region �������ݿ���ؼ���Ⱥ��
        /// <summary>
        /// �����ݿ��м����ѽ���Ⱥ���б�
        /// </summary>
        public void LoadJoinedRoomsByDb()
        {
            Task.Run(() =>
            {
                MyGroupList = new ObservableCollection<MessageListItem>();
                var rooms = new Room().GetJoinedList();
                //���ݷ������ѭ����ӵ�����
                if (rooms.Count > 0)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        lock (MyGroupList)
                        {
                            for (int i = 0; i < rooms.Count; i++)
                            {
                                AddToMyGroupList(rooms[i].ToMsgItem());//��ӵ�������
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
        #region ���ؼ��غ����б�
        /// <summary>
        /// ���ؼ��غ����б�
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
                ConsoleLog.Output("��ӵ�����ʱ��" + ex.Message);
            }
        }
        #endregion
        #region ���ؼ��غ������б�
        /// <summary>
        /// ���ؼ��غ������б�
        /// </summary>
        internal void LoadBlockListByDb()
        {
            var blacks = new DataOfFriends().GetBlacksList();
            App.Current.Dispatcher.Invoke(() =>
            {
                lock (BlackList)
                {
                    BlackList.Clear();//����ռ���
                    foreach (var blackObj in blacks)
                    {
                        AddToBlackList(blackObj.ToMsgListItem());
                    }
                }
            });
        }
        #endregion

        #region �Ӻ������б���ɾ��
        /// <summary>
        /// �Ӻ������б���ɾ��
        /// </summary>
        /// <param name="item">��Ϣ��</param>
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

        #region �ӿڼ��غ����б�
        /// <summary>
        /// �ӿڼ��غ����б�
        /// </summary>
        public void LoadFriendsByApi()
        {
            var client = APIHelper.GetFriendsAsync();//��ȡ�����б�
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
                                    friObj.AutoInsert();//�������ݿ�
                                }
                            });
                            #region ����ϵͳ�˺�
                            for (int i = 0; i < friends.data.Count; i++)
                            {
                                if (friends.data[i].toUserId.Length <= 6)
                                {
                                    friends.data.Remove(friends.data[i]);//���Ϊϵͳ�����˺��򲻱���
                                }
                            }
                            //���Ѽ���
                            List<MessageListItem> friendlist = new List<MessageListItem>();
                            #endregion
                            for (int i = 0; i < friends.data.Count; i++)
                            {
                                MessageListItem item = friends.data[i].ToMsgListItem();
                                friendlist.Add(item);
                            }
                            AddToFriendList(friendlist);//����������б���(���̲߳���)
                            //��Ҫ����ͷ�񵽱��ص��û���Ϣ
                            List<DownLoadFile> downlist = new List<DownLoadFile>();
                            foreach (var item in friendlist)
                            {
                                string path = Applicate.LocalConfigData.GetDownloadAvatorPath(item.Jid);//���Ի�ȡ���صĻ���ͷ��
                                if (!File.Exists(path))//��ǰ�û�������ͷ��,��ӵ����ؼ���������
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
                                    item.Avator = path;//ͷ����ھ�ֱ����ʾͷ��
                                }
                            }
                            //����API����ͷ��
                            HttpDownloader.Download(downlist, (file) =>
                             {
                                 if (file.State == DownloadState.Error)//ͷ������ʧ�ܵ�ʱ����Ϊ��ʼͷ��
                                 {
                                     return;//��ʼ��ͷ��ʱ����������ʾͷ��(���������ͷ�������Ĭ��ͷ��),�ʴ˴�������ش��󲻸���UI
                                     //file.LocalUrl = Applicate.LocalConfigData.GetDisplayAvatorPath(file.Jid);
                                 }
                                 //������ݻش���ID�ж�����һ���û���ͷ�񻺴���ˡ���UI����ȥ��ȡ
                                 var uifriend = friendList.FirstOrDefault(f => f.Jid == file.Jid);
                                 if (uifriend != null)
                                 {
                                     App.Current.Dispatcher.Invoke(() =>
                                     {
                                         uifriend.Avator = file.LocalUrl;//ˢ���û�ͷ��
                                     });
                                 }
                             });
                        }
                    });
                }
                else
                {
                    Snackbar.Enqueue("�����б��ȡʧ��:" + e.Error.Message, "����", () => { LoadFriendsByApi(); });
                }
            };
        }
        #endregion

        #region �ӿڼ��غ������б�
        /// <summary>
        /// �ӿڼ��غ������б�
        /// </summary>
        internal void LoadBlockListByApi()
        {
            //��ȡ�������б�
            var bclient = APIHelper.GetBlackListAsync();//��ȡ�������б�
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
                            black.DeleteByStatus(-1);//ɾ������������
                            string tmp = Thread.CurrentThread.Name;
                            foreach (var blkObj in blacks.data)
                            {
                                blkObj.AutoInsert();//����
                            }
                        }
                    });
                }
                else
                {
                    Snackbar.Enqueue("��ȡ������ʧ��:" + eve.Error.Message, "����", () => { LoadBlockListByApi(); });
                }
            };
        }
        #endregion

        #region ��ӵ��������б�
        /// <summary>
        /// ��ӵ��������б�
        /// </summary>
        /// <param name="item">Ҫ��ӵ���</param>
        private void AddToBlackList(MessageListItem item)
        {
            if (item != null)//������
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    BlackList.Add(item);
                });
            }
        }
        #endregion

        #endregion

        #region ��ӵ�����UI������
        /// <summary>
        /// ��ӵ�����UI������
        /// </summary>
        /// <param name="item">��Ҫ��ӵ���</param>
        public void AddToFriendList(MessageListItem item)
        {
            try
            {
                if (FriendList.Count(i => i.Jid == item.Jid) == 0)//��������ڲ����
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
                            AddToFriendList(item);//��������ڲ����
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

        #region ����Tab�ı�ʱ�޸İ󶨵�ֵ
        private void FriendSelectionChanged()
        {
            Task.Run(() =>
            {
                GC.Collect();
            });
            SelectedFriendProfileVisiblity = false;
            SelectedFriend = new DataOfUserDetial();
            //���°󶨵�ֵ
            if (FriendTypeIndex == 0)//����
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
                    GetSelectUser(friend.Jid);//��ѯ����ӦUserId��Ϣ
                }
            }
            else if (FriendTypeIndex == 1)
            {
                if (BlackSelectedIndex > -1)
                {
                    //��ѯ����ֵ
                    var black = BlackList[BlackSelectedIndex];
                    if (black != null)
                    {
                        GetSelectUser(black.Jid);//��ѯ����ӦUserId��Ϣ
                    }
                }
                else
                {
                    //����
                }
            }
        }
        #endregion

        #region ��ʾѡ�е��û�
        /// <summary>
        /// ��ȡѡ���û�����ʾ
        /// </summary>
        /// <param name="jid">�û�Jid</param>
        private void GetSelectUser(string jid)
        {
            var client = APIHelper.GetUserDetialAsync(jid);
            LogHelper.log.Info("ѡ���û�GetUserDetial" + jid);
            var localuser = new DataOfFriends().GetByUserId(jid);//����ʾ����
            SelectedFriend = localuser.ToDataOfUserDetial();
            SelectedFriend.sex = -1;
            SelectedFriend.birthday = 0;//��ʼ��
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    string restext = Encoding.UTF8.GetString(res.Result);
                    var detial = JsonConvert.DeserializeObject<JsonuserDetial>(restext);
                    detial.data.remarkName = detial.data.friends.remarkName;
                    SelectedFriend = detial.data;
                    //����ǳ��Ѹ���, ��ѡ���û��ǳƲ�Ϊ��ʱ����
                    if (localuser.nickname != detial.data.nickname && !string.IsNullOrWhiteSpace(SelectedFriend.remarkName))
                    {
                        var fItem = new MessageListItem
                        {
                            Jid = detial.data.userId,
                            ShowTitle = detial.data.friends.remarkName,
                            Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(detial.data.userId)
                        };
                        //֪ͨҳ��ˢ���ǳ�
                        Messenger.Default.Send(fItem, MainViewNotifactions.UpdateAccountName);
                    }
                }
                else
                {
                    Snackbar.Enqueue("�������,��ȡ�û�����ʧ��:" + res.Error.Message, true);
                    var tmp = new DataOfFriends().GetByUserId(jid);//��ȡ���غ�������
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
                SelectedFriendProfileVisiblity = true;//��ʾѡ���û�����
            };
        }
        #endregion

        #region ��MsgItem��������
        /// <summary>
        /// ��MsgItem��������
        /// </summary>
        internal void StartNewChatFromItem(MessageListItem targetItem)
        {
            try
            {
                MessageListSelectedIndex = -1;
                //����Ự������
                if (!RecentMessageList.Any(m => m.Jid == targetItem.Jid))
                {
                    SetChatTitle(targetItem.Jid, targetItem.ShowTitle);//���ûỰ����
                    targetItem.MessageItemType = ItemType.Message;//������Ϣ����Ϊ��Ϣ
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        InsertOrTopSingleMessageList(targetItem.Clone());//���뵽��һ������ؼ���
                        MessageListSelectedIndex = 0;//��ת����һ��Ự
                        targetItem.Insert();//���������ݿ�
                    });
                }
                else//������ڻỰ
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MessageListItem chatItem = RecentMessageList.First(m => m.Jid == targetItem.Jid);//��ȡ��Ӧ��Message
                        //Sess.Jid = chatItem.Jid;
                        //Sess.NickName = chatItem.MessageTitle;
                        //Sess.RemarkName = chatItem.ShowTitle;
                        MessageListSelectedIndex = RecentMessageList.IndexOf(chatItem);//
                        chatItem.Update();
                    });
                }
                //��ת����Ӧ���������
                Messenger.Default.Send(targetItem.Jid, ChatBubblesNotifications.ShowBubbleList);//��ʾ��Ϣ��¼
                MainTabSelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region �½�Ⱥ��ʱ
        /// <summary>
        /// �½�Ⱥ��ʱ(����һ���µĻỰ)
        /// </summary>
        /// <param name="room">�����õ�Ⱥ��</param>
        internal void OnCreateNewGroup(Room room)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainTabSelectedIndex = 0;//��ת����Ϣҳ��
                AddToMyGroupList(room.ToMsgItem());
                var item = MyGroupList.FirstOrDefault(r => r.Jid == room.jid);
                if (item != null)
                {
                    StartNewChatFromItem(item.Clone());
                }
                Snackbar.Enqueue("����Ⱥ ��" + room.name + "���ɹ���");
            });
        }
        #endregion

        #region ��ӵ���Message��
        /// <summary>
        /// ��ӵ���Message��
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

        #region �˳�Ⱥ��
        /// <summary>
        /// �˳�Ⱥ��    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExitGroup(MessageListItem room)
        {
            var tmp = new DataofMember().GetListByRoomId(room.Id);//ʹ��
                                                                  //��ѯ���Լ�����ݱ��
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
            client.Tag2 = room.Jid;//ָ��Jid
            client.UploadDataCompleted += GroupDeleteComplete;//ָ���ص�
        }
        private void GroupDeleteComplete(object sender, UploadDataCompletedEventArgs e)
        {
            var resstr = Encoding.UTF8.GetString(e.Result);//��ȡ
            var result = JsonConvert.DeserializeObject<JsonBase>(resstr);
            var jid = ((HttpClient)sender).Tag2 as string;//��ȡJid
            var rroom = ((HttpClient)sender).Tag as MessageListItem;//��Tag��ȡ������Ϣ
            if (result.resultCode == 1)
            {
                DeleteFromMessageList(jid);//ɾ����Ϣ�б���
                Snackbar.Enqueue("�˳�Ⱥ " + rroom.MessageTitle + " �ɹ�", true);
                RemoveMyGroupByJid(rroom.Jid);//��Ⱥ���б����Ƴ�
                RoomInfoVisible = false;//����Ⱥ����
                ConsoleLog.Output("��Ⱥ�ɹ�");
            }
            else
            {
                RoomInfoVisible = true;
                Snackbar.Enqueue("��Ⱥʧ��:" + result.resultMsg);
                ConsoleLog.Output("��Ⱥʧ��");
            }
        }
        #endregion

        #region ɾ������
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        public void FriendDelete(MessageListItem item)
        {
            //��ʾȷ����Ϣ
            if (System.Windows.MessageBox.Show("ȷ��Ҫɾ��\"" + item.MessageTitle + "\"�� ��", "ɾ����", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                try
                {
                    var client = APIHelper.DeleteFriendAsync(item.Jid);
                    client.Tag = item;//��ֵ�Զ������
                    client.UploadDataCompleted += FriendDeleteComplete;
                }
                catch (Exception ex)
                {
                    LogHelper.log.Error(ex.Message, ex);
                    ConsoleLog.Output("ɾ���Ѵ���:" + ex.Message);
                }
            }
        }

        private void FriendDeleteComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)//��������
            {
                var tmp = Encoding.UTF8.GetString(e.Result);
                var result = JsonConvert.DeserializeObject<JsonBase>(tmp);
                var item = ((HttpClient)sender).Tag as MessageListItem;
                if (result.resultCode == 1)//�������
                {
                    //����ɾ����������
                    ShiKuManager.SendDeleteFriend(item);
                    new DataOfFriends().UpdateFriendState(item.Jid, 0);//�������ݿ�
                    //ɾ����Ϣ�б����
                    OnDeleteFriend(item.Jid);
                }
                else
                {
                    Snackbar.Enqueue("ɾ������ʧ��:" + result.resultMsg);
                }
            }
            else
            {
                Snackbar.Enqueue("�������, ɾ������ʧ��:" + e.Error.Message);
            }
        }
        #endregion

        #region ����Ϣ�б�ɾ��
        /// <summary>
        /// ����Ϣ�б�ɾ��
        /// </summary>
        /// <param name="userId">��Ҫɾ����UserId</param>
        public void DeleteFromMessageList(string userId)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var item = RecentMessageList.FirstOrDefault(m => m.Jid == userId);
                int index = RecentMessageList.IndexOf(item);
                if (index >= 0)
                {
                    item.Delete();//ɾ�����ݿ�
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

        #region �Ƴ���Ϣ�б�����
        /// <summary>
        /// �Ƴ���Ϣ�б�����
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
                    if (RecentMessageList.Count > +1)// If the index of MessageList Count is larger than Selected Item Index �����Ϣ������ѡ��ֵ��
                    {
                        RecentMessageList.RemoveAt(index);// Remove it �Ƴ�
                        if (index == 0)
                        {
                            MessageListSelectedIndex = index;
                        }
                        else
                        {
                            MessageListSelectedIndex = index - 1;// Select Last item ѡ����һ��Ԫ��
                        }
                    }
                    else if (RecentMessageList.Count - 1 == 0)
                    {
                        MessageListSelectedIndex = -1;// Don't select any item ��ѡ���κ� Ԫ��
                        Sess.Jid = null;
                        Sess.NickName = "";
                        Sess.RemarkName = "";
                    }
                    else if (RecentMessageList.Count == index + 1)
                    {
                        MessageListSelectedIndex = index;// Select Last item ѡ����һ��Ԫ��
                        RecentMessageList.RemoveAt(index);//Remove �Ƴ�
                    }
                    else
                    {
                        Sess.Jid = null;
                    }
                    msgItem.Delete();//�������ݿ�
                    SetTotalUnReadCount();//�ۼ�δ����Ϣ����
                    //UpdateMessageList();
                }
                catch (Exception ex)
                {
                    ConsoleLog.Output(ex.Message);
                }
            });
        }
        #endregion

        #region ���������
        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="sender">�ؼ�Դ</param>
        /// <param name="e"></param>
        public void BlackListAdd(MessageListItem item)
        {
            if (System.Windows.MessageBox.Show("�Ƿ� " + item.ShowTitle + " �����������", "���������", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                try
                {
                    var client = APIHelper.BlockFriendAsync(item.Jid);
                    client.Tag = item;//������ʱ����
                    client.UploadDataCompleted += (sender, e) =>
                    {
                        var res = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(e.Result));
                        var tmpClient = (HttpClient)sender;//��ȡ�¼�Դ
                        if (tmpClient.Tag != null)
                        {
                            var tmpfriendItem = (MessageListItem)client.Tag;
                            if (res.resultCode == 1)
                            {
                                ShiKuManager.DefriendReq(tmpfriendItem);//�������ں�������
                                new DataOfFriends().UpdateFriendState(tmpfriendItem.Jid, -1);//�������ݿ�
                                OnBlackFriend(tmpfriendItem.Jid);//����UI
                                //var toBlack = FriendList.FirstOrDefault(f => f.Jid == tmpFriend.toUserId);
                                //FriendList.Remove(toBlack);//ɾ�������б�����
                                //AddToBlackList(toBlack);//��Ӻ�������
                                Snackbar.Enqueue("��\"" + tmpfriendItem.ShowTitle + "\"���ڳɹ�");
                            }
                            else
                            {
                                Snackbar.Enqueue("���������ʧ��:" + res.resultMsg);
                            }
                        }
                        else
                        {
                            Snackbar.Enqueue("���������ʧ�ܣ�������");
                        }
                    };
                }
                catch (Exception ex)
                {
                    LogHelper.log.Error(ex.Message, ex);
                    ConsoleLog.Output("���ڴ���:" + ex.Message);
                }
            }
        }
        #endregion

        #region �Ƴ�������
        /// <summary>
        /// �Ƴ�������
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
                        OnCancelBlack(blackitem.Jid);//�޸�UI
                        Snackbar.Enqueue("��\"" + blackitem.ShowTitle + "\"�Ƴ��������ɹ�");
                        var item = FriendList.FirstOrDefault(f => f.Jid == useritem.Jid);
                        if (item != null)
                        {
                            RecentMessageList.Add(item.Clone());//����һ���µ�������
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("�� \"" + blackitem.ShowTitle + "\" �Ƴ�������ʧ�ܣ�������");
                    }
                };
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                Snackbar.Enqueue("�Ƴ�������ʧ�ܣ����Ժ�����");
                ConsoleLog.Output("�Ƴ�����������:" + ex.Message);
            }
        }
        #endregion

        #region ���ú����ǳ�
        /// <summary>
        /// ���ú�������
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="userName">�����ǳ�(Ⱥ��ʱ��Ϊ��)</param>
        internal void SetChatTitle(string userId, string userName)
        {
            var tmpsess = Sess;
            tmpsess.Jid = userId;
            Sess = tmpsess;
            if (Sess.Jid.Length < 20)//���useridС��15��
            {
                if (Sess.Jid == "10001")
                {
                    Sess.NickName = "�µ�����";
                    Sess.RemarkName = "�µ�����";
                    Sess.IsOnlineText = "";
                }
                else if (Sess.Jid == "10000")
                {
                    Sess.NickName = "�ͷ����ں�";
                    Sess.RemarkName = "�ͷ����ں�";
                    Sess.IsOnlineText = "";
                }
                else
                {
                    var friends = new DataOfFriends().GetByUserId(userId);
                    var client = ShiKuManager.GetFriendState(userId);//�ص���ʽ��ֵ����״̬
                    client.UploadDataCompleted += (sender, e) =>
                    {
                        try
                        {
                            var tmpres = Encoding.UTF8.GetString(e.Result);
                            var tmp = JsonConvert.DeserializeObject<JsonUserState>(tmpres);
                            Sess.IsOnlineText = (tmp.data == 1) ? "����" : "����";
                        }
                        catch (Exception ex)
                        {
                            ConsoleLog.Output("��ȡ��������״̬����:--" + ex.Message);
                        }
                    };
                    //��ȡNickname
                    Sess.NickName = userName;
                    Sess.RemarkName = userName;
                }
            }
            else
            {
                var room = new Room().GetByJid(userId);
                if (room != null)
                {
                    Sess.RoomId = room.id;//����ȺID
                    var mem = new DataofMember { userId = Applicate.MyAccount.userId }.GetModelByJid(room.jid);
                    if (mem != null)
                    {
                        Sess.MyMemberNickname = mem.nickname;
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Sess.NickName = room.name;
                        Sess.RemarkName = room.name;
                        Sess.IsOnlineText = room.userSize + " λ��Ա";//Ⱥ��Ϊ3, ����ʾ����״̬
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Sess.NickName = "";
                        Sess.RemarkName = "";
                        Sess.IsOnlineText = "";//Ⱥ��Ϊ3, ����ʾ����״̬
                    });
                }
            }
        }
        #endregion

        #region ������Ϣ�б����ݿ�
        internal void UpdateMessageList()
        {
            if (MessageListSelectedIndex < 0)
            {
                return;
            }
            if (RecentMessageList.Count > 0)
            {
                RecentMessageList[MessageListSelectedIndex].Update();//��Ϣ�б���µ����ݿ�
            }
        }
        #endregion

        #region ˢ������ͼƬ����
        /// <summary>
        /// ˢ������ͼƬ����
        /// </summary>
        internal void RefreshAllImg(string jid)
        {

            if (!string.IsNullOrWhiteSpace(jid))
            {
                if (jid == Applicate.MyAccount.userId)
                {
                    Me.userId = "";//�Ժ��ٸ�
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

        #region �½�����
        internal void NewChat(string userId)
        {
            if (userId.Length < 20)
            {//��ͨ�û�
                if (MyGroupList.Count(r => r.Jid == userId) > 0)
                {//�������
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MessageListItem ritem = new MessageListItem();
                        ritem = MyGroupList.FirstOrDefault(r => r.Jid == userId);
                        MessageListItem chatItem = RecentMessageList.First(m => m.Jid == userId);//��ȡ��Ӧ��Message
                        MessageListSelectedIndex = RecentMessageList.IndexOf(chatItem);//
                    });
                }
            }
            else
            {
            }
        }
        #endregion

        #region ˢ��Ⱥ������Ԥ��ҳ
        /// <summary>
        /// ˢ��Ⱥ������Ԥ��ҳ
        /// </summary>
        /// <param name="roomId">Ⱥ��Id</param>
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
                    HiddenMembers = "Ⱥ���ر�Ⱥ��Ա��ʾ";
                    //Ⱥ���ر���ʾȺ��Ա
                    SelectedGroupMember.Clear();
                }
            };
        }
        #endregion

        #region ����Ⱥ���б�����
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

        #region ��ȡѡ�е�Ⱥ��Ա
        private void GetSelectedGroupMember(string roomId)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                IList<DataofMember> memberList = new List<DataofMember>();
                memberList = new DataofMember().GetListByRoomId(roomId);//�����ݿ��в�ѯ
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
                        member.userId = complete.Jid;//����ͷ��
                    }
                });
            });
        }
        #endregion

        #region ��������ѡ�иı�ʱ
        /// <summary>
        /// ��������ѡ�иı�ʱ
        /// </summary>
        public void FriendTypeChanged()
        {
            if (FriendTypeIndex == 0)
            {
                if (friendSelectedIndex >= 0)//��ѡ����Ļ�
                {
                    GetSelectUser(FriendList[FriendSelectedIndex].Jid);//��ȡ��������
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
                    GetSelectUser(BlackList[BlackSelectedIndex].Jid);//��ȡ����������
                }
                else
                {
                    SelectedFriend = null;
                }
            }
        }
        #endregion
        #region ����״̬
        /// <summary>
        /// ����Xmpp״̬
        /// </summary>
        /// <param name="state">Xmpp����״̬</param>
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
                            AccountStateText = "δ����";
                            break;
                        case XmppConnectionState.Connecting:
                            AccountStateFill = new SolidColorBrush(Colors.DimGray);
                            AccountStateText = "������...";
                            break;
                        case XmppConnectionState.Connected:
                            AccountStateFill = new SolidColorBrush(Colors.DeepSkyBlue);
                            AccountStateText = "Connected";
                            break;
                        case XmppConnectionState.Authenticating:
                            AccountStateFill = new SolidColorBrush(Colors.DeepPink);
                            AccountStateText = "��֤��";
                            break;
                        case XmppConnectionState.Authenticated:
                            AccountStateFill = new SolidColorBrush(Colors.Tomato);
                            AccountStateText = "Authenticated";
                            break;
                        case XmppConnectionState.Binding:
                            AccountStateFill = new SolidColorBrush(Colors.Thistle);
                            AccountStateText = "����...";
                            break;
                        case XmppConnectionState.Binded:
                            AccountStateFill = new SolidColorBrush(Colors.SlateGray);
                            AccountStateText = "�����";
                            break;
                        case XmppConnectionState.StartSession:
                            AccountStateFill = new SolidColorBrush(Colors.SeaGreen);
                            AccountStateText = "������";
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
                            AccountStateText = "����";
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
                    //ConsoleLog.Output("-=-=-=-=״̬��ɫΪ��" + AccountStateFill.Color.ToString());
                });
            }
        }
        #endregion

        #region ��Ϣ�б�ѡ�иı�
        /// <summary>
        /// ��Ϣ�б�ѡ�иı�
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
            TypingTimer.Stop();//ֹͣ���ڷ��ͼ�ʱ���Ա��⸱���ⱻ����
            //Temp Media Player Stop(I'll change the mediaplayer into mainwindow as a control)
            var main = Applicate.GetWindow<MainWindow>();
            if (main.VlcPlayer.SourceProvider.MediaPlayer != null)
            {
                main.VlcPlayer.SourceProvider.MediaPlayer.Stop();
                //main.SelectedContact.SelectedItems.Clear();//ɾ��//�˴��滻Ϊ������ݼ���
            }
            MessageListItem item = RecentMessageList[MessageListSelectedIndex];
            if (string.IsNullOrWhiteSpace(item.Jid) || (item.Jid == Sess.Jid))
            {
                return;
            }
            TotalUnReadCount -= item.UnReadCount;//��ȥ��Ӧδ����Ϣ����
            item.UnReadCount = 0;//δ������Ϊ0
            //�����ı� 
            TextFieldDocument.Blocks.Clear();
            var emptypara = new Paragraph(new Run(""));
            TextFieldDocument.Blocks.Add(emptypara);
            MsgItemMouseDown(item);
        }
        #endregion

        #region ������Ϣ��������
        /// <summary>
        /// ������Ϣ��������
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="e"></param>
        internal void MsgItemMouseDown(MessageListItem Item)
        {
            SetChatTitle(Item.Jid, Item.ShowTitle);//�����������
            Sess.RoomId = Item.Id;//����ȺId
            if (Item.Jid.Length >= 7)
            {
                Messenger.Default.Send(Item.Jid, ChatBubblesNotifications.ShowBubbleList);//֪ͨ�����б���ʾ��Ϣ               
            }
            if (Item.Jid.Length > 15)
            {
                Messenger.Default.Send(Item, ChatBubblesNotifications.SetMessageInfo);//֪ͨ��������
            }
            //var lastMsg = ServiceLocator.Current.GetInstance<ChatBubbleListViewModel>().ShowDefaultMessage(Item.Jid);//������ʾ��Ϣ
            //Item.Msg = lastMsg;//��ʾ���һ����Ϣ
            //���������Ϣ�б�
            Task.Run(() =>
            {
                UpdateMessageList();
            });
        }
        #endregion

        #region �϶��ļ�����
        #region �϶�ʱ����
        /// <summary>
        /// �϶��ļ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eve"></param>
        private void ChatDragEnter(object sender, System.Windows.DragEventArgs eve)
        {
            eve.Handled = true;
        }
        #endregion

        #endregion

        #region ��Ϣ����
        /// <summary>
        /// MainWindow��Ϣ����(ֻ��UI����)
        /// MainWindow ChatMessage Processing (UI process only)
        /// </summary>
        /// <param name="msg">�յ�����Ϣ</param>
        public void ProcessNewMessage(Messageobject msg)
        {
            CreateOrUpdateMsgItem(msg.Clone());
            switch (msg.type)
            {
                case kWCMessageType.PokeMessage://��һ��
                    Messenger.Default.Send(true, MainWindow.ShowWindow);//��ʾ
                    break;
                case kWCMessageType.Typing://��������
                    Sess.IsOnlineText = "�Է���������...";
                    TypingTimer.Start();
                    break;
                case kWCMessageType.RoomExit://��Ⱥ
                    var room = new Room { jid = msg.objectId }.GetByJid();
                    if (room != null)
                    {
                        ReloadRoomDetialBoard(room.id);
                    }
                    if (msg.toUserId == Applicate.MyAccount.userId)//������Լ���Ⱥ
                    {
                        RemoveMyGroupByJid(msg.objectId);//MyGroupList�����Ƴ�
                        DeleteFromMessageList(msg.objectId);//ɾ�������ϵ��
                    }
                    break;
                case kWCMessageType.RoomDismiss://��ɢȺ
                    RemoveMyGroupByJid(msg.objectId);//MyGroupList�����Ƴ�
                    //msg.content = "�����˳���Ⱥ";
                    DeleteFromMessageList(msg.objectId);
                    //���˽�ɢ
                    //SetChatTitle(msg.objectId, null);
                    //}
                    break;
                case kWCMessageType.RoomNotice://Ⱥ����
                    break;
                case kWCMessageType.RoomMemberBan://��Ա����
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
                            //IsJoined = true,//���������Ϊ�Ѽ���
                        };
                        //SetChatTitle(tmpRoom.jid, tmpRoom.name);//���ñ���
                        AddToMyGroupList(tmpRoom.ToMsgItem());//���Ⱥ����
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
                case kWCMessageType.FriendRequest://���к�
                    break;
                case kWCMessageType.RequestFriendDirectly://ֱ����Ӻ���
                    var fItem = new DataOfFriends().GetByUserId(msg.fromUserId);
                    AddToFriendList(fItem.ToMsgListItem());//���º����б�
                    StartNewChatFromItem(fItem.ToMsgListItem());//��ʼ����
                    break;
                case kWCMessageType.RequestAgree://�Է�ͨ����֤
                    var item = new DataOfFriends().GetByUserId(msg.fromUserId);
                    AddToFriendList(item.ToMsgListItem());//��ӵ������б�
                    break;
                case kWCMessageType.DeleteFriend://������ɾ��
                    OnDeleteFriend(msg.fromUserId);
                    break;
                case kWCMessageType.RoomNameChange://Ⱥ�����޸�
                    OnUserNameChanged(new MessageListItem { Jid = msg.jid, ShowTitle = msg.content });
                    break;
                case kWCMessageType.BlackFriend://������
                    OnBlackFriend(msg.fromUserId);//�����Լ�������
                    break;
                default:
                    break;
            }
        }

        #region �ҵ�Ⱥ���б����Jid�Ƴ�
        /// <summary>
        /// �ҵ�Ⱥ���б����Jid�Ƴ�
        /// </summary>
        /// <param name="jid">��Ӧ��Jid</param>
        private void RemoveMyGroupByJid(string jid)
        {
            var tmproom = MyGroupList.FirstOrDefault(m => m.Jid == jid);
            if (tmproom != null)
            {
                App.Current.Dispatcher.Invoke(() => { MyGroupList.Remove(tmproom); });
            }
        }
        #endregion

        #region �޸Ķ�ӦJid�ǳ�
        /// <summary>
        /// �޸Ķ�ӦJid�ǳ�
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
                            var item = MyGroupList.FirstOrDefault(m => m.Jid == nitem.Jid);//��ȡ��
                            item.ShowTitle = nitem.ShowTitle;//����Ⱥ���б���
                            if (DisplayGroup.jid == nitem.Jid)
                            {
                                DisplayGroup.name = nitem.ShowTitle;//����Ԥ������Ⱥ����
                            }
                        }
                    }
                }
                else
                {
                    if (BlackList.Count(i => i.Jid == nitem.Jid) > 0)
                    {
                        var item = BlackList.FirstOrDefault(m => m.Jid == nitem.Jid);
                        item.ShowTitle = nitem.ShowTitle;//����Ⱥ���б���//���º�����
                    }
                    if (FriendList.Count(f => f.Jid == nitem.Jid) > 0)
                    {
                        var item = FriendList.FirstOrDefault(f => f.Jid == nitem.Jid);//��ȡ��Ӧ������
                        item.ShowTitle = nitem.ShowTitle;//����Ⱥ���б���/���º����б�
                    }
                }
                if (RecentMessageList.Count(m => m.Jid == nitem.Jid) > 0)//���������Ϣ���б�
                {
                    var item = RecentMessageList.FirstOrDefault(m => m.Jid == nitem.Jid);
                    item.ShowTitle = nitem.ShowTitle;//����Ⱥ���б���
                }
                if (Sess.Jid == nitem.Jid)//���»Ự����
                {
                    Sess.NickName = nitem.MessageTitle;
                    Sess.RemarkName = nitem.ShowTitle;//����
                }
                if (SelectedFriend.userId == nitem.Jid)//����ѡ�к���
                {
                    SelectedFriend.remarkName = nitem.ShowTitle;//����ѡ�к�������
                }
            });
        }
        #endregion

        #region ��ӵ��ҵ�Ⱥ��
        /// <summary>
        /// ��ӵ��ҵ�Ⱥ��UI
        /// </summary>
        /// <param name="item">�ж��Ƿ���ڵ���</param>
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

        #region ��Ϣ��ִ
        /// <summary>
        /// ��Ϣ��ִ
        /// </summary>
        /// <param name="msg">��Ӧ����Ϣ</param>
        public void MsgReceipt(Messageobject msg)
        {
            switch (msg.type)
            {
                case kWCMessageType.kWCMessageTypeNone:
                    break;
                #region ��Ϣ��ִ����(�ʹ��ִ)
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
                    OnCancelBlack(msg.ToId);//ȡ��������
                    break;
                case kWCMessageType.BlackFriend:
                    OnBlackFriend(msg.ToId);//���öԷ�������
                    break;
                case kWCMessageType.DeleteFriend:
                    Snackbar.Enqueue("��ɾ������ " + msg.toUserName);//��ʾSnackbar.Enqueue��ʾ
                    //OnDeleFriend(new Messageobject() { FromId = msg.ToId });
                    break;
                case kWCMessageType.FriendRequest://��������
                    break;
                case kWCMessageType.RequestFriendDirectly://ֱ����Ӻ���
                    var item = new DataOfFriends().GetByUserId(msg.ToId);
                    AddToFriendList(item.ToMsgListItem());
                    if (RecentMessageList.Count(r => r.Jid == msg.ToId) == 0)
                    {
                        var tmp = FriendList.FirstOrDefault(m => m.Jid == msg.ToId);
                        StartNewChatFromItem(tmp.Clone());//����
                    }
                    break;
                case kWCMessageType.RequestAgree://ͬ��Է���������
                    var friend = new DataOfFriends().GetByUserId(msg.toUserId);
                    AddToFriendList(friend.ToMsgListItem());
                    break;
                case kWCMessageType.Typing://��������
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
                case kWCMessageType.RoomDismiss://��ɢȺ
                case kWCMessageType.RoomExit://��Ⱥ
                    var tmproom = MyGroupList.FirstOrDefault(r => r.Jid == msg.objectId);
                    if (tmproom != null)
                    {
                        MyGroupList.Remove(tmproom);//�Ƴ�Ⱥ���б��ж�Ӧ��
                    }
                    var msgitem = RecentMessageList.FirstOrDefault(m => m.Jid == msg.objectId);
                    if (msgitem != null)
                    {
                        int rIndex = RecentMessageList.IndexOf(msgitem);//��ȡ����
                        RemoveMessageListItem(rIndex);//ɾ�������Ϣ��
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

        #region ɾ������
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="msg">ͨ��FromUserId���в���</param>
        public void OnDeleteFriend(string jid)
        {
            //ɾ������
            DeleteFromMessageList(jid);//ɾ����Ϣ�б��е�����
            DeletefromFriendList(jid);//ɾ�������б��е�����
            DeletefromBlockList(jid);
        }
        #endregion

        #region �Ӻ�����ɾ��
        private void DeletefromBlockList(string jid)
        {
            var item = BlackList.FirstOrDefault(b => b.Jid == jid);
            if (item != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    BlackList.Remove(item);//�Ƴ���Ӧ��
                });
            }
        }
        #endregion

        #region ɾ������UI�б�������
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
                        FriendList.RemoveAt(tmpindex);//�Ƴ���Ӧ�ĺ���
                    }
                });
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// ����(˫������)
        /// </summary>
        /// <param name="msg"></param>
        public void OnBlackFriend(string userId)
        {
            DeleteFromMessageList(userId);//ɾ��
            App.Current.Dispatcher.Invoke(() =>
            {
                var tmpfitem = FriendList.FirstOrDefault(f => f.Jid == userId);
                if (tmpfitem != null)
                {
                    AddToBlackList(tmpfitem);
                    FriendList.Remove(tmpfitem);//ɾ������ӵ�������
                }
            });
        }
        #endregion

        #region ȡ��������
        /// <summary>
        /// ȡ��������
        /// </summary>
        /// <param name="userId">ȡ�����û�</param>
        internal void OnCancelBlack(string userId)
        {
            var black = BlackList.FirstOrDefault(b => b.Jid == userId);
            if (black != null)
            {
                AddToFriendList(black);//���
                RemoveFromBlackList(black);//�Ƴ�
            }
        }
        #endregion

        #endregion

        #region ��Ƶ����
        public string playingAudioId = "";

        public bool MainAudio(string msgId, string path = null)
        {
            bool isPlaying = false;
            try
            {
                if (playingAudioId != msgId)
                {
                    //ִ�н�������
                    //Player.MediaPlayer.Stop();
                    //Player.SourceProvider.MediaPlayer.OnMediaPlayerEndReached();
                    Task.Factory.StartNew(() =>
                    {
                        //�ȴ�Main���ڵ�Player��EndReachedִ����
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
                                Player.SourceProvider.MediaPlayer.Audio.Volume = 98;//����
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

        #region  ***����������Ϣ
        /// <summary>
        /// �������߸���MessageListItem
        /// </summary>
        /// <param name="newMsg">��Ҫ�������Ϣ</param>
        internal void CreateOrUpdateMsgItem(Messageobject newMsg)
        {
            App.Current.Dispatcher.Invoke(() =>
         {
             MessageListItem item = new MessageListItem();
             if (RecentMessageList.Count(m => m.Jid == newMsg.jid) > 0)//�����ڶ�Ӧ����Ϣ��
             {
                 item = RecentMessageList.FirstOrDefault(m => m.Jid == newMsg.jid);//��ȡ��Ӧ��
             }
             else
             {
                 item.Jid = newMsg.jid;
                 item.Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(newMsg.jid);//������ʾ��ͷ��·��
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
                 //item.UnReadCount = (newMsg.isMySend == 1 || newMsg.FromId == Applicate.MyAccount.userId) ? (0) : (1);//���
             }
             InsertOrTopSingleMessageList(item);//�ö�Ŀǰ����Ϣ
             if (Sess.Jid == newMsg.jid)//����ǵ�ǰѡ�лỰ��ֱ����ʾ��Ϣ������
             {
                 Messenger.Default.Send(newMsg, ChatBubblesNotifications.ShowSingleBubble);//��ʾ��Ϣ
                 //MessageListSelectedIndex = 0;//ѡ�е�һ��
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
                     //case kWCMessageType.VideoChatAsk://��Ƶͨ��������ʾ�Ǳ�
                     case kWCMessageType.VideoChatCancel:
                     case kWCMessageType.VideoChatEnd:
                         item.UnReadCount++;//�ۼ�δ���Ǳ�
                         TotalUnReadCount++;
                         soundPlayer.Play();//������ʾ��
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
                         if (item.MessageItemType == ItemType.Mute)//���Ϊ
                         {
                             break;
                         }
                         item.UnReadCount++;//�ۼ�δ���Ǳ�
                         TotalUnReadCount++;
                         soundPlayer.Play();//������ʾ��
                         Messenger.Default.Send(true, MainViewNotifactions.FlashWindow);
                         if (newMsg.jid.Length > 10)//���JidΪȺ��ID
                         {
                             var room = new Room().GetByJid(newMsg.jid);//��ȡȺ����Ϣ
                             if (room != null)
                             {
                                 item.ShowTitle = room.name;//�����ǳ�
                                 if (room.offlineNoPushMsg == 1)//��������͵Ļ�
                                 {
                                     item.MessageItemType = ItemType.Mute;//���Ⱥ��Ϊ����������������δ���Ǳ�
                                                                          //break;
                                 }
                             }
                         }
                         break;
                 }
             }
             #region ��ʾ����
             if (string.IsNullOrWhiteSpace(item.ShowTitle))
             {
                 if (newMsg.jid.Length > 10)//���JidΪȺ��ID
                 {
                     var room = new Room().GetByJid(newMsg.jid);//��ȡȺ����Ϣ
                     if (room != null)
                     {
                         item.ShowTitle = room.name;//�����ǳ�
                         if (room.offlineNoPushMsg == 1)//��������͵Ļ�
                         {
                             item.MessageItemType = ItemType.Mute;//���Ⱥ��Ϊ����������������δ���Ǳ�
                                                                  //break;
                         }
                     }
                 }
                 else//����ǵ�����Ϣ
                 {
                     if (newMsg.PlatformType > 0)//��ϢΪ������¼�豸ת��
                     {
                         switch (newMsg.PlatformType)//���ñ����ͷ��
                         {
                             case 1:
                                 item.ShowTitle = "�ҵ�Android";
                                 item.Tag = "android";
                                 break;
                             case 2:
                                 item.ShowTitle = "�ҵ�iPhone";
                                 item.Tag = "ios";
                                 break;
                             case 3:
                                 item.ShowTitle = "�ҵ�Web��";
                                 item.Tag = "web";
                                 break;
                             case 4:
                                 item.ShowTitle = "�ҵ�Mac����";
                                 item.Tag = "mac";
                                 break;
                             default:
                                 item.ShowTitle = "pc";
                                 item.Tag = "pc";
                                 break;
                         }
                     }
                     else//����������Ϣ����
                     {
                         if (newMsg.jid == Applicate.MyAccount.userId)//�ҷ�������Ϣ
                         {
                             item.ShowTitle = Applicate.MyAccount.nickname;//ʹ�ý����û��ǳ�
                         }
                         else
                         {
                             item.ShowTitle = new DataOfFriends().GetUserNameByUserId(newMsg.toUserId);//ʹ�÷����û��ǳ�
                         }
                     }
                 }
             }
             #endregion
             if (string.IsNullOrWhiteSpace(item.ShowTitle))//��������Ϊ�����Ƴ�������
             {
                 int tmpindex = RecentMessageList.IndexOf(item);
                 if (tmpindex >= 0)
                 {
                     RemoveMessageListItem(tmpindex);
                 }
             }
             else//���ⲻΪ�������������
             {
                 //��ʾ��ϢԤ������
                 item.Msg = newMsg;
                 item.MessageItemContent = newMsg.content;
             }
         });
        }
        #endregion

        #region Public Helper

        #region ��ȡRichTextBox���ı�
        /// <summary>
        /// ��ȡRichTextBox���ı�
        /// </summary>
        /// <param name="richTextBox">��Ҫ��ȡ�ı���rtb����</param>
        /// <returns>��ȡ�����ı�</returns>
        private string GetRTBText(System.Windows.Controls.RichTextBox richTextBox)
        {
            //��ȡ���е��ı�
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

        #region ��ȡRichTextBox���ı�ת�ַ���
        /// <summary>
        /// ��ȡRichTextBox���ı�ת�ַ���
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="objectId">@Ⱥ��ԱuserId</param>
        /// <returns></returns>
        private string ConvertDocumentToText(FlowDocument documents, ref string objectId)
        {
            //��ȡ���е��ı�
            StringBuilder sb = new StringBuilder();
            int blockCount = documents.Blocks.Count;
            //�Կ����ʽ����
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
            //��ȡ���е��ı�
            StringBuilder sb = new StringBuilder();
            //�Կ����ʽ����
            foreach (var item in paragraph.Inlines)
            {
                sb.Append(SetData(item, ref objectId));
            }
            return sb.ToString();
        }
        #endregion

        #region ��Document�л�ȡý�����
        /// <summary>
        /// ��Document�л�ȡý�����
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
                        string path = Applicate.LocalConfigData.ChatDownloadPath + "ͼƬ����" +
                            strDateTime + ".png";
                        img.Save(path);
                        tmpimg.Tag = path;//����·��
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
                    string path = Applicate.LocalConfigData.ChatDownloadPath + "ͼƬ����" +
                        strDateTime + rand.Next(10000) + ".png";
                    img.Save(path);
                    tmpimg.Tag = path;//����·��
                }
            }
            else if (blockUIContainer.Child != null)
            {

            }
            return tmpimg;
        }
        #endregion

        #region ������Ԫ��
        /// <summary>
        /// ������Ԫ��
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
