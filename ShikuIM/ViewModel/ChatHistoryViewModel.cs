using GalaSoft.MvvmLight;
using ShikuIM.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShikuIM.ViewModel
{
    public class ChatHistoryViewModel : ViewModelBase
    {
        Messageobject msgManager;
        string thisJid;

        ObservableCollection<Messageobject> _msgList;
        public ObservableCollection<Messageobject> msgList
        {
            get
            {
                return _msgList;
            }
            set
            {
                _msgList = value;
                RaisePropertyChanged(nameof(msgList));
            }
        }

        string keywords;
        public string keyWords
        {
            get
            {
                return keywords;
            }
            set
            {
                keywords = value;
                RaisePropertyChanged(nameof(keyWords));
                showChatRecord();
            }
        }
        public ChatHistoryViewModel()
        {
            //InitialHistory();
        }

        #region MyRegion

        public void InitialHistory(string jid)
        {
            msgManager = new Messageobject()
            {
                fromUserId = Applicate.MyAccount.userId,
                FromId = Applicate.MyAccount.userId,
                toUserId = jid,
                ToId = jid,
            };
            this.thisJid = jid;
            keyWords = "";
            msgList = new ObservableCollection<Messageobject>();
            if (IsInDesignMode)
            {
                return;
            }
            showChatRecord();
        }
        #endregion

        private void showChatRecord()
        {
            string keywords = keyWords.TrimStart().TrimEnd();
            var list = GetChatRecord(keywords);
            if (list != null)
            {
                msgList = new ObservableCollection<Messageobject>(GetChatRecord(keywords));
            }
        }

        #region 获取聊天记录
        private List<Messageobject> GetChatRecord(string keyword = "")
        {
            //所有类型
            int type = -1;
            int page = 0;
            return msgManager.GetList(type, ref page, 0, keyword);
        }
        #endregion
    }
}
