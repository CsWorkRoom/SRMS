using CS.Base.DBHelper;
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
    public class AfScriptTaskFlowNodeLogController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int nodeId = 0, int TaskId = 0)
        {
            ViewBag.nodeId = nodeId;
            ViewBag.taskId = TaskId;
            BF_ST_TASK_FLOW_NODE.Entity entity = BF_ST_TASK_FLOW_NODE.Instance.GetEntity<BF_ST_TASK_FLOW_NODE.Entity>("TASK_ID=? AND NODE_ID=?", TaskId, nodeId);
            if (entity != null && string.IsNullOrWhiteSpace(entity.CODE) == false)
            {
                ViewBag.CODE = entity.CODE.Replace("<", "&lt;").Replace(">", "&gt;");
            }
            else
            {
                ViewBag.CODE = "未查到代码";
            }
            return View();
        }

        #region 查看节点日志

        //加载日志列表
        public string GetList(int page, int limit, int nodeId = 0, int taskId = 0, string message = "", string sql = "", string orderByField = "STFNL.ID", string orderByType = "DESC")
        {
            int count = 0;
               DataTable data = BF_ST_NODE.Instance.GetLogDataTable(limit, page, taskId, nodeId, ref count, message, sql, orderByField, orderByType);
            return ToJsonString(data, count);
        }
        #endregion

    }
}