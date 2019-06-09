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
    public class SR_TOPIC_BUDGET : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_BUDGET Instance = new SR_TOPIC_BUDGET();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_BUDGET()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_BUDGET";
            this.ItemName = "课题预算信息";
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
            /// 预算类型ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "预算类型ID")]
            public int BUDGET_TYPE_ID { get; set; }

            /// <summary>
            /// 预算项目名
            /// </summary>
            [Field(IsNotNull = true, Comment = "预算项目名")]
            public string NAME { get; set; }

            /// <summary>
            /// 预算金额
            /// </summary>
            [Field(IsNotNull = true, Comment = "预算金额")]
            public double FEE { get; set; }

            /// <summary>
            /// 预算备注
            /// </summary>
            [Field(IsNotNull = false, Comment = "预算备注")]
            public string REMARK { get; set; }

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
            /// 修改人
            /// </summary>
            [Field(IsNotNull = true, Comment = "修改人")]
            public int UPDATE_UID { get; set; }
        }
        #endregion

        public void SaveBudget(int topicId,string budgets, out int addCount, out int updateCount, out int delCount)
        {
            List<Entity> newBudgetList = null;
            if (!string.IsNullOrWhiteSpace(budgets))
            {
                newBudgetList = CS.Common.FW.JSON.EncodeToEntity<List<Entity>>(budgets);
            }
            List<Entity> oldBudgetList = Instance.GetList<Entity>("TOPIC_ID=?", topicId).ToList();

            #region 找到增删改的集合
            List<Entity> addBudgetList = new List<Entity>();
            List<Entity> deleteBudgetList = new List<Entity>();
            List<Entity> updateBudgetList = new List<Entity>();

            int count = 0;

            if (newBudgetList != null && newBudgetList.Count > 0)
            {
                foreach (var newcp in newBudgetList)
                {
                    newcp.TOPIC_ID = topicId;
                    newcp.UPDATE_UID = SystemSession.UserID;
                    newcp.UPDATE_TIME = DateTime.Now;
                    count = oldBudgetList.Count(p => p.NAME == newcp.NAME);
                    if (count > 0)
                    {
                        updateBudgetList.Add(newcp);
                    }
                    else
                    {
                        newcp.CREATE_UID = SystemSession.UserID;
                        newcp.CREATE_TIME = DateTime.Now;
                        addBudgetList.Add(newcp);
                    }
                }
            }

            if (oldBudgetList != null && oldBudgetList.Count > 0)
            {
                if (newBudgetList == null || newBudgetList.Count == 0)
                {
                    deleteBudgetList = oldBudgetList;
                }
                else
                {
                    foreach (var oldcp in oldBudgetList)
                    {
                        if (newBudgetList.Count(p => p.NAME == oldcp.NAME) == 0)
                        {
                            deleteBudgetList.Add(oldcp);
                        }
                    }
                }
            }
            #endregion

            #region 操作增删改
            delCount = 0; addCount = 0; updateCount = 0;
            int i = -1;
            if (deleteBudgetList != null && deleteBudgetList.Count > 0)
            {
                foreach (var item in deleteBudgetList)
                {
                    i = Instance.DeleteByKey(item.ID);
                    if (i > 0)
                    {
                        delCount++;
                    }
                }
            }

            if (addBudgetList != null && addBudgetList.Count > 0)
            {
                foreach (var item in addBudgetList)
                {
                    i = Instance.Add(item);
                    if (i > 0)
                    {
                        addCount++;
                    }
                }
            }

            if (updateBudgetList != null && updateBudgetList.Count > 0)
            {
                foreach (var item in updateBudgetList)
                {
                    i = Instance.UpdateByKey(item, item.ID);
                    if (i > 0)
                    {
                        updateCount++;
                    }
                }
            }
            #endregion
        }
    }
}
