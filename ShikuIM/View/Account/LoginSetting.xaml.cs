using CommonServiceLocator;
using MaterialDesignThemes.Wpf;
using ShikuIM.Model;
using ShikuIM.UserControls;
using ShikuIM.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM
{
    /// <summary>
    /// SetUp.xaml 的交互逻辑
    /// </summary>
    public partial class LoginSettingsWindow : Window, INotifyPropertyChanged
    {
        private SnackbarMessageQueue _snackBar;

        #region UI更新接口

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private static LoginSettingsWindow thisSetUp { get; set; }

        /// <summary>
        /// 提示控件
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return _snackBar; }
            set { _snackBar = value; }
        }
        #region Contructor

        public LoginSettingsWindow()
        {
            InitializeComponent();
            Snackbar = new SnackbarMessageQueue();
            this.tb_InitialServer.Text = ConfigurationUtil.GetValue("InitialServer");
            this.tb_InitialServerOfKey.Text = ConfigurationUtil.GetValue("InitialServer_key");

            #region 添加右上角按钮
            FormOperation operation = new FormOperation(this, false);
            this.body.Children.Add(operation);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            #endregion
        }
        #endregion

        #region 窗体加载时
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // this.tb_InitialServer.Text = ConfigurationUtil.GetValue("InitialServer");
        }
        #endregion

        #region 单例方法
        public static LoginSettingsWindow getSetUp()
        {
            if (thisSetUp != null)
            {
                thisSetUp.Close();
            }
            thisSetUp = new LoginSettingsWindow();
            return thisSetUp;
        }
        #endregion

        #region 窗体拖动
        /// <summary>
        /// 窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion

        #region 确定
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            //if (this.tb_InitialServer.Text.Trim() != "" && this.tb_InitialServerOfKey.Text.Trim() != "")
            if (this.tb_InitialServer.Text.Trim() != "")
            {
                ConfigurationUtil.SetValue("InitialServer", this.tb_InitialServer.Text);
                ConfigurationUtil.SetValue("InitialServer_key", this.tb_InitialServerOfKey.Text);
            }
            else
            {
                Snackbar.Enqueue("请输入地址！");
                return;
            }

            ShiKuManager.GetConfigAsync();

            this.Close();
        }
        #endregion

    }
}
