using CS.Base.Log;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 系统操作日志
    /// </summary>
    public class AfOperationLogController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.DATE_RANGE = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + " - " + DateTime.Today.ToString("yyyy-MM-dd");
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
        public string GetList(int page, int limit, string content = "", string daterange = "", string userName = "", string orderByField = "ID", string orderByType = "DESC")
        {
            #region 查询条件
            List<object> param = new List<object>();
            string where = "1=1";

            if (string.IsNullOrWhiteSpace(content) == false)
                where += " and content LIKE '%" + content.Replace('\'', ' ') + "%'";
            DateTime beginDate = DateTime.Today.AddDays(-1);
            DateTime endDate = DateTime.Today;
            if (string.IsNullOrWhiteSpace(daterange) == false)
                Functions.GetIntervalDate(daterange, ref beginDate, ref endDate);
            where += " and LOG_TIME >= to_date('" + beginDate + "','yyyy-mm-dd hh24:mi:ss') and LOG_TIME < to_date('" + endDate.AddDays(1) + "','yyyy-mm-dd hh24:mi:ss')";
            if (string.IsNullOrWhiteSpace(userName) == false)
                where += " and USER_NAME LIKE '%" + userName.Replace('\'', ' ') + "%'";
            #endregion
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BLL.FW.BF_OP_LOG.Entity> list = BLL.FW.BF_OP_LOG.Instance.GetListPage<BLL.FW.BF_OP_LOG.Entity>(limit, page, order, where, param);
            int count = BLL.FW.BF_OP_LOG.Instance.GetCount(where, param);
            if (list == null)
            {
                list = new List<BLL.FW.BF_OP_LOG.Entity>();
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
        public ActionResult ExportFile(string content = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_操作日志_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

                List<object> param = new List<object>();
                #region 添加参数
                string where = "1=1";

                if (string.IsNullOrWhiteSpace(content) == false)
                    where += " and content LIKE '%" + content.Replace('\'', ' ') + "%'";
                #endregion

                var dt = BLL.FW.BF_OP_LOG.Instance.GetTableFields("LOG_TIME,LOG_LEVEL,IS_SUCCESS,SRC_IP,SRC_PORT,USER_NAME,CONTROLLER,ACTION,REQ_URL,CONTENT,DETAIL", where, param);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["LOG_TIME"].Caption = "日志时间";
                dt.Columns["LOG_LEVEL"].Caption = "等级";
                dt.Columns["IS_SUCCESS"].Caption = "成功";
                dt.Columns["SRC_IP"].Caption = "源IP";
                dt.Columns["SRC_PORT"].Caption = "源端口";
                dt.Columns["USER_NAME"].Caption = "登录名";
                dt.Columns["CONTROLLER"].Caption = "控制器名称";
                dt.Columns["ACTION"].Caption = "ACTION名称";
                dt.Columns["REQ_URL"].Caption = "请求URL地址";
                dt.Columns["CONTENT"].Caption = "请求内容";
                dt.Columns["DETAIL"].Caption = "源IP";

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