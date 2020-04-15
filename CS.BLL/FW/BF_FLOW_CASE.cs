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
    /// 流程实例
    /// </summary>
    public class BF_FLOW_CASE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FLOW_CASE Instance = new BF_FLOW_CASE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW_CASE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW_CASE";
            this.ItemName = "流程实例";
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
            /// 流程名
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "流程名")]
            public string NAME { get; set; }

            /// <summary>
            /// 对应的信息名称
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "对应的信息名称")]
            public string FLOW_CASE_NAME { get; set; }

            
            /// <summary>
            /// 流程类型
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程类型")]
            public int FLOW_TYPE_ID { get; set; }

            /// <summary>
            /// 流程ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程ID")]
            public int FLOW_ID { get; set; }

            /// <summary>
            /// 主表单表
            /// </summary>
            [Field(IsNotNull = true, Length = 128, Comment = "主表单表")]
            public string MAIN_TABLE { get; set; }

            /// <summary>
            /// 主键值
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "主键值")]
            public int PRIMARY_KEY { get; set; }

            /// <summary>
            /// 主页面
            /// </summary>
            [Field(IsNotNull = true, Length = 512, Comment = "主页面")]
            public string MAIN_PAGE { get; set; }

            /// <summary>
            /// 是否归档
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否归档")]
            public short IS_ARCHIVE { get; set; }

            /// <summary>
            /// 归档时间
            /// </summary>
            [Field(Comment = "归档时间")]
            public DateTime ARCHIVE_TIME { get; set; }

            /// <summary>
            /// 是否有效
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public short IS_ENABLE { get; set; }

            /// <summary>
            /// 无效说明
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "无效说明")]
            public string ENABLE_REMARK { get; set; }

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

            /// <summary>
            /// 流程执行过程说明
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "流程执行过程说明")]
            public string REMARK { get; set; }

        }
        #endregion
        /// <summary>
        /// 创建一个流程实例
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public int CreateFlowCase(int flowId, int mainTableKey)
        {
            //流程
            var flow = BF_FLOW.Instance.GetEntityByKey<BF_FLOW.Entity>(flowId);
            //主节点
            var mainNode = BF_FLOW_NODE.Instance.GetEntity<BF_FLOW_NODE.Entity>("FLOW_ID=? AND IS_MAIN=1");
            #region 保存流程实例
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
                MAIN_PAGE =(mainTableKey > 0)?((flow.MAIN_PAGE.IndexOf('?') >= 0) ? (flow.MAIN_PAGE + "&id=" + mainTableKey) : (flow.MAIN_PAGE + "?id=" + mainTableKey)):flow.MAIN_TABLE,
                MAIN_TABLE = flow.MAIN_TABLE,
                PRIMARY_KEY = mainTableKey,
                NAME = flow.NAME
            };
            BF_FLOW_CASE.Instance.Add(flowCase);
            #endregion
            return flowCaseId;
        }

        /// <summary>
        /// 为当前节点添加下级节点的实例s
        /// 追加流程是否结束的处理
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="currFlowNode"></param>
        /// <param name="flowCaseId"></param>
        /// <returns></returns>
        public bool CreateNextNodesCase(BF_FLOW.Entity flow, BF_FLOW_NODE.Entity currFlowNode,int flowCaseId)
        {
            bool resBool = true;
            try
            {
                //获得流程的所有节点信息
                var allNodeList = BF_FLOW_NODE.Instance.GetList<BF_FLOW_NODE.Entity>("FLOW_ID=?", flow.ID);
                var allNodeJoinList = BF_FLOW_NODE_JOIN.Instance.GetList<BF_FLOW_NODE_JOIN.Entity>("FLOW_ID=?", flow.ID);

                //已审批通过的节点实例集合
                var allNodeCaseList = BF_FLOW_NODE_CASE.Instance.GetList<BF_FLOW_NODE_CASE.Entity>("FLOW_ID=? AND FLOW_CASE_ID=? AND AUDIT_STATUS=?", flow.ID, flowCaseId, CS.Common.Enums.AuditStatus.通过.GetHashCode());

                //先获得当前节点下个走向节点集合
                var nodeJoinList = allNodeJoinList.Where(p => p.FROM_NODE_ID == currFlowNode.ID).ToList();
                //var nodeJoinList = BF_FLOW_NODE_JOIN.Instance.GetList<BF_FLOW_NODE_JOIN.Entity>("FLOW_ID=? AND FROM_NODE_ID=?", flow.ID, mainNode.ID);
                if (nodeJoinList != null && nodeJoinList.Count > 0)
                {
                    //分别为下级节点添加待办，分别为其创建待处理节点（暂时只考虑具体到人的情况）
                    foreach (var toNode in nodeJoinList)
                    {
                        var curFromNodeIdList = allNodeJoinList.Where(p => p.TO_NODE_ID == toNode.ID).Select(p => p.FROM_NODE_ID).ToList();
                        var ncCount = allNodeCaseList.Count(p => curFromNodeIdList.Contains(p.FLOW_NODE_ID));
                        if (curFromNodeIdList.Count == ncCount)//from节点数=from节点实例数，添加下级待办
                        {
                            var curToNode = allNodeList.FirstOrDefault(p => p.ID == toNode.TO_NODE_ID);
                            int curToNodeCase = BF_FLOW_NODE_CASE.Instance.AddFlowNodeCase(curToNode, flowCaseId, flow.ID, toNode.FROM_NODE_ID);
                            //修改原表的状态信息,下发到下一个节点
                            var flowCase=BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(flowCaseId);
                            UpdateMainTableState(flowCase.PRIMARY_KEY, flowCase.MAIN_TABLE, curToNode.ID.ToString(), "0");
                        }
                    }
                }
                else
                {
                    //验证流程是否结束（末梢节点是否均处理完毕）
                    if(ValidateFlowCaseIsFinish(flow.ID, flowCaseId))
                    {
                        string remark = null;
                        //结束流程
                        var flowCase= BF_FLOW_CASE.Instance.GetEntityByKey<BF_FLOW_CASE.Entity>(flowCaseId);
                        flowCase.IS_ARCHIVE = 1;
                        flowCase.IS_ENABLE = 1;
                        flowCase.ARCHIVE_TIME = DateTime.Now;
                        BF_FLOW_CASE.Instance.UpdateByKey(flowCase, flowCaseId);
                        //修改原表的状态信息-当前流程结束
                        UpdateMainTableState(flowCase.PRIMARY_KEY, flowCase.MAIN_TABLE, "0", "1");
                        //验证和触发下个流程
                        var flowRefList = BF_FLOW_REF.Instance.GetNextFlow(flow.ID);
                        if(flowRefList!=null&&flowRefList.Count>0)
                        {
                            foreach(var flowRef in flowRefList)
                            {
                                CreateFlowCaseAndMainNodeCase(flowRef.FLOW_ID);
                            }
                            remark = string.Format(@"创建了[{0}]个下级流程", flowRefList.Count);
                        }
                        else
                        {
                            remark = "未找到下级走向节点";
                           
                        }

                        #region 加入走向说明
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("REMARK", remark);//是否流程就结束了呢？
                        BF_FLOW_CASE.Instance.UpdateByKey(dic, flowCaseId);
                        #endregion
                    }
                }
            }
            catch(Exception ex)
            {
                string errMsg = string.Format(@"为流程[{0}]实例[{1}]的节点[{2}]添加下级节点实例失败：{3}",
                    flow.ID, flowCaseId, currFlowNode.ID, ex.Message);
                BLog.Write(BLog.LogLevel.ERROR, errMsg);
                throw new Exception(errMsg);
            }
            return resBool;
        }
        /// <summary>
        /// 验证流程是否已执行完毕（末梢节点是否已执行完毕）
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="flowCaseId"></param>
        /// <returns></returns>
        public bool ValidateFlowCaseIsFinish(int flowId,int flowCaseId)
        {
            bool resBool = false;
            var endFlowNodeList= BF_FLOW_NODE_JOIN.Instance.GetList<BF_FLOW_NODE_JOIN.Entity>("TO_NODE_ID NOT IN (SELECT FROM_NODE_ID FROM BF_FLOW_NODE_JOIN) AND FLOW_ID=?", flowId);
            if(endFlowNodeList!=null&&endFlowNodeList.Count>0)
            {
                var flowNodeCaseList= BF_FLOW_NODE_CASE.Instance.GetList<BF_FLOW_NODE_CASE>("FLOW_CASE_ID=? AND FLOW_ID=? AND IS_FINISH=1 AND FLOW_NODE_ID IN (?) AND AUDIT_STATUS=?",
                   flowCaseId, flowId, string.Join(",", endFlowNodeList.Select(p => p.TO_NODE_ID.ToString())),
                   CS.Common.Enums.AuditStatus.通过.GetHashCode());
                if(flowNodeCaseList!=null&&flowNodeCaseList.Count>0&&flowNodeCaseList.Count==endFlowNodeList.Count)
                {
                    resBool = true;
                }
            }
            return resBool;
        }
        /// <summary>
        /// 创建流程实例及第一个节点待办项
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public int CreateFlowCaseAndMainNodeCase(int flowId)
        {
            //01.创建流程实例
            int flowCaseId = CreateFlowCase(flowId, 0);
            //02.创建主流程节点实例
            var mainNode = BF_FLOW_NODE.Instance.GetEntity<BF_FLOW_NODE.Entity>("FLOW_ID=? AND IS_MAIN=1");
            int mainFlowNodeCaseId = BF_FLOW_NODE_CASE.Instance.GetNextValueFromSeqDef();
            BF_FLOW_NODE_CASE.Entity mainFlowNodeCase = new BF_FLOW_NODE_CASE.Entity()
            {
                ID = mainFlowNodeCaseId,
                DIV_X = mainNode.DIV_X,
                DIV_Y = mainNode.DIV_Y,
                AUDIT_STATUS = Convert.ToInt16(CS.Common.Enums.AuditStatus.未审批.GetHashCode()),
                CREATE_TIME = DateTime.Now,
                CREATE_UID = SystemSession.UserID,
                USER_IDS = mainNode.USER_IDS,//待处理人的指定，当前是从流程配置中互获取
                DPT_IDS = mainNode.DPT_IDS,
                ROLE_IDS = mainNode.ROLE_IDS,
                FLOW_CASE_ID = flowCaseId,
                FLOW_ID = flowId,
                FROM_FLOW_NODE_CASE_ID = 0,
                IS_FINISH = 0,
                IS_MAIN = 1,
                NAME = mainNode.NAME,
                REMARK = "由上级流程自动触发创建",
                FLOW_NODE_ID = mainNode.ID,
                DEAL_WAY=mainNode.DEAL_WAY
            };
            BF_FLOW_NODE_CASE.Instance.Add(mainFlowNodeCase);
            return mainFlowNodeCaseId;
        }

        /// <summary>
        /// 更新源表的状态
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="mainTable"></param>
        /// <param name="flowState"></param>
        /// <param name="isAdopt">-1:表示未通过；1表示通过</param>
        public void UpdateMainTableState(int entityId,string mainTable, string flowState, string isAdopt)
        {
            string updateSql = string.Format(@"update {0} set flow_state={1}, is_adopt={2} where id={3}", mainTable, flowState, isAdopt,entityId);

            BF_DATABASE.Instance.ExecuteSQL(0, "lb@em", updateSql, null);

        }
    }
}
