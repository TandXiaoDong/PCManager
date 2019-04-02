using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM
{
    /// <summary>
    /// VideoPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPlayer : Window
    {

        /// <summary>
        /// 媒体路径
        /// </summary>
        public string MediaPath { get; set; }

        #region Constructor
        public VideoPlayer()
        {
            InitializeComponent();
        }


        public VideoPlayer(string path)
        {
            InitializeComponent();
            this.MediaPath = path;//设置路径
            //Task.Run(() =>
            //{
            //});
        }
        #endregion

        #region 窗口加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Player.VlcConfig();
            Player.SourceProvider.MediaPlayer.Play(new FileInfo(MediaPath));
            isPlay = true;
            //Player.
        }
        #endregion

        #region 关闭
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 音量
        bool haveVoice = true;
        private void Voice_Click(object sender, RoutedEventArgs e)
        {
            if (haveVoice == true)
            {
            }
            else
            {

            }
        }
        #endregion

        #region 播放、暂停
        bool isPlay = false;
        private void Video_Click(object sender, RoutedEventArgs e)
        {
            if (isPlay == true)
            {
                Player.SourceProvider.MediaPlayer.Stop();
            }
            else
            {
                Player.SourceProvider.MediaPlayer.Play();
            }
        }
        #endregion

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}
