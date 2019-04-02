using System.ComponentModel.DataAnnotations;

namespace ShikuIM.Model
{

    #region Rtnuser
    /// <summary>
    /// 用于存储http请求登录后服务器返回的数用户数据
    /// </summary>
    public class Jsonuser : JsonBase
    {
        #region 构造函数
        public Jsonuser()
        {
            data = new DataOfUser();
        }
        #endregion

        /// <summary>
        /// data数据
        /// </summary>
        public DataOfUser data { get; set; }
    }
    #endregion

    #region dataOfUser
    /// <summary>
    /// 用户
    /// </summary>
    public class DataOfUser
    {
        #region 初始化登录信息
        public DataOfUser()
        {
            login = new LoginInfo();
        }
        #endregion

        [Key]
        public string id { get; set; }
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public long companyId { get; set; }

        /// <summary>
        /// 我的电话
        /// </summary>
        public string telephone { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long expires_in { get; set; }

        /// <summary>
        /// 好友个数
        /// </summary>
        public int friendCount { get; set; }

        /// <summary>
        /// 登录
        /// </summary>
        public LoginInfo login { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 返回结果代码
        /// </summary>
        public int resultCode { get; set; }

        /// <summary>
        /// 返回密码
        /// </summary>
        public string passWord { get; set; }

    }
    #endregion

    #region 登录信息
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfo
    {

        /// <summary>
        /// 是否首次登录
        /// </summary>
        public int isFirstLogin { get; set; }

        /// <summary>
        /// 是否应该同步当前好友列表(该值表示好友列表是否在其他端有变化)
        /// </summary>
        public int isupdate { get; set; }

        /// <summary>
        /// laitude
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// longitude
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public long loginTime { get; set; }

        /// <summary>
        /// 离线时间
        /// </summary>
        public long offlineTime { get; set; }

    }
    #endregion

}
