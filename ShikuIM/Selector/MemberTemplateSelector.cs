using ShikuIM.Model;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM.Selector
{

    /// <summary>
    /// 群成员预览模板项选择器
    /// </summary>
    public class MemberTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// 群成员预览模板项选择器
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = null;
            if (item is DataofMember)//如果项是Member对象才使用
            {
                var member = (DataofMember)item;
                var groupwindow = Applicate.GetWindow<GroupChatDetial>();//获取窗口
                switch (member.role)
                {
                    case MemberRole.Owner:
                        template = groupwindow.FindResource("NormalMemberTemplate") as DataTemplate;
                        break;
                    case MemberRole.Admin:
                        template = groupwindow.FindResource("NormalMemberTemplate") as DataTemplate;
                        break;
                    case MemberRole.Member:
                        template = groupwindow.FindResource("NormalMemberTemplate") as DataTemplate;
                        break;
                    case MemberRole.PlusIcon:
                        template = groupwindow.FindResource("PlusItemTemplate") as DataTemplate;
                        break;
                    case MemberRole.KickIcon:
                        template = groupwindow.FindResource("MinusItemTemplate") as DataTemplate;
                        break;
                    default:
                        template = groupwindow.FindResource("NormalMemberTemplate") as DataTemplate;
                        break;
                }
                return template;
            }
            return template;
        }
    }
}
