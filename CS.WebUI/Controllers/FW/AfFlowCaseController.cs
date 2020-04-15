using CS.BLL.FW;
using CS.Common;
using CS.WebUI.Models.Flow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using NPOI.SS.Formula.Functions;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 流程流转管理
    /// </summary>
    public class AfFlowCaseController : ABaseController
    {
        public string Modular = "流程流转管理";

        #region 流程实例触发发起
        public ActionResult Create(int flowId = 0,int Id=0,string auditType="")
        {
            ViewBag.MainType = auditType;
            ViewBag.MainId = Id;
            //声明系统流程实例
            Models.FW.FlowCaseModel sysFlow = new Models.FW.FlowCaseModel();

            var flow = BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(flowId);
            if (flow == null)
            {
                return ShowAlert(string.Format(@"未找到编号为[{0}]的流程项目", flowId));
            }
            else
            {
                if (flow.IS_ENABLE == 0)
                {
                    return ShowAlert(string.Format(@"编号为[{0}]的流程项目未启用,若需要请联系管理员处理。", flowId));
                }
                else
                {
                    sysFlow.SysCsFlowID = flow.ID;
                    sysFlow.SysCsFlowTypeID = flow.FLOW_TYPE_ID;
                    sysFlow.SysCsFlowName = flow.NAME;
                    string mainPage = flow.MAIN_PAGE.Split(';')[0];
                    mainPage = mainPage.Replace("=0", "=" + Id);
                    sysFlow.SysCsMainPage = ApplicationPath + mainPage;
                    sysFlow.SysCsMainTable = flow.MAIN_TABLE;
                    sysFlow.SysCsRemark = flow.REMARK;
                    sysFlow.sysCsIsEnable = flow.IS_ENABLE;
                    sysFlow.SysCsMainFun = flow.MAIN_FUN;//主函数
                }
            }
            #region 验证是否具有发起流程表单的权限(暂未实现)
            //BF_FLOW_NODE获得主节点，验证用户权限。
            #endregion
            return View(sysFlow);
        }

        [HttpPost]
        public ActionResult Create(int SysCsFlowID, int sysCsMainTableKey,string flowCaseName,string k="")
        {
            //考虑限定开始节点是主节点，并且只能有一个？

            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            try
            {
                //流程
                var flow = BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(SysCsFlowID);
                //主节点
                var mainNode = BF_FLOW_NODE.Instance.GetEntity<BF_FLOW_NODE.Entity>("FLOW_ID=? AND IS_MAIN=1", SysCsFlowID);
                #region 01.保存流程实例
                int flowCaseId = BF_FLOW_CASE.Instance.GetNextValueFromSeqDef();
                BF_FLOW_CASE.Entity flowCase = new BF_FLOW_CASE.Entity()
                {
                    ID = flowCaseId,
                    CREATE_TIME = DateTime.Now,
                    CREATE_UID = SystemSession.UserID,
                    FLOW_ID = flow.ID,
                    FLOW_TYPE_ID = flow.FLOW_TYPE_ID,
                    IS_ARCHIVE = 0,
                    IS_ENABLE = 1,
                    MAIN_PAGE = ((flow.MAIN_PAGE.IndexOf('?') >= 0) ? (flow.MAIN_PAGE + "&id=" + sysCsMainTableKey) : (flow.MAIN_PAGE + "?id=" + sysCsMainTableKey)).Replace("id=0&", ""),
                    MAIN_TABLE = flow.MAIN_TABLE,
                    PRIMARY_KEY = sysCsMainTableKey,
                    FLOW_CASE_NAME = flowCaseName,
                    NAME = flow.NAME
                };
                BF_FLOW_CASE.Instance.Add(flowCase);
                #endregion

                #region 02.保存主节点实例
                #region 02-1.保存主节点(是否需要记录填报人信息)
                int mainFlowNodeCaseId = BF_FLOW_NODE_CASE.Instance.AddMainFlowNodeCase(mainNode, flowCaseId, flow.ID);
                #endregion

                #region 02-2.保存主节点处理(填报)人信息
                BF_FLOW_NODE_CASE_RECORD.Entity mainFlowNodeCaseRecord = new BF_FLOW_NODE_CASE_RECORD.Entity
                {
                    ID = BF_FLOW_NODE_CASE_RECORD.Instance.GetNextValueFromSeqDef(),
                    AUDIT_CONTENT = string.Format(@"[{0}]发起的主节点填报信息，该条信息仅记录，不存在审批", SystemSession.FullUserName),
                    AUDIT_STATUS = Convert.ToInt16(CS.Common.Enums.AuditStatus.通过.GetHashCode()),//默认通过1
                    AUDIT_TIME = DateTime.Now,
                    AUDIT_UID = SystemSession.UserID,//审批人即填报人
                    FLOW_ID = flow.ID,
                    FLOW_CASE_ID = flowCaseId,
                    FLOW_NODE_CASE_ID = mainFlowNodeCaseId
                };
                #endregion
                #endregion

                #region 03.生成（下个）节点待办信息.
                var isSuccess = BF_FLOW_CASE.Instance.CreateNextNodesCase(flow, mainNode, flowCaseId);
                #endregion

                result.IsSuccess = true;
                result.Message = "流程实例创建成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "流程实例创建出错：" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region 流程流转
        public ActionResult Deal(int flowNodeCaseId)
        {
            JsonResultData result = new JsonResultData();
            //当前待办节点实例
            var nodeCase = BF_FLOW_NODE_CASE.Instance.GetEntityByKey<BF_FLOW_NODE_CASE.Entity>(flowNodeCaseId);
            //当前流程实例
            var flowCase = BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(nodeCase.FLOW_CASE_ID);
            //已处理通过的节点实例集合
            var dealNodeCaseList = BF_FLOW_NODE_CASE.Instance.GetList<BF_FLOW_NODE_CASE.Entity>("FLOW_CASE_ID=? AND AUDIT_STATUS=?",
                flowCase.ID,
                CS.Common.Enums.AuditStatus.通过.GetHashCode()).OrderBy(p => p.ID);

            //表单主信息（MAIN_PAGE）
            //流转记录（暂未实现）
            //审核页面
            var record = new BF_FLOW_NODE_CASE_RECORD.Entity()
            {
                FLOW_ID = flowCase.FLOW_ID,
                FLOW_CASE_ID = flowCase.ID,
                FLOW_NODE_CASE_ID = flowNodeCaseId,
                FLOW_NODE_ID = nodeCase.FLOW_NODE_ID
            };

            ViewBag.AuditStatusList = new List<KV> {
                new KV { key=CS.Common.Enums.AuditStatus.通过.ToString(),value=CS.Common.Enums.AuditStatus.通过.GetHashCode().ToString()},
                new KV { key=CS.Common.Enums.AuditStatus.退回.ToString(),value=CS.Common.Enums.AuditStatus.退回.GetHashCode().ToString()}
            };
            string[] pages = flowCase.MAIN_PAGE.Split(';');
            if (pages.Length > 1)
            {
                ViewBag.MainFormPage = pages[1];//表单页面
            }
            else
            {
                ViewBag.MainFormPage = pages[0];//表单页面  
            }
            #region 审核流转记录
            //获取流转flowcase集合，表示流转了几次
          //  var allFlowCaseList = BF_FLOW_CASE.Instance.GetList<BF_FLOW_CASE.Entity>("FLOW_ID=? AND MAIN_PAGE=?", nodeCase.FLOW_ID, flowCase.MAIN_PAGE).OrderBy(p => p.ID);
            var allFlowCaseList = BF_FLOW_CASE.Instance.GetList<BF_FLOW_CASE.Entity>(" MAIN_TABLE=? AND PRIMARY_KEY=?", flowCase.MAIN_TABLE.ToUpper(), flowCase.PRIMARY_KEY).OrderBy(p => p.ID);
            var newFlowCaseList = allFlowCaseList.Select(e => new FlowCaseNewModel()
            {
                ID = e.ID,
                NAME = e.NAME,
                CREATE_TIME = e.CREATE_TIME,
                FlowNodeModes = GetFlowNodeList(e.ID, flowCase.FLOW_ID)
            });
            ViewBag.dealNodeCaseAllList = newFlowCaseList.ToList();
            #endregion

            return View(record);
        }
        

        /// <summary>
        ///  流转记录
        /// </summary>
        /// <param name="mainTable"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public ActionResult DealFlow(string mainTable ,int primaryKey)
        {
            #region 审核流转记录
            //获取流转flowcase集合，表示流转了几次
            var allFlowCaseList = BF_FLOW_CASE.Instance.GetList<BF_FLOW_CASE.Entity>(" MAIN_TABLE=? AND PRIMARY_KEY=?",  mainTable.ToUpper(), primaryKey).OrderBy(p => p.ID);
            var newFlowCaseList = allFlowCaseList.Select(e => new FlowCaseNewModel()
            {
                ID = e.ID,
                NAME = e.NAME,
                CREATE_TIME = e.CREATE_TIME,
                FlowNodeModes = GetFlowNodeList(e.ID, e.FLOW_ID)
            });
            ViewBag.dealNodeCaseAllList = newFlowCaseList.ToList();
            #endregion


            return View();
        }

        /// <summary>
        /// 获取摸个流程下的所有节点
        /// </summary>
        /// <param name="flowCaseId"></param>
        /// <param name="flowId"></param>
        private List<FlowNodeModel> GetFlowNodeList(int flowCaseId, int flowId)
        {
            //查询当前流程所有的节点
            var dealNodeAllList = BF_FLOW_NODE.Instance.GetList<BF_FLOW_NODE.Entity>("FLOW_ID=?",
                flowId).OrderBy(p => p.ID);
            var dealRecord = dealNodeAllList.Select(e => new FlowNodeModel()
            {
                ID = e.ID,
                NAME =e.NAME,
                IS_MAIN = e.IS_MAIN,
                DEAL_WAY = e.DEAL_WAY,
                FlowNodes = GetNodeCaseRecode(e.ID, flowCaseId)
            });
            return dealRecord.ToList();
        }

        /// <summary>
        /// 返回摸个节点的处理情况
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="flowCaseId"></param>
        /// <returns></returns>
        private List<FlowNodeCaseModel> GetNodeCaseRecode(int nodeId, int flowCaseId)
        {
            var nodeCase = BF_FLOW_NODE_CASE.Instance.GetList<BF_FLOW_NODE_CASE.Entity>("FLOW_CASE_ID=? AND FLOW_NODE_ID=?",
                 flowCaseId, nodeId).FirstOrDefault();
            if (nodeCase != null)
            {
                string uid = nodeCase.USER_IDS;
                var userList = BF_USER.Instance.GetList<BF_USER.Entity>("ID IN("+uid+")" );
                var newUserList = userList.Select(e => new FlowNodeCaseModel()
                {
                    Uid = e.ID,
                    UserName = string.Format("{0}({1})",e.FULL_NAME,e.NAME),
                    FlowNodeCaseRecords = BF_FLOW_NODE_CASE_RECORD.Instance.GetList<BF_FLOW_NODE_CASE_RECORD.Entity>("FLOW_NODE_CASE_ID=? AND AUDIT_UID=?", nodeCase.ID, e.ID).ToList()
                });
                return newUserList.ToList();
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult Deal(BF_FLOW_NODE_CASE_RECORD.Entity record)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;

            try
            {
                var flow = BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(record.FLOW_ID);
                var currFlowNode = BF_FLOW_NODE.Instance.GetEntityByKey<BF_FLOW_NODE.Entity>(record.FLOW_NODE_ID);
                #region 01.保存审批记录
                record.AUDIT_TIME = DateTime.Now;
                record.AUDIT_UID = SystemSession.UserID;
                BF_FLOW_NODE_CASE_RECORD.Instance.Add(record);
                #endregion

                bool isPass = false;
                #region 02.修改当前节点实例状态
                var flowNodeCase = BF_FLOW_NODE_CASE.Instance.GetEntityByKey<BF_FLOW_NODE_CASE.Entity>(record.FLOW_NODE_CASE_ID);
                if (record.AUDIT_STATUS == CS.Common.Enums.AuditStatus.退回.GetHashCode())
                {
                    flowNodeCase.AUDIT_STATUS = (short)CS.Common.Enums.AuditStatus.退回.GetHashCode();
                    flowNodeCase.IS_FINISH = 1;
                    flowNodeCase.FINISH_TIME = DateTime.Now;
                    BF_FLOW_NODE_CASE.Instance.UpdateByKey(flowNodeCase, flowNodeCase.ID);

                    //退回流程至流程主节点
                    //BF_FLOW_NODE_CASE.Instance.ReturnFlowNodeCase(record.FLOW_NODE_ID, record.FLOW_CASE_ID);
                    //修改原表的状态信息
                    var flowCase = BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(flowNodeCase.FLOW_CASE_ID);
                    BF_FLOW_CASE.Instance.UpdateMainTableState(flowCase.PRIMARY_KEY, flowCase.MAIN_TABLE, "0", "2");
                }
                else
                {
                    //验证是否多人全部审批通过
                    if (BF_FLOW_NODE_CASE.Instance.ValidateFlowNodeCaseIsFinish(record.FLOW_NODE_CASE_ID))
                    {
                        flowNodeCase.AUDIT_STATUS = (short)CS.Common.Enums.AuditStatus.通过.GetHashCode();
                        flowNodeCase.IS_FINISH = 1;
                        flowNodeCase.FINISH_TIME = DateTime.Now;
                        BF_FLOW_NODE_CASE.Instance.UpdateByKey(flowNodeCase, flowNodeCase.ID);

                        isPass = true;
                    }
                }
                #endregion

                #region 03.触发下个节点流转
                if (isPass)
                {
                    var resBool = BF_FLOW_CASE.Instance.CreateNextNodesCase(flow, currFlowNode, record.FLOW_CASE_ID);      
                }
                #endregion

                result.Message = "流程处理成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "流程处理出错：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 流程关联创建下个流程的首次处理
        /// <summary>
        /// 关联流程：首个节点处理页
        /// </summary>
        /// <param name="flowCaseId">流程实例ID</param>
        /// <param name="FirstFlowNodeCaseId">流程首个节点实例ID</param>
        /// <returns></returns>
        public ActionResult ProcessCreate(int flowCaseId = 0,int FirstFlowNodeCaseId=0)
        {
            int flowId = 0;
            #region 获得流程编号
            var flowCase = BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(flowCaseId);
            if (flowCase != null && flowCase.FLOW_ID > 0)
            {
                flowId = flowCase.FLOW_ID;
            }
            else
            {
                return ShowAlert(string.Format(@"未找到编号为[{0}]的流程项目实例", flowCaseId));
            }
            #endregion
            //声明系统流程实例
            Models.FW.FlowCaseModel sysFlow = new Models.FW.FlowCaseModel();

            var flow = BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(flowId);
            if (flow == null)
            {
                return ShowAlert(string.Format(@"未找到编号为[{0}]的流程项目", flowId));
            }
            else
            {
                if (flow.IS_ENABLE == 0)
                {
                    return ShowAlert(string.Format(@"编号为[{0}]的流程项目未启用,若需要请联系管理员处理。", flowId));
                }
                else
                {
                    sysFlow.SysCsFlowID = flow.ID;
                    sysFlow.SysCsFlowTypeID = flow.FLOW_TYPE_ID;
                    sysFlow.SysCsFlowName = flow.NAME;
                    sysFlow.SysCsMainPage = flow.MAIN_PAGE;
                    sysFlow.SysCsMainTable = flow.MAIN_TABLE;
                    sysFlow.SysCsRemark = flow.REMARK;
                    sysFlow.sysCsIsEnable = flow.IS_ENABLE;
                    sysFlow.SysCsMainFun = flow.MAIN_FUN;//主函数

                    #region 关联流程的信息
                    sysFlow.SysCsFlowCaseID = flowCaseId;
                    sysFlow.SysCsFirstFlowNodeCaseID = FirstFlowNodeCaseId;
                    #endregion
                }
            }
            #region 验证是否具有发起流程表单的权限(暂未实现)
            //BF_FLOW_NODE获得主节点，验证用户权限。
            #endregion
            return View(sysFlow);
        }
        /// <summary>
        /// 关联流程：首个节点处理post
        /// </summary>
        /// <param name="SysCsFlowCaseID">流程实例ID</param>
        /// <param name="SysCsFirstFlowNodeCaseID">流程首个节点实例ID</param>
        /// <param name="sysCsMainTableKey">表单表返回的主键值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProcessCreate(int SysCsFlowCaseID, int SysCsFirstFlowNodeCaseID,int sysCsMainTableKey)
        {
            //限定开始节点是主节点，并且只能有一个
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            try
            {
                //流程实例
                var flowCase= BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(SysCsFlowCaseID);
                //流程
                var flow = BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(flowCase.FLOW_ID);

                #region 修改流程实例主键字段值
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("PRIMARY_KEY", sysCsMainTableKey);
                dic.Add("MAIN_PAGE", (sysCsMainTableKey > 0) ? ((flow.MAIN_PAGE.IndexOf('?') >= 0) ? (flow.MAIN_PAGE + "&id=" + sysCsMainTableKey) : (flow.MAIN_PAGE + "?id=" + sysCsMainTableKey)) : flow.MAIN_TABLE);
                BF_FLOW_CASE.Instance.UpdateByKey(dic, SysCsFlowCaseID);
                #endregion

                var firstFlowNodeCase= BF_FLOW_NODE_CASE.Instance.GetEntityByKey<BF_FLOW_NODE_CASE.Entity>(SysCsFirstFlowNodeCaseID);
                #region 修改主节点实例信息
                Dictionary<string, object> nodeDic = new Dictionary<string, object>();
                nodeDic.Add("AUDIT_STATUS", Convert.ToInt16(CS.Common.Enums.AuditStatus.通过.GetHashCode()));
                nodeDic.Add("DEAL_WAY", 1);
                nodeDic.Add("IS_FINISH", 1);
                nodeDic.Add("FINISH_TIME", DateTime.Now);
                BF_FLOW_NODE_CASE.Instance.UpdateByKey(nodeDic, SysCsFirstFlowNodeCaseID);
                #endregion

                #region 保存主节点处理(填报)人信息
                BF_FLOW_NODE_CASE_RECORD.Entity mainFlowNodeCaseRecord = new BF_FLOW_NODE_CASE_RECORD.Entity
                {
                    ID = BF_FLOW_NODE_CASE_RECORD.Instance.GetNextValueFromSeqDef(),
                    AUDIT_CONTENT = string.Format(@"关联流程：[{0}]发起的主节点填报信息，该条信息仅记录，不存在审批", SystemSession.FullUserName),
                    AUDIT_STATUS = Convert.ToInt16(CS.Common.Enums.AuditStatus.通过.GetHashCode()),//默认通过1
                    AUDIT_TIME = DateTime.Now,
                    AUDIT_UID = SystemSession.UserID,//审批人即填报人
                    FLOW_ID = flow.ID,
                    FLOW_CASE_ID = SysCsFlowCaseID,
                    FLOW_NODE_CASE_ID = SysCsFirstFlowNodeCaseID
                };
                #endregion
                #endregion

                //主节点
                var mainNode = BF_FLOW_NODE.Instance.GetEntityByKey<BF_FLOW_NODE.Entity>(firstFlowNodeCase.FLOW_NODE_ID);
                #region 03.生成（下个）节点待办信息.
                var isSuccess = BF_FLOW_CASE.Instance.CreateNextNodesCase(flow, mainNode, SysCsFlowCaseID);
                #endregion

                result.IsSuccess = true;
                result.Message = "关联流程：初始节点表单填报成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "关联流程：初始节点表单填报出错：" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //验证节点权限(暂未实现)
    }
}