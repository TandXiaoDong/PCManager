using GalaSoft.MvvmLight;

namespace ShikuIM.Model
{

    /// <summary>
    /// 
    /// </summary>
    public class ServerModel : ObservableObject
    {

        private string url;
        private string port;

        #region Public Member
        /// <summary>
        /// Url
        /// </summary>
        public string Url
        {
            get { return url; }
            set
            {
                if (url == value)
                    return;
                url = value; RaisePropertyChanged(nameof(Url));
            }
        }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port
        {
            get { return port; }
            set
            {
                if (port == value)
                    return;
                port = value; RaisePropertyChanged(nameof(Port));
            }
        }
        #endregion


    }
}
