using ShikuIM;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Http下载工具类
/// </summary>
public static class HttpDownloader
{
    #region 下载
    internal static void Download(List<DownLoadFile> downlist, Action<DownLoadFile> action)
    {
        Task.Run(() =>
        {
            //这里需要在线程中循环下载下载
            foreach (var item in downlist)
            {
                try
                {
                    if (File.Exists(item.LocalUrl) && item.ShouldDeleteWhileFileExists)//如果对应文件存在 且 需要下载替换对应文件时
                    {
                        File.Delete(item.LocalUrl);//先删除文件(再下载文件)
                    }
                    else if (File.Exists(item.LocalUrl) && !item.ShouldDeleteWhileFileExists)
                    {
                        continue;//如果文件存在不需要删除时 ,, 不执行循环剩下代码并执行下一个循环
                    }
                    var result = APIHelper.DownloadAvator(item.Jid, true);
                    //HTTP http = new HTTP();
                    //HttpResult result = http.GetHtml(new HttpItem()
                    //{
                    //    URL = item.Url,
                    //    ResultType = ResultType.Byte,
                    //    Postdata = ""
                    //});
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)//服务器返回正常才处理字节
                    {
                        if (item.Type == DownLoadFileType.Image)
                        {
                            var avator = Helpers.ByteToBitmap(result.ResultByte);
                            #region 保存文件
                            if (avator.RawFormat.Guid == ImageFormat.Png.Guid)
                            {
                                avator.Save(item.LocalUrl, ImageFormat.Png);//写入Png图片到目录
                            }
                            else if (avator.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                            {
                                avator.Save(item.LocalUrl, ImageFormat.Png);//写入Png图片到目录
                            }
                            else
                            {
                                avator.Save(item.LocalUrl);//写入图片
                            }
                            #endregion
                        }
                        //委托调用 通知前端 某一张 下载完成
                        item.State = DownloadState.Successed;
                        action(item);
                    }
                    else
                    {
                        item.State = DownloadState.Error;
                        action(item);
                    }
                }
                catch (Exception ex)//下载错误的时候该怎么弄头像
                {
                    ConsoleLog.Output("File--DownloadError" + ex.Message);
                    item.State = DownloadState.Error;
                    action(item);
                }
            }
        });
    }
    #endregion
}

