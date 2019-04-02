//using System.Windows.Forms;

using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;

namespace ShikuIM
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 同步基元(进程间通信)
        /// </summary>
        //System.Threading.Mutex mutex;

        #region 应用程序启动时
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });//设置为30帧动画
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;//设置为但
            ShiKuManager.GetConfigAsync();//获取配置
            //mutex = new System.Threading.Mutex(true, "ElectronicNeedleTherapySystem", out ret);
            /*if (!ret)
            {
                MessageBox.Show("你已经打开了酷聊，请查看托盘或结束进程重启");
                Environment.Exit(0);
                return;
            }*/
            //Application.Current.MainWindow = new TestingWindow();
            Current.MainWindow = new Login();
            Current.MainWindow.Show();
            //检查目录
            CheckOrCreateChatDir();

        }
        #endregion

        #region 检查并创建聊天文件夹
        /// <summary>
        /// 检查并创建聊天文件夹
        /// </summary>
        private void CheckOrCreateChatDir()
        {
            if (!Directory.Exists(Applicate.LocalConfigData.CatchPath))//聊天文件目录
            {
                Directory.CreateDirectory(Applicate.LocalConfigData.CatchPath);
            }
            if (!Directory.Exists(Applicate.LocalConfigData.ChatDownloadPath))//聊天文件目录
            {
                Directory.CreateDirectory(Applicate.LocalConfigData.ChatDownloadPath);
            }
            if (!Directory.Exists(Applicate.LocalConfigData.ChatPath))//聊天目录(语音消息)
            {
                Directory.CreateDirectory(Applicate.LocalConfigData.ChatPath);
            }
            if (!Directory.Exists(Applicate.LocalConfigData.TempFilepath))//临时聊天文件存放目录
            {
                Directory.CreateDirectory(Applicate.LocalConfigData.TempFilepath);
            }
            if (!Directory.Exists(Applicate.LocalConfigData.UserAvatorFolderPath))//头像目录
            {
                Directory.CreateDirectory(Applicate.LocalConfigData.UserAvatorFolderPath);
            }

        }
        #endregion

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        #region 抛出异常
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //System.Windows.MessageBox("Error encountered! Please contact support." + Environment.NewLine + e.Exception.Message);
            LogHelper.log.Error("程序全局捕捉错误：" + Environment.NewLine + e.Exception.Message, e.Exception);
            ConsoleLog.Output("程序全局捕捉错误：" + Environment.NewLine + e.Exception.Message, e.Exception);
            //Shutdown(1);
            e.Handled = true;
        }
        #endregion
    }
}
