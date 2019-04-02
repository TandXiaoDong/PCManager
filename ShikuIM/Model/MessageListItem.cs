using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShikuIM.Model
{

    #region 设置消息项的类型
    /// <summary>
    /// 设置消息项的类型
    /// </summary>
    public enum ItemType
    {

        /// <summary>
        /// 消息类型
        /// </summary>
        Message = 0,

        /// <summary>
        /// 群组 (或者群主)
        /// </summary>
        Group = 1,

        /// <summary>
        /// 用户 (或者管理员)
        /// </summary>
        User = 2,

        /// <summary>
        /// 好友请求消息列表项 (或者普通群成员)
        /// </summary>
        VerifyMsg = 3,

        /// <summary>
        /// 静音的项
        /// </summary>
        Mute = 4
    }
    #endregion
    /// <summary>
    /// 用于存储消息列表项的
    /// </summary>
    public class MessageListItem : ObservableObject
    {
        #region Private Member
        private int noReadCount;
        private string messageTitle;
        private ItemType messageItemType;
        private string messageItemContent;
        private long timeSend;
        private string _jid;
        private Messageobject msg;
        private string _id;
        private string _showTitle = "";
        private bool _isVisibility = true;
        private string message;
        private string avator;
        #endregion

        #region Public Member

        /// <summary>
        /// 头像路径
        /// </summary>
        public string Avator
        {
            get { return avator; }
            set { avator = value; RaisePropertyChanged(nameof(Avator)); }
        }

        /// <summary>
        /// 未读计数,
        /// </summary>
        public int UnReadCount
        {
            get { return noReadCount; }
            set
            {
                noReadCount = value;
                RaisePropertyChanged(nameof(UnReadCount));
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string MessageTitle
        {
            get
            { return messageTitle; }
            set
            {
                messageTitle = value;
                RaisePropertyChanged(nameof(MessageTitle));
            }
        }

        /// <summary>
        /// 消息项类型
        /// </summary>
        public ItemType MessageItemType
        {
            get
            {
                return messageItemType;
            }

            set
            {
                messageItemType = value;
                RaisePropertyChanged(nameof(MessageItemType));
            }
        }

        /// <summary>
        /// 项内容 (或用于传递发送列表时为当前登录用户在对应会话群里的群昵称)
        /// </summary>
        public string MessageItemContent
        {
            get { return messageItemContent; }

            set
            {
                messageItemContent = value;
                RaisePropertyChanged(nameof(MessageItemContent));
            }
        }

        /// <summary>
        /// 发送时间
        /// </summary>
        public long TimeSend
        {
            get { return timeSend; }
            set
            {
                timeSend = value;
                RaisePropertyChanged(nameof(TimeSend));
            }
        }

        /// <summary>
        /// 一般用于存放UserId或房间Jid
        /// </summary>
        [Key]
        public string Jid
        {
            get
            {
                return _jid;
            }
            set
            {
                _jid = value;
                RaisePropertyChanged(nameof(Jid));
            }
        }

        [Column("RoomId")]
        /// <summary>
        /// 一般存RoomID
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 数据库存储的消息(仅存储)
        /// </summary>
        public string Message
        {
            get
            {
                if (Msg == null || string.IsNullOrEmpty(Msg.messageId))
                {
                    message = JsonConvert.SerializeObject(Msg);
                }
                return message;
            }
            set
            {
                Msg = new Messageobject();
                message = value;
                Msg = JsonConvert.DeserializeObject<Messageobject>(value);
            }
        }

        /// <summary>
        /// Json消息(此属性运行时操作,不参与数据库存储)
        /// </summary>
        [NotMapped]
        public Messageobject Msg
        {
            get
            {
                return msg;
            }
            set
            {
                message = JsonConvert.SerializeObject(value);
                msg = value;
                RaisePropertyChanged(nameof(Msg));
            }
        }

        /// <summary>
        /// 备注名
        /// </summary>
        public string ShowTitle
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_showTitle))//如果为空显示真实昵称
                {
                    return MessageTitle;
                }
                else//如果不为空显示备注
                {
                    return _showTitle;
                }
            }
            set
            {
                //if (value == null)
                //{
                //    return;
                //}
                _showTitle = value;
                RaisePropertyChanged(nameof(ShowTitle));
            }
        }

        [NotMapped]
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisibility
        {
            get { return _isVisibility; }
            set
            {
                _isVisibility = value;
                RaisePropertyChanged(nameof(IsVisibility));
            }
        }
        #endregion

        /// <summary>
        /// 用于暂时存储数据的标签数据
        /// </summary>
        public string Tag { get; set; }


        #region Constructor
        public MessageListItem()
        {

        }
        #endregion

        #region 实现数据库操作的构造函数
        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="ConnString">数据库连接字符串</param>
        public MessageListItem(string ConnString)
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
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    if (Jid == null || Jid == Guid.Empty.ToString("N"))
                    {
                        Jid = Guid.NewGuid().ToString("N");
                    }
                    Applicate.AccountDbContext.MessageList.Add(this);
                    int res = Applicate.AccountDbContext.SaveChanges();
                    ConsoleLog.Output("Insert RecentList --:" + res);//
                }
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
        public void Update()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from msg in Applicate.AccountDbContext.MessageList
                    where msg.Jid == this.Jid
                    select msg
                    ).FirstOrDefault();
                if (result != null)
                {
                    //result.Jid = this.Jid;
                    result.UnReadCount = this.UnReadCount;
                    result.MessageTitle = this.MessageTitle;
                    result.MessageItemType = this.MessageItemType;
                    result.MessageItemContent = this.MessageItemContent;
                    result.TimeSend = this.TimeSend;
                    result.Msg = this.Msg;
                    result.Avator = this.Avator;
                    result.ShowTitle = this.ShowTitle;
                    //result.Id = this.Id;
                    //Applicate.dbContext.Entry(result).State = EntityState.Modified;
                    var res = Applicate.AccountDbContext.SaveChanges();
                    ConsoleLog.Output("Update RecentList --:" + res);//
                }
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void Update(MessageListItem item)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from msg in Applicate.AccountDbContext.MessageList
                    where msg.Jid == this.Jid
                    select msg
                    ).FirstOrDefault();
                result = item.Clone();
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    Applicate.AccountDbContext.MessageList.Remove(this);
                    Applicate.AccountDbContext.SaveChanges();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        public void DeleteAllList()
        {
            lock (Applicate.AccountDbContext)
            {
                try
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                        from msg in Applicate.AccountDbContext.MessageList
                        select msg
                        ).ToList();
                    if (result != null && result.Count != 0)
                    {
                        Applicate.AccountDbContext.MessageList.RemoveRange(result);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ConsoleLog.Output(ex.Message);
                }
            }
        }
        #endregion

        #region 获取集合
        public List<MessageListItem> GetAllList()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from msg in Applicate.AccountDbContext.MessageList
                    orderby msg.TimeSend descending
                    select msg
                    ).ToList();
                return result;
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
        public MessageListItem toModel(string msgJson)
        {
            MessageListItem msgObj = JsonConvert.DeserializeObject<MessageListItem>(msgJson);
            return msgObj;
        }

        #endregion

    }
}
