using ShikuIM.Model;
using ShikuIM.UserControls.Message;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShikuIM
{
    /// <summary>
    /// AsyncImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class AsyncImageControl : UserControl
    {

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...

        /// <summary>
        /// 消息气泡临时的消息
        /// </summary>
        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register(
                nameof(ImageUri),
                typeof(string),
                typeof(AsyncImageControl),
                new PropertyMetadata(OnBubbleMsgChanged));

        #region Public Member

        public string ImageFileSize
        {
            get { return (string)GetValue(ImageFileSizeProperty); }
            set { SetValue(ImageFileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageFileSizeProperty =
            DependencyProperty.Register(nameof(ImageFileSize), typeof(string), typeof(AsyncImageControl), new PropertyMetadata(OnFileSizeChanged));

        /// <summary>
        /// 消息气泡临时的消息
        /// </summary>
        public string ImageUri
        {
            get { return (string)GetValue(ImageUriProperty); }
            set { SetValue(ImageUriProperty, value); }
        }
        #endregion

        #region Contructor
        public AsyncImageControl()
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
            AsyncImageControl imagecontrol = (AsyncImageControl)d;
            if (e.NewValue != null)
            {
                #region Download Image File
                string uri = (string)e.NewValue;
                try
                {

                    if (uri.Contains("http"))//if it's Web url
                    {
                        //get info about the path
                        string localpath = Applicate.LocalConfigData.ChatDownloadPath;
                        ChatBubbleItemModel msg = imagecontrol.Tag as ChatBubbleItemModel;
                        string filename = msg.fileName.Substring(
                            msg.fileName.LastIndexOf('/') + 1,
                            msg.fileName.Length - msg.fileName.LastIndexOf('/') - 1
                            );
                        var localfile = localpath + filename;
                        if (File.Exists(localfile))//if file exists, fill the image of the border
                        {
                            try
                            {
                                var imguri = new Uri(localfile);
                                imagecontrol.Imgborder.Source = new BitmapImage(imguri);
                                imagecontrol.transitioner.SelectedIndex = 1;
                                imagecontrol.MouseLeftButtonUp += (sen, res) =>
                                {
                                    var photoView = new PictureWindow().GetPhotoView(localfile);
                                    photoView.Show();
                                //photoView.Activate();
                                //photoView.Focus();
                            };
                            }
                            catch (Exception ex)
                            {
                                ConsoleLog.Output("显示图片cc" + ex.Message);
                            }
                        }
                        else//if not exists
                        {
                            using (WebClient web = new WebClient())
                            {
                                try
                                {
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
                                            var imguri = new Uri(localfile);
                                            imagecontrol.Imgborder.Source = new BitmapImage(imguri);
                                            imagecontrol.transitioner.SelectedIndex = 1;
                                            imagecontrol.BtnDownload.Visibility = Visibility.Collapsed;//隐藏下载按钮
                                        imagecontrol.MouseLeftButtonUp += (sen, res) =>
                                            {
                                                var photoView = new PictureWindow().GetPhotoView(localfile);
                                                photoView.Show();
                                                photoView.Activate();
                                                photoView.Focus();
                                            };
                                            web.Dispose();//Dispose the WebClient
                                    }
                                        else
                                        {
                                            ConsoleLog.Output("图片下载失败");
                                        }
                                    };
                                    //Start download image async
                                    web.DownloadFileAsync(new Uri(uri), localfile);
                                }
                                catch (Exception ex)
                                {
                                    ConsoleLog.Output("AsyncImageControl--GetImage--Error" + ex.Message);
                                    imagecontrol.Dispatcher.Invoke(() =>
                                    {
                                        imagecontrol.BtnDownload.Visibility = Visibility.Visible;//显示下载按钮
                                });
                                }
                            }
                        }
                    }
                    #endregion
                    else//if it's local path 
                    {
                        if (File.Exists(uri))
                        {
                            imagecontrol.Imgborder.Source = new BitmapImage(new Uri(uri));
                            imagecontrol.transitioner.SelectedIndex = 1;
                            imagecontrol.MouseLeftButtonUp += (sen, res) =>
                            {
                                var photoView = new PictureWindow().GetPhotoView(uri);
                                photoView.Show();
                                photoView.Activate();
                                photoView.Focus();
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleLog.Output("Download Aaudio Error:;;", ex);
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
            AsyncImageControl imageControl = (AsyncImageControl)d;
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

    }
}
