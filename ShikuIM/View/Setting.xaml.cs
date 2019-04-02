using CommonServiceLocator;
using MaterialDesignThemes.Wpf;
using ShikuIM.Model;
using ShikuIM.UserControls;
using ShikuIM.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM
{
    /// <summary>
    /// SetUp.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        private SnackbarMessageQueue _snackBar;

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

        private static SettingsWindow thisSetUp { get; set; }


        /// <summary>
        /// 提示控件
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return _snackBar; }
            set { _snackBar = value; }
        }
        #region Contructor
        public SettingsWindow()
        {
            InitializeComponent();
            Snackbar = new SnackbarMessageQueue();
            #region 添加右上角按钮
            FormOperation operation = new FormOperation(this, false);
            this.body.Children.Add(operation);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            #endregion
        }
        #endregion

        #region 窗体加载时
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region 单例方法
        public static SettingsWindow getSetUp()
        {
            if (thisSetUp != null)
            {
                thisSetUp.Close();
            }
            thisSetUp = new SettingsWindow();
            return thisSetUp;
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

        #region 清空聊天记录
        /// <summary>
        /// 清空聊天记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmptyRecords_Click(object sender, RoutedEventArgs e)
        {
            Messageobject msgService = new Messageobject();
            int i = msgService.ClearAllMessage();
            var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
            if (mControl != null)
            {
                //mControl.SP_chatBottom.Children.Clear();//清理消息
                mControl.RecentMessageList.ToList().ForEach(d => { d.UnReadCount = 0; d.MessageItemContent = ""; });
                mControl.SetTotalUnReadCount();
                mControl.UpdateMessageList();
            }
            Snackbar.Enqueue("已清空");
            ConsoleLog.Output("删除" + i.ToString());
        }
        #endregion

        #region 清空缓存
        private void EmptyCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileUtil.DeleteHeadImg(Applicate.MyAccount.userId);//删除本地所有头像
                Snackbar.Enqueue("已清空");
            }
            catch (Exception ex)
            {
                LogHelper.log.Info("清空缓存失败:" + ex.Message);
                Snackbar.Enqueue("清空缓存失败");
            }
        }
        #endregion

    }
}
