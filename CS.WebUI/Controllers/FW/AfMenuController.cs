using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.WebUI.Models.FW;
using System.Data;
using CS.Base.Log;
using System.Text;
using CS.BLL.FW;
using System.Text.RegularExpressions;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    public class AfMenuController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.DIC_MENU_NAME = BF_MENU.Instance.GetDictionary("ID", "NAME");
            //
            var dt = BF_MENU.Instance.GetTableFields("ID,PID,NAME", "IS_ENABLE = ?", 1);
            var obj = new List<object>();
            obj.Add(
                new
                {
                    id = 0,
                    pId = 0,
                    name = BLL.FW.BF_SYS_CONFIG.SystemName
                });
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = Convert.ToInt32(dr["ID"]),
                        pId = Convert.ToInt32(dr["PID"]),
                        name = dr["NAME"].ToString()
                    });
            }
            ViewBag.DIC_MENUS = SerializeObject(obj);
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="pid"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, int pid = -1, string name = "", string url = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            List<object> param = new List<object>();
            if (pid >= 0)
            {
                where += " AND PID=?";
                param.Add(pid);
            }
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(url) == false)
            {
                where += " AND URL LIKE '%" + url.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BF_MENU.Entity> list = BF_MENU.Instance.GetListPage<BF_MENU.Entity>(limit, page, order, where, param);
            int count = BF_MENU.Instance.GetCount(where, param);
            if (list == null)
            {
                list = new List<BF_MENU.Entity>();
                count = 0;
            }
            return ToJsonString(list, count);
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        public string GetMenus()
        {
            int userID = SystemSession.UserID;
            if (userID < 0)
            {
                return ToJsonString(null, 0, 1, "未登录");
            }

            List<MenuModels> list = new List<MenuModels>();
            try
            {
                //从SESSION里面读取
                Dictionary<int, BF_MENU.Entity> dic = SystemSession.UserMenus;
                //从数据库里面查询最新（开发期间开启）
#if DEBUG
                dic = BF_USER.Instance.GetMenusByUserID(userID);
#endif
                MenuModels menuModel = new MenuModels();
                menuModel.id = 0;
                menuModel.pid = 0;
                menuModel.title = "";
                menuModel.url = string.Empty;
                LoadMenuByPid(dic, menuModel, menuModel.id);
                list.Add(menuModel);
                return ToJsonString(list, list.Count);
            }
            catch (Exception ex)
            {
                return ToJsonString(null, 0, 1, "获致菜单出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 加载子菜单
        /// </summary>
        /// <param name="dicMenus">所有可访问菜单</param>
        /// <param name="menuModel">菜单</param>
        /// <param name="pid">父节点ID</param>
        private void LoadMenuByPid(Dictionary<int, BF_MENU.Entity> dicMenus, MenuModels menuModel, int pid)
        {
            var list = dicMenus.Where(p => p.Value.PID == pid && p.Value.IS_ENABLE == 1 && p.Value.IS_SHOW_NAV == 1).OrderBy(o => o.Value.ORDER_NUM);
            if (list.Count() > 0)
            {
                menuModel.children = new List<MenuModels>();

                foreach (var kvp in list)
                {
                    MenuModels model = new MenuModels();
                    model.id = kvp.Key;
                    model.pid = pid;
                    model.title = kvp.Value.NAME;
                    model.font = string.IsNullOrWhiteSpace(kvp.Value.FONT) ? "layui-icon" : kvp.Value.FONT;
                    model.icon = string.IsNullOrWhiteSpace(kvp.Value.ICON) ? "layui-icon-wenzhang3" : kvp.Value.ICON;
                    if (string.IsNullOrEmpty(kvp.Value.URL))
                    {
                        model.url = string.Empty;
                    }
                    else
                    {
                        model.url = ApplicationPath + kvp.Value.URL.ToLower();
                    }

                    menuModel.children.Add(model);

                    //递归，加载子菜单
                    LoadMenuByPid(dicMenus, model, model.id);
                }
            }
        }

        /// <summary>
        /// 加载所有图标
        /// </summary>
        /// <returns></returns>
        public List<string> LoadIconNames()
        {
            List<string> list = new List<string>();
            string fileName = System.AppDomain.CurrentDomain.BaseDirectory + "\\Content\\layui\\font\\iconfontN.svg";
            if (System.IO.File.Exists(fileName))
            {
                Regex regex = new Regex("\\<glyph\\sglyph-name=\"(?<name>.*?)\"");
                string content = System.IO.File.ReadAllText(fileName);
                Match match = regex.Match(content);
                while (match.Success == true)
                {
                    list.Add(match.Result("${name}"));
                    match = match.NextMatch();
                }
            }
            return list;
        }

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid">上级菜单ID</param>
        /// <returns></returns>
        public ActionResult Edit(int id, int pid = 0)
        {
            Dictionary<int, string> dicRquestType = new Dictionary<int, string>();
            foreach (CS.Common.FW.Enums.ReportType source in Enum.GetValues(typeof(CS.Common.FW.Enums.ReportType)))
            {
                dicRquestType.Add((int)source, source.ToString());
            }

            var dt = BF_MENU.Instance.GetTableFields("ID,PID,NAME", "IS_ENABLE = ?", 1);
            var obj = new List<object>();
            obj.Add(
                new
                {
                    id = 0,
                    pId = 0,
                    name = BLL.FW.BF_SYS_CONFIG.SystemName
                });
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = Convert.ToInt32(dr["ID"]),
                        pId = Convert.ToInt32(dr["PID"]),
                        name = dr["NAME"].ToString()
                    });
            }

            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.DIC_MENUS = SerializeObject(obj);
            ViewBag.DIC_REQUEST_TYPE = dicRquestType;
            List<string> iconNames = LoadIconNames();
            ViewBag.LoadIconNames = "var iconNames = " + SerializeObject(LoadIconNames()) + ";";

            BF_MENU.Entity entity = new BF_MENU.Entity();

            if (id > 0)
            {
                entity = BF_MENU.Instance.GetEntityByKey<BF_MENU.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "菜单不存在";
                    entity = new BF_MENU.Entity();
                    entity.ID = -1;
                }
            }
            else
            {
                entity.PID = pid;
            }

            ModelState.Clear();
            return View(entity);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModel"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BF_MENU.Entity entity)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                if (entity.IS_SHOW_NAV == 1 && BF_MENU.Instance.IsSetMenus(entity.PID) == false)//是否超过三级
                {
                    result.IsSuccess = false;
                    result.Message = "增加的菜单层级不能超过3级，请重新选择上层父级";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("PID", entity.PID);
                dic.Add("NAME", entity.NAME);
                dic.Add("ORDER_NUM", entity.ORDER_NUM);
                dic.Add("URL", entity.URL);
                dic.Add("IS_SHOW_NAV", entity.IS_SHOW_NAV);
                if (string.IsNullOrWhiteSpace(entity.ICON) == false)
                {
                    dic.Add("FONT", "layui-icon");
                    dic.Add("ICON", entity.ICON);
                }

                dic.Add("REPORT_TYPE", entity.REPORT_TYPE);
                dic.Add("REPORT_ID", entity.REPORT_ID);
                dic.Add("UPDATE_UID", SystemSession.UserID);
                dic.Add("UPDATE_TIME", DateTime.Now);

                if (entity.ID > 0)
                {
                    //修改
                    int i = BF_MENU.Instance.UpdateByKey(dic, entity.ID);
                    if (i > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "更新成功";
                    }
                    else
                    {
                        result.Message = "出现了未知错误";
                    }
                }
                else
                {
                    //是否启用(默认)
                    dic.Add("IS_ENABLE", 1);
                    //是否为内置菜单(默认)
                    dic.Add("IS_DEFAULT", 0);
                    //创建者
                    dic.Add("CREATE_UID", SystemSession.UserID);
                    //创建时间
                    dic.Add("CREATE_TIME", DateTime.Now);
                    //添加
                    int i = BF_MENU.Instance.Add(dic);

                    if (i > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "添加成功";
                    }
                    else
                    {
                        result.Message = "出现了未知错误";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取报表列表
        /// </summary>
        /// <returns></returns>
        public string GetReport(string ReportType)
        {
            var reportType = (CS.Common.FW.Enums.ReportType)Enum.Parse(typeof(CS.Common.FW.Enums.ReportType), ReportType);
            //对象集合，用于存储报表列表信息
            List<object> objList = new List<object>();
            //数据字典，用户存储各个枚举标识与名称
            Dictionary<int, string> dic = new Dictionary<int, string>();
            //模板地址
            switch (reportType)
            {
                case CS.Common.FW.Enums.ReportType.表格报表查询:
                    dic = BF_TB_REPORT.Instance.GetDictionary("ID", "NAME", "IS_ENABLE =?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.表格报表导出:
                    dic = BF_TB_REPORT.Instance.GetDictionary("ID", "NAME", "IS_ENABLE =?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.图形报表查询:
                    dic = BF_CHART_REPORT.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.图形报表导出:
                    break;
                case CS.Common.FW.Enums.ReportType.数据外导:
                    dic = BF_IMPORT.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.表单修改:
                    dic = BF_FORM.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.表单删除:
                    dic = BF_FORM.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.表单添加:
                    dic = BF_FORM.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.附件上传:
                    dic = BF_FILE.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                case CS.Common.FW.Enums.ReportType.附件下载:
                    dic = BF_FILE.Instance.GetDictionary("ID", "NAME", "IS_ENABLE = ?", 1);
                    break;
                default:
                    break;
            }
            //将数据字典转换成对象集合
            foreach (var item in dic)
            {
                objList.Add(new
                {
                    id = item.Key,
                    name = item.Value,
                    url = GetEnumsReportTypeUrl(reportType).Replace("[Key]", item.Key.ToString())
                });
            }

            try
            {
                return SerializeObject(objList);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return "[]";
            }
        }
        /// <summary>
        ///  获取报表类型对应的Url地址信息
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>Url地址信息</returns>
        private string GetEnumsReportTypeUrl(Common.FW.Enums.ReportType type)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)Common.FW.Enums.ReportType.图形报表查询, "/AfChartReport/show?id=[Key]");
            dic.Add((int)Common.FW.Enums.ReportType.图形报表导出, "");
            dic.Add((int)Common.FW.Enums.ReportType.数据外导, "/AfImport/Import?id=[Key]");
            dic.Add((int)Common.FW.Enums.ReportType.系统页面, "");
            dic.Add((int)Common.FW.Enums.ReportType.表单修改, "/AfForm/Template?formId=[Key]&op=1");
            dic.Add((int)Common.FW.Enums.ReportType.表单删除, "/AfForm/Template?formId=[Key]&op=2");
            dic.Add((int)Common.FW.Enums.ReportType.表单添加, "/AfForm/Template?formId=[Key]&op=0");
            dic.Add((int)Common.FW.Enums.ReportType.表格报表查询, "/Aftablereport/show?id=[Key]");
            dic.Add((int)Common.FW.Enums.ReportType.表格报表导出, "/Aftablereport/ExportExcel?id=[Key]");
            dic.Add((int)Common.FW.Enums.ReportType.附件上传, "/AfFile/Upload?id=[Key]");
            dic.Add((int)Common.FW.Enums.ReportType.附件下载, "/AfFile/Download?id=[Key]");
            return dic[(int)type];
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetEnable(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "启用成功";
            try
            {
                int i = BF_MENU.Instance.SetEnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "启用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetUnable(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "禁用成功";
            try
            {
                int i = BF_MENU.Instance.SetUnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "禁用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "删除成功";

            try
            {
                int i = BF_MENU.Instance.DeleteByKey(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "删除失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(int pid = 0, string name = "", string url = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_菜单管理_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                List<object> param = new List<object>();
                if (pid > 0)
                {
                    where += " AND PID=?";
                    param.Add(pid);
                }
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                if (string.IsNullOrWhiteSpace(url) == false)
                {
                    where += " AND URL LIKE '%" + url.Replace('\'', ' ') + "%'";
                }

                var dt = BF_MENU.Instance.GetTableFields("PID,NAME,URL,IS_ENABLE,IS_SHOW_NAV,CREATE_TIME,UPDATE_TIME", where, param);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }

                dt.Columns["PID"].Caption = "上级菜单";
                dt.Columns["NAME"].Caption = "菜单名称";
                dt.Columns["URL"].Caption = "菜单URL地址";
                dt.Columns["IS_ENABLE"].Caption = "启用状态";
                dt.Columns["IS_SHOW_NAV"].Caption = "导航显示";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                dt.Columns["UPDATE_TIME"].Caption = "最后更新时间";


                Library.Export.ExcelFile export = new Library.Export.ExcelFile(path);
                string fullName = export.ToExcel(dt);
                if (string.IsNullOrWhiteSpace(fullName) == true)
                {
                    return ShowAlert("未生成Excel文件");
                }
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                System.Web.HttpContext.Current.Response.WriteFile(fullName);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
                //删除文件
                export.Delete(fullName);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "导出默认报表[DB]到Excel出错:" + ex.ToString());
                return ShowAlert("导出数据到Excel出现未知错误：" + ex.Message);
            }
            return ShowAlert("导出成功！");
        }
    }
}