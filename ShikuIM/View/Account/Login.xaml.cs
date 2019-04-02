using GalaSoft.MvvmLight.Messaging;
using ShikuIM.Security;
using ShikuIM.ViewModel.Base;
using System.Configuration;
using System.Diagnostics;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// 视酷信息主命名空间
/// </summary>
namespace ShikuIM
{
    /// <summary>
    /// 登录窗口
    /// </summary>
    public partial class Login : Window, IHavePassword
    {
        /// <summary>
        /// 头像路径
        /// </summary>
        string filePath { get; set; }

        #region Public Member
        /// <summary>
        /// 登录密码
        /// </summary>
        public SecureString LoginSecurePassword
        {
            get { return PasswordBox.SecurePassword; }
            set
            {
                PasswordBox.Password = value.UnSecure();
            }
        }

        /// <summary>
        /// 第一次密码
        /// </summary>
        public SecureString FirstRegisterSecurePassword
        {
            get { return RegisterPasswordBox.SecurePassword; }
            set { RegisterPasswordBox.Password = value.ToString(); }
        }

        /// <summary>
        /// 第二次验证密码
        /// </summary>
        public SecureString FinalRegisterSecurePassword
        {
            get { return FirstRegisterPassword.SecurePassword; }
            set { FirstRegisterPassword.Password = value.ToString(); }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public static string CloseWindow { get; } = nameof(CloseWindow);
        #endregion


        #region Constructor
        public Login()
        {
            InitializeComponent();
        }
        #endregion


        #region 窗口加载时
        private void loginWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (Debugger.IsAttached)
            {
                PasswordBox.Password = "123456";
            }
            //注册关闭窗口
            Messenger.Default.Register<bool>(this, para => this.Close());

            //通知加载记住密码的用户
            Messenger.Default.Send(true, LoginNotifications.InitialAccount);
        }
        #endregion

        #region Set window dragable
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //设置窗口内按下鼠标后窗口可以被拖动
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion

        #region Copy Log Path
        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
        #endregion


        #region 限制注册账号格式
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9a-zA-Z]+");
            e.Handled = re.IsMatch(e.Text);
        }
        #endregion

        #region 限制只能输入数字
        /// <summary>
        /// 限制只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUsername_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;//判断shift键是否按下
            if (shiftKey == true)                  //当按下shift
            {
                e.Handled = true;//不可输入
            }
            else//未按shift
            {
                if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Enter))
                {
                    e.Handled = true;//不可输入
                }
            }
        }
        #endregion

        #region 切换服务器
        /// <summary>
        /// 切换服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerSwitchCommand(object sender, RoutedEventArgs e)
        {
            Cards.SelectedIndex = 2;
        }
        #endregion

        #region 保存上次输入的服务器地址
        private void btn_server_Click(object sender, RoutedEventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            cfa.Save();
        }
        #endregion

        #region 登录窗口关闭时
        /// <summary>
        /// 登录窗口关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Applicate.IsAccountVerified)//如果未通过登录验证
            {
                Application.Current.Shutdown();
            }
        }
        #endregion
        
    }
}
