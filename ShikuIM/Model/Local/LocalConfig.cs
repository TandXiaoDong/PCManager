using GalaSoft.MvvmLight;
using System.IO;
using System;
using ShikuIM.Model;

namespace ShikuIM.Model
{

    /// <summary>
    /// 本地部分信息配置
    /// </summary>
    public class LocalConfig : ObservableObject
    {
        #region Private Member
        private string chatDownloadPath;
        public static string userAvatorPath;
        private string tempFilepath;
        private string chatPath;
        private string catchPath;
        private string messageDatabasePath;

        #endregion

        #region Public Member

        /// <summary>
        /// 用户头像路径
        /// </summary>
        public string UserAvatorFolderPath
        {
            get { return userAvatorPath; }
            set
            {
                userAvatorPath = value;
                RaisePropertyChanged(nameof(UserAvatorFolderPath));
            }
        }

        /// <summary>
        /// 聊天记录下载路径
        /// </summary>
        public string ChatDownloadPath
        {
            get { return chatDownloadPath; }
            set
            {
                chatDownloadPath = value;
                RaisePropertyChanged(nameof(ChatDownloadPath));
            }
        }

        /// <summary>
        /// 聊天记录下载临时数据路径
        /// </summary>
        public string TempFilepath
        {
            get { return tempFilepath; }
            set
            {
                tempFilepath = value;
                RaisePropertyChanged(nameof(TempFilepath));
            }
        }

        /// <summary>
        /// 聊天文件
        /// </summary>
        public string ChatPath
        {
            get { return chatPath; }
            set { chatPath = value; RaisePropertyChanged(nameof(ChatPath)); }
        }

        /// <summary>
        /// 缓存目录
        /// </summary>
        public string CatchPath
        {
            get { return catchPath; }
            set { catchPath = value; RaisePropertyChanged(nameof(CatchPath)); }
        }

        /// <summary>
        /// 消息数据库路径
        /// </summary>
        public string MessageDatabasePath
        {
            get { return messageDatabasePath; }
            set { messageDatabasePath = value; }
        }

        #endregion

        #region Method

        /// <summary>
        /// 根据UserId获取对应头像路径
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>拼接好的头像路径</returns>
        public string GetDisplayAvatorPath(string userId)
        {
            if (!Directory.Exists(UserAvatorFolderPath))
            {
                Directory.CreateDirectory(UserAvatorFolderPath);
            }
            string avatorPath = FileUtil.GetDirFiles(UserAvatorFolderPath,userId);
            LogHelper.log.Debug("***************是否存在完成文件名："+avatorPath+"   userID:"+userId);
            userAvatorPath = UserAvatorFolderPath;
            //string avatarpath = UserAvatorFolderPath + userId + ".png";
            string avatarpath = "";
            if (!File.Exists(avatorPath))
                avatarpath = UserAvatorFolderPath + avatorPath;
            else
                avatarpath = avatorPath;

            if (userId.Length < 25)
            {
                if (userId.Length < 7)
                {
                    switch (userId)
                    {
                        case "10000"://1000号为系统公众号
                            return Helpers.GetCurrentProjectPath() + "\\Resource\\Avator\\avatar_notice.png";//系统公众号
                        case "10001"://10001为好友验证账号
                            return Helpers.GetCurrentProjectPath() + "\\Resource\\Avator\\avatar_newfriends.png";//系统公众号
                        default:
                            return Helpers.GetCurrentProjectPath() + "\\Resource\\Avator\\avatar_default.png";
                    }
                }
                else
                {
                    if (File.Exists(avatarpath))//如果头像存在则返回原始头像
                    {
                        return avatarpath;//返回用户自定义头像
                    }
                    else//不存在则返回默认头像
                    {
                        LogHelper.log.Debug("===============不存在则返回默认头像 " + avatarpath);
                        return Helpers.GetCurrentProjectPath() + "\\Resource\\Avator\\avatar_default.png";
                    }
                }
            }
            else
            {
                return Helpers.GetCurrentProjectPath() + "\\Resource\\Avator\\avatar_group.png";
            }
        }

        /// <summary>
        /// 根据UserId获取对应头像路径
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>拼接好的头像路径</returns>
        public string GetDownloadAvatorPath(string userId)
        {
            string fullPath = FileUtil.GetDirFiles(UserAvatorFolderPath, userId);
            FileUtil.DeleteUserAvatar(fullPath);
            LogHelper.log.Debug("***********已删除："+fullPath);
            string avatarName = DataOfUserDetial._avatarName;
            string fileName = FileUtil.GetFileName(fullPath, true);


            if (string.IsNullOrEmpty(avatarName))
            {
                avatarName = DateTime.Now.ToString("yyyyMMddHHmmss");
                DataOfUserDetial._avatarName = avatarName;
            }
            else
            {
                DataOfUserDetial._avatarName = fileName;
            }
            if (!Directory.Exists(UserAvatorFolderPath))
            {
                Directory.CreateDirectory(UserAvatorFolderPath);
            }
            //保存当前用户图片格式：用户ID+_图片名称
            return UserAvatorFolderPath + userId + "_" + avatarName + ".png";
        }

        #endregion


    }
}
