using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ShikuIM.Model
{

    public enum MemberRole
    {
        Owner = 1,
        Admin = 2,
        Member = 3,
        PlusIcon = 11,
        KickIcon = 12
    }
    public class JsonRoomMember : JsonBase
    {
        public JsonRoomMember()
        {
            data = new DataofMember();
        }
        public DataofMember data { get; set; }
    }


    #region DataofMember
    /// <summary>
    /// 群聊中群聊个人的DataOfRoomMember
    /// </summary>
    [JsonObject("member")]
    public class DataofMember : ObservableObject
    {
        public DataofMember()
        {
            if (Id == null || Id == "")
            {
                Id = Guid.NewGuid().ToString("N");//主键
            }
        }

        #region PrivaetMember
        private string _id;
        private string _roomid;
        private int _offlineNoPushMsg;
        private MemberRole _role;
        private int _sub;
        private long _active;
        private long _talkTime;
        private string _call;
        private long _createTime;
        private long _modifyTime;
        private string _nickname;
        private string _userId;
        private string _videoMeetingNo;
        #endregion

        #region Public Member

        [Key]
        public string Id
        {
            get
            {
                return _id;
            }
            set { _id = value; }
        }

        /// <summary>
        /// 所属的房间Id
        /// </summary>
        public string groupid
        {
            get { return _roomid; }
            set { _roomid = value; RaisePropertyChanged(nameof(groupid)); }
        }

        /// <summary>
        /// UserId
        /// </summary>
        public string userId
        {
            get { return _userId; }
            set { _userId = value; RaisePropertyChanged(nameof(userId)); }
        }

        /// <summary>
        /// 最后一次互动时间
        /// </summary>
        public long active
        {
            get { return _active; }
            set { _active = value; RaisePropertyChanged(nameof(active)); }
        }

        /// <summary>
        /// 语音通话标识符
        /// </summary>
        public string call
        {
            get { return _call; }
            set { _call = value; RaisePropertyChanged(nameof(call)); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long createTime
        {
            get { return _createTime; }
            set { _createTime = value; RaisePropertyChanged(nameof(createTime)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long modifyTime
        {
            get { return _modifyTime; }
            set { _modifyTime = value; RaisePropertyChanged(nameof(modifyTime)); }
        }

        /// <summary>
        /// 成员昵称
        /// </summary>
        public string nickname
        {
            get { return _nickname; }
            set { _nickname = value; RaisePropertyChanged(nameof(nickname)); }
        }

        /// <summary>
        /// 消息免打扰
        /// </summary>
        public int offlineNoPushMsg
        {
            get { return _offlineNoPushMsg; }
            set { _offlineNoPushMsg = value; RaisePropertyChanged(nameof(offlineNoPushMsg)); }
        }



        /// <summary>
        /// 成员角色：1=创建者 2=管理员 3=成员
        /// <para>附加：如果为预览成员的话,,则为</para>
        /// </summary>
        public MemberRole role
        {
            get { return _role; }
            set { _role = value; RaisePropertyChanged(nameof(role)); }
        }

        /// <summary>
        /// 订阅群信息：0=否 1=是
        /// </summary>
        public int sub
        {
            get { return _sub; }
            set { _sub = value; RaisePropertyChanged(nameof(sub)); }
        }

        /// <summary>
        /// 大于当前时间时禁止发言
        /// </summary>
        public long talkTime
        {
            get { return _talkTime; }
            set { _talkTime = value; RaisePropertyChanged(nameof(talkTime)); }
        }



        /// <summary>
        /// 视频会议标识符
        /// </summary>
        public string videoMeetingNo
        {
            get { return _videoMeetingNo; }
            set { _videoMeetingNo = value; RaisePropertyChanged(nameof(videoMeetingNo)); }
        }
        #endregion

        #region 自动批量插入
        /// <summary>
        /// 自动批量插入
        /// </summary>
        /// <param name="members">成员</param>
        public void AutoInsertRange(IList<DataofMember> members, string roomid)
        {
            lock (Applicate.AccountDbContext)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    //如果两个主键(userid 和 roomid)
                    var tmpmem = members[i];
                    if (Applicate.AccountDbContext.RoomMembers.Count(m => m.groupid == roomid && m.userId == tmpmem.userId) == 0)
                    {
                        tmpmem.groupid = roomid;//设置群组ID
                        Applicate.AccountDbContext.RoomMembers.Add(tmpmem);
                    }
                    else
                    {
                        //获取主键对应的值
                        var tmpres = Applicate.AccountDbContext.RoomMembers.FirstOrDefault(m => m.userId == tmpmem.userId && m.groupid == roomid);
                        tmpres.groupid = roomid;
                        tmpres = members[i].MemberwiseClone() as DataofMember;
                    }
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
                Applicate.AccountDbContext.RoomMembers.Remove(this);
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion


        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(string userId)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var item = Applicate.AccountDbContext.RoomMembers.FirstOrDefault(m => m.userId == userId);//获取群聊
                if (item != null)
                {
                    Applicate.AccountDbContext.RoomMembers.Remove(item);
                }
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 删除群成员
        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteTheMemberByRoomJid(string jid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var roomResult = (
                        from room in Applicate.AccountDbContext.Rooms
                        where room.jid == jid
                        select room
                        ).FirstOrDefault();
                if (roomResult != null)
                {
                    var result = (
                        from RoomMember in Applicate.AccountDbContext.RoomMembers
                        where RoomMember.groupid == roomResult.id
                        && RoomMember.userId == this.userId
                        select RoomMember
                        );
                    Applicate.AccountDbContext.RoomMembers.RemoveRange(result);
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region 删除群成员
        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteTheMemberByRoomId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var roomResult = (
                        from room in Applicate.AccountDbContext.Rooms
                        where room.id == this.groupid
                        select room
                        ).FirstOrDefault();
                if (roomResult != null)
                {
                    var result = (
                        from RoomMember in Applicate.AccountDbContext.RoomMembers
                        where RoomMember.groupid == roomResult.id
                        && RoomMember.userId == this.userId
                        select RoomMember
                        );
                    Applicate.AccountDbContext.RoomMembers.RemoveRange(result);
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region 删除群成员
        /// <summary>
        /// 删除
        /// </summary>
        public void DeleteByRoomJid(string jid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var roomResult = (
                        from room in Applicate.AccountDbContext.Rooms
                        where room.jid == jid
                        select room
                        ).FirstOrDefault();
                if (roomResult != null)
                {
                    var result = (
                        from RoomMember in Applicate.AccountDbContext.RoomMembers
                        where RoomMember.groupid == roomResult.id
                        select RoomMember
                        );
                    Applicate.AccountDbContext.RoomMembers.RemoveRange(result);
                    Applicate.AccountDbContext.SaveChanges();
                }
            }
        }
        #endregion

        #region 更新群权限
        public void UpdateRoleByJid(string jid)
        {
            var room = new Room().GetByJid(jid);
            if (room != null)
            {
                lock (Applicate.AccountDbContext)
                {
                    var result = Applicate.AccountDbContext.RoomMembers.FirstOrDefault(m => m.userId == this.userId && m.groupid == room.id);
                    if (result != null)
                    {
                        result.role = this.role;//更新数据
                        Applicate.AccountDbContext.Entry(result).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存数据库
                    }
                }
            }
        }
        #endregion

        #region 更新禁言时间
        public void UpdateTalkTime(string jid, long talkTime)
        {
            var room = new Room().GetByJid(jid);
            if (room != null)
            {
                lock (Applicate.AccountDbContext)
                {
                    APIHelper.GetRoomDetialByRoomId(room.id);
                    var result = Applicate.AccountDbContext.RoomMembers.FirstOrDefault(m => m.userId == this.userId && m.groupid == room.id);
                    if (result != null)
                    {
                        result.talkTime = talkTime;//更新数据
                        Applicate.AccountDbContext.Entry(result).State = EntityState.Modified;
                        Applicate.AccountDbContext.SaveChanges();//保存数据库
                    }
                }
            }
        }
        #endregion

        #region 获取对象
        public DataofMember GetModel()
        {
            using (var DBContext = new SQLiteDBContext())
            {
                //var result = (
                //    from RoomMember in DBContext.RoomMembers
                //    where RoomMember.groupid == this.groupid
                //    && RoomMember.userId == this.userId
                //    select RoomMember
                //    ).FirstOrDefault();
                //return result;
                DbRawSqlQuery<DataofMember> queue = DBContext.Database.SqlQuery<DataofMember>("SELECT * FROM RoomMember WHERE userId = '" + this.userId + "' and groupid='" + this.groupid + "'");
                DataofMember obj = queue.FirstOrDefault();
                return obj;
            }
        }
        #endregion

        #region 获取对象
        public DataofMember GetModelByJid(string jid)
        {
            using (var DBContext = new SQLiteDBContext())
            {
                //var result = (
                //    from RoomMember in DBContext.RoomMembers
                //    where RoomMember.groupid == this.groupid
                //    && RoomMember.userId == this.userId
                //    select RoomMember
                //    ).FirstOrDefault();
                //return result;
                DbRawSqlQuery<DataofMember> queue =
                    DBContext.Database.SqlQuery<DataofMember>("SELECT * FROM RoomMember JOIN Room ON RoomMember.groupid=Room.id WHERE RoomMember.userId = '" +
                    this.userId + "' and Room.jid='" + jid + "'");
                DataofMember obj = queue.FirstOrDefault();
                return obj;
            }
        }
        #endregion

        #region 根据群聊ID删除
        /// <summary>
        /// 根据群聊ID删除
        /// </summary>
        public void DeleteByRoomId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                        from RoomMember in Applicate.AccountDbContext.RoomMembers
                        where RoomMember.groupid == this.groupid
                        select RoomMember
                        ).ToList();
                if (result != null && result.Count > 0)
                {
                    lock (Applicate.AccountDbContext)
                    {
                        Applicate.AccountDbContext.RoomMembers.RemoveRange(result);
                        Applicate.AccountDbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion

        #region 根据群聊ID查询
        /// <summary>
        /// 根据群聊ID查询
        /// </summary>
        public List<DataofMember> GetListByRoomId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from RoomMember in Applicate.AccountDbContext.RoomMembers
                    where RoomMember.groupid == this.groupid
                    select RoomMember
                    ).ToList();
                return result;
            }
        }
        #endregion

        #region 根据GroupId获取成员数量
        /// <summary>
        /// 根据GroupId获取成员数量
        /// </summary>
        /// <returns></returns>
        public int CountByRoomId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                int result = Applicate.AccountDbContext.RoomMembers.Count(m => m.groupid == this.groupid);
                return result;
            }
        }
        #endregion

        #region 更新到数据库
        /// <summary>
        /// 更新到数据库
        /// </summary>
        public void UpdateMemberCall()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                    from RoomMember in Applicate.AccountDbContext.RoomMembers
                    where RoomMember.groupid == this.groupid
                    where RoomMember.userId == this.userId
                    select RoomMember
                    ).FirstOrDefault();
                result.nickname = this.nickname;
                Applicate.AccountDbContext.Entry(result).State = EntityState.Modified;
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 根据群聊ID查询
        /// <summary>
        /// 根据群聊ID查询
        /// <paramref name="roomid">对应的群组ID</paramref>
        /// </summary>
        public List<DataofMember> GetListByRoomId(string roomid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                var result = (
                        from RoomMember in Applicate.AccountDbContext.RoomMembers
                        where RoomMember.groupid == roomid
                        select RoomMember
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

        #region 转MessageListItem
        public MessageListItem ToMsgItem()
        {
            var item = new MessageListItem
            {
                Jid = this.userId,
                ShowTitle = this.nickname,
                Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(this.userId)
            };
            return item;
        }
        #endregion

        #region 反序列化
        public DataofMember toModel(string strJson)
        {
            DataofMember msgObj = JsonConvert.DeserializeObject<DataofMember>(strJson);
            return msgObj;
        }
        #endregion

        #region 更新成员昵称
        /// <summary>
        /// 更新成员昵称
        /// </summary>
        /// <param name="jid"></param>
        /// <param name="fromId"></param>
        /// <param name="content"></param>
        internal void UpdateMemberNickname(string jid, string fromId, string content)
        {
            var room = new Room().GetByJid(jid);
            if (room != null)
            {
                lock (Applicate.AccountDbContext)
                {
                    var member = Applicate.AccountDbContext.RoomMembers.FirstOrDefault(m => m.userId == this.userId && m.groupid == room.id);
                    if (member != null)
                    {
                        member.nickname = content;//更新数据
                        Applicate.AccountDbContext.SaveChanges();//保存数据库
                    }
                }
            }
        }
        #endregion

        #region 根据RoomId获取群成员数量
        /// <summary>
        /// 根据RoomId获取群成员数量
        /// </summary>
        /// <param name="roomid">RoomId</param>
        /// <returns></returns>
        internal int GetCountByRoomId(string roomid)
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                int count = Applicate.AccountDbContext.RoomMembers.Count(m => m.groupid == roomid);
                return count;
            }
        }
        #endregion

    }
    #endregion

}
