using ShikuIM.Model;
using ShikuIM.UserControls;
using ShikuIM.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShikuIM.View
{
    /// <summary>
    /// RoomVerify.xaml 的交互逻辑
    /// </summary>
    public partial class RoomVerifyForm : Window
    {
        public RoomVerifyForm(Messageobject msg)
        {
            InitializeComponent();
            var rverifymodel = new RoomVerifyViewModel();
            rverifymodel.Initial(msg);
            this.DataContext = rverifymodel;
            App.Current.Dispatcher.Invoke(() =>
            {
                //UserId = userId;
                #region 添加右上角按钮
                FormOperation operation = new FormOperation(this, false);
                Grid.SetColumnSpan(operation, 3);//
                operation.VerticalAlignment = VerticalAlignment.Top;
                operation.HorizontalAlignment = HorizontalAlignment.Right;
                //添加到窗口中
                this.gd_main.Children.Add(operation);
                #endregion
            });
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Esc键默认关闭窗口
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
