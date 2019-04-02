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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// 群聊详情ViewModel
    /// </summary>
    public class GroupDetialViewModel : ViewModelBase
    {

        #region Properties
        /// <summary>
        /// 标识用于初始化群组的通知Token，参数为RoomId
        /// </summary>
        public static string InitialGroupDetial { get; } = nameof(InitialGroupDetial);
        #region Private Properties
        private SmartHint hintOfSearch;//文本框内提示
        //private GroupChatDetail window;//窗口对象
        private PackIcon groupNameEdit;
        private Room displayRoom = new Room();
        private SnackbarMessageQueue snackbar;
        private string myNickName = "";//群内昵称
        private string editGroupName;
        private string editGroupDesc;
        private string exitContent = "退出群组";
        private string searchText = "";
        private string dialogTitle = "";
        private bool editNameVIsible = true;
        private bool editDescVisible = true;
        private bool isRoomOwner = false;
        private bool membersVisible = true;
        private bool searchVisible = true;
        private bool memberAddVisible = true;
        private bool isGroupNameEditing = false;
        private bool isGroupDescEditing = false;
        private bool isNickNameEditing = false;
        private bool isOthersVerifyEditing = false;
        private bool isMyVerifyEditing = false;
        private bool isVerifyEditing = false;
        private bool isBannedTimeEditing = false;
        private bool isNoticeEditing = false;
        private bool isEditing = false;
        private bool isJoinRoom = false;
        private string tempGroupName;
        private string tempGroupDesc;
        private string tempNickName;
        private string tempMyVerifyDesc;
        private string tempOthersVerifyDesc;
        private string tempNotice;
        private string roomQRCode;
        private int panelIndex = 0;
        private bool isNotDisturb;
        /// <summary>
        /// 添加群成员FAB是否隐藏
        /// </summary>
        private bool IsInviteFloatButtonHide = false;
        private bool isAllMemberBanned;

        private bool isOfflineNoPushMsgEnable = true;
        private bool isShowReadEnable = true;
        private bool isPublicGroupEnable = true;
        private bool isNeedVerifyEnable = true;
        public bool isShowMemberEnable = true;
        private bool isAllowSendCardEnable = true;
        private bool isAllowInviteFriendEnable = true;
        private bool isAllowUploadFileEnable = true;
        private bool isAllowConferenceEnable = true;
        private bool isAllowSpeakCourseEnable = true;
        private bool isRoomBannedEnable = true;
        private DataofMember tempBannedUser;
        #endregion


        #region Public Properties


        /// <summary>
        /// 是否在编辑群资料(控制弹出框)
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
            set { isEditing = value; RaisePropertyChanged(nameof(IsEditing)); }
        }

        /// <summary>
        /// 对话框标题
        /// </summary>
        public string DialogTitle
        {
            get
            {
                return dialogTitle;
            }
            set
            {
                if (dialogTitle == value)
                {
                    return;
                }

                dialogTitle = value;
                RaisePropertyChanged(nameof(DialogTitle));
            }
        }

        /// <summary>
        /// 群昵称是否在编辑
        /// </summary>
        public bool IsGroupNameEditing
        {
            get { return isGroupNameEditing; }
            set
            {
                isGroupNameEditing = value;
                if (isGroupNameEditing)
                {
                    DialogTitle = "编辑群名称";
                }
                RaisePropertyChanged(nameof(IsGroupNameEditing));
            }
        }

        /// <summary>
        /// 临时群名称
        /// </summary>
        public string TempGroupName
        {
            get { return tempGroupName; }
            set { tempGroupName = value; RaisePropertyChanged(nameof(TempGroupName)); }
        }

        /// <summary>
        /// 群描述是否在编辑
        /// </summary>
        public bool IsGroupDescEditing
        {
            get { return isGroupDescEditing; }
            set
            {
                isGroupDescEditing = value;
                if (isGroupDescEditing)
                {
                    DialogTitle = "编辑群描述";
                }

                RaisePropertyChanged(nameof(IsGroupDescEditing));
            }
        }
        /// <summary>
        /// 临时群描述
        /// </summary>
        public string TempGroupDesc
        {
            get { return tempGroupDesc; }
            set { tempGroupDesc = value; RaisePropertyChanged(nameof(TempGroupDesc)); }
        }

        /// <summary>
        /// 自己进群验证是否在编辑
        /// </summary>
        public bool IsMyVerifyEditing
        {
            get { return isMyVerifyEditing; }
            set
            {
                isMyVerifyEditing = value;
                if (isMyVerifyEditing)
                {
                    DialogTitle = "编辑进群验证";
                }
                RaisePropertyChanged(nameof(IsMyVerifyEditing));
            }
        }

        /// <summary>
        /// 临时申请进群验证
        /// </summary>
        public string TempMyVerifyDesc
        {
            get { return tempMyVerifyDesc; }
            set { tempMyVerifyDesc = value; RaisePropertyChanged(nameof(TempMyVerifyDesc)); }
        }

        /// <summary>
        /// 群内昵称是否在编辑
        /// </summary>
        public bool IsNickNameEditing
        {
            get { return isNickNameEditing; }
            set
            {
                isNickNameEditing = value;
                if (isNickNameEditing)
                {
                    DialogTitle = "编辑群内昵称";
                }

                RaisePropertyChanged(nameof(IsNickNameEditing));
            }
        }

        /// <summary>
        /// 临时群昵称
        /// </summary>
        public string TempNickName
        {
            get { return tempNickName; }
            set { tempNickName = value; RaisePropertyChanged(nameof(TempNickName)); }
        }

        /// <summary>
        /// 是否为群主
        /// </summary>
        public bool IsRoomOwner
        {
            get { return isRoomOwner; }
            set { isRoomOwner = value; RaisePropertyChanged(nameof(IsRoomOwner)); }
        }



        /// <summary>
        /// 提示控件
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackbar; }
            set { snackbar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }

        /// <summary>
        /// 是否为免打扰状态
        /// </summary>
        public bool IsNotDisturb
        {
            get { return isNotDisturb; }
            set { isNotDisturb = value; RaisePropertyChanged(nameof(IsNotDisturb)); }
        }

        /// <summary>
        /// 是否为全员禁言
        /// </summary>
        public bool IsAllMemberBanned
        {
            get { return isAllMemberBanned; }
            set { isAllMemberBanned = value; RaisePropertyChanged(nameof(IsAllMemberBanned)); }
        }

        /// <summary>
        /// 当前面板选中索引
        /// </summary>
        public int PanelIndex
        {
            get { return panelIndex; }
            set { panelIndex = value; RaisePropertyChanged(nameof(PanelIndex)); }
        }

        /// <summary>
        /// 临时邀请进群验证
        /// </summary>
        public string TempOthersVerifyDesc
        {
            get { return tempOthersVerifyDesc; }
            set { tempOthersVerifyDesc = value; RaisePropertyChanged(nameof(TempOthersVerifyDesc)); }
        }
        /// <summary>
        /// 临时公告内容
        /// </summary>
        public string TempNotice
        {
            get { return tempNotice; }
            set { tempNotice = value; RaisePropertyChanged(nameof(TempNotice)); }
        }

        /// <summary>
        /// 禁言是否在编辑
        /// </summary>
        public bool IsBannedTimeEditing
        {
            get { return isBannedTimeEditing; }
            set
            {
                isBannedTimeEditing = value;
                RaisePropertyChanged(nameof(IsBannedTimeEditing));
            }
        }
        /// <summary>
        /// 公告是否在编辑
        /// </summary>
        public bool IsNoticeEditing
        {
            get { return isNoticeEditing; }
            set
            {
                isNoticeEditing = value;
                RaisePropertyChanged(nameof(IsNoticeEditing));
            }
        }
        /// <summary>
        /// 临时选中禁言成员id
        /// </summary>
        public DataofMember TempBannedUser
        {
            get { return tempBannedUser; }
            set
            {
                tempBannedUser = value;
                RaisePropertyChanged(nameof(TempBannedUser));
            }
        }
        /// <summary>
        /// 是否在发送群验证(控制弹出框)
        /// </summary>
        public bool IsVerifyEditing
        {
            get { return isVerifyEditing; }
            set { isVerifyEditing = value; RaisePropertyChanged(nameof(IsVerifyEditing)); }
        }


        /// <summary>
        /// 好友验证是否在编辑
        /// </summary>
        public bool IsOthersVerifyEditing
        {
            get { return isOthersVerifyEditing; }
            set
            {
                isOthersVerifyEditing = value;
                if (isOthersVerifyEditing)
                {
                    DialogTitle = "编辑进群验证";
                }
                RaisePropertyChanged(nameof(IsOthersVerifyEditing));
            }
        }

        /// <summary>
        /// 编辑按钮图标
        /// </summary>
        public PackIcon GroupNameEdit
        {
            get
            {
                if (groupNameEdit == null)
                {
                    groupNameEdit = new PackIcon();
                    groupNameEdit.Kind = PackIconKind.Pencil;
                }
                return groupNameEdit;
            }
            set { groupNameEdit = value; RaisePropertyChanged(nameof(GroupNameEdit)); }
        }


        /// <summary>
        /// 编辑的群名称
        /// </summary>
        public string EditGroupName
        {
            get { return editGroupName; }
            set { editGroupName = value; RaisePropertyChanged(nameof(EditGroupName)); }
        }

        /// <summary>
        /// 搜索文本框提示
        /// </summary>
        public SmartHint HintOfSearch
        {
            get { return hintOfSearch; }
            set { hintOfSearch = value; RaisePropertyChanged(nameof(HintOfSearch)); }
        }

        /// <summary>
        /// 搜索文字
        /// </summary>
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                SearchMember(searchText);//搜索操作
                //RaisePropertyChanged(nameof(SearchText));
            }
        }

        /// <summary>
        /// 销毁群按钮内容
        /// </summary>
        public string ExitBtnContent
        {
            get { return exitContent; }
            set { exitContent = value; RaisePropertyChanged(nameof(ExitBtnContent)); }
        }

        /// <summary>
        /// 当前群组详情
        /// </summary>
        public Room DisplayRoom
        {
            get { return displayRoom; }
            set { displayRoom = value; RaisePropertyChanged(nameof(DisplayRoom)); }
        }
        public string MyNickName
        {
            get { return myNickName; }
            set { myNickName = value; RaisePropertyChanged(nameof(MyNickName)); }
        }
        /// <summary>
        /// 搜索控件显示
        /// </summary>
        public bool SearchVisible
        {
            get { return searchVisible; }
            set { searchVisible = value; RaisePropertyChanged(nameof(SearchVisible)); }
        }

        /// <summary>
        /// 邀请好友显示
        /// </summary>
        public bool MemberAddVisible
        {
            get { return memberAddVisible; }
            set { memberAddVisible = value; RaisePropertyChanged(nameof(MemberAddVisible)); }
        }

        /// <summary>
        /// 显示编辑群公告
        /// </summary>
        public bool EditDescVisible
        {
            get { return editDescVisible; }
            set { editDescVisible = value; RaisePropertyChanged(nameof(EditDescVisible)); }
        }


        /// <summary>
        /// 显示群成员
        /// </summary>
        public bool MembersVisible
        {
            get { return membersVisible; }
            set
            {
                membersVisible = value;
                RaisePropertyChanged(nameof(MembersVisible));
            }
        }

        /// <summary>
        /// 显示编辑群昵称
        /// </summary>
        public bool EditNameVIsible
        {
            get { return editNameVIsible; }
            set
            {
                editNameVIsible = value;
                RaisePropertyChanged(nameof(EditNameVIsible));
            }
        }


        /// <summary>
        /// 编辑的群描述
        /// </summary>
        public string EditGroupDesc
        {
            get { return editGroupDesc; }
            set
            {
                editGroupDesc = value;
                RaisePropertyChanged(nameof(EditGroupDesc));
            }
        }
        /// <summary>
        /// 当前用户是否在本群
        /// </summary>
        public bool IsJoinRoom
        {
            get { return isJoinRoom; }
            set
            {
                isJoinRoom = value; RaisePropertyChanged(nameof(IsJoinRoom));
            }
        }
        /// <summary>
        /// 群二维码
        /// </summary>
        public string RoomQRCode
        {
            get { return roomQRCode; }
            set
            {
                roomQRCode = value; RaisePropertyChanged(nameof(RoomQRCode));
            }
        }

        /// <summary>
        /// 未加入的好友列表
        /// </summary>
        public ObservableCollection<MessageListItem> FriendList { get; set; }

        /// <summary>
        /// 群成员列表（搜索结果）
        /// </summary>
        public ObservableCollection<DataofMember> MembersList { get; set; }

        /// <summary>
        /// 普通成员列表
        /// </summary>
        public ObservableCollection<DataofMember> NormalMembersList { get; set; }

        /// <summary>
        /// 管理员列表
        /// </summary>
        public ObservableCollection<DataofMember> AdminsList { get; set; }

        /// <summary>
        /// 五个预览成员列表    
        /// </summary>
        public ObservableCollection<DataofMember> PreviewsList { get; set; }

        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsOfflineNoPushMsgEnable
        {
            get { return isOfflineNoPushMsgEnable; }
            set { isOfflineNoPushMsgEnable = value; RaisePropertyChanged(nameof(IsOfflineNoPushMsgEnable)); }
        }
        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsShowReadEnable
        {
            get { return isShowReadEnable; }
            set { isShowReadEnable = value; RaisePropertyChanged(nameof(IsShowReadEnable)); }
        }
        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsPublicGroupEnable
        {
            get { return isPublicGroupEnable; }
            set { isPublicGroupEnable = value; RaisePropertyChanged(nameof(IsPublicGroupEnable)); }
        }
        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsNeedVerifyEnable
        {
            get { return isNeedVerifyEnable; }
            set { isNeedVerifyEnable = value; RaisePropertyChanged(nameof(IsNeedVerifyEnable)); }
        }
        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsShowMemberEnable
        {
            get { return isShowMemberEnable; }
            set { isShowMemberEnable = value; RaisePropertyChanged(nameof(IsShowMemberEnable)); }
        }
        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsAllowSendCardEnable
        {
            get { return isAllowSendCardEnable; }
            set { isAllowSendCardEnable = value; RaisePropertyChanged(nameof(IsAllowSendCardEnable)); }
        }

        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsAllowInviteFriendEnable
        {
            get { return isAllowInviteFriendEnable; }
            set { isAllowInviteFriendEnable = value; RaisePropertyChanged(nameof(IsAllowInviteFriendEnable)); }
        }

        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsAllowUploadFileEnable
        {
            get { return isAllowUploadFileEnable; }
            set { isAllowUploadFileEnable = value; RaisePropertyChanged(nameof(IsAllowUploadFileEnable)); }
        }

        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsAllowConferenceEnable
        {
            get { return isAllowConferenceEnable; }
            set { isAllowConferenceEnable = value; RaisePropertyChanged(nameof(IsAllowConferenceEnable)); }
        }

        /// <summary>
        /// 防止开关连续点击
        /// </summary>
        public bool IsAllowSpeakCourseEnable
        {
            get { return isAllowSpeakCourseEnable; }
            set { isAllowSpeakCourseEnable = value; RaisePropertyChanged(nameof(IsAllowSpeakCourseEnable)); }
        }

        /// <summary>
        /// 全员禁言按钮是否启用
        /// </summary>
        public bool IsRoomBannedBtnEnable
        {
            get { return isRoomBannedEnable; }
            set { isRoomBannedEnable = value; RaisePropertyChanged(nameof(IsRoomBannedBtnEnable)); }
        }
        #endregion
        #region Commands


        /// <summary>
        /// 取消编辑Command
        /// </summary>
        public ICommand CancelEditCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsGroupDescEditing = false;
                    IsGroupNameEditing = false;
                    IsEditing = false;//关闭弹出框
                });
            }
        }

        /// <summary>
        /// 设置Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoSettingCommand => new RelayCommand(() => { PanelIndex = 2; });

        /// <summary>
        /// 编辑群聊信息确认Command
        /// </summary>
        public ICommand GroupInfoConfirmCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (IsGroupNameEditing)
                    {
                        UpdateGroupName();//更新群组名称
                    }

                    if (IsGroupDescEditing)
                    {
                        UpdateGroupDesc();//更新群组描述
                    }

                    if (IsNickNameEditing)
                    {
                        UpdateNickName();//更新我的昵称
                    }

                    if (IsMyVerifyEditing || IsOthersVerifyEditing)
                    {
                        SendRoomVerify();//更新群组验证
                    }
                    if (IsNoticeEditing)
                    {
                        UpdateNotice();//更新群公告
                    }
                });
            }
        }

        /// <summary>
        /// 编辑群描述Command
        /// </summary>
        public ICommand EditGroupDescCommand => new RelayCommand(() =>
        {
            IsEditing = true;//打开
            IsVerifyEditing = false;
            IsGroupDescEditing = true;//编辑群聊公告
            IsGroupNameEditing = false;
            IsNickNameEditing = false;
            IsMyVerifyEditing = false;
            IsOthersVerifyEditing = false;
            TempGroupDesc = DisplayRoom.desc;//赋值临时变量
        });



        /// <summary>
        /// 滚动改变事件
        /// </summary>
        public ICommand ScrollChanged => new RelayCommand<ScrollChangedEventArgs>((e) =>
        {
            var mainWindow = Applicate.GetWindow<GroupChatDetial>();//获取群详情窗口
            if (mainWindow == null)
            {
                return;
            }
            var storyboard = new Storyboard();
            if (e.VerticalChange > 0)//如果滚动距离大于0为向上滚动
            {
                if (!IsInviteFloatButtonHide)
                {
                    storyboard = mainWindow.FindResource("OnScrollChangedDown") as Storyboard;
                    IsInviteFloatButtonHide = true;
                    storyboard.Begin();
                }
            }
            else//否则为向下滚动
            {
                if (IsInviteFloatButtonHide)
                {
                    storyboard = mainWindow.FindResource("ScrollViewerUp") as Storyboard;
                    IsInviteFloatButtonHide = false;
                    storyboard.Begin();
                }
            }
        });

        /// <summary>
        /// 返回主页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoMainPageCommand => new RelayCommand(() => { PanelIndex = 0; FriendList.Clear(); GC.Collect(); });

        /// <summary>
        /// 公告成员页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoItemsPageCommand => new RelayCommand(() => { PanelIndex = 3; });

        /// <summary>
        /// 禁言页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoBannedPageCommand => new RelayCommand(() =>
        {
            ReLoadMembersList(true);

            PanelIndex = 8;
        });

        /// <summary>
        /// 指定管理员页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoAddAdminPageCommand => new RelayCommand(() =>
        {
            ReLoadMembersList(false);
            PanelIndex = 6;
        });

        /// <summary>
        /// 取消管理员页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoRemoveAdminPageCommand => new RelayCommand(() =>
        {
            RefreshAdminsList();
            PanelIndex = 7;
        });

        /// <summary>
        /// 移除成员页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoRemoveMemberPageCommand => new RelayCommand(() =>
        {
            ReLoadMembersList(true);
            PanelIndex = 9;
        });
        /// <summary>
        /// 转让群组
        /// </summary>
        public ICommand GotoTransferRoomPageCommand => new RelayCommand(() =>
        {
            ReLoadMembersList(true);
            PanelIndex = 10;
        });
        /// <summary>
        /// 公告Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoNoticePageCommand => new RelayCommand(() =>
        {
            PanelIndex = 5;
        });
        /// <summary>
        /// 禁言操作
        /// </summary>
        public ICommand BannedMemberCommand => new RelayCommand<DataofMember>((memberObj) =>
        {
            TempBannedUser = memberObj;
            IsBannedTimeEditing = true;
        });

        /// <summary>
        /// 指定管理员操作
        /// </summary>
        public ICommand AddAdminsCommand => new RelayCommand<DataofMember>((memberObj) =>
        {
            var client = APIHelper.SetRoomAdminAsync(DisplayRoom.id, memberObj.userId, (int)MemberRole.Admin);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var jsonObj = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (jsonObj.resultCode == 1)
                    {
                        //回执处理
                        DisplayRoom.members.First(d => d.userId == memberObj.userId).role = MemberRole.Admin;
                        RefreshAdminsList();//更新管理员表
                        ReLoadMembersList(false);//更新普通成员表
                        Snackbar.Enqueue("已将 " + memberObj.nickname + " 设为群管理员");
                    }
                    else
                    {
                        Snackbar.Enqueue("指定管理员失败:" + jsonObj.resultMsg);
                    }
                }
                else
                {
                    Snackbar.Enqueue("制定管理员失败:" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 取消管理员操作
        /// </summary>
        public ICommand RemoveAdminsCommand => new RelayCommand<DataofMember>((memberObj) =>
        {
            if (MessageBox.Show("是否取消 " + memberObj.nickname + " 管理员权限？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var client = APIHelper.SetRoomAdminAsync(DisplayRoom.id, memberObj.userId, (int)MemberRole.Member);
                client.UploadDataCompleted += (sen, res) =>
                {
                    if (res.Error == null)
                    {
                        var jsonObj = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                        if (jsonObj.resultCode == 1)
                        {
                            //回执处理
                            DisplayRoom.members.First(d => d.userId == memberObj.userId).role = MemberRole.Member;
                            RefreshAdminsList();//更新管理员表
                            ReLoadMembersList(false);//更新普通成员表
                            Snackbar.Enqueue("已取消 " + memberObj.nickname + " 管理员权限");
                        }
                        else
                        {
                            Snackbar.Enqueue("取消管理员失败:" + jsonObj.resultMsg);
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("取消管理员失败:" + res.Error.Message);
                    }
                };
            }
        });

        /// <summary>
        /// 指定群主操作
        /// </summary>
        public ICommand TransferCommand => new RelayCommand<DataofMember>((memberObj) =>
        {
            if (MessageBox.Show("确定选择 " + memberObj.nickname + " 为新群主，你将自动放弃群主身份", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var client = APIHelper.RoomTransferAsync(DisplayRoom.id, memberObj.userId);
                client.UploadDataCompleted += (sen, res) =>
                {
                    if (res.Error == null)
                    {
                        var jsonObj = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                        if (jsonObj.resultCode == 1)
                        {
                            //回执处理
                            DisplayRoom.members.First(d => d.userId == memberObj.userId).role = MemberRole.Owner;
                            DisplayRoom.members.First(d => d.userId == Applicate.MyAccount.userId).role = MemberRole.Member;
                            RefreshAdminsList();//更新管理员表
                            ReLoadMembersList(false);//更新普通成员表
                            IsRoomOwner = false;//我不是群主
                            EditDescVisible = false;//也不是管理员
                            SearchMember(SearchText);
                            PriviewMembers();//显示预览页成员
                            PanelIndex = 0;//跳到第一页
                            Snackbar.Enqueue("转让群主管理权限成功");
                        }
                        else
                        {
                            Snackbar.Enqueue("转让群主管理权限失败:" + jsonObj.resultMsg);
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("转让群主管理权限失败:" + res.Error.Message);
                    }
                };
            }
        });

        /// <summary>
        /// 移除成员操作
        /// </summary>
        public ICommand RemoveMemberCommand => new RelayCommand<DataofMember>((memberObj) =>
        {
            if (MessageBox.Show("是否将 " + memberObj.nickname + " 移出群组？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var client = APIHelper.DeleteRoomMemberAsync(DisplayRoom.id, memberObj.userId);
                client.UploadDataCompleted += (sen, res) =>
                {
                    if (res.Error == null)
                    {
                        var restxt = Encoding.UTF8.GetString(res.Result);
                        var jsonObj = JsonConvert.DeserializeObject<JsonBase>(restxt);
                        if (jsonObj.resultCode == 1)
                        {
                            //回执处理
                            var mem = DisplayRoom.members.FirstOrDefault(d => d.userId == memberObj.userId);
                            new DataofMember().Delete(mem.userId);//更新数据库
                            DisplayRoom.members.Remove(mem);//更新当前窗口
                            new Room().UpdateMemberSize(displayRoom.id, displayRoom.members.Count);//更新成员数量
                            DisplayRoom.userSize -= 1;//更新人数
                            RefreshAdminsList();//更新管理员列表
                            var tmpMembers = new Dictionary<string, List<DataofMember>>();
                            tmpMembers.Add(displayRoom.jid, new List<DataofMember> { memberObj });//设置更新信息
                            Messenger.Default.Send(tmpMembers, CommonNotifications.RemoveGroupMember);//通知其他页面更新群成员
                            ReLoadMembersList(true);//更新普通成员表
                            LoadFriendListExceptMembersAsync();//更新好友表
                            SearchMember(searchText);//更新群成员表
                            PriviewMembers();//更新预览五个成员
                            Snackbar.Enqueue("已将 " + memberObj.nickname + " 移出群组");
                        }
                        else
                        {
                            Snackbar.Enqueue("移出成员失败:" + jsonObj.resultMsg);
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("移除成员失败:" + res.Error.Message);
                    }
                };
            }
        });

        /// <summary>
        /// 成员页面Command
        /// </summary>
        /// <returns></returns>
        public ICommand GotoInvitePageCommand => new RelayCommand(() =>
        {
            if (FriendList.Count == 0)
            {
                LoadFriendListExceptMembersAsync();//加载好友列表
            }
            PanelIndex = 1;
        });

        #region 关闭窗口Command
        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(true, GroupChatDetial.CloseGroupDetialWindow);
                });
            }
        }
        #endregion

        /// <summary>
        /// 搜索Command
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SearchMember(SearchText);
                });
            }
        }

        /// <summary>
        /// 个人
        /// </summary>
        public ICommand DetailCommand => new RelayCommand<string>((para) =>
        {
            if (para != null)
            {
                var userid = para.ToString();
                if (userid == Applicate.MyAccount.userId)
                {
                    Personal.GetPersonal();
                }
                else
                {
                    Messenger.Default.Send(para.ToString());
                    UserDetailView.GetWindow().Show();//显示
                }
            }
        });

        /// <summary>
        /// 更新群成员禁言
        /// </summary>
        public ICommand MemberBannedCommand => new RelayCommand<long>((para) =>
        {
            if (IsBannedTimeEditing)
            {
                long talkTime = para == 0 ? 0 : Helpers.DatetimeToStamp(DateTime.Now) + para;
                var client = APIHelper.SetMemberTalkTimeAsync(DisplayRoom.id, TempBannedUser.userId, talkTime.ToString());
                client.UploadDataCompleted += (sen, res) =>
                {
                    if (res.Error == null)
                    {
                        var restxt = Encoding.UTF8.GetString(res.Result);
                        var jsonObj = JsonConvert.DeserializeObject<JsonBase>(restxt);
                        if (jsonObj.resultCode == 1)
                        {
                            //回执处理
                            TempBannedUser = new DataofMember();
                            IsBannedTimeEditing = false;
                            Snackbar.Enqueue("修改成员禁言成功!");
                        }
                        else
                        {
                            Snackbar.Enqueue("设置成员禁言失败：" + jsonObj.resultMsg);
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("禁言失败:" + res.Error.Message);
                    }
                };
            }
        });

        /// <summary>
        /// 编辑群名称Command
        /// </summary>
        public ICommand EditGroupNameCommand => new RelayCommand(() =>
        {
            IsEditing = true;//打开
            IsVerifyEditing = false;
            IsGroupNameEditing = true;//编辑群聊名称
            IsGroupDescEditing = false;
            IsNickNameEditing = false;
            IsMyVerifyEditing = false;
            IsOthersVerifyEditing = false;
            TempGroupName = DisplayRoom.name;//赋值临时变量
        });

        /// <summary>
        /// 编辑群公告Command
        /// </summary>
        public ICommand EditNoticeCommand => new RelayCommand(() =>
        {
            DialogTitle = "发布公告";
            TempNotice = "";
            IsNoticeEditing = true;
        });


        /// <summary>
        /// 编辑群内昵称Command
        /// </summary>
        public ICommand EditNickNameCommand => new RelayCommand(() =>
        {
            IsEditing = true;//打开
            IsVerifyEditing = false;
            IsNickNameEditing = true;//编辑群聊名称
            IsGroupDescEditing = false;
            IsGroupNameEditing = false;
            isMyVerifyEditing = false;
            IsOthersVerifyEditing = false;
            TempNickName = MyNickName;//赋值临时变量
        });

        /// <summary>
        /// 退/解散群聊Command
        /// </summary>
        public ICommand QuitOrDeleteCommand => new RelayCommand(() =>
        {
            var groupitem = new MessageListItem();
            groupitem.Id = DisplayRoom.id;//唯一Id
            groupitem.Jid = DisplayRoom.jid;//群Jid
            groupitem.MessageTitle = DisplayRoom.nickname;//群昵称
            groupitem.Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(displayRoom.jid);
            //查询自己的身份
            var self = DisplayRoom.members.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId);
            if (self != null && self.role == MemberRole.Owner)
            {
                var client = APIHelper.DismissRoomAsync(groupitem);
                client.UploadDataCompleted += DeleteComplete;//指定回调
            }
            else if (IsJoinRoom)
            {
                var teclient = ShiKuManager.LeaveRoom(groupitem);
                teclient.UploadDataCompleted += DeleteComplete;//指定回调
            }
            else if (DisplayRoom.isNeedVerify == 1)
            {
                //要验证
                IsEditing = true;//打开
                IsVerifyEditing = false;
                IsNickNameEditing = false;//编辑群聊名称
                IsGroupDescEditing = false;
                IsGroupNameEditing = false;
                IsMyVerifyEditing = true;
                IsOthersVerifyEditing = false;
                TempMyVerifyDesc = "";//赋值临时变量
            }
        });

        /// <summary>
        /// 取消搜索Command
        /// </summary>
        public ICommand CancelSearchCommand => new RelayCommand(() =>
        {
            //取消搜索

        });

        /// <summary>
        /// 邀请进群Command
        /// </summary>
        public ICommand InviteFriendCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (selectedInvites.Count < 1)
                    {
                        //向前翻
                        Snackbar.Enqueue("请至少选择一个好友");
                        return;
                    }
                    if (DisplayRoom.members.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*查询出自己的身份编号*/).role != MemberRole.Owner && DisplayRoom.isNeedVerify == 1)//需要验证
                    {
                        IsEditing = false;//打开
                        IsVerifyEditing = true;
                        IsNickNameEditing = false;//编辑群聊名称
                        IsGroupDescEditing = false;
                        IsGroupNameEditing = false;
                        IsMyVerifyEditing = false;
                        IsOthersVerifyEditing = true;
                        TempOthersVerifyDesc = "";//赋值临时变量

                        return;
                    }
                    //邀请的成员
                    List<string> members = new List<string>();
                    for (int i = 0; i < selectedInvites.Count; i++)
                    {
                        members.Add(((MessageListItem)selectedInvites[i]).Jid);
                    }
                    var client = ShiKuManager.InviteGroup(DisplayRoom.id, members);//访问接口
                    client.UploadDataCompleted += (send, res) =>
                    {
                        if (res.Error == null)
                        {
                            var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                            if (rtn.resultCode == 1)
                            {
                                foreach (var tjid in members)
                                {
                                    var user = FriendList.FirstOrDefault(f => f.Jid == tjid);
                                    DisplayRoom.members.Add(new DataofMember() { userId = user.Jid, nickname = user.MessageTitle, role = MemberRole.Member, groupid = DisplayRoom.id });
                                    DisplayRoom.userSize += 1;//更新群组详情窗口
                                }
                                displayRoom.UpdateMemberSize(displayRoom.id, displayRoom.userSize);//更新数据库
                                Messenger.Default.Send(displayRoom, CommonNotifications.AddGroupMemberSize);//通知其他页面更新群组数据  
                                ReLoadMembersList(false);//更新普通成员表
                                LoadFriendListExceptMembersAsync();//更新好友表
                                SearchMember(searchText);//更新群成员表
                                PriviewMembers();//更新预览五个成员
                                Snackbar.Enqueue("邀请成功");
                                IsOfflineNoPushMsgEnable = true;
                            }
                        }
                        else
                        {
                            Snackbar.Enqueue("添加失败:" + res.Error.Message);
                        }
                    };
                });
            }
        }

        /// <summary>
        /// 群文件共享Command
        /// </summary>
        public ICommand openRoomShare
        {
            get
            {
                return new RelayCommand(() =>
                {
                    GroupShareController.ShowShareForm(DisplayRoom.id);
                });
            }
        }

        private List<object> selectedInvites;

        /// <summary>
        /// 选中的备邀请好友
        /// </summary>
        public List<object> SelectedInvites
        {
            get { return selectedInvites; }
            set { selectedInvites = value; RaisePropertyChanged(nameof(SelectedInvites)); }
        }



        /// <summary>
        /// 群消息免打扰
        /// </summary>
        public ICommand DoNotDisturbCommand => new RelayCommand(() =>
        {
            IsOfflineNoPushMsgEnable = false;//禁用按钮
            int offlineNoPushMsg = DisplayRoom.offlineNoPushMsg;
            if (offlineNoPushMsg == 0)
            {
                offlineNoPushMsg = 1;
            }
            else if (offlineNoPushMsg == 1)
            {
                offlineNoPushMsg = 0;
            }
            var client = APIHelper.DoNotDisturbGroupAsync(DisplayRoom.id, Applicate.MyAccount.userId, offlineNoPushMsg.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.offlineNoPushMsg = offlineNoPushMsg;
                        IsNotDisturb = (offlineNoPushMsg == 1) ? (true) : (false);
                        DisplayRoom.UpdateOfflineNoPush(offlineNoPushMsg);
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsOfflineNoPushMsgEnable = true;//允许操作
                }
                else
                {
                    Snackbar.Enqueue("设置失败：" + res.Error.Message);
                }
            };
        });
        /// <summary>
        /// 更改是否公开群
        /// </summary>
        public ICommand checkPublicGroup => new RelayCommand(() =>
        {
            IsPublicGroupEnable = false;
            int isLook = DisplayRoom.isLook;
            if (isLook == 0)
            {
                isLook = 1;
            }
            else if (isLook == 1)
            {
                isLook = 0;
            }

            var client = APIHelper.SetPublicRoomAsync(DisplayRoom.id, isLook.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.isLook = isLook;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsPublicGroupEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });
        /// <summary>
        /// 显示群消息已读人数
        /// </summary>
        public ICommand checkShowRead => new RelayCommand(() =>
        {
            IsShowReadEnable = false;
            int showRead = DisplayRoom.showRead;
            if (showRead == 0)
            {
                showRead = 1;
            }
            else if (showRead == 1)
            {
                showRead = 0;
            }

            var client = APIHelper.SetGroupShowReadAsync(DisplayRoom.id, showRead.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.showRead = showRead;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsShowReadEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });
        /// <summary>
        /// 是否显示群成员
        /// </summary>
        public ICommand UpdateShowMemberCommand => new RelayCommand(() =>
        {
            IsShowMemberEnable = false;
            var client = APIHelper.SetShowMemberAsync(DisplayRoom.id, (DisplayRoom.showMember == 1 ? 0 : 1).ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.showMember = DisplayRoom.showMember == 0 ? 0 : 1;
                        Snackbar.Enqueue("设置成功!" + rtn.resultMsg);
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsShowMemberEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 是否允许发送名片
        /// </summary>
        public ICommand checkAllowSendCard => new RelayCommand(() =>
        {
            IsAllowSendCardEnable = false;
            int allowSendCard = DisplayRoom.allowSendCard;
            if (allowSendCard == 0)
            {
                allowSendCard = 1;
            }
            else if (allowSendCard == 1)
            {
                allowSendCard = 0;
            }

            var client = APIHelper.UpdateAllowSendCardAsync(DisplayRoom.id, allowSendCard.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.allowSendCard = allowSendCard;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsAllowSendCardEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 是否允许普通成员邀请好友
        /// </summary>
        public ICommand checkAllowInviteFriend => new RelayCommand(() =>
        {
            IsAllowInviteFriendEnable = false;
            int allowInviteFriend = DisplayRoom.allowInviteFriend;
            if (allowInviteFriend == 0)
            {
                allowInviteFriend = 1;
            }
            else if (allowInviteFriend == 1)
            {
                allowInviteFriend = 0;
            }

            var client = APIHelper.UpdateAllowInviteFriendAsync(DisplayRoom.id, allowInviteFriend.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.allowInviteFriend = allowInviteFriend;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsAllowInviteFriendEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 是否允许普通成员上传文件
        /// </summary>
        public ICommand checkAllowUploadFile => new RelayCommand(() =>
        {
            IsAllowUploadFileEnable = false;
            int allowUploadFile = DisplayRoom.allowUploadFile;
            if (allowUploadFile == 0)
            {
                allowUploadFile = 1;
            }
            else if (allowUploadFile == 1)
            {
                allowUploadFile = 0;
            }

            var client = APIHelper.UpdateAllowUploadFileAsync(DisplayRoom.id, allowUploadFile.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.allowUploadFile = allowUploadFile;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsAllowUploadFileEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 是否允许普通成员发起会议
        /// </summary>
        public ICommand checkAllowConference => new RelayCommand(() =>
        {
            IsAllowConferenceEnable = false;
            int allowConference = DisplayRoom.allowConference;
            if (allowConference == 0)
            {
                allowConference = 1;
            }
            else if (allowConference == 1)
            {
                allowConference = 0;
            }

            var client = APIHelper.UpdateAllowConferenceAsync(DisplayRoom.id, allowConference.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.allowConference = allowConference;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsAllowConferenceEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 是否允许普通成员发起讲课
        /// </summary>
        public ICommand checkAllowSpeakCourse => new RelayCommand(() =>
        {
            IsAllowSpeakCourseEnable = false;
            int allowSpeakCourse = DisplayRoom.allowSpeakCourse;
            if (allowSpeakCourse == 0)
            {
                allowSpeakCourse = 1;
            }
            else if (allowSpeakCourse == 1)
            {
                allowSpeakCourse = 0;
            }

            var client = APIHelper.UpdateAllowSpeakCourseAsync(DisplayRoom.id, allowSpeakCourse.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.allowSpeakCourse = allowSpeakCourse;
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsAllowSpeakCourseEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 全员禁言
        /// </summary>
        public ICommand TotalMemberBannedCommand => new RelayCommand(() =>
        {
            IsRoomBannedBtnEnable = false;
            long talkTime = DisplayRoom.talkTime;
            if (talkTime == 0)
            {
                talkTime = Helpers.DatetimeToStamp(DateTime.Now) + 1296000;//禁言半个月
            }
            else
            {
                talkTime = 0;
            }
            var client = APIHelper.SetRoomTalkTimeAsync(DisplayRoom.id, DisplayRoom.name, talkTime.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var restxt = Encoding.UTF8.GetString(res.Result);
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(restxt);
                    if (rtn.resultCode == 1)
                    {
                        DisplayRoom.talkTime = talkTime;
                        if (talkTime > Helpers.DatetimeToStamp(DateTime.Now))//允许发言时间大于当前时间时为禁言
                        {
                            IsAllMemberBanned = true;
                        }
                        else
                        {
                            IsAllMemberBanned = false;
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败：" + rtn.resultMsg);
                    }
                    IsRoomBannedBtnEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });

        /// <summary>
        /// 是否开启群验证
        /// </summary>
        public ICommand SetRoomVerifyCommand => new RelayCommand(() =>
        {
            IsNeedVerifyEnable = false;
            var client = APIHelper.SetNeedVerifyAsync(DisplayRoom.id, (DisplayRoom.isNeedVerify == 0 ? "1" : "0"));
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var tmptxt = Encoding.UTF8.GetString(res.Result);
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(tmptxt);
                    if (rtn.resultCode == 1)
                    {
                        //DisplayRoom.isNeedVerify = DisplayRoom.isNeedVerify == 1 ? 0 : 1;
                        Snackbar.Enqueue("群验证已开启" + rtn.resultMsg);
                    }
                    else
                    {
                        Snackbar.Enqueue("群验证开启失败：" + rtn.resultMsg);
                        DisplayRoom.isNeedVerify = DisplayRoom.isNeedVerify == 1 ? 0 : 1;
                    }
                    IsNeedVerifyEnable = true;
                }
                else
                {
                    Snackbar.Enqueue("设置失败:" + res.Error.Message);
                }
            };
        });


        #endregion
        #endregion

        #region Contructor
        public GroupDetialViewModel()
        {
            if (IsInDesignMode == true)
            {
                return;
            }
            InitialGroup();
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgReceipt, (msg) => MsgReceipt(msg));
            Messenger.Default.Register<string>(this, GroupDetialViewModel.InitialGroupDetial, (roomId) => InitialGroupDetail(roomId));
            //Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgRecived, (msg) => MsgRecived(msg));
        }
        #endregion

        #region Methods
        #region 加载成员
        public void SearchMember(string text)
        {
            MembersList.Clear();
            if (text != null && text != "")//查询
            {
                DisplayRoom.members.ToList().FindAll(m => m.nickname.ToLower().Contains(text.ToLower())).ForEach(d =>
                {
                    MembersList.Add(d);
                });
            }
            else//默认
            {
                DisplayRoom.members.ToList().ForEach(d =>
                {
                    MembersList.Add(d);
                }); ;
            }
        }
        #endregion

        #region 更新群名
        /// <summary>
        /// 更新群名
        /// </summary>
        private void UpdateGroupName()
        {
            if (TempGroupName.Trim() == "")
            {
                Snackbar.Enqueue("群名不能为空!");
                return;
            }
            //修改群名
            if (TempGroupName.Length > 0 && DisplayRoom.name != TempGroupName)
            {
                var client = APIHelper.SetRoomNameAsync(DisplayRoom.id, TempGroupName);
                client.UploadDataCompleted += (sen, res) =>
                {
                    if (res.Error == null)
                    {
                        var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                        if (result.resultCode == 1)
                        {
                            IsGroupNameEditing = false;//设置为编辑状态
                            IsEditing = false;//关闭对话框
                            DisplayRoom.name = TempGroupName;//更新UI
                            Snackbar.Enqueue("修改群名成功!");
                            Messenger.Default.Send(new MessageListItem
                            {
                                Jid = displayRoom.jid,
                                ShowTitle = DisplayRoom.name,
                                Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(displayRoom.jid)
                            }, MainViewNotifactions.UpdateAccountName);//通知各界面修改昵称
                            DisplayRoom.UpdateNameByJid(TempGroupName);//更新数据库
                        }
                        else
                        {
                            Snackbar.Enqueue("修改群名失败" + result.resultMsg);
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("修改群资料失败:" + res.Error.Message);
                    }
                };
            }
        }
        #endregion

        #region 更新群描述
        public void UpdateGroupDesc()
        {
            if (TempGroupDesc.Trim() == "")
            {
                Snackbar.Enqueue("群组描述不能为空!");
                return;
            }
            //修改群描述
            if (DisplayRoom.desc == TempGroupDesc)
            {
                IsGroupDescEditing = false;//设置为展示状态
            }
            else
            {
                var client = APIHelper.UpdateGroupChatDescAsync(DisplayRoom.id, TempGroupDesc);
                client.UploadDataCompleted += (s, res) =>
                {
                    if (res.Error == null)
                    {
                        var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                        if (result.resultCode == 1)
                        {
                            IsGroupDescEditing = false;//设置为编辑状态
                            IsEditing = false;
                            DisplayRoom.desc = TempGroupDesc;//更新UI
                            DisplayRoom.UpdateDescByJid(TempGroupDesc);//更新数据库
                            Snackbar.Enqueue("修改群描述成功!");
                        }
                        else
                        {
                            Snackbar.Enqueue("修改群描述失败");
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("修改群描述失败:" + res.Error.Message);
                    }
                };//回调内容
            }
        }
        #endregion

        #region 更新群内昵称
        /// <summary>
        /// 更新群内昵称
        /// </summary>
        public void UpdateNickName()
        {
            if (TempNickName.Trim() == "")
            {
                Snackbar.Enqueue("群内昵称不能为空!");
                return;
            }
            if (MyNickName == TempNickName)
            {
                IsGroupDescEditing = false;//设置为展示状态
            }
            else
            {
                var client = APIHelper.UpdateGroupNickNameAsync(DisplayRoom.id, Applicate.MyAccount.userId, TempNickName);
                client.UploadDataCompleted += (s, res) =>
                {
                    if (res.Error == null)
                    {
                        var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                        if (result.resultCode == 1)
                        {
                            IsNickNameEditing = false;//设置为编辑状态
                            IsEditing = false;
                            MyNickName = TempNickName;//更新UI
                            new DataofMember()
                            {
                                groupid = DisplayRoom.id,
                                userId = Applicate.MyAccount.userId,
                                nickname = TempNickName
                            }.UpdateMemberCall();
                            var tmp = DisplayRoom.members.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId);
                            if (tmp != null)
                            {
                                tmp.nickname = TempNickName;
                            }
                            var priTmp = PreviewsList.FirstOrDefault(p => p.userId == Applicate.MyAccount.userId);
                            if (priTmp != null)
                            {
                                priTmp.nickname = TempNickName;
                            }
                            //RefreshMembersList();
                            Messenger.Default.Send(new MessageListItem { Jid = Applicate.MyAccount.userId, ShowTitle = TempNickName,Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(Applicate.MyAccount.userId) },
                                CommonNotifications.UpdateGroupMemberNickname);//通知修改群内昵称
                            Snackbar.Enqueue("修改内昵称成功!");
                        }
                        else
                        {
                            Snackbar.Enqueue("修改群内昵称失败!");
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("修改群内昵称失败:" + res.Error.Message);
                    }
                };//回调内容
            }
        }
        #endregion

        #region 更新群公告
        public void UpdateNotice()
        {
            if (TempNotice.Trim() == "")
            {
                Snackbar.Enqueue("群公告不能为空！");
                return;
            }
            var client = APIHelper.UpdateGroupChatNoticeAsync(DisplayRoom.id, TempNotice);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (result.resultCode == 1)
                    {
                        IsNoticeEditing = false;//设置为编辑状态
                        DisplayRoom.notices.Add(new Notice()
                        {
                            RoomId = DisplayRoom.id,
                            Text = TempNotice,// 通知文本
                            UserId = int.Parse(Applicate.MyAccount.userId),// 用户Id
                            Nickname = Applicate.MyAccount.nickname,// 用户昵称
                            Time = result.currentTime,// 时间 
                        });
                        DisplayRoom.notice.Text = TempNotice;//更新UI
                        TempNotice = "";
                        Snackbar.Enqueue("发布公告成功!");
                    }
                    else
                    {
                        Snackbar.Enqueue("发布公告失败");
                    }
                }
                else
                {
                    Snackbar.Enqueue("公共发布失败:" + res.Error.Message);
                }
            };
        }
        #endregion

        #region 发送进群验证
        public void SendRoomVerify()
        {
            RoomVerify roomVerify = new RoomVerify();
            if (IsVerifyEditing)//邀请好友
            {
                //邀请的成员
                for (int i = 0; i < selectedInvites.Count; i++)
                {
                    roomVerify.userIds += ((MessageListItem)selectedInvites[i]).Jid + ",";
                    roomVerify.userNames += ((MessageListItem)selectedInvites[i]).MessageTitle + ",";
                }
                roomVerify.userIds = roomVerify.userIds.TrimEnd(',');
                roomVerify.userNames = roomVerify.userNames.TrimEnd(',');
                roomVerify.roomJid = DisplayRoom.jid;
                roomVerify.isInvite = "0";
                roomVerify.reason = TempOthersVerifyDesc;
            }
            else if (IsEditing)//自己申请
            {
                roomVerify.userIds = Applicate.MyAccount.userId;
                roomVerify.userNames = Applicate.MyAccount.nickname;
                roomVerify.roomJid = DisplayRoom.jid;
                roomVerify.isInvite = "1";
                roomVerify.reason = TempMyVerifyDesc;
            }
            IsMyVerifyEditing = false;//设置为编辑状态
            IsEditing = false;
            ShiKuManager.SendRoomVerify(DisplayRoom.userId.ToString(), roomVerify);
            Snackbar.Enqueue("等待群主通过验证");
            IsEditing = false;//打开
            IsVerifyEditing = false;
            IsOthersVerifyEditing = false;
            TempOthersVerifyDesc = "";//赋值临时变量
            PanelIndex = 0;
        }
        #endregion

        #region 初始化群组信息
        /// <summary>
        /// 初始化群组信息
        /// </summary>
        /// <param name="roomId">RoomId</param>
        internal void InitialGroup()
        {
            Snackbar = new SnackbarMessageQueue();
            AdminsList = new ObservableCollection<DataofMember>();
            FriendList = new ObservableCollection<MessageListItem>();
            MembersList = new ObservableCollection<DataofMember>();
            NormalMembersList = new ObservableCollection<DataofMember>();
            PreviewsList = new ObservableCollection<DataofMember>();
            #region 防止显示上次打开群组详情
            App.Current.Dispatcher.Invoke(() =>
            {
                HintOfSearch = new SmartHint();
                groupNameEdit = new PackIcon();
            });
            EditNameVIsible = false;
            EditDescVisible = false;
            SearchVisible = true;
            MemberAddVisible = true;
            IsGroupNameEditing = false;
            IsGroupDescEditing = false;
            IsMyVerifyEditing = false;
            IsOthersVerifyEditing = false;
            TempGroupName = "";
            TempGroupDesc = "";
            TempNickName = "";
            TempMyVerifyDesc = "";
            TempOthersVerifyDesc = "";
            PanelIndex = 0;
            FriendList.Clear();
            AdminsList.Clear();
            PreviewsList.Clear();
            MembersList.Clear();
            TempBannedUser = new DataofMember();
            IsRoomOwner = false;
            #endregion
            //ReloadGroupDetail(roomId);
            //CheckAccess();//显示或隐藏编辑
        }
        #endregion-

        #region 检查当前用户权限
        /// <summary>
        /// 检查当前用户权限
        /// </summary>
        private void CheckAccess()
        {
            //判断当前用户的身份以确定是"退出"还是"解散"群组
            if (DisplayRoom.members != null && DisplayRoom.members.Count > 0)
            {
                var user = DisplayRoom.members.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*查询出自己的身份编号*/);

                if (user != null)
                {
                    IsJoinRoom = true;
                    int access = (int)user.role;
                    switch (access)
                    {
                        case 1:
                            ExitBtnContent = "解散群组";
                            EditNameVIsible = true;
                            EditDescVisible = true;
                            MembersVisible = true;
                            break;
                        case 2:
                            ExitBtnContent = "退出群组";
                            EditNameVIsible = true;
                            EditDescVisible = true;
                            MembersVisible = true;
                            break;
                        case 3:
                            ExitBtnContent = "退出群组";
                            EditNameVIsible = false;
                            EditDescVisible = false;
                            MembersVisible = DisplayRoom.showMember == 1;//0不显示群成员，1显示
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    IsJoinRoom = false;
                    ExitBtnContent = "加入群组";
                    EditNameVIsible = false;
                    EditDescVisible = false;
                }
            }
        }
        #endregion

        #region 重新加载详情
        /// <summary>
        /// 加载群详情
        /// </summary>
        /// <param name="roomId"></param>
        public void InitialGroupDetail(string roomId)
        {
            var tempRoom = new Room().GetByRoomId(roomId);//从数据库获取
            if (tempRoom != null)
            {
                DisplayRoom = tempRoom;
                MembersVisible = DisplayRoom.showMember == 1;//0不显示群成员，1显示
                //判断当前用户的身份以确定是"退出"还是"解散"群组
                if (DisplayRoom.members != null && DisplayRoom.members.Count > 0)
                {
                    var user = DisplayRoom.members.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*查询出自己的身份编号*/);
                    CheckAccess();
                }
                if (DisplayRoom.members == null || DisplayRoom.members.Count == 0)
                {
                    var mem = new DataofMember().GetListByRoomId(DisplayRoom.id);
                    if (mem != null && mem.Count > 0)
                    {
                        DisplayRoom.members = Helpers.ToObservableCollection(mem);
                        MyNickName = DisplayRoom.members.FirstOrDefault(d => d.userId == Applicate.MyAccount.userId).nickname;
                    }
                }
                IsRoomOwner = (DisplayRoom.userId.ToString() == Applicate.MyAccount.userId);//我是不是群主
                SearchMember(SearchText);
                //PriviewMembers();//显示预览页成员
                //LoadFriendListExceptMembersAsync();//加载好友列表
                RoomQRCode = Applicate.QRCodeBase + roomId;
            }
            var client = APIHelper.GetRoomDetialByRoomIdAsync(roomId);
            client.Tag = roomId;
            client.UploadDataCompleted += (sen, e) =>
            {
                if (e.Error == null)
                {
                    var s = Encoding.UTF8.GetString(e.Result);
                    var tmp = JsonConvert.DeserializeObject<JsonRoom>(Encoding.UTF8.GetString(e.Result));
                    DisplayRoom = tmp.data;//从接口获取
                    if (tmp.data.talkTime > Helpers.DatetimeToStamp(DateTime.Now))
                    {
                        IsAllMemberBanned = true;//处于禁言
                    }
                    else
                    {
                        IsAllMemberBanned = false;//未禁言
                    }
                    var tmpMember = tmp.data.members.FirstOrDefault(d => d.userId == Applicate.MyAccount.userId);

                    DisplayRoom.offlineNoPushMsg = tmpMember.offlineNoPushMsg;
                    if (DisplayRoom != null)
                    {
                        CheckAccess();
                        IsRoomOwner = (DisplayRoom.userId.ToString() == Applicate.MyAccount.userId);//我是不是群主
                        MyNickName = DisplayRoom.members.FirstOrDefault(d => d.userId == Applicate.MyAccount.userId).nickname;
                        SearchMember(SearchText);
                        PriviewMembers();//显示预览页成员
                        RoomQRCode = Applicate.QRCodeBase + roomId;
                        //LoadFriendListExceptMembersAsync();//加载好友列表
                    }
                }
                else
                {
                    Snackbar.Enqueue(e.Error.Message);//提示消息
                }
            };
        }
        #endregion

        #region 显示预览主页预览成员
        /// <summary>
        /// 在主页面显示成员
        /// </summary>
        public void PriviewMembers()
        {
            HintOfSearch.Hint = "搜索群成员(共" + DisplayRoom.members.Count + "人)";//显示文本框水印
            PreviewsList.Clear();//先清空集合
            int priviewRange = 4;
            if (DisplayRoom.members.Count <= 5)//如果小于或等于5
            {
                priviewRange = DisplayRoom.members.Count - 1;
            }
            //根据群成员等级排序
            DisplayRoom.members = DisplayRoom.members.OrderBy(m => m.role).ToList().ToObservableCollection();
            if (MembersVisible)
            {
                for (int i = 0; i <= priviewRange; i++)
                {
                    PreviewsList.Add(DisplayRoom.members[i]);//加载前5位成员
                }
            }
            else
            {
                //只显示自己和群主
                DisplayRoom.members.Where(d => d.role == MemberRole.Owner || d.userId == Applicate.MyAccount.userId).ToList().ForEach(d =>
                {
                    PreviewsList.Add(d);
                });
            }

            if (PreviewsList.Count(m => m.role == MemberRole.PlusIcon || m.role == MemberRole.KickIcon) < 1)
            {
                if (DisplayRoom.allowInviteFriend == 1 || EditDescVisible)
                {
                    PreviewsList.Add(new DataofMember { role = MemberRole.PlusIcon });
                }
                if (EditDescVisible)//普通成员不能有减号
                {
                    PreviewsList.Add(new DataofMember { role = MemberRole.KickIcon });
                }
            }
        }
        #endregion

        #region 加载所有好友(排除已在群内的)
        /// <summary>
        /// 加载所有好友(已排除群内成员)
        /// </summary>
        private void LoadFriendListExceptMembersAsync()
        {
            if (DisplayRoom.members != null)
            {
                FriendList.Clear();
                Task.Run(() =>
                {
                    var friends = new DataOfFriends().GetFriendsList();//从数据库获取好友列表
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        for (int i = 0; i < friends.Count; i++)
                        {
                            if (DisplayRoom.members.Count(d => d.userId == friends[i].toUserId) == 0)//排除已经在群里的好友
                            {
                                if (friends[i].toUserId.Length <= 6 || friends[i].toUserId == "10000")
                                {
                                    continue;//系统账号不添加
                                }
                                FriendList.Add(friends[i].ToMsgListItem());
                            }
                        }
                    });
                });
            }
        }
        #endregion

        #region 删除(解散)群聊成功后
        /// <summary>
        /// 删除(解散)群聊成功后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var resstr = Encoding.UTF8.GetString(e.Result);//获取
                var result = JsonConvert.DeserializeObject<JsonBase>(resstr);
                if (result.resultCode == 1)
                {
                    Messenger.Default.Send(DisplayRoom.jid, MainViewNotifactions.MainRemoveGroupItem);//通知主窗口
                    Messenger.Default.Send(true, GroupChatDetial.CloseGroupDetialWindow);//如果成功关闭此窗口
                }
                else
                {
                    Snackbar.Enqueue("删除群聊失败!\n请尝试重新刷新群聊, 进入群聊详情再试");
                }
            }
            else
            {
                Snackbar.Enqueue("删除群聊失败:" + e.Error.Message, "重试", () => { QuitOrDeleteCommand.Execute(null); });
            }
        }
        #endregion

        #region 删除群聊成员
        /// <summary>
        /// 删除群聊成员
        /// </summary>
        private void deleteGroupMember(string jid)
        {
            string userId = "";
            //判断是否删除成功
            var client = APIHelper.DeleteRoomMemberAsync(DisplayRoom.id, userId);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (result.resultCode == 1)
                    {
                        Snackbar.Enqueue("已将 ’" + jid + "‘ 移出群聊");
                        //resetMember();
                    }
                    else
                    {
                        Snackbar.Enqueue("移除 '" + jid + "‘ 失败, 请重试 ");
                    }
                }
                else
                {
                    Snackbar.Enqueue("移除 '" + jid + "‘ 失败" + res.Error.Message);
                }
            };
        }
        #endregion

        #region 更新群组成员
        /// <summary>
        /// 更新群组成员
        /// </summary>
        public void UpdateMemberList()
        {
            var rclient = APIHelper.GetRoomMemberAsync(DisplayRoom.id);
            rclient.UploadDataCompleted += (sen, e) =>
            {
                if (e.Error == null)
                {
                    var members = JsonConvert.DeserializeObject<JsonRoomMemberList>(Encoding.UTF8.GetString(e.Result));
                    DisplayRoom.members = Helpers.ToObservableCollection(members.data);
                    //LoadExceptExistsFriends();//加载群聊列表(邀请好友进群时刷新未进群列表)
                }
                else
                {
                    Snackbar.Enqueue("获取群成员失败:" + e.Error.Message, "重试", () => { UpdateMemberList(); });
                }
            };
        }
        #endregion

        #region 重新判断我的权限
        private bool GetRole(MemberRole role)
        {
            var jsonObj = APIHelper.GetRoomDetialByRoomId(DisplayRoom.id);
            var user = jsonObj.data.members.FirstOrDefault(d => d.userId == Applicate.MyAccount.userId);
            if (user != null)
            {
                return user.role == role;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 更新管理员列表
        private void RefreshAdminsList()
        {
            AdminsList.Clear();
            DisplayRoom.members.ToList().ForEach(d =>
            {
                if (d.role == MemberRole.Admin)
                {
                    AdminsList.Add(d);
                }
            });
        }
        #endregion

        #region 更新群成员列表
        private void RefreshMembersList()
        {
            MembersList.Clear();
            if (MembersVisible || EditDescVisible)
            {
                DisplayRoom.members.ToList().ForEach(d =>
                {
                    MembersList.Add(d);
                });
            }
            else
            {
                //只显示自己和群主
                DisplayRoom.members.Where(d => d.role == MemberRole.Owner || d.userId == Applicate.MyAccount.userId).ToList().ForEach(d =>
                      {
                          MembersList.Add(d);
                      });
            }
        }
        #endregion

        #region 更新普通成员列表(含管理员)
        /// <summary>
        /// 更新普通成员列表(含管理员)
        /// </summary>
        /// <param name="withAdmin"></param>
        private void ReLoadMembersList(bool withAdmin)
        {
            NormalMembersList.Clear();
            DisplayRoom.members.ToList().ForEach(d =>
            {
                if (d.role == MemberRole.Member)
                {
                    NormalMembersList.Add(d);
                }
            });
            if (IsRoomOwner && withAdmin)//如果我群主
            {
                RefreshAdminsList();
                //添加管理员到禁言列表
                AdminsList.ToList().ForEach(d =>
                {
                    NormalMembersList.Insert(0, d);
                });
            }
        }
        #endregion

        #region 更新显示群成员列表

        #endregion

        #region 收到消息回执
        /// <summary>
        /// 收到消息回执
        /// </summary>
        /// <param name="msg"></param>
        public void MsgReceipt(Messageobject msg)
        {
            switch (msg.type)
            {
                case kWCMessageType.RoomExit:
                    break;
                case kWCMessageType.RoomNotice:
                    string content = msg.content;
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

        #region 刷新所有头像
        public void RefreshAllImg(string userId)
        {
            if (DisplayRoom != null && DisplayRoom.members != null)
            {
                DisplayRoom.members.Where(d => d.userId == userId).ToList().ForEach(d => d.userId = userId);
            }
            if (FriendList != null)
            {
                FriendList.Where(d => d.Jid == userId).ToList().ForEach(d => d.Jid = userId);
            }
        }
        #endregion
        #endregion
    }
}
