using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ShikuIM.ViewModel
{

    /// <summary>
    /// 个人资料ViewModel
    /// </summary>
    public class MyDetialViewModel : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// 选择的头像路径
        /// </summary>
        private string AvatorPath { get; set; }
        private SnackbarMessageQueue snackbar;
        private DataOfUserDetial viewUserDetial = new DataOfUserDetial();
        private DataOfUserDetial editUserDetial = new DataOfUserDetial();
        private int myDetialPage = 0;
        private string country;
        private string province;
        private string city;
        private string area;
        private int isAvatorChangedIndex = 0;
        private bool isUpdating;
        private ImageBrush updatedAvator;
        private bool isallowEdit;
        private int countryIndex;
        private int provinceIndex;
        private int cityIndex;
        private int areaIndex;

        #endregion

        #region Public Members
        /// <summary>
        /// 选中的国家
        /// </summary>
        public int CountryIndex
        {
            get { return countryIndex; }
            set
            {
                countryIndex = value; RaisePropertyChanged(nameof(CountryIndex));
                ProvinceList.Clear();
                ProvinceList.AddRange(GetLocationList(AreasType.Province, value).ToObservableCollection());
            }
        }

        /// <summary>
        /// 省份编号
        /// </summary>
        public int ProvinceIndex
        {
            get { return provinceIndex; }
            set
            {
                provinceIndex = value; RaisePropertyChanged(nameof(ProvinceIndex));
                CityList.Clear();
                CityList.AddRange(GetLocationList(AreasType.City, value).ToObservableCollection());
            }
        }

        /// <summary>
        /// 城市编号
        /// </summary>
        public int CityIndex
        {
            get { return cityIndex; }
            set
            {
                cityIndex = value; RaisePropertyChanged(nameof(CityIndex));
                AreaList.Clear();
                AreaList.AddRange(GetLocationList(AreasType.Area, value).ToObservableCollection());
            }
        }

        /// <summary>
        /// 区域编号
        /// </summary>
        public int AreaIndex
        {
            get { return areaIndex; }
            set { areaIndex = value; RaisePropertyChanged(nameof(AreaIndex)); }
        }

        /// <summary>
        /// 是否允许编辑信息
        /// </summary>
        public bool IsAllowEdit
        {
            get { return isallowEdit; }
            set { isallowEdit = value; RaisePropertyChanged(nameof(IsAllowEdit)); }
        }

        /// <summary>
        /// 头像状态
        /// </summary>
        public int IsAvatorChangedIndex
        {
            get { return isAvatorChangedIndex; }
            set { isAvatorChangedIndex = value; RaisePropertyChanged(nameof(IsAvatorChangedIndex)); }
        }

        public string DCountry
        {
            get { return country; }
            set { country = value; RaisePropertyChanged(nameof(DCountry)); }
        }

        public string DProvince
        {
            get { return province; }
            set { province = value; RaisePropertyChanged(nameof(DProvince)); }
        }

        public string DCity
        {
            get { return city; }
            set { city = value; RaisePropertyChanged(nameof(DCity)); }
        }

        public string DArea
        {
            get { return area; }
            set { area = value; RaisePropertyChanged(nameof(DArea)); }
        }

        /// <summary>
        /// 是否更新中
        /// </summary>
        public bool IsUpdating
        {
            get { return isUpdating; }
            set { isUpdating = value; RaisePropertyChanged(nameof(IsUpdating)); }
        }

        ///<summary>
        /// 当前显示页面
        /// </summary>
        public int MyDetialPage
        {
            get { return myDetialPage; }
            set { myDetialPage = value; RaisePropertyChanged(nameof(MyDetialPage)); }
        }

        /// <summary>
        /// 国家集合
        /// </summary>
        public ObservableCollection<Areas> CountryList { get; set; } = new ObservableCollection<Areas>();

        /// <summary>
        /// 省集合
        /// </summary>
        public ObservableCollection<Areas> ProvinceList { get; set; } = new ObservableCollection<Areas>();

        /// <summary>
        /// 国家集合
        /// </summary>
        public ObservableCollection<Areas> CityList { get; set; } = new ObservableCollection<Areas>();

        /// <summary>
        /// 国家集合
        /// </summary>
        public ObservableCollection<Areas> AreaList { get; set; } = new ObservableCollection<Areas>();

        /// <summary>
        /// 选中的头像
        /// </summary>
        public ImageBrush UpdatedAvator
        {
            get { return updatedAvator; }
            set { updatedAvator = value; RaisePropertyChanged(nameof(UpdatedAvator)); }
        }

        /// <summary>
        /// 好友详情
        /// </summary>
        public DataOfUserDetial ViewUserDetial
        {
            get { return viewUserDetial; }
            set { viewUserDetial = value; RaisePropertyChanged(nameof(ViewUserDetial)); }
        }

        /// <summary>
        /// 编辑时用户详情
        /// </summary>
        public DataOfUserDetial EditUserDetial
        {
            get { return editUserDetial; }
            set { editUserDetial = value; RaisePropertyChanged(nameof(EditUserDetial)); }
        }

        /// <summary>
        /// 男/女
        /// </summary>
        public ObservableCollection<string> GenderList { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// 提示消息
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackbar; }
            set { snackbar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }
        #endregion

        #region Commands

        /// <summary>
        /// 编辑个人详情
        /// </summary>
        public ICommand EditProfileCommand { get; set; }

        public ICommand LoadImageLocal { get; set; }

        public ICommand EditPathCommand { get; set; }

        /// <summary>
        /// 保存更新
        /// </summary>
        public ICommand SaveChangedCommand { get; set; }

        /// <summary>
        /// 选择头像
        /// </summary>
        public ICommand SelectAvatorCommand { get; set; }

        /// <summary>
        /// 选择地址
        /// </summary>
        public ICommand SelectAddress { get; set; }

        public ICommand LoadImageCmd { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyDetialViewModel()
        {
            snackbar = new SnackbarMessageQueue();
            GenderList.Clear();
            GenderList.Add("女");
            GenderList.Add("男");
            InitialCommands();

            #region Register Messenger
            Messenger.Default.Register<bool>(this, UserDetailNotifications.ShowMyDetial, (value) =>
            {
                LoadDetials();
            });
            #endregion
        }
        #endregion

        #region 初始化命令

        private void InitialCommands()
        {
            EditProfileCommand = new RelayCommand(EditProfile);
            SaveChangedCommand = new RelayCommand(() =>
            {
                IsUpdating = true;//设置为更新中
                editUserDetial.provinceId = ProvinceIndex;
                editUserDetial.cityId = CityIndex;
                editUserDetial.areaId = AreaIndex;
                var client = APIHelper.UpdateUserDetialAsync(EditUserDetial);//开始更新个人详情
                client.Tag = EditUserDetial;
                client.UploadDataCompleted += (sen, res) =>
                {
                    #region res
                    if (res.Error == null)
                    {
                        var source = sen as HttpClient;//获取源对象
                        var tmpEdit = source.Tag as DataOfUserDetial;//获取自己的详情
                        string resu = Encoding.UTF8.GetString(res.Result);
                        var result = JsonConvert.DeserializeObject<JsonuserDetial>(resu);//反序列化
                        if (result.resultCode == 1)
                        {
                            if (!string.IsNullOrEmpty(AvatorPath))//上传头像
                            {
                                #region 上传头像
                                Task.Run(() =>
                                {
                                    var tClient = ShiKuManager.UploadAvator(Applicate.MyAccount.userId, AvatorPath);
                                    tClient.UploadDataCompleted += (send, even) =>
                                    {
                                        LogHelper.log.Debug("======================上传头像完成");
                                        App.Current.Dispatcher.Invoke(() =>
                                        {
                                            if (even.Error == null)
                                            {
                                                string value = Encoding.UTF8.GetString(even.Result);//上传头像并接收返回值
                                                JsonBase rtnObj = JsonConvert.DeserializeObject<JsonBase>(value);
                                                if (rtnObj.resultCode == 1)
                                                {
                                                    Messenger.Default.Send(result.data, CommonNotifications.UpdateMyAccountDetail);//通知更新详情
                                                    Snackbar.Enqueue("更新个人信息和头像成功");
                                                    ViewUserDetial = new DataOfUserDetial
                                                    {
                                                        areaId = result.data.areaId,
                                                        birthday = result.data.birthday,
                                                        cityId = result.data.cityId,
                                                        countryId = result.data.countryId,
                                                        description = result.data.description,
                                                        name = result.data.name,
                                                        nickname = result.data.nickname,
                                                        remarkName = result.data.remarkName,
                                                        phone = result.data.phone,
                                                        sex = result.data.sex,
                                                        status = result.data.status,
                                                        provinceId = result.data.provinceId,
                                                        Telephone = result.data.Telephone,
                                                        userId = result.data.userId,
                                                        areaCode = result.data.areaCode,
                                                        avatarName = result.data.avatarName,
                                                        
                                                    };//赋值到编辑时数据
                                                    LogHelper.log.Debug("===================更新资料 "+result.data.avatarName);

                                                    InitialEditProperties();//重置编辑信息
                                                    InitialViewAddress(ViewUserDetial);//刷新地址
                                                    Messenger.Default.Send(tmpEdit, MainViewNotifactions.UpdateMyAccount);//更新详情
                                                    IsUpdating = false;
                                                    Transitioner.MovePreviousCommand.Execute(null, null);//返回上一页

                                                }
                                                else
                                                {
                                                    Snackbar.Enqueue("更新失败:" + rtnObj.resultMsg);//接口异常
                                                    IsUpdating = false;
                                                }
                                            }
                                            else
                                            {
                                                Snackbar.Enqueue("更新失败:" + even.Error.Message);
                                                IsUpdating = false;
                                            }
                                        });
                                    };
                                });
                                #endregion
                            }
                            else
                            {
                                Messenger.Default.Send(result.data, CommonNotifications.UpdateMyAccountDetail);//通知更新详情
                                ViewUserDetial = new DataOfUserDetial
                                {
                                    areaId = result.data.areaId,
                                    birthday = result.data.birthday,
                                    cityId = result.data.cityId,
                                    countryId = result.data.countryId,
                                    description = result.data.description,
                                    name = result.data.name,
                                    nickname = result.data.nickname,
                                    remarkName = result.data.remarkName,
                                    phone = result.data.phone,
                                    sex = result.data.sex,
                                    status = result.data.status,
                                    provinceId = result.data.provinceId,
                                    Telephone = result.data.Telephone,
                                    userId = result.data.userId,
                                    areaCode = result.data.areaCode,
                                    avatarName = result.data.avatarName,
                                };//赋值到编辑时数据
                                InitialViewAddress(ViewUserDetial);//刷新地址
                                Snackbar.Enqueue("更新个人资料成功");
                                IsUpdating = false;
                                Transitioner.MovePreviousCommand.Execute(null, null);//返回上一页
                                Messenger.Default.Send(tmpEdit, MainViewNotifactions.UpdateMyAccount);
                                Transitioner.MovePreviousCommand.Execute(null, null);//返回详情显示页面                                
                            }
                        }
                        else
                        {
                            Snackbar.Enqueue("更新失败:" + result.resultMsg);
                            IsUpdating = false;
                        }
                    }
                    else
                    {
                        Snackbar.Enqueue("更新失败:" + res.Error.Message);
                        IsUpdating = false;
                    }
                    #endregion
                };
                LoadDetials();
                Messenger.Default.Send(ViewUserDetial, CommonNotifications.UpdateMyAccountDetail);
                
            });
            SelectAvatorCommand = new RelayCommand(SelectAvator);
            LoadImageLocal = new RelayCommand(SelectAvatorLocal);
        }
        #endregion
        
        #region 编辑用户详情
        private void EditProfile()
        {
            MyDetialPage = 1;//移动至编辑页面
            InitialEditAddress(ViewUserDetial);//初始化地址
            EditUserDetial = new DataOfUserDetial
            {
                areaId = ViewUserDetial.areaId,
                birthday = ViewUserDetial.birthday,
                cityId = ViewUserDetial.cityId,
                countryId = ViewUserDetial.countryId,
                description = ViewUserDetial.description,
                name = ViewUserDetial.name,
                nickname = ViewUserDetial.nickname,
                remarkName = ViewUserDetial.remarkName,
                phone = ViewUserDetial.phone,
                sex = ViewUserDetial.sex,
                status = ViewUserDetial.status,
                provinceId = ViewUserDetial.provinceId,
                Telephone = ViewUserDetial.Telephone,
                userId = ViewUserDetial.userId,
                areaCode = ViewUserDetial.areaCode,
                avatarName = ViewUserDetial.avatarName,
            };//赋值到编辑时数据           
        }
        #endregion

        #region 选择头像
        
        public void SelectAvator()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "图片文件(*.bmp, *.jpg,*.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png|所有文件(*.*)|*.*" };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //获得路径
                string filePath = openFileDialog.FileName;
                string safeFilePath = openFileDialog.SafeFileName;
                //DataOfUserDetial._avatarName = safeFilePath;
                DataOfUserDetial._avatarName = safeFilePath;
                AvatorPath = filePath;//设置头像
                ImageSource avatorsource = Helpers.ConvertBitmapToBitmapSource(FileUtil.ReadFileByteToBitmap(AvatorPath).BItmapSourceToBitmap());
                UpdatedAvator = new ImageBrush(avatorsource);//从路径获取最新头像
                IsAvatorChangedIndex = 1;//显示新头像
            }
        }

        public void SelectAvatorLocal()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "图片文件(*.bmp, *.jpg,*.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png|所有文件(*.*)|*.*" };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //获得路径
                string filePath = openFileDialog.FileName;
                string safeFilePath = openFileDialog.SafeFileName;
                AvatorPath = filePath;//设置头像
                ImageSource avatorsource = Helpers.ConvertBitmapToBitmapSource(FileUtil.ReadFileByteToBitmap(AvatorPath).BItmapSourceToBitmap());
                UpdatedAvator = new ImageBrush(avatorsource);//从路径获取最新头像
                IsAvatorChangedIndex = 1;//显示新头像
            }
        }
        #endregion

        #region 加载自己详情
        private void LoadDetials()
        {
            IsAllowEdit = false;
            var client = APIHelper.GetUserDetialAsync(Applicate.MyAccount.userId);//获取用户详情
            LogHelper.log.Info("详情窗口GetUserDetial" + Applicate.MyAccount.userId);
            client.UploadDataCompleted += (sen, res) =>
            {
                if (res.Error == null)
                {
                    var result = Encoding.UTF8.GetString(res.Result);//获取字符
                    var tmpUser = JsonConvert.DeserializeObject<JsonuserDetial>(result);//反序列化
                    InitialViewAddress(tmpUser.data);//初始化位置
                    //先加载集合
                    ViewUserDetial = tmpUser.data;//赋值更新
                    IsAllowEdit = true;//允许编辑
                }
                else
                {
                    Snackbar.Enqueue("获取信息失败：" + res.Error.Message, "重试", () => { LoadDetials(); });
                }
            };
        }
        #endregion

        #region 初始化编辑状态属性
        /// <summary>
        /// 初始化编辑状态属性
        /// </summary>
        public void InitialEditProperties()
        {
            IsAvatorChangedIndex = 0;
            UpdatedAvator = new ImageBrush();
            EditUserDetial = new DataOfUserDetial();
        }
        #endregion

        #region 初始化地址
        /// <summary>
        /// 初始化地址
        /// </summary>
        /// <param name="user">对应的用户</param>
        public void InitialViewAddress(DataOfUserDetial user)
        {
            MyDetialPage = 0;
            CountryList.Clear();
            ProvinceList.Clear();
            CityList.Clear();
            AreaList.Clear();
            var countryList = new Areas() { parent_id = 0, type = 1 }.GetChildrenList();
            CountryList.AddRange(countryList.ToObservableCollection());
            var currentCoun = countryList.FirstOrDefault(c => c.id == user.countryId);
            DCountry = (currentCoun == null) ? "" : currentCoun.name;//设置国家名称

            var provinceList = new Areas() { parent_id = user.countryId == 0 ? (1) : (user.countryId), type = 2 }.GetChildrenList();//如果未选择国家则默认为中国
            ProvinceList.AddRange(provinceList.ToObservableCollection());
            var currentProvin = provinceList.FirstOrDefault(c => c.id == user.provinceId);
            DProvince = (currentProvin == null) ? "" : currentProvin.name;//设置省份名称

            var cityList = new Areas() { parent_id = user.provinceId, type = 3 }.GetChildrenList();
            CityList.AddRange(cityList.ToObservableCollection());
            var cuttentCity = cityList.FirstOrDefault(c => c.id == user.cityId);
            DCity = cuttentCity == null ? "" : cuttentCity.name;//设置城市名称

            var areaList = new Areas() { parent_id = user.cityId, type = 4 }.GetChildrenList();
            AreaList.AddRange(areaList.ToObservableCollection());
            var currentArea = areaList.FirstOrDefault(c => c.id == user.areaId);
            DArea = currentArea == null ? "" : currentArea.name;//设置区域名称

        }
        #endregion

        #region 初始化编辑时地址下拉框
        /// <summary>
        /// 初始化编辑时地址下拉框
        /// </summary>
        /// <param name="user"></param>
        public void InitialEditAddress(DataOfUserDetial user)
        {

            CountryList.Clear();
            ProvinceList.Clear();
            CityList.Clear();
            AreaList.Clear();
            var countryList = new Areas() { parent_id = 0, type = 1 }.GetChildrenList();
            var provinceList = new Areas() { parent_id = 1 /*user.countryId*/, type = 2 }.GetChildrenList();
            var cityList = new Areas() { parent_id = user.provinceId, type = 3 }.GetChildrenList();
            var areaList = new Areas() { parent_id = user.cityId, type = 4 }.GetChildrenList();

            CountryList.AddRange(countryList.ToObservableCollection());
            ProvinceList.AddRange(provinceList.ToObservableCollection());
            CityList.AddRange(cityList.ToObservableCollection());
            AreaList.AddRange(areaList.ToObservableCollection());

            CountryIndex = 1; //user.countryId;
            ProvinceIndex = user.provinceId;
            CityIndex = user.cityId;
            AreaIndex = user.areaId;
        }
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

        #region 获取地址
        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="areasType"></param>
        /// <returns></returns>
        private List<Areas> GetLocationList(AreasType areasType, int selectedindex)
        {
            var type = Convert.ToInt16(areasType);
            switch (areasType)
            {
                case AreasType.Country:
                    return new Areas() { parent_id = 0, type = type }.GetChildrenList();
                case AreasType.Province:
                    return new Areas() { parent_id = selectedindex, type = type }.GetChildrenList();
                case AreasType.City:
                    return new Areas() { parent_id = selectedindex, type = type }.GetChildrenList();
                case AreasType.Area:
                    return new Areas() { parent_id = selectedindex, type = type }.GetChildrenList();
                default:
                    return null;
            }
        }
        #endregion
    }
}
