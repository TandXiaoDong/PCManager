using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace ShikuIM.Model
{
    public enum orderBy
    {
        byDefult = 0,
        byAsc = 1,
        byDesc = 2,
    }

    #region 消息
    /// <summary>
    /// 接口返回的消息
    /// </summary>
    public class JsonParentMessage : JsonBase
    {

        #region 构造函数
        public JsonParentMessage()
        {
            data = new List<JsonMessage>();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public List<JsonMessage> data { get; set; }
    }
    #endregion

    #region JsonMessage
    /// <summary>
    /// 从Http获取的 聊天记录
    /// </summary>
    public class JsonMessage
    {

        /// <summary>
        /// ID
        /// </summary>
        public string _id { get; set; }

        /// <summary>
        /// Json消息主体
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// XML消息主体
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 回复者
        /// </summary>
        public string receiver { get; set; }

        /// <summary>
        /// 回复者Jid
        /// </summary>
        public string receiver_jid { get; set; }

        /// <summary>
        /// 发送者
        /// </summary>
        public string sender { get; set; }

        /// <summary>
        /// 发送者Jid
        /// </summary>
        public string sender_jid { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public int isRead { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long ts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int contentType { get; set; }

        /// <summary>
        /// direction=1（receiver=发送方，sender=接收方）
        /// direction=0（receiver=接收方；sender=发送方）
        /// </summary>
        public int direction { get; set; }

        /// <summary>
        /// RoomJid
        /// </summary>
        public string room_jid_id { get; set; }

        /// <summary>
        /// 原Xmpp格式Jid
        /// </summary>
        public string room_jid { get; set; }

    }
    #endregion

    #region MessageObject
    /// <summary>
    /// 信息主体
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Messageobject : ObservableObject
    {

        #region Private Member

        private int _isDelay;
        private string _fromUserId;
        private string _fromUserName;
        //[JsonIgnore]
        private string _toUserId;
        private string _toUserName;
        private string _content;
        private string _fileName;
        private string _filePath;
        private int _fileSize;
        private int _timeLen;
        private double? _location_x;
        private double? _location_y;
        private bool? _isReadDel;
        private int _isEncrypt;
        private long _timeSend;
        private kWCMessageType _type;
        /*以下数据存于本地数据库中*/
        private long _readTime;
        private string _readPersons;
        private int _reSendCount;
        private int _messageState;
        private bool _isDownload;
        private int _sendRead;
        private int _isSend;
        private int _isRead;
        private int _isReceive;
        private int _isUpload;
        private long _timeReceive;
        private int _isShowTime;
        private int _sendCount;
        private string _fromId;
        private string _objectId;
        private string toId;
        private int _platformType;




        #endregion

        /// <summary>
        /// 数据表名前缀
        /// </summary>
        public static readonly string Prefex = "msg_";


        #region Constructor
        public Messageobject()
        {
            this.myUserId = Applicate.MyAccount.userId;
            this.messageId = Guid.NewGuid().ToString("N");
        }
        #endregion

        #region Public Member

        /// <summary>
        /// 我的UserId
        /// </summary>
        [JsonIgnore]
        public string myUserId { get; set; }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        [JsonIgnore]
        public int _id { get; set; }


        /// <summary>
        /// 消息类型
        /// </summary>
        public kWCMessageType type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(nameof(type));
            }
        }

        /// <summary>
        /// 接收者类型
        /// </summary>
        public int toUserType { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public int companyId { get; set; }

        /// <summary>
        /// 此条消息对应的UserId(如果是自己发送, jid就为ToUserId, 否则为FromUserId, 不能为自己的UserId)
        /// </summary>
        [JsonIgnore]
        public string jid
        {
            get
            {
                if (myUserId == ToId || string.IsNullOrWhiteSpace(ToId))
                {
                    return FromId;
                }
                else
                {
                    return ToId;
                }
            }
        }

        /// <summary>
        /// 实际发送者Id
        /// </summary>
        [JsonIgnore]
        public string FromId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_fromId))
                {
                    return fromUserId;
                }
                else
                {
                    return _fromId;
                }
            }
            set { _fromId = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string ToId
        {
            get { return toId == null ? toUserId : toId; }
            set { toId = value; }
        }

        /// <summary>
        /// 发送者的类型
        /// </summary>
        public int fromUserType { get; set; }

        /// <summary>
        /// 是否离线消息,单聊
        /// </summary>
        public int isDelay
        {
            get
            {
                return _isDelay;
            }
            set
            {
                _isDelay = value;
                RaisePropertyChanged(nameof(isDelay));
            }
        }

        /// <summary>
        /// 消息唯一ID(每条JsonMessage都必须存在)
        /// </summary>
        public string messageId { get; set; }

        /// <summary>
        /// 发送者
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fromUserId
        {
            get { return _fromUserId; }
            set
            {
                _fromUserId = value;
                RaisePropertyChanged(nameof(fromUserId));
            }
        }

        /// <summary>
        /// 源
        /// </summary>
        public string fromUserName
        {
            get
            {
                return _fromUserName;
            }
            set
            {
                _fromUserName = value;
                RaisePropertyChanged(nameof(fromUserName));
            }
        }

        /// <summary>
        /// 目标
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toUserId
        {
            get
            {
                return _toUserId;
            }
            set
            {
                _toUserId = value;
                RaisePropertyChanged(nameof(toUserId));
            }
        }

        /// <summary>
        /// 目标
        /// </summary>
        public string toUserName
        {
            get
            {
                return _toUserName;
            }
            set
            {
                _toUserName = value;
                RaisePropertyChanged(nameof(toUserName));
            }
        }

        /// <summary>
        /// 内容,祝福语
        /// </summary>
        //[JsonConverter(typeof(JsonToString))]
        public string content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value == null)
                {
                    _content = "";
                }
                else
                {
                    _content = value;
                }
                RaisePropertyChanged(nameof(content));
            }
        }

        /// <summary>
        /// 文件名，发送方的本地文件名，音视频时为呼叫号码
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                RaisePropertyChanged(nameof(fileName));
            }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string filePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                RaisePropertyChanged(nameof(filePath));
            }
        }

        /// <summary>
        /// 对象ID，一般用来存roomJid(实际为了方便用于存放@群成员时的UserId或RoomId)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string objectId
        {
            get { return _objectId; }
            set { _objectId = value; RaisePropertyChanged(nameof(objectId)); }
        }

        /// <summary>
        /// 文件尺寸(kb),红包领取状态
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int fileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
                RaisePropertyChanged(nameof(fileSize));
            }
        }

        /// <summary>
        /// 录音或语音时长，秒
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int timeLen
        {
            get
            {
                return _timeLen;
            }
            set
            {
                _timeLen = value;
                RaisePropertyChanged(nameof(timeLen));
            }
        }

        /// <summary>
        /// 位置经度(或者图片长度)
        /// 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? location_x
        {
            get
            {
                return _location_x;
            }
            set
            {
                _location_x = value;
                RaisePropertyChanged(nameof(location_x));
            }
        }

        /// <summary>
        /// 位置纬度(或者图片宽度)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? location_y
        {
            get
            {
                return _location_y;
            }
            set
            {
                _location_y = value;
                RaisePropertyChanged(nameof(location_y));
            }
        }

        /// <summary>
        /// 是否阅后即焚
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? isReadDel
        {
            get
            {
                return _isReadDel;
            }
            set
            {
                _isReadDel = value;
                RaisePropertyChanged(nameof(isReadDel));
            }
        }

        /// <summary>
        /// 是否加密信息
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int isEncrypt
        {
            get
            {
                return _isEncrypt;
            }
            set
            {
                _isEncrypt = value;
                RaisePropertyChanged(nameof(isEncrypt));
            }
        }

        /// <summary>
        /// 发送的时间，发送前赋当前机器时间
        /// </summary>
        public long timeSend
        {
            get
            {
                return _timeSend;
            }
            set
            {
                _timeSend = value;
                RaisePropertyChanged(nameof(timeSend));
            }
        }


        /*--------------------------------------------以下字段存本地数据库,,不参与Json序列化--------------------------------------------*/


        /// <summary>
        /// 阅读时间
        /// </summary>
        [JsonIgnore]
        public long readTime
        {
            get
            {
                return _readTime;
            }
            set
            {
                _readTime = value;
                RaisePropertyChanged(nameof(readTime));
            }
        }

        /// <summary>
        /// 已读人数
        /// </summary>
        [JsonIgnore]
        public string readPersons
        {
            get
            {
                return _readPersons;
            }
            set
            {
                _readPersons = value;
                RaisePropertyChanged(nameof(readPersons));
            }
        }

        /// <summary>
        /// 重发次数
        /// </summary>
        [JsonIgnore]
        public int reSendCount
        {
            get
            {
                return _reSendCount;
            }
            set
            {
                _reSendCount = value;
                RaisePropertyChanged(nameof(reSendCount));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int sipDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public bool sipStatus { get; set; }

        /// <summary>
        /// 消息状态 -1为发送失败，0为未发送，1为送达，2为已读
        /// </summary>
        [JsonIgnore]
        public int messageState
        {
            get
            {
                return _messageState;
            }
            set
            {
                _messageState = value;
                RaisePropertyChanged(nameof(messageState));
            }
        }

        /// <summary>
        /// 是否已下载（群验证拿来当是否已确认）
        /// </summary>
        [JsonIgnore]
        public bool isDownload
        {
            get
            {
                return _isDownload;
            }
            set
            {
                _isDownload = value;
                RaisePropertyChanged(nameof(isDownload));
            }
        }

        /// <summary>
        /// 是否已经发送已读
        /// </summary>
        [JsonIgnore]
        public int sendRead
        {
            get
            {
                return _sendRead;
            }
            set
            {
                _sendRead = value;
                RaisePropertyChanged(nameof(sendRead));
            }
        }

        /// <summary>
        /// 序列号，数值型
        /// </summary>
        [JsonIgnore]
        public int messageNo { get; set; }

        /// <summary>
        /// 是否已送达
        /// </summary>
        [JsonIgnore]
        public int isSend
        {
            get
            {
                return _isSend;
            }
            set
            {
                _isSend = value;
                RaisePropertyChanged(nameof(isSend));
            }
        }

        /// <summary>
        /// 是否已被对方阅读
        /// </summary>
        [JsonIgnore]
        public int isRead
        {
            get
            {
                return _isRead;
            }
            set
            {
                _isRead = value;
                RaisePropertyChanged(nameof(isRead));
            }
        }

        /// <summary>
        /// 是否下载成功
        /// </summary>
        [JsonIgnore]
        public int isReceive
        {
            get
            {
                return _isReceive;
            }
            set
            {
                _isReceive = value;
                RaisePropertyChanged(nameof(isReceive));
            }
        }

        /// <summary>
        /// 是否上传
        /// </summary>
        [JsonIgnore]
        public int isUpload
        {
            get
            {
                return _isUpload;
            }
            set
            {
                _isUpload = value;
                RaisePropertyChanged(nameof(isUpload));
            }
        }

        /// <summary>
        /// 收到回执的时间
        /// </summary>
        [JsonIgnore]
        public long timeReceive
        {
            get
            {
                return _timeReceive;
            }
            set
            {
                _timeReceive = value;
                RaisePropertyChanged(nameof(timeReceive));
            }
        }

        /// <summary>
        /// 文件内容
        /// </summary>
        [JsonIgnore]
        public byte FileData { get; set; }

        /// <summary>
        /// 是否是自己发送
        /// </summary>
        [JsonIgnore]
        public int isMySend
        {
            get
            {
                if (fromUserId == myUserId)
                {
                    if (this.PlatformType > 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 是否群聊，发送前赋值
        /// </summary>
        [JsonIgnore]
        public int isGroup
        {
            get
            {
                if (jid == null)
                {
                    return 0;
                }
                else
                {
                    return jid.Length > 15 ? 1 : 0;
                }
            }
            set
            {
                RaisePropertyChanged(nameof(isGroup));
            }
        }

        /// <summary>
        /// 是否展示时间
        /// </summary>
        [JsonIgnore]
        public int isShowTime
        {
            get
            {
                return _isShowTime;
            }
            set
            {
                _isShowTime = value;
                RaisePropertyChanged(nameof(isShowTime));
            }
        }

        /// <summary>
        /// 聊天图片的长度
        /// </summary>
        [JsonIgnore]
        public int imageWidth { get; set; }

        /// <summary>
        /// 聊天图片的高度
        /// </summary>
        [JsonIgnore]
        public int imageHeight { get; set; }

        /// <summary>
        /// [这个暂时不知道用途.]
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, string> dictionary { get; set; }

        /// <summary>
        /// progress
        /// </summary>
        [JsonIgnore]
        public double progress { get; set; }

        /// <summary>
        /// index
        /// </summary>
        [JsonIgnore]
        public int index { get; set; }

        /// <summary>
        /// 没收到回执时,自动重发次数
        /// </summary>
        [JsonIgnore]
        public int sendCount
        {
            get
            {
                return _sendCount;
            }
            set
            {
                _sendCount = value;
                RaisePropertyChanged(nameof(sendCount));
            }
        }

        /// <summary>
        /// 消息的平台类型
        /// <para>
        /// 1为android 2为ios 3为web 4为mac 5为pc
        /// </para>
        /// <para>如果此属性>=0时，表示当前消息为多点登录时同账号设备间转发消息</para>
        /// </summary>
        [JsonIgnore]
        public int PlatformType
        {
            get { return _platformType; }
            set { _platformType = value; RaisePropertyChanged(nameof(PlatformType)); }
        }
        #endregion

        #region Public Methods

        #region 序列化
        /// <summary>
        /// 转当前对象为Json字符串
        /// </summary>
        /// <returns>Json字符串</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);//序列化
        }
        #endregion

        #region 反序列化
        public Messageobject toModel(string MsgJson)
        {
            Messageobject msgObj = JsonConvert.DeserializeObject<Messageobject>(MsgJson);
            return msgObj;
        }
        #endregion

        #region 复制对象
        public Messageobject Clone()
        {
            Messageobject msgObj = new Messageobject()
            {
                type = this.type,
                timeSend = this.timeSend,
                messageId = this.messageId,
                timeReceive = this.timeReceive,
                fromUserId = this.fromUserId,
                fromUserName = this.fromUserName,
                toUserId = this.toUserId,
                toUserName = this.toUserName,
                content = this.content,
                filePath = this.filePath,
                fileName = this.fileName,
                location_y = this.location_y,
                location_x = this.location_x,
                sendRead = this.sendRead,
                isUpload = this.isUpload,
                isDownload = this.isDownload,
                messageState = this.messageState,
                timeLen = this.timeLen,
                fileSize = this.fileSize,
                objectId = this.objectId,
                sipStatus = this.sipStatus,
                sipDuration = this.sipDuration,
                isReadDel = this.isReadDel,
                isEncrypt = this.isEncrypt,
                reSendCount = this.reSendCount,
                readPersons = this.readPersons,
                readTime = this.readTime,
                FromId = this.FromId,
                ToId = this.ToId,
                PlatformType = this.PlatformType,
            };
            return msgObj;
        }
        #endregion

        #region 是否存在该表
        /// <summary>
        /// 是否存在表
        /// </summary>
        /// <param name="TabName">表名</param>
        /// <returns></returns>
        public bool ExistsTable()
        {
            lock (Applicate.AccountDbContext)
            {
                if (string.IsNullOrWhiteSpace(this.ToId) || string.IsNullOrWhiteSpace(this.FromId))
                {
                    return false;
                }
                string TabName = Prefex + (this.jid ?? ((this.myUserId == this.FromId) ? this.ToId : this.FromId));
                return DbHelperSQLite.GetTable(TabName);
            }
        }
        #endregion

        #region 创建表
        /// <summary>
        /// 创建表
        /// </summary>
        public bool CreatTable()
        {
            lock (Applicate.AccountDbContext)
            {
                if (string.IsNullOrWhiteSpace(this.ToId) || string.IsNullOrWhiteSpace(this.FromId))
                {
                    return false;
                }
                if (ExistsTable())
                {
                    return true;
                }
                string TabName = Prefex + (this.jid ?? ((this.myUserId == this.FromId) ? this.ToId : this.FromId));
                #region Add the fields
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(@"CREATE TABLE {0} (", TabName);
                strSql.AppendFormat(" {0} VARCHAR PRIMARY KEY NOT NULL,", nameof(messageId));
                strSql.AppendFormat(" {0} INTEGER NOT NULL,", nameof(type));
                strSql.AppendFormat(" {0} INTEGER NOT NULL,", nameof(timeSend));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(jid));
                strSql.AppendFormat(" {0} INTEGER,", nameof(timeReceive));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(fromUserId));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(fromUserName));
                strSql.AppendFormat(" {0} INTEGER,", nameof(fromUserType));
                strSql.AppendFormat(" {0} INTEGER,", nameof(toUserType));
                strSql.AppendFormat(" {0} INTEGER,", nameof(imageHeight));
                strSql.AppendFormat(" {0} INTEGER,", nameof(imageWidth));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(toUserId));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(toUserName));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isMySend));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isDelay));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isGroup));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isRead));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isReadDel));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isReceive));
                strSql.AppendFormat(" {0} INTEGER,", nameof(messageNo));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isSend));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isShowTime));
                strSql.AppendFormat(" {0} INTEGER,", "[" + nameof(index) + "]");
                strSql.AppendFormat(" {0} DOUBLE,", nameof(progress));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(content));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(filePath));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(fileName));
                strSql.AppendFormat(" {0} DOUBLE,", nameof(location_y));
                strSql.AppendFormat(" {0} DOUBLE,", nameof(location_x));
                strSql.AppendFormat(" {0} INTEGER,", nameof(sendRead));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isUpload));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isDownload));
                strSql.AppendFormat(" {0} INTEGER, ", nameof(messageState));
                strSql.AppendFormat(" {0} INTEGER, ", nameof(timeLen));
                strSql.AppendFormat(" {0} INTEGER,", nameof(fileSize));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(objectId));
                strSql.AppendFormat(" {0} INTEGER,", nameof(sipStatus));
                strSql.AppendFormat(" {0} INTEGER,", nameof(sipDuration));
                strSql.AppendFormat(" {0} INTEGER,", nameof(isEncrypt));
                strSql.AppendFormat(" {0} INTEGER,", nameof(reSendCount));
                strSql.AppendFormat(" {0} INTEGER,", nameof(sendCount));
                strSql.AppendFormat(" {0} INTEGER,", nameof(readPersons));
                strSql.AppendFormat(" {0} INTEGER,", nameof(readTime));
                strSql.AppendFormat(" {0} VARCHAR,", nameof(FromId));
                strSql.AppendFormat(" {0} VARCHAR)", nameof(ToId));
                #endregion
                //bool obj = (DbHelperSQLite.ExecuteSql(strSql) > 0);用下列方法即可
                DbHelperSQLite.ExecuteSql(strSql.ToString());
                return true;
            }
        }
        #endregion

        /*********************Database Storage*********************/

        #region 更新对应UserId的消息昵称
        /// <summary>
        /// 更新对应UserId的消息昵称
        /// </summary>
        /// <param name="targetJid">对话名称</param>
        /// <param name="userId">需要修改的UserId</param>
        /// <param name="nickname">修改的昵称</param>
        public int UpdateNicknameByUserId(string targetJid, string userId, string nickname)
        {
            StringBuilder strb = new StringBuilder();
            strb.Append("update " + Prefex + targetJid);//拼接表名
            strb.Append(string.Format(" set fromUserName = @nickname where fromUserId = @userid"));
            //SQL参数
            SQLiteParameter[] parameters = {
                                new SQLiteParameter("@nickname",DbType.String),
                                new SQLiteParameter("@userid",DbType.String)
                            };
            parameters[0].Value = nickname;
            parameters[1].Value = userId;
            int rows = DbHelperSQLite.ExecuteSql(strb.ToString(), parameters);
            return rows;
        }
        #endregion

        #region 将当前消息存入数据库
        /// <summary>
        /// 将当前消息存入数据库
        /// </summary>
        public int Insert()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.ToId : this.FromId));
                    #region SQL语句赋值 
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into " + TabName + "(");
                    strSql.AppendFormat(@"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},
                                            {21},{22},{23},{24},{25},{26},{27},{28},{29},{30})",
                                                nameof(type), nameof(timeSend), nameof(messageId), nameof(jid), nameof(timeReceive),
                                                nameof(fromUserId), nameof(fromUserName), nameof(toUserId), nameof(toUserName),
                                                nameof(isMySend), nameof(content), nameof(filePath), nameof(fileName), nameof(location_y),
                                                nameof(location_x), nameof(sendRead), nameof(isUpload), nameof(isDownload), nameof(messageState),
                                                nameof(timeLen), nameof(fileSize), nameof(objectId), nameof(sipStatus), nameof(sipDuration), nameof(isReadDel),
                                                nameof(isEncrypt), nameof(reSendCount), nameof(readPersons), nameof(readTime), nameof(FromId), nameof(ToId)
                                    );
                    strSql.Append(" values (");
                    strSql.Append("@type,@timeSend,@messageId,@jid,@timeReceive,@fromUserId,@fromUserName,@toUserId,@toUserName,@isMySend,@content,@filePath,@fileName,@location_y,@location_x,@sendRead,@isUpload,@isDownload,@messageState,@timeLen,@fileSize,@objectId,@sipStatus,@sipDuration,@isReadDel,@isEncrypt,@reSendCount,@readPersons,@readTime,@FromId,@ToId)");
                    strSql.Append(";select LAST_INSERT_ROWID()");
                    SQLiteParameter[] parameters = {
                    new SQLiteParameter("@type", DbType.Int32,8),
                    new SQLiteParameter("@timeSend", DbType.Int32,8),
                    new SQLiteParameter("@messageId", DbType.String,2147483647),
                    new SQLiteParameter("@jid",DbType.String,2147483647),
                    new SQLiteParameter("@timeReceive", DbType.Int32, 8),
                    new SQLiteParameter("@fromUserId", DbType.String, 2147483647),
                    new SQLiteParameter("@fromUserName", DbType.String, 2147483647),
                    new SQLiteParameter("@toUserId", DbType.String, 2147483647),
                    new SQLiteParameter("@toUserName", DbType.String, 2147483647),
                    new SQLiteParameter("@isMySend", DbType.Int32, 2),
                    new SQLiteParameter("@content", DbType.String, 2147483647),
                    new SQLiteParameter("@filePath", DbType.String, 2147483647),
                    new SQLiteParameter("@fileName", DbType.String, 2147483647),
                    new SQLiteParameter("@location_y", DbType.Double, 2147483647),
                    new SQLiteParameter("@location_x", DbType.Double, 2147483647),
                    new SQLiteParameter("@sendRead", DbType.Int32, 2),
                    new SQLiteParameter("@isUpload", DbType.Int32, 2),
                    new SQLiteParameter("@isDownload", DbType.Int32, 2),
                    new SQLiteParameter("@messageState", DbType.Int32, 8),
                    new SQLiteParameter("@timeLen", DbType.Int32, 8),
                    new SQLiteParameter("@fileSize", DbType.Int32, 8),
                    new SQLiteParameter("@objectId", DbType.String, 2147483647),
                    new SQLiteParameter("@sipStatus", DbType.Int32, 8),
                    new SQLiteParameter("@sipDuration", DbType.Int32, 8),
                    new SQLiteParameter("@isReadDel", DbType.Int32, 8),
                    new SQLiteParameter("@isEncrypt", DbType.Int32, 8),
                    new SQLiteParameter("@reSendCount", DbType.Int32, 8),
                    new SQLiteParameter("@readPersons", DbType.Int32, 8),
                    new SQLiteParameter("@readTime", DbType.Int32, 8),
                    new SQLiteParameter("@FromId", DbType.String, 2147483647),
                    new SQLiteParameter("@ToId", DbType.String, 2147483647) };
                    parameters[0].Value = this.type;
                    parameters[1].Value = this.timeSend;
                    parameters[2].Value = this.messageId;
                    parameters[3].Value = this.jid;
                    parameters[4].Value = this.timeReceive;
                    parameters[5].Value = this.fromUserId;
                    parameters[6].Value = this.fromUserName;
                    parameters[7].Value = this.toUserId;
                    parameters[8].Value = this.toUserName;
                    parameters[9].Value = this.isMySend;
                    parameters[10].Value = this.content;
                    parameters[11].Value = this.filePath;
                    parameters[12].Value = this.fileName;
                    parameters[13].Value = this.location_y;
                    parameters[14].Value = this.location_x;
                    parameters[15].Value = this.sendRead;
                    parameters[16].Value = this.isUpload;
                    parameters[17].Value = this.isDownload;
                    parameters[18].Value = this.messageState;
                    parameters[19].Value = this.timeLen;
                    parameters[20].Value = this.fileSize;
                    parameters[21].Value = this.objectId;
                    parameters[22].Value = this.sipStatus;
                    parameters[23].Value = this.sipDuration;
                    parameters[24].Value = this.isReadDel;
                    parameters[25].Value = this.isEncrypt;
                    parameters[26].Value = this.reSendCount;
                    parameters[27].Value = this.readPersons;
                    parameters[28].Value = this.readTime;
                    parameters[29].Value = this.FromId;
                    parameters[30].Value = this.ToId;
                    #endregion
                    object obj = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (obj == null)
                    {
                        ConsoleLog.Output("数据库写入失败");
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt32(obj);
                    }
                }
                return 0;
            }
        }
        #endregion

        #region 修改消息类型
        /// <summary>
        /// 修改消息类型
        /// </summary>
        /// <param name="msgId">消息唯一Id</param>
        /// <param name="type">消息类型</param>
        /// <returns>是否成功</returns>
        public bool UpdateMessageType(string msgId, kWCMessageType type)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("type=@type");
                    strSql.Append(" where messageId=@messageId");
                    //SQL参数
                    SQLiteParameter[] parameters = {
                                new SQLiteParameter("@type",type),
                                new SQLiteParameter("@messageId",msgId)
                            };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows < 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 更新消息内容
        /// <summary>
        /// 更新消息内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="messageId">消息对应的Id</param>
        /// <returns>是否执行成功</returns>
        public bool UpdateMessageContent(string msgId, string content)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("content=@content");
                    strSql.Append(" where messageId=@messageId");
                    //SQL参数
                    SQLiteParameter[] parameters = {
                            new SQLiteParameter("@content", content),
                            new SQLiteParameter("@messageId",msgId)
                        };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows < 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 更新消息状态
        /// <summary>
        /// 更新消息状态
        /// </summary>
        /// <param name="messageId">消息唯一Id</param>
        /// <param name="msgState">消息状态</param>
        public bool UpdateMessageState()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("messageState=@messageState");
                    strSql.Append(" where messageId=@messageId");
                    //SQL参数
                    SQLiteParameter[] parameters = {
                    new SQLiteParameter("@messageState", this.messageState),
                    new SQLiteParameter("@messageId",this.messageId)
                };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 更新送达
        /// <summary>
        /// 更新送达
        /// </summary>
        /// <param name="messageId">消息唯一Id</param>
        /// <param name="msgState">消息状态</param>
        public bool UpdateReceived(string messageId = null)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    if (!String.IsNullOrWhiteSpace(messageId))
                    {
                        this.messageId = messageId;
                    }

                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("messageState=@messageState");
                    strSql.Append(" where messageId=@messageId");
                    //SQL参数
                    SQLiteParameter[] parameters =
                     {
                    new SQLiteParameter("@messageState", 1),
                    new SQLiteParameter("@messageId",this.messageId)
                 };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 更新已读
        /// <summary>
        /// 更新已读
        /// </summary>
        /// <param name="messageId">消息唯一Id</param>
        /// <param name="msgState">消息状态</param>
        public bool UpdateIsRead(string msgId)
        {
            lock (Applicate.AccountDbContext)
            {
                string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update " + TabName + " set ");
                strSql.Append("messageState=@messageState");
                strSql.Append(", isRead=@isRead");
                strSql.Append(" where messageId=@messageId");
                //SQL参数
                SQLiteParameter[] parameters = {
                new SQLiteParameter("@messageState", "2"),
                new SQLiteParameter("@isRead","1"),
                new SQLiteParameter("@messageId",msgId),
            };
                int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                if (rows >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region 更新已读
        /// <summary>
        /// 更新已读
        /// </summary>
        /// <param name="messageId">消息唯一Id</param>
        /// <param name="msgState">消息状态</param>
        public bool UpdateIsReadPersons(string msgId)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("messageState=@messageState");
                    strSql.Append(",readPersons=case when readPersons is null then 1 else readPersons+1 end");
                    strSql.Append(" where messageId=@messageId");
                    //SQL参数
                    SQLiteParameter[] parameters = {
                new SQLiteParameter("@messageState", 2),
                new SQLiteParameter("@messageId",msgId)
            };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 更新撤回
        /// <summary>
        /// 更新撤回
        /// </summary>
        /// <param name="messageId">消息唯一Id</param>
        /// <param name="msgState">消息状态</param>
        public bool UpdateIsReCall()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("messageState=@messageState");
                    strSql.Append(" where messageId=@messageId");
                    //SQL参数
                    SQLiteParameter[] parameters = {
                new SQLiteParameter("@messageState", -1),
                new SQLiteParameter("@messageId",this.messageId)
            };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 获取消息状态
        /// <summary>
        /// 获取消息状态
        /// </summary>
        /// <param name="messageId">需要查询的MsgID</param>
        /// <returns>状态码(0为正在发送，1为已送达，2为已读)</returns>
        public int GetMessageState()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(" select messageState from " + TabName);
                    strSql.Append(" where messageId='" + this.messageId + "'");
                    return DbHelperSQLite.GetSingle<int>(strSql.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region 更新一条消息
        /// <summary>
        /// 更新一条消息
        /// </summary>
        public bool Update()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update " + TabName + " set ");
                    strSql.Append("type=@type,");
                    strSql.Append("timeSend=@timeSend,");
                    strSql.Append("jid=@jid,");
                    strSql.Append("timeReceive=@timeReceive,");
                    strSql.Append("fromUserId=@fromUserId,");
                    strSql.Append("fromUserName=@fromUserName,");
                    strSql.Append("toUserId=@toUserId,");
                    strSql.Append("toUserName=@toUserName,");
                    strSql.Append("isMySend=@isMySend,");
                    strSql.Append("content=@content,");
                    strSql.Append("filePath=@filePath,");
                    strSql.Append("location_y=@location_y,");
                    strSql.Append("location_x=@location_x,");
                    strSql.Append("sendRead=@sendRead,");
                    strSql.Append("isUpload=@isUpload,");
                    strSql.Append("isDownload=@isDownload,");
                    strSql.Append("messageState=@messageState,");
                    strSql.Append("timeLen=@timeLen,");
                    strSql.Append("fileSize=@fileSize,");
                    strSql.Append("objectId=@objectId,");
                    strSql.Append("sipStatus=@sipStatus,");
                    strSql.Append("sipDuration=@sipDuration,");
                    strSql.Append("isReadDel=@isReadDel,");
                    strSql.Append("isEncrypt=@isEncrypt,");
                    strSql.Append("reSendCount=@reSendCount,");
                    strSql.Append("readPersons=@readPersons,");
                    strSql.Append("readTime=@readTime,");
                    strSql.Append("FromId=@FromId,");
                    strSql.Append("ToId=@ToId");
                    strSql.Append(" where messageId=@messageId");
                    SQLiteParameter[] parameters = {
                    new SQLiteParameter("@type", DbType.Int32,8),
                    new SQLiteParameter("@timeSend", DbType.Int32,8),
                    new SQLiteParameter("@jid",DbType.String,2147483647),
                    new SQLiteParameter("@timeReceive", DbType.Int32,8),
                    new SQLiteParameter("@fromUserId", DbType.String,2147483647),
                    new SQLiteParameter("@fromUserName", DbType.String,2147483647),
                    new SQLiteParameter("@toUserId", DbType.String,2147483647),
                    new SQLiteParameter("@toUserName", DbType.String,2147483647),
                    new SQLiteParameter("@isMySend", DbType.Int32,2),
                    new SQLiteParameter("@content", DbType.String,2147483647),
                    new SQLiteParameter("@filePath", DbType.String,2147483647),
                    new SQLiteParameter("@location_y", DbType.String,2147483647),
                    new SQLiteParameter("@location_x", DbType.String,2147483647),
                    new SQLiteParameter("@sendRead", DbType.Int32,2),
                    new SQLiteParameter("@isUpload", DbType.Int32,2),
                    new SQLiteParameter("@isDownload", DbType.Int32,2),
                    new SQLiteParameter("@messageState", DbType.Int32,8),
                    new SQLiteParameter("@timeLen", DbType.Int32,8),
                    new SQLiteParameter("@fileSize", DbType.Int32,8),
                    new SQLiteParameter("@objectId", DbType.String,2147483647),
                    new SQLiteParameter("@sipStatus", DbType.Int32,8),
                    new SQLiteParameter("@sipDuration", DbType.Int32,8),
                    new SQLiteParameter("@isReadDel", DbType.Int32,8),
                    new SQLiteParameter("@isEncrypt", DbType.Int32,8),
                    new SQLiteParameter("@reSendCount", DbType.Int32,8),
                    new SQLiteParameter("@readPersons", DbType.Int32,8),
                    new SQLiteParameter("@readTime", DbType.Int32,8),
                    new SQLiteParameter("@FromId",DbType.String,2147483647),
                    new SQLiteParameter("@ToId",DbType.String,2147483647),
                    new SQLiteParameter("@messageId", DbType.String,2147483647)};
                    parameters[0].Value = this.type;
                    parameters[1].Value = this.timeSend;
                    //parameters[2].Value = model.messageId;
                    parameters[2].Value = this.jid;
                    parameters[3].Value = this.timeReceive;
                    parameters[4].Value = this.fromUserId;
                    parameters[5].Value = this.fromUserName;
                    parameters[6].Value = this.toUserId;
                    parameters[7].Value = this.toUserName;
                    parameters[8].Value = this.isMySend;
                    parameters[9].Value = this.content;
                    parameters[10].Value = this.filePath;
                    parameters[11].Value = this.location_y;
                    parameters[12].Value = this.location_x;
                    parameters[13].Value = this.sendRead;
                    parameters[14].Value = this.isUpload;
                    parameters[15].Value = this.isDownload;
                    parameters[16].Value = this.messageState;
                    parameters[17].Value = this.timeLen;
                    parameters[18].Value = this.fileSize;
                    parameters[19].Value = this.objectId;
                    parameters[20].Value = this.sipStatus;
                    parameters[21].Value = this.sipDuration;
                    parameters[22].Value = this.isReadDel;
                    parameters[23].Value = this.isEncrypt;
                    parameters[24].Value = this.reSendCount;
                    parameters[25].Value = this.readPersons;
                    parameters[26].Value = this.readTime;
                    parameters[27].Value = this.FromId;
                    parameters[28].Value = this.ToId;
                    parameters[29].Value = this.messageId;
                    //parameters[26].Value = model._id;

                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.FromId) ? this.ToId : this.FromId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("delete from " + TabName);
                    strSql.Append(" where messageId=@messageId");
                    SQLiteParameter[] parameters = {
                            new SQLiteParameter("@messageId", DbType.String,2147483647)
                    };
                    parameters[0].Value = this.messageId;
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 得到一个对象实体
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Messageobject GetModel()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"select * from " + TabName);
                    strSql.Append(" where messageId=@messageId");
                    SQLiteParameter[] parameters = {
                    new SQLiteParameter("@messageId", DbType.String,2147483647)
            };
                    parameters[0].Value = this.messageId;

                    DataSet ds = DbHelperSQLite.Query(strSql.ToString(), parameters);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return DataRowToModel(ds.Tables[0].Rows[0]);
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
        }
        #endregion

        #region DataRowToModel
        /// <summary>
        /// DataRowToModel
        /// </summary>
        public Messageobject DataRowToModel(DataRow row)
        {
            if (CreatTable())
            {
                string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                Messageobject model = new Messageobject();
                if (row != null)
                {
                    if (row["type"] != null && row["type"].ToString() != "")
                    {
                        model.type = (kWCMessageType)Int32.Parse(row["type"].ToString());
                    }
                    if (row["timeSend"] != null && row["timeSend"].ToString() != "")
                    {
                        model.timeSend = int.Parse(row["timeSend"].ToString());
                    }
                    if (row["messageId"] != null)
                    {
                        model.messageId = row["messageId"].ToString();
                    }
                    if (row["timeReceive"] != null && row["timeReceive"].ToString() != "")
                    {
                        model.timeReceive = int.Parse(row["timeReceive"].ToString());
                    }
                    if (row["fromUserId"] != null)
                    {
                        model.fromUserId = row["fromUserId"].ToString();
                    }
                    if (row["fromUserName"] != null)
                    {
                        model.fromUserName = row["fromUserName"].ToString();
                    }
                    if (row["toUserId"] != null)
                    {
                        model.toUserId = row["toUserId"].ToString();
                    }
                    if (row["toUserName"] != null)
                    {
                        model.toUserName = row["toUserName"].ToString();
                    }
                    if (row["content"] != null)
                    {
                        model.content = row["content"].ToString();
                    }
                    if (row["filePath"] != null)
                    {
                        model.filePath = row["filePath"].ToString();
                    }
                    if (row["fileName"] != null)
                    {
                        model.fileName = row["fileName"].ToString();
                    }
                    if (row["location_y"] != null && row["location_y"].ToString() != "")
                    {
                        var locationy = row["location_y"].ToString();
                        model.location_y = Convert.ToDouble(locationy);
                    }
                    if (row["location_x"] != null && row["location_x"].ToString() != "")
                    {
                        model.location_x = Convert.ToDouble(row["location_x"].ToString());
                    }
                    if (row["sendRead"] != null && row["sendRead"].ToString() != "")
                    {
                        model.sendRead = int.Parse(row["sendRead"].ToString());
                    }
                    if (row["isUpload"] != null && row["isUpload"].ToString() != "")
                    {
                        model.isUpload = int.Parse(row["isUpload"].ToString());
                    }
                    if (row["isDownload"] != null && row["isDownload"].ToString() != "")
                    {
                        model.isDownload = row["isDownload"].ToString() == "1" ? true : false;
                    }
                    if (row["messageState"] != null && row["messageState"].ToString() != "")
                    {
                        model.messageState = int.Parse(row["messageState"].ToString());
                    }
                    if (row["timeLen"] != null && row["timeLen"].ToString() != "")
                    {
                        model.timeLen = int.Parse(row["timeLen"].ToString());
                    }
                    if (row["fileSize"] != null && row["fileSize"].ToString() != "")
                    {
                        model.fileSize = int.Parse(row["fileSize"].ToString());
                    }
                    if (row["objectId"] != null)
                    {
                        model.objectId = row["objectId"].ToString();
                    }
                    if (row["sipStatus"] != null && row["sipStatus"].ToString() != "")
                    {
                        model.sipStatus = row["sipStatus"].ToString() == "1" ? true : false;
                    }
                    if (row["sipDuration"] != null && row["sipDuration"].ToString() != "")
                    {
                        model.sipDuration = int.Parse(row["sipDuration"].ToString());
                    }
                    if (row["isReadDel"] != null && row["isReadDel"].ToString() != "")
                    {
                        model.isReadDel = row["isReadDel"].ToString() == "1" ? true : false;
                    }
                    if (row["isEncrypt"] != null && row["isEncrypt"].ToString() != "")
                    {
                        model.isEncrypt = Convert.ToInt16(row["isEncrypt"].ToString());
                    }
                    if (row["reSendCount"] != null && row["reSendCount"].ToString() != "")
                    {
                        model.reSendCount = int.Parse(row["reSendCount"].ToString());
                    }
                    if (row["readPersons"] != null && row["readPersons"].ToString() != "")
                    {
                        model.readPersons = row["readPersons"].ToString();
                    }
                    if (row["readTime"] != null && row["readTime"].ToString() != "")
                    {
                        model.readTime = int.Parse(row["readTime"].ToString());
                    }
                    if (row["FromId"] != null)
                    {
                        model.FromId = row["FromId"].ToString();
                    }
                    if (row["ToId"] != null)
                    {
                        model.ToId = row["ToId"].ToString();
                    }
                }
                return model;
            }
            return null;
        }
        #endregion

        #region 获得所有表
        /// <summary>
        /// 获得所有表
        /// </summary>
        public DataSet GetAllTables()
        {
            lock (Applicate.AccountDbContext)
            {
                string strSql = "select * from sqlite_master where type='table'";
                return DbHelperSQLite.Query(strSql);
            }
        }
        #endregion

        #region 清空表数据
        /// <summary>
        /// 清空表数据
        /// </summary>
        public bool DeleteTable(string tab)
        {
            lock (Applicate.AccountDbContext)
            {
                string strSql = string.Format("delete from {0}", tab);
                int rows = DbHelperSQLite.ExecuteSql(strSql);
                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region 获取记录总数
        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("select count(1) FROM " + TabName);
                    if (strWhere.Trim() != "")
                    {
                        strSql.Append(" where " + strWhere);
                    }
                    return DbHelperSQLite.GetSingle<int>(strSql.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region 获取记录总数
        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string[] content, string strWhere)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    List<SQLiteParameter> parameters = new List<SQLiteParameter>();
                    strSql.Append("select count(1) FROM " + TabName);
                    strSql.Append(" WHERE 1=1 ");
                    if (strWhere.Trim() != "")
                    {
                        strSql.Append(" AND " + strWhere);
                    }
                    if (content.Length > 0)
                    {
                        for (int i = 0; i < content.Length; i++)
                        {
                            strSql.Append(i == 0 ? " and content like @content" + i : " or content like @content" + i);
                            parameters.Add(new SQLiteParameter("@content" + i, DbType.String, 2147483647) { Value = "%" + content[i].Trim() + "%" });
                        }
                    };
                    return DbHelperSQLite.GetSingle<int>(strSql.ToString(), parameters.ToArray());
                }
                return 0;
            }
        }
        #endregion

        #region 获取记录总数
        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount2(string strWhere)
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.FromId) ? this.ToId : this.FromId));
                    StringBuilder strSql = new StringBuilder();
                    List<SQLiteParameter> parameters = new List<SQLiteParameter>();
                    strSql.Append("select count(1) FROM " + TabName);
                    strSql.Append(" WHERE 1=1 ");
                    if (strWhere.Trim() != "")
                    {
                        strSql.Append(" AND " + strWhere);
                    }
                    return DbHelperSQLite.GetSingle<int>(strSql.ToString(), parameters.ToArray());
                }
                return 0;
            }
        }
        #endregion

        #region 获取和对应用户聊天记录的时间戳
        /// <summary>
        /// 获取对应用户聊天记录的时间戳
        /// </summary>
        /// <param name="UserId">UserId</param>
        public long GetLastTimeStamp()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder sb = new StringBuilder();
                    sb.Append("select MAX(timesend) from ");
                    sb.Append(TabName);
                    return DbHelperSQLite.GetSingle<long>(sb.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region 获得数据列表
        /// <summary>
        /// 获得数据列表
        /// </summary>
        /// <param name="dt">DataSet对象</param>
        /// <returns>对应的数据列表</returns>
        public List<Messageobject> DataTableToList(DataTable dt)
        {
            lock (Applicate.AccountDbContext)
            {
                List<Messageobject> modelList = new List<Messageobject>();
                int rowsCount = dt.Rows.Count;
                if (rowsCount > 0)
                {
                    Messageobject msgModel;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        msgModel = DataRowToModel(dt.Rows[n]);
                        if (msgModel != null)
                        {
                            modelList.Add(msgModel);
                        }
                    }
                }
                return modelList;
            }
        }
        #endregion

        #region 分页获取数据列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">获取对应类型的消息(小于零时获取全部)</param>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<Messageobject> GetList(int type, ref int PageNum, int PageSize, string content)
        {
            lock (Applicate.AccountDbContext)
            {
                StringBuilder strSql = new StringBuilder();
                string tabname = Prefex + this.ToId;
                strSql.Append("SELECT *  FROM " + tabname + " as tt WHERE 1=1");
                if (type > 0)
                {
                    strSql.AppendFormat(" and type={0}", type);
                }
                string[] contents = content.Split(' ');
                List<SQLiteParameter> parameters = new List<SQLiteParameter>();
                if (contents.Length > 0)
                {
                    for (int i = 0; i < contents.Length; i++)
                    {
                        strSql.Append(i == 0 ? " and content like @content" + i : " or content like @content" + i);
                        parameters.Add(new SQLiteParameter("@content" + i, DbType.String, 2147483647) { Value = "%" + contents[i].Trim() + "%" });
                    }
                };
                strSql.Append(" order by timeSend");
                var ds = DbHelperSQLite.Query(strSql.ToString(), parameters.ToArray());
                return DataTableToList(ds.Tables[0]);
            }
        }
        #endregion

        #region 分页获取数据列表
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        /// <param name="pageNum">目标页</param>
        /// <param name="order">排序方式</param>
        /// <param name="lastRowid">最后的RowId</param>
        /// <param name="PageSize">每页数据条数</param>
        /// <returns></returns>
        public List<Messageobject> GetPageList(int pageNum, int PageSize = 15)
        {
            if (ExistsTable())//检查表是否存在
            {
                lock (Applicate.AccountDbContext)
                {
                    var strSql = new StringBuilder();
                    if (PageSize > 0)
                    {
                        string tabname = Prefex + this.ToId;
                        int pagenums = pageNum * PageSize;
                        strSql.AppendFormat("select * from {0} order by timeSend DESC limit {1},{2}", tabname, pageNum, PageSize);
                    }
                    var reader = DbHelperSQLite.ExecuteReader(strSql.ToString());
                    return DataReaderToList(reader);
                }
            }
            else
            {
                return new List<Messageobject>();
            }
        }
        #endregion

        #region DataReader转List
        /// <summary>
        /// DataReader转List集合
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<Messageobject> DataReaderToList(SQLiteDataReader reader)
        {
            lock (Applicate.AccountDbContext)
            {
                var list = new List<Messageobject>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        #region Fill the properties
                        var msg = new Messageobject();
                        msg.content = Convert.ToString(reader[nameof(content)]);
                        //msg.FileData = Convert.ToByte(reader[nameof(FileData)]);
                        msg.fileName = Convert.ToString(reader[nameof(fileName)]);
                        msg.filePath = Convert.ToString(reader[nameof(filePath)]);
                        msg.fileSize = Convert.ToInt32(reader[nameof(fileSize)]);
                        msg.FromId = Convert.ToString(reader[nameof(FromId)]);
                        msg.fromUserId = Convert.ToString(reader[nameof(fromUserId)]);
                        msg.fromUserName = Convert.ToString(reader[nameof(fromUserName)]);
                        //msg.fromUserType = Convert.ToInt32(reader[nameof(fromUserType)]);
                        //msg.imageHeight = Convert.ToInt32(reader[nameof(imageHeight)]);
                        //msg.imageWidth = Convert.ToInt32(reader[nameof(imageWidth)]);
                        //msg.index = Convert.ToInt32(reader[nameof(index)]);
                        //msg.isDelay = Convert.ToInt32(reader[nameof(isDelay)]);
                        msg.isDownload = Convert.ToBoolean(reader[nameof(isDownload)]);
                        msg.isEncrypt = Convert.ToInt32(reader[nameof(isEncrypt)]);
                        var tmp = reader[nameof(isRead)];
                        msg.isRead = (tmp == DBNull.Value) ? (0) : (Convert.ToInt32(tmp));
                        //msg.isGroup = Convert.ToInt32(reader[nameof(isGroup)]);
                        //msg.isReadDel = Convert.ToBoolean(reader[nameof(isReadDel)]);
                        //msg.isReceive = Convert.ToInt32(reader[nameof(isReceive)]);
                        msg.isSend = (reader[nameof(isSend)] == DBNull.Value) ? (0) : ((int)reader[nameof(isSend)]);
                        //msg.isShowTime = Convert.ToInt32(reader[nameof(isShowTime)]);
                        msg.isUpload = Convert.ToInt32(reader[nameof(isUpload)]);
                        //msg.jid = Convert.ToString(reader[nameof(jid)]);
                        msg.location_x = (reader[nameof(location_x)] == DBNull.Value) ? (0) : (Convert.ToInt32(reader[nameof(location_x)]));
                        msg.location_y = (reader[nameof(location_y)] == DBNull.Value) ? (0) : (Convert.ToInt32(reader[nameof(location_y)]));
                        msg.messageId = Convert.ToString(reader[nameof(messageId)]);
                        //msg.messageNo = Convert.ToInt32(reader[nameof(messageNo)]);
                        msg.messageState = Convert.ToInt32(reader[nameof(messageState)]);
                        msg.objectId = Convert.ToString(reader[nameof(objectId)]);
                        //msg.progress = Convert.ToDouble(reader[nameof(progress)]);
                        msg.readPersons = Convert.ToString(reader[nameof(readPersons)]);//已读人数
                        msg.readTime = Convert.ToInt64(reader[nameof(readTime)]);
                        msg.reSendCount = Convert.ToInt32(reader[nameof(reSendCount)]);
                        //msg.sendCount = Convert.ToInt32(reader[nameof(sendCount)]);
                        msg.sendRead = Convert.ToInt32(reader[nameof(sendRead)]);
                        msg.sipDuration = Convert.ToInt32(reader[nameof(sipDuration)]);
                        msg.sipStatus = Convert.ToBoolean(reader[nameof(sipStatus)]);
                        msg.timeLen = Convert.ToInt32(reader[nameof(timeLen)]);
                        msg.timeReceive = Convert.ToInt64(reader[nameof(timeReceive)]);
                        msg.timeSend = Convert.ToInt64(reader[nameof(timeSend)]);
                        msg.ToId = Convert.ToString(reader[nameof(ToId)]);
                        msg.toUserId = Convert.ToString(reader[nameof(toUserId)]);
                        msg.toUserName = Convert.ToString(reader[nameof(toUserName)]);
                        //msg.toUserType = Convert.ToInt32(reader[nameof(toUserType)]);
                        msg.type = (kWCMessageType)Convert.ToInt32(reader[nameof(type)]);//消息类型
                        #endregion
                        list.Insert(0, msg);//Add to the list 
                    }
                }
                return list;
            }
        }
        #endregion

        #region 获取最近一条消息
        /// <summary>
        /// 获取最后一条消息
        /// </summary>
        /// <returns></returns>
        public Messageobject GetLastContent()
        {
            lock (Applicate.AccountDbContext)
            {
                if (CreatTable())
                {
                    string TabName = Prefex + (this.jid ?? ((this.myUserId == this.fromUserId) ? this.toUserId : this.fromUserId));
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT TOP 1 * ");
                    strSql.Append(" FROM " + TabName);
                    strSql.AppendFormat(" ORDER BY {0} DESC", nameof(timeSend));//按照降序排列
                    return DbHelperSQLite.GetSingle<Messageobject>(strSql.ToString());
                }
                else
                {
                    return new Messageobject();
                }
            }
        }
        #endregion

        #region 清空聊天记录
        public int ClearAllMessage()
        {
            lock (Applicate.AccountDbContext)
            {
                int i = 0;
                DataSet ds = GetAllTables();
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Columns.Contains("name"))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string tabName = dr["name"].ToString();
                            if (tabName.StartsWith(Prefex))
                            {
                                i = DeleteTable(tabName) ? i + 1 : i + 0;
                            }
                        }
                    }
                }
                return i;
            }
        }
        #endregion

        #endregion
    }
    #endregion

}
