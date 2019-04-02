using System.IO;

namespace ShikuIM.Model
{
    public class PubConstant
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                //此处动态赋值
                string path = Path.GetFullPath("./db/" + Applicate.MyAccount.userId + ".db");
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
                string connectionString = "Data Source=" + path + ";Version=3";
                string ConStringEncrypt = "false";
                if (ConStringEncrypt == "true")
                {
                    connectionString = DESEncrypt.Decrypt(connectionString);
                }
                return connectionString;
            }
        }

        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {

            string connectionString = "Data Source=" + Path.GetFullPath("./db/" + Applicate.MyAccount.userId + ".db") + ";Version=3";
            string ConStringEncrypt = "false";
            if (ConStringEncrypt == "true")
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
            return connectionString;
        }

    }
}
