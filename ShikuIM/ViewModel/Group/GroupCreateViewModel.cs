using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// 创建群聊ViewModel
    /// </summary>
    public class GroupCreateViewModel : ViewModelBase
    {

        #region Private Member
        private bool confirmEnable;//确认按钮启用
        private string groupName = "";//名称
        private string groupDesc = "";//描述
        private bool enableCreate = true;
        private SnackbarMessageQueue snackbar;

        #endregion  

        #region Public Member
        /// <summary>
        /// SnackBarMessage 队列
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackbar; }
            set { snackbar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }

        /// <summary>
        /// 是否启用创建按钮
        /// </summary>
        public bool EnableCreate
        {
            get { return enableCreate; }
            set { enableCreate = value; RaisePropertyChanged(nameof(EnableCreate)); }
        }

        /// <summary>
        /// 群组描述
        /// </summary>
        public string GroupDesc
        {
            get { return groupDesc; }
            set
            {
                if (value == groupDesc)
                {
                    return;
                }

                groupDesc = value; RaisePropertyChanged(nameof(GroupDesc));
            }
        }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string GroupName
        {
            get { return groupName; }
            set
            {
                if (value == groupName)
                {
                    return;
                }

                groupName = EmojiFilter(value);//过滤Emoji表情字符
                RaisePropertyChanged(nameof(groupName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string EmojiFilter(string str)
        {
            return Regex.Replace(str, @"\p{Cs}", "");
        }

        /// <summary>
        /// 启用
        /// </summary>
        public bool ConfirmEnable
        {
            get { return confirmEnable; }
            set { confirmEnable = value; RaisePropertyChanged(nameof(ConfirmEnable)); }
        }

        /// <summary>
        /// 好友列表
        /// </summary>
        public ObservableCollection<MessageListItem> FriendList { get; set; } = new ObservableCollection<MessageListItem>();

        /// <summary>
        /// 选中的成员
        /// </summary>
        public ObservableCollection<MessageListItem> SelectedMember { get; set; } = new ObservableCollection<MessageListItem>();
        #endregion

        #region Commands

        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand => new RelayCommand(() => { Applicate.GetWindow<GroupCreate>().Hide(); });

        /// <summary>
        /// 最大化
        /// </summary>
        public ICommand MaxCommand => new RelayCommand(() =>
        {
            if (Applicate.GetWindow<GroupCreate>().WindowState == WindowState.Maximized)
            {
                Applicate.GetWindow<GroupCreate>().WindowState = WindowState.Normal;
            }
            else
            {
                Applicate.GetWindow<GroupCreate>().WindowState = WindowState.Maximized;
            }
        });

        /// <summary>
        /// 最小化命令
        /// </summary>
        public ICommand MinCommand => new RelayCommand(() => { Applicate.GetWindow<GroupCreate>().Hide(); });

        /// <summary>
        /// 确认创建
        /// </summary>
        public ICommand ConfirmInviteCommand => new RelayCommand(() =>
        {
            if (GroupName == "")
            {
                Snackbar.Enqueue("群组名称不能为空！");
                EnableCreate = true;
                return;
            }
            else if (GroupDesc == "")
            {
                Snackbar.Enqueue("群组描述不能为空！");
                EnableCreate = true;
                return;
            }
            else if (GroupName.Length > 20)
            {
                Snackbar.Enqueue("群名称长度不能大于20");
                EnableCreate = true;
                return;
            }
            BeginCreateGroup();
            EnableCreate = false;
        });

        /// <summary>
        /// 选中改变事件
        /// </summary>
        public ICommand SelectionChangedCommand => new RelayCommand<SelectionChangedEventArgs>((para) =>
        {
            if (para.AddedItems.Count > 0)//如果选中列表中数量大于0添加
            {
                var item = para.AddedItems[0] as MessageListItem;
                SelectedMember.Add(item);
            }
            else if (para.RemovedItems.Count > 0)
            {
                var item = para.RemovedItems[0] as MessageListItem;
                SelectedMember.Remove(item);
            }
            else
            {
                SelectedMember.Clear();
            }
        });

        /// <summary>
        /// 选中改变事件
        /// </summary>
        public ICommand RemoveSelectedCommand => new RelayCommand<MessageListItem>((para) =>
        {
            var item = para;
            SelectedMember.Remove(item);
        });

        #endregion

        #region 构造函数
        public GroupCreateViewModel()
        {
            InitialProperties();
        }
        #endregion

        #region 重置属性
        /// <summary>
        /// 重置属性
        /// </summary>
        public void InitialProperties()
        {
            groupDesc = "";
            groupName = "";
            SelectedMember.Clear();//清空列表
            FriendList.Clear();
            EnableCreate = true;
            Snackbar = new SnackbarMessageQueue();
            Task.Run(() =>
            {
                Task.Delay(100);
                var friends = new DataOfFriends().GetFriendsList();
                //填充好友列表
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var item in friends)
                    {
                        if (item.toUserId == "10000")
                        {
                            continue;
                        }
                        FriendList.Add(item.ToMsgListItem());
                    }
                });
            });
        }
        #endregion


        #region 开始创建群聊
        /// <summary>
        /// 开始创建群聊
        /// </summary>
        public void BeginCreateGroup()
        {
            /* if (SelectedMember.Count <= 1)
             {
                 Snackbar.Enqueue("请选择至少两个好友!");
                 return;
             }
             else
             */
            EnableCreate = true;
            ConfirmEnable = false;//禁用确认
            Room room = new Room();//实例化新的Room对象
            //room.IsJoined = true;//我已加入的群
            room.name = GroupName;
            room.desc = GroupDesc;
            room.jid = Guid.NewGuid().ToString("N");//随机生成
            room.nickname = Applicate.MyAccount.nickname; //建群
            room.userSize = SelectedMember.Count + 1;//成员数量
            #region 添加群成员
            for (int i = 0; i < SelectedMember.Count; i++)
            {
                //添加邀请的好友
                room.members.Add(new DataofMember()
                {
                    nickname = (SelectedMember[i]).MessageTitle,
                    userId = (SelectedMember[i]).Jid,
                    groupid = room.id,
                    role = MemberRole.Member
                });
            }
            //添加自己到群成员中
            room.members.Add(new DataofMember()
            {
                nickname = Applicate.MyAccount.nickname,
                userId = Applicate.MyAccount.userId,
                groupid = room.id,
                role = MemberRole.Owner
            });
            #endregion
            List<string> members = new List<string>();
            for (int i = 0; i < SelectedMember.Count; i++)
            {
                members.Add(SelectedMember[i].Jid.ToString());//添加成员
            }
            members.Add(Applicate.MyAccount.userId);//添加自己
            //异步执行
            Task.Run(() =>
            {
                var client = ShiKuManager.CreateGroup(room, members);
                client.Tag = room;////设置Tag为本地群组
                client.UploadDataCompleted += CreateGroupChatComplete;
            });
        }
        #endregion

        #region 建群成功后
        /// <summary>
        /// 建群成功后
        /// </summary>
        /// <param name="sender">源</param>
        /// <param name="e">事件</param>
        private void CreateGroupChatComplete(object sender, UploadDataCompletedEventArgs e)
        {
            var tmpstr = Encoding.UTF8.GetString(e.Result);
            var result = JsonConvert.DeserializeObject<JsonRoom>(tmpstr);
            if (result != null && result.resultCode == 1)
            {
                if (result.data.members == null || result.data.members.Count == 0)
                {
                    var room = ((HttpClient)sender).Tag as Room;////获取群组
                    //jsonres.data.members = troom.members;//成员列表
                    room.members[0].AutoInsertRange(room.members, result.data.id);
                    result.data.userSize = room.userSize;//成员数量
                }
                Messenger.Default.Send(result.data, CommonNotifications.CreateNewGroupFinished);//通知主窗口

                App.Current.Dispatcher.Invoke(() =>
                {
                    Messenger.Default.Send(true, GroupCreate.CloseWindow);//关闭窗口
                });
                
                Snackbar.Enqueue("创建群聊成功！");
            }
            else
            {
                Snackbar.Enqueue("群聊创建失败！\n" + result.resultMsg ?? "");
                EnableCreate = true;
            }
        }

        #endregion

        #region 刷新所有图片对象
        /// <summary>
        /// 刷新所有图片对象
        /// </summary>
        internal void RefreshAllImg(string jid)
        {
            if (!String.IsNullOrWhiteSpace(jid))
            {
                var itemObj = FriendList.FirstOrDefault(d => d.Jid == jid);
                if (itemObj != null)
                {
                    itemObj.Jid = jid;
                }
            }
        }
        #endregion


        #region 修改对应Jid昵称
        /// <summary>
        /// 修改对应Jid昵称
        /// </summary>
        /// <param name="jid"></param>
        /// <param name="nickname"></param>
        public void OnUserNameChanged(string jid, string nickname)
        {
            for (int i = 0; i < FriendList.Count; i++)
            {
                if (FriendList[i].Jid == jid)
                {
                    FriendList[i].MessageTitle = nickname;//修改昵称
                }
            }

            for (int i = 0; i < SelectedMember.Count; i++)
            {
                if (SelectedMember[i].Jid == jid)
                {
                    SelectedMember[i].MessageTitle = nickname;//修改昵称
                }
            }
        }
        #endregion

    }
}