using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace CS.WebUI.Controllers.FW
{
    public class AfScriptTaskController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.NodeName = BLL.FW.BF_ST_NODE.Instance.GetDictionary("ID", "NAME");
            ViewBag.FlowName = BLL.FW.BF_ST_FLOW.Instance.GetDictionary("ID", "NAME");
            ViewBag.UserName = BLL.FW.BF_USER.Instance.GetDictionary("ID", "NAME");
            DateTime begin = DateTime.Today.AddDays(-DateTime.Today.Day);
            begin = begin.AddDays(1 - begin.Day);
            ViewBag.DATE_RANGE = begin.ToString("yyyy-MM-dd") + " - " + DateTime.Today.ToString("yyyy-MM-dd");

            GetStatus();//运行状态
            return View();
        }

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="CONTENT"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string daterange = "", string flowName = "", string nodeName = "", string param = "", string statusId = "", string successId = "", string orderByField = "st.ID", string orderByType = "DESC")
        {
            int count = 0;
            DataTable list = BLL.FW.BF_ST_TASK.Instance.GetDataTable(limit, page, ref count, daterange, flowName, nodeName, param, statusId, successId, orderByField, orderByType);
            return ToJsonString(list, count);
        }
        #endregion

        #region 启动
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Start(int id = 0)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "节点停止失败";
            string message = "";
            if (id > 0)
            {
                result.IsSuccess = BF_ST_TASK.Instance.Start(id, ref message);
                result.Message = message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Stop(int id = 0)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "节点停止失败";
            string message = "";
            if (id > 0)
            {
                result.IsSuccess = BF_ST_TASK.Instance.Stop(id, ref message);
                result.Message = message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 运行状态
        private void GetStatus()
        {
            #region 状态
            IDictionary dic = new Dictionary<int, string>();
            foreach (Enums.RunStatus source in Enum.GetValues(typeof(Common.FW.Enums.RunStatus)))
                dic.Add((int)source, source.ToString());
            ViewBag.Status = dic;
            #endregion
        }
        #endregion

        #region 导出日志文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string daterange = "", string flowName = "", string nodeName = "", string param = "", string statusId = "", string successId = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_脚本任务_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

                int count = 0;
                var dt = BF_ST_TASK.Instance.GetDataTable(0, 0, ref count, daterange, flowName, nodeName, param, statusId, successId);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns.Remove("ID");
                dt.Columns["FLOWNAME"].Caption = "流程名称";
                dt.Columns["NODENAME"].Caption = "节点名称";
                dt.Columns["IS_MANUAL"].Caption = "手动启动";
                dt.Columns["REFERENCE_DATE"].Caption = "基准日期";
                dt.Columns["RETRY_TIMES"].Caption = "重试次数";
                dt.Columns["RUN_STATUS"].Caption = "运行状态";
                dt.Columns["IS_SUCCESS"].Caption = "是否成功";
                dt.Columns["START_TIME"].Caption = "开始时间";
                dt.Columns["FINISH_TIME"].Caption = "结束时间";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                dt.Columns["REMARK"].Caption = "备注";

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

        #region  查看任务
        Dictionary<int, string> dicNodeInfo = null;
        /// <summary>
        /// 根据ID取名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetNodeNameByID(int id)
        {
            if (dicNodeInfo == null)
            {
                dicNodeInfo = BLL.FW.BF_ST_NODE.Instance.GetDictionary("ID", "NAME");
            }
            if (dicNodeInfo.ContainsKey(id))
            {
                return dicNodeInfo[id];
            }
            return "未知";
        }

        public ActionResult look(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.TASK_ID = id;
            //渠道类型信息
            DataTable dt = BLL.FW.BF_ST_TYPE.Instance.GetTable();
            if (dt == null) ViewBag.ScriptTypeSelect = "[]";
            else
            {
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
                ViewBag.ScriptTypeSelect = SerializeObject(obj);
            }
            //获取
            IList<BLL.FW.BF_ST_TASK_FLOW_NODE.Entity> EntityTaskFlowNode = new List<BLL.FW.BF_ST_TASK_FLOW_NODE.Entity>();
            //
            BLL.FW.BF_ST_FLOW.Entity entity = new BLL.FW.BF_ST_FLOW.Entity();
            if (id > 0)
            {
                var StTask = BLL.FW.BF_ST_TASK.Instance.GetEntityByKey<BLL.FW.BF_ST_TASK.Entity>(id);
                if (StTask == null)
                {
                    ViewBag.Message = "脚本流不存在！";
                    entity.ID = -1;
                }
                else
                {
                    //当脚本流ID大于0时，默认为脚本流
                    if (StTask.FLOW_ID > 0)
                    {
                        entity = BLL.FW.BF_ST_FLOW.Instance.GetEntityByKey<BLL.FW.BF_ST_FLOW.Entity>(StTask.FLOW_ID);
                        if (entity == null)
                        {
                            ViewBag.Message = "流程不存在";
                            entity.ID = -1;
                        }
                    }
                    EntityTaskFlowNode = BLL.FW.BF_ST_TASK_FLOW_NODE.Instance.GetList<BLL.FW.BF_ST_TASK_FLOW_NODE.Entity>(" TASK_ID = ? ", StTask.ID);
                    var _r = EntityTaskFlowNode.Select(x => new
                    {
                        ID = x.ID,
                        FLOW_ID = x.FLOW_ID,
                        NODE_ID = x.NODE_ID,
                        PRE_NODE_IDS = x.PRE_NODE_IDS,
                        DIV_X = x.DIV_X,
                        DIV_Y = x.DIV_Y,
                        RUN_STATUS = x.RUN_STATUS,
                        IS_SUCCESS = x.IS_SUCCESS,
                        NODE_NAME = GetNodeNameByID(x.NODE_ID)

                    });
                    ViewBag.ScriptFlowNode = SerializeObject(_r);
                }
            }
            else
                ViewBag.ScriptFlowNode = SerializeObject(EntityTaskFlowNode);

            return View(entity);
        }
        /// <summary>
        /// 获取任务实例节点当前执行情况
        /// </summary>
        /// <param name="TaskId">任务实例</param>
        /// <param name="NodeId">任务实例节点</param>
        /// <returns></returns>
        public string TaskNodeShow(int TaskId = 0, int NodeId = 0)
        {
            if (TaskId > 0 && NodeId > 0)
            {
                var _r = BLL.FW.BF_ST_TASK_FLOW_NODE.Instance.GetEntity<BLL.FW.BF_ST_TASK_FLOW_NODE.Entity>(" TASK_ID = ? AND NODE_ID = ?", TaskId, NodeId);
                if (_r == null) return "[]";
                var _task = BLL.FW.BF_ST_TASK.Instance.GetEntity<BLL.FW.BF_ST_TASK.Entity>(" ID = ?", TaskId);
                if (_task == null) return "[]";
                var IsEidt = false;
                if (_r.RUN_STATUS == (int)Common.FW.Enums.RunStatus.结束 && _r.IS_SUCCESS == 0)
                    IsEidt = true;
                else if (_task.RUN_STATUS == (int)Common.FW.Enums.RunStatus.结束 && _r.RUN_STATUS == (int)Common.FW.Enums.RunStatus.等待)
                    IsEidt = true;
                return SerializeObject(new
                {
                    IsEidt = IsEidt,
                    data = _r
                });
            }
            return "[]";
        }
        /// <summary>
        /// 重新执行任务实例节点
        /// </summary>
        /// <param name="reference_date">基准时间</param>
        /// <param name="content">任务实例节点执行脚本</param>
        /// <param name="taskId">任务实例ID</param>
        /// <param name="nodeId">任务实例节点ID</param>
        /// <returns>执行情况</returns>
        public string TaskNodeRestart(DateTime reference_date, string content = "", int taskId = 0, int nodeId = 0)
        {
            JsonResultData result = new JsonResultData();
            if (taskId > 0 && nodeId > 0)
            {
                int i = BF_ST_TASK_FLOW_NODE.Instance.ReStart(taskId, nodeId, content);
                if (i > 0)
                {
                    result.IsSuccess = true;
                    result.Message = "重启成功!";
                    return SerializeObject(result);
                }
            }
            result.IsSuccess = false;
            result.Message = "任务节点无法重启,参数错误";
            return SerializeObject(result);
        }
        #endregion
    }
}