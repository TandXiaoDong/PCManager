using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ShikuIM
{
    /// <summary>
    /// API帮助类
    /// </summary>
    internal static class APIHelper
    {
        #region 获取配置信息
        /// <summary>       
        /// 从接口获取所有URL配置信息并赋值为全局变量
        /// </summary>
        internal static HttpClient GetFullConfig()
        {
            //string url = "http://test.shiku.co/config";//官方测试服
            //string url = "http://oem.shiku.co/config";//OEM测试环境
            //string url = "http://imapi.shiku.co/config";//官方正式服
            //string url = "http://imapi.qsnrko.com/config";//小爱
            //string url = "http://14.29.48.75:8092/config";//土白菜旧
            //string url = "http://14.29.48.213:8092/config";//土白菜新
            //string url = "  http://47.107.89.14:8092/config";
            //string url = "http://imapi.18806595500.cn:8092/config";//
            string url = ConfigurationUtil.GetValue("InitialServer");

            if (url.Contains("/config") == false)
            {
                url = url + "/config";
            }

            string para = "";
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            var result = client.UploadData(new Uri(url), parabyte);
            if (result != null)
            {
                string str = Encoding.UTF8.GetString(result);
                Applicate.URLDATA = JsonConvert.DeserializeObject<JsonConfigData>(str);
            }
            return client;
        }
        #endregion

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="tel">电话</param>
        /// <param name="pwd">密码</param>
        /// <returns>当前登录后返回的User对象</returns>
        internal static HttpClient UserLogin(string tel, string pwd, string latitude, string longitude, string areaCode)
        {
            //登录页面地址
            string url = Applicate.URLDATA.data.apiUrl + "user/login";
            //声明Post提交参数
            string encryptPwd = Helpers.MD5create(pwd);
            string para = "telephone=" + Helpers.MD5create(tel) + "&password=" + encryptPwd + "&latitude=" + latitude + "&longitude=" + longitude + "&areaCode=" + areaCode;
            //string para = "telephone=ba3a07ee00491f35e0efe7252e545f0b&password=e10adc3949ba59abbe56e057f20f883e";
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    Jsonuser currUser = null;
                    string resu = Encoding.UTF8.GetString(res.Result);
                    currUser = JsonConvert.DeserializeObject<Jsonuser>(resu);
                    //反序列化
                    if (currUser.resultCode == 0)
                    {
                        ConsoleLog.Output("Server返回详细信息：" + currUser.resultMsg);
                    }
                    else
                    {

                    }
                }
                else
                {
                    ConsoleLog.Output("网络异常:" + res.Error.Message);
                }
            };
            return client;
        }
        #endregion  

        #region 游客登录
        /// <summary>
        /// 游客登录
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>当前登录后返回的User对象</returns>
        internal static HttpClient VisitorLogin(string key, string latitude, string longitude)
        {
            //登录页面地址
            string url = Applicate.URLDATA.data.apiUrl + "tourist/touristLogin";
            //声明Post提交参数
            string para = "foreignKey=" + key;
            //string para = "telephone=ba3a07ee00491f35e0efe7252e545f0b&password=e10adc3949ba59abbe56e057f20f883e";
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    Jsonuser currUser = null;
                    string resu = Encoding.UTF8.GetString(res.Result);
                    currUser = JsonConvert.DeserializeObject<Jsonuser>(resu);
                    //反序列化
                    if (currUser.resultCode == 0)
                    {
                        ConsoleLog.Output("Server返回详细信息：" + currUser.resultMsg);
                    }
                    else
                    {

                    }
                }
                else
                {
                    ConsoleLog.Output("网络异常:" + res.Error.Message);
                }
            };
            return client;
        }
        #endregion  

        #region 验证手机号
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="telephone"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        internal static HttpClient TelephoneVerifyAsync(string telephone, string areaCode = "86")
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "verify/telephone";
            //传参
            /////////////用户令牌
            string para = "telephone=" + telephone + "&areaCode=" + areaCode;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 验证手机号
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="telephone"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        internal static string TelephoneVerify(string telephone, string areaCode = "86")
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "verify/telephone";
            //传参
            /////////////用户令牌
            string para = "telephone=" + telephone + "&areaCode=" + areaCode;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            //同步下载
            string result = Encoding.UTF8.GetString(client.UploadData(new Uri(url), parabyte));
            return result;
        }
        #endregion

        #region 上传头像
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static WebClient UploadAvatorAsync(string uploadUrl, byte[] bytes)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=---------------------------7d5b915500cee");
            webClient.UploadDataAsync(new Uri(uploadUrl), bytes);
            return webClient;
        }
        #endregion

        #region 用户注册
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="telephone"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        internal static HttpClient RegisterAccountAsync(string telephone, string areaCode, string password, string nickname, int sex, long birthday, int countryId, int provinceId, int cityId, int areaId)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "user/register";
            //传参
            /////////////用户令牌RSelectedCountryCode
            string para = "access_token=" + Applicate.Access_Token + "&telephone=" + telephone + "&areaCode=" + areaCode + "&password=" + Helpers.MD5create(password) + "&nickname=" + nickname + "&sex=" + sex
                + "&birthday=" + birthday + "&countryId=" + countryId + "&provinceId=" + provinceId + "&cityId=" + cityId + "&areaId=" + areaId + "&userType=1";
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 接口退出群聊
        /// <summary>
        /// 退出群聊
        /// </summary>
        /// <param name="roomId">需要退出的 群聊ID</param>
        /// <returns>是否成功</returns>
        internal static HttpClient ExitRoomAsync(string roomId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/delete";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&userId=" + Applicate.MyAccount.userId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 获取群聊详情
        /// <summary>
        /// 获取群聊详情
        /// </summary>
        /// <param name="roomId">对应的RoomId</param>
        /// <returns></returns>
        internal static HttpClient GetRoomDetialByRoomIdAsync(string roomId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/get";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var s = Encoding.UTF8.GetString(res.Result);
                    var roomDetail = JsonConvert.DeserializeObject<JsonRoom>(Encoding.UTF8.GetString(res.Result));
                    if (roomDetail.resultCode == 1)
                    {
                        new DataofMember { groupid = roomDetail.data.id }.DeleteByRoomId();//删除群成员
                        if (roomDetail.data.members.Count > 0)//如果群成员不为空,则存入数据库
                        {
                            roomDetail.data.members[0].AutoInsertRange(roomDetail.data.members, roomDetail.data.id);
                        }
                        if (roomDetail.data.notice != null)//更新群公告
                        {
                            roomDetail.data.notice.AutoInsertRange(roomDetail.data.notices);//更新公告
                        }
                        roomDetail.data.AutoInsert();
                    }
                }
            };
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;//返回群聊详情
        }
        #endregion

        #region 获取群聊详情
        /// <summary>
        /// 获取群聊详情
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        internal static JsonRoom GetRoomDetialByRoomId(string roomId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/get";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId;
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;//UTF8进行上传和下载
            var resText = client.DownloadString(url + "?" + para);//获取字符串
            var roomDetail = JsonConvert.DeserializeObject<JsonRoom>(resText);
            var memberLst = roomDetail.data.members;
            #region 存储群
            if (new DataofMember { groupid = roomId }.CountByRoomId() == 0)
            {
                memberLst[0].AutoInsertRange(memberLst, roomDetail.data.id);
            }
            var tmp = new Room() { id = roomId }.GetByRoomId();
            if (tmp == null)
            {
                roomDetail.data.AutoInsert();
            }
            #endregion
            return null;//返回群聊详情
        }
        #endregion

        #region 修改群允许普通成员邀请好友
        internal static HttpClient UpdateAllowInviteFriendAsync(string roomid, string allowInviteFriend)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&allowInviteFriend=" + allowInviteFriend;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群允许普通成员上传文件
        internal static HttpClient UpdateAllowUploadFileAsync(string roomid, string allowUploadFile)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&allowUploadFile=" + allowUploadFile;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群允许普通成员发起会议
        internal static HttpClient UpdateAllowConferenceAsync(string roomid, string allowConference)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&allowConference=" + allowConference;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群允许普通成员发起讲课
        internal static HttpClient UpdateAllowSpeakCourseAsync(string roomid, string allowSpeakCourse)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&allowSpeakCourse=" + allowSpeakCourse;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 获取成员详情
        internal static HttpClient GetMemberDetialAsync(string roomId, string userId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/get";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&userId=" + userId;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 加入群聊
        /// <summary>
        /// 接口加群
        /// </summary>
        /// <param name="RoomId">群组的RoomId(非Jid)</param>
        /// <returns></returns>
        internal static HttpClient GroupJoinAsync(string RoomId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/join";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + RoomId;//type (1自己房间 2加入的房间)
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群名称
        internal static HttpClient SetRoomNameAsync(string roomid, string roomname)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&roomName=" + roomname;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群允许发送名片
        internal static HttpClient UpdateAllowSendCardAsync(string roomid, string allowSendCard)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&allowSendCard=" + allowSendCard;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群描述
        /// <summary>
        /// 修改群描述
        /// </summary>
        /// <param name="roomid">群组ID(非jid)</param>
        /// <param name="desc">群组描述</param>
        /// <returns></returns>
        internal static HttpClient UpdateGroupChatDescAsync(string roomid, string desc)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&desc=" + desc;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群公告
        internal static HttpClient UpdateGroupChatNoticeAsync(string roomid, string notice)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&notice=" + notice;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群公告
        internal static HttpClient SetGroupChatNoticeAsync(string roomid, string notice)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&notice=" + notice;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改显示群成员
        /// <summary>
        /// 修改显示群成员
        /// </summary>
        /// <param name="roomid">RoomId</param>
        /// <param name="showMember">ShowMember</param>
        /// <returns></returns>
        internal static HttpClient SetShowMemberAsync(string roomid, string showMember)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&showMember=" + showMember;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改群验证
        internal static HttpClient SetNeedVerifyAsync(string roomid, string isNeedVerify)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&isNeedVerify=" + isNeedVerify;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 修改公开群
        internal static HttpClient SetPublicRoomAsync(string roomid, string isLook)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomid + "&isLook=" + isLook;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 获取群聊成员列表
        /// <summary>
        /// 获取群聊成员列表
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="keyword">关键词</param>
        /// <returns>RtnRoomMemberList</returns>
        public static HttpClient GetRoomMemberAsync(string roomId, string keyword = "")
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/list";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId;
            //可选参数
            if (keyword != null && keyword != "")
            {
                para = para + "&keyword=" + keyword;
            }
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var memberList = JsonConvert.DeserializeObject<JsonRoomMemberList>(Encoding.UTF8.GetString(res.Result));
                    new DataofMember() { groupid = roomId }.DeleteByRoomId();
                    if (memberList.data.Count > 0)//如果不为空则自动插入
                    {
                        memberList.data[0].AutoInsertRange(memberList.data, roomId);
                        new Room().UpdateMemberSize(roomId, memberList.data.Count);//更新群人数
                    }
                }
            };
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 获取我加入的群聊
        /// <summary>
        /// 获取我加入的群聊
        /// </summary>
        internal static HttpClient GetMyRoomsAsync()
        {
            //room/list
            string url = Applicate.URLDATA.data.apiUrl + "room/list/his";
            //参数
            string para = "access_token=" + Applicate.Access_Token + "&type=0&pageSize=10000&Userid";//0=所有；1=自己的房间；2=加入的房间（默认为0）
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 获取群聊
        /// <summary>
        /// 获取群聊
        internal static HttpClient GetAllRoomsAsync()
        {
            //    /room/list
            string url = Applicate.URLDATA.data.apiUrl + "room/list";
            //参数
            string para = "access_token=" + Applicate.Access_Token + "&pageSize=50"; //+ "&type=0&pageSize=10000&Userid"; //;//0=所有；1=自己的房间；2=加入的房间（默认为0）
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;//返回HttpClient
        }
        #endregion

        #region 删除群聊成员(仅管理员和群主操作)
        /// <summary>
        /// 删除群聊成员(仅管理员和群主操作)
        /// </summary>
        /// <param name="RoomId">目标RoomID</param>
        /// <param name="userId">目标UserID</param>
        internal static HttpClient DeleteRoomMemberAsync(string RoomId, string userId)
        {
            //此处为先调用接口再调用Xmpp
            string url = Applicate.URLDATA.data.apiUrl + "room/member/delete";//初始化URL
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + RoomId + "&userId=" + userId;
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;//如果Code为1则成功返回true，0则失败返回false
        }
        #endregion

        #region 获取用户详细信息
        /// <summary>
        /// 获取用户详细信息
        /// </summary>
        /// <param name="userId">用户的ID</param>
        /// <returns>返回的用户</returns>
        internal static HttpClient GetUserDetialAsync(string userId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "user/get";//定义Url
            string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId;//定义参数
            var parabytes = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabytes.Length.ToString());
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var user = JsonConvert.DeserializeObject<JsonuserDetial>(Encoding.UTF8.GetString(res.Result));
                    var detial = new DataOfFriends();
                    if (!string.IsNullOrWhiteSpace(user.data.userId))
                    {
                        detial = user.data.ToDataOfFriend();//转DataofFriend
                        detial.AutoInsert();//保存或更新数据库
                    }
                }
                else
                {
                    //网络错误
                }
            };
            client.UploadDataAsync(new Uri(url), parabytes);//异步执行
            return client;
        }
        #endregion

        #region 异步获取头像
        /// <summary>
        /// 异步获取头像
        /// </summary>
        /// <param name="userId">对应的UserId</param>
        /// <param name="isThumbnail">是否为缩略图</param>
        public static HttpResult DownloadAvator(string userId, bool isThumbnail = false)
        {
            string url = Applicate.URLDATA.data.downloadAvatarUrl + "avatar/" + ((isThumbnail) ? ("t") : ("o")) + "/" + Convert.ToInt32(userId) % 10000 + "/" + userId + ".jpg";
            HTTP http = new HTTP();
            HttpItem item = new HttpItem()
            {
                URL = url,
                ResultType = ResultType.Byte
            };
            HttpResult result = http.GetHtml(item);
            return result;
        }
        #endregion

        #region 添加关注(同意添加好友请求)
        /// <summary>
        /// 添加关注(同意添加好友请求)
        /// </summary>
        /// <param name="toUserId">需要同意的UserId</param>
        /// <returns>状态值</returns>
        internal static HttpClient AttentionAddAsync(string toUserId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "friends/attention/add";
            var para = "access_token=" + Applicate.Access_Token + "&toUserId=" + toUserId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 删除群聊
        /// <summary>
        /// 删除群聊
        /// </summary>
        /// <param name="roomId">群聊Id</param>
        /// <returns>是否成功</returns>
        internal static HttpClient DeleteRoomAsync(string roomId)
        {
            //声明Url
            string url = Applicate.URLDATA.data.apiUrl + "room/delete";
            //定义参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId;
            byte[] paras = Encoding.UTF8.GetBytes(para);//获取参数
            var client = new HttpClient();
            client.Headers.Add("ContentLength", paras.Length.ToString());
            client.UploadDataAsync(new Uri(url), paras);
            return client;
        }
        #endregion

        #region 解散群聊
        /// <summary>
        /// 解散群聊
        /// </summary>
        /// <param name="roomId">群聊Id</param>
        /// <returns>是否成功</returns>
        internal static HttpClient DismissRoomAsync(MessageListItem room)
        {
            //声明Url
            string url = Applicate.URLDATA.data.apiUrl + "room/delete";
            //定义参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + room.Id;
            byte[] paras = Encoding.UTF8.GetBytes(para);//获取参数
            var client = new HttpClient();
            client.Headers.Add("ContentLength", paras.Length.ToString());
            client.Tag = room;//指定Room参数
            client.UploadDataAsync(new Uri(url), paras);
            client.UploadDataCompleted += (sen, e) =>
            {
                if (e.Error == null)
                {
                    var resstr = Encoding.UTF8.GetString(e.Result);
                    var result = JsonConvert.DeserializeObject<JsonBase>(resstr);//获取返回值
                    ConsoleLog.Output("解散群：" + result);
                    //new Room().DeleteById(rid);//删除数据库(收到xmpp后操作)
                }
            };
            return client;
        }
        #endregion

        #region 接口退出
        /// <summary>
        /// 接口退出
        /// </summary>
        public static void UserLogout()
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "user/outtime";
            string para = "?access_token=" + Applicate.Access_Token + "&userId=" + Applicate.MyAccount.userId;
            //获取请求到的数据流
            //byte[] paras = Encoding.UTF8.GetBytes(para);//获取参数
            var client = new WebClient();
            var resstr = client.DownloadString(new Uri(url + para));
            LogHelper.log.Info("退出成功:" + resstr);
            ConsoleLog.Output("退出成功" + resstr);
        }
        #endregion

        #region 获取附近的人
        /// <summary>
        /// 获取附近的人(查找好友)
        /// </summary>
        /// <param name="currUser">已登录用户的详细信息</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="sex">性别</param>
        /// <param name="minAge">最小年龄(默认为0)</param>
        /// <param name="maxAge">最大年龄(默认为1000)</param>
        /// <param name="acticve">最后出现时间(单位为秒，默认18000)</param>
        /// <param name="pageIndex">页码(默认0)</param>
        /// <param name="pageSize">页大小(默认50)</param>
        /// <returns>返回附近的好友集合</returns>
        internal static WebClient GetNerbyFriendsAsync(string nickname, int pageIndex = 1, int pageSize = 50, double longitude = 0, double latitude = 0, string sex = "", int minAge = 0, int maxAge = 1000, int acticve = 18000)
        {
            //声明URL和需要传的参数
            string nerurl = Applicate.URLDATA.data.apiUrl + "nearby/user";
            /////////////用户令牌
            string nerpara = "access_token=" + Applicate.Access_Token + "&nickname=" + nickname +
                //////////////////经度////////////////////////////纬度//////////////////性别////////////////////最小年龄////////////////////最大年龄
                "&longitude=" + longitude + "&latitude=" + latitude + "&sex" + sex + "&minAge=" + minAge + "&maxAge=" + maxAge
                ///////////////最后出现时间////////////////页码///////////////////////////////页大小
                + "&acticve=" + acticve + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize;
            var client = new HttpClient();
            byte[] paras = Encoding.UTF8.GetBytes(nerpara);//获取参数
            client.Headers.Add("ContentLength", paras.Length.ToString());//Http头
            client.UploadDataAsync(new Uri(nerurl), paras);//异步搜索
            return client;
        }
        #endregion

        #region 添加好友
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="toUserId">需要添加的好友UserId</param>
        /// <returns>返回接口数据, 2、4分别为添加好友成功、已经是好友, 1, 3分别为</returns>
        internal static HttpClient AddFriendAsync(string toUserId)
        {
            //声明Url //string url = Application.URLDATA.data.apiUrl + "friends/add";
            string url = Applicate.URLDATA.data.apiUrl + "friends/attention/add";
            //定义参数
            string para = "access_token=" + Applicate.Access_Token + "&toUserId=" + toUserId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 获取好友列表
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <returns>查找到的好友集合</returns>
        internal static HttpClient GetFriendsAsync(int pageIndex = 1, int pageSize = 100)
        {
            //然后声明对应的URL和POST提交参数
            string url = Applicate.URLDATA.data.apiUrl + "friends/list";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&userId=" + Applicate.MyAccount.userId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 更改用户资料
        /// <summary>
        /// 更改用户资料
        /// </summary>
        /// <param name="editedUser">编辑过的用户对象</param>
        /// <returns>是否成功</returns>
        internal static HttpClient UpdateUserDetialAsync(DataOfUserDetial editedUser)
        {
            //登录页面地址
            string url = Applicate.URLDATA.data.apiUrl + "user/update";
            //声明Post提交参数
            string para = "access_token=" + Applicate.Access_Token + "&userType=" + 1 + "&nickname=" + editedUser.nickname + "&sex=" +
                editedUser.sex + "&birthday=" + editedUser.birthday + "&countryId=" + editedUser.countryId + "&provinceId=" +
                editedUser.provinceId + "&cityId=" + editedUser.cityId + "&areaId=" + editedUser.areaId; // + "&description=" + currUser.description;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 创建群聊
        internal static HttpClient CreateGroupAsync(Room room)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/add";
            string para = "access_token=" + Applicate.Access_Token + "&jid=" + room.jid + "&name=" + room.name + "&desc=" + room.desc;//showRead=0
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 创建群聊
        internal static HttpClient CreateGroupAsynctemp(Room room)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/add";
            string para = "access_token=" + Applicate.Access_Token + "&jid=" + room.jid + "&name=" + room.name + "&desc=" + room.desc;//showRead=0
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 创建带成员的群组
        /// <summary>
        /// 创建带成员的群组
        /// </summary>
        /// <param name="room"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        internal static HttpClient CreateGroupWithMembersAsync(Room room, List<string> members)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/add";
            StringBuilder para = new StringBuilder();
            para.Append("access_token=" + Applicate.Access_Token);
            para.Append("&name=" + room.name);
            para.Append("&jid=" + room.jid);
            para.Append("&desc=" + room.desc);
            para.Append("&text=" + JsonConvert.SerializeObject(members));
            //showRead=0
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 更新群组
        /// <summary>
        /// 更新群组
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        internal static HttpClient UpdateCreateGroupAsync(string roomId, List<string> member)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/update";
            string para = "access_token=" + Applicate.Access_Token + "&text=" + JsonConvert.SerializeObject(member) + "&roomId=" + roomId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 设置或取消管理员操作
        /// <summary>
        /// 设置或取消管理员操作
        /// </summary>
        /// <param name="roomId">对应的房间Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="isAdmin">对应的用户字段</param>
        /// <returns>返回的Json对象</returns>
        internal static HttpClient SetRoomAdminAsync(string roomId, string userId, int isAdmin)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/set/admin";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&touserId=" + userId + "&type=" + isAdmin;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 转让群组
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static HttpClient RoomTransferAsync(string roomId, string userId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/transfer";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&toUserId=" + userId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 成员禁言
        /// <summary>
        /// 成员禁言
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="userId"></param>
        /// <param name="talkTime"></param>
        /// <returns></returns>
        internal static HttpClient SetMemberTalkTimeAsync(string roomId, string userId, string talkTime)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/update";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&userId=" + userId + "&talkTime=" + talkTime;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 全部禁言
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="userId"></param>
        /// <param name="talkTime">开启群禁言的时候传15天的时间戳，关闭的时候传0</param>
        /// <returns></returns>
        internal static HttpClient SetRoomTalkTimeAsync(string roomId, string roomName, string talkTime)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&roomName=" + roomName + "&talkTime=" + talkTime;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 拉黑
        /// <summary>
        /// 拉黑
        /// </summary>
        internal static HttpClient BlockFriendAsync(string userId)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "friends/blacklist/add";
            //传参
            string para = "access_token=" + Applicate.Access_Token + "&toUserId=" + userId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 获取黑名单用户列表 
        /// <summary>
        /// 获取黑名单用户列表
        /// </summary>
        internal static HttpClient GetBlackListAsync()
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "friends/blacklist";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            client.UploadDataCompleted += (sender, e) =>
            {
                if (e.Error == null)
                {
                    var results = Encoding.UTF8.GetString(e.Result);
                    var blacks = JsonConvert.DeserializeObject<JsonFriends>(results);
                    var friendServer = new DataOfFriends() { status = -1 };
                    friendServer.DeleteByStatus();
                    foreach (var blkObj in blacks.data)
                    {
                        blkObj.InsertAuto();
                    }
                }
            };
            return client;
        }
        #endregion

        #region 取消拉黑
        /// <summary>
        /// 取消拉黑
        /// </summary>
        internal static HttpClient CancelBlockfriendAsync(string userId)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "friends/blacklist/delete";
            //传参
            string para = "access_token=" + Applicate.Access_Token + "&toUserId=" + userId;
            //获取请求到的数据流
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 删除好友
        /// <summary>
        /// 删除好友
        /// </summary>
        internal static HttpClient DeleteFriendAsync(string userId)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "friends/delete";
            //传参
            string para = "access_token=" + Applicate.Access_Token + "&toUserId=" + userId;
            //获取请求到的数据流
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 修改好友备注
        /// <summary>
        /// 修改好友备注
        /// </summary>
        internal static HttpClient RemarkFriendAsync(string userId, string remarkName)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "friends/remark";
            //传参
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&toUserId=" + userId + "&remarkName=" + remarkName;
            //获取请求到的数据流
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 删除/撤回消息
        /// <summary>
        /// 删除/撤回消息
        /// </summary>
        /// <param name="messageId">要删除的消息Id</param>
        /// <param name="type">聊天类型 1 单聊  2 群聊</param>
        /// <param name="delete">1： 删除属于自己的消息记录 2：撤回 删除整条消息记录</param>
        /// <returns>是否成功</returns>
        internal static HttpClient DeleteMessageAsync(string messageId, int type, int delete)
        {
            //声明URL和需要传的参数
            string url = Applicate.URLDATA.data.apiUrl + "tigase/deleteMsg";
            /////////////用户令牌
            string para = "access_token=" + Applicate.Access_Token + "&type=" + type + "&delete=" + delete + "&messageId=" + messageId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;//如果为1则成功
        }
        #endregion

        #region 用户隐私设置修改
        /// <summary>
        /// 用户隐私设置修改
        /// </summary>
        /// <param name="allowAtt">允许关注（0=不允许；1=允许）</param>
        /// <param name="allowGreet">允许打招呼（0=不允许；1=允许）</param>
        /// <param name="friendsVerify">加好友验证状态（0=不验证；1=需验证）</param>
        /// <returns></returns>
        internal static HttpClient SetConcealAsync(Settings setting)
        {
            string url = Applicate.URLDATA.data.apiUrl + "user/settings/update";//定义Url
            StringBuilder para = new StringBuilder("access_token=" + Applicate.Access_Token);
            if (setting.allowAtt == 0 || setting.allowAtt == 1)
            {
                para.Append("&allowAtt=" + setting.allowAtt);
            }
            if (setting.allowGreet == 0 || setting.allowGreet == 1)
            {
                para.Append("&allowGreet=" + setting.allowGreet);
            }
            if (setting.friendsVerify == 0 || setting.friendsVerify == 1)
            {
                para.Append("&friendsVerify=" + setting.friendsVerify);
            }
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 用户隐私设置查询
        /// <summary>
        /// 用户隐私设置查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static HttpClient GetConcealAsync(string userId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "user/settings";//定义Url
            string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 修改密码
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        internal static HttpClient ResetPasswordAsync(string oldPassword, string newPassword)
        {
            string url = Applicate.URLDATA.data.apiUrl + "user/password/update";//定义Url
            string para = "access_token=" + Applicate.Access_Token + "&oldPassword=" + oldPassword + "&newPassword=" + newPassword;
            var parabyte = Encoding.UTF8.GetBytes(para);
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 获取聊天记录
        /// <summary>
        /// 获取聊天记录
        /// <para>请求对应Jid用户的聊天记录, endTime为本地当前用户最早的消息时间</para>
        /// </summary>
        /// <param name="jid">用户(或群组)Jid</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="isGroupMessage">是否为群聊消息</param>
        /// <param name="endTime" >本地消息最早一条消息</param>
        /// <returns>查找到的   好友集合</returns>
        internal static HttpClient GetMessageAsync(string jid, long endTime, int pageIndex = 0, int pageSize = 100)
        {
            //然后声明对应的URL和POST提交参数
            string url = Applicate.URLDATA.data.apiUrl + "tigase" + ((jid.Length > 10) ? ("/shiku_muc_msgs") : ("/shiku_msgs"));
            //拼接参数
            StringBuilder para = new StringBuilder();
            para.Append("access_token=" + Applicate.Access_Token);
            para = (jid.Length > 10) ? (para.Append("&roomId=" + jid)) : (para.Append("&receiver=" + jid));//判断不同的
            para.Append("&pageSize=" + pageSize + "&startTime=1262275200000" + "&endTime=" + endTime + "&pageIndex=" + pageIndex);
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 获取上次聊天列表
        /// <summary>
        /// 获取上次聊天列表
        /// </summary>
        /// <param name="syncTime">上次离线时间</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal static HttpClient GetLastChatListAsync(string syncTime, int pageSize = 100, int pageIndex = 0)
        {
            //然后声明对应的URL和POST提交参数
            string url = Applicate.URLDATA.data.apiUrl + "tigase/getLastChatList";
            //拼接参数
            StringBuilder para = new StringBuilder();
            para.Append("access_token=" + Applicate.Access_Token);
            para.Append("&startTime=" + syncTime);
            para.Append("&pageSize=" + pageSize + "&pageIndex=" + pageIndex);
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 查询群共享
        internal static HttpClient GetRoomSharesAsync(string roomId, int pageIndex = 0, int pageSize = 0, long time = 0)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/share/find";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize;
            //if (pageSize != 0)
            //    para+= "&pageIndex=" + pageIndex + "&pageSize=" + pageSize;
            //if (time != 0)
            //    para += "&time=" + time.ToString();
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 添加群共享
        internal static HttpClient AddRoomSharesAsync(RoomShare share)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/add/share";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + share.roomId + "&type=" + share.type + "&size=" + share.size + "&userId=" + share.userId + "&url=" + share.url + "&name=" + share.name;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 删除群共享
        internal static HttpClient DelRoomSharesAsync(RoomShare share, string userId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/share/delete";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + share.roomId + "&shareId=" + share.shareId + "&userId=" + userId;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 获取群共享详情
        internal static HttpClient GetShareDetailAsync(string roomId, string shareId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/share/get";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&shareId=" + shareId;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 更新群内昵称
        internal static HttpClient UpdateGroupNickNameAsync(string roomId, string userId, string nickname)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/update";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&userId=" + userId + "&nickname=" + nickname;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 消息免打扰
        /// <summary>
        /// 设置群组消息免打扰
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="userId"></param>
        /// <param name="offlineNoPushMsg"></param>
        /// <returns></returns>
        internal static HttpClient DoNotDisturbGroupAsync(string roomId, string userId, string offlineNoPushMsg)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/member/setOfflineNoPushMsg";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&userId=" + userId + "&offlineNoPushMsg=" + offlineNoPushMsg;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 显示群聊消息已读人数
        internal static HttpClient SetGroupShowReadAsync(string roomId, string showRead)
        {
            string url = Applicate.URLDATA.data.apiUrl + "room/update";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&roomId=" + roomId + "&showRead=" + showRead;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);

            return client;
        }
        #endregion

        #region 获取标识列表
        /// <summary>
        /// 获取标识列表
        /// </summary>
        /// <param name="userId">对应</param>
        /// <returns></returns>
        internal static HttpClient GetIdCodeListAsync(string userId)
        {
            string url = Applicate.URLDATA.data.apiUrl + "label/getUserLabels";
            string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId;
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 添加标识
        internal static HttpClient CreatIdCodeListAsync(string userId, string logo, string name)
        {
            string url = ConfigurationUtil.GetValue("gladdress") + "label/create";
            //拼接参数
            string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId + "&name=" + name;
            if (!string.IsNullOrWhiteSpace(logo))
            {
                para += "&logo=" + logo;
            }
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        #region 获取后台config
        internal static HttpClient GetServerConfigAsync()
        {
            string url = Applicate.URLDATA.data.apiUrl + "/config";
            //拼接参数
            string para = "";
            var parabyte = Encoding.UTF8.GetBytes(para.ToString());
            var client = new HttpClient();
            client.Headers.Add("ContentLength", parabyte.Length.ToString());
            client.UploadDataAsync(new Uri(url), parabyte);
            return client;
        }
        #endregion

        /*
        //打开标识验证
        string url = ConfigurationUtil.GetValue("gladdress") + "label/open";
        //拼接参数
        string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId;

        //标识是否已用
        string url = ConfigurationUtil.GetValue("gladdress") + "label/queryLabelByName";
        string para = "access_token=" + Applicate.Access_Token + "&userId=" + userId + "&name=" + name;
        
        //获取标识信息
        string url = ConfigurationUtil.GetValue("gladdress") + "label/getlabel";
        string para = "access_token=" + Applicate.Access_Token + "&labelId=" + labelId;
        */

    }
}
