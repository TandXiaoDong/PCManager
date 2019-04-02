using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ShikuIM.View
{
    /// <summary>
    /// 消息气泡集合
    /// </summary>
    public partial class ChatBubbleListControl : UserControl
    {
        #region Public Static Token

        /// <summary>
        /// 下拉消息列表到底部通知Token
        /// </summary>
        public static string ScrollChatBubbleMessageToBottom { get; set; } = nameof(ScrollChatBubbleMessageToBottom);

        /// <summary>
        /// 控制消息列表滚动指定偏移量通知Token
        /// </summary>
        public static string ScrollChatMessageListVerticalOffset { get; } = nameof(ScrollChatMessageListVerticalOffset);

        /// <summary>
        /// FAB显示
        /// </summary>
        public static string FloatingActionButtonShowUp { get; } = nameof(FloatingActionButtonShowUp);

        /// <summary>
        /// FAB隐藏
        /// </summary>
        public static string FloatingActionButtonHideOff { get; } = nameof(FloatingActionButtonHideOff);
        #endregion

        public bool IsInviteFloatButtonHide { get; private set; }


        /// <summary>
        /// 隐藏的故事板
        /// </summary>
        public Storyboard HideStory;

        /// <summary>
        /// 显示的故事板
        /// </summary>
        public Storyboard ShowStory;

        #region Contructor
        public ChatBubbleListControl()
        {
            InitializeComponent();
            HideStory = FindResource("ScrollViewerUp") as Storyboard;
            ShowStory = FindResource("OnScrollChangedDown") as Storyboard;
            RegisterMessenger();
        }
        #endregion

        #region 注册通知
        /// <summary>
        /// 注册通知
        /// </summary>
        private void RegisterMessenger()
        {
            Messenger.Default.Register<bool>(this, ScrollChatBubbleMessageToBottom, value =>
            {
                if (value)
                {
                    SVChatBubbleList.ScrollToBottom();
                }
            });
            Messenger.Default.Register<double>(this, ScrollChatMessageListVerticalOffset, offset =>
            {
                SVChatBubbleList.ScrollToVerticalOffset(offset);
            });

            Messenger.Default.Register<bool>(this, FloatingActionButtonShowUp, value =>
            {
                if (value)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ShowStory.Begin();
                    });
                }
            });
            Messenger.Default.Register<bool>(this, FloatingActionButtonHideOff, value =>
            {
                if (value)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        HideStory.Begin();
                    });
                }
            });
        }
        #endregion

        #region 滚动到底部
        /// <summary>
        /// 滚动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnScrollDownClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SVChatBubbleList.ScrollToBottom();
            var temp = FindResource("OnScrollChangedDown") as Storyboard;
            temp.Begin();
        }
        #endregion

        private void SVChatBubbleList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var storyboard = new Storyboard();
            if (e.VerticalChange > 0)//如果滚动距离大于0为向上滚动
            {
                if (!IsInviteFloatButtonHide)
                {
                    storyboard = FindResource("") as Storyboard;
                    IsInviteFloatButtonHide = true;
                    storyboard.Begin();
                }
            }
            else if (e.VerticalChange < 0)//否则为向下滚动
            {
                if (IsInviteFloatButtonHide)
                {
                    IsInviteFloatButtonHide = false;
                    storyboard.Begin();
                }
            }
        }

    }
}
