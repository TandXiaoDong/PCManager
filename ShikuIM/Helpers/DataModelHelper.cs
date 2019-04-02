using ShikuIM.Model;

namespace ShikuIM
{
    public static class DataModelHelper
    {
        #region DataOfFriends 转 MessageListItem
        /// <summary>
        /// <see cref="DataOfFriends"/>转<see cref="MessageListItem"/>
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        public static MessageListItem ToMsgListItem(this DataOfFriends friend)
        {
            var tmpitem = new MessageListItem();
            try
            {
                tmpitem = new MessageListItem()//实例化一个消息项
                {
                    MessageTitle = friend.toNickname,
                    Jid = friend.toUserId,//设置UserId
                    //MessageItemType = ItemType.User,//好友项
                    ShowTitle = friend.remarkName,//备注名
                    Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(friend.toUserId),//获取头像
                };
            }
            catch (System.Exception ex)
            {
                ConsoleLog.Output("ToMsgListItem=-" + ex.Message);
            }
            return tmpitem;
        }
        #endregion


        #region DataOfUserDetial 转 DataOfFriends
        /// <summary>
        /// <see cref="DataOfUserDetial"/>转<see cref="DataOfFriends"/>
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        public static DataOfFriends ToDataOfFriend(this DataOfUserDetial friend)
        {
            return new DataOfFriends()//实例化DataOfFriend
            {
                active = friend.active,
                areaCode = friend.areaCode,
                areaId = friend.areaId,
                attCount = friend.attCount,
                balance = friend.balance,
                birthday = friend.birthday,
                cityId = friend.cityId,
                companyId = friend.companyId,
                countryId = friend.countryId,
                createTime = friend.createTime,
                description = friend.description,
                fansCount = friend.fansCount,
                friendsCount = friend.friendsCount,
                idcard = friend.idcard,
                idcardUrl = friend.idcardUrl,
                isAuth = friend.isAuth,
                level = friend.level,
                modifyTime = friend.modifyTime,
                name = friend.name,
                nickname = friend.nickname,
                num = friend.num,
                offlineNoPushMsg = friend.offlineNoPushMsg,
                onlinestate = friend.onlinestate,
                password = friend.password,
                phone = friend.phone,
                provinceId = friend.provinceId,
                sex = friend.sex,
                status = friend.status,
                telephone = friend.Telephone,
                totalConsume = friend.totalConsume,
                totalRecharge = friend.totalRecharge,
                toUserId = friend.userId,
                toNickname = friend.nickname,
                userKey = friend.userKey,
                userType = friend.userType,
                vip = friend.vip,
                avatarName = friend.avatarName,
            };
        }
        #endregion



        #region Room转MessageListItem
        /// <summary>
        /// Room转MessageListItem
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static MessageListItem ToMsgItem(this Room room)
        {
            MessageListItem item = new MessageListItem()
            {
                Id = room.id,
                ShowTitle = room.name,
                MessageTitle = room.name,
                MessageItemType = ItemType.Group,
                Jid = room.jid,
                Avator = Applicate.LocalConfigData.GetDisplayAvatorPath(room.jid)
            };
            return item;
        }
        #endregion

    }
}
