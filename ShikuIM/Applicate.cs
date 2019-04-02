using ShikuIM.Model;
using ShikuIM.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShikuIM
{
    /// <summary>
    /// 此应用程序存储的全局信息
    /// </summary>
    internal static class Applicate
    {
        #region Private Member
        private static JsonConfigData _URLDATA;
        private static string access_Token;
        private static DataOfUserDetial myAccount;
        private static MemoryStream tmpStream;
        private static SQLiteDBContext _dbContext;
        #endregion

        #region Public & Internal Member

        // <summary>
        // Json解析设置
        // </summary>
        //public static JsonSerializerSettings settings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// 设备名称列表
        /// </summary>
        public static string[] PlatformNames = new string[] { "android", "web", "ios", "mac" };

        /// <summary>
        /// 设备列表
        /// </summary>
        public static List<Platform> PlatForms { get; set; } = new List<Platform>();

        /// <summary>
        /// 临时存放流变量(需注意线程安全)
        /// </summary>
        public static MemoryStream TempMemeryStream
        {
            get
            {
                if (tmpStream == null)
                {
                    tmpStream = new MemoryStream();
                }
                return tmpStream;
            }
            set { tmpStream = value; }
        }


        internal static List<MessageTimer> _reSendList { get; set; } = new List<MessageTimer>();

        /// <summary>
        /// 账号数据库
        /// </summary>
        public static SQLiteDBContext AccountDbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = new SQLiteDBContext();
                }

                return _dbContext;
            }
            set { _dbContext = value; }
        }

        /// <summary>
        /// 系统数据库
        /// </summary>
        public static SystemDBContext SystemDbContext { get; set; }

        /// <summary>
        /// 常量数据库
        /// </summary>
        public static ConstantDBContext ConstantDBContext { get; set; }

        /// <summary>
        /// URL对象
        /// </summary>
        public static JsonConfigData URLDATA
        {
            get
            {
                if (_URLDATA != null)
                {
                    return _URLDATA;
                }
                else
                {
                    APIHelper.GetFullConfig();
                }

                return _URLDATA;
            }
            set
            {
                _URLDATA = value;
            }
        }

        /// <summary>
        /// 需重发的消息列表
        /// </summary>
        public static List<MessageTimer> SendingList { get { return _reSendList; } set { _reSendList = value; } }

        /// <summary>
        /// 撤回列表
        /// </summary>
        public static List<Messageobject> WithDrawMessageList = new List<Messageobject>();

        /// <summary>
        /// 应用程序接口访问令牌
        /// </summary>
        public static string Access_Token
        {
            get { return access_Token; }
            set { access_Token = value; }
        }

        /// <summary>
        /// 当前的用户
        /// </summary>
        public static DataOfUserDetial MyAccount
        {
            get
            {
                if (myAccount == null)
                {
                    myAccount = new DataOfUserDetial();
                }
                return myAccount;
            }
            set
            {
                myAccount = value;
            }
        }

        /// <summary>
        /// 用户是否通过密码验证
        /// <para>True时Xmpp断线会重连</para> 
        /// false时Xmpp不会重连
        /// </summary>
        public static bool IsAccountVerified { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public static long LoginTime { get; set; }

        /// <summary>
        /// 本地配置数据(包括下载路径,头像地址)
        /// </summary>
        public static LocalConfig LocalConfigData { get; set; } =
            new LocalConfig
            {
                CatchPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\ShikuIM" + "\\.temp",
                ChatDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\ShikuIM" + "\\",
                ChatPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\ShikuIM\\" + "Chat" + "\\",
                TempFilepath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\ShikuIM" + "\\Temp" + "\\",
                UserAvatorFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\ShikuIM" + "\\" + "avator" + "\\",
                MessageDatabasePath = Environment.CurrentDirectory + "\\db\\" + Applicate.MyAccount.userId + ".db"
            };

        /// <summary>
        /// 二维码基础Url
        /// </summary>
        public static string QRCodeBase { get; }
            = "http://shiku.co/im-download.html?action=group&amp;shikuId=";

        /// <summary>
        /// 百度地图Url
        /// </summary>
        public static string MapApiUrl { get; } = "http://api.map.baidu.com/marker?";

        /// <summary>
        /// 转发列表
        /// </summary>
        public static List<Messageobject> ForwardMessageList { get; internal set; } = new List<Messageobject>();



        #endregion


        #region Function

        /// <summary>
        /// 根据Jid获取昵称
        /// </summary>
        /// <param name="jid"></param>
        /// <param name="memberCountVisiblity"></param>
        /// <returns></returns>
        public static string GetNicknameByJid(string jid, bool memberCountVisiblity = true)
        {
            string displayName = "";
            if (jid.Length == 5)//系统类型账号
            {
                return GetSystemNicknameByJid(jid);//获取系统账号昵称
            }
            else if (jid.Length >= 15)//群组类型账号
            {
                var room = new Room() { jid = jid }.GetByJid();
                if (room != null)
                {
                    displayName = room.name + "(共" + room.userSize + "人)";
                }
            }
            else//普通用户账号
            {
                var user = new DataOfFriends() { toUserId = jid }.GetByUserId();
                if (user != null)
                {
                    displayName = string.IsNullOrWhiteSpace(user.remarkName) ? user.toNickname : user.remarkName;
                }
            }
            return displayName;//
        }


        /// <summary>
        /// 根据Jid获取
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public static string GetSystemNicknameByJid(string jid)
        {
            switch (jid)
            {
                case "10000":
                    return "客服公众号";
                case "10001":
                    return "新的好友";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <typeparam name="T">窗口</typeparam>
        /// <returns></returns>
        public static T GetWindow<T>()
        {
            try
            {
                T window = default(T);
                foreach (var item in Application.Current.Windows)
                {
                    if (item is T)
                    {
                        window = (T)item;
                    }
                }
                return window;
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
                return default(T);
            }
        }


        #endregion


    }
}
