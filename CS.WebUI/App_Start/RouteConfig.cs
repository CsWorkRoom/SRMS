using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CS.WebUI
{
    /// <summary>
    /// 路由配置
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "CS.WebUI.Controllers", "CS.WebUI.Controllers.FW" }
            );
            //routes.IgnoreRoute("{API}/{resource}.asmx/{*pathInfo}");
        }
    }
}
