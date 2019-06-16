using CS.Base.DBHelper;
using CS.Base.Log;
using CS.Common.FW;
using CS.Library.BaseQuery;
using CS.Library.Export;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CS.BLL.FW.BF_FORM.FieldInfo;
using static CS.Common.FW.Enums;

namespace CS.BLL.FW
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    public class BF_FLOW_NODE_CASE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FLOW_NODE_CASE Instance = new BF_FLOW_NODE_CASE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW_NODE_CASE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW_NODE_CASE";
            this.ItemName = "流程节点实例";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
        }

        #region 实体

        /// <summary>
        /// 实体
        /// </summary>
        public class Entity
        {
            /// <summary>
            /// ID 
            /// </summary>
            [Field(IsPrimaryKey = true, IsNotNull = true, Comment = "ID ")]
            public int ID { get; set; }

            /// <summary>
            /// 所属流程ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程ID")]
            public int FLOW_ID { get; set; }

            /// <summary>
            /// 流程实例ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程实例ID")]
            public int FLOW_CASE_ID { get; set; }

            /// <summary>
            /// 流程节点ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程节点ID")]
            public int FLOW_NODE_ID { get; set; }

            /// <summary>
            /// 上级触发节点实例ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "上级触发节点实例ID")]
            public int FROM_FLOW_NODE_CASE_ID { get; set; }

            /// <summary>
            /// 流程节点名
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "流程名")]
            public string NAME { get; set; }

            /// <summary>
            /// 是否主节点
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否主节点")]
            public Int16 IS_MAIN { get; set; }

            /// <summary>
            /// 横坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "横坐标")]
            public double DIV_X { get; set; }

            /// <summary>
            /// 纵坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "纵坐标")]
            public double DIV_Y { get; set; }

            /// <summary>
            /// 节点说明
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "流程说明")]
            public string REMARK { get; set; }

            /// <summary>
            /// 用户权限IDS
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "用户权限IDS")]
            public string USER_IDS { get; set; }

            /// <summary>
            /// 角色权限IDS
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "用户权限IDS")]
            public string ROLE_IDS { get; set; }

            /// <summary>
            /// 组织权限IDS
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "用户权限IDS")]
            public string DPT_IDS { get; set; }

            /// <summary>
            /// 审批状态
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批状态")]
            public short AUDIT_STATUS { get; set; }

            /// <summary>
            /// 流程处理方式
            /// 1：一人审批通过就进入下个流程
            /// 2：全部人员审批通过进入下个流程
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程处理方式")]
            public short DEAL_WAY { get; set; }

            /// <summary>
            /// 是否处理完毕
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否处理完毕")]
            public short IS_FINISH { get; set; }

            /// <summary>
            /// 完毕时间
            /// </summary>
            [Field(Comment = "完毕时间")]
            public DateTime FINISH_TIME { get; set; }

            /// <summary>
            /// 创建者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "创建者ID")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }
        }
        #endregion

        public int AddMainFlowNodeCase(BF_FLOW_NODE.Entity mainNode, int flowCaseId, int flowId)
        {
            int mainFlowNodeCaseId = BF_FLOW_NODE_CASE.Instance.GetNextValueFromSeqDef();
            try
            {
                BF_FLOW_NODE_CASE.Entity mainFlowNodeCase = new BF_FLOW_NODE_CASE.Entity()
                {
                    ID = mainFlowNodeCaseId,
                    DIV_X = mainNode.DIV_X,
                    DIV_Y = mainNode.DIV_Y,
                    AUDIT_STATUS = Convert.ToInt16(CS.Common.Enums.AuditStatus.通过.GetHashCode()),//默认通过1
                    CREATE_TIME = DateTime.Now,
                    CREATE_UID = SystemSession.UserID,
                    USER_IDS = mainNode.USER_IDS,
                    DPT_IDS = mainNode.DPT_IDS,
                    ROLE_IDS = mainNode.ROLE_IDS,
                    FLOW_CASE_ID = flowCaseId,
                    FLOW_ID = flowId,
                    FROM_FLOW_NODE_CASE_ID = 0,
                    IS_FINISH = 1,
                    IS_MAIN = 1,
                    NAME = mainNode.NAME,
                    REMARK = "主节点默认通过状态",
                    FINISH_TIME = DateTime.Now,
                    FLOW_NODE_ID = mainNode.ID,
                    DEAL_WAY = mainNode.DEAL_WAY
                };
                BF_FLOW_NODE_CASE.Instance.Add(mainFlowNodeCase);
            }
            catch (Exception ex)
            {
                string errMsg = string.Format(@"为流程[{0}]实例[{1}]添加节点[{2}]的实例[3]失败：{4}",
                    flowId, flowCaseId, mainNode.ID, mainFlowNodeCaseId, ex.Message);
                BLog.Write(BLog.LogLevel.ERROR, errMsg);
                throw ex;
            }
            return mainFlowNodeCaseId;
        }
        /// <summary>
        /// 退回流程至主节点
        /// </summary>
        /// <param name="flowNodeId"></param>
        /// <param name="flowCaseId"></param>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public int ReturnFlowNodeCase(int returnFlowNodeCaseId, int flowCaseId)
        {
            int mainFlowNodeCaseId = 0;
            BF_FLOW_CASE.Entity flowCase = new BF_FLOW_CASE.Entity();
            BF_FLOW_NODE.Entity mainFlowNode = new BF_FLOW_NODE.Entity();
            try
            {
                flowCase = BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(flowCaseId);
                mainFlowNode = BF_FLOW_NODE.Instance.GetEntity<BF_FLOW_NODE.Entity>("FLOW_ID=? AND IS_MAIN=1");
                //获得上次主节点流程实例信息
                var mainFlowNodeCaseList = BF_FLOW_NODE_CASE.Instance.GetList<BF_FLOW_NODE_CASE.Entity>("FLOW_NODE_ID=? AND FLOW_CASE_ID=? AND FLOW_ID=?", mainFlowNode.ID, flowCaseId, flowCase.FLOW_ID).OrderByDescending(p => p.ID).ToList();
                if (mainFlowNodeCaseList != null && mainFlowNodeCaseList.Count() > 0)
                {
                    var lastMainFlowNodeCase = mainFlowNodeCaseList[0];
                    mainFlowNodeCaseId = BF_FLOW_NODE_CASE.Instance.GetNextValueFromSeqDef();
                    BF_FLOW_NODE_CASE.Entity mainFlowNodeCase = new BF_FLOW_NODE_CASE.Entity()
                    {
                        ID = mainFlowNodeCaseId,
                        DIV_X = lastMainFlowNodeCase.DIV_X,
                        DIV_Y = lastMainFlowNodeCase.DIV_Y,
                        AUDIT_STATUS = Convert.ToInt16(CS.Common.Enums.AuditStatus.未审批.GetHashCode()),//未审批
                        CREATE_TIME = DateTime.Now,
                        CREATE_UID = SystemSession.UserID,
                        USER_IDS = flowCase.CREATE_UID.ToString(),
                        FLOW_CASE_ID = flowCaseId,
                        FLOW_ID = flowCase.FLOW_ID,
                        FROM_FLOW_NODE_CASE_ID = 0,
                        IS_FINISH = 0,
                        IS_MAIN = 1,
                        NAME = lastMainFlowNodeCase.NAME,
                        REMARK = "被退回至主节点",
                        FINISH_TIME = DateTime.Now,
                        FLOW_NODE_ID = mainFlowNode.ID,
                        DEAL_WAY = mainFlowNode.DEAL_WAY
                    };
                    BF_FLOW_NODE_CASE.Instance.Add(mainFlowNodeCase);
                }
            }
            catch (Exception ex)
            {
                string errMsg = string.Format(@"为流程[{0}]实例[{1}]添加节点[{2}]的实例[3]失败：{4}",
                    flowCase.FLOW_ID, flowCaseId, mainFlowNode.ID, mainFlowNodeCaseId, ex.Message);
                BLog.Write(BLog.LogLevel.ERROR, errMsg);
                throw ex;
            }
            return mainFlowNodeCaseId;
        }

        public int AddFlowNodeCase(BF_FLOW_NODE.Entity node, int flowCaseId, int flowId, int fromFlowNodeCaseId = 0)
        {
            int flowNodeCaseId = BF_FLOW_NODE_CASE.Instance.GetNextValueFromSeqDef();
            try
            {
                BF_FLOW_NODE_CASE.Entity flowNodeCase = new BF_FLOW_NODE_CASE.Entity()
                {
                    ID = flowNodeCaseId,
                    DIV_X = node.DIV_X,
                    DIV_Y = node.DIV_Y,
                    AUDIT_STATUS = Convert.ToInt16(CS.Common.Enums.AuditStatus.未审批.GetHashCode()),
                    CREATE_TIME = DateTime.Now,
                    CREATE_UID = SystemSession.UserID,
                    USER_IDS = node.USER_IDS,
                    DPT_IDS = node.DPT_IDS,
                    ROLE_IDS = node.ROLE_IDS,
                    FLOW_CASE_ID = flowCaseId,
                    FLOW_ID = flowId,
                    FROM_FLOW_NODE_CASE_ID = fromFlowNodeCaseId,
                    IS_FINISH = 0,
                    IS_MAIN = 0,
                    NAME = node.NAME,
                    FLOW_NODE_ID = node.ID,
                    DEAL_WAY=node.DEAL_WAY
                };
                BF_FLOW_NODE_CASE.Instance.Add(flowNodeCase);
            }
            catch (Exception ex)
            {
                string errMsg = string.Format(@"为流程[{0}]实例[{1}]添加节点[{2}]的实例[3]失败：{4}",
                    flowId, flowCaseId, node.ID, flowNodeCaseId, ex.Message);
                BLog.Write(BLog.LogLevel.ERROR, errMsg);
                throw ex;
            }
            return flowNodeCaseId;
        }

        /// <summary>
        /// 验证当前节点是否已处理完毕
        /// </summary>
        /// <param name="flowNodeCaseId"></param>
        public bool ValidateFlowNodeCaseIsFinish(int flowNodeCaseId)
        {
            bool resBool = false;
            var flowNodeCase = BF_FLOW_NODE_CASE.Instance.GetEntityByKey<BF_FLOW_NODE_CASE.Entity>(flowNodeCaseId);
            
            var userIds = flowNodeCase.USER_IDS.Split(new char[] { ',' }, StringSplitOptions.None).ToList();

            var caseRecordList = BF_FLOW_NODE_CASE_RECORD.Instance.GetList<BF_FLOW_NODE_CASE_RECORD.Entity>(string.Format(@"FLOW_NODE_CASE_ID={0} AND AUDIT_STATUS={1}",
                flowNodeCaseId, CS.Common.Enums.AuditStatus.通过.GetHashCode()));
            if (caseRecordList != null && caseRecordList.Count > 0)
            {
                if (flowNodeCase.DEAL_WAY ==(short) CS.Common.Enums.DealWay.多人全部审批.GetHashCode())
                {
                    var dealUserIds = caseRecordList.Select(p => p.AUDIT_UID.ToString()).ToList();
                    if (userIds.Where(p => dealUserIds.Contains(p)).Count() > userIds.Count)
                    {
                        resBool = true;
                    }
                }
                else if(flowNodeCase.DEAL_WAY == (short)CS.Common.Enums.DealWay.一人审批即可.GetHashCode())
                {
                    resBool = true;
                }
            }
            return resBool;
        }
    }
}
