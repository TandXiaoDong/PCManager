using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using Newtonsoft.Json;
using ShikuIM.Model;
using ShikuIM.View;
using ShikuIM.ViewModel;

namespace ShikuIM.UserControls.Message
{
    /// <summary>
    /// JXRoomVerify.xaml 的交互逻辑
    /// </summary>
    public partial class JXRoomVerify : UserControl
    {
        Messageobject message;
        public JXRoomVerify(bool isgroup, Messageobject msg)
        {
            InitializeComponent();
            this.message = msg;
            this.DataContext = this;//指定绑定的对象
            if (msg.type == kWCMessageType.RoomIsVerify)
            {
                var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
                Regex reg = new Regex("^[A-F0-9]{8}([A-F0-9]{4}){3}[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                if (reg.IsMatch(msg.objectId))
                {
                    return;
                }
                var roomVerify = JsonConvert.DeserializeObject<RoomVerify>(msg.objectId);
                if (roomVerify.isInvite == "0")//邀请进群
                    tb_prompt.Text = string.Format("{0} 邀请 {1} 位朋友加入群聊（{2}）", msg.fromUserName, roomVerify.userIds.Split(',').Length, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
                else
                    tb_prompt.Text = string.Format("{0} 申请加入群聊（{1}）", msg.fromUserName, Helpers.StampToDatetime(msg.timeSend).ToString("MM-dd HH:mm:ss"));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            message = new Messageobject()
            {
                messageId = message.messageId,
                fromUserId = message.fromUserId,
                FromId=message.FromId,
                toUserId = message.toUserId,
                ToId=message.ToId,
            }.GetModel();
            if (message!=null)
                new RoomVerifyForm(message).ShowDialog();
        }
    }
}
