using CommonServiceLocator;
using ShikuIM.ViewModel;
using System.Windows.Controls;

namespace ShikuIM.View
{
    /// <summary>
    /// 好友验证列表
    /// </summary>
    public partial class VerifyUserListControl : UserControl
    {

        #region Consturctor
        /// <summary>
        /// Constuctor
        /// </summary>
        public VerifyUserListControl()
        {
            InitializeComponent();
            ///Set the instance of <see cref="UserVerifyListViewModel"/> to this DataContext
            this.DataContext = ServiceLocator.Current.GetInstance<UserVerifyListViewModel>();
        }
        #endregion


    }
}
