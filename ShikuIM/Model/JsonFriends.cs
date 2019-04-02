using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace ShikuIM.Model
{

    #region 朋友
    /// <summary>
    /// 接口返回的朋友
    /// </summary>
    public class JsonFriends : JsonBase
    {

        #region 构造函数
        public JsonFriends()
        {
            data = new List<DataOfFriends>();
        }
        #endregion

        public List<DataOfFriends> data { get; set; }

    }
    #endregion

    #region DataOfFriends
    /// <summary>
    /// Friend数据结构
    /// </summary>
    public class DataOfFriends
    {


        #region Public Member

        /// <summary>
        /// 此UserId为好友真实UserId
        /// </summary>
        [Key]
        public string toUserId { get; set; }

        /// <summary>
        /// 是否为设备
        /// </summary>
        public bool isDevice { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public int isOnLine { get; set; }

        /// <summary>
        /// 当前端是否发送过回执给我(在我发送200消息给其他端时，需要将该字段置为false，如果收到他们的回执，置为true)
        /// </summary>
        public int isLastPinSuccessed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int blacklist { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int companyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long lastTalkTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long modifyTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int msgNum { get; set; }

        /// <summary>
        /// 好友状态
        /// <para>-2:对方拉黑我方；-1:我方拉黑对方；0：陌生人；1:单方关注；2:互为好友</para>
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 好友昵称
        /// </summary>
        public string toNickname { get; set; }

        /// <summary>
        /// 自己的UserId(不知道有什么用。。。)
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int active { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string areaCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int areaId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int attCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double balance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long birthday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int cityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int countryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int fansCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int friendsCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string idcard { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string idcardUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int isAuth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int offlineNoPushMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int onlinestate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int provinceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int sex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double totalConsume { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double totalRecharge { get; set; }

        /// <summary>
        /// 好友的UserId
        /// </summary>
        public string userKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int vip { get; set; }

        /// <summary>
        /// 备注名
        /// </summary>
        public string remarkName { get; set; }

        public string avatarName { get; set; }

        #endregion

        #region Consturtor
        public DataOfFriends()
        {
            // if (id == null || id == "")
            //   id = Guid.NewGuid().ToString("N");
        }
        #endregion

        #region 实现数据库操作的构造函数
        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="ConnString">数据库连接字符串</param>
        public DataOfFriends(string ConnString)
        {
            //DBContext = new SQLiteDBContext(ConnString);
        }
        #endregion

        #region 分页获取好友
        public List<DataOfFriends> GetByPage(int pageindex = 0, int take = 30)
        {
            lock (Applicate.AccountDbContext)
            {
                int skip = pageindex * take;
                var Lists = Applicate.AccountDbContext.Friends.OrderBy(f => f.toNickname).ToList()
                    .Skip(skip).Take(take).ToList();//13001234567
                return Lists;
            }
        }
        #endregion

        #region 插入到数据库
        /// <summary>
        /// 插入到数据库
        /// </summary>
        public void InsertAuto()
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    if (Applicate.AccountDbContext.Friends.Count(f => f.toUserId == this.toUserId) == 0)
                    {
                        SQLiteDBContext.DBAutoConnect();
                        Applicate.AccountDbContext.Friends.Add(this);
                    }
                    else
                    {
                        var tmpres = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.toUserId == this.toUserId);
                        tmpres = this.MemberwiseClone() as DataOfFriends;
                    }
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ConsoleLog.Output("存储好友时出错" + e.Message);
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void Update()
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                            from friend in Applicate.AccountDbContext.Friends
                            where friend.toUserId == this.toUserId
                            select friend
                            ).FirstOrDefault();
                    result.active = this.active;
                    result.areaCode = this.areaCode;
                    result.areaId = this.areaId;
                    result.attCount = this.attCount;
                    result.balance = this.balance;
                    result.birthday = this.birthday;
                    result.cityId = this.cityId;
                    result.companyId = this.companyId;
                    result.countryId = this.countryId;
                    result.createTime = this.createTime;
                    result.description = this.description;
                    result.fansCount = this.fansCount;
                    result.friendsCount = this.friendsCount;
                    result.idcard = this.idcard;
                    result.idcardUrl = this.idcardUrl;
                    result.isAuth = this.isAuth;
                    result.level = this.level;
                    result.modifyTime = this.modifyTime;
                    result.name = this.name;
                    result.nickname = this.nickname;
                    result.num = this.num;
                    result.offlineNoPushMsg = this.offlineNoPushMsg;
                    result.onlinestate = this.onlinestate;
                    result.password = this.password;
                    result.phone = this.phone;
                    result.provinceId = this.provinceId;
                    result.sex = this.sex;
                    result.status = this.status;
                    result.telephone = this.telephone;
                    result.totalConsume = this.totalConsume;
                    result.totalRecharge = this.totalRecharge;
                    result.toUserId = this.toUserId;
                    result.toNickname = this.toNickname;
                    result.userKey = this.userKey;
                    result.userType = this.userType;
                    result.vip = this.vip;
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region 更新好友关系
        /// <summary>
        /// 更新好友关系
        /// </summary>
        /// <param name="UserId">好友Id</param>
        /// <param name="state">状态
        /// <para>好友状态-2黑名单(对方拉黑我方) -1://黑名单(我方拉黑对方)；0：陌生人；1:单方关注；2:互为好友</para>
        /// </param>
        public void UpdateFriendState(string UserId, int state)
        {
            try
            {
                this.toUserId = UserId;
                UpdateFriendState(state);
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region 更新好友关系
        /// <summary>
        /// 更新好友关系
        /// <para>好友状态-2黑名单(对方拉黑我方) -1://黑名单(我方拉黑对方)；0：陌生人；1:单方关注；2:互为好友</para>
        /// </summary>
        /// <param name="UserId">好友Id</param>
        /// <param name="state">状态
        /// </param>
        public void UpdateFriendState(int state)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.toUserId == this.toUserId);
                    if (result != null)
                    {
                        result.status = state;//赋值状态
                        Applicate.AccountDbContext.Entry(result).State = EntityState.Modified;
                        //Applicate.dbContext.Database.ExecuteSqlCommand("Update Friend set status = " + state + " where toUserId=" + UserId);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
                //var tmp = Applicate.dbContext.Friends.FirstOrDefault(f => f.toUserId == UserId);
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void UpdateDetial()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.toUserId == this.toUserId);
                if (result == null)
                {
                    result = new DataOfFriends();
                }
                result.active = this.active;
                result.areaCode = this.areaCode;
                result.areaId = this.areaId;
                result.attCount = this.attCount;
                result.balance = this.balance;
                result.birthday = this.birthday;
                result.cityId = this.cityId;
                result.companyId = this.companyId;
                result.countryId = this.countryId;
                result.createTime = this.createTime;
                result.description = this.description;
                result.fansCount = this.fansCount;
                result.friendsCount = this.friendsCount;
                result.idcard = this.idcard;
                result.idcardUrl = this.idcardUrl;
                result.isAuth = this.isAuth;
                result.level = this.level;
                result.modifyTime = this.modifyTime;
                result.name = this.name;
                result.nickname = this.nickname;
                result.num = this.num;
                result.offlineNoPushMsg = this.offlineNoPushMsg;
                result.onlinestate = this.onlinestate;
                result.password = this.password;
                result.phone = this.phone;
                result.provinceId = this.provinceId;
                result.sex = this.sex;
                //result.status = this.status;
                result.telephone = this.telephone;
                result.totalConsume = this.totalConsume;
                result.totalRecharge = this.totalRecharge;
                result.toUserId = this.toUserId;
                result.toNickname = this.nickname;
                result.userKey = this.userKey;
                result.userType = this.userType;
                result.vip = this.vip;
                Applicate.AccountDbContext.Entry(result).State = EntityState.Modified;
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 转DataOfUserDetial
        public DataOfUserDetial ToDataOfUserDetial()
        {
            return new DataOfUserDetial()
            {
                active = this.active,
                areaCode = this.areaCode,
                areaId = this.areaId,
                attCount = this.attCount,
                balance = this.balance,
                birthday = this.birthday,
                cityId = this.cityId,
                companyId = this.companyId,
                countryId = this.countryId,
                createTime = this.createTime,
                description = this.description,
                fansCount = this.fansCount,
                friendsCount = this.friendsCount,
                idcard = this.idcard,
                idcardUrl = this.idcardUrl,
                isAuth = this.isAuth,
                level = this.level,
                modifyTime = this.modifyTime,
                name = this.name,
                nickname = this.toNickname,
                num = this.num,
                offlineNoPushMsg = this.offlineNoPushMsg,
                onlinestate = this.onlinestate,
                password = this.password,
                phone = this.phone,
                provinceId = this.provinceId,
                sex = this.sex,
                status = this.status,
                Telephone = this.telephone,
                totalConsume = this.totalConsume,
                totalRecharge = this.totalRecharge,
                userId = this.toUserId,
                userKey = this.userKey,
                userType = this.userType,
                vip = this.vip,
                avatarName = this.avatarName,
                remarkName = this.remarkName,
            };
        }
        #endregion

        #region 根据status批量删除
        /// <summary>
        /// 根据status批量删除
        /// </summary>
        public void DeleteByStatus()
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                        from friend in Applicate.AccountDbContext.Friends
                        where friend.status == this.status
                        select friend);
                    if (result != null)
                    {
                        Applicate.AccountDbContext.Friends.RemoveRange(result);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLog.Output(e.Message);
            }
        }
        #endregion

        #region 根据status批量删除
        /// <summary>
        /// 根据status批量删除
        /// <paramref name="status">状态值</paramref>
        /// </summary>
        public void DeleteByStatus(int status)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                        from friend in Applicate.AccountDbContext.Friends
                        where friend.status == status
                        select friend).ToList();
                    if (result != null)
                    {
                        Applicate.AccountDbContext.Friends.RemoveRange(result);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region 序列化
        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        #endregion

        #region 反序列化
        public DataOfFriends toModel(string strJson)
        {
            DataOfFriends msgObj = JsonConvert.DeserializeObject<DataOfFriends>(strJson);
            return msgObj;
        }
        #endregion

        #region 根据status获得列表
        /// <summary>
        /// 根据status获得列表
        /// </summary>
        public List<DataOfFriends> GetListByStatus()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                from friend in Applicate.AccountDbContext.Friends
                where friend.status == this.status
                select friend);
                return result.ToList();
            }
        }
        #endregion

        #region 根据toUserId从本地数据库获得对象
        /// <summary>
        /// 根据toUserId获得本地数据库对象
        /// </summary>
        /// <returns></returns>
        public DataOfFriends GetByUserId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.toUserId == this.toUserId);
                return result;
            }
        }
        #endregion

        #region 是否存在数据库中
        /// <summary>
        /// 是否存在数据库中
        /// </summary>
        /// <param name="userId">需要查询的UserId</param>
        /// <returns>表示是否存在</returns>
        public bool ExistsLocal(string userId)
        {
            bool isExists = false;
            lock (Applicate.AccountDbContext)
            {
                int count = Applicate.AccountDbContext.Friends.Count(f => f.toUserId == userId);//获取数量
                isExists = (count > 0) ? (true) : (false);//赋值
                return isExists;
            }
        }
        #endregion

        #region 是否存在我方黑名单中
        /// <summary>
        /// 是否存在我方黑名单中
        /// </summary>
        /// <param name="userId">需要查询的UserId</param>
        /// <returns>是否存在值</returns>
        public bool ExistsBlackList(string userId)
        {
            bool isExists = false;
            lock (Applicate.AccountDbContext)
            {
                int count = Applicate.AccountDbContext.Friends.Count(f => f.toUserId == userId && f.status == -1);
                isExists = (count > 0) ? (true) : (false);//赋值
            }
            return isExists;
        }
        #endregion

        #region 是否存在我方黑名单中
        /// <summary>
        /// 是否存在我方黑名单中
        /// </summary>
        /// <param name="userId">需要查询的UserId</param>
        /// <returns>是否存在值</returns>
        public bool Exists(string userId)
        {
            bool isExists = false;
            lock (Applicate.AccountDbContext)
            {
                int count = Applicate.AccountDbContext.Friends.Count(f => f.toUserId == userId  );
                isExists = (count > 0) ? (true) : (false);//赋值
            }
            return isExists;
        }
        #endregion

        #region 与该好友是否为好友
        /// <summary>
        /// 与该好友是否为好友
        /// </summary>
        /// <param name="userId">需要查询的UserId</param>
        /// <returns>是否存在</returns>
        public bool ExistsFriend(string userId)
        {
            this.toUserId = userId;
            return ExistsFriend();
        }
        #endregion

        #region 与该好友是否为好友
        /// <summary>
        /// 与该好友是否为好友
        /// </summary>
        /// <param name="userId">需要查询的UserId</param>
        /// <returns>是否存在</returns>
        public bool ExistsFriend()
        {
            bool isExists = false;
            lock (Applicate.AccountDbContext)
            {
                var count = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.status == 2 && f.toUserId == this.toUserId);
                isExists = (count != null) ? (true) : (false);//赋值
            }

            return isExists;
        }
        #endregion

        #region 根据UserId获得对象
        /// <summary>
        /// 根据UserId获得对象
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataOfFriends GetByUserId(string userId)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.Friends
                    where friend.toUserId == userId
                    select friend).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据UserName获取(支持模糊查询)
        public List<DataOfFriends> GetByUserName()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                from friend in Applicate.AccountDbContext.Friends
                where friend.toNickname.Contains(this.toNickname)
                select friend).ToList();
                return result;
            }
        }
        #endregion

        #region 自动添加或更新
        /// <summary>
        /// 自动添加或更新
        /// </summary>
        public void AutoInsert()
        {
            //if (this.toUserId == Applicate.MyAccount.userId)
            //    return;
            lock (Applicate.AccountDbContext)
            {
                int exists = Applicate.AccountDbContext.Friends.Count(f => f.toUserId == this.toUserId);
                if (exists == 0)
                {
                    Applicate.AccountDbContext.Friends.Add(this);
                }
                else
                {
                    var tmpres = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.toUserId == this.toUserId);
                    tmpres = this.MemberwiseClone() as DataOfFriends;
                }
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 获取好友列表
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        public List<DataOfFriends> GetFriendsList()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                //var DBContext = new SQLiteDBContext();
                var result = (
                from friend in Applicate.AccountDbContext.Friends
                where friend.status == 2
                select friend
                );
                return result.ToList();
            }
        }
        #endregion

        #region 获取黑名单列表
        /// <summary>
        /// 获取黑名单列表
        /// </summary>
        /// <returns></returns>
        public List<DataOfFriends> GetBlacksList()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                from friend in Applicate.AccountDbContext.Friends
                where friend.status == -1
                select friend
                ).ToList();
                return result;
            }
        }
        #endregion

        #region 获取黑名单列表
        /// <summary>
        /// 获取黑名单列表
        /// </summary>
        /// <returns></returns>
        public DataOfFriends GetBlackUser(string userid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from fri in Applicate.AccountDbContext.Friends
                    where fri.status == -1 && fri.toUserId == userid
                    select fri
                ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 获取所有用户列表
        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        public List<DataOfFriends> GetAllList()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                from friend in Applicate.AccountDbContext.Friends
                select friend);
                if (result == null)
                {
                    return new List<DataOfFriends>();
                }
                else
                {
                    return result.ToList();
                }
            }
        }
        #endregion

        #region 更新备注
        public void UpdateRemarkName(string UserId, string remarkName)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = Applicate.AccountDbContext.Friends.FirstOrDefault(f => f.toUserId == UserId);
                    if (result != null)
                    {
                        result.remarkName = remarkName;//赋值
                        Applicate.AccountDbContext.Entry(result).State = EntityState.Modified;
                        //Applicate.dbContext.Database.ExecuteSqlCommand("Update Friend set status = " + state + " where toUserId=" + UserId);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Output(ex.Message);
            }
        }
        #endregion

        #region 好友总数
        /// <summary>
        /// 好友总数
        /// </summary>
        /// <returns></returns>
        internal int FriendsCount()
        {
            lock (Applicate.AccountDbContext)
            {
                int res = 0;
                SQLiteDBContext.DBAutoConnect();
                res = Applicate.AccountDbContext.Friends.Count();
                return res;
            }
        }
        #endregion

        #region 获取对应UserId的好友昵称
        /// <summary>
        /// 获取对应UserId的好友昵称
        /// </summary>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public string GetUserNameByUserId(string toUserId)
        {
            string nickname = "";
            SQLiteDBContext.DBAutoConnect();
            nickname = (from friends in Applicate.AccountDbContext.Friends
                        where friends.userId == toUserId
                        select friends.toNickname
                        ).FirstOrDefault();//获取对应UserId的好友的昵称
            return nickname;
        }
        #endregion

    }
    #endregion


}
