using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CS.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 应用系统名称
        /// </summary>
        public static string SystemName = "后台管理系统";
        /// <summary>
        /// 当前登录用户名
        /// </summary>
        public static string UserName
        {
            get { try { return BLL.FW.SystemSession.UserName; } catch { return "未登录"; } }
        }

        /// <summary>
        /// 发布网站的根目录（URL中的应用程序目录）
        /// </summary>
        public static string ApplicationPath
        {
            get { return Controllers.FW.ABaseController.ApplicationPath; }
        }

        /// <summary>
        /// 启动网站
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            UpdateSystemConfigs();
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        public static void UpdateSystemConfigs()
        {
            SystemName = BLL.FW.BF_SYS_CONFIG.SystemName;
        }

        public static void SetApplicationPath(string path)
        {

        }
    }
}
