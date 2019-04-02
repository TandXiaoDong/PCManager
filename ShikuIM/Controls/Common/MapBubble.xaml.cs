using ShikuIM.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShikuIM
{
    /// <summary>
    /// MapBubble.xaml 的交互逻辑
    /// </summary>
    public partial class MapBubble : UserControl
    {

        /// <summary>
        /// 对应的消息
        /// </summary>
        public Messageobject Message
        {
            get { return (Messageobject)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// 标识消息属性
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(Messageobject), typeof(MapBubble), new PropertyMetadata(OnMessageChanged));


        #region 消息改变时
        private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var message = e.NewValue as Messageobject;
                var mapcontrol = (MapBubble)d;

                var serverUri = new Uri(System.Web.HttpUtility.HtmlDecode(message.content));//解码

                string url = Applicate.MapApiUrl + "location={0},{1}&title={2}&content={3}&output=html";//删除
                string mapurl = string.Format(url, message.location_x, message.location_y, "我的位置", message.objectId);

                string uri = message.content;

                //get info about the path
                string localpath = Applicate.LocalConfigData.CatchPath;
                //ChatBubbleItemModel message = mapcontrol.Tag as ChatBubbleItemModel;
                string filename = message.fileName.Substring(
                    message.fileName.LastIndexOf('/') + 1,
                    message.fileName.Length - message.fileName.LastIndexOf('/') - 1
                    );
                var localfile = localpath + filename;//获取缓存路径
                if (File.Exists(localfile))//if file exists, fill the image of the border
                {
                    var imguri = new Uri(localfile);
                    mapcontrol.MapImg.Source = new BitmapImage(imguri);
                    mapcontrol.transitioner.SelectedIndex = 1;
                    mapcontrol.MouseLeftButtonUp += (sen, res) =>
                    {
                        Process.Start(mapurl);
                    };
                }
                else//if not exists
                {
                    WebClient web = new WebClient();
                    //when the progress changed , set the value of the progressbar
                    web.DownloadProgressChanged += (s, ev) =>
                    {
                        mapcontrol.ImageProgress.Value = ev.ProgressPercentage;
                    };
                    //When download completed, fill the image of the border
                    web.DownloadFileCompleted += (s, ev) =>
                    {
                        if (ev.Error == null)
                        {
                            var imguri = new Uri(localfile);
                            mapcontrol.MapImg.Source = new BitmapImage(imguri);
                            mapcontrol.transitioner.SelectedIndex = 1;
                            mapcontrol.MouseLeftButtonUp += (sen, res) =>
                            {
                                Process.Start(mapurl);
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
            }
            catch (Exception ex)
            {
                ConsoleLog.Output("显示地址失败:" + ex.Message);
            }
        }
        #endregion

        #region Contructor
        public MapBubble()
        {
            InitializeComponent();
        }
        #endregion

        #region 控件加载事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class LocationService
    {
        //百度api
        private static string url = @"http://api.map.baidu.com/geocoder/v2/?location={0}&ak=WEc8RlPXzSifaq9RHxE1WW7lRKgbid6Y";
        /// <summary>
        /// 根据经纬度获取地理位置
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">经度</param>
        /// <returns>具体的地埋位置</returns>
        public static string GetLocation(string lat, string lng)
        {
            HttpClient client = new HttpClient();
            string location = string.Format("{0},{1}", lat, lng);
            return string.Format(url, location);
        }

    }
}
