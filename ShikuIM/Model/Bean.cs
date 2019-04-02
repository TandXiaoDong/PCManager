using System;

namespace ShikuIM.Model
{
    public class Bean
    {
        public String id { get; set; }// Id
                                      //创建人
        public string userId { get; set; }
        //标签码
        public String code { get; set; }
        //标签名称
        public String name { get; set; }
        //存放图片文件地址
        public String logo { get; set; }
        //备注
        public String mark { get; set; }
        //有效期
        public long time { get; set; }
        //是否有效
        public Boolean valid { get; set; }

    }
}
