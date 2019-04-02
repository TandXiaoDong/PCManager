using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ShikuIM.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace ShikuIM.ViewModel
{
    public class GroupShareViewModel : ViewModelBase
    {
        ObservableCollection<RoomShare> _shareList;
        ObservableCollection<RoomShare> _uploadList;

        private SnackbarMessageQueue snackbar;


        /// <summary>
        /// 提示控件
        /// </summary>
        public SnackbarMessageQueue Snackbar
        {
            get { return snackbar; }
            set { snackbar = value; RaisePropertyChanged(nameof(Snackbar)); }
        }

        /*
        bool isShowSnackbar = false;
        string downLoadMessage = "";*/
        public ObservableCollection<RoomShare> ShareList
        {
            get
            {
                return _shareList;
            }
            set
            {
                _shareList = value;
                RaisePropertyChanged(nameof(ShareList));
            }
        }
        public ObservableCollection<RoomShare> uploadList
        {
            get
            {
                return _uploadList;
            }
            set
            {
                _uploadList = value;
                RaisePropertyChanged(nameof(uploadList));
            }
        }
        public DateTime selectDate { get; set; }
        bool isOpen = false;
        bool allowUploadFile;
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
                RaisePropertyChanged(nameof(isOpen));
            }
        }
        public bool AllowUploadFile
        {
            get
            {
                return allowUploadFile;
            }
            set
            {
                allowUploadFile = value;
                RaisePropertyChanged(nameof(AllowUploadFile));
            }
        }
        int pageNum = 0;
        //int pageSize = 50;
        Room room;
        MemberRole role;
        #region Contructor
        public GroupShareViewModel(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return;
            }
            Snackbar = new SnackbarMessageQueue();
            room = new Room() { id = roomId }.GetByRoomId();

            if (room != null)
            {
                var memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                if (memlist.Count == 0)
                {
                    APIHelper.GetRoomDetialByRoomId(room.id);
                    memlist = new DataofMember() { groupid = room.id }.GetListByRoomId();
                }
                var user = memlist.FirstOrDefault(m => m.userId == Applicate.MyAccount.userId/*查询出自己的身份编号*/);
                role = user.role;
                AllowUploadFile = room.allowUploadFile == 1 || user.role != MemberRole.Member;
            }
            uploadList = new ObservableCollection<RoomShare>();
            GetRoomShare();
            RegisterMessengers();
        }
        #endregion

        #region 注册消息
        private void RegisterMessengers()
        {
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgRecived, msg => { ProcessNewMsg(msg); });
            Messenger.Default.Register<Messageobject>(this, CommonNotifications.XmppMsgReceipt, msg => { ProcessMsgReceipt(msg); });

        }
        #endregion

        #region 处理消息回执
        /// <summary>
        /// 处理消息回执
        /// </summary>
        /// <param name="msg">收到回执的消息</param>
        private void ProcessMsgReceipt(Messageobject msg)
        {
            switch (msg.type)
            {
                case kWCMessageType.RoomFileDelete:
                    //DeleteFileItem(msg.fileName);//删除文件
                    break;
                case kWCMessageType.RoomFileDownload:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 处理新消息   
        /// <summary>
        /// 处理新消息
        /// </summary>
        /// <param name="msg">新消息</param>
        private void ProcessNewMsg(Messageobject msg)
        {
            switch (msg.type)
            {
                case kWCMessageType.RoomFileDelete:
                    DeleteFileItem(msg.content);//删除文件
                    break;
                case kWCMessageType.RoomFileDownload:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 删除对应文件名称项
        /// <summary>
        /// 删除对应文件名称项
        /// </summary>
        /// <param name="shareId"></param>
        private void DeleteFileItem(string shareId)
        {
            var item = ShareList.FirstOrDefault(f => f.shareId == shareId);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (item != null)
                {
                    ShareList.Remove(item);
                }
            });
        }
        #endregion

        #region Commands
        #region 第一页
        public ICommand HomePage
        {
            get
            {
                return new RelayCommand(() =>
                {
                    pageNum = 0; GetRoomShare();
                });
            }
        }
        #endregion

        #region 上一页
        public ICommand PreviousPage
        {
            get
            {
                return new RelayCommand(() =>
                {
                    pageNum--; GetRoomShare();
                });
            }
        }
        #endregion

        #region 下一页
        public ICommand NextPage
        {
            get
            {
                return new RelayCommand(() =>
                {
                    pageNum++; GetRoomShare();
                });
            }
        }
        #endregion

        #region 最后一页
        public ICommand LastPage
        {
            get
            {
                return new RelayCommand(() =>
                {

                });
            }
        }
        #endregion

        public ICommand UpdateShare
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //文件按钮先选中并打开一个文件进行上传操作
                    System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
                    fd.Multiselect = true;//可多选
                    fd.Filter = "文件|*.*|图片|*.jpg;*.png;*.jpeg;*.bmp";//文件筛选器
                    fd.ShowDialog();
                    //是否为
                    if (fd.FileNames.Length > 0)
                    {
                        foreach (var fileName in fd.FileNames)
                        {
                            ConsoleLog.Output("////******************************选中的文件为" + fileName);
                            //异步发送消息
                            UpdateFile(fileName);
                        }
                    }
                });
            }
        }

        #region 打开日期选择
        public ICommand DialogHostShow
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsOpen = true;
                });
            }
        }
        #endregion

        #region 隐藏日期选择
        public ICommand DialogHostHiden
        {
            get
            {
                return new RelayCommand(() =>
                {
                    selectDate = default(DateTime);
                    IsOpen = false;
                });
            }
        }
        #endregion

        #region 搜索
        public ICommand getDateSearch
        {
            get
            {
                return new RelayCommand(() =>
                {
                    pageNum = 0; GetRoomShare(); IsOpen = false;
                });
            }
        }
        #endregion

        #region 下载
        public ICommand OpenFilePath
        {
            get
            {
                return new RelayCommand<string>((para) =>
                {
                    RoomShare roomShare = null;

                    if (para != null)
                    {
                        roomShare = new RoomShare().toModel(para.ToString());
                        if (roomShare.filePath != null && File.Exists(roomShare.filePath))
                        {
                            Process.Start(Path.GetDirectoryName(roomShare.filePath));
                            return;
                        }
                        if (!String.IsNullOrWhiteSpace(roomShare.progress))
                        {
                            return;
                        }
                    }
                    if (roomShare != null)
                    {
                        var obj = ShareList.FirstOrDefault(d => d.shareId == roomShare.shareId);
                        System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                        saveFileDialog1.InitialDirectory = Applicate.LocalConfigData.ChatDownloadPath;
                        saveFileDialog1.Filter = "All files (*.*)|*.*";
                        saveFileDialog1.FileName = roomShare.name;
                        //saveFileDialog1.DefaultExt = roomShare.name.Substring(roomShare.name.LastIndexOf('.') + 1);
                        saveFileDialog1.FilterIndex = 2;
                        saveFileDialog1.RestoreDirectory = true;
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            HttpClient myWebClient = new HttpClient();
                            myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler((s, o) =>
                            {
                                roomShare.filePath = saveFileDialog1.FileName;
                                roomShare.UpdateFilePath();
                                GetRoomShare();
                            });
                            myWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((s, o) =>
                            {
                                if (obj != null)
                                {
                                    obj.progress = o.ProgressPercentage.ToString() + "%";
                                }
                            });
                            myWebClient.DownloadFileAsync(new Uri(roomShare.url), saveFileDialog1.FileName);
                        }
                    }
                });
            }
        }

        #endregion

        #region 删除群共享
        public ICommand DelFileCommand
        {
            get
            {
                return new RelayCommand<string>((para) =>
                {
                    if (para != null)
                    {
                        RoomShare roomShare = new RoomShare().toModel(para.ToString());
                        var client = APIHelper.DelRoomSharesAsync(roomShare, Applicate.MyAccount.userId);
                        client.Tag = roomShare;//设置
                        client.UploadDataCompleted += (s, ev) =>
                        {
                            if (ev.Error == null)
                            {
                                var res = Encoding.UTF8.GetString(ev.Result);
                                var result = JsonConvert.DeserializeObject<JsonBase>(res);
                                if (result.resultCode == 1)
                                {
                                    var hclient = (HttpClient)s;
                                    RoomShare file = hclient.Tag as RoomShare;
                                    DeleteFileItem(file.shareId);//删除对应的群共享文件
                                    Snackbar.Enqueue("删除成功");//提示
                                    //GetRoomShare();//刷新列表
                                }
                                else
                                {
                                    Snackbar.Enqueue("删除失败:" + result.resultMsg);
                                }
                            }
                            else
                            {
                                Snackbar.Enqueue("删除失败");
                                ConsoleLog.Output("删除群文件失败:" + ev.Error.Message);
                            }
                        };
                    }
                });
            }
        }

        #endregion 
        #endregion

        private void GetRoomShare()
        {
            if (pageNum < 0)
            {
                pageNum = 0;
            }
            long time = selectDate == default(DateTime) ? 0 : Helpers.DatetimeToStamp(selectDate);
            if (room != null)
            {
                var client = APIHelper.GetRoomSharesAsync(room.id, 0, 999, time);
                client.UploadDataCompleted += (sen, res) =>
                {
                    string s = Encoding.UTF8.GetString(res.Result);
                    var rtn = JsonConvert.DeserializeObject<JsonRoomShare>(Encoding.UTF8.GetString(res.Result));
                    if (rtn.data == null || rtn.data.Count == 0)
                    {
                        pageNum--;
                    }
                    else
                    {
                        var list = new RoomShare() { roomId = room.id }.GetListByRoomId();
                        rtn.data.ForEach(d =>
                        {
                            var obj = list.FirstOrDefault(v => v.shareId == d.shareId);
                            if (obj != null && !string.IsNullOrWhiteSpace(obj.filePath))
                            {
                                d.filePath = obj.filePath;
                            }
                            d.detial = d.toJson();
                            d.AllowDel = d.userId == Applicate.MyAccount.userId || role != MemberRole.Member;
                        });
                        ShareList = Helpers.ToObservableCollection(rtn.data);
                    }
                };
            }
        }

        #region 发送文件类信息
        /// <summary>
        /// 发送文件类信息
        /// </summary>
        /// <param name="localpath">本地文件路径(包括文件名)</param>
        private void UpdateFile(string localpath)
        {
            #region 封装roomShare
            if (room == null)
            {
                return;
            }
            var roomShare = new RoomShare()
            {
                roomId = room.id,
                type = GetFileType(localpath),
                size = Convert.ToInt32(new FileInfo(localpath).Length),//文件大小
                userId = Applicate.MyAccount.userId,
                name = Path.GetFileName(localpath),
            };
            #endregion
            //上传文件
            //byte[] bytes = FileUtil.HttpUpLoad(localpath, Applicate.Access_Token);//上传文件
            var clientAsync = new HttpClient();
            var uri = new Uri(Applicate.URLDATA.data.uploadUrl + "upload/UploadServlet");
            clientAsync.UploadFileCompleted += (sen, res) =>
            {
                if (res.Error != null)
                {
                    if (res.Error.Message.Contains("413"))//以后再改
                    {
                        roomShare.progress = "文件过大，上传失败";
                    }
                    else
                    {
                        roomShare.progress = "上传失败";
                    }

                    return;
                }
                uploadList.Remove(roomShare);
                var ss = Encoding.UTF8.GetString(res.Result);
                var rtnInfo = JsonConvert.DeserializeObject<JsonFileinfo>(Encoding.UTF8.GetString(res.Result));//获取返回值
                if (rtnInfo.data.images.Count > 0)
                {
                    roomShare.url = rtnInfo.data.images[0].oUrl;//消息内容为返回的图片原始Url
                }
                else if (rtnInfo.data.videos.Count > 0)
                {
                    roomShare.url = rtnInfo.data.videos[0].oUrl;
                }
                else if (rtnInfo.data.audios.Count > 0)
                {
                    roomShare.url = rtnInfo.data.audios[0].oUrl;
                }
                else if (rtnInfo.data.others.Count > 0)
                {
                    roomShare.url = rtnInfo.data.others[0].oUrl;//消息内容为返回的文件原始Url
                }
                var client = APIHelper.AddRoomSharesAsync(roomShare);
                client.UploadDataCompleted += (s, o) =>
                {
                    //var shareObj = JsonConvert.DeserializeObject<RoomShare>(Encoding.UTF8.GetString(o.Result), typeof(RoomShare));
                    //shareObj.filePath = localpath;
                    //shareObj.Insert();
                    GetRoomShare();
                };
            };
            clientAsync.UploadProgressChanged += (s, o) =>
            {
                roomShare.progress = (o.ProgressPercentage * 2).ToString() + "%";//50%表示上传成功
            };
            uploadList.Add(roomShare);
            clientAsync.UploadFileAsync(uri, localpath);
        }
        #endregion

        #region 获取文件类型
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private int GetFileType(string fileName)
        {
            int t = 0;
            string extName = fileName.ToLower().Substring(fileName.LastIndexOf('.') + 1);
            switch (extName)
            {
                case "png":
                case "jpg":
                case "jpeg":
                case "bmp":
                case "gif":
                    t = 1;
                    break;
                case "mp3":
                    t = 2;
                    break;
                case "mp4":
                    t = 3;
                    break;
                case "xls":
                    t = 5;
                    break;
                case "doc":
                    t = 6;
                    break;
                case "ppt":
                    t = 4;
                    break;
                case "pdf":
                    t = 10;
                    break;
                case "apk":
                    t = 11;
                    break;
                case "txt":
                    t = 8;
                    break;
                case "rar":
                case "zip":
                    t = 7;
                    break;
            }
            return t;
        }

        #endregion
    }
}