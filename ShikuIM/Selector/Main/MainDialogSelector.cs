using ShikuIM.Model;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM.Selector
{

    /// <summary>
    /// 主窗口对话框模板选择器
    /// </summary>
    public class MainDialogSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var main = App.Current.MainWindow;
            if (item is Messageobject)
            {
                return (DataTemplate)main.FindResource("ForwardMessageCard");//返回发送联系人卡片
            }
            else
            {
                return (DataTemplate)main.FindResource("SendContactCard");//返回发送联系人卡片
            }

            //return base.SelectTemplate(item, container);
        }

    }
}
