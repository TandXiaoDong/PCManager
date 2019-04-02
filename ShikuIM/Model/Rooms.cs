using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ShikuIM.Model
{

    #region 外层
    /// <summary>
    /// 返回的群聊列表
    /// </summary>
    public class RtnRooms : JsonBase
    {
        #region 返回的房间列表
        public RtnRooms()
        {
            data = new List<Room>();
        }
        #endregion
        /// <summary>
        /// 房间列表
        /// </summary>
        public List<Room> data { get; set; }
    }
    #endregion

    #region 序列化的Room
    /// <summary>
    /// 返回的单个群聊
    /// </summary>
    public class JsonRoom : JsonBase
    {
        #region 群组消息
        public JsonRoom()
        {
            data = new Room();
        }
        #endregion

        /// <summary>
        /// 数据房间实体
        /// </summary>
        public Room data { get; set; }
    }
    #endregion

    #region 房间类
    /// <summary>
    /// 房间
    /// </summary>
    public class Room : ObservableObject
    {

        #region Private Member
        private List<string> tags;// 房间标签
        private ObservableCollection<DataofMember> _members;//
        private ObservableCollection<Notice> _notices;//公告列表
        private int _allowHostUpdate;
        private int _offlineNoPushMsg;
        private int _allowSendCard;
        private int _allowInviteFriend;
        private int _allowUploadFile;
        private int _allowConference;
        private int _allowSpeakCourse;
        private int _showMember;
        private string _nickname;
        private int _isNeedVerify;
        private int _showRead;
        private string _name;
        private string _desc;
        private int _isLook;
        private int _userSize;
        #endregion

        #region Public Member
        /// <summary>
        /// // 房间公告(最新公告)
        /// </summary>
        [NotMapped]
        public Notice notice { get; set; }// 房间公告(最新公告)

        /// <summary>
        /// 群公告
        /// </summary>
        public ObservableCollection<Notice> notices
        {
            get { return _notices; }
            set
            {
                _notices = value;
                if (_notices != null || _notices.Count != 0)
                {
                    notice = _notices.OrderByDescending(n => n.Time).FirstOrDefault();//经排序后选择第一个
                }
            }
        }

        /// <summary>
        /// 是否当前用户加入的群
        /// </summary>
        //public bool IsJoined { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public int areaId { get; set; }

        /// <summary>
        /// 语音通话标识符
        /// </summary>
        public string call { get; set; }

        /// <summary>
        /// 房间分类
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        public int cityId { get; set; }

        /// <summary>
        /// 国家Id
        /// </summary>
        public int countryId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public int createTime { get; set; }

        /// <summary>
        /// 房间描述
        /// </summary>
        public string desc
        {
            get { return _desc; }
            set { _desc = value; RaisePropertyChanged(nameof(desc)); }
        }

        /// <summary>
        /// 房间编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 群的id
        /// </summary>
        [Key]
        public string jid { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double longitude { get; set; }

        /// <summary>
        /// 最大成员数
        /// </summary>
        public int maxUserSize { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public int modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public int modifyTime { get; set; }

        /// <summary>
        /// 房间名称
        /// </summary>
        public string name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(nameof(name)); }
        }

        /// <summary>
        /// 创建者昵称
        /// </summary>
        public string nickname
        {
            get { return _nickname; }
            set { _nickname = value; RaisePropertyChanged(nameof(nickname)); }
        }

        /// <summary>
        /// 是否免打扰
        /// </summary>
        public int offlineNoPushMsg
        {
            get { return _offlineNoPushMsg; }
            set { _offlineNoPushMsg = value; RaisePropertyChanged(nameof(offlineNoPushMsg)); }
        }
        /// <summary>
        /// 是否可见   0为可见   1为不可见
        /// </summary>
        public int isLook
        {
            get { return _isLook; }
            set { _isLook = value; RaisePropertyChanged(nameof(isLook)); }
        }
        /// <summary>
        /// 加群是否需要通过验证  0：不要   1：要
        /// </summary>
        public int isNeedVerify
        {
            get { return _isNeedVerify; }
            set { _isNeedVerify = value; RaisePropertyChanged(nameof(isNeedVerify)); }
        }
        /// <summary>
        /// 允许发送名片 好友  1 允许  0  不允许
        /// </summary>
        public int allowSendCard
        {
            get { return _allowSendCard; }
            set { _allowSendCard = value; RaisePropertyChanged(nameof(allowSendCard)); }
        }
        /// <summary>
        /// 允许普通成员邀请好友  1 允许  0  不允许
        /// </summary>
        public int allowInviteFriend
        {
            get { return _allowInviteFriend; }
            set { _allowInviteFriend = value; RaisePropertyChanged(nameof(allowInviteFriend)); }
        }
        /// <summary>
        /// 是否允许群主修改 群属性
        /// </summary>
        public int allowHostUpdate
        {
            get { return _allowHostUpdate; }
            set { _allowHostUpdate = value; RaisePropertyChanged(nameof(allowHostUpdate)); }
        }
        /// <summary>
        /// 是否允许普通成员上传群共享文件
        /// </summary>
        public int allowUploadFile
        {
            get { return _allowUploadFile; }
            set { _allowUploadFile = value; RaisePropertyChanged(nameof(allowUploadFile)); }
        }
        /// <summary>
        /// 是否允许普通成员召开会议
        /// </summary>
        public int allowConference
        {
            get { return _allowConference; }
            set { _allowConference = value; RaisePropertyChanged(nameof(allowConference)); }
        }
        /// <summary>
        /// 是否允许普通成员发起讲课
        /// </summary>
        public int allowSpeakCourse
        {
            get { return _allowSpeakCourse; }
            set { _allowSpeakCourse = value; RaisePropertyChanged(nameof(allowSpeakCourse)); }
        }
        /// <summary>
        /// 显示群成员给 普通用户   1 显示  0  不显示
        /// </summary>
        public int showMember
        {
            get { return _showMember; }
            set { _showMember = value; RaisePropertyChanged(nameof(showMember)); }
        }
        /// <summary>
        /// 省份Id
        /// </summary>
        public int provinceId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int s { get; set; }

        /// <summary>
        /// 群主设置 群内消息是否发送已读 回执 显示数量
        /// </summary>
        public int showRead
        {
            get { return _showRead; }
            set { _showRead = value; RaisePropertyChanged(nameof(showRead)); }
        }



        /// <summary>
        /// 房间主题
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        /// 创建者Id
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 成员数量
        /// </summary>
        public int userSize
        {
            get { return _userSize; }
            set
            {
                _userSize = value;
                RaisePropertyChanged(nameof(userSize));
            }
        }

        /// <summary>
        /// 视频会议标识符
        /// </summary>
        public string videoMeetingNo { get; set; }

        /// <summary>
        /// 拿来放群公告
        /// </summary>
        public string detial { get; set; }

        /// <summary>
        /// 全群允许发言时间
        /// </summary>
        public long talkTime { get; set; }

        /// <summary>
        /// 群聊成员
        /// </summary>
        public ObservableCollection<DataofMember> members
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged(nameof(members));
                userSize = _members.Count;//赋值实际的用户数量
            }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Contructor
        /// </summary>
        public Room()
        {
            notices = new ObservableCollection<Notice>();
            members = new ObservableCollection<DataofMember>();
            tags = new List<string>();
            if (id == null || id == "")
            {
                //id = Guid.NewGuid().ToString("N");
            }
        }
        #endregion

        #region Methods

        #region 更新群名
        /// <summary>
        /// 根据Jid修改群名称
        /// </summary>
        /// <param name="name"></param>
        public void UpdateNameByJid(string name)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                Room tmp = (
                    from rom in Applicate.AccountDbContext.Rooms
                    where rom.jid == this.jid
                    select rom
                    ).FirstOrDefault();
                if (tmp != null)//判断非空并赋值
                {
                    tmp.name = name;
                    Applicate.AccountDbContext.Entry(tmp).Property(r => r.name).IsModified = true;//设置单字段更新
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region 更新公告至数据库(By Id)
        /// <summary>
        /// 更新公告至数据库(By Id)
        /// </summary>
        /// <param name="desc">公告</param>
        public void UpdateDescById(string desc)
        {
            lock (Applicate.AccountDbContext)
            {
                try
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                        from rom in Applicate.AccountDbContext.Rooms
                        where rom.id == this.id
                        select rom
                        ).FirstOrDefault();
                    if (result != null)
                    {
                        result.desc = desc;
                        Applicate.AccountDbContext.Entry(result).Property(r => r.desc).IsModified = true;//设置单字段更新
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region 更新公告至数据库(By Jid)
        /// <summary>
        /// 更新公告至数据库(By Jid)
        /// </summary>
        /// <param name="desc">公告</param>
        public void UpdateDescByJid(string desc)
        {
            lock (Applicate.AccountDbContext)
            {
                try
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                        from rom in Applicate.AccountDbContext.Rooms
                        where rom.jid == this.jid
                        select rom
                        ).FirstOrDefault();
                    result.desc = desc;
                    Applicate.AccountDbContext.Entry(result).Property(r => r.desc).IsModified = true;//设置单字段更新
                    Applicate.AccountDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region 分页获取好友
        public List<Room> GetByPage(int pageindex = 0, int take = 30)
        {
            lock (Applicate.AccountDbContext)
            {
                int skip = pageindex * take;
                var Lists = Applicate.AccountDbContext.Rooms.OrderBy(r => r.name).ToList()
                    .Skip(skip).Take(take).ToList();//分页按
                return Lists;
            }
        }
        #endregion

        #region 更新群聊名称
        public void UpdateRoomNameByRoomJid()
        {
            lock (Applicate.AccountDbContext)
            {
                try
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                            from rom in Applicate.AccountDbContext.Rooms
                            where rom.jid == this.jid
                            select rom
                            ).FirstOrDefault();
                    result.name = this.name;
                    Applicate.AccountDbContext.Entry(result).Property(r => r.name).IsModified = true;//设置单字段更新
                    Applicate.AccountDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region 更新群聊公告
        public void UpdateRoomNoticeByJid()
        {
            lock (Applicate.AccountDbContext)
            {
                try
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                            from rom in Applicate.AccountDbContext.Rooms
                            where rom.jid == this.jid
                            select rom
                            ).FirstOrDefault();
                    result.detial = this.detial;
                    Applicate.AccountDbContext.Entry(result).Property(r => r.detial).IsModified = true;//设置单字段更新
                    Applicate.AccountDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region 根据RoomId删除
        /// <summary>
        /// 根据RoomId删除
        /// </summary>
        /// <param name="roomid"></param>
        internal void DeleteById(string roomid)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var tmproom = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.id == roomid);//查出
                    if (tmproom != null)
                    {
                        Applicate.AccountDbContext.Rooms.Remove(tmproom);//删除
                        int res = Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteByJid()
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);
                    if (result != null)
                    {
                        Applicate.AccountDbContext.Rooms.Remove(result);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除(根据传参删除)
        /// </summary>
        public void DeleteByJid(string jid)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                            from rom in Applicate.AccountDbContext.Rooms
                            where rom.jid == jid
                            select rom
                            );
                    Applicate.AccountDbContext.Rooms.RemoveRange(result);
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        public void DeleteAllList()
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var result = (
                        from rom in Applicate.AccountDbContext.Rooms
                        select rom
                        );
                    if (result != null)
                    {
                        Applicate.AccountDbContext.Rooms.RemoveRange(result);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        public Room toModel(string roomJson)
        {
            Room msgObj = JsonConvert.DeserializeObject<Room>(roomJson);
            return msgObj;
        }
        #endregion

        #region 根据Jid查询
        /// <summary>
        /// 根据Jid查询
        /// </summary>
        /// <returns></returns>
        public Room GetByJid()
        {
            Room result = null;
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    result = (
                        from room in Applicate.AccountDbContext.Rooms
                        where room.jid == this.jid
                        select room
                        ).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        #endregion

        #region 是否存在
        /// <summary>
        /// 是否存在对应Jid的群
        /// </summary>
        /// <returns></returns>
        internal bool Exists()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = Applicate.AccountDbContext.Rooms.Count(r => r.jid == this.jid);
                return result != 0;
            }
        }
        #endregion  

        #region 根据Jid查询
        /// <summary>
        /// 根据Jid查询
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public Room GetByJid(string jid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.jid == jid
                    select room
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据RoomId查询
        /// <summary>
        /// 根据RoomId查询
        /// </summary>
        /// <returns></returns>
        public Room GetByRoomId()
        {
            lock (Applicate.AccountDbContext.Rooms)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.id == this.id
                    select room
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据RoomId查询
        /// <summary>
        /// 根据RoomId查询
        /// </summary>
        /// <param name="id">对应的Id</param>
        /// <returns></returns>
        public Room GetByRoomId(string id)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.id == id
                    select room
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据RoomJid查询
        /// <summary>
        /// 根据RoomJid查询
        /// </summary>
        /// <param name="id">对应的Id</param>
        /// <returns></returns>
        public Room GetByRoomJid(string jid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.jid == jid
                    select room
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据Jid获取RoomId
        /// <summary>
        /// 根据Jid获取RoomId
        /// </summary>
        /// <param name="jid">查询的Jid</param>
        /// <returns></returns>
        public string GetRoomIdByJid(string jid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var resultId = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.jid == jid
                    select room.id
                    ).FirstOrDefault();
                return resultId;
            }
        }
        #endregion

        #region 自动插入
        /// <summary>
        /// 自动插入
        /// </summary>
        public void AutoInsert()
        {
            if (this.jid == null)
            {
                return;
            }
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                if (Applicate.AccountDbContext.Rooms.Count(r => r.jid == this.jid) == 0)//如果不存在则
                {
                    var obj = this.MemberwiseClone() as Room;
                    Applicate.AccountDbContext.Rooms.Add(obj);
                }
                else
                {
                    var result = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);
                    result = this.MemberwiseClone() as Room;
                }
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 根据roomJid获得对象
        public Room GetObjectByRoomJid()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.jid == this.jid
                    select room
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 根据Jid获取
        /// <summary>
        /// 根据Jid获取
        /// </summary>
        /// <param name="jid">需要查询的Jid</param>
        /// <returns></returns>
        internal string GetRoomNameByJid(string jid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                    where room.jid == jid
                    select room.name
                    ).FirstOrDefault();
                return result;
            }
        }
        #endregion

        #region 更新公开群状态
        /// <summary>
        /// 更新公开群状态
        /// </summary>
        /// <param name="isLook">0公开,1不公开</param>
        internal void UpdateIsPublic(int isLook)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.isLook = isLook;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
        /// <summary>
        /// 修改全群禁言
        /// </summary>
        /// <param name="talkTime">talkTime==0取消禁言</param>
        internal void UpdateTalkTime(long talkTime)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                if (room != null)
                {
                    room.talkTime = talkTime;
                    Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                    Applicate.AccountDbContext.SaveChanges();//保存
                }
            }
        }

        #region 更新显示已读人数
        /// <summary>
        /// 更新显示已读人数
        /// </summary>
        /// <param name="isLook">0不显示,1显示</param>
        internal void UpdateShowRead(int isShowRead)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.showRead = isShowRead;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 更新进群验证状态
        /// <summary>
        /// 更新进群验证状态
        /// </summary>
        /// <param name="isLook">0不验证,1需验证</param>
        public void UpdateNeedVerify(int isNeedVerify)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                if (room != null)
                {
                    room.isNeedVerify = isNeedVerify;
                    Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                    Applicate.AccountDbContext.SaveChanges();//保存
                }
            }
        }
        #endregion

        #region 更新免打扰设置
        /// <summary>
        /// 更新免打扰设置
        /// </summary>
        /// <param name="offlineNoPushMsg"></param>
        internal void UpdateOfflineNoPush(int offlineNoPushMsg)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                if (room != null)
                {
                    room.offlineNoPushMsg = offlineNoPushMsg;
                    Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                    Applicate.AccountDbContext.SaveChanges();//保存
                }
            }
        }
        #endregion

        #region 更新允许群内发送名片
        /// <summary>
        /// 更新进群验证状态
        /// </summary>
        /// <param name="isLook">0不验证,1需验证</param>
        internal void UpdateAllowSendCard(int allowSendCard)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.allowSendCard = allowSendCard;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 更新允许允许普通成员邀请好友
        /// <summary>
        /// 更新允许允许普通成员邀请好友
        /// </summary>
        /// <param name="allowInviteFriend">0不允许,1允许</param>
        internal void UpdateAllowInviteFriend(int allowInviteFriend)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.allowInviteFriend = allowInviteFriend;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 更新显示群成员状态
        /// <summary>
        /// 更新显示群成员状态
        /// </summary>
        /// <param name="showMember"></param>
        internal void UpdateShowMember(int showMember)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.showMember = showMember;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 允许群成员上传群共享文件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowUploadFile"></param>
        internal void UpdateAllowUploadFile(int allowUploadFile)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.allowUploadFile = allowUploadFile;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 允许成员 召开会议
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowUploadFile"></param>
        internal void UpdateAllowConference(int allowConference)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.allowConference = allowConference;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 允许群成员 开启 讲课
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowUploadFile"></param>
        internal void UpdateAllowSpeakCourse(int allowSpeakCourse)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.allowSpeakCourse = allowSpeakCourse;
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 更新群主
        /// <summary>
        /// 更新群主
        /// </summary>
        /// <param name="userId"></param>
        internal void UpdateOwner(string userId)
        {
            try
            {
                lock (Applicate.AccountDbContext)
                {
                    SQLiteDBContext.DBAutoConnect();
                    var room = Applicate.AccountDbContext.Rooms.FirstOrDefault(r => r.jid == this.jid);//根据Id查询
                    if (room != null)
                    {
                        room.userId = int.Parse(userId);
                        Applicate.AccountDbContext.Entry(room).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region 获取已加入群组
        /// <summary>
        /// 获取已加入群组
        /// </summary>
        /// <returns>已加入群组列表</returns>
        public List<Room> GetJoinedList()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from room in Applicate.AccountDbContext.Rooms
                        //where room.IsJoined == true
                    select room
                    ).ToList();
                return result;
            }
        }
        #endregion

        #region 更新群聊成员人数
        /// <summary>
        /// 更新群聊成员人数
        /// </summary>
        /// <param name="roomId">RoomId</param>
        public void UpdateMemberSize(string roomId, int count)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var tmp = Applicate.AccountDbContext.Rooms.FirstOrDefault(m => m.id == roomId);//获取当前群组
                if (tmp != null && tmp.userSize != count)
                {
                    tmp.userSize = count;
                    Applicate.AccountDbContext.SaveChanges();//保存更改
                }
            }
        }
        #endregion
        #endregion
    }
    #endregion

    #region 公告
    /// <summary>
    /// 公告
    /// </summary>
    public class Notice : ObservableObject
    {
        public Notice()
        {
            if (id == null || id == "")
            {
                id = Guid.NewGuid().ToString("N");
            }
        }

        #region Private Member
        private string id;// 通知Id
        private string roomId;// 房间Id
        private string text;// 通知文本
        private int userId;// 用户Id
        private string nickname;// 用户昵称
        private long time;// 时间 
        #endregion

        #region Public Member
        /// <summary>
        /// 数据库主键(通知Id)
        /// </summary>
        [Key]
        [JsonProperty("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if (id == value)
                {
                    return;
                }

                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// 房间Id
        /// </summary>
        [JsonProperty("roomId")]
        public string RoomId
        {
            get { return roomId; }
            set
            {
                if (roomId == value)
                {
                    return;
                }

                roomId = value;
                RaisePropertyChanged(nameof(RoomId));
            }
        }

        /// <summary>
        /// 通知文本
        /// </summary>
        [JsonProperty("text")]
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                {
                    return;
                }

                text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("userId")]
        public int UserId
        {
            get { return userId; }
            set
            {
                if (userId == value)
                {
                    return;
                }

                userId = value;
                RaisePropertyChanged(nameof(UserId));
            }
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [JsonProperty("nickname")]
        public string Nickname
        {
            get { return nickname; }
            set
            {
                if (nickname == value)
                {
                    return;
                }

                nickname = value;
                RaisePropertyChanged(nameof(Nickname));
            }
        }

        /// <summary>
        /// 时间
        /// </summary>
        [JsonProperty("time")]
        public long Time
        {
            get { return time; }
            set
            {
                if (time == value)
                {
                    return;
                }

                time = value;
                RaisePropertyChanged(nameof(Time));
            }
        }
        #endregion

        #region 自动批量插入
        /// <summary>
        /// 自动批量插入
        /// </summary>
        /// <param name="notices">成员</param>
        public void AutoInsertRange(IList<Notice> notices)
        {
            lock (Applicate.AccountDbContext)
            {
                for (int i = 0; i < notices.Count; i++)
                {
                    var notice = notices[i];
                    //如果两个主键(userid 和 roomid)
                    if (Applicate.AccountDbContext.Notices.Count(m => m.RoomId == notice.RoomId) == 0)
                    {
                        Applicate.AccountDbContext.Notices.Add(notice);
                    }
                    else
                    {
                        //获取主键对应的值
                        var tmpres = Applicate.AccountDbContext.Notices.FirstOrDefault(m => m.RoomId == notice.RoomId);
                        tmpres = notice.MemberwiseClone() as Notice;
                    }
                }
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #endregion

    }

}
