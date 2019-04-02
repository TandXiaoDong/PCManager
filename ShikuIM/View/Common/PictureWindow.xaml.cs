using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ShikuIM.UserControls.Message
{
    /// <summary>
    /// PhotoView.xaml 的交互逻辑
    /// </summary>
    public partial class PictureWindow : Window
    {
        private bool isMouseLeftButtonDown = false;
        Point previousMousePoint = new Point(0, 0);
        double db_Width;
        double db_Height;
        public PictureWindow()
        {
            InitializeComponent();
            db_Width = SystemParameters.PrimaryScreenWidth * 0.7;
            db_Height = SystemParameters.PrimaryScreenHeight * 0.7;
        }
        #region 加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 显示窗口位置大小
            //this.WindowState = System.Windows.WindowState.Normal;
            //this.WindowStyle = System.Windows.WindowStyle.None;
            //this.ResizeMode = System.Windows.ResizeMode.NoResize;
            //this.Topmost = true;

            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth * 0.15;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight * 0.15;
            this.Width = db_Width;
            this.Height = db_Height;
            #endregion
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
            }
        }
        #endregion
        /// <summary>
        /// 设置媒体的Uri
        /// </summary>
        /// <param name="uri"></param>
        public void setResource(Uri uri)
        {
            try
            {
                if (uri.AbsoluteUri != null)
                {
                    //img.Source = new BitmapImage(uri);
                    BitmapImage image = new BitmapImage(uri);
                    img.Source = image; //指定为已经下载的图片对象
                    double Img_Width = image.Width;
                    double Img_Heigth = image.Height;//记录原有的宽和高
                    if (Img_Width > db_Width || Img_Heigth > db_Height)
                    {
                        if (Img_Width <= db_Width || Img_Heigth <= db_Height)
                        {
                            if (Img_Width <= db_Width)
                            {
                                img.Width = Img_Width * db_Height / Img_Heigth;
                                img.Height = db_Height;
                            }
                            else
                            {
                                img.Height = Img_Heigth * db_Width / Img_Width;
                                img.Width = db_Width;
                            }
                        }
                        else
                        {
                            if (Img_Width / db_Width >= Img_Heigth / db_Height)
                            {
                                img.Height = Img_Heigth * db_Width / Img_Width;
                                img.Width = db_Width;
                            }
                            else
                            {
                                img.Width = Img_Width * db_Height / Img_Heigth;
                                img.Height = db_Height;
                            }
                        }
                    }
                    else
                    {
                        img.Width = Img_Width;
                        img.Height = Img_Heigth;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.log.Error(e.Message, e);
                ConsoleLog.Output(e.Message);
            }
        }
        /// <summary>
        /// 设置媒体的Uri
        /// </summary>
        /// <param name="uri"></param>
        public void setResource(string localPath)
        {
            try
            {
                Uri uri = new Uri(localPath, UriKind.Absolute);//指定
                if (uri.AbsoluteUri != null)
                {
                    //img.Source = new BitmapImage(uri);
                    BitmapImage image = FileUtil.ReadFileByteToBitmap(localPath);//指定为已经下载的图片对象
                    thisPhotoView.img.Source = image;
                    double Img_Width = image.Width;
                    double Img_Heigth = image.Height;//记录原有的宽和高
                    if (Img_Width > db_Width || Img_Heigth > db_Height)
                    {
                        if (Img_Width <= db_Width || Img_Heigth <= db_Height)
                        {
                            if (Img_Width <= db_Width)
                            {
                                thisPhotoView.img.Width = Img_Width * db_Height / Img_Heigth;
                                thisPhotoView.img.Height = db_Height;
                            }
                            else
                            {
                                thisPhotoView.img.Height = Img_Heigth * db_Width / Img_Width;
                                thisPhotoView.img.Width = db_Width;
                            }
                        }
                        else
                        {
                            if (Img_Width / db_Width >= Img_Heigth / db_Height)
                            {
                                thisPhotoView.img.Height = Img_Heigth * db_Width / Img_Width;
                                thisPhotoView.img.Width = db_Width;
                            }
                            else
                            {
                                thisPhotoView.img.Width = Img_Width * db_Height / Img_Heigth;
                                thisPhotoView.img.Height = db_Height;
                            }
                        }
                    }
                    else
                    {
                        thisPhotoView.img.Width = Img_Width;
                        thisPhotoView.img.Height = Img_Heigth;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.log.Error(e.Message, e);
                ConsoleLog.Output(e.Message);
            }
        }
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            isMouseLeftButtonDown = true;
            previousMousePoint = e.GetPosition(img);
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }

        private void img_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }

        #region 窗口大小
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        #endregion

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown == true)
            {
                Point position = e.GetPosition(img);
                tlt.X += position.X - this.previousMousePoint.X;
                tlt.Y += position.Y - this.previousMousePoint.Y;
            }
        }

        private void img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point centerPoint = e.GetPosition(img);

            double val = (double)e.Delta / 2000;   //描述鼠标滑轮滚动
            if (sfr.ScaleX < 0.3 && sfr.ScaleY < 0.3 && e.Delta < 0)
            {
                return;
            }
            sfr.CenterX = centerPoint.X;
            sfr.CenterY = centerPoint.Y;

            sfr.ScaleX += val;
            sfr.ScaleY += val;
        }
        #region 窗体关闭时 ,只隐藏 ,不关闭
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thisPhotoView = null;
        }
        #endregion

        #region 窗体拖动
        private void body_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            e.Handled = true;
        }
        #endregion

        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 需实现单例的对象
        /// </summary>
        private static PictureWindow thisPhotoView { get; set; }

        #region 单例方法
        public PictureWindow GetPhotoView(string localPath)
        {
            if (thisPhotoView == null)
            {
                thisPhotoView = new PictureWindow();
            }
            setResource(localPath);
            return thisPhotoView;
        }
        #endregion


    }
}
