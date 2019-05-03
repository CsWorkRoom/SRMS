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
    /// 流程定义
    /// </summary>
    public class BF_FLOW_NODE_CASE_RECORD : BBaseQuery
    {
        /// <summary>
        /// 节点处理记录
        /// </summary>
        public static BF_FLOW_NODE_CASE_RECORD Instance = new BF_FLOW_NODE_CASE_RECORD();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW_NODE_CASE_RECORD()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW_NODE_CASE_RECORD";
            this.ItemName = "节点处理记录";
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
            /// 流程ID
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
            /// 流程节点实例ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程节点实例ID")]
            public int FLOW_NODE_CASE_ID { get; set; }

            /// <summary>
            /// 审批状态
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批状态")]
            public int AUDIT_STATUS { get; set; }

            /// <summary>
            /// 审批内容
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "审批内容")]
            public string AUDIT_CONTENT { get; set; }

            /// <summary>
            /// 审批人
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批人")]
            public int AUDIT_UID { get; set; }

            /// <summary>
            /// 审批时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "审批时间")]
            public DateTime AUDIT_TIME { get; set; }
        }
        #endregion

    }
}
