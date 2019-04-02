
using GalaSoft.MvvmLight.Messaging;
using ShikuIM.UserControls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM
{
    /// <summary>
    /// GroupCreate.xaml 的交互逻辑
    /// </summary>
    public partial class GroupCreate : Window
    {

        private static GroupCreate thisGroupCreate { get; set; }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public static string CloseWindow { get; } = nameof(CloseWindow);

        #region 构造函数
        public GroupCreate()
        {
            InitializeComponent();
            InitialUI();//初始化窗体UI
            RegisterMessenger();
            //设置数据上下文
        }
        #endregion

        #region 关闭建群窗口
        private void RegisterMessenger()
        {
            Messenger.Default.Register<bool>(this, CloseWindow, para => App.Current.Dispatcher.Invoke(() => { this.Close(); }));//注册关闭窗口消息
        }
        #endregion

        #region 单例方法
        /// <summary>
        /// 单例方法
        /// </summary>
        /// <returns></returns>
        public static GroupCreate GetGroupCreate()
        {
            if (thisGroupCreate == null)
            {
                thisGroupCreate = new GroupCreate();
            }
            return thisGroupCreate;
        }
        #endregion

        #region 初始化窗体UI
        /// <summary>
        /// 初始化窗体UI
        /// </summary>
        private void InitialUI()
        {
            #region 初始化三个按钮
            FormOperation operation = new FormOperation(this, false);
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            operation.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumn(operation, 1);//
            //gd_Children.Add(operation);
            #endregion
        }

        #endregion

        #region 关闭时赋值为Null
        private void window_Closed(object sender, EventArgs e)
        {
            if (thisGroupCreate != null)
            {
                thisGroupCreate = null;
            }
        }
        #endregion

    }
}
