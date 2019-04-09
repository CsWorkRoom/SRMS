using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Base.Log;
using System.IO;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 后台运行日志
    /// </summary>
    public class AfFileLogController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="filename">日志文件名</param>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.DATE_RANGE = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + " - " + DateTime.Today.ToString("yyyy-MM-dd");
            return View();
        }

        /// <summary>
        /// 列表页
        /// </summary>
        /// <param name="daterange">日期范围</param>
        /// <param name="filename">日志文件名</param>
        [HttpPost]
        public ActionResult Index(string daterange = "", string filename = "")
        {
            if (string.IsNullOrWhiteSpace(filename) == false)
            {
                if (System.IO.File.Exists(BLog.LogFilePath + "/" + filename) == false)
                {
                    return ShowAlert("日志文件" + filename + "不存在");
                }
                try
                {
                    using (FileStream fs = new FileStream(BLog.LogFilePath + "/" + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                        string content = sr.ReadToEnd().Replace("<", "&lt;").Replace(">", "&gt;");
                        sr.Close();
                        return Content(content);
                    }
                }
                catch (Exception ex)
                {
                    return Content("读取日志出错：" + ex.Message);
                }
            }

            DateTime beginDate = DateTime.Today.AddDays(-1);
            DateTime endDate = DateTime.Today;
            if (string.IsNullOrWhiteSpace(daterange) == false)
            {
                string[] ss = daterange.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length == 2)
                {
                    DateTime.TryParse(ss[0], out beginDate);
                    DateTime.TryParse(ss[1], out endDate);
                }
            }
            endDate = endDate.AddDays(1);

            List<string> files = new List<string>();
            if (Directory.Exists(BLog.LogFilePath) == true)
            {
                DirectoryInfo di = new DirectoryInfo(BLog.LogFilePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.LastWriteTime >= beginDate && file.LastWriteTime < endDate)
                    {
                        files.Add(file.Name);
                    }
                }
            }

            return Content(SerializeObject(files));
        }
    }
}