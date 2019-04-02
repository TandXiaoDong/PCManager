using GalaSoft.MvvmLight.Messaging;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ShikuIM
{
    /// <summary>
    /// 可绑定的富文本框
    /// </summary>
    public partial class BindableRichText : UserControl
    {

        #region Dependency stuff
        /// <summary>
        /// 富文本框内文本
        /// </summary>
        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public TextSelection Selection
        {
            get { return (TextSelection)GetValue(SelectionProperty); }
            //set { SetValue(SelectionProperty, value); }
        }



        #endregion

        #region Dependency Static Property
        /// <summary>
        /// Selection依赖属性
        /// </summary>
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register(nameof(Selection),
                typeof(TextSelection),
                typeof(BindableRichText),
                new PropertyMetadata(OnSelectionChanged));


        /// <summary>
        /// Document依赖属性
        /// </summary> 
        public static readonly DependencyProperty DocumentProperty =
                DependencyProperty.Register(nameof(Document),
                    typeof(FlowDocument),
                    typeof(BindableRichText),
                    new PropertyMetadata(OnDocumentChanged));
        #endregion

        #region Dependency Property Changed

        /// <summary>
        /// Selection属性改变时
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BindableRichText)d;
            if (e.NewValue == null)
            {
                //if no new value then do nothing
            }
            else
            {
                //control.ThisRichText.Selection = (TextSelection)e.NewValue;//Set the new value,,but the Selection is ead only
            }
        }

        /// <summary>
        /// Document属性改变时
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableRichText control = (BindableRichText)d;
            if (e.NewValue == null)//如果为空则显示
            {
                control.ThisRichText.Document = new FlowDocument(); //Document is not amused by null :)
                control.FontFamily = new System.Windows.Media.FontFamily("微软雅黑");
                control.Document.Foreground = new SolidColorBrush(Colors.Black);
                control.Document.LineHeight = 2;
            }
            else
            {
                var document = (FlowDocument)e.NewValue;
                control.ThisRichText.Document = document;
                control.FontFamily = new System.Windows.Media.FontFamily("微软雅黑");
                control.Document.Foreground = new SolidColorBrush(Colors.Black);
                control.Document.LineHeight = 2;
            }
        }
        #endregion

        #region Contrustor
        public BindableRichText()
        {
            InitializeComponent();

            //ThisRichText.Selection
        }
        #endregion

        #region 文本变化时
        /// <summary>
        /// 文本变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisRichText_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
            var data = Clipboard.GetDataObject();//获取剪贴板内容
            var imgdata = data.GetDataPresent(typeof(Bitmap)) ? data.GetData(typeof(Bitmap)) : null;
            //if (data.GetDataPresent(typeof(Bitmap)))
            //{
            //    = data.GetData(typeof(Bitmap));
            //}
            if (imgdata is System.Drawing.Image)
            {
                var txtbox = (RichTextBox)e.Source;
                var res = txtbox.Undo();//撤销粘贴的图片
            }*/

            Messenger.Default.Send(true, MainViewNotifactions.InputTextChanged);//
        }
        #endregion

    }
}
