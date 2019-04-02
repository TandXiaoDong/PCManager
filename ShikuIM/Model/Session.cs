using GalaSoft.MvvmLight;

namespace ShikuIM.Model
{
    public class Session : ObservableObject
    {

        private string _jid = "";
        private string nickname = "";
        private string isOnlineText;
        public string roomId;
        private string remarkName = "";

        /// <summary>
        /// 会话中的UserId
        /// </summary>
        public string Jid
        {
            get
            { return _jid; }
            set
            {
                if (value == null)
                {
                    _jid = "";
                }
                else
                {
                    _jid = value;
                }
                RaisePropertyChanged(nameof(Jid));
            }
        }


        /// <summary>
        /// 会话中的昵称
        /// </summary>
        public string NickName
        {
            get
            { return nickname; }
            set
            {
                nickname = value;
                RaisePropertyChanged(nameof(NickName));
            }
        }

        /// <summary>
        /// 会话中的备注 (如果备注为空则显示昵称)
        /// </summary>
        public string RemarkName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(remarkName))//如果为空显示真实昵称
                {
                    return nickname;
                }
                else//如果不为空显示备注
                {
                    return remarkName;
                }
            }
            set
            {
                //会话标题过滤空字符(可能会有坑)
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                remarkName = value;
                RaisePropertyChanged(nameof(RemarkName));
            }
        }

        /// <summary>
        /// 对方是否在线 (0.不在线  1.在线  2.群组)
        /// </summary>
        public string IsOnlineText
        {
            get
            {
                return isOnlineText;
            }
            set
            {
                isOnlineText = value;
                RaisePropertyChanged(nameof(IsOnlineText));
            }
        }


        /// <summary>
        /// RoomId
        /// </summary>
        public string RoomId
        {
            get { return roomId; }
            set
            {
                roomId = value;
                RaisePropertyChanged(nameof(RoomId));
            }
        }


        /// <summary>
        /// 当前会话是否为群聊
        /// </summary>
        public bool isGroup
        {
            get
            {
                return (this.Jid.Length > 15);
            }
        }

        /// <summary>
        /// 在当前选中群中的群昵称
        /// </summary>
        public string MyMemberNickname { get; set; }

    }
}


