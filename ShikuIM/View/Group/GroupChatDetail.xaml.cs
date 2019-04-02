using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM
{
    /// <summary>
    /// RoomDetailCube.xaml 的交互逻辑
    /// </summary>
    public partial class GroupChatDetial : Window
    {

        /// <summary>
        /// 关闭窗口通知Token
        /// </summary>
        public static string CloseGroupDetialWindow { get; } = nameof(CloseGroupDetialWindow);

        /// <summary>
        /// 单例窗口
        /// </summary>
        private static GroupChatDetial GrouDetialWindow;

        /// <summary>
        /// 获取单例窗口
        /// </summary>
        /// <returns></returns>
        public static GroupChatDetial GetWindow()
        {
            if (GrouDetialWindow == null)
            {
                //GrouDetialWindow.Close();
                GrouDetialWindow = new GroupChatDetial();
            }
            return GrouDetialWindow;
        }

        #region Constructor
        private GroupChatDetial()
        {
            InitializeComponent();
            //关闭窗口注册
            Messenger.Default.Register<bool>(this, GroupChatDetial.CloseGroupDetialWindow, (para) => { App.Current.Dispatcher.Invoke(() => { this.Close(); }); });
        }
        #endregion


        #region 窗口关闭时
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GrouDetialWindow = null;
            ClearMemory();
        }
        #endregion


        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary> 
        /// 释放内存
        /// </summary> 
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion


        #region 加载事件
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lb_BannedTime.ItemsSource = new Dictionary<long, string>()
            {
                { 0,"不禁言"},
                { 1800,"禁言30分钟"},
                { 3600,"禁言1小时"},
                { 86400,"禁言1天"},
                { 259200,"禁言3天"},
                { 604800,"禁言1周"},
                { 1296000,"禁言半个月"},
                { 315360000,"永久禁言"},//十年‘
            };
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

    }
}
