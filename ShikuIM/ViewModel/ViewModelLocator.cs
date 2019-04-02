/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ShikuIM" x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
//using Microsoft.Practices.ServiceLocation;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<GroupCreateViewModel>();
            SimpleIoc.Default.Register<GroupDetialViewModel>();
            SimpleIoc.Default.Register<UserVerifyListViewModel>();
            SimpleIoc.Default.Register<AccountQueryViewModel>();
            SimpleIoc.Default.Register<LoginAndRegisterViewModel>();
            SimpleIoc.Default.Register<ChatHistoryViewModel>();
            SimpleIoc.Default.Register<RoomVerifyViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<ChatBubbleListViewModel>();
            SimpleIoc.Default.Register<UserDetailViewModel>();
            SimpleIoc.Default.Register<MyDetialViewModel>();
            //SimpleIoc.Default.Register<MyDetialViewModel>();
        }

        #region Declare ViewModel Properties

        /// <summary>
        /// ������ViewModel
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// ��ȺViewModel
        /// </summary>
        public GroupCreateViewModel GroupCreate => ServiceLocator.Current.GetInstance<GroupCreateViewModel>();

        /// <summary>
        /// Ⱥ����ViewModel
        /// </summary>
        public GroupDetialViewModel GroupDetail => ServiceLocator.Current.GetInstance<GroupDetialViewModel>();

        /// <summary>
        /// ������֤ViewModel
        /// </summary>
        public UserVerifyListViewModel UserVerify => ServiceLocator.Current.GetInstance<UserVerifyListViewModel>();

        /// <summary>
        /// ���Һ���
        /// </summary>
        public AccountQueryViewModel AccountQuery => ServiceLocator.Current.GetInstance<AccountQueryViewModel>();

        /// <summary>
        /// Ⱥ����
        /// </summary>
        public GroupShareViewModel GroupShare => ServiceLocator.Current.GetInstance<GroupShareViewModel>();

        /// <summary>
        /// ��½ע��
        /// </summary>
        public LoginAndRegisterViewModel LoginAndRegister => ServiceLocator.Current.GetInstance<LoginAndRegisterViewModel>();

        /// <summary>
        /// �����¼
        /// </summary>
        public ChatHistoryViewModel AccountSearch => ServiceLocator.Current.GetInstance<ChatHistoryViewModel>();

        /// <summary>
        /// ����
        /// </summary>
        public SettingViewModel Settings => ServiceLocator.Current.GetInstance<SettingViewModel>();

        /// <summary>
        /// �����б�
        /// </summary>
        public ChatBubbleListViewModel ChatBubbleList => ServiceLocator.Current.GetInstance<ChatBubbleListViewModel>();

        /// <summary>
        /// ��������
        /// </summary>
        public UserDetailViewModel UserDetial => ServiceLocator.Current.GetInstance<UserDetailViewModel>();

        /// <summary>
        /// ��������
        /// </summary>
        public MyDetialViewModel MyDetial => ServiceLocator.Current.GetInstance<MyDetialViewModel>();
        #endregion


        #region Clear the ViewModels

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
        #endregion

    }
}