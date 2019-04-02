using ShikuIM.ViewModel;

namespace ShikuIM
{


    #region 登录注册页面通知Token
    /// <summary>
    /// 登录注册页面通知Token
    /// </summary>
    public static class LoginNotifications
    {
        /// <summary>
        /// 登录页面初始化用户(记住密码使用)
        /// </summary>
        public static string InitialAccount { get; }

        static LoginNotifications()
        {
            InitialAccount = nameof(InitialAccount);
        }
    }
    #endregion

    #region 主窗口通知Token
    /// <summary>
    /// 主窗口通知Token
    /// </summary>
    public static class MainViewNotifactions
    {

        /// <summary>
        /// 主页面添加最近消息项(一般用于<see cref="MainViewModel"/>中RecentList集合的添加)
        /// </summary>
        public static string MainAddRecentItem { get; }

        /// <summary>
        /// 设置主窗口选中的最近消息项
        /// </summary>
        public static string MainChangeRecentListIndex { get; }

        /// <summary>
        /// 更新主窗口对应UserId的名称(或备注)
        /// </summary>
        public static string UpdateAccountName { get; }

        /// <summary>
        /// 插入RecentMessageList
        /// </summary>
        public static string MainInsertRecentItem { get; }

        /// <summary>
        /// 主窗口添加好友项目
        /// </summary>
        public static string MainAddFriendListItem { get; }

        /// <summary>
        /// 控制主窗口页面
        /// </summary>
        public static string MainGoToPage { get; }

        /// <summary>
        /// 主窗口加载好友列表(True为本地加载,False为调用接口加载)
        /// </summary>
        public static string MainViewLoadFriendList { get; }

        /// <summary>
        /// 主窗口加载群组列表(True为本地加载,False为调用接口加载)
        /// </summary>
        public static string MainViewLoadGroupList { get; }

        /// <summary>
        /// 主窗口加载黑名单列表(True为本地加载,False为调用接口加载)
        /// </summary>
        public static string MainViewLoadBlockList { get; }

        /// <summary>
        /// 转发消息
        /// </summary>
        public static string ForwardMessage { get; }

        /// <summary>
        /// 撤回消息
        /// </summary>
        public static string WithDrawMessage { get; }

        /// <summary>
        ///  主窗口提示消息通知Token
        /// </summary>
        public static string SnacbarMessage { get; }

        /// <summary>
        /// 创建或更新消息列表项通知Token
        /// </summary>
        public static string CreateOrUpdateRecentItem { get; }

        /// <summary>
        /// 主窗口更新最近消息项通知Token
        /// </summary>
        public static string UpdateRecentItemContent { get; }

        /// <summary>
        /// 更新我的账号详情
        /// </summary>
        public static string UpdateMyAccount { get; }

        /// <summary>
        /// Xmpp连接状态改变时
        /// </summary>
        public static string XmppConnectionStateChanged { get; }

        /// <summary>
        /// 聊天输入框内容改变
        /// </summary>
        public static string InputTextChanged { get; }

        /// <summary>
        /// 取消黑名单
        /// </summary>
        public static string CancelBlockItem { get; }

        /// <summary>
        /// 删除单个群组
        /// </summary>
        public static string MainRemoveGroupItem { get; }

        /// <summary>
        /// 闪烁任务栏
        /// </summary>
        public static string FlashWindow { get; }

        #region Contructor
        static MainViewNotifactions()
        {
            MainAddRecentItem = nameof(MainAddRecentItem);
            MainInsertRecentItem = nameof(MainInsertRecentItem);
            MainChangeRecentListIndex = nameof(MainChangeRecentListIndex);
            UpdateAccountName = nameof(UpdateAccountName);
            MainAddFriendListItem = nameof(MainAddFriendListItem);
            MainGoToPage = nameof(MainGoToPage);
            MainViewLoadGroupList = nameof(MainViewLoadGroupList);
            MainViewLoadBlockList = nameof(MainViewLoadBlockList);
            ForwardMessage = nameof(ForwardMessage);
            WithDrawMessage = nameof(WithDrawMessage);
            SnacbarMessage = nameof(SnacbarMessage);
            CreateOrUpdateRecentItem = nameof(CreateOrUpdateRecentItem);
            UpdateRecentItemContent = nameof(UpdateRecentItemContent);
            XmppConnectionStateChanged = nameof(XmppConnectionStateChanged);
            InputTextChanged = nameof(InputTextChanged);
            CancelBlockItem = nameof(CancelBlockItem);
            MainRemoveGroupItem = nameof(MainRemoveGroupItem);
            FlashWindow = nameof(FlashWindow);
        }
        #endregion

    }
    #endregion


    #region 通用通知Token
    /// <summary>
    /// 通用通知Token
    /// </summary>
    public static class CommonNotifications
    {
        /// <summary>
        /// 收到Xmpp消息时
        /// </summary>
        public static string XmppMsgRecived { get; }

        /// <summary>
        /// 收到Xmpp消息回执时
        /// </summary>
        public static string XmppMsgReceipt { get; }


        /// <summary>
        /// 更新群组成员昵称
        /// </summary>
        public static string UpdateGroupMemberNickname { get; }

        /// <summary>
        /// 更新我的详情
        /// </summary>
        public static string UpdateMyAccountDetail { get; }

        /// <summary>
        /// 群聊创建成功
        /// </summary>
        public static string CreateNewGroupFinished { get; }

        /// <summary>
        /// 更新群聊成员人数
        /// </summary>
        public static string AddGroupMemberSize { get; }

        /// <summary>
        /// 移除群组成员
        /// 参数为Dictionary<string，List<DataofMember>>键为Jid
        /// </summary>
        public static string RemoveGroupMember { get; }

        /// <summary>
        /// 音频聊天请求
        /// </summary>
        public static string AudioChatRequest { get; }

        /// <summary>
        /// 视频聊天请求
        /// </summary>
        public static string VideoChatRequest { get; }


        /// <summary>
        /// 收到Xmpp消息时
        /// </summary>
        //public static string XmppMsgRecived { get; }

        #region Contructor
        static CommonNotifications()
        {
            XmppMsgRecived = nameof(XmppMsgRecived);
            XmppMsgReceipt = nameof(XmppMsgReceipt);
            UpdateGroupMemberNickname = nameof(UpdateGroupMemberNickname);
            UpdateMyAccountDetail = nameof(UpdateMyAccountDetail);
            CreateNewGroupFinished = nameof(CreateNewGroupFinished);
            AddGroupMemberSize = nameof(AddGroupMemberSize);
            RemoveGroupMember = nameof(RemoveGroupMember);
            AudioChatRequest = nameof(AudioChatRequest);
            VideoChatRequest = nameof(VideoChatRequest);
            //XmppMsgRecived = nameof(XmppMsgRecived);
        }
        #endregion

    }
    #endregion

    #region 消息气泡列表通知Token
    /// <summary>
    /// 消息气泡列表通知Token
    /// </summary>
    public static class ChatBubblesNotifications
    {
        /// <summary>
        /// 批量显示消息气泡(一般用于<see cref="ChatBubbleListViewModel"/>批量显示消息气泡)
        /// </summary>
        public static string ShowBubbleList { get; }

        /// <summary>
        /// 批量显示消息气泡(一般用于<see cref="ChatBubbleListViewModel"/>单个显示消息气泡)
        /// </summary>
        public static string ShowSingleBubble { get; }

        /// <summary>
        /// 气泡撤回消息通知Token
        /// </summary>
        public static string WithDrawSingleMessage { get; }

        /// <summary>
        /// 气泡删除消息通知Token
        /// </summary>
        public static string DeleteMessage { get; }

        //可以在下面添加自己以后还要用到的消息

        /// <summary>
        /// 点击消息通知到气泡列
        /// </summary>
        public static string SetMessageInfo { get; }

        #region Initial Proprities
        static ChatBubblesNotifications()
        {
            ShowBubbleList = nameof(ShowBubbleList);
            ShowSingleBubble = nameof(ShowSingleBubble);
            WithDrawSingleMessage = nameof(WithDrawSingleMessage);
            DeleteMessage = nameof(DeleteMessage);
            SetMessageInfo = nameof(SetMessageInfo);
        }
        #endregion

    }
    #endregion

    #region 好友详情通知Token
    /// <summary>
    /// 好友详情通知Token
    /// </summary>
    public static class UserDetailNotifications
    {
        /// <summary>
        /// 显示用户详情,用于通知<see cref="UserDetailViewModel"/>中
        /// </summary>
        public static string ShowUserDetial { get; }

        /// <summary>
        /// 显示自己详情
        /// </summary>
        public static string ShowMyDetial { get; }

        /// <summary>
        /// 更新用户详情，用于通知<see cref="UserDetailViewModel"/>中
        /// </summary>
        public static string UpdateUserDetail { get; }


        #region Contructor
        /// <summary>
        /// 构造函数
        /// </summary>
        static UserDetailNotifications()
        {
            ShowUserDetial = nameof(ShowUserDetial);
            UpdateUserDetail = nameof(UpdateUserDetail);
            ShowMyDetial = nameof(ShowMyDetial);
        }
        #endregion
    }
    #endregion

    #region 好友验证列表通知Token
    /// <summary>
    /// 好友验证列表通知Token
    /// </summary>
    public static class VerifyFriendLIstToken
    {
        /// <summary>
        /// 显示用户详情,用于通知<see cref="UserVerifyListViewModel"/>中
        /// </summary>
        public static string DeleteVerifyItem { get; }

        #region Contructor
        static VerifyFriendLIstToken()
        {
            DeleteVerifyItem = nameof(DeleteVerifyItem);
        }
        #endregion

    }
    #endregion


}
