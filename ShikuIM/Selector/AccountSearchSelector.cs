using ShikuIM.Model;
using System.Windows;
using System.Windows.Controls;

namespace ShikuIM.Selector
{
    public class AccountSearchSelector : DataTemplateSelector
    {

        #region 模板选择器
        /// <summary>
        /// 模板选择器
        /// </summary>
        /// <param name="item">源数据</param>
        /// <param name="container"></param>
        /// <returns>需要的控件</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tmp = new DataTemplate();
            var tmpmodel = (DataOfRtnNerbyuser)item;
            if (new DataOfFriends().ExistsFriend(tmpmodel.userId))//如果好友存在
            {
                tmp = ((ContentPresenter)container).FindResource("FriendTemplate") as DataTemplate;//不允许添加好友,,允许发消息
            }
            else//不存在(一种为自己,一种为陌生人)
            {
                if (tmpmodel.userId == Applicate.MyAccount.userId)
                    tmp = ((ContentPresenter)container).FindResource("CurrentAccountTemplate") as DataTemplate;//
                else
                    tmp = ((ContentPresenter)container).FindResource("AccountTemplate") as DataTemplate;//允许添加好友

            }
            return tmp;
        }
        #endregion

    }
}
