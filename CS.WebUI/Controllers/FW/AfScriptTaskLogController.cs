using CS.Base.Log;
using CS.BLL.FW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    public class AfScriptTaskLogController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Request["TaskId"] == null)
                return null;
            
            ViewBag.TaskId = Request["TaskId"];//是否呈现
            return View();
        }

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="id"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string message = "", int TaskId = 0, string orderByField = "ID", string orderByType = "ASC")
        {
            if (TaskId <= 0)
                return "[]";

            List<object> param = new List<object>();
            string where = "TASK_ID=?";
            param.Add(TaskId);

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                where += " and message LIKE '%" + message.Replace('\'', ' ') + "%'";
            }

            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BLL.FW.BF_ST_TASK_LOG.Entity> list = BLL.FW.BF_ST_TASK_LOG.Instance.GetListPage<BLL.FW.BF_ST_TASK_LOG.Entity>(limit, page, order, where, param);
            int count = BF_ST_TASK_LOG.Instance.GetCount(where, param);
            if (list == null)
            {
                list = new List<BLL.FW.BF_ST_TASK_LOG.Entity>();
                count = 0;
            }
            return ToJsonString(list, count);
        }
        #endregion

        #region 导出文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string message = "", int TaskId = 0)
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_任务日志_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

                List<object> param = new List<object>();
                #region 添加参数
                string where = "TASK_ID=?";
                param.Add(TaskId);

                if (string.IsNullOrWhiteSpace(message) == false)
                    where += " and message LIKE '%" + message.Replace('\'', ' ') + "%'";
                #endregion

                var dt = BLL.FW.BF_ST_TASK_LOG.Instance.GetTableFields("LOG_TIME,LOG_LEVEL,MESSAGE", where, param);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["LOG_TIME"].Caption = "日志时间";
                dt.Columns["LOG_LEVEL"].Caption = "等级";
                dt.Columns["MESSAGE"].Caption = "日志内容";

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
        #endregion
    }
}