using CS.BLL.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class AfGlobalController : ABaseController
    {
        /// <summary>
        /// 加载页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string json = "<script type='text/javascript'>";
            json += " configData=" + SerializeObject(BLL.FW.BF_SYS_CONFIG.GetAllConfigs()) + ";";
            json += "</script>";
            ViewBag.Config = json;
            return View();
        }
        /// <summary>
        /// 获取系统名称
        /// </summary>
        /// <returns></returns>
        public JsonResult getSysname()
        {
            string jsonString = SerializeObject(BLL.FW.BF_SYS_CONFIG.GetAllConfigs()[0]);
            return Json(jsonString);
        }
        /// <summary>
        /// 获取登录用户名
        /// </summary>
        /// <returns></returns>
        public JsonResult getUsername()
        {
            var username = SystemSession.UserName;
            return Json(username);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="configjson"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string configjson)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                int n = BLL.FW.BF_SYS_CONFIG.SetAllConfigs(configjson);
                result.IsSuccess = true;
                result.Message = "成功更新了" + n + "个配置";
                MvcApplication.UpdateSystemConfigs();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}