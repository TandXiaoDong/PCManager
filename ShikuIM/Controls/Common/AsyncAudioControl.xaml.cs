using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM
{
    /// <summary>
    /// AsyncAudioControl.xaml 的交互逻辑
    /// </summary>
    public partial class AsyncAudioControl : UserControl
    {

        #region Private Members

        /// <summary>
        /// 本地音频路径
        /// </summary>
        private string LocalAudioPath { get; set; }
        #endregion

        #region Public ReadOnly Members
        // Using a DependencyProperty as the backing store for AudioFileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AudioFileNameProperty =
            DependencyProperty.Register(nameof(AudioFileName), typeof(string), typeof(AsyncAudioControl), new PropertyMetadata(OnFileNameChanged));

        // Using a DependencyProperty as the backing store for AudioUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AudioUriProperty =
            DependencyProperty.Register(nameof(AudioUri), typeof(string), typeof(AsyncAudioControl), new PropertyMetadata(OnAudioUriChanged));

        #endregion

        #region Public Members
        /// <summary>
        /// 音频文件名称
        /// </summary>
        public string AudioFileName
        {
            get { return (string)GetValue(AudioFileNameProperty); }
            set { SetValue(AudioFileNameProperty, value); }
        }

        /// <summary>
        /// 音频Uri
        /// </summary>
        public string AudioUri
        {
            get { return (string)GetValue(AudioUriProperty); }
            set { SetValue(AudioUriProperty, value); }
        }

        #endregion

        #region Constructor
        public AsyncAudioControl()
        {
            InitializeComponent();
        }
        #endregion

        #region 点击下载(暂无用)
        private void DownloadCommand(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 音频Uri变化时
        /// <summary>
        /// 音频Uri变化时
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnAudioUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                AsyncAudioControl audio = d as AsyncAudioControl;
                string uri = e.NewValue as string;
                string filepath = audio.AudioFileName;
                string filename = filepath.Substring(filepath.LastIndexOf('/') + 1,
                    filepath.Length - filepath.LastIndexOf("/") - 1);
                string localpath = Applicate.LocalConfigData.ChatPath + filename;
                #region Download Image File
                if (File.Exists(localpath))
                {
                    //设置控件本身的
                    audio.LocalAudioPath = localpath;
                }
                else if (uri.Contains("http"))//if it's Web url
                {
                    //get info about the path
                    //string localpath = Applicate.LocalConfigData.ChatDownloadPath;
                    WebClient web = new WebClient();
                    //when the progress changed , set the value of the progressbar
                    web.DownloadProgressChanged += (s, ev) =>
                    {
                        audio.AudioProgress.Value = ev.ProgressPercentage;
                    };
                    //When download completed, fill the image of the border
                    web.DownloadFileCompleted += (s, ev) =>
                    {
                        if (ev.Error == null)
                        {
                            //设置控件本身的
                            audio.LocalAudioPath = localpath;
                            //Applicate.GetWindow<MainWindow>().VlcPlayer.SourceProvider.MediaPlayer
                            Task.Run(() =>
                            {
                                //转码(不需要)
                                //string localChatpath = Applicate.LocalConfigData.ChatPath + filename.Replace("amr", "mp3");
                                //string resu = Helpers.AmrConvertToMp3(localTemppath, localChatpath);
                                //ConsoleLog.Output("转码：：：：：：" + resu);

                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    audio.transitioner.SelectedIndex = 1;
                                });
                                web.Dispose();// Dispose the WebClient
                            });
                        }
                        else
                        {
                            ConsoleLog.Output("音频下载失败:" + ev.Error.Message);
                        }
                    };
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Output("Download Aaudio Error:;;", ex);
                    }
                    //Start download image async
                    web.DownloadFileAsync(new Uri(uri), localpath);
                    #endregion
                }
            }
        }
        #endregion

        #region 文件名改变时
        /// <summary>
        /// 文件名改变时
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion


        #region 播放或暂停
        private void PlayOrPauseCommand(object sender, RoutedEventArgs e)
        {
            Applicate.GetWindow<MainWindow>().VlcPlayer.SourceProvider.MediaPlayer.Play(new FileInfo(LocalAudioPath));
        }
        #endregion
    }
}
