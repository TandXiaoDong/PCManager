namespace ShikuIM.Model
{

    /// <summary>
    /// 客户端做的sdk分享进来的链接
    /// </summary>
    public class ShareSDKModel
    {

        private string appId;
        private string appSecret;

        // 分享的类型 目前只支持 [链接]
        private int shareType;

        private string appName;
        private string appIcon;

        // 标题
        private string title;
        // 内容
        private string subTitle;
        // 链接地址
        private string url;
        // 链接图片地址 如不填，使用icon地址
        private string imageUrl;




    }
}
