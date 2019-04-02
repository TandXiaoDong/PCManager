using ShikuIM.UserControls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShikuIM
{

    /// <summary>
    /// windowBase.xaml 的交互逻辑
    /// </summary>
    public partial class AccountQuery : Window
    {

        #region Public Members
        public static AccountQuery Window { get; set; }
        #endregion

        #region 单例方法
        public static AccountQuery GetWindow()
        {
            if (Window == null)
            {
                Window = new AccountQuery();
            }
            return Window;
        }
        #endregion

        #region 关闭窗口
        /// <summary>
        /// 关闭窗口
        /// </summary>
        internal static void CloseWindow()
        {
            Window = null;
        }
        #endregion

        public AccountQuery()
        {
            InitializeComponent();
        }

        #region 窗体加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 添加最大化、最小化、关闭按钮
            //设置标题栏的按钮
            FormOperation operation = new FormOperation(this, false);
            //设置对齐  向左  向上
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            //添加到窗口中
            Grid.SetColumn(operation, 2);
            Grid.SetRow(operation, 0);
            this.gd_Content.Children.Add(operation);
            #endregion
        }
        #endregion

        #region 鼠标拖动
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }//窗口随意拖动
        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CloseWindow();
        }
    }
}
