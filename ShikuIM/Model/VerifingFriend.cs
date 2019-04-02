using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShikuIM.Model
{
    public class VerifingFriend : ObservableObject
    {

        #region Property and variable statement
        /// <summary>
        /// 数据库操作
        /// </summary>
        //SQLiteDBContext DBContext { get; set; }
        [Key]
        public string id { get; set; }
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
        public int createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int lastTalkTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int modifyTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int msgNum { get; set; }


        private int verifyStatus;

        /*<para>-6:对方添加我方，我回话；-5:我方添加对方, 对方回话；</para>*/
        /// <summary>
        /// <para>验证状态：</para>
        /// <para> -4:对方添加自己为好友；-3:我方添加对方为好友；-2:对方拉黑己方；-1我方拉黑对方</para>
        /// <para>0:陌生人；1:互为好友；2:白名单；3:系统号；4:非显示系统号</para>
        /// </summary>
        public int VerifyStatus
        {
            get { return verifyStatus; }
            set
            {
                if (verifyStatus == value)//值相同则返回
                {
                    return;
                }

                verifyStatus = value;
                //RaisePropertyChanged(nameof(VerifyStatus));
                if (verifyStatus >= -2)
                {
                    CanAgree = false;
                    CanAnswer = false;
                }
                else if (verifyStatus == -3)
                {
                    CanAgree = false;
                    CanAnswer = false;
                }
                else if (verifyStatus == -4)
                {
                    CanAgree = true;
                    CanAnswer = true;
                }
            }
        }

        private bool canAgree;
        /// <summary>
        /// 是否允许通过验证
        /// </summary>
        public bool CanAgree
        {
            get { return canAgree; }
            set { canAgree = value; RaisePropertyChanged(nameof(CanAgree)); }
        }

        private bool canAnswer;
        /// <summary>
        /// 是否允许回话
        /// </summary>
        public bool CanAnswer
        {
            get { return canAnswer; }
            set { canAnswer = value; RaisePropertyChanged(nameof(CanAnswer)); }
        }


        /// <summary>
        /// 最后一条消息类型
        /// </summary>
        public int Type { get; set; }

        private string content;
        /// <summary>
        /// 消息内容(好友关系与状态描述)
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(nameof(Content)); }
        }



        private string statusTag;
        /// <summary>
        /// 状态
        /// </summary>
        public string StatusTag
        {
            get { return statusTag; }
            set { statusTag = value; RaisePropertyChanged(nameof(StatusTag)); }
        }


        /// <summary>
        /// 对方昵称
        /// </summary>
        public string toNickname { get; set; }

        /// <summary>
        /// 此UserId为好友真实UserId
        /// </summary>
        string _toUserId;
        public string toUserId
        {
            get { return _toUserId; }
            set { _toUserId = value; RaisePropertyChanged(nameof(toUserId)); }
        }

        /// <summary>
        /// 自己的UserId
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 活动？..
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
        public int totalConsume { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double totalRecharge { get; set; }

        /// <summary>
        /// 拿来放申请加群的userIds
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

        #endregion

        #region Commands

        /// <summary>
        /// 删除好友验证项
        /// </summary>
        public ICommand DeleteVerifyItemCommand
        {
            get
            {
                return new RelayCommand(() =>
               {
                   try
                   {
                       //VerifyUserList.Remove(item);
                       Task.Run(() =>
                      {
                          this.Delete();//删除数据库
                      });
                       Messenger.Default.Send(this, VerifyFriendLIstToken.DeleteVerifyItem);//
                   }
                   catch (Exception ex)
                   {
                       ConsoleLog.Output(ex.Message);
                   }
               });
            }
        }
        #endregion

        #region 实体类
        public VerifingFriend()
        {
        }
        #endregion

        #region 实现数据库操作的构造函数
        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="ConnString">数据库连接字符串</param>
        public VerifingFriend(string ConnString)
        {
            //DBContext = new SQLiteDBContext(ConnString);
        }
        #endregion

        #region 插入到数据库
        /// <summary>
        /// 插入到数据库
        /// </summary>
        public void Insert()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                if (this.id == null || this.id == Guid.Empty.ToString("N"))
                {
                    this.id = Guid.NewGuid().ToString("N");
                }

                Applicate.AccountDbContext.VerifingFriends.Add(this);
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void Update()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.toUserId == this.toUserId
                    select friend
                    ).FirstOrDefault();
                if (result != null)
                {
                    result.active = this.active;
                    result.CanAgree = this.CanAgree;
                    result.CanAnswer = this.CanAnswer;
                    result.Content = this.Content;
                    result.StatusTag = this.StatusTag;
                    result.VerifyStatus = this.VerifyStatus;
                    result.toNickname = this.toNickname;
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.id == this.id
                    select friend
                    );
                Applicate.AccountDbContext.VerifingFriends.RemoveRange(result);
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 根据status删除
        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteByStatus()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.VerifyStatus == this.VerifyStatus
                    select friend
                    );
                Applicate.AccountDbContext.VerifingFriends.RemoveRange(result);
                Applicate.AccountDbContext.SaveChanges();
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
        public VerifingFriend toModel(string strJson)
        {
            VerifingFriend msgObj = JsonConvert.DeserializeObject<VerifingFriend>(strJson);
            return msgObj;
        }
        #endregion

        #region 根据status获得列表
        /// <summary>
        /// 根据status获得列表
        /// </summary>
        public List<VerifingFriend> GetListByStatus()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.VerifyStatus == this.VerifyStatus
                    select friend
                    );
                return result.ToList();
            }
        }
        #endregion

        #region 根据toUserId获得对象
        /// <summary>
        /// 根据toUserId获得对象
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public VerifingFriend GetByUserId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.toUserId == this.toUserId
                    select friend
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据UserName获取(支持模糊查询)
        public List<VerifingFriend> GetByUserName()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.toNickname.Contains(this.toNickname)
                    select friend
                    ).ToList();
                return result;
            }
        }
        #endregion

        #region 添加新的对象
        /// <summary>
        /// 添加新的对象
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void InsertOrUpdate()
        {
            var result = GetByUserId();
            if (result == null)
            {
                Insert();
            }
            else
            {
                Update();
            }
        }
        #endregion


        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void DelFriend()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from friend in Applicate.AccountDbContext.VerifingFriends
                    where friend.toUserId == this.toUserId
                    select friend
                    ).FirstOrDefault();
                result.VerifyStatus = 0;
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public void AddBlack()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                     from friend in Applicate.AccountDbContext.VerifingFriends
                     where friend.toUserId == this.toUserId
                     select friend
                     ).FirstOrDefault();
                result.VerifyStatus = -1;
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 移出黑名单
        /// </summary>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        public void DelBlack()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                     from friend in Applicate.AccountDbContext.VerifingFriends
                     where friend.toUserId == this.toUserId
                     select friend
                     ).FirstOrDefault();
                result.VerifyStatus = 2;
                Applicate.AccountDbContext.SaveChanges();
            }
        }


        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        public List<VerifingFriend> GetVerifingsList()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from verify in Applicate.AccountDbContext.VerifingFriends
                    select verify
                    );
                return result.ToList();
            }
        }



    }
}

