using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    public class AfScriptFlowController : ABaseController
    {
        #region 列表页面
        // GET: AfScriptFlow
        public ActionResult Index(int self = 0)
        {
            ViewBag.SHOW_SELF = self;
            //脚本流类型
            var ST_TYPE = BLL.FW.BF_ST_TYPE.Instance.GetDictionary("ID", "NAME");
            //字段数据类型
            Dictionary<int, string> dicRunState = new Dictionary<int, string>();
            foreach (Enums.RunStatus source in Enum.GetValues(typeof(Enums.RunStatus)))
            {
                dicRunState.Add((int)source, source.ToString());
            }
            ViewBag.ST_TYPE = ST_TYPE;
            ViewBag.RUN_STATE = dicRunState;

            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <param name="cron"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", int runState = -1, int isEnable = -1, int lastTaskIs = -1, int self = 0, string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (!string.IsNullOrWhiteSpace(name))
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            if (runState > -1)
            {
                where += " AND RUN_STATUS = " + runState;
            }
            if (isEnable > -1)
            {
                where += " AND IS_ENABLE = " + isEnable;
            }
            if (lastTaskIs > -1)
            {
                where += " AND LAST_TASK_IS =" + lastTaskIs;
            }
            if (self > 0)
            {
                where += " AND CREATE_UID = " + SystemSession.UserID;
            }

            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BLL.FW.BF_ST_FLOW.Entity> list = BLL.FW.BF_ST_FLOW.Instance.GetListPage<BLL.FW.BF_ST_FLOW.Entity>(limit, page, order, where);
            int count = BLL.FW.BF_ST_FLOW.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BLL.FW.BF_ST_FLOW.Entity>();
                count = 0;
            }
            return ToJsonString(list, count);
        }
        #endregion

        #region  编辑页面
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
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
            IList<BLL.FW.BF_ST_FLOW_NODE.Entity> EntityFlowNode = new List<BLL.FW.BF_ST_FLOW_NODE.Entity>();
            //
            BLL.FW.BF_ST_FLOW.Entity entity = new BLL.FW.BF_ST_FLOW.Entity();
            entity.OFFSET = -1;
            if (id > 0)
            {
                entity = BLL.FW.BF_ST_FLOW.Instance.GetEntityByKey<BLL.FW.BF_ST_FLOW.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "流程不存在";
                    entity = new BLL.FW.BF_ST_FLOW.Entity();
                    entity.ID = -1;
                }

                EntityFlowNode = BLL.FW.BF_ST_FLOW_NODE.Instance.GetList<BLL.FW.BF_ST_FLOW_NODE.Entity>(" FLOW_ID = ?", id);
            }

            ViewBag.ScriptFlowNode = SerializeObject(EntityFlowNode);
            return View(entity);
        }
        /// <summary>
        /// 编辑页面提交
        /// </summary>
        /// <param name="entity">对象集合</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BLL.FW.BF_ST_FLOW.Entity entity, FormCollection collection)
        {
            //
            JsonResultData result = new JsonResultData();
            //验证
            EntityValiable(entity, ref result);
            if (result.IsSuccess == false)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //
            var FLowId = entity.ID;
            string FlowNodeStr = Request["dicFlowNode"];
            //
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("NAME", entity.NAME);
            dic.Add("TYPE_ID", entity.TYPE_ID);
            dic.Add("CRON", entity.CRON);
            dic.Add("OFFSET", entity.OFFSET);
            dic.Add("PARAMETERS", entity.PARAMETERS);
            dic.Add("RETRY_TIMES", entity.RETRY_TIMES);
            dic.Add("REMARK", entity.REMARK);
            dic.Add("UPDATE_UID", BLL.FW.SystemSession.UserID);
            dic.Add("UPDATE_TIME", DateTime.Now);
            if (entity.ID > 0)
            {
                var r = BLL.FW.BF_ST_FLOW.Instance.UpdateByKey(dic, entity.ID);
                if (r > 0)
                {
                    result.IsSuccess = true;
                    result.Message = "更新成功!";
                }
            }
            else
            {
                dic.Add("IS_ENABLE", 1);
                dic.Add("CREATE_UID", BLL.FW.SystemSession.UserID);
                dic.Add("CREATE_TIME", DateTime.Now);
                var r = BLL.FW.BF_ST_FLOW.Instance.Add(dic, true);
                if (r > 0)
                {
                    result.IsSuccess = true;
                    result.Message = "添加成功!";
                    FLowId = entity.ID;

                }
            }
            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(FlowNodeStr))
                {
                    //更新节点 先删除
                    BLL.FW.BF_ST_FLOW_NODE.Instance.Delete(" FLOW_ID = ?", FLowId);
                    //
                    var _FlowNode = DeserializeObject<List<BLL.FW.BF_ST_FLOW_NODE.Entity>>(FlowNodeStr);
                    List<Dictionary<string, object>> dicNodeList = new List<Dictionary<string, object>>();
                    foreach (var item in _FlowNode)
                    {
                        Dictionary<string, object> dicNode = new Dictionary<string, object>();
                        dicNode.Add("FLOW_ID", FLowId);
                        dicNode.Add("NODE_ID", item.NODE_ID);
                        dicNode.Add("PRE_NODE_IDS", item.PRE_NODE_IDS);
                        dicNode.Add("DIV_X", item.DIV_X);
                        dicNode.Add("DIV_Y", item.DIV_Y);

                        dicNodeList.Add(dicNode);
                    }
                    var r = BLL.FW.BF_ST_FLOW_NODE.Instance.LoadDataInList(dicNodeList);
                    if (r == _FlowNode.Count)
                    {
                        result.IsSuccess = true;
                        result.Message = "更新条数";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format("更新条数不符合,需加入节点数:{0},导入节点数:{1}", _FlowNode.Count, r);
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 验证提交值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="result"></param>
        private void EntityValiable(BLL.FW.BF_ST_FLOW.Entity entity, ref JsonResultData result)
        {
            result.IsSuccess = true;

            List<string> Message = new List<string>();
            if (string.IsNullOrEmpty(entity.NAME))
            {
                Message.Add("脚本流名称不能为空!");
            }
            else
            {
                var _IsDuplicate = BLL.FW.BF_ST_FLOW.Instance.IsDuplicate(entity.ID, "NAME", entity.NAME);
                if (_IsDuplicate == true)
                {
                    Message.Add("脚本流名称重复!");
                }
            }
            if (entity.TYPE_ID == 0)
            {
                Message.Add("脚本流类型不能为空!");
            }
            if (string.IsNullOrEmpty(entity.CRON))
            {
                Message.Add("时间表达式不能为空!");
            }
            if (Message.Count > 0)
            {
                result.IsSuccess = false;
                result.Message = string.Join("<br/>", Message);
                return;
            }
        }

        /// <summary>
        /// 编辑按钮-》获取类型树结构
        /// </summary>
        /// <returns></returns>
        public string SciptFlowZtree()
        {
            var Typedt = BLL.FW.BF_ST_TYPE.Instance.GetTable();
            if (Typedt == null) return "[]";
            var ScriptNodedt = BLL.FW.BF_ST_NODE.Instance.GetTable();
            if (ScriptNodedt == null) return "[]";
            var objList = new List<object>();
            foreach (DataRow dr in Typedt.Rows)
            {
                objList.Add(
                    new
                    {
                        id = "Type_" + Convert.ToInt32(dr["ID"]),
                        pId = "Type_" + Convert.ToInt32(dr["PID"]),
                        name = dr["NAME"].ToString(),
                        dataType = "Type",
                        iconSkin = "zclose",
                    });
            }

            foreach (DataRow dr in ScriptNodedt.Rows)
            {
                objList.Add(
                new
                {
                    id = Convert.ToInt32(dr["ID"]),
                    pId = "Type_" + Convert.ToInt32(dr["TYPE_ID"]),
                    name = dr["NAME"].ToString(),
                    dataType = "script",
                    iconSkin = "zdocu"
                });
            }

            return SerializeObject(objList);
        }
        #endregion

        #region 启用与禁用
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
                int i = BF_ST_FLOW.Instance.SetEnable(id);
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
                int i = BF_ST_FLOW.Instance.SetUnable(id);
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
        #endregion

        #region 启动与停用
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="id">脚本流Id标识</param>
        /// <param name="sdate">基准日期</param>
        /// <param name="bdate">开始日期</param>
        /// <param name="edate">结束日期</param>
        /// <param name="para">可选参数</param>
        /// <returns>消息信息</returns>
        public ActionResult Start(int id = 0, string sdate = "", string bdate = "", string edate = "", string para = "")
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "脚本流启动失败";
            string message = "";
            try
            {
                if (id <= 0)
                {
                    throw new Exception("任务组ID不正确");
                }
                if (string.IsNullOrWhiteSpace(sdate))
                {
                    throw new Exception("基准日期不可为空");
                }
                if (string.IsNullOrWhiteSpace(bdate) || string.IsNullOrWhiteSpace(edate))
                {
                    throw new Exception("起止日期不可为空");
                }

                DateTime referenceDate = DateTime.Parse(sdate);
                DateTime beginDate = DateTime.Parse(bdate);
                DateTime endDate = DateTime.Parse(edate);
                if (beginDate.Year != endDate.Year || beginDate.Month != endDate.Month)
                {
                    throw new Exception("任务不可以跨月");
                }
                int i = 0;
                if (string.IsNullOrWhiteSpace(para))
                {
                    result.IsSuccess = BF_ST_FLOW.Instance.Start(id, referenceDate, beginDate, endDate, para, ref message);
                    result.Message = message;
                    i = result.IsSuccess ? 1 : 0;
                }
                else
                {
                    int n = 0;
                    foreach (string p in para.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        n++;
                        i += BF_ST_FLOW.Instance.Start(id, referenceDate, beginDate, endDate, p, ref message) ? 1 : 0;
                    }
                    result.IsSuccess = i > 0;
                    result.Message = "根据参数创建了" + i + "个子任务";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.ToString();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
                result.IsSuccess = BF_ST_FLOW.Instance.Stop(id, ref message);
                result.Message = message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 导出日志文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string name = "", int runState = -1, int isEnable = -1, int lastTaskIs = -1, int self = 0)
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_脚本任务_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), System.Text.Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

                string where = "1=1";
                List<object> param = new List<object>();

                #region 添加参数
                if (!string.IsNullOrWhiteSpace(name))
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                if (runState > -1)
                {
                    where += " AND RUN_STATUS = " + runState;
                }
                if (isEnable > -1)
                {
                    where += " AND IS_ENABLE = " + isEnable;
                }
                if (lastTaskIs > -1)
                {
                    where += " AND LAST_TASK_IS =" + lastTaskIs;
                }
                if (self > 0)
                {
                    where += " AND CREATE_UID = " + SystemSession.UserID;
                }

                #endregion
                var dt = BLL.FW.BF_ST_FLOW.Instance.GetTableFields("ID,NAME,TYPE_ID,CRON,RETRY_TIMES,IS_ENABLE,RUN_STATUS,LAST_TASK_IS,LAST_TASK_ID", where, param);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns.Remove("ID");
                dt.Columns["NAME"].Caption = "脚本流名称";
                dt.Columns["TYPE_ID"].Caption = "脚本流类型";
                dt.Columns["CRON"].Caption = "时间表达式";
                dt.Columns["RETRY_TIMES"].Caption = "失败重试次数";
                dt.Columns["IS_ENABLE"].Caption = "是否启用";
                dt.Columns["RUN_STATUS"].Caption = "运行状态";
                dt.Columns["LAST_TASK_IS"].Caption = "最新任务是否成功";
                dt.Columns["LAST_TASK_ID"].Caption = "最新任务ID";

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

        #region
        /// <summary>
        /// 查看最新节点日志
        /// </summary>
        /// <param name="id">BF_ST_FLOW的ID</param>
        /// <returns>视图跳转</returns>
        public ActionResult Look(int id = 0)
        {
            if (id > 0)
            {
                var _r = BLL.FW.BF_ST_FLOW.Instance.GetEntity<BLL.FW.BF_ST_FLOW.Entity>(" ID = ?", id);
                if (_r == null) return RedirectToAction("look", "AfScriptTask");
                //Dictionary<string, object> dic = new Dictionary<string, object>();
                //dic.Add("id", _r.LAST_TASK_ID);
                //return RedirectToAction("look", "AfScriptTask");
                return Redirect("~/AfScriptTask/look?id=" + _r.LAST_TASK_ID);
            }
            return RedirectToAction("look", "AfScriptTask");
        }
        #endregion

    }
}