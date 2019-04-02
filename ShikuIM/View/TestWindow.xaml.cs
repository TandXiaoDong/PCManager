using GalaSoft.MvvmLight.Command;
using ShikuIM.Model;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ShikuIM
{
    /// <summary>
    /// XmppTest.xaml 的交互逻辑
    /// </summary>
    public partial class TestingWindow : Window, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        private double videoProgress;

        /// <summary>
        /// 视频播放进度
        /// </summary>
        public double VideoProgress
        {
            get { return videoProgress; }
            set { videoProgress = value; RaisePropertyChanged(nameof(VideoProgress)); }
        }

        public ICommand ProgressChange
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //VlcPlayer.SourceProvider.MediaPlayer.Position = Convert.ToSingle(VideoProgress);
                    ConsoleLog.Output(VideoProgress);
                    //VlcPlayer.SourceProvider.MediaPlayer.();
                    //ConsoleLog.Output(VlcPlayer.SourceProvider.MediaPlayer.IsPlaying());
                });
            }
        }

        public TestingWindow()
        {
            InitializeComponent();
            this.DataContext = this;//设置绑定
            //VlcPlayer.SourceProvider.MediaPlayer.Play();
            //VlcPlayer.SourceProvider = new VlcVideoSourceProvider(App.Current.Dispatcher);
            //VlcControl
        }

        #region 使用多线程加载头像
        /// <summary>
        /// 测试使用方法--多线程加载头像
        /// </summary>
        /// <param name="userId">当前用户UserId</param>
        private void LoadImageByAwait(string userId)
        {
            //tmpimg = await ShiKuManager.GetHeadImgSync(userId);

        }
        #endregion

        /// <summary>
        /// 把 Emoji编码 [e]1f1e6-1f1fa[/e]
        ///     [e]1f1e6[/e]
        ///     [e]1f1fa[/e]
        ///     [e]1f1e6[/e]
        ///     [e]1f1f9[/e] 换成对应的字符串,此字符串能被window8.1,ios,andriod 直接显示.
        ///     
        /// 如果在网页中显示,需要设置字体为 'Segoe UI Emoji' 如下.当然客户机还必须得有这个字体.
        /// 
        ///     <span style="font-family:'Segoe UI Emoji';"></span>
        ///     
        /// </summary>
        /// <param name="paramContent"></param>
        /// <returns></returns>
        public string GetEmoji(string paramContent)
        {
            string paramContentR = paramContent.Replace("[e]", "\\u").Replace("[/e]", "");
            var unicodehex = new char[6] { '0', '0', '0', '0', '0', '0' };
            StringBuilder newString = new StringBuilder(2000);
            StringBuilder tempEmojiSB = new StringBuilder(20);
            StringBuilder tmps = new StringBuilder(5);
            int ln = paramContent.Length;
            for (int index = 0; index < ln; index++)
            {
                int i = index; //把指针给一个临时变量,方便出错时,现场恢复.
                try
                {
                    if (paramContent[i] == '[')
                    {
                        //预测
                        if (paramContent[i + 1] == 'e')
                        {
                            if (paramContent[i + 2] == ']') //[e]的后面4位是 unicode 的16进制数值.
                            {
                                i = i + 3; //前进3位 
                                i = ChangUnicodeToUTF16(paramContent, tempEmojiSB, tmps, i);
                                if (paramContent[i] == '-')//向前探测1位 看看是否双字符 形如1f1e7-1f1ea 
                                {
                                    i++;
                                    i = ChangUnicodeToUTF16(paramContent, tempEmojiSB, tmps, i);
                                };
                                if (paramContent[i] == '[')
                                {
                                    if (paramContent[i + 1] == '/')
                                    {
                                        if (paramContent[i + 2] == 'e')
                                        {
                                            if (paramContent[i + 3] == ']')
                                            {
                                                i = i + 3; //再前进4位
                                                index = i;//识别转换成功
                                                newString.Append(tempEmojiSB.ToString());   //识别转换成功
                                                tempEmojiSB.Clear();
                                                continue;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                    index = i;

                }
                catch (Exception ex)
                {
                    ConsoleLog.Output(ex);
                    //解析失败仍然继续
                }
                newString.Append(paramContent[index]);
            }
            return newString.ToString();
        }

        #region 转Unicode为UTF16
        /// <summary>
        /// 转Unicode为UTF16
        /// </summary>
        /// <param name="paramContent"></param>
        /// <param name="tempSB"></param>
        /// <param name="tmps"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public int ChangUnicodeToUTF16(string paramContent, StringBuilder tempSB, StringBuilder tmps, int i)
        {
            for (int maxln = 0; maxln < 20; maxln++)
            {
                if (paramContent[i] != '-' && paramContent[i] != '[')
                {  //向前探测1位
                    tmps.Append(paramContent[i]);
                    i++;
                }
                else
                {
                    break;
                }
            }
            tempSB.Append(EmojiCodeToUTF16String(tmps.ToString()));
            tmps.Clear();
            return i;
        }
        #endregion

        #region 字符串形式的 Emoji 16进制Unicode编码  转换成 UTF16字符串 能够直接输出到客户端
        /// <summary>
        /// 字符串形式的 Emoji 16进制Unicode编码  转换成 UTF16字符串 能够直接输出到客户端
        /// </summary>
        /// <param name="EmojiCode"></param>
        /// <returns></returns>
        public string EmojiCodeToUTF16String(string EmojiCode)
        {
            if (EmojiCode.Length != 4 && EmojiCode.Length != 5)
            {
                throw new ArgumentException("错误的 EmojiCode 16进制数据长度.一般为4位或5位");
            }
            //1f604
            int EmojiUnicodeHex = int.Parse(EmojiCode, System.Globalization.NumberStyles.HexNumber);

            //1f604对应 utf16 编码的int
            Int32 EmojiUTF16Hex = EmojiToUTF16(EmojiUnicodeHex, true);             //这里字符的低位在前.高位在后.
            //Response.Write(Convert.ToString(lon, 16)); Response.Write("<br/>"); //这里字符的低位在前.高位在后. 
            var emojiBytes = BitConverter.GetBytes(EmojiUTF16Hex);                     //把整型值变成Byte[]形式. Int64的话 丢掉高位的空白0000000   
            return Encoding.Unicode.GetString(emojiBytes);
        }
        #endregion

        #region EmoJi U+字符串对应的 int 值 转换成UTF16字符编码的值
        /// <summary>
        /// EmoJi U+字符串对应的 int 值 转换成UTF16字符编码的值
        /// </summary>
        /// <param name="V">EmojiU+1F604 转成计算机整形以后的值V=0x1F604 </param>
        /// <param name="LowHeight">低字节在前的顺序.(默认)</param>
        /// <remarks>
        ///参考  
        ///http://blog.csdn.net/fengsh998/article/details/8668002
        ///http://punchdrunker.github.io/iOSEmoji/table_html/index.html
        /// V  = 0x64321
        /// Vx = V - 0x10000
        ///    = 0x54321
        ///    = 0101 0100 0011 0010 0001
        ///
        /// Vh = 01 0101 0000 // Vx 的高位部份的 10 bits
        /// Vl = 11 0010 0001 // Vx 的低位部份的 10 bits
        /// wh = 0xD800 //結果的前16位元初始值
        /// wl = 0xDC00 //結果的後16位元初始值
        ///
        /// wh = wh | Vh
        ///    = 1101 1000 0000 0000
        ///    |        01 0101 0000
        ///    = 1101 1001 0101 0000
        ///    = 0xD950
        ///
        /// wl = wl | Vl
        ///    = 1101 1100 0000 0000
        ///    |        11 0010 0001
        ///    = 1101 1111 0010 0001
        ///    = 0xDF21
        /// </remarks>
        /// <returns>EMOJI字符对应的UTF16编码16进制整形表示</returns>
        public Int32 EmojiToUTF16(Int32 V, bool LowHeight = true)
        {
            Int32 Vx = V - 0x10000;
            Int32 Vh = Vx >> 10;//取高10位部分
            Int32 Vl = Vx & 0x3ff; //取低10位部分
            //Response.Write("Vh:"); Response.Write(Convert.ToString(Vh, 2)); Response.Write("<br/>"); //2进制显示
            //Response.Write("Vl:"); Response.Write(Convert.ToString(Vl, 2)); Response.Write("<br/>"); //2进制显示
            Int32 wh = 0xD800; //結果的前16位元初始值,这个地方应该是根据Unicode的编码规则总结出来的数值.
            Int32 wl = 0xDC00; //結果的後16位元初始值,这个地方应该是根据Unicode的编码规则总结出来的数值.
            wh = wh | Vh;
            wl = wl | Vl;
            //Response.Write("wh:"); Response.Write(Convert.ToString(wh, 2)); Response.Write("<br/>");//2进制显示
            //Response.Write("wl:"); Response.Write(Convert.ToString(wl, 2)); Response.Write("<br/>");//2进制显示
            if (LowHeight)
            {
                return wl << 16 | wh;   //低位左移16位以后再把高位合并起来 得到的结果是UTF16的编码值   //适合低位在前的操作系统 
            }
            else
            {
                return wh << 16 | wl; //高位左移16位以后再把低位合并起来 得到的结果是UTF16的编码值   //适合高位在前的操作系统
            }
        }
        #endregion

        private void Button_Clicktemp(object sender, RoutedEventArgs e)
        {
            var friend = new DataOfFriends();
            var temp = friend.GetByPage(0);
            friend.GetByPage(1);
        }
    }
}
