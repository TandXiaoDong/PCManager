namespace ShikuIM.Model
{
    public class JsonSettings : JsonBase
    {
        #region 隐私
        public JsonSettings()
        {
            data = new Settings();
        }
        #endregion

        /// <summary>
        /// 数据
        /// </summary>
        public Settings data { get; set; }
    }
}
