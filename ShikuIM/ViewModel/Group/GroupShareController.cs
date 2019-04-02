using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShikuIM.View;

namespace ShikuIM
{
    public static class GroupShareController
    {
        static Dictionary<string, GroupShare> ShareDic = new Dictionary<string, GroupShare>();
        public static void ShowShareForm(string roomId)
        {
            if (!ShareDic.ContainsKey(roomId))
            {
                var groupShare = new GroupShare(roomId);
                groupShare.delShareForm += GroupShare_delShareForm;
                ShareDic.Add(roomId, groupShare);
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                ShareDic[roomId].Show();
                ShareDic[roomId].Activate();
            });

        }

        private static void GroupShare_delShareForm(string roomId)
        {
            if (ShareDic.ContainsKey(roomId))
                ShareDic.Remove(roomId);
        }
        
    }
}
