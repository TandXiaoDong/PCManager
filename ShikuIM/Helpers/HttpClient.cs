using System;
using System.Net;
using System.Text;

namespace ShikuIM
{
    /// <summary>
    /// 提供用于自定义的WebClient
    /// </summary>
    public class HttpClient : WebClient
    {
        public delegate void JXHandle();
        public event JXHandle OnStart;
        public event JXHandle OnFinish;
        public event JXHandle OnFailed;

        /// <summary>
        /// 获取或设置一个用于存储有关此元素的自定义信息的任意对象值
        /// <para>通常用于回调函数中对数据进行二次获取</para>
        /// </summary>
        public object Tag { get; set; }
        public object Tag2 { get; set; }
        public string Tag3 { get; set; }

        #region 初始化一个HttpClient类
        /// <summary>
        /// 初始化一个HttpClient类(表单提交方式为Post)
        /// </summary>
        public HttpClient() : base()
        {
            Headers.Add("Content-Type", "application/x-www-form-urlencoded");//Post提交方式需要的
            //this.Proxy = WebRequest.GetSystemWebProxy();//默认使用IE代理
            Proxy = null;
            //日志记录回调函数
            this.UploadDataCompleted += (sen, e) =>
            {
                try
                {
                    var resstr = Encoding.UTF8.GetString(e.Result);
                    LogHelper.log.Info("接口返回:0.0:" + resstr);
                }
                catch (Exception ex)
                {
                    ConsoleLog.Output(ex.Message);
                    LogHelper.log.Error("调用接口错误：URL:" + this.BaseAddress + "ErrorInfo" + ex.Message);
                }
            };
        }
        #endregion

        public new void UploadDataAsync(Uri address, byte[] data)
        {
            try
            {
                Start();//开始请求前
                this.UploadDataCompleted += Finish;//请求完成
                base.UploadDataAsync(address, data);
            }
            catch (WebException ex)
            {
                var result = ex.Response as HttpWebResponse;
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    Failed(ex.Message);//404
                }
                else
                {
                    LogHelper.log.Error("接口调用出错" + ex, ex);
                    ConsoleLog.Output("接口调用出错" + ex);
                }
            }
            catch (Exception eex)
            {
                LogHelper.log.Error("接口调用出错" + eex.Message, eex);
                ConsoleLog.Output("接口调用出错" + eex);
            }

        }

        private void Finish(object sender, UploadDataCompletedEventArgs e)
        {
            OnFinish?.Invoke();
        }

        private void Start()
        {
            OnStart?.Invoke();
        }

        private void Failed(string ex)
        {
            OnFailed?.Invoke();
        }
        public void SetComleteEvent(UploadDataCompletedEventHandler upload)
        {
            this.UploadDataCompleted -= Finish;
            this.UploadDataCompleted += upload;
            this.UploadDataCompleted += Finish;
        }
        //private void TestMethod(int isShow)
        //{
        ////命名空间
        //var nsp = parent.GetType().Namespace;
        ////类名称
        //var fnm = parent.GetType().FullName;
        ////方法名
        //var methodName = "ProgressControl";
        //try
        //{
        //    Type type = Assembly.Load(nsp).GetType(fnm);
        //    var methods = type.GetMethods();
        //    if (methods.Any(d => d.Name == methodName))
        //    {
        //        MethodInfo method = type.GetMethod(methodName);
        //        object obj = Activator.CreateInstance(type);
        //        object[] parameters = new object[] { Parent, isShow, null, 0 };
        //        method.Invoke(obj, parameters);
        //    }
        //}
        //catch (Exception e)
        //{
        //    //MessageBox.Show(e.Message);
        //}

        //}
    }
}
