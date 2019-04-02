using ShikuIM.Model;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ShikuIM
{
    /// <summary>
    /// AsyncImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class AsyncVideoControl : UserControl
    {

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...

        /// <summary>
        /// 消息气泡临时的消息
        /// </summary>
        public static readonly DependencyProperty VideoUriProperty =
            DependencyProperty.Register(
                nameof(VideoUri),
                typeof(string),
                typeof(AsyncVideoControl),
                new PropertyMetadata(OnBubbleMsgChanged));

        #region Public Member

        public string VIdeoFileSize
        {
            get { return (string)GetValue(VIdeoFileSizeProperty); }
            set { SetValue(VIdeoFileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VIdeoFileSizeProperty =
            DependencyProperty.Register(nameof(VIdeoFileSize), typeof(string), typeof(AsyncVideoControl), new PropertyMetadata(OnFileSizeChanged));

        /// <summary>
        /// 消息气泡临时的消息
        /// </summary>
        public string VideoUri
        {
            get { return (string)GetValue(VideoUriProperty); }
            set { SetValue(VideoUriProperty, value); }
        }
        #endregion

        #region Contructor
        public AsyncVideoControl()
        {
            InitializeComponent();
        }
        #endregion

        #region 消息改变时
        /// <summary>
        /// 消息改变时
        /// </summary>
        /// <param name="d">母控件</param>
        /// <param name="e">更改相关值</param>
        private static void OnBubbleMsgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AsyncVideoControl imagecontrol = (AsyncVideoControl)d;
            if (e.NewValue != null)
            {
                #region Download Image File
                string uri = (string)e.NewValue;
                if (uri.Contains("http"))//if it's Web url
                {
                    //get info about the path
                    string localpath = Applicate.LocalConfigData.ChatDownloadPath;
                    ChatBubbleItemModel msg = imagecontrol.Tag as ChatBubbleItemModel;
                    if (msg.fileName == null)
                    {
                        return;
                    }
                    string filename = msg.fileName.Substring(
                        msg.fileName.LastIndexOf('/') + 1,
                        msg.fileName.Length - msg.fileName.LastIndexOf('/') - 1
                        );
                    var localfile = localpath + filename;
                    if (File.Exists(localfile))//if file exists, fill the image of the border
                    {
                        var imguri = new Uri(localfile);
                        BitmapImage mapImage = ImageUtil.ConvertBitmapToBitmapImage(WindowsThumbnailProvider.GetThumbnail(localfile, 256, 256, ThumbnailOptions.None));
                        imagecontrol.Imgborder.Background = new ImageBrush(mapImage);
                        imagecontrol.transitioner.SelectedIndex = 1;
                    }
                    else//if not exists
                    {
                        try
                        {

                            WebClient web = new WebClient();
                            //when the progress changed , set the value of the progressbar
                            web.DownloadProgressChanged += (s, ev) =>
                            {
                                imagecontrol.ImageProgress.Value = ev.ProgressPercentage;
                            };
                            //When download completed, fill the image of the border
                            web.DownloadFileCompleted += (s, ev) =>
                            {
                                if (ev.Error == null)
                                {
                                    BitmapImage mapImage = ImageUtil.ConvertBitmapToBitmapImage(WindowsThumbnailProvider.GetThumbnail(localfile, 256, 256, ThumbnailOptions.None));
                                    imagecontrol.Imgborder.Background = new ImageBrush(mapImage);
                                    imagecontrol.transitioner.SelectedIndex = 1;
                                    //Set Border Background(Not for now)
                                    //var imguri = new Uri(localfile);
                                    //imagecontrol.Imgborder.Background = new ImageBrush(new BitmapImage(imguri));
                                    web.Dispose();// Dispose the WebClient
                                }
                                else
                                {
                                    ConsoleLog.Output("视频下载失败");
                                }
                            };
                            //Start download image async
                            web.DownloadFileAsync(new Uri(uri), localfile);
                        }
                        catch (Exception ex)
                        {
                            ConsoleLog.Output("Download Aaudio Error:;;", ex);
                        }
                        #endregion
                    }
                }
                else//if it's local path 
                {
                    if (File.Exists(uri))
                    {
                        BitmapImage mapImage = ImageUtil.ConvertBitmapToBitmapImage(WindowsThumbnailProvider.GetThumbnail(uri, 256, 256, ThumbnailOptions.None));
                        imagecontrol.Imgborder.Background = new ImageBrush(mapImage);
                        imagecontrol.transitioner.SelectedIndex = 1;
                    }
                }
            }
            else
            {
                return;
            }
        }
        #endregion

        #region 文件大小改变时
        /// <summary>
        /// 文件大小改变时,赋值到TextBlock
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnFileSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AsyncVideoControl imageControl = (AsyncVideoControl)d;
            if (e.NewValue != null)
            {
                imageControl.BorderFileSize.Visibility = Visibility.Visible;
                imageControl.txtFilesize.Text = (string)e.NewValue;
            }
            else
            {
                imageControl.BorderFileSize.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region 播放
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            //Get info about the path
            string localfile = "";
            ChatBubbleItemModel msg = (ChatBubbleItemModel)(this).Tag;
            string filename = msg.fileName.Substring(
                msg.fileName.LastIndexOf('/') + 1,
                msg.fileName.Length - msg.fileName.LastIndexOf('/') - 1
                );
            string localpath = Applicate.LocalConfigData.ChatDownloadPath;
            localfile = localpath + filename;
            //VideoPlayer playerWindow = new VideoPlayer(localfile);
            //playerWindow.Show();

            //Process.Start("Explorer", "/select," + localfile);//Open file directory

            ME_VideoPlayer playerWindow = new ME_VideoPlayer(localfile);
            playerWindow.Show();

        }
        #endregion


    }
}
