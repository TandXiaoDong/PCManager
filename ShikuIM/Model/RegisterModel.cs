namespace ShikuIM.Model
{

    public class RegisterModel : JsonBase
    {
        public RegisterModel()
        {
            Data = new RegisterData();
        }
        public RegisterData Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RegisterData
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int createTime { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string resultMsg { get; set; }

    }
}
