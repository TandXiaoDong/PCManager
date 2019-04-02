using ShikuIM.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShikuIM
{
    /// <summary>
    /// 个人资料修改
    /// </summary>
    public partial class Personal : Window
    {

        private static Personal ThisPersonal { get; set; }

        #region Contructor
        public Personal()
        {
            InitializeComponent();
        }
        #endregion

        #region 单例方法
        public static Personal GetPersonal()
        {
            //if (ThisPersonal == null || ThisPersonal.Visibility == Visibility.Hidden)
            //{
            ThisPersonal = new Personal();
            //}
            return ThisPersonal;
        }
        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 添加右上角按钮
            FormOperation operation = new FormOperation(this, false);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumnSpan(operation, 3);
            gd_main.Children.Add(operation);
            #endregion

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

        #region 头像上传点击事件
        /// <summary>
        /// 头像上传点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Upload_Click(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "图像文件(*.bmp, *.jpg,*.jpeg, *.png)|*.bmp;*.jpg;*.jpeg;*.png|所有文件(*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //获得路径
                string fileParth = openFileDialog.FileName;
                //head.Userid =;   //ShiKuManager.GetHeadImg(Application.Me.data.userId,false);
                //head.SetAvatorPath(fileParth);
            }
        }
        #endregion

    }
}
