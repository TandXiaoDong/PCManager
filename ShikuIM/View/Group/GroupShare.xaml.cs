using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ShikuIM.UserControls;
using ShikuIM.ViewModel;

namespace ShikuIM.View
{
    /// <summary>
    /// GroupShare.xaml 的交互逻辑
    /// </summary>
    public partial class GroupShare : Window
    {
        public delegate void delUc(string roomId);
        public event delUc delShareForm;//删除窗口列表
        string _roomId;
        public GroupShare(string roomId)
        {
            InitializeComponent();
            #region 添加右上角按钮
            this._roomId = roomId;
            FormOperation operation = new FormOperation(this, false);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            Panel.SetZIndex(operation, 4);
            //添加到窗口中
            this.gd_main.Children.Add(operation);
            #endregion
            DataContext = new GroupShareViewModel(roomId);
        }

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
        private void btn_OpenFilePath_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            delShareForm?.Invoke(_roomId);
        }
    }
}
