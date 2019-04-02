﻿using GalaSoft.MvvmLight;
using System.Timers;

namespace ShikuIM.Model
{

    /// <summary>
    /// 酷聊已知支持的设备平台
    /// </summary>
    public partial class Platform : ObservableObject
    {

        #region Private Members
        private int id;
        private string machineName;
        private bool isOnline;
        private bool isSendRecipt;

        #endregion

        #region Public Members
        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatformName
        {
            get { return machineName; }
            set { machineName = value; RaisePropertyChanged(nameof(PlatformName)); }
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline
        {
            get { return isOnline; }
            set { isOnline = value; RaisePropertyChanged(nameof(IsOnline)); }
        }

        /// <summary>
        /// 是否发送过回执
        /// </summary>
        public bool IsSendRecipt
        {
            get { return isSendRecipt; }
            set { isSendRecipt = value; RaisePropertyChanged(nameof(IsSendRecipt)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformTimer Timer { get; internal set; }
        #endregion




        #region Contructor
        public Platform()
        {

        }
        #endregion

    }
}
