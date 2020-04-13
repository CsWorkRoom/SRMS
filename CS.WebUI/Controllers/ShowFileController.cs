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
        public ActionResult Download(int fileId)
        {
            if (fileId > 0)
            {
                var file = BLL.SR.SR_FILES.Instance.GetEntityByKey<BLL.SR.SR_FILES.Entity>(fileId);
                if (file == null)
                {
                    return Content("未找到编号为[" + fileId + "]的文件信息");
                }
                string fileName = file.PATH + "\\" + file.REAL_NAME;
                if (System.IO.File.Exists(fileName) == false)
                {
                    return Content("文件" + fileName + "不存在");
                }

                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + file.DISPLAY_NAME);
                System.Web.HttpContext.Current.Response.WriteFile(fileName);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
            }
            else
            {
                return Content("未传入满足的文件编号");
            }
            return null;
        }
    }
}