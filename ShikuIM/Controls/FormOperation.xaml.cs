
using ShikuIM.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM.UserControls
{
    /// <summary>
    /// formClose.xaml 的交互逻辑
    /// </summary>
    public partial class FormOperation : UserControl, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Private Member
        private DependencyWindow window;

        #endregion


        #region 需要操作的窗口
        private bool isMax;

        public bool IsMax
        {
            get { return isMax; }
            set
            {
                if (isMax == value)
                {
                    return;
                }

                isMax = value;
                OnPropertyChanged(nameof(IsMax));
            }
        }

        /// <summary>
        /// 需要操作的窗口
        /// </summary>
        public object Window
        {
            get { return window.Window; }
            set
            {
                if (window == null)
                {
                    window = new DependencyWindow();
                }

                window.Window = value as Window;//强转为Window
            }
        }
        #endregion


        public FormOperation()
        {
            InitializeComponent();//窗体的初始过程需要第一时间执行
            this.DataContext = this;//设置绑定
        }

        #region 构造方法
        /// <summary>
        /// 窗体标题栏
        /// </summary>
        /// <param name="form">需要操作的窗体</param>
        /// <param name="isMax">是否需要最大化按钮</param>
        public FormOperation(Window form, bool isMax = true)
        {
            InitializeComponent();//窗体的初始过程需要第一时间执行
            this.DataContext = this;//设置绑定
            //判断是否需要最大化按钮
            if (!isMax)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    //动态适应窗口
                    this.btn_max.Visibility = Visibility.Collapsed;
                });
            }
            //传入一个窗口对象进行操作
            Window = form;
        }
        #endregion

        #region 关闭窗口
        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Window != null)
            {
                if (Window is Window)
                {
                    ((Window)Window).Close();
                }
                else
                {
                    ((DependencyWindow)Window).Window.Close();
                }
            }
        }
        #endregion

        #region 窗口最大化或者恢复
        /// <summary>
        /// 窗口最大化或者恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_max_Click(object sender, RoutedEventArgs e)
        {
            if (Window != null)
            {
                //if (Window is Window)
                //{
                //    if (((Window)Window).WindowState != WindowState.Maximized)
                //        ((Window)Window).WindowState = WindowState.Maximized;
                //    else
                //        ((Window)Window).WindowState = WindowState.Normal;
                //}
                //如果窗体状态为最大化时，则还原窗口
                if (((Window)Window) != null)
                {
                    if (((Window)Window).WindowState == WindowState.Normal)
                    {
                        ((Window)Window).WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        //否则就最大化窗口
                        ((Window)Window).WindowState = WindowState.Normal;
                    }
                }
            }
        }
        #endregion

        #region 窗口最小化
        /// <summary>
        /// 窗口最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_min_Click(object sender, RoutedEventArgs e)
        {
            if (window != null)
            {
                window.Window.WindowState = WindowState.Minimized;
            }
        }
        #endregion

    }
}
