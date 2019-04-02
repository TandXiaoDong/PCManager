using System;

namespace ShikuIM
{
    /// <summary>
    /// 接口列表
    /// </summary>
    public enum HttpUrlType
    {
        /// <summary>
        /// 服务器配置Url
        /// </summary>
        ServerConfig,

        /// <summary>
        /// 用户头像
        /// </summary>
        UserAvator,

        /// <summary>
        /// 用户登录
        /// </summary>
        UserLogin,




    }




    /// <summary>
    /// 接口Url获取
    /// </summary>
    public static class UrlManager
    {

        #region 获取头像HttpUrl
        /// <summary>
        /// 获取头像Http路径
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="isThumbnail">是否为缩略图</param>
        /// <returns></returns>
        public static string GetUrl(string userId, bool isThumbnail)
        {
            return Applicate.URLDATA.data.downloadAvatarUrl + "avatar/" + ((isThumbnail) ? ("t") : ("o"))
               + "/" + Convert.ToInt32(userId) % 10000 + "/" + userId + ".jpg";
        }
        #endregion







    }


}
