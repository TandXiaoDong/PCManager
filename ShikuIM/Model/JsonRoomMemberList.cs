using System.Collections.Generic;

namespace ShikuIM.Model
{
    /// <summary>
    /// 群组成员列表
    /// </summary>
    public class JsonRoomMemberList : JsonBase
    {

        #region Constructor
        public JsonRoomMemberList()
        {
            data = new List<DataofMember>();
        }
        #endregion

        public List<DataofMember> data { get; set; }

    }
}
