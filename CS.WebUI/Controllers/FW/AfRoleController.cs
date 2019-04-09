using CS.Base.Log;
using CS.BLL.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class AfRoleController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);
            IList<BLL.FW.BF_ROLE.Entity> list = BLL.FW.BF_ROLE.Instance.GetListPage<BLL.FW.BF_ROLE.Entity>(limit, page, order, where);
            int count = BLL.FW.BF_ROLE.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BLL.FW.BF_ROLE.Entity>();
                count = 0;
            }

            return ToJsonString(list, count);
        }

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {

            var dt = BLL.FW.BF_MENU.Instance.GetTableFields("ID,PID,NAME", "IS_ENABLE = ? ", 1);
            var obj = new List<object>();
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
            BLL.FW.BF_ROLE.Entity entity = new BLL.FW.BF_ROLE.Entity();

            if (id > 0)
            {
                entity = BLL.FW.BF_ROLE.Instance.GetEntityByKey<BLL.FW.BF_ROLE.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "角色不存在";
                    entity = new BLL.FW.BF_ROLE.Entity();
                    entity.ID = -1;
                }
            }
            ModelState.Clear();
            return View(entity);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BLL.FW.BF_ROLE.Entity entity)
        {
            JsonResultData result = new JsonResultData();

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("NAME", entity.NAME);
            dic.Add("MENU_IDS", entity.MENU_IDS);
            dic.Add("REMARK", entity.REMARK);
            dic.Add("UPDATE_UID", BLL.FW.SystemSession.UserID);
            dic.Add("UPDATE_TIME", DateTime.Now);
            try
            {
                if (entity.ID > 0)
                {
                    var r = BLL.FW.BF_ROLE.Instance.UpdateByKey(dic, entity.ID);
                    if (r > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "更新成功";
                    }
                }
                else
                {
                    dic.Add("IS_ENABLE", 1);
                    dic.Add("CREATE_UID", BLL.FW.SystemSession.UserID);
                    dic.Add("CREATE_TIME", DateTime.Now);

                    var r = BLL.FW.BF_ROLE.Instance.Add(dic);
                    if (r > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "新增成功";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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
                int i = BF_ROLE.Instance.SetEnable(id);
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
                int i = BF_ROLE.Instance.SetUnable(id);
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
                int i = BF_ROLE.Instance.DeleteByKey(id);
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
        public ActionResult ExportFile(string name = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_角色管理_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }

                var dt = BLL.FW.BF_ROLE.Instance.GetTableFields("NAME,IS_ENABLE,REMARK,CREATE_TIME,UPDATE_TIME", where);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["NAME"].Caption = "角色名称";
                dt.Columns["IS_ENABLE"].Caption = "启用状态";
                dt.Columns["REMARK"].Caption = "角色说明";
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
            return ShowAlert("导出成功");
        }
    }
}