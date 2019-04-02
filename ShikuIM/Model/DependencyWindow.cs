using System.ComponentModel;
using System.Windows;

namespace ShikuIM.Model
{

    /// <summary>
    /// 封装为依赖属性的窗口对象
    /// </summary>
    public class DependencyWindow : DependencyObject, INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private Window window;

        /// <summary>
        /// 需要进行操作的窗口
        /// </summary>
        public Window Window
        {
            get { return window; }
            set
            {
                window = value;
                OnPropertyChanged(nameof(Window));
            }
        }

    }
}
