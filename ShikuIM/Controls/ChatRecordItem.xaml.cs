using ShikuIM.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ShikuIM.UserControls
{
    /// <summary>
    /// ChatRecordItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatRecordItem : UserControl
    {
        public ChatRecordItem()
        {
            InitializeComponent();
        }

        public Messageobject Message
        {
            get { return (Messageobject)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(Messageobject), typeof(ChatRecordItem));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Message;
            rtb.Document.Blocks.Add(NewConvertEmoji(Message.content));
        }
        #region 替换emoji表情新
        /// <summary>
        /// 替换emoji的字符串
        /// </summary>
        /// <param name="emojiContent">需要转换的字符串</param>
        /// <returns>替换掉的结果</returns>
        private Paragraph NewConvertEmoji(string Content)
        {
            try
            {
                Paragraph parah = new Paragraph();
                //首先判断是否有一对中括号
                if (Content.Contains("[") && Content.Contains("]"))
                {
                    Regex EmojiRegex = new Regex(@"\[(\w|\-)*\]");
                    MatchCollection matchCollection = EmojiRegex.Matches(Content);
                    int jj = matchCollection.Count;
                    int j = 0;
                    foreach (Match match in matchCollection)
                    {
                        Dictionary<string,ImageSource> tmplist = new Dictionary<string, ImageSource>();
                        if (tmplist.ContainsKey(match.Value.TrimStart('[').TrimEnd(']')))
                        {
                            string t = Content.Substring(j, match.Index - j);
                            parah.Inlines.Add(new Run(t));
                            j = match.Index + match.Length;
                            var a = new Image
                            {
                                Source = tmplist[match.Value.TrimStart('[').TrimEnd(']')],
                                Width = 25,
                                Height = 25,
                                Tag = match.Value
                            };
                            parah.Inlines.Add(a);
                        }
                        else
                        {
                            parah.Inlines.Add(new Run(match.Value));
                        }

                    }
                    parah.Inlines.Add(new Run(Content.Substring(j, Content.Length - j)));
                }
                else
                {
                    parah.Inlines.Add(new Run(Content));
                }
                return parah;
            }
            catch (Exception ex)
            {
                LogHelper.log.Error(ex.Message, ex);
                ConsoleLog.Output(ex.Message);
                return null;
            }
        }
        #endregion
    }
}
