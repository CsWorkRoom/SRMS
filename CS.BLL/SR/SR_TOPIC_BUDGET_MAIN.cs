using CS.BLL.FW;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.SR
{
    /// <summary>
    /// 课题预算信息
    /// </summary>
    public class SR_TOPIC_BUDGET_MAIN : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_BUDGET_MAIN Instance = new SR_TOPIC_BUDGET_MAIN();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_BUDGET_MAIN()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_BUDGET_MAIN";
            this.ItemName = "课题预算信息主表";
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
            [Field(IsPrimaryKey = true, IsAutoIncrement = true, IsNotNull = true, Comment = "ID ")]
            public int ID { get; set; }

            /// <summary>
            /// 课题ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题ID")]
            public int TOPIC_ID { get; set; }
            

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 更新时间
            /// </summary>
            [Field(IsNotNull = true, Comment = "更新时间")]
            public DateTime UPDATE_TIME { get; set; }

            /// <summary>
            /// 创建人
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建人")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 审批状态
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批状态")]
            public int FLOW_STATE { get; set; }

            /// <summary>
            /// 是否审批通过
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否审批通过")]
            public int IS_ADOPT { get; set; }
        }
        #endregion

        public int SaveBudgetMain(int topicId)
        {
            int mid = 0;
           var budgetMain = Instance.GetList<Entity>("TOPIC_ID=?", topicId).FirstOrDefault();
            if (budgetMain != null && budgetMain.ID > 0)
            {
                mid = budgetMain.ID;
                budgetMain.UPDATE_TIME=DateTime.Now;
                SR_TOPIC_BUDGET_MAIN.Instance.UpdateByKey(budgetMain, budgetMain.ID);
            }
            else
            {
                Entity entity=new Entity();
                mid = SR_TOPIC_BUDGET_MAIN.Instance.GetNextValueFromSeqDef();
                entity.ID = mid;
                entity.CREATE_UID = SystemSession.UserID;
                entity.CREATE_TIME = DateTime.Now;
                entity.UPDATE_TIME = DateTime.Now;
                entity.TOPIC_ID = topicId;
                SR_TOPIC_BUDGET_MAIN.Instance.Add(entity, true);
            }

            return mid;
        }
    }
}
