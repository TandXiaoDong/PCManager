using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace ShikuIM
{
    /// <summary>
    /// GifImage.xaml 的交互逻辑
    /// </summary>
    public partial class GifImage : UserControl
    {
        public GifImage()
        {
            InitializeComponent();
        }

        #region 加载时
        /// <summary>
        /// 加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GifImageLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is string path)//获取路径
            {
                var imgs = new BitmapImage();
                imgs.BeginInit();
                imgs.UriSource = new Uri(path);//   
                imgs.EndInit();
                ImageBehavior.SetAnimatedSource(mainimage, imgs);//设置动画Uri
                //ImageBehavior.SetRepeatBehavior(mainimage, new RepeatBehavior(01));//重复一次     
            }
        }
        #endregion


        #region Gif播放完成后
        private void GifComplete(object sender, RoutedEventArgs e)
        {
            if (IsMouseOver)
            {
                var controller = ImageBehavior.GetAnimationController(mainimage);
                controller.Play();
            }
        }
        #endregion

        #region 鼠标悬浮时
        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var controller = ImageBehavior.GetAnimationController(mainimage);
            //if (controller.IsComplete)
            //{

            //}
            controller.Play();
            controller.Play();
            //controller.
        }
        #endregion


        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var controller = ImageBehavior.GetAnimationController(mainimage);
            controller.Pause();
        }


    }
}
