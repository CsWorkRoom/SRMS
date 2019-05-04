using CS.Base.Log;
using CS.BLL.FW;
using CS.Common;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 流程流转管理
    /// </summary>
    public class AfFlowCaseController : ABaseController
    {
        public string Modular = "流程流转管理";

        #region 流程实例触发发起
        public ActionResult Create(int flowId = 0)
        {
            //声明系统流程实例
            Models.FW.FlowCaseModel sysFlow = new Models.FW.FlowCaseModel();

            var flow= BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(flowId);
            if(flow==null)
            {
                ShowAlert(string.Format(@"未找到编号为[{0}]的流程项目", flowId));
            }
            else
            {
                if(flow.IS_ENABLE==0)
                {
                    ShowAlert(string.Format(@"编号为[{0}]的流程项目未启用,若需要请联系管理员处理。", flowId));
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
                }
            }
            #region 验证是否具有发起流程表单的权限(暂未实现)
            //BF_FLOW_NODE获得主节点，验证用户权限。
            #endregion
            return View(sysFlow);
        }

        [HttpPost]
        public ActionResult Create(int SysCsFlowID ,int sysCsMainTableKey)
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
                    MAIN_PAGE = (flow.MAIN_PAGE.IndexOf('?') >= 0) ? (flow.MAIN_PAGE + "&id=" + sysCsMainTableKey) : (flow.MAIN_PAGE + "?id=" + sysCsMainTableKey),
                    MAIN_TABLE = flow.MAIN_TABLE,
                    PRIMARY_KEY = sysCsMainTableKey,
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
            //当前待办节点实例
            var nodeCase= BF_FLOW_NODE_CASE.Instance.GetEntityByKey<BF_FLOW_NODE_CASE.Entity>(flowNodeCaseId);
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
                FLOW_NODE_ID= nodeCase.FLOW_NODE_ID
            };

            ViewBag.AuditStatusList = new List<KV> {
                new KV { key=CS.Common.Enums.AuditStatus.通过.ToString(),value=CS.Common.Enums.AuditStatus.通过.GetHashCode().ToString()},
                new KV { key=CS.Common.Enums.AuditStatus.退回.ToString(),value=CS.Common.Enums.AuditStatus.退回.GetHashCode().ToString()}
            };
            ViewBag.MainFormPage = flowCase.MAIN_PAGE;//表单页面

            return View(record);
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
                BF_FLOW_NODE_CASE_RECORD.Instance.Add(record);
                #endregion

                bool isPass = false;
                #region 02.修改当前节点实例状态
                var flowNodeCase= BF_FLOW_NODE_CASE.Instance.GetEntityByKey<BF_FLOW_NODE_CASE.Entity>(record.FLOW_NODE_CASE_ID);
                if (record.AUDIT_STATUS == CS.Common.Enums.AuditStatus.退回.GetHashCode())
                {
                    flowNodeCase.AUDIT_STATUS = (short)CS.Common.Enums.AuditStatus.退回.GetHashCode();
                    flowNodeCase.IS_FINISH = 1;
                    flowNodeCase.FINISH_TIME = DateTime.Now;
                    BF_FLOW_NODE_CASE.Instance.UpdateByKey(flowNodeCase, flowNodeCase.ID);

                    //退回流程至流程主节点
                    BF_FLOW_NODE_CASE.Instance.ReturnFlowNodeCase(record.FLOW_NODE_ID, record.FLOW_CASE_ID);
                }
                else
                {
                    //验证是否多人全部审批通过
                    if(BF_FLOW_NODE_CASE.Instance.ValidateFlowNodeCaseIsFinish(record.FLOW_NODE_CASE_ID))
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
            }
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "流程处理出错：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        //验证节点权限(暂未实现)


        #region 校验是否有正在运行中的流程实例（暂未实现）
        #endregion

        #region 创建下个节点的实例(在实体类中)

        #endregion
    }
}