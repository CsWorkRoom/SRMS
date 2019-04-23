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
        /// 为当前节点添加下级节点的实例s
        /// 追加流程是否结束的处理（暂未实现）
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
                var allNodeCaseList = BF_FLOW_NODE_CASE.Instance.GetList<BF_FLOW_NODE_CASE.Entity>("FLOW_ID=? AND FLOW_CASE_ID=? AUDIT_STATUS=?", flow.ID, flowCaseId, CS.Common.Enums.AuditStatus.通过.GetHashCode());

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
                        }
                    }
                }
                else
                {
                    #region 加入走向说明
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("REMARK", "未找到下级走向节点");//是否流程就结束了呢？
                    BF_FLOW_CASE.Instance.UpdateByKey(dic, flowCaseId);
                    #endregion
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

    }
}
