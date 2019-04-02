using ShikuIM.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM
{
    /// <summary>
    /// AsyncImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class AsyncFileControl : UserControl
    {

        /// <summary>
        /// 文件的网络路径
        /// </summary>
        public string FileUri
        {
            get { return (string)GetValue(FileUriProperty); }
            set { SetValue(FileUriProperty, value); }
        }

        /// <summary>
        /// Dependency Static Property
        /// </summary>
        public static readonly DependencyProperty FileUriProperty =
            DependencyProperty.Register(nameof(FileUri), typeof(string), typeof(AsyncFileControl), new PropertyMetadata(OnUriChanged));



        #region Event
        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AsyncFileControl control = (AsyncFileControl)d;
            if (e.NewValue != null)
            {
                string uri = (string)e.NewValue;
                ChatBubbleItemModel msg = (ChatBubbleItemModel)control.Tag;
                //Get info about the path
                string localpath = Applicate.LocalConfigData.ChatDownloadPath;
                //Get filename 
                string filename = "";
                if (string.IsNullOrEmpty(msg.fileName))
                {
                    filename = uri.Substring(
                   uri.LastIndexOf('/') + 1,
                   uri.Length - uri.LastIndexOf('/') - 1
                   );
                }
                else
                {
                    filename = msg.fileName.Substring(msg.fileName.LastIndexOf('/') + 1, msg.fileName.Length - msg.fileName.LastIndexOf("/") - 1);
                }
                var localfile = localpath + filename;
                if (File.Exists(localfile))// If file exists
                {
                    control.page.SelectedIndex = 1;//Go to completed page
                }
                else
                {
                    WebClient web = new WebClient();
                    control.FileProgress.IsIndeterminate = true;
                    control.page.SelectedIndex = 0;//Go to download page
                    web.DownloadProgressChanged += (s, ev) =>
                    {
                        control.FileProgress.Value = ev.ProgressPercentage;//refresh the progress of the FileProgress
                    };
                    web.DownloadFileCompleted += (s, ev) =>
                    {
                        control.FileProgress.IsIndeterminate = false;// Stop the animation of the progress 
                        control.page.SelectedIndex = 1;//Go to completed page
                    };
                    try
                    {
                        web.DownloadFileAsync(new System.Uri(uri), localfile);//Start download Async
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Output("Download File Error:;;", ex);
                    }
                }
            }
            else
            {
                return;
            }
        }
        #endregion




        #region Contructor
        public AsyncFileControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 开始下载
        private void DownliadCommand(object sender, System.Windows.RoutedEventArgs e)
        {
            //FileUri
        }
        #endregion

        #region 打开文件夹
        private void OpenDirectoryCommand(object sender, RoutedEventArgs e)
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
            Process.Start("Explorer", "/select," + localfile);//Open file directory
        }
        #endregion
    }
}
