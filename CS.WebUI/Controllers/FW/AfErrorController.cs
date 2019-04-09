using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public class AfErrorController : ABaseController
    {
        /// <summary>
        /// 错误页面
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="message">错误信息</param>
        /// <param name="url">URL地址</param>
        /// <returns></returns>
        public ActionResult Index(int code = 0, string message = "", string url = "")
        {
            ViewBag.ErrorCode = code;
            ViewBag.ErrorMessage = message;
            ViewBag.URL = url;
            return View();
        }
    }
}