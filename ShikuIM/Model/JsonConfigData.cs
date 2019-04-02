namespace ShikuIM.Model
{
    #region 配置
    public class ConfigData
    {
        /// <summary>
        /// 
        /// </summary>
        public string XMPPDomain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string XMPPHost { get; set; }

        /// <summary>
        /// Xmpp超时时间
        /// </summary>
        public int XMPPTimeout { get; set; }

        /// <summary>
        /// Xmpp心跳包间隔
        /// </summary>
        public int xmppPingTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string androidAppUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string androidExplain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int androidVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string apiUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string apiUrlOfKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string audioLen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int displayRedPacket { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int distance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string downloadAvatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string downloadUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string freeswitch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string helpUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string iosAppUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string iosExplain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int iosVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liveUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string meetingHost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string shareUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string softUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uploadUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string videoLen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string xMPPDomain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string xMPPHost { get; set; }

        /// <summary>
        /// Jisti服务器
        /// </summary>
        public string jitsiServer { get; set; }
    }
    #endregion

    /// <summary>
    /// 配置信息
    /// </summary>
    public class JsonConfigData : JsonBase
    {
        #region 配置文件
        public JsonConfigData()
        {
            data = new ConfigData();
        }
        #endregion

        /// <summary>
        /// 配置
        /// </summary>
        public ConfigData data { get; set; }
    }
}
