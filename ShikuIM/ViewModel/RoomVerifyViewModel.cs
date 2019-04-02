using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{
    public class RoomVerifyViewModel : ViewModelBase
    {
        DataOfUserDetial friend;
        ObservableCollection<DataofMember> membersList;
        string inviteText;
        string reason;
        Room room;
        string btnContent;
        bool btnIsEnable;
        Messageobject message;
        /// <summary>
        /// 好友详情
        /// </summary>
        public DataOfUserDetial Friend
        {
            get { return friend; }
            set { friend = value; RaisePropertyChanged(nameof(Friend)); }
        }
        /// <summary>
        /// 邀请成员
        /// </summary>
        public ObservableCollection<DataofMember> MembersList
        {
            get { return membersList; }
            set
            {
                membersList = value;
                RaisePropertyChanged(nameof(MembersList));
            }
        }
        /// <summary>
        /// 验证消息
        /// </summary>
        public string Reason
        {
            get { return reason; }
            set
            {
                reason = value;
                RaisePropertyChanged(nameof(Reason));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string InviteText
        {
            get { return inviteText; }
            set
            {
                inviteText = value;
                RaisePropertyChanged(nameof(InviteText));
            }
        }
        /// <summary>
        /// 按钮内容
        /// </summary>
        public string BtnContent
        {
            get { return btnContent; }
            set
            {
                btnContent = value;
                RaisePropertyChanged(nameof(BtnContent));
            }
        }
        /// <summary>
        /// 按钮可用
        /// </summary>
        public bool BtnIsEnable
        {
            get { return btnIsEnable; }
            set
            {
                btnIsEnable = value;
                RaisePropertyChanged(nameof(BtnIsEnable));
            }
        }

        #region Contructor
        public RoomVerifyViewModel()
        {
        }
        #endregion

        #region 初始化
        public void Initial(Messageobject msg = null)
        {
            if (msg == null)
            {
                return;
            }

            this.message = msg;
            membersList = new ObservableCollection<DataofMember>();
            Friend = new DataOfUserDetial() { userId = msg.fromUserId, nickname = msg.fromUserName };
            var roomVerify = JsonConvert.DeserializeObject<RoomVerify>(msg.objectId);
            room = new Room() { jid = roomVerify.roomJid }.GetByJid();
            string[] userIds = roomVerify.userIds.Split(',');
            string[] nicknames = roomVerify.userNames.Split(',');
            if (roomVerify.isInvite == "0")//邀请进群
            {
                InviteText = String.Format("{0} 邀请 {1} 位朋友加入群聊", msg.fromUserName, userIds.Length);

            }
            else
            {
                InviteText = String.Format("{0} 申请加入群聊", msg.fromUserName);
            }
            Reason = roomVerify.reason;
            if (userIds.Length > 0)
            {
                for (int i = 0; i < userIds.Length; i++)
                {
                    string nk = "";
                    if (i < nicknames.Length)
                    {
                        nk = nicknames[i];
                    }

                    membersList.Add(new DataofMember() { userId = userIds[i], nickname = nk });
                }
            }
            if (msg.isDownload)
            {
                //已确认
                BtnIsEnable = false;
                BtnContent = "已确认";
            }
            else
            {
                BtnIsEnable = true;
                BtnContent = "确认邀请";
            }
        }
        #endregion

        /// <summary>
        /// 确认邀请
        /// </summary>
        public ICommand AgreeInvitation => new RelayCommand(() =>
        {
            if (room != null)
            {
                var memList = membersList.ToList().ConvertAll(d => d.userId);
                var client = APIHelper.UpdateCreateGroupAsync(room.id, memList);
                client.UploadDataCompleted += (sen, res) =>
                {
                    var rtn = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.resultCode == 1)
                    {
                        message.isDownload = true;
                        message.Update();
                        BtnIsEnable = false;
                        BtnContent = "已确认";
                    }
                };
            }
        });


    }
}
