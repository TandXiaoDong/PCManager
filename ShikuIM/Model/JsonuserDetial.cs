
using System;
using System.Data;
using GalaSoft.MvvmLight;

namespace ShikuIM.Model
{

    public class JsonuserDetial : JsonBase
    {
        #region 初始化引用类型
        public JsonuserDetial()
        {
            data = new DataOfUserDetial();
        }
        #endregion

        /// <summary>
        /// 用户详细信息
        /// </summary>
        public DataOfUserDetial data { get; set; }

    }

    #region 用户详细信息
    /// <summary>
    /// 用户详细信息
    /// </summary>
    public class DataOfUserDetial : ObservableObject
    {

        #region 更新用户资料
        public void UpdateDataOfUserDetial()
        {
            DataOfFriends friendObj = new DataOfFriends() { toUserId = this.userId }.GetByUserId(); ;
            if (friendObj != null)
            {
                friendObj.active = this.active;
                friendObj.areaCode = this.areaCode;
                friendObj.areaId = this.areaId;
                friendObj.attCount = this.attCount;
                friendObj.balance = this.balance;
                friendObj.birthday = this.birthday;
                friendObj.cityId = this.cityId;
                friendObj.companyId = this.companyId;
                friendObj.countryId = this.countryId;
                friendObj.createTime = this.createTime;
                friendObj.description = this.description;
                friendObj.fansCount = this.fansCount;
                friendObj.friendsCount = this.friendsCount;
                friendObj.idcard = this.idcard;
                friendObj.idcardUrl = this.idcardUrl;
                friendObj.isAuth = this.isAuth;
                friendObj.level = this.level;
                friendObj.modifyTime = this.modifyTime;
                friendObj.name = this.name;
                friendObj.nickname = this.nickname;
                friendObj.num = this.num;
                friendObj.offlineNoPushMsg = this.offlineNoPushMsg;
                friendObj.onlinestate = this.onlinestate;
                friendObj.password = this.password;
                friendObj.phone = this.phone;
                friendObj.provinceId = this.provinceId;
                friendObj.sex = this.sex;
                friendObj.avatarName = this.avatarName;
                friendObj.status = this.status;
                friendObj.telephone = this.Telephone;
                friendObj.totalConsume = this.totalConsume;
                friendObj.totalRecharge = this.totalRecharge;
                friendObj.userId = this.userId;
                friendObj.userKey = this.userKey;
                friendObj.userType = this.userType;
                friendObj.vip = this.vip;
                friendObj.avatarName = this.avatarName;
                friendObj.Update();
            }

        }
        #endregion

        #region 转换为DataOfFfriend
        /// <summary>
        /// 转换为DataOfFfriend
        /// </summary>
        /// <param name="friend">对应User进行</param>
        /// <returns>转换后的对象</returns>
        public DataOfFriends ConvertToDataFriend(DataOfUserDetial friend)
        {
            return new DataOfFriends()
            {
                active = friend.active,
                areaCode = friend.areaCode,
                areaId = friend.areaId,
                attCount = friend.attCount,
                balance = friend.balance,
                birthday = friend.birthday,
                cityId = friend.cityId,
                companyId = friend.companyId,
                countryId = friend.countryId,
                createTime = friend.createTime,
                description = friend.description,
                fansCount = friend.fansCount,
                friendsCount = friend.friendsCount,
                idcard = friend.idcard,
                isAuth = friend.isAuth,
                idcardUrl = friend.idcardUrl,
                level = friend.level,
                modifyTime = friend.modifyTime,
                name = friend.name,
                //nickname = friend.nickname,
                num = friend.num,
                offlineNoPushMsg = friend.offlineNoPushMsg,
                onlinestate = friend.onlinestate,
                password = friend.password,
                phone = friend.phone,
                provinceId = friend.provinceId,
                sex = friend.sex,
                status = friend.status,
                telephone = friend.Telephone,
                toNickname = friend.nickname,
                totalConsume = friend.totalConsume,
                totalRecharge = friend.totalRecharge,
                toUserId = friend.userId,
                userKey = friend.userKey,
                userType = friend.userType,
                avatarName = friend.avatarName,
                vip = friend.vip
            };
        }

        /// <summary>
        /// 无参默认使用自身对象进行转换
        /// </summary>
        /// <returns></returns>
        public DataOfFriends ConvertToDataFriend()
        {
            return ConvertToDataFriend(this);
        }
        #endregion

        #region 初始化引用类型
        public DataOfUserDetial()
        {
            loc = new LocOfDetial();
            loginLog = new LoginLogOfDetial();
            settings = new SettingsOfDetial();
            friends = new DataOfFriends();
        }
        #endregion

        private int _cityId;
        private string _nickname;
        private DataOfFriends _friend;
        public int _areaId;
        private long _birthday;

        /// <summary>
        /// 
        /// </summary>
        public int active { get; set; }

        /// <summary>
        /// 好友对象
        /// </summary>
        public DataOfFriends friends
        {
            get { return _friend; }
            set { _friend = value; RaisePropertyChanged(nameof(friends)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string areaCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int areaId
        {
            get
            {
                return _areaId;
            }
            set
            {
                _areaId = value;
                RaisePropertyChanged(nameof(areaId));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int attCount { get; set; }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public double balance { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public long birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                RaisePropertyChanged(nameof(birthday));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int cityId
        {
            get
            {
                return _cityId;
            }
            set
            {
                _cityId = value;
                RaisePropertyChanged(nameof(cityId));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int companyId { get; set; }

        int _countryId;
        /// <summary>
        /// 
        /// </summary>
        public int countryId
        {
            get
            {
                return _countryId;
            }
            set
            {
                _countryId = value;
                RaisePropertyChanged(nameof(countryId));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }

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
        public LocOfDetial loc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoginLogOfDetial loginLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long modifyTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                RaisePropertyChanged(nameof(nickname));
            }
        }

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
        int _provinceId;
        /// <summary>
        /// 
        /// </summary>
        public int provinceId
        {
            get
            {
                return _provinceId;
            }
            set
            {
                _provinceId = value;
                RaisePropertyChanged(nameof(provinceId));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SettingsOfDetial settings { get; set; }

        private int _sex;

        /// <summary>
        /// 性别
        /// </summary>
        public int sex
        {
            get { return _sex; }
            set { _sex = value; RaisePropertyChanged(nameof(sex)); }
        }
        /// <summary>
        /// 好友状态
        /// <para>-2:对方拉黑我方；-1:我方拉黑对方；0：陌生人；1:单方关注；2:互为好友</para>
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 当前登录账号的电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double totalConsume { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double totalRecharge { get; set; }

        string _userId;
        /// <summary>
        /// 好友的UserId
        /// </summary>
        public string userId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                RaisePropertyChanged(nameof(userId));
            }
        }
        public static string personalId;

        public static string _avatarName;
        /// <summary>
        /// 头像名称
        /// </summary>
        public string avatarName
        {
            get
            {
                return _avatarName;
            }
            set
            {
                _avatarName = value;
                RaisePropertyChanged(nameof(avatarName));
            }
        }

        /// <summary>
        /// 
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

        public string _remarkName;

        /// <summary>
        /// 备注名称
        /// </summary>
        public string remarkName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_remarkName))//如果为空显示真实昵称
                {
                    return _nickname;
                }
                else//如果不为空显示备注
                {
                    return _remarkName;
                }
            }
            set
            {
                _remarkName = value;
                RaisePropertyChanged(nameof(remarkName));
            }
        }
    }
    #endregion

    #region SettingsOfDetial
    /// <summary>
    /// 用户的设置
    /// </summary>
    public class SettingsOfDetial
    {

        /// <summary>
        /// 
        /// </summary>
        public int allowAtt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int allowGreet { get; set; }

        /// <summary>
        /// 是否需要好友验证
        /// </summary>
        public int friendsVerify { get; set; }
    }
    #endregion

    #region LocOfDetial
    /// <summary>
    /// LocOfDetial
    /// </summary>
    public class LocOfDetial
    {
        /// <summary>
        /// 
        /// </summary>
        public string lat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string lng { get; set; }

    }
    #endregion

    #region LoginLogOfDetial
    /// <summary>
    /// LoginLogOfDetial
    /// </summary>
    public class LoginLogOfDetial
    {
        /// <summary>
        /// 
        /// </summary>
        public int isFirstLogin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int loginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int offlineTime { get; set; }
    }
    #endregion


}
