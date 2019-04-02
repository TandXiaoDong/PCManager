using System.ComponentModel.DataAnnotations;

namespace ShikuIM.Model
{
    public class JsonAttention : JsonBase
    {
        public JsonAttention()
        {

        }
        public dataOfAttention data { get; set; }
    }
    public class dataOfAttention
    {
        [Key]
        public string id { get; set; }


        /// <summary>
        /// 添加好友返回状态
        /// </summary>
        public int type { get; set; }
    }
}
