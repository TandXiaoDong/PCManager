using ShikuIM.Model;
using ShikuIM.UserControls;
using ShikuIM.ViewModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ShikuIM
{
    /// <summary>
    /// ChatRecord.xaml 的交互逻辑
    /// </summary>
    public partial class ChatRecord : Window
    {
        Messageobject msgManager;
        string thisJid;
        int pageNum = 1;
        int pageSize = 0;
        public ChatRecord(string jid)
        {
            msgManager = new Messageobject() {  fromUserId = Applicate.MyAccount.userId, toUserId = jid };
            this.thisJid = jid;
            InitializeComponent();
            var histmodel = new ChatHistoryViewModel();
            histmodel.InitialHistory(jid);//初始化
            this.DataContext = histmodel;

        }

        /// <summary>
        /// 需实现单例的对象
        /// </summary>
        private static ChatRecord thisChatRecord { get; set; }

        #region 单例方法
        public static ChatRecord ShowChatRecord(string jid)
        {
            if (thisChatRecord == null)
            {
                thisChatRecord = new ChatRecord(jid);
            }
            else
            {
                if (thisChatRecord.thisJid == null || thisChatRecord.thisJid != jid)
                {
                    thisChatRecord.Close();
                    thisChatRecord = new ChatRecord(jid);
                }
            }
            return thisChatRecord;
        }
        #endregion

        #region 窗口随意拖动
        /// <summary>
        /// 窗口随意拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch (Exception ex)
                {
                    LogHelper.log.Error("-—窗口拖动时错误" + ex.Message, ex);
                    ConsoleLog.Output(ex.Message);
                }
            }
        }//窗口随意拖动
        #endregion
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pageNum = int.MaxValue;
            searchRecord();
        }
        private void searchRecord()
        {
            //ComboBoxItem lbi = (cb_pageSize.SelectedItem as ComboBoxItem);
            //if (lbi.Content != null)
            //{
            //    int.TryParse(lbi.Content.ToString(), out this.pageSize);
            //}
            //switch (this.cb_type.SelectedIndex)
            //{
            //    case 0:
            //        showChatRecord();
            //        break;
            //    case 1:
            //        showImageRecord();
            //        break;
            //    case 2:
            //        showFileRecord();
            //        break;
            //    default:
            //        break;
            //}
        }

        private void showChatRecord()
        {
            //this.scr.Content = null;
            //ListBox lb = new ListBox();
            //string keywords = this.tb_keyword.Text.TrimStart().TrimEnd();
            //var chatList = getChatRecord(keywords);
            //foreach (var msg in chatList)
            //{
            //    RichTextBox rtb = new RichTextBox() { IsReadOnly = true, BorderThickness = new Thickness(0), Margin = new Thickness(0) };
            //    rtb.Document.Blocks.Add(NewConvertEmoji(msg.fromUserName + '\t' + Utils.StampToDatetime(msg.timeSend)));
            //    rtb.Document.Blocks.Add(NewConvertEmoji(msg.content));
            //    lb.Items.Insert(0, rtb);
            //    //lb.Items.Add(rtb);
            //}
            //this.scr.Content = lb;
            //scr.ScrollToEnd();
        }
        private List<Messageobject> getChatRecord(string keyword = "")
        {
            //文字
            int pageSize = this.pageSize;
            int type = 1;
            var ArryDate = getDateList().ToArray();
            return msgManager.GetList(type, ref pageNum, pageSize, keyword);
        }
        private void showImageRecord()
        {
            //图片
            //this.scr.Content = null;
            //var imageList = getImageRecord();
            //WrapPanel wp = new WrapPanel();
            //foreach (var msg in imageList)
            //{
            //    BitmapImage image = FileUtil.ReadFileByteToBitmap(msg.filePath);
            //    Image img = new Image() { Source = image, Width = 150, Height = 150, Margin = new Thickness(2), Cursor = Cursors.Hand };
            //    img.MouseLeftButtonDown += new MouseButtonEventHandler((s, e) => System.Diagnostics.Process.Start(msg.filePath));
            //    wp.Children.Add(img);
            //}
            //this.scr.Content = wp;
        }

        private List<Messageobject> getImageRecord()
        {
            //图片
            int pageSize = this.pageSize;
            int type = 2;
            var ArryDate = getDateList().ToArray();
            return msgManager.GetList(type, ref pageNum, pageSize, "");
        }
        private void showFileRecord()
        {
            //文件
            //this.scr.Content = null;
            //ObservableCollection<Messageobject> imageList =new ObservableCollection<Messageobject>(getFileRecord());
            //DATA_GRID.DataContext = imageList;
            //this.scr.Content = DATA_GRID;
        }
        private List<Messageobject> getFileRecord()
        {
            //文件
            int pageSize = this.pageSize;
            int type = 9;

            string keywords = this.tb_keyword.Text;
            var ArryDate = getDateList().ToArray();
            return msgManager.GetList(type, ref pageNum, pageSize, keywords);
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
                        Dictionary<string, ImageSource> tmplist = new Dictionary<string, ImageSource>();
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

        private void btn_date_Click(object sender, RoutedEventArgs e)
        {
            //this.dlg_Edit.IsOpen = true;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            //取消按钮
            //this.cld_date.SelectedDates.Clear();
            //this.dlg_Edit.IsOpen = false;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //确认按钮
            //this.dlg_Edit.IsOpen = false;
        }
        private List<long> getDateList()
        {
            List<DateTime> dateList = new List<DateTime>();
            //foreach (var date in cld_date.SelectedDates.ToList())
            //{
            //    dateList.Add(cld_date.SelectedDates.ToList().First());
            //    dateList.Add(cld_date.SelectedDates.ToList().Last().AddDays(1));
            //};
            return dateList.ConvertAll(x => Helpers.DatetimeToStamp(x));
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            //上一页
            pageNum--;
            searchRecord();

        }

        private void btn_first_Clike(object sender, RoutedEventArgs e)
        {
            //最后一页
            pageNum = int.MaxValue;
            searchRecord();

        }

        private void btn_last_Click(object sender, RoutedEventArgs e)
        {
            //下一页
            pageNum++;
            searchRecord();
        }

        private void btn_previous_Click(object sender, RoutedEventArgs e)
        {
            //首页
            pageNum = 1;
            searchRecord();
        }

        private void cb_pageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchRecord();
        }

        private void btn_Download_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGridHyperlinkColumn_Click(object sender, RoutedEventArgs e)
        {
            //if (DATA_GRID.SelectedItem == null)
            //{
            //    return;
            //}
            //string filePath = (DATA_GRID.SelectedItem as Messageobject).filePath;
            //if (System.IO.File.Exists(filePath))
            //{
            //    Process.Start(filePath); //打开文件
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thisChatRecord.thisJid = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 添加右上角按钮
            FormOperation operation = new FormOperation(this, false);
            operation.VerticalAlignment = VerticalAlignment.Top;
            operation.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumnSpan(operation, 3);
            gd_body.Children.Add(operation);

            #endregion
        }

        private void tb_keyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            scr.ScrollToEnd();
        }
    }
}
