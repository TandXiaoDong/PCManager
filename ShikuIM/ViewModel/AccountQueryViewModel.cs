using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{
    /// <summary>
    /// 好友查询
    /// </summary>
    public class AccountQueryViewModel : ViewModelBase
    {

        #region Private Members
        private string _searchtxt;
        private int pageNum = 0;
        private int pageSize = 10;
        private bool searchBtnEnable = true;
        private SnackbarMessageQueue snackbar;

        #endregion

        #region Public Members
        /// <summary>
        /// 提示消息
        /// </summary>
        public SnackbarMessageQueue SnackBar
        {
            get { return snackbar; }
            set { snackbar = value; RaisePropertyChanged(nameof(SnackBar)); }
        }

        /// <summary>
        /// 搜索按钮是否启用
        /// </summary>
        public bool SearchEnable
        {
            get { return searchBtnEnable; }
            set
            {
                searchBtnEnable = value;
                RaisePropertyChanged(nameof(SearchEnable));
            }
        }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchWords
        {
            get { return _searchtxt; }
            set
            {
                _searchtxt = value;
                RaisePropertyChanged(nameof(SearchWords));
            }
        }
        #endregion

        /// <summary>
        /// 查询出的用户列表
        /// </summary>
        public ObservableCollection<DataOfRtnNerbyuser> AccountList { get; set; } = new ObservableCollection<DataOfRtnNerbyuser>();

        /// <summary>
        /// 搜索Command
        /// </summary>
        public ICommand SearchCommand => new RelayCommand(() =>
        {
            pageNum = 0;
            ShowLoadingProgressBar(true);//禁用按钮并显示
            GetFriends(SearchWords);
        });

        /// <summary>
        /// 查看好友详情
        /// </summary>
        public ICommand AccountDetailCommand => new RelayCommand<string>((UserId) =>
        {
            if (UserId == Applicate.MyAccount.userId)
            {
                Personal.GetPersonal();
            }
            else
            {
                Messenger.Default.Send(UserId, UserDetailNotifications.ShowUserDetial);
                UserDetailView.GetWindow().Show();
            }
        });

        /// <summary>
        /// 显示首页Command
        /// </summary>
        public ICommand FirstPageCommand => new RelayCommand(() =>
        {
            if (pageNum == 0)
            {
                return;
            }

            pageNum = 0;
            GetFriends();
        });

        /// <summary>
        /// 上一页Command
        /// </summary>
        public ICommand LastPageCommand => new RelayCommand(() =>
        {
            pageNum--;
            if (pageNum < 0)
            {
                pageNum = 0;
                return;
            }
            GetFriends();
        });

        /// <summary>
        /// 下一页Command
        /// </summary>
        public ICommand NextPageCommand => new RelayCommand(() =>
        {
            pageNum++;
            GetFriends();
        });

        /// <summary>
        /// 最后一页Command
        /// </summary>
        public ICommand EndPageCommand => new RelayCommand(() =>
        {

        });

        /// <summary>
        /// 发消息Command
        /// </summary>
        public ICommand StartChatCommand => new RelayCommand<DataOfRtnNerbyuser>((UserTmp) =>
        {
            var mControl = ServiceLocator.Current.GetInstance<MainViewModel>();
            MessageListItem item = mControl.FriendList.FirstOrDefault(f => f.Jid == UserTmp.userId);
            mControl.MainTabSelectedIndex = 0;//转到消息界面
            mControl.StartNewChatFromItem(item.Clone());
            AccountQuery.CloseWindow();
        });


        /// <summary>
        /// 发消息Command
        /// </summary>
        public ICommand FriendRequestCommand => new RelayCommand<DataOfRtnNerbyuser>((UserTmp) =>
        {

            if (UserTmp != null && !String.IsNullOrWhiteSpace(UserTmp.userId))
            {
                var strange = new DataOfUserDetial()
                {
                    userId = UserTmp.userId,
                    nickname = UserTmp.nickname
                };
                var client = ShiKuManager.AddFriend(strange);
                client.Tag = strange;//设置Tag为临时用户
                client.UploadDataCompleted += (sen, res) =>
                {
                    if (res.Error == null)
                    {
                        var result = JsonConvert.DeserializeObject<JsonAttention>(Encoding.UTF8.GetString(res.Result));
                        var friend = ((HttpClient)sen).Tag as DataOfUserDetial;
                        if (result.data.type == 2 || result.data.type == 4)
                        {
                            if (friend != null)
                            {
                                ShiKuManager.SendFriendRequest(friend.userId, true);//发送508
                                var newFriend = friend.ConvertToDataFriend();//转为DataFriend
                                newFriend.AutoInsert();//插入数据库
                                newFriend.UpdateFriendState(newFriend.toUserId, 1);//我方单方关注
                                Messenger.Default.Send(newFriend.ToMsgListItem(), MainViewNotifactions.MainAddFriendListItem);
                            }
                            //允许发消息
                            UserTmp.IsSendedRequest = true;
                        }
                        else //if (data.resultCode > 1 && data.resultCode < 5)//返回代码在1和5之间
                        {
                            if (friend != null)
                            {
                                ShiKuManager.SendFriendRequest(friend.userId, false);//发送500
                                UserTmp.IsSendedRequest = true;
                            }
                        }
                    }
                    else
                    {
                        SnackBar.Enqueue("错误:" + res.Error.Message);
                    }
                };
            }
        });


        #region 添加好友按下
        /// <summary>
        /// 添加好友事件
        /// </summary>
        /// <param name="sender">控件</param>
        /// <param name="e">事件源</param>
        public void userAdd_MouseDown(object sender)
        {
            //var web = new WebClient();
            string userId = "";
            //非空判断
            if (userId != "")
            {
                //接口添加好友
                var client = APIHelper.AddFriendAsync(userId);
                client.UploadDataCompleted += (sen, res) =>
                {
                    var result = JsonConvert.DeserializeObject<JsonAttention>(Encoding.UTF8.GetString(res.Result));//获取返回值
                    //var user = ((HttpClient)sender).Tag as DataOfUserDetial;
                    if (result.resultCode == 2 || result.resultCode == 4)
                    {
                        ShiKuManager.SendFriendRequest(userId, true);//发送Xmpp
                    }
                };
            }
            else
            {
                //MessageBox.Show("用户ID无效或获取ID失败", "提示");
            }
        }
        #endregion

        private void btn_first_Clike(object sender)
        {
            if (pageNum == 0)
            {
                return;
            }

            pageNum = 0;
            GetFriends();
        }

        private void btn_last_Click(object sender)
        {
            pageNum--;
            if (pageNum < 0)
            {
                pageNum = 0;
                return;
            }
            GetFriends();
        }

        private void btn_next_Click(object sender)
        {
            pageNum++;
            GetFriends();
        }

        #region 获取朋友
        /// <summary>
        /// 获取朋友
        /// </summary>
        /// <param name="nickname">昵称</param>
        public void GetFriends(string nickname = "1893888")
        {
            if (SearchWords != "")
            {
                nickname = SearchWords;
            }
            AccountList.Clear();
            var client = APIHelper.GetNerbyFriendsAsync(nickname, pageNum, pageSize);
            client.UploadDataCompleted += OnSearchComplete;//指定回调
        }
        #endregion

        #region 接口调用完成后
        private void OnSearchComplete(object sender, UploadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                JsonNerbyuser resusers = new JsonNerbyuser();
                var str = Encoding.UTF8.GetString(e.Result);//转byte为字符串
                resusers = JsonConvert.DeserializeObject<JsonNerbyuser>(str);//反序列化
                if (resusers.resultCode == 1)
                {
                    List<DownLoadFile> files = new List<DownLoadFile>();
                    if (resusers.data.Count > 0)
                    {
                        for (int i = 0; i < resusers.data.Count; i++)
                        {
                            AccountList.Add(resusers.data[i]);

                            //这里需要把特殊账号 排除出来。
                            string localpath = Applicate.LocalConfigData.GetDisplayAvatorPath(resusers.data[i].userId);
                            if (!File.Exists(localpath))//添加需要下载的头像
                            {
                                files.Add(new DownLoadFile
                                {
                                    Jid = resusers.data[i].userId,
                                    Name = resusers.data[i].nickname,
                                    Token = resusers.data[i].userId,
                                    ShouldDeleteWhileFileExists = true,
                                    LocalUrl = localpath,
                                    Type = DownLoadFileType.Image
                                });
                            }
                        }
                        HttpDownloader.Download(files, (item) =>
                        {
                            switch (item.State)
                            {
                                case DownloadState.Successed:
                                    var tmp = AccountList.FirstOrDefault(a => a.userId == item.Jid);
                                    if (tmp != null)
                                    {
                                        App.Current.Dispatcher.Invoke(() =>
                                        {
                                            tmp.userId = item.Jid;//下载完成后刷新头像
                                        });
                                    }
                                    break;
                                case DownloadState.Error:
                                    break;
                                default:
                                    break;
                            }
                        });
                    }
                    else
                    {
                        pageNum--;
                    }
                }
            }
            else
            {
                SnackBar.Enqueue("错误:" + e.Error.Message);
            }
            ShowLoadingProgressBar(false);//禁用按钮并显示
        }
        #endregion

        #region 显示或进度条
        /// <summary>
        /// 显示或进度条
        /// </summary>
        /// <param name="IsShow">显示或隐藏</param>
        internal void ShowLoadingProgressBar(bool IsShow)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (IsShow)
                {
                    SearchEnable = false;
                }
                else
                {
                    SearchEnable = true;
                }
            });
        }
        #endregion

    }
}
