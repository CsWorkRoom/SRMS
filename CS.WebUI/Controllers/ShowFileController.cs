using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace CS.WebUI.Controllers
{
    /// <summary>
    /// 显示附件
    /// </summary>
    public class ShowFileController : Controller
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="pathName">路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public ActionResult Download(string pathName, string fileName)
        {
            string path = CS.Base.Config.BConfig.GetConfigToString(pathName);
            if (Directory.Exists(path) == false)
            {
                return Content("目录" + path + "不存在");
            }

            if (System.IO.File.Exists(path + "/" + fileName) == false)
            {
                return Content("文件" + fileName + "不存在");
            }

            //"{0}-{1}-{2}_{3}"  样本：20181106-104216-admin-4286_&&_favicon.ico
            int index = Math.Max(0, fileName.IndexOf("_&&_"));
            string name = fileName.Substring(index + 4);

            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
            System.Web.HttpContext.Current.Response.WriteFile(path + "/" + fileName);
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();

            return null;
        }
    }
}