using ShikuIM.Resource;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShikuIM.UserControls.Message
{
    #region 媒体详情类型
    /// <summary>
    /// 媒体详情类型
    /// </summary>
    public enum mediaType
    {
        image = 1,
        video = 2
    }
    #endregion


    /// <summary>
    /// 媒体详情窗口
    /// </summary>
    public partial class MediaView : Window
    {

        /// <summary>
        /// 媒体信息的类型
        /// </summary>
        public mediaType Type { get; set; }

        /// <summary>
        /// 媒体的本地Uri
        /// </summary>
        public Uri localUri { get; set; }

        /// <summary>
        /// 是否为播放状态
        /// </summary>
        private bool isPlay;

        /// <summary>
        /// 播放背景
        /// </summary>
        private ImageBrush BGI_Play = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(ShikuRec.play.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));

        /// <summary>
        /// 暂停背景
        /// </summary>
        private ImageBrush BGI_Pause = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(ShikuRec.pause.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));

        #region 构造方法
        /// <summary>
        /// 初始化一个媒体展示控件
        /// </summary>
        public MediaView()
        {
            InitializeComponent();
            media_Detial.LoadedBehavior = MediaState.Manual;//设置媒体为手动执行
        }
        #endregion

        #region 窗口随意拖动
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }//窗口随意拖动
        #endregion

        #region 加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 右上方三个按钮
            //右上方三个按钮
            FormOperation operation = new FormOperation(this, true);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            this.body.Children.Add(operation);//添加控件
            #endregion


            media_Detial.Source = localUri;//设置Source
            media_Detial.Margin = new Thickness(0, 20, 0, 0);//设置顶部边距20

            #region 如果为视频类型 ,就添加播放按钮
            //如果为媒体类型 ,就添加播放按钮
            if (Type == mediaType.video)
            {
                //如果是视频类型 ,就手动播放音频
                media_Detial.LoadedBehavior = MediaState.Manual;
            }
            else if (Type == mediaType.image)
            {
                //隐藏播放按钮
                controlBtn.Visibility = Visibility.Hidden;
                media_Detial.LoadedBehavior = MediaState.Play;
            }
            #endregion

            //设置宽度
            bd_out.Width = media_Detial.Width;
            body.Width = media_Detial.Width;
            Width = media_Detial.Width;
        }
        #endregion

        #region 设置媒体的Uri
        /// <summary>
        /// 设置媒体的Uri
        /// </summary>
        /// <param name="uri"></param>
        public void setResource(Uri uri)
        {
            if (uri.AbsoluteUri != null)
            {
                media_Detial.Source = uri;//设置Source
            }
        }
        #endregion


        #region 播放按钮点击事件
        private void btn_PlayClick(object sender, MouseButtonEventArgs e)
        {
            if (media_Detial.LoadedBehavior == MediaState.Manual)
            {
                if (isPlay)//如果正在播放视频
                {
                    media_Detial.Pause();
                    controlBtn.gd_content.Background = BGI_Play;//背景图片设为播放
                    isPlay = false;//已停止
                }
                else
                {
                    media_Detial.Play();
                    controlBtn.gd_content.Background = BGI_Pause;//背景设置为暂停
                    isPlay = true;//正在播放
                }
            }
        }
        #endregion

        #region 当并没有媒体播放时，将媒体设置为停止播放状态
        private void media_Detial_MediaEnded(object sender, RoutedEventArgs e)
        {
            //如果为图片
            if (Type == mediaType.image)
            {
                return;
            }
            //停止媒体的播放
            media_Detial.Stop();
            //并设置按钮为播放状态
            controlBtn.gd_content.Background = BGI_Play;
        }
        #endregion

        #region 键盘按下事件
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    //如果按下按钮为Esc键，就关闭此窗口
                    Close();
                    break;
                case Key.Space:
                    //如果按下按钮为空格键，调用播放按钮点击的事件方法
                    btn_PlayClick(null, null);
                    break;
            }
        }
        #endregion

        #region 媒体元素加载事件
        private void media_Detial_Loaded(object sender, RoutedEventArgs e)
        {
            //设置宽度
            bd_out.Width = media_Detial.Width;
            body.Width = media_Detial.Width;
            Width = media_Detial.Width;
        }
        #endregion

        #region 窗体关闭时 ,只隐藏 ,不关闭
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;//取消关闭的操作
            //只隐藏
            this.Hide();
            return;
        }
        #endregion



    }
}
