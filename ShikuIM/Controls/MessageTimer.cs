using ShikuIM.Model;
using System.Timers;


namespace ShikuIM.UserControls
{
    /// <summary>
    /// 重发消息专用Timer
    /// </summary>
    internal class MessageTimer : Timer
    {
        #region 无参构造函数
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MessageTimer() : base()
        {
            this.Interval = 2000;//默认为20秒发送时间
            ReSendCount = 0;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 可以设置Interval的构造函数
        /// </summary>
        internal MessageTimer(double interval) : base(interval)
        {
            ReSendCount = 0;
        }
        #endregion

        /// <summary>
        /// Xmpp消息
        /// </summary>
        internal Messageobject TmpMsg { get; set; }

        /// <summary>
        /// 对应的MessageId
        /// </summary>
        internal string MessageId { get; set; } = "";

        /// <summary>
        /// 重发次数
        /// </summary>
        internal int ReSendCount { get; set; }

    }
}
