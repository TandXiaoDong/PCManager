using CommonServiceLocator;
using ShikuIM.ViewModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace ShikuIM
{

    /// <summary>
    /// Emoji文本
    /// </summary>
    public partial class EmojiTextBlock : UserControl
    {

        #region public static members

        /// <summary>
        /// 文本剪裁方式依赖属性
        /// </summary>
        public static readonly DependencyProperty TextTrimProperty =
            DependencyProperty.Register(nameof(TextTrim), typeof(TextTrimming), typeof(EmojiTextBlock), new PropertyMetadata(OnTextTrimChanged));
        #endregion

        /// <summary>
        /// 文本消息
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(EmojiTextBlock), new PropertyMetadata(OnTextChanged));


        #region public member
        /// <summary>
        /// 文本剪裁方式
        /// </summary>
        public TextTrimming TextTrim
        {
            get { return (TextTrimming)GetValue(TextTrimProperty); }
            set { SetValue(TextTrimProperty, value); }
        }

        #endregion

        #region Events
        private static void OnTextTrimChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EmojiTextBlock)d;
            control.ContentBlock.TextTrimming = (TextTrimming)e.NewValue;
            //if (e.NewValue!=null)
            //{

            //}
        }


        #region 文本改变事件
        /// <summary>
        /// 文本改变事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                string text = e.NewValue as string;
                EmojiTextBlock emojitxt = (EmojiTextBlock)d;
                emojitxt.Dispatcher.Invoke(() => { emojitxt.ContentBlock.Text = ""; });
                Task.Run(() =>
                {
                    try
                    {
                        //首先判断是否有一对中括号
                        if (text.Contains("[") && text.Contains("]"))
                        {
                            Regex regex = new Regex(@"\[(\w|\-)*\]");
                            MatchCollection matchCollection = regex.Matches(text);
                            int j = 0;
                            var emojilist = ServiceLocator.Current.GetInstance<MainViewModel>().EmojiEmotionList;
                            for (int i = 0; i < matchCollection.Count; i++)
                            {
                                string tmp = matchCollection[i].Value.TrimStart('[').TrimEnd(']');
                                var tp = "\\" + tmp + ".png";
                                string cont = emojilist.FirstOrDefault(l => l.Contains(tp));
                                if (cont != null)
                                {
                                    string t = text.Substring(j, matchCollection[i].Index - j);
                                    emojitxt.Dispatcher.Invoke(() =>
                                    {
                                        emojitxt.ContentBlock.Inlines.Add(new Run(t));
                                        j = matchCollection[i].Index + matchCollection[i].Length;
                                        var a = new Image
                                        {
                                            Source = new BitmapImage(new Uri(cont)),
                                            Width = 20,
                                            Height = 20,
                                            Tag = matchCollection[i].Value
                                        };
                                        emojitxt.ContentBlock.Inlines.Add(a);
                                    });
                                }
                                else
                                {
                                    //emojitxt.Dispatcher.Invoke(() =>
                                    //{
                                    //    //emojitxt.ContentBlock.Inlines.Add(new Run(matchCollection[i].Value));
                                    //});
                                }
                            }
                            emojitxt.Dispatcher.Invoke(() =>
                            {
                                emojitxt.ContentBlock.Inlines.Add(new Run(text.Substring(j, text.Length - j)));
                            });
                        }
                        else
                        {
                            emojitxt.Dispatcher.Invoke(() =>
                            {
                                emojitxt.ContentBlock.Inlines.Add(new Run(text));
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Emoji Error--" + ex.Message);
                    }
                });

            }
        }
        #endregion

        #endregion

        #region Contructor
        public EmojiTextBlock()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载文本信息
        public static void LoadTextWithEmoji(string message)
        {
        }
        #endregion

    }
}
