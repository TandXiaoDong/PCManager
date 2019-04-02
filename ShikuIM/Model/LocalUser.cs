using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShikuIM.Model
{
    /// <summary>
	/// user实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
    public partial class LocalUser
    {

        #region 实现数据库操作的构造函数
        /// <summary>
        /// 有参构造函数
        /// </summary>
        public LocalUser()
        {
        }
        #endregion

        #region 插入到数据库
        /// <summary>
        /// 插入到数据库
        /// </summary>
        public void InsertOrUpdatePassword()
        {
            lock (Applicate.SystemDbContext)
            {
                SystemDBContext.DBAutoConnect();
                if (this.id == null || this.id == Guid.Empty.ToString("N"))
                {
                    this.id = Guid.NewGuid().ToString("N");
                }
                if (Applicate.SystemDbContext.user.Count(l => l.Telephone == this.Telephone) > 0)//如果数据库存在项
                {
                    UpdatePwd(this.Telephone, this.Password, this.PasswordLength);//更新密码
                }
                else
                {
                    Applicate.SystemDbContext.user.Add(this);//插入
                }
                int res = Applicate.SystemDbContext.SaveChanges();//保存更改
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void Update()
        {
            SystemDBContext.DBAutoConnect();
            var result = (
                from user in Applicate.SystemDbContext.user
                where user.UserId == this.UserId
                select user
                ).FirstOrDefault();
            result = this;
            Applicate.SystemDbContext.SaveChanges();
        }
        #endregion

        #region 根据时间获取本地用户
        /// <summary>
        /// 根据时间获取本地用户
        /// </summary>
        /// <returns>最后登录的用户</returns>
        internal LocalUser GetLastUserByTime()
        {
            SystemDBContext.DBAutoConnect();
            lock (Applicate.SystemDbContext)
            {
                if (Applicate.SystemDbContext.user.Count() > 0)
                {
                    var u1 = (from users in Applicate.SystemDbContext.user
                             orderby users.LastLoginTime descending
                             select users).FirstOrDefault();

                    var user = (from users in Applicate.SystemDbContext.user
                                orderby users.LastLoginTime ascending
                                select users
                                ).FirstOrDefault();
                    return u1;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void UpdatePwd(string phoneNumber, string password, int pwdLength)
        {
            lock (Applicate.SystemDbContext)
            {
                SystemDBContext.DBAutoConnect();
                var localuser = Applicate.SystemDbContext.user.FirstOrDefault(l => l.Telephone == phoneNumber);
                localuser.LastLoginTime = LastLoginTime;//设置登录时间
                localuser.Password = password;//设置密码
                localuser.PasswordLength = pwdLength;//设置密码长度
                Applicate.SystemDbContext.SaveChanges();//保存更改
            }
        }
        #endregion

        #region 获取上次的离线时间
        /// <summary>
        /// 获取上次的离线时间
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        internal long GetLastExitTime(string userId)
        {
            lock (Applicate.SystemDbContext.user)
            {
                long exitTime = 0;
                if (Applicate.SystemDbContext.user.Count(u => u.UserId == userId) > 0)//如果存在则查出
                {
                    var tmp = (from users in Applicate.SystemDbContext.user
                               where users.UserId == userId
                               select users
                                 ).FirstOrDefault(); //获取
                    if (tmp != null)
                    {
                        exitTime = tmp.LastExitTime;
                    }
                }
                return exitTime;
            }
        }
        #endregion

        #region 更新上次离线时间
        /// <summary>
        /// 更新上次离线时间
        /// </summary>
        /// <param name="userId">对应的用户</param>
        /// <param name="lastexittime">上次离线时间</param>
        internal void UpdateLastExitTime(string userId, long lastexittime)
        {
            lock (Applicate.SystemDbContext)
            {
                if (Applicate.SystemDbContext.user.Count(u => u.UserId == userId) > 0)//如果存在此人
                {
                    SystemDBContext.DBAutoConnect();
                    var user = Applicate.SystemDbContext.user.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        ConsoleLog.Output("用户" + userId + "退出时间为:" + Helpers.StampToDatetime(LastExitTime).ToShortDateString());
                        user.LastExitTime = lastexittime;//更新离线时间
                        var tmp = Applicate.SystemDbContext.SaveChanges();//保存更改
                        ConsoleLog.Output("保存退出时间为:" + tmp);
                    }
                }
                else
                {

                }
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(string userId)
        {
            SystemDBContext.DBAutoConnect();
            lock (Applicate.SystemDbContext.user)
            {
                var item = Applicate.SystemDbContext.user.FirstOrDefault(i => i.UserId == userId);
                if (item != null)
                {
                    Applicate.SystemDbContext.user.Remove(item);
                }
            }
            Applicate.SystemDbContext.SaveChanges();
        }
        #endregion

        #region 序列化
        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        #endregion

        #region 反序列化
        public LocalUser toModel(string strJson)
        {
            LocalUser msgObj = JsonConvert.DeserializeObject<LocalUser>(strJson);
            return msgObj;
        }
        #endregion

        #region 获取集合
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public List<LocalUser> GetAllList()
        {
            var result = new List<LocalUser>();
            lock (Applicate.SystemDbContext.user)
            {
                SystemDBContext.DBAutoConnect();
                result = (
                    from user in Applicate.SystemDbContext.user
                    select user
                ).ToList();
            }
            return result;
        }
        #endregion

        #region 获取对象
        public LocalUser GetModel()
        {
            SystemDBContext.DBAutoConnect();
            var result = (
            from user in Applicate.SystemDbContext.user
            where user.UserId == this.UserId
            select user
            ).FirstOrDefault();
            return result;
        }
        #endregion

        #region 根据telephone获取对象
        public LocalUser GetModelByPhone(string phoneNumber)
        {
            SystemDBContext.DBAutoConnect();
            var result = Applicate.SystemDbContext.user.FirstOrDefault(u => u.Telephone == phoneNumber);
            return result;
        }
        #endregion


        #region Model
        private string _id;
        private string _userid;
        private string _telephone;
        private string _password;
        private long lastLoginTime;
        private int passwordLength;
        private long lastExitTime;

        /// <summary>
        /// 
        /// </summary>
        [Key]
        public string id
        {
            set { _id = value; }
            get { return _id; }
        }


        /// <summary>
        /// 上次退出时间
        /// </summary>
        public long LastExitTime
        {
            get { return lastExitTime; }
            set { lastExitTime = value; }
        }

        /// <summary>
        /// 本地用户UserId
        /// </summary>
        public string UserId
        {
            set { _userid = value; }
            get { return _userid; }
        }

        /// <summary>
        /// 本地用户电话
        /// </summary>
        public string Telephone
        {
            set { _telephone = value; }
            get { return _telephone; }
        }

        /// <summary>
        /// 本地用户密码
        /// </summary>
        public string Password
        {
            set { _password = value; }
            get { return _password; }
        }

        /// <summary>
        /// 登录时间
        /// </summary>
        public long LastLoginTime
        {
            set
            {
                lastLoginTime = value;
            }
            get { return lastLoginTime; }
        }

        /// <summary>
        /// 密码长度
        /// </summary>
        public int PasswordLength
        {
            set { passwordLength = value; }
            get { return passwordLength; }
        }

        #endregion

    }
}
