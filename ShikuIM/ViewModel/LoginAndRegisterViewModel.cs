using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using ShikuIM.Security;
using ShikuIM.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// 登录注册ViewModel
    /// </summary>
    public class LoginAndRegisterViewModel : ViewModelBase
    {

        #region Public Static Member
        /// <summary>
        /// 错误消息
        /// </summary>
        public static string ErrorMessage { get; } = nameof(ErrorMessage);

        #endregion

        #region 标识国家地区的类型
        /// <summary>
        /// 标识国家地区的类型
        /// </summary>
        enum AreasType
        {
            Country = 1,
            Province = 2,
            City = 3,
            Area = 4
        }
        #endregion

        #region Private Member
        #region Login properties
        private string userId;
        private bool usernameEnable;
        private bool loginPwdEnabled;
        private bool loginBtnEnabled;
        private bool rememberPwdEnabled;
        private bool gotoRegisterBtnEnabled;
        private bool isRememberPwd;
        private bool _isVisitorLogin;
        private Visibility _visiShowKey;
        private bool _visitorLoginEnabled;
        private string visitorKey;
        #endregion
        #region Register Properies
        private string phoneNumber;
        private SecureString rPassword;
        private DateTime bornDate = DateTime.Now;
        private int selectedCountry;
        private int selectedProvince;
        private int selectedCity;
        private int selectedArea;
        private ImageBrush uploadedAvator;
        private int avatorUploadPage;
        private string rNickname;
        private bool registerProgressVisible;
        private int registerProgress;
        private bool enablePhonenumber;
        private bool enableRegisterPasswords;
        private bool uploadBtnEnable;
        private string AvatorPath;
        private int loginPageIndex = 0;
        private bool isLogining;
        string UploadfilePath;
        private int gender = 1;
        private int selectedCountryCode;
        private int _rSelectedCountryCode;

        #endregion

        /// <summary>
        /// 真实密码
        /// </summary>
        private string TruePasswordWhenRemembered { get; set; }
        private bool enabledRegister = true;
        private SnackbarMessageQueue snackBar;
        /// <summary>
        /// 是否为   取消记住密码后，输入新密码，又记住密码
        /// </summary>
        private bool IsCancelRememberPwdAndTextNewPwd { get; set; }
        #endregion

        #region Public Member


        /// <summary>
        /// 位置定位器
        /// </summary>
        public GeoCoordinateWatcher GPSLocator { get; set; }

        /// <summary>
        /// 获取到的地理位置
        /// </summary>
        public GeoCoordinate Location { get; set; }

        #region Login Properties


        /// <summary>
        /// 是否启用注册
        /// </summary>
        public bool EnabledRegister
        {
            get { return enabledRegister; }
            set { enabledRegister = value; RaisePropertyChanged(nameof(EnabledRegister)); }
        }

        /// <summary>
        /// 用户名启用状态
        /// </summary>
        public bool UsernameEnabled
        {
            get { return usernameEnable; }
            set
            {
                usernameEnable = value;
                RaisePropertyChanged(nameof(UsernameEnabled));
            }
        }

        /// <summary>
        /// 密码启用状态
        /// </summary>
        public bool LoginPwdEnabled
        {
            get { return loginPwdEnabled; }
            set
            {
                loginPwdEnabled = value;
                RaisePropertyChanged(nameof(LoginPwdEnabled));
            }
        }

        /// <summary>
        /// 记住密码启用状态
        /// </summary>
        public bool RememberPwdEnabled
        {
            get { return rememberPwdEnabled; }
            set
            {
                rememberPwdEnabled = value;
                RaisePropertyChanged(nameof(RememberPwdEnabled));
            }
        }

        /// <summary>
        /// 是否游客登录启用状态
        /// </summary>
        public bool VisitorLoginEnabled
        {
            get { return _visitorLoginEnabled; }
            set
            {
                _visitorLoginEnabled = value;
                RaisePropertyChanged(nameof(VisitorLoginEnabled));
            }
        }

        /// <summary>
        /// 注册按钮启用状态
        /// </summary>
        public bool GotoRegisterBtnEnabled
        {
            get { return gotoRegisterBtnEnabled; }
            set { gotoRegisterBtnEnabled = value; RaisePropertyChanged(nameof(GotoRegisterBtnEnabled)); }
        }

        /// <summary>
        /// 登录按钮启用状态
        /// </summary>
        public bool LoginBtnEnabled
        {
            get { return loginBtnEnabled; }
            set
            {
                loginBtnEnabled = value;
                RaisePropertyChanged(nameof(LoginBtnEnabled));
            }
        }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserId
        {
            get { return userId; }
            set { userId = value; RaisePropertyChanged(nameof(UserId)); }
        }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool IsRememberPwd
        {
            get { return isRememberPwd; }
            set
            {
                isRememberPwd = value;
                RaisePropertyChanged(nameof(IsRememberPwd));
            }
        }

        /// <summary>
        /// 游客key
        /// </summary>
        public string VisitorKey
        {
            get { return visitorKey; }
            set
            {
                visitorKey = value;
                RaisePropertyChanged(nameof(VisitorKey));
            }
        }

        /// <summary>
        /// 是否游客登录
        /// </summary>
        public bool IsVisitorLogin
        {
            get { return _isVisitorLogin; }
            set
            {
                _isVisitorLogin = value;
                RaisePropertyChanged(nameof(IsVisitorLogin));
                if (value == true)
                {
                    //visiShowKey = Visibility.Visible;
                    UsernameEnabled = false;//用户框
                    LoginPwdEnabled = false;//密码框
                    RememberPwdEnabled = false;
                }
                else
                {
                    //visiShowKey = Visibility.Collapsed;
                    UsernameEnabled = true;//用户框
                    LoginPwdEnabled = true;//密码框
                    RememberPwdEnabled = true;
                }
            }
        }

        /// <summary>
        /// 游客输入Key
        /// </summary>
        public Visibility visiShowKey
        {
            get { return _visiShowKey; }
            set
            {
                _visiShowKey = value;
                RaisePropertyChanged(nameof(visiShowKey));
            }
        }

        private string _txtServer;
        /// <summary>
        /// 游客输入server
        /// </summary>
        public string txtServer
        {
            get { return _txtServer; }
            set
            {
                _txtServer = value;
                RaisePropertyChanged(nameof(txtServer));
            }
        }

        private string _txtServerOfKey;
        /// <summary>
        /// 游客输入serverOfKey
        /// </summary>
        public string txtServerOfKey
        {
            get { return _txtServerOfKey; }
            set
            {
                _txtServerOfKey = value;
                RaisePropertyChanged(nameof(txtServerOfKey));

                try
                {
                    int index = value.LastIndexOf("?");
                    VisitorKey = value.Substring(index + 1);

                    int index2 = value.LastIndexOf("=");
                    VisitorKey = value.Substring(index2 + 1);
                }
                catch
                {
                    VisitorKey = "1";
                }
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public bool IsLogining
        {
            get { return isLogining; }
            set
            {
                isLogining = value;
                //延迟200ms执行以确认切换页面
                Task.Run(() =>
                {
                    Thread.Sleep(280);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        RaisePropertyChanged(nameof(IsLogining));
                    });
                });
            }
        }

        /// <summary>
        /// 是否启用登录控件
        /// </summary>
        public bool EnabledLogin
        {
            get
            {
                return UsernameEnabled && LoginPwdEnabled && RememberPwdEnabled && GotoRegisterBtnEnabled && LoginBtnEnabled && !IsLogining;
            }
            set
            {
                UsernameEnabled = value;//用户框
                LoginPwdEnabled = value;//密码框
                RememberPwdEnabled = value;//记住密码
                GotoRegisterBtnEnabled = value;//注册
                LoginBtnEnabled = value;//登录
                IsLogining = !value;//设置登录按钮
                VisitorLoginEnabled = value;//游客登录
            }
        }
        #endregion

        #region Register Properies

        /// <summary>
        /// 是否启用电话号码
        /// </summary>
        public bool EnablePhonenumber
        {
            get { return enablePhonenumber; }
            set
            {
                enablePhonenumber = value;
                RaisePropertyChanged(nameof(EnablePhonenumber));
            }
        }

        /// <summary>
        /// 是否启用注册密码
        /// </summary>
        public bool EnableRegisterPasswords
        {
            get { return enableRegisterPasswords; }
            set { enableRegisterPasswords = value; RaisePropertyChanged(nameof(EnableRegisterPasswords)); }
        }

        /// <summary>
        /// 注册进度
        /// </summary>
        public int RegisterProgress
        {
            get { return registerProgress; }
            set
            {
                registerProgress = value;
                RaisePropertyChanged(nameof(RegisterProgress));
            }
        }

        /// <summary>
        /// 注册进度条显示
        /// </summary>
        public bool RegisterProgressVisible
        {
            get { return registerProgressVisible; }
            set
            {
                registerProgressVisible = value;
                RaisePropertyChanged(nameof(RegisterProgressVisible));
            }
        }

        /// <summary>
        /// 注册电话号码
        /// </summary>
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; RaisePropertyChanged(nameof(PhoneNumber)); }
        }

        /// <summary>
        /// 注册密码
        /// </summary>
        public SecureString RPassword
        {
            get { return rPassword; }
            set { rPassword = value; RaisePropertyChanged(nameof(RPassword)); }
        }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime BornDate
        {
            get { return bornDate; }
            set { bornDate = value; RaisePropertyChanged(nameof(BornDate)); }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string RNickname
        {
            get { return rNickname; }
            set { rNickname = value; RaisePropertyChanged(nameof(RNickname)); }
        }

        /// <summary>
        /// Gender(not for binding)
        /// </summary>
        public int Gender
        {
            get { return gender; }
            set { gender = value; RaisePropertyChanged(nameof(Gender)); }
        }

        /// <summary>
        /// 上传按钮是否启用
        /// </summary>
        public bool UploadBtnEnable
        {
            get { return uploadBtnEnable; }
            set { uploadBtnEnable = value; RaisePropertyChanged(nameof(UploadBtnEnable)); }
        }

        /// <summary>
        /// 选中国家
        /// </summary>
        public int SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                selectedCountry = value;
                RaisePropertyChanged(nameof(SelectedCountry));
                ProvinceList.Clear();
                var tmp = GetLocationList(AreasType.Province).ToObservableCollection();
                SelectedProvince = -1;
                ProvinceList.AddRange(tmp);
            }
        }

        /// <summary>
        /// 选中省份
        /// </summary>
        public int SelectedProvince
        {
            get { return selectedProvince; }
            set
            {
                selectedProvince = value;
                RaisePropertyChanged(nameof(SelectedProvince));
                CityList.Clear();
                var tmp = GetLocationList(AreasType.City).ToObservableCollection();
                SelectedCity = -1;//不选中以避免显示验证错误信息
                CityList.AddRange(tmp);//填充列表
            }
        }

        /// <summary>
        /// 选中城市
        /// </summary>
        public int SelectedCity
        {
            get { return selectedCity; }
            set
            {
                selectedCity = value;
                RaisePropertyChanged(nameof(SelectedCity));
                AreaList.Clear();
                var areas = GetLocationList(AreasType.Area).ToObservableCollection();
                SelectedArea = -1;
                AreaList.AddRange(areas);
            }
        }

        /// <summary>
        /// 选中区县
        /// </summary>
        public int SelectedArea
        {
            get { return selectedArea; }
            set
            {
                selectedArea = value;
                RaisePropertyChanged(nameof(SelectedArea));
            }
        }

        /// <summary>
        /// 头像控件页面
        /// </summary>
        public int AvatorUploadPage
        {
            get { return avatorUploadPage; }
            set
            {
                avatorUploadPage = value;
                RaisePropertyChanged(nameof(AvatorUploadPage));
            }
        }

        /// <summary>
        /// 上传完成后的头像
        /// </summary>
        public ImageBrush UploadedAvator
        {
            get { return uploadedAvator; }
            set { uploadedAvator = value; RaisePropertyChanged(nameof(UploadedAvator)); }
        }

        /// <summary>
        /// 国家列表
        /// </summary>
        public ObservableCollection<Areas> CountryList { get; set; }

        /// <summary>
        /// 省列表
        /// </summary>
        public ObservableCollection<Areas> ProvinceList { get; set; }

        /// <summary>
        /// 城市列表
        /// </summary>
        public ObservableCollection<Areas> CityList { get; set; }

        /// <summary>
        /// 地区列表
        /// </summary>
        public ObservableCollection<Areas> AreaList { get; set; }

        /// <summary>
        /// 国家和区域列表
        /// </summary>
        public ObservableCollection<Country> GobalAreaList { get; set; }

        #endregion

        /// <summary>
        /// 注册时选中的国家编号
        /// </summary>
        public int RSelectedCountryCode
        {
            get { return _rSelectedCountryCode; }
            set { _rSelectedCountryCode = value; RaisePropertyChanged(nameof(RSelectedCountryCode)); }
        }


        /// <summary>
        /// 选中的地区索引
        /// </summary>
        public int SelectedCountryCode
        {
            get { return selectedCountryCode; }
            set { selectedCountryCode = value; RaisePropertyChanged(nameof(SelectedCountryCode)); }
        }

        ///<summary>
        ///页数判断
        /// </summary>
        public int LoginPageIndex
        {
            get { return loginPageIndex; }
            set
            {
                loginPageIndex = value;
                RaisePropertyChanged(nameof(loginPageIndex));
            }
        }

        /// <summary>
        /// 通用提示
        /// </summary>
        public SnackbarMessageQueue SnackBar
        {
            get { return snackBar; }
            set
            {
                snackBar = value;
                RaisePropertyChanged(nameof(snackBar));
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// 设置
        /// </summary>
        public ICommand SettingCommand
        {
            get
            {
                return new RelayCommand(() => 
                {
                    LoginSettingsWindow.getSetUp().Show();
                });
            }
        }

        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Applicate.GetWindow<Login>().Close();
                    Applicate.AccountDbContext.Dispose();
                    Application.Current.Shutdown();//在登录界面点击关闭时，关闭整个程序
                });
            }
        }

        /// <summary>
        /// 登录命令
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// 注册命令
        /// </summary>
        public ICommand RegisterCommand { get; set; }

        /// <summary>
        /// 记住密码命令
        /// </summary>
        public ICommand RememberPwdCommand { get; set; }

        /// <summary>
        /// 电话号码失去焦点
        /// </summary>
        public ICommand RegisterTxtLostFocus { get; set; }

        /// <summary>
        /// Female 或 Male选项改变
        /// </summary>
        public ICommand FemaleOrMaleCommand { get; set; }

        /// <summary>
        /// 用户名称改变
        /// </summary>
        public ICommand UsernameTextChangeCommand { get; set; }

        /// <summary>
        /// 上传头像
        /// </summary>
        public ICommand UploadAvatorCommand { get; set; }

        /// <summary>
        /// 剪贴板复制日志路径
        /// </summary>
        public ICommand LogPathCopy => new RelayCommand(() => { Clipboard.SetDataObject("C:\\temp\\ShikuLog", true); });

        /// <summary>
        /// 密码发生改变时
        /// </summary>
        public ICommand PasswordChanged { get; set; }

        #endregion

        #region Contructor
        public LoginAndRegisterViewModel()
        {
            SelectedCountryCode = 6;//默认选中中国
            RSelectedCountryCode = 6;
            EnabledLogin = true;//允许登录
            SnackBar = new SnackbarMessageQueue();
            #region Initial Commands
            LoginCommand = new RelayCommand<IHavePassword>(UserLogin);
            RegisterCommand = new RelayCommand<IHavePassword>(UserRegisterAccount);
            UsernameTextChangeCommand = new RelayCommand<IHavePassword>(UsernameChanged);
            UploadAvatorCommand = new RelayCommand(AvatorUpload);
            FemaleOrMaleCommand = new RelayCommand<object>(FemaleOrMaleChoice);
            RememberPwdCommand = new RelayCommand<bool>(RememberPassword);
            RegisterTxtLostFocus = new RelayCommand(CheckPhoneNumber);
            PasswordChanged = new RelayCommand<RoutedEventArgs>(PasswordChange);

            visiShowKey = Visibility.Collapsed;
            IsVisitorLogin = false;
            #endregion

            #region Initial Location Lists
            GPSLocator = new GeoCoordinateWatcher();
            Location = new GeoCoordinate();
            Task.Run(() =>
            {
                GPSLocator.TryStart(false, TimeSpan.FromMilliseconds(5000));////超过5S则返回False;
                Location = GPSLocator.Position.Location;//获取位置
            });
            CountryList = GetLocationList(AreasType.Country).ToObservableCollection();
            ProvinceList = new ObservableCollection<Areas>();
            ProvinceList.AddRange(new Areas() { parent_id = 1, type = 2 }.GetChildrenList().ToObservableCollection());
            CityList = new ObservableCollection<Areas>();
            AreaList = new ObservableCollection<Areas>();
            //GobalAreaList = new ObservableCollection<Country>();
            GobalAreaList = Country.GetCountries().ToObservableCollection();//国家和地区列表
            #endregion

            if (Debugger.IsAttached)
            {
                //UserId = "15625294668";
            }
            Messenger.Default.Register<bool>(this, LoginNotifications.InitialAccount, (para) => { InitialAccount(); });
            Messenger.Default.Register<string>(this, LoginAndRegisterViewModel.ErrorMessage, (texts) => { SnackBar.Enqueue(texts, "重试", () => { ShiKuManager.GetConfigAsync(); }); });
        }
        #endregion

        #region 密码改变时候
        /// <summary>
        /// 密码改变时候
        /// </summary>
        /// <param name="args">事件</param>
        private void PasswordChange(RoutedEventArgs args)
        {
            var tmp = args.Source as PasswordBox;
            var tmmm = tmp.Password;
            IsCancelRememberPwdAndTextNewPwd = true;
        }
        #endregion

        #region 检查电话号码是否注册
        /// <summary>
        /// 检查电话号码是否注册
        /// </summary>
        private void CheckPhoneNumber()
        {
            var client = APIHelper.TelephoneVerifyAsync(PhoneNumber);
            client.UploadDataCompleted += (sen, eve) =>
            {
                if (eve.Error == null)
                {

                    var res = Encoding.UTF8.GetString(eve.Result);
                    var result = JsonConvert.DeserializeObject<JsonBase>(res);
                    if (result.resultCode != 1)
                    {
                        SnackBar.Enqueue(result.resultMsg);//电话验证出错
                    }
                    else
                    {
                    }
                }
                else
                {
                    SnackBar.Enqueue(eve.Error.Message);//显示网络错误
                }
            };
        }
        #endregion

        #region 初始化账号
        private void InitialAccount()
        {
            var recentUser = new LocalUser().GetLastUserByTime();
            if (recentUser != null)
            {
                UserId = recentUser.Telephone;//如果最近登录账号为当前输入账号
                var login = Applicate.GetWindow<IHavePassword>();
                login.LoginSecurePassword = new SecureString();
                int length = Convert.ToInt32(recentUser.PasswordLength);//获取上次登录成功后的密码长度
                string fakePwd = Guid.NewGuid().ToString("N").Substring(0, length);//随机生成假的密码
                login.LoginSecurePassword = fakePwd.ToSecureString();//设置显示密码为假密码
                TruePasswordWhenRemembered = recentUser.Password;//设置真实密码
                if (!string.IsNullOrWhiteSpace(recentUser.Password))
                {
                    IsRememberPwd = true;
                }
            }
        }
        #endregion

        #region 记住密码与取消记住密码
        public void RememberPassword(bool isChecked)
        {
            if (!isChecked)
            {
                new LocalUser().Delete(UserId);//移除数据库中记住过的用户
            }
        }
        #endregion

        #region 初始化注册属性
        /// <summary>
        /// 初始化注册属性
        /// </summary>
        public void InitialRegisterProperties()
        {
            this.GotoRegisterBtnEnabled = true;
            this.BornDate = DateTime.Now;
            this.EnabledLogin = true;
            this.EnableRegisterPasswords = true;
            this.LoginBtnEnabled = true;
            this.LoginPwdEnabled = true;
            this.RNickname = "";
            this.LoginPageIndex = 0;
            this.PhoneNumber = "";
            this.GotoRegisterBtnEnabled = true;
            AvatorUploadPage = 0;//
            UploadedAvator = new ImageBrush();//清空头像
            if (IsVisitorLogin != true)
            {
                var pass = Applicate.GetWindow<Login>();
                App.Current.Dispatcher.Invoke(() =>
                {
                    pass.FirstRegisterPassword.Password = "";
                    pass.RegisterPasswordBox.Password = "";
                //pass.FinalRegisterSecurePassword = new SecureString(tm, 0);
            });
            }
            //this.
        }
        #endregion

        #region 性别选择
        private void FemaleOrMaleChoice(object genders)
        {
            string gen = (string)genders;
            if (gen == "0")
            {
                gender = 0;
            }
            else
            {
                gender = 1;
            }
        }
        #endregion

        #region 头像上传
        private void AvatorUpload()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "图片文件(*.bmp, *.jpg,*.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png|所有文件(*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //获得路径
                UploadfilePath = openFileDialog.FileName;
                AvatorPath = UploadfilePath;//设置头像
                ImageSource avatorsource = Helpers.ConvertBitmapToBitmapSource(
                    FileUtil.ReadFileByteToBitmap(UploadfilePath).BItmapSourceToBitmap());
                UploadedAvator = new ImageBrush(avatorsource);//重新从路径获取
                AvatorUploadPage = 1;//显示头像
            }
        }
        #endregion

        #region 用户名改变时
        /// <summary>
        /// 用户名改变时
        /// </summary>
        /// <param name="Login"></param>
        private void UsernameChanged(IHavePassword Login)
        {
            IsRememberPwd = false;//设置为空

            Task.Run(() =>
            {
                var list = new LocalUser().GetAllList().Where(d => d.Telephone == UserId);
                if (list.Count() > 0 && !string.IsNullOrWhiteSpace(list.First().Password))
                {
                    int pwdLength = 0;
                    if (int.TryParse(list.First().PasswordLength.ToString(), out pwdLength) && pwdLength > 0)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            if (TruePasswordWhenRemembered != null)
                            {
                                Login.LoginSecurePassword = TruePasswordWhenRemembered.Substring(0, pwdLength).ToSecureString();
                                IsRememberPwd = true;
                            }
                        });
                    }
                }
                else
                {
                    //  if (TruePasswordWhenRemembered.StartsWith(UserId))
                    //  {
                    //      App.Current.Dispatcher.Invoke(() =>
                    //      {
                    //          Login.LoginSecurePassword = null;
                    //          IsRememberPwd = false;
                    //      });
                    //  }
                }
            });
        }
        #endregion

        #region 用户登录
        private void UserLogin(IHavePassword parameter)
        {
            Task.Run(() =>
            {
                if (IsVisitorLogin == true)//游客登录
                {
                    Applicate.URLDATA.data.apiUrl = ConfigurationUtil.GetValue("InitialServer") + "/";
                    VisitorKey = ConfigurationUtil.GetValue("InitialServer_key");

                    int index = VisitorKey.LastIndexOf("?");
                    VisitorKey = VisitorKey.Substring(index + 1);

                    int index2 = VisitorKey.LastIndexOf("=");
                    VisitorKey = VisitorKey.Substring(index2 + 1);
                    VisitorLogin();
                    return;
                }

                //接收用户名和密码
                string password;
                if (IsRememberPwd &&
                TruePasswordWhenRemembered != null &&
                !IsCancelRememberPwdAndTextNewPwd)//如果当前处于记住有效密码 且 不为加载密码后重新输入的密码
                {
                    password = TruePasswordWhenRemembered;//使用数据库密码
                    //parameter.LoginSecurePassword = TruePasswordWhenRemembered.ToSecureString();
                }
                else//没记住密码 或 
                {
                    password = parameter.LoginSecurePassword.UnSecure();//使用文本框内密码
                }
                EnabledLogin = false;//暂时禁用登录
                //获取经纬度
                string Longitude = "0";
                string Latitude = "0";
                if (Location.IsUnknown != true)
                {
                    Longitude = Location.Longitude.ToString();
                    Latitude = Location.Latitude.ToString();
                }
                try
                {
                    var client = ShiKuManager.ShiKuLogin(UserId, password, Latitude, Longitude, GobalAreaList[SelectedCountryCode].prefix.ToString());
                    client.UploadDataCompleted += LoginComplete;
                }
                catch (Exception ex)
                {
                    SnackBar.Enqueue("登录失败:" + ex.Message, "重试", () => { LoginCommand.Execute(Applicate.GetWindow<IHavePassword>()); });
                    Console.WriteLine("登录失败:" + ex.Message);
                    EnabledLogin = true;//启用登录
                }
            });
        }
        #endregion

        #region 完成登录后
        private void LoginComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)//网络正常
            {
                string res = Encoding.UTF8.GetString(e.Result);
                var curruser = JsonConvert.DeserializeObject<Jsonuser>(res);//获取登录用户详情
                var client = (HttpClient)sender;
                string pwd = client.Tag3.ToString();//获取密码
                if (curruser != null && curruser.resultCode == 1)
                {
                    Task.Run(() =>
                    {
                        var dblocalUser = new LocalUser();//保存最近消息
                        dblocalUser.LastLoginTime = Helpers.DatetimeToStamp(DateTime.Now);//记录登录时间
                        dblocalUser.Telephone = UserId;//电话号码
                        dblocalUser.UserId = curruser.data.userId;//UserId
                        if (IsRememberPwd)//如果选中记住密码(保存密文密码到数据库)
                        {
                            dblocalUser.Password = pwd;//存储密码
                            dblocalUser.PasswordLength = pwd.Length;//密码长度
                        }
                        dblocalUser.InsertOrUpdatePassword();//写入数据库
                        //成功后关闭登录
                        Messenger.Default.Send(true, Login.CloseWindow);
                    });

                }
                else
                {
                    //如ResultCode不为1的话，就输出错误信息，，，，，，并在界面给予提示
                    SnackBar.Enqueue(curruser.resultMsg, true);
                    EnabledLogin = true;//失败后启用登录按-钮
                }
            }
            else//网络错误
            {
                var error = e.Error as WebException;
                SnackBar.Enqueue("登录失败！\n" + error.Message, false);
                EnabledLogin = true;//失败后启用登录按钮
            }
        }
        #endregion

        #region 游客登录
        /// <summary>
        /// 游客登录
        /// </summary>
        private void VisitorLogin()
        {
            //获取经纬度
            string Longitude = "0";
            string Latitude = "0";
            if (Location.IsUnknown != true)
            {
                Longitude = Location.Longitude.ToString();
                Latitude = Location.Latitude.ToString();
            }
            try
            {
                var client = ShiKuManager.ShiKuVisitorLogin(VisitorKey, Latitude, Longitude);
                client.UploadDataCompleted += VisitorLoginComplete;
            }
            catch (Exception ex)
            {
                SnackBar.Enqueue("登录失败:" + ex.Message, "重试", () => { LoginCommand.Execute(Applicate.GetWindow<IHavePassword>()); });
                Console.WriteLine("登录失败:" + ex.Message);
                EnabledLogin = true;//启用登录
            }
        }
        #endregion

        /// <summary>
        /// 完成游客登录后
        /// </summary>>
        private void VisitorLoginComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)//网络正常
            {
                string res = Encoding.UTF8.GetString(e.Result);
                var curruser = JsonConvert.DeserializeObject<Jsonuser>(res);//获取登录用户详情
                var client = (HttpClient)sender;
                string pwd = client.Tag3.ToString();//获取密码
                if (curruser != null && curruser.resultCode == 1)
                {
                    Task.Run(() =>
                    {
                        var dblocalUser = new LocalUser();//保存最近消息
                        dblocalUser.LastLoginTime = Helpers.DatetimeToStamp(DateTime.Now);//记录登录时间
                        dblocalUser.Telephone = UserId;//电话号码
                        dblocalUser.UserId = curruser.data.userId;//UserId

                        dblocalUser.InsertOrUpdatePassword();//写入数据库
                        //成功后关闭登录
                        Messenger.Default.Send(true, Login.CloseWindow);
                    });

                }
                else
                {
                    //如ResultCode不为1的话，就输出错误信息，，，，，，并在界面给予提示
                    SnackBar.Enqueue(curruser.resultMsg, true);
                    EnabledLogin = true;//失败后启用登录按-钮
                }
            }
            else//网络错误
            {
                var error = e.Error as WebException;
                SnackBar.Enqueue("登录失败！\n" + error.Message, false);
                EnabledLogin = true;//失败后启用登录按钮
            }
        }

        #region 用户注册
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="parameter">包含密码的Login对象</param>
        private void UserRegisterAccount(IHavePassword parameter)
        {
            if (UploadfilePath == null)
            {
                SnackBar.Enqueue("请选择头像!");
                return;
            }
            if (rNickname == null || rNickname.Trim() == "")
            {
                SnackBar.Enqueue("昵称不能为空!");
                return;
            }
            if (parameter.FirstRegisterSecurePassword.UnSecure().Length < 6)
            {
                SnackBar.Enqueue("密码长度不能小于6位!");
                return;
            }
            if (parameter.FirstRegisterSecurePassword.UnSecure() != parameter.FinalRegisterSecurePassword.UnSecure())
            {
                SnackBar.Enqueue("两次输入密码不一致!");
                return;
            }
            /*
            else if (string.IsNullOrEmpty(AvatorPath))
            {
                SnackBar.Enqueue("请选择需要上传的头像!");
                return;
            }
            */

            HttpClient client = APIHelper.RegisterAccountAsync(
                PhoneNumber, GobalAreaList[RSelectedCountryCode].prefix.ToString(),
                parameter.FinalRegisterSecurePassword.UnSecure(),
                RNickname,
                gender,
                Helpers.DatetimeToStamp(BornDate),
                SelectedCountry,
                SelectedProvince,
                SelectedCity,
                SelectedArea);
            EnabledRegister = false;//暂时禁用控件
            //指定注册成功事件
            client.UploadDataCompleted += RegisterComplete;
        }
        #endregion

        #region 注册成功
        /// <summary>
        /// 注册成功
        /// </summary>
        /// <param name="sender">Client</param>
        /// <param name="e"></param>
        private void RegisterComplete(object sender, UploadDataCompletedEventArgs e)
        {
            Task.Run(() =>
            {
                if (e.Error == null)
                {
                    string result = Encoding.UTF8.GetString(e.Result);
                    var resuser = JsonConvert.DeserializeObject<RegisterModel>(result);
                    Task.Run(() =>
                    {
                        var client = ShiKuManager.UploadAvator(resuser.Data.userId, UploadfilePath);
                        client.UploadDataCompleted += (sen, eve) =>
                        {
                            if (eve.Error == null)
                            {
                                string restxt = Encoding.UTF8.GetString(eve.Result);
                                var resavator = JsonConvert.DeserializeObject<JsonBase>(restxt);
                                if (resavator.resultCode == 1)
                                {
                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        LoginPageIndex = 0;
                                        SnackBar.Enqueue("注册成功");
                                        UserId = PhoneNumber;
                                        InitialRegisterProperties();//重置注册页面
                                        EnabledRegister = true;
                                    });
                                }
                                else
                                {
                                    SnackBar.Enqueue(resuser.resultMsg, true);//提示错误信息
                                    App.Current.Dispatcher.Invoke(() => { EnabledRegister = true; });//启用注册
                                }
                            }
                            else
                            {
                                SnackBar.Enqueue(eve.Error.Message);
                                App.Current.Dispatcher.Invoke(() => { EnabledRegister = true; });//启用注册
                            }
                        };
                    });
                }
                else
                {
                    SnackBar.Enqueue("注册失败:" + e.Error.Message);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        EnabledRegister = true;
                    });
                    ConsoleLog.Output("注册失败:" + e.Error.Message);
                }
            });
        }

        #endregion

        #region Private Helpers

        #region 获取地址
        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="areasType"></param>
        /// <returns>获取到的地址</returns>
        private List<Areas> GetLocationList(AreasType areasType)
        {
            var type = Convert.ToInt16(areasType);
            var relist = new List<Areas>();
            switch (areasType)
            {
                case AreasType.Country:
                    relist = new Areas() { parent_id = 0, type = type }.GetChildrenList();
                    break;
                case AreasType.Province:
                    relist = new Areas() { parent_id = SelectedCountry, type = type }.GetChildrenList();
                    break;
                case AreasType.City:
                    relist = new Areas() { parent_id = SelectedProvince, type = type }.GetChildrenList();
                    break;
                case AreasType.Area:
                    relist = new Areas() { parent_id = SelectedCity, type = type }.GetChildrenList();
                    break;
                default:
                    break;
            }
            return relist;
        }
        #endregion

        #endregion
    }
}
