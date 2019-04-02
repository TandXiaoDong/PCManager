using System;
using System.Collections.Generic;
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
using ShikuIM.ViewModel;

namespace ShikuIM.View
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class GroupSetting : Window
    {
        string _roomId;
        public delegate void delUc(string userId);
        public event delUc delDetailForm;//删除窗口列表
        public GroupSetting(string roomId)
        {
            InitializeComponent();
            _roomId = roomId;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GroupShareController.ShowShareForm(_roomId);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            delDetailForm?.Invoke(_roomId);
        }
    }
}
