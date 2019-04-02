using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        private string currentPassword;
        private string newpassword;
        private bool needFriendVerify;
        private string newCheckPassword;
        private Settings Settings = new Settings();
        private SnackbarMessageQueue _snackBar;
        private bool enableVerifySwitch;
        private string versionNum;

        /// <summary>
        /// 更新好友验证Command
        /// </summary>
        public ICommand UpdateFriendVerifyCommand { get; set; }

        /// <summary>
        /// 更新密码
        /// </summary>
        public ICommand UpdatePasswordCommand { get; set; }

        public ICommand MyProperty { get; set; }

        #region Public Members

        /// <summary>
        /// 提示控件
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return _snackBar; }
            set { _snackBar = value; }
        }

        /// <summary>
        /// 新的密码(确认)
        /// </summary>
        public string NewCheckPassword
        {
            get { return newCheckPassword; }
            set { newCheckPassword = value; RaisePropertyChanged(nameof(NewCheckPassword)); }
        }

        /// <summary>
        /// 新的密码
        /// </summary>
        public string NewPassword
        {
            get { return newpassword; }
            set { newpassword = value; RaisePropertyChanged(nameof(NewPassword)); }
        }

        /// <summary>
        /// 当前密码(旧密码)
        /// </summary>
        public string CurrentPassword
        {
            get { return currentPassword; }
            set { currentPassword = value; RaisePropertyChanged(nameof(CurrentPassword)); }
        }

        /// <summary>
        /// 是否需要验证
        /// </summary>
        public bool NeedFriendVerify
        {
            get { return needFriendVerify; }
            set { needFriendVerify = value; RaisePropertyChanged(nameof(NeedFriendVerify)); }
        }

        /// <summary>
        /// 是否通过验证开关
        /// </summary>
        public bool EnableVerifySwitch
        {
            get { return enableVerifySwitch; }
            set { enableVerifySwitch = value; RaisePropertyChanged(nameof(EnableVerifySwitch)); }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNum
        {
            get { return versionNum; }
            set { versionNum = value; RaisePropertyChanged(nameof(VersionNum)); }
        }

        #endregion

        #region Contructor
        public SettingViewModel()
        {
            Snackbar = new SnackbarMessageQueue();
            GetSettingsByAPI();
            InitialCommands();
            GetServerConfig();
            VersionNum = " V 1.0";
        }
        #endregion

        private void InitialCommands()
        {
            UpdateFriendVerifyCommand = new RelayCommand(UpdateFriendVerify);
            UpdatePasswordCommand = new RelayCommand(UpdatePassword);
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        private void UpdatePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                Snackbar.Enqueue("请输入密码以修改");
                CurrentPassword = "";
                CurrentPassword = "";
                return;
            }
            if (NewPassword == "")
            {
                Snackbar.Enqueue("密码不能为空");
                NewCheckPassword = "";
                NewPassword = "";
                return;
            }
            if (Pwd_spaces(NewPassword) == false)
            {
                Snackbar.Enqueue("密码不能包含空格");
                NewCheckPassword = "";
                NewPassword = "";
                return;
            }
            if (NewPassword.Length < 6)
            {
                //Password's length Can't be smaller than 6
                Snackbar.Enqueue("密码长度不能小于6位");
                NewCheckPassword = "";
                NewPassword = "";
                return;
            }
            if (NewPassword != NewCheckPassword)
            {
                Snackbar.Enqueue("新密码设置不一致");
                NewPassword = "";
                NewCheckPassword = "";
                return;
            }
            else
            {
                //No Problem
            }
            if (NewCheckPassword == CurrentPassword)
            {
                Snackbar.Enqueue("新密码不能与旧密码相同");
                NewCheckPassword = "";
                NewPassword = "";
                return;
            }
            var client = APIHelper.ResetPasswordAsync(Helpers.MD5create(CurrentPassword), Helpers.MD5create(NewCheckPassword));
            client.UploadDataCompleted += (sen, res) =>
            {
                var restxt = Encoding.UTF8.GetString(res.Result);
                var rtn = JsonConvert.DeserializeObject<JsonBase>(restxt);
                if (rtn.resultCode == 1)
                {
                    //成功
                    CurrentPassword = "";
                    NewPassword = "";
                    NewCheckPassword = "";
                    Task.Run(() =>
                    {
                        var userObj = new LocalUser() { UserId = Applicate.MyAccount.userId }.GetModel();
                        if (userObj != null)
                        {
                            userObj.UpdatePwd(Applicate.MyAccount.Telephone, NewCheckPassword, NewCheckPassword.Length);
                        }
                    });
                    Snackbar.Enqueue("设置成功,请重新登录");
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            ShiKuManager.ApplicationExit();
                            System.Windows.Forms.Application.Restart();
                            Application.Current.Shutdown();
                        });
                    });
                }
                else
                {
                    //设置失败
                    Snackbar.Enqueue("设置失败" + rtn.resultMsg);
                }
            };
        }

        #region 判断密码中是否有空格
        private bool Pwd_spaces(string pwd)
        {
            string[] strCodes = pwd.Split(' ');
            if (strCodes.Length > 1)
            {
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 从服务器获取设置
        /// </summary>
        private void GetSettingsByAPI()
        {
            var client = APIHelper.GetConcealAsync(Applicate.MyAccount.userId);//重新获得隐私
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var sets = JsonConvert.DeserializeObject<JsonSettings>(Encoding.UTF8.GetString(res.Result));
                    if (sets != null)
                    {
                        Settings = sets.data;
                    }
                    if (Settings == null)
                    {
                        Snackbar.Enqueue("设置获取失败!", "重试", () => { GetSettingsByAPI(); });
                        return;
                    }
                    NeedFriendVerify = Settings.friendsVerify == 1 ? true : false;
                }
                else
                {

                }
            };
        }

        #region 获取服务器上的config
        /// <summary>
        /// 获取服务器上的config
        /// </summary>
        /// <param name="lastTimeLen"></param>
        /// <returns></returns>
        public static HttpClient GetServerConfig()
        {
            var client = APIHelper.GetServerConfigAsync();
            client.UploadDataCompleted += (sen, eve) =>
            {
                if (eve.Error != null)
                {
                    var restxt = Encoding.UTF8.GetString(eve.Result);//
                }
            };
            return client;
        }
        #endregion

        #region 更新好友验证
        /// <summary>
        /// 更新好友验证
        /// </summary>
        private void UpdateFriendVerify()
        {
            EnableVerifySwitch = false;//暂时禁用开关
            Settings.friendsVerify = (NeedFriendVerify == true) ? 1 : 0;
            var client = APIHelper.SetConcealAsync(Settings);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error != null)
                {
                    var result = JsonConvert.DeserializeObject<JsonBase>(Encoding.UTF8.GetString(res.Result));
                    if (result.resultCode == 1)
                    {
                        Snackbar.Enqueue("已启用好友验证", true);
                        EnableVerifySwitch = true;//启用开关
                    }
                    else
                    {
                        Snackbar.Enqueue("设置失败\n" + result.resultMsg);
                        EnableVerifySwitch = true;//启用开关
                    }
                }
                else
                {
                    EnableVerifySwitch = true;//启用开关
                }
            };
        }
        #endregion

    }
}
