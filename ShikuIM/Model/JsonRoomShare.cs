using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace ShikuIM.Model
{
    public class JsonRoomShare
    {
        #region 初始化引用类型
        public JsonRoomShare()
        {
            data = new List<RoomShare>();
        }
        #endregion


        /// <summary>
        /// 房间详情
        /// </summary>
        public List<RoomShare> data { get; set; }
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class RoomShare : ObservableObject
    {
        #region Private Properties
        private string _name;
        private string _nickname;
        private string _shareId;
        private long _size;
        private long _time;
        private int _type;
        private string _url;
        private string _userId;
        private string _filePath;
        private string _progress;
        private bool _allowDel;
        #endregion

        #region Public Properties
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 房间Id
        /// </summary>
        public string roomId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(nameof(name)); }
        }

        /// <summary>
        /// 上传的用户Id
        /// </summary>
        public string userId
        {
            get { return _userId; }
            set { _userId = value; RaisePropertyChanged(nameof(userId)); }
        }

        /// <summary>
        /// 上传用户昵称
        /// </summary>
        public string nickname
        {
            get { return _nickname; }
            set { _nickname = value; RaisePropertyChanged(nameof(nickname)); }
        }

        /// <summary>
        /// 分享的Id
        /// </summary>
        public string shareId
        {
            get { return _shareId; }
            set { _shareId = value; RaisePropertyChanged(nameof(shareId)); }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged(nameof(size)); }
        }

        /// <summary>
        /// 时间
        /// </summary>
        public long time
        {
            get { return _time; }
            set { _time = value; RaisePropertyChanged(nameof(time)); }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public int type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged(nameof(type)); }
        }

        /// <summary>
        /// URL
        /// </summary>
        public string url
        {
            get { return _url; }
            set { _url = value; RaisePropertyChanged(nameof(url)); }
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath
        {
            get { return _filePath; }
            set { _filePath = value; RaisePropertyChanged(nameof(filePath)); }
        }

        [NotMapped]
        [JsonIgnore]
        public string detial { get; set; }


        [NotMapped]
        [JsonIgnore]
        public string progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged(nameof(progress));
            }
        }

        [NotMapped]
        [JsonIgnore]
        /// <summary>
        /// 能否删除群文件操作
        /// </summary>
        public bool AllowDel
        {
            get
            {
                return _allowDel;
            }
            set
            {
                _allowDel = value;
                RaisePropertyChanged(nameof(AllowDel));
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
        public RoomShare toModel(string roomJson)
        {
            RoomShare msgObj = JsonConvert.DeserializeObject<RoomShare>(roomJson);
            return msgObj;
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
                if (this.Id == null || this.Id == Guid.Empty.ToString("N"))
                {
                    this.Id = Guid.NewGuid().ToString("N");
                }

                Applicate.AccountDbContext.RoomShares.Add(this);
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 保存下载路径
        public void UpdateFilePath()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                if (GetByShareId() == null)
                {
                    Insert();
                }
                string id = GetByShareId().Id;
                var result = (
                        from share in Applicate.AccountDbContext.RoomShares
                        where share.Id == id
                        select share
                        ).FirstOrDefault();
                result.filePath = this.filePath;
                Applicate.AccountDbContext.SaveChanges();
            }
        }
        #endregion

        #region 根据roomId和ShareId获取对象
        public RoomShare GetByShareId()
        {
            lock (Applicate.AccountDbContext)
            {
                SQLiteDBContext.DBAutoConnect();
                return Applicate.AccountDbContext.RoomShares.FirstOrDefault(d => d.roomId == this.roomId && d.shareId == this.shareId);
            }
        }
        #endregion

        #region 根据roomId获取集合
        public List<RoomShare> GetListByRoomId()
        {
            var result = (
                    from share in Applicate.AccountDbContext.RoomShares
                    where roomId == this.roomId
                    select share
                    );
            return result.ToList();
        }
        #endregion
    }
}
