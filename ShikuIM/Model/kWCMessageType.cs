using Newtonsoft.Json;

namespace ShikuIM.Model
{
    /// <summary>
    /// 信息类型枚举
    /// </summary>
    public enum kWCMessageType
    {
        /// <summary>
        /// 本地消息提示
        /// </summary>
        [JsonIgnore]
        LocalTypeNotice = -1,

        #region 聊天一般信息
        /// <summary>
        /// 不显示的无用类型
        /// </summary>
        kWCMessageTypeNone = 0,

        /// <summary>
        /// 文本
        /// </summary>
        Text = 1,

        /// <summary>
        /// 图片
        /// </summary>
        Image = 2,

        /// <summary>
        /// 语音
        /// </summary>
        Voice = 3,

        /// <summary>
        /// 位置
        /// </summary>
        Location = 4,

        /// <summary>
        /// 动画
        /// </summary>
        Gif = 5,

        /// <summary>
        /// 视频
        /// </summary>
        Video = 6,

        /// <summary>
        /// 音频
        /// </summary>
        Audio = 7,

        /// <summary>
        /// 名片
        /// </summary>
        Card = 8,

        /// <summary>
        /// 文件
        /// </summary>
        File = 9,

        /// <summary>
        /// 提醒 
        /// </summary>
        Remind = 10,

        /// <summary>
        /// 已读标志
        /// </summary>
        IsRead = 26,//已读标志

        /// <summary>
        /// 发红包
        /// </summary>
        RedPacket = 28, //发红包
        #region 聊天特殊消息
        /// <summary>
        /// 单条图文消息
        /// </summary>
        SystemImage1 = 80,
        /// <summary>
        /// 多条图文消息
        /// </summary>
        SystemImage2 = 81,
        /// <summary>
        /// 链接
        /// </summary>
        Link = 82,

        /// <summary>
        /// 戳一戳
        /// </summary>
        PokeMessage = 84,

        #endregion
        #endregion

        /// <summary>
        /// 客户端做的sdk分享进来的链接
        /// </summary>
        SDKLink = 87,

        #region 好友关系消息
        /// <summary>
        /// 打招呼
        /// </summary>
        FriendRequest = 500,

        /// <summary>
        /// 同意加好友
        /// </summary>
        RequestAgree = 501,

        /// <summary>
        /// 拒绝加好友(回话)
        /// </summary>
        RequestRefuse = 502,

        /// <summary>
        /// 新关注
        /// </summary>
        NewSub = 503,

        /// <summary>
        /// 删除关注
        /// </summary>
        DeleteNotice = 504,


        /// <summary>
        /// 彻底删除好友
        /// </summary>
        DeleteFriend = 505,

        /// <summary>
        /// 新推荐好友
        /// </summary>
        Recommand = 506,

        /// <summary>
        /// 加入黑名单
        /// </summary>
        BlackFriend = 507,

        /// <summary>
        /// 直接添加好友
        /// </summary>
        RequestFriendDirectly = 508,

        /// <summary>
        /// 取消黑名单
        /// </summary>
        CancelBlackFriend = 509,

        #region 手机通讯录
        /// <summary>
        /// 对方通过 手机联系人 添加我 直接成为好友
        /// </summary>
        PhoneContactToFriend = 510,
        #endregion


        #region 群文件
        /// <summary>
        /// 上传群文件
        /// </summary>
        RoomFileUpload = 401,

        /// <summary>
        /// 删除群文件
        /// </summary>
        RoomFileDelete = 402,

        /// <summary>
        /// 下载群文件
        /// </summary>
        RoomFileDownload = 403,
        #endregion


        /// <summary>
        /// 我之前上传给服务端的联系人表内有人注册了，更新 手机联系人
        /// </summary>
        NewContactRegistered = 511,
        #endregion

        #region 输入时信息

        /// <summary>
        /// 多点登录时，此枚举用于标识对应的设备平台是否在线，在线为1，离线为0
        /// </summary>
        OnlineStatus = 200,

        /// <summary>
        /// 正在输入
        /// </summary>
        Typing = 201,

        /// <summary>
        /// 消息撤回
        /// </summary>
        Withdraw = 202,
        #endregion

        #region 聘吧(无用)
        /// <summary>
        /// 企业发布的职位信息
        /// </summary>
        kWCMessageTypeEnterpriseJob = 11, //企业发布的职位信息
        /// <summary>
        /// 个人发布的职位信息
        /// </summary>
        kWCMessageTypePersonJob = 31, //个人发布的职位信息
        /// <summary>
        /// 简历信息
        /// </summary>
        kWCMessageTypeResume = 12, //简历信息
        /// <summary>
        /// 问交换手机
        /// </summary>
        kWCMessageTypePhoneAsk = 13, //问交换手机
        /// <summary>
        /// 答交换手机
        /// </summary>
        kWCMessageTypePhoneAnswer = 14, //答交换手机
        /// <summary>
        /// 问发送简历
        /// </summary>
        kWCMessageTypeResumeAsk = 16, //问发送简历
        /// <summary>
        /// 答发送简历
        /// </summary>
        kWCMessageTypeResumeAnswer = 17, //答发送简历
        /// <summary>
        /// 发起笔试题（暂无用）
        /// </summary>
        kWCMessageTypeExamSend = 19, //发起笔试题（暂无用）
        /// <summary>
        /// 接受笔试题（暂无用）
        /// </summary>
        kWCMessageTypeExamAccept = 20, //接受笔试题（暂无用）
        /// <summary>
        /// 做完笔试题，显示结果（暂无用）
        /// </summary>
        kWCMessageTypeExamEnd = 21, //做完笔试题，显示结果（暂无用）
        #endregion

        #region 音视频

        AudioChatAsk = 100, //发起语音通话
        AudioChatAccept = 102, //接听语音通话
        AudioChatCancel = 103, //拒绝语音通话 || 对方不响应30秒
        AudioChatEnd = 104, //结束语音通话 

        VideoChatAsk = 110, //发起视频通话
        VideoChatAccept = 112, //接听视频通话
        VideoChatCancel = 113, //拒绝视频通话 || 对方未响应30秒
        VideoChatEnd = 114, //结束视频通话 

        VideoMeetingInvite = 115, //邀请加入视频会议
        AudioMeetingInvite = 120, //语音会议邀请

        PhoneCalling = 123, //通话中
        AudioMeetingSetSpeaker = 124, //忙线中

        #region 暂未使用
        VideoMeetingJoin = 116, //加入视频会议
        VideoMeetingQuit = 117, //退出视频会议
        VideoMeetingKick = 118, //踢出视频会议
        AudioMeetingJoin = 121, //加入语音会议
        AudioMeetingQuit = 122, //退出语音会议
        AudioChatReady = 101, //确定可以接听语音通话
        VideoChatReady = 111, //确定可以接听视频通话
        AudioMeetingAllSpeaker = 125, //踢出语音会议 
        #endregion

        #region 朋友圈
        WeiboPraise = 301,//朋友圈点赞
        WeiboComment = 302,//朋友圈评论
        #endregion

        #endregion

        #region 群组

        /// <summary>
        /// //群内昵称改变
        /// </summary>
        RoomMemberNameChange = 901,

        /// <summary>
        /// 修改房间名称
        /// </summary>
        RoomNameChange = 902,

        /// <summary>
        /// 解散群聊
        /// </summary>
        RoomDismiss = 903,

        /// <summary>
        /// 删除成员或退出房间
        /// </summary>
        RoomExit = 904,

        /// <summary>
        /// 新公告
        /// <para>
        /// 特殊字段
        /// content，fromUserId（房主），fromUserName，objectId(房间Jid)，timeSend
        ///</para>
        /// </summary>
        RoomNotice = 905,

        /// <summary>
        /// 群组禁言
        /// <para>
        /// 特殊字段：
        /// content（禁言截止时间，单位：秒）
        /// fromUserName（房主）
        /// objectId(房间Jid)
        /// toUserName（被禁言者）
        /// </para>
        /// </summary>
        RoomMemberBan = 906,

        /// <summary>
        /// 进入群聊
        /// <para>
        /// 特殊字段：
        /// fromUserId(邀请者) 
        /// objectId(房间Jid)
        /// toUserId(被邀请者)
        /// FileSize(是否显示阅读人数，1:开启，0:关闭)
        /// content(房间名称）
        /// </para>
        /// </summary>
        RoomInvite = 907,

        /// <summary>
        /// 群管理员
        /// </summary>
        RoomAdmin = 913,

        /// <summary>
        /// 显示阅读人数
        /// </summary>
        RoomReadVisiblity = 915,

        /// <summary>
        /// 群组是否需要验证
        /// </summary>
        RoomIsVerify = 916,

        /// <summary>
        /// 是否为公开群组
        /// </summary>
        RoomIsPublic = 917,

        /// <summary>
        /// 是否查看群成员
        /// </summary>
        RoomInsideVisiblity = 918,

        /// <summary>
        /// 群内是否允许发送名片
        /// </summary>
        RoomUserRecommend = 919,

        /// <summary>
        /// 群组全员禁言   禁言截止时间
        /// </summary>
        RoomAllBanned = 920,

        /// <summary>
        /// 是否允许群内普通成员邀请  1允许  0不允许去
        /// </summary>
        RoomAllowMemberInvite = 921,

        /// <summary>
        /// 允许成员上传群共享文件  1允许   0不允许
        /// </summary>
        RoomAllowUploadFile = 922,

        /// <summary>
        /// 是否允许群会议  1允许  0不允许
        /// </summary>
        RoomAllowConference = 923,

        /// <summary>
        /// 群组允许成员开启讲课  1允许   0不允许
        /// </summary>
        RoomAllowSpeakCourse = 924,

        /// <summary>
        /// 群管理员转让
        /// </summary>
        RoomManagerTransfer = 925,
        #endregion

    }

}
