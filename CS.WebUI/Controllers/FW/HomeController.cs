using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.BLL.FW;
using CS.BLL;
using CS.Base.Log;
using CS.WebUI.Models;
using System.Data;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 主页
    /// </summary>
    public class HomeController : ABaseController
    {
        /// <summary>
        /// 框架首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //写其它内容
            #region 其它内容写到这里以方便剥离

            #endregion

            return View();
        }

        /// <summary>
        /// 默认标签页
        /// </summary>
        /// <returns></returns>
        public ActionResult Default()
        {
            int userID = SystemSession.UserID;
            ViewBag.Title = MvcApplication.SystemName + "-" + "首页";

            //写其它内容
            #region 其它内容写到这里以方便剥离

            #endregion
            return View();
        }

        #region 项目中的个性化内容写到这里，以方便剥离
        
        #endregion


        #region 框架中的方法，请勿进行个性化修改

        /// <summary>
        /// 根据菜单名称：获取菜单URL
        /// </summary>
        /// <param name="name">菜单名称(名称可以重复，取名复杂点)</param>
        /// <returns></returns>
        public ActionResult ModuleUrl(string name)
        {
            JsonResultData result = new JsonResultData();
            int num = 0;
            try
            {
                if (name == "" || name == null)
                {
                    //获取当前用户未读的数量
                    int userID = SystemSession.UserID;
                    num = BF_BULLETIN_USER.Instance.GetBullReadNum(userID);
                    result.IsSuccess = true;
                    result.Message = num.ToString();
                }
                else
                {
                    var model = BLL.FW.BF_MENU.Instance.GetList<BF_MENU.Entity>("NAME=?", name);
                    if (model.Count() < 1)
                    {
                        result.IsSuccess = false;
                        result.Message = "未找到该菜单!";
                    }
                    else
                    {
                        //只取第一条
                        result.IsSuccess = true;
                        result.Message = model[0].URL;
                    }
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "删除公告程序出现错误：" + ex.ToString());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult SecurityCode()
        {
            string code = BF_USER.GetValidateCode(4);
            TempData["SecurityCode"] = code;
            return File(BF_USER.GetValidateImage(code), "image/Jpeg");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            SystemSession.Clear();
            ViewBag.ErrorMessage = "";
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string password, string captcha)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.ErrorMessage = "用户名不可为空";
                return View();
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "密码不可为空";
                return View();
            }
            if (string.IsNullOrWhiteSpace(captcha))
            {
                ViewBag.ErrorMessage = "验证码不可为空";
                return View();
            }
            if (CheckCaptcha(captcha) == false)
            {
                ViewBag.ErrorMessage = "验证码不正确";
                return View();
            }

            string errorMessage = string.Empty;
            BF_USER.Entity userInfo = null;
            Dictionary<int, BF_MENU.Entity> menus = new Dictionary<int, BF_MENU.Entity>();
            if (BF_USER.Instance.Login(name, password, out userInfo, out menus, out errorMessage) == false)
            {
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }

            if (menus == null || menus.Count < 1)
            {
                ViewBag.ErrorMessage = "没有可访问的菜单，请联系管理员分配角色";
                return View();
            }

            //写SESSION
            WriteSession(userInfo, menus);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        private bool CheckCaptcha(string captcha)
        {
            return true;//开发期间临时关闭验证码
            string code = TempData["SecurityCode"] as string;
            return string.IsNullOrWhiteSpace(captcha) == false && captcha.ToUpper() == code.ToUpper();
        }

        /// <summary>
        /// 写入SESSION
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="menus">可访问菜单</param>
        private void WriteSession(BF_USER.Entity userInfo, Dictionary<int, BF_MENU.Entity> menus)
        {
            SystemSession.WriteSession(userInfo.ID, userInfo.NAME,userInfo.FULL_NAME, userInfo.DEPT_ID, menus);
        }

        #endregion

        
    }
}