using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace ShikuIM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ME_VideoPlayer : Window
    {
        private DispatcherTimer m_Timer;              //用来更新滚动条
        private bool m_IsNoSound = false;               //判断是否禁止声音
        private bool m_IsClickSlider = false;           //判断是否鼠标点击滚动条

        public static DependencyProperty VideoImageProperty = DependencyProperty.Register("VideoPath", typeof(String), typeof(Window),
          new PropertyMetadata(string.Empty));

        public string VideoPath
        {
            get
            {
                return this.GetValue(VideoImageProperty).ToString();
            }
            set
            {
                this.SetValue(VideoImageProperty, value);
            }
        }

        public ME_VideoPlayer(string filePath)
        {
            InitializeComponent();
            VideoPath = @"D:\AudioOpened.bmp";
            m_Timer = new DispatcherTimer();
            m_Timer.Interval = new TimeSpan(1000);
            m_Timer.Tick += new EventHandler(timer_Tick);
            OpenMedia(filePath);
        }

        #region Public Methods
        /// <summary>
        /// 打开媒体
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenMedia(string filePath)
        {
            MediaShowElement.Source = new Uri(filePath);
            //MediaShowElement.Source = new Uri(@"C:\Users\Administrator\Desktop\视频测试2.mp4");
        }
        #endregion

        #region 窗口加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MediaShowElement.Play();
            m_Timer.Start();
            BtnOperator.Style = Resources["MediaBtnPause"] as Style;
            BtnOperator.Tag = "Pause";
        }
        #endregion

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

        #region 关闭
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 键盘检测
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
        #endregion

        #region 窗口随意拖动
        private void OnPlayerViewLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();

                if (BtnOperator.Tag.ToString() == "Play")
                {
                    MediaShowElement.Play();
                    m_Timer.Start();
                    BtnOperator.Style = Resources["MediaBtnPause"] as Style;
                    BtnOperator.Tag = "Pause";
                }
                else
                {
                    MediaShowElement.Pause();
                    BtnOperator.Style = Resources["MediaBtnPlay"] as Style;
                    BtnOperator.Tag = "Play";
                }
            }
        }
        #endregion

        #region Event Handle
        void timer_Tick(object sender, EventArgs e)
        {
            if (MediaShowElement.NaturalDuration.HasTimeSpan && !m_IsClickSlider)
            {
                MediaTimeSplider.Value = MediaShowElement.Position.TotalMilliseconds;
                txtNowPostion.Text = MediaShowElement.Position.Hours.ToString().PadLeft(2, '0') + ":" +
                         MediaShowElement.Position.Minutes.ToString().PadLeft(2, '0') + ":" +
                         MediaShowElement.Position.Seconds.ToString().PadLeft(2, '0');
            }

            if (MediaTimeSplider.Value > 0 && MediaTimeSplider.Value == (double)MediaShowElement.Position.Seconds * 1000)
            {
                MediaShowElement.Stop();

                BtnOperator.Style = Resources["MediaBtnPlay"] as Style;
                BtnOperator.Tag = "Play";
            }
        }

        private void BtnModel_Click(object sender, RoutedEventArgs e)
        {
            if (BtnModel.Tag.ToString() == "Normal")
            {
                BtnModel.Style = Resources["MediaBtnMaxMode"] as Style;
                BtnModel.Tag = "Max";
            }
            else
            {
                BtnModel.Style = Resources["MediaBtnNormalMode"] as Style;
                BtnModel.Tag = "Normal";
            }
        }

        private void BtnOperator_Click(object sender, RoutedEventArgs e)
        {
            if (BtnOperator.Tag.ToString() == "Play")
            {
                MediaShowElement.Play();
                m_Timer.Start();
                BtnOperator.Style = Resources["MediaBtnPause"] as Style;
                BtnOperator.Tag = "Pause";
            }
            else
            {
                MediaShowElement.Pause();
                BtnOperator.Style = Resources["MediaBtnPlay"] as Style;
                BtnOperator.Tag = "Play";
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            MediaShowElement.Stop();

            BtnOperator.Style = Resources["MediaBtnPlay"] as Style;
            BtnOperator.Tag = "Play";
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            MediaShowElement.Position += TimeSpan.FromSeconds(10);
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            MediaShowElement.Position -= TimeSpan.FromSeconds(10);
        }

        private void VoiceImage_MouseEnter(object sender, MouseEventArgs e)
        {
            VoiceImage.Cursor = Cursors.Hand;
        }

        private void VoiceImage_MouseLeave(object sender, MouseEventArgs e)
        {
            VoiceImage.Cursor = Cursors.None;
        }

        private void VoiceImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (VoiceImage.Tag.ToString() == "Sound")
            {
                VoiceImage.Tag = "NoSound";
                MediaShowElement.Volume = 0;
                m_IsNoSound = true;
                VideoPath = @"E:\VSSWorkFolder\门禁系统3.0\Debug\bin\Images\AlarmWorkStation\AudioClosed.bmp";
            }
            else
            {
                VoiceImage.Tag = "Sound";
                MediaShowElement.Volume = VolumeSlider.Value;
                m_IsNoSound = false;
                VideoPath = @"E:\VSSWorkFolder\门禁系统3.0\Debug\bin\Images\AlarmWorkStation\AudioOpened.bmp";

            }
        }

        void MediaTimeSplider_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(MediaTimeSplider);
            double dValue = (pt.X / MediaTimeSplider.ActualWidth) * MediaTimeSplider.Maximum;
            TimeSpan timeNow = TimeSpan.FromMilliseconds(dValue);
            MediaTimeSplider.ToolTip = timeNow.Hours.ToString().PadLeft(2, '0') + ":" +
                timeNow.Minutes.ToString().PadLeft(2, '0') + ":" +
                timeNow.Seconds.ToString().PadLeft(2, '0');
        }

        private void MediaShowElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaTimeSplider.Maximum = MediaShowElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            txtTimeTotal.Text = MediaShowElement.NaturalDuration.TimeSpan.Hours.ToString().PadLeft(2, '0') + ":" +
                    MediaShowElement.NaturalDuration.TimeSpan.Minutes.ToString().PadLeft(2, '0') + ":" +
                    MediaShowElement.NaturalDuration.TimeSpan.Seconds.ToString().PadLeft(2, '0');
            txtNowPostion.Text = "00:00:00";
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!m_IsNoSound)
            {
                MediaShowElement.Volume = VolumeSlider.Value;
            }
        }

        private void Local_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(MediaTimeSplider, e.GetPosition((UIElement)MediaTimeSplider));
            if (result != null)
                m_IsClickSlider = true;
        }

        private void Local_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (m_IsClickSlider)
            {
                MediaShowElement.Position = TimeSpan.FromMilliseconds(MediaTimeSplider.Value);
                m_IsClickSlider = false;
            }
        }
        #endregion
    }
}
