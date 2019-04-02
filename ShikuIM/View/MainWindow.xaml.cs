using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using ShikuIM.Model;
using ShikuIM.Resource;
using ShikuIM.ViewModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace ShikuIM
{

    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RisCaptureLib.ScreenCaputre screenCaputre = new RisCaptureLib.ScreenCaputre();

        #region Private & Internal Member
        /// <summary>
        /// 酷聊托盘图标
        /// </summary>
        internal NotifyIcon notifyIcon = null;
        #endregion

        #region Public Member
        public System.Timers.Timer msgNotice = new System.Timers.Timer();

        /// <summary>
        /// 标识为激活窗口的通知Token
        /// </summary>
        public static string ActiveWindow { get; } = nameof(ActiveWindow);

        /// <summary>
        /// 显示窗口
        /// </summary>
        public static object ShowWindow { get; } = nameof(ShowWindow);

        #endregion

        #region Constructor
        /// <summary>
        /// 窗体构造函数  
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region 注册通知
        /// <summary>
        /// 注册通知
        /// </summary>
        private void InitialMessengers()
        {
            Messenger.Default.Register<bool>(this, ActiveWindow, value => { App.Current.Dispatcher.Invoke(() => { this.Activate(); }); });//激活当前窗口
            Messenger.Default.Register<bool>(this, ShowWindow, value =>
            {
                App.Current.Dispatcher.Invoke(() => { this.Show(); });
            });//显示当前窗口
            Messenger.Default.Register<bool>(this, MainViewNotifactions.FlashWindow, value =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var helper = new WindowInteropHelper(this);
                    Win32.FlashWindow(helper.Handle, true);//闪烁窗口
                });
            });//显示当前窗口
        }
        #endregion

        #region 窗口加载时
        private void window_Loaded(object sender, RoutedEventArgs e)
        {

            InitialMessengers();//注册通知
            Task.Run(() =>
            {
                screenCaputre.ScreenCaputred += OnScreenCaputred;
                screenCaputre.ScreenCaputreCancelled += OnScreenCaputreCancelled;
                bool vlcCfg = VlcPlayer.VlcConfig();
                LogHelper.log.Info("Vlc播放器配置：" + vlcCfg);
            });
            ////FullScreenManager.RepairWpfWindowFullScreenBehavior(this);//适应全屏幕(窗口最小长宽限制会消失)
            //初始化UI
            UIInitial();
        }
        #endregion

        #region 程序退出
        private void OnMainWindowClose(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }
        #endregion

        #region 截屏取消后
        /// <summary>
        /// 截屏取消后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScreenCaputreCancelled(object sender, System.EventArgs e)
        {
            Show();
            Focus();
        }
        #endregion

        #region 截图完成
        /// <summary>
        /// 截图完成后
        /// </summary>
        /// <param name="sender">控件源</param>
        /// <param name="e"></param>
        private void OnScreenCaputred(object sender, RisCaptureLib.ScreenCaputredEventArgs e)
        {
            var bmp = e.Bmp;
            try
            {
                Bitmap tmpimg = Helpers.BItmapSourceToBitmap(bmp);
                string path = Applicate.LocalConfigData.ChatDownloadPath + DateTime.Now.ToFileTime() + ".png";
                ConsoleLog.Output("保存截图成功==" + path);
                LogHelper.log.Info("保存截图成功==" + path);
                tmpimg.Save(path, ImageFormat.Png);
                var mainview = this.DataContext as MainViewModel;//获取MainViewModel
                ShiKuManager.SendMessageFile(new MessageListItem
                {
                    Jid = mainview.Sess.Jid,
                    ShowTitle = mainview.Sess.NickName,
                    MessageItemContent = mainview.Sess.MyMemberNickname,
                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(mainview.Sess.Jid)
                }, path);
                //添加至富文本框
                //new InlineUIContainer(img, rtb_sendMessage.ThisRichText.Selection.Start);
            }
            catch (Exception ex)
            {
                ConsoleLog.Output("添加截图失败" + ex.Message);
                LogHelper.log.Error("添加截图失败", ex);
            }
        }
        #endregion

        #region 标题点击事件
        /// <summary>
        /// 标题点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fbtn_Click(object sender, RoutedEventArgs e)
        {

            e.Handled = true;
        }
        #endregion

        #region 主界面UI初始化
        /// <summary>
        /// 主界面UI初始化
        /// </summary>
        private void UIInitial()
        {
            //绑定托盘图标
            BindNotiyIcon();
            #region 设置新消息提醒(任务栏图标闪动)
            msgNotice.Interval = 600;//600毫秒闪动
            msgNotice.Elapsed += new ElapsedEventHandler((s, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (this.IsActive == false)
                    {
                        if (Convert.ToBoolean(notifyIcon.Tag))
                        {
                            notifyIcon.Icon = ShikuRec.ShikuIcoBig;
                            notifyIcon.Tag = false;
                        }
                        else
                        {
                            notifyIcon.Icon = ShikuRec.ico_Notice;
                            notifyIcon.Tag = true;
                        }
                        notifyIcon.Text = "您有新消息~";
                    }
                    else
                    {
                        msgNotice.Stop();
                        notifyIcon.Tag = true;
                        notifyIcon.Text = "微微";
                        notifyIcon.Icon = ShikuRec.ShikuIcoBig;
                    }
                });
            });
            #endregion
        }
        #endregion

        #region 窗体关闭时取消操作，只隐藏窗口
        /// <summary>
        /// 窗体关闭时取消操作，只隐藏窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
            return;
        }
        #endregion

        #region 窗口随意拖动
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }//窗口随意拖动
        #endregion

        #region 托盘绑定
        /// <summary>
        /// 托盘图标绑定
        /// </summary>
        private void BindNotiyIcon()
        {
            //设置托盘的各个属性
            notifyIcon = new NotifyIcon
            {
                Tag = true,//设置图标闪动标记
                BalloonTipText = "微微开始运行",
                Text = "微微\n" + Applicate.MyAccount.nickname + "(" + Applicate.MyAccount.userId + ")",
                Visible = true,//显示任务栏图标
                Icon = ShikuRec.ShikuIcoBig
            };
            notifyIcon.Visible = true;//设置为可见
            notifyIcon.ShowBalloonTip(2000);//设置鼠标悬浮两秒后显示气泡
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseClick);
            //设置菜单项
            ContextMenu cm = new ContextMenu();
            MenuItem menuExit = new MenuItem("退出");
            MenuItem menuLogout = new MenuItem("切换账号");
            menuExit.Click += new EventHandler((s, e) =>
            {
                ShiKuManager.ApplicationExit();
                System.Windows.Application.Current.Shutdown();//退出整个应用程序
            });
            menuLogout.Click += new EventHandler((s, e) =>
            {
                ShiKuManager.ApplicationExit();
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            });
            cm.MenuItems.Add(menuLogout);//注销
            cm.MenuItems.Add(menuExit);//注销菜单
            notifyIcon.ContextMenu = cm;//绑定右键菜单
        }
        #endregion

        #region 托盘图标鼠标点击事件
        /// <summary>
        /// 托盘图标鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Activate();
                    this.Focus();
                    //this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
        }
        #endregion

        #region 窗体状态改变时
        private void Window_StateChanged(object sender, EventArgs e)
        {
            //最大化不遮任务栏
            if (this.WindowState == WindowState.Maximized)
            {
                //bd_Outer.Margin = new Thickness(0, 0, 0, 30);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                //bd_Outer.Margin = new Thickness(15);
            }
        }
        #endregion

        #region 打开个人详情
        /// <summary>
        /// 打开个人详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountItemDetail_Click(object sender, RoutedEventArgs e)
        {
            //获取UserId并打开
            string UserId = ((System.Windows.Controls.Button)sender).Tag == null ? ("") : (((System.Windows.Controls.Button)sender).Tag.ToString());
            Messenger.Default.Send(UserId, UserDetailNotifications.ShowUserDetial);
            UserDetailView.GetWindow().Show();
        }
        #endregion

        #region 屏幕截图点击事件
        /// <summary>
        /// 屏幕截图点击
        /// </summary>
        /// <param name="sender">源控件</param>
        /// <param name="e"></param>
        private void screenShotClick(object sender, RoutedEventArgs e)
        {
            //Hide();
            //Thread.Sleep(300);
            this.IsEnabled = false;
            var mainview = ServiceLocator.Current.GetInstance<MainViewModel>();
            if (mainview.CheckIsBanned(mainview.Sess.Jid))//检查发起群聊消息是否被禁言
            {
                return;
            }
            screenCaputre.StartCaputre(5, new System.Windows.Size());
            this.IsEnabled = true;
        }
        #endregion
        
    }



    /// <summary>
    /// 窗口闪动的辅助类
    /// </summary>
    public class FlashWindowHelper
    {
        System.Timers.Timer timer;
        int _count = 0;
        int _maxTimes = 0;
        IntPtr _window;


        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwfi"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);


        public enum falshType : uint
        {
            FLASHW_STOP = 0,    //停止闪烁
            FALSHW_CAPTION = 1,  //只闪烁标题
            FLASHW_TRAY = 2,   //只闪烁任务栏
            FLASHW_ALL = 3,//标题和任务栏同时闪烁
            FLASHW_PARAM1 = 4,
            FLASHW_PARAM2 = 12,
            FLASHW_TIMER = FLASHW_TRAY | FLASHW_PARAM1,//无条件闪烁任务栏直到发送停止标志，停止后高亮
            FLASHW_TIMERNOFG = FLASHW_TRAY | FLASHW_PARAM2//未激活时闪烁任务栏直到发送停止标志或者窗体被激活，停止后高亮
        }

        /// <summary>
        /// 闪动任务栏图标
        /// </summary>
        /// <param name="times"></param>
        /// <param name="millliseconds"></param>
        /// <param name="window"></param>
        public void Flash(int times, double millliseconds, IntPtr window)
        {
            _maxTimes = times;
            _window = window;

            timer = new System.Timers.Timer();
            timer.Interval = millliseconds;
            timer.Elapsed += _timer_Elapsed;
            timer.Start();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (++_count < _maxTimes)
            {
                Win32.FlashWindow(_window, (_count % 2) == 0);
            }
            else
            {
                timer.Stop();
            }
        }

    }

    /// <summary>
    /// Win32 API
    /// </summary>
    internal static class Win32
    {
        /// <summary>
        /// 窗口闪动
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="bInvert">是否为闪</param>
        /// <returns>成功返回0</returns>
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
    }
}
