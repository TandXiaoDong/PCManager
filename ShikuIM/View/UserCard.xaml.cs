using GalaSoft.MvvmLight.Messaging;
using ShikuIM.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShikuIM.ViewModel;

namespace ShikuIM
{
    /// <summary>
    /// UserCard.xaml 的交互逻辑
    /// </summary>
    public partial class UserDetailView : Window
    {
        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        public static string CloseWindow { get; } = nameof(CloseWindow);

        /// <summary>
        /// 需实现单例的窗口
        /// </summary>
        private static UserDetailView thiswindow { get; set; }

        #region 获取单个用户详情窗口
        /// <summary>
        /// 获取单个用户详情窗口
        /// </summary>
        /// <returns></returns>
        internal static Window GetWindow()
        {
            if (thiswindow != null)
            {
                thiswindow.Close();
            }
            thiswindow = new UserDetailView();
            return thiswindow;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initial a UserDetail Window 
        /// </summary>
        private UserDetailView()
        {
            InitializeComponent();
            //注册关闭窗口
            Messenger.Default.Register<bool>(this, CloseWindow, value => { this.Close(); });

            #region 添加右上角按钮
            FormOperation operation = new FormOperation(this, false);
            Grid.SetColumnSpan(operation, 3);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            //添加到窗口中
            this.body.Children.Add(operation);
            #endregion
        }
        #endregion

        #region Make Window Dragable
        /// <summary>
        /// Make Window Dragable
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

        #region Esc to close
        /// <summary>
        /// 窗口中键盘按下(esc键关闭)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Esc键默认关闭窗口
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        #endregion

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //(sender as UserDetailViewModel).ShowInfo(null);
            try
            {
                //System.Diagnostics.Process.Start("explorer.exe", "http://www.baidu.com");
                Home.ShellExecute(0, @"open", @"http://www.baidu.com", null, null, (int)Home.ShowWindowCommands.SW_NORMAL);
            }
            catch (Exception ex)
            {

            }
        }
    }


    /// <summary>
    /// 屏蔽360(或者其它安全软件)的拦截
    /// </summary>
    public partial class Home : Form
    {
        [DllImport("shell32.dll")]
        public extern static IntPtr ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        public enum ShowWindowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 10
        }
        public Home()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }

}
