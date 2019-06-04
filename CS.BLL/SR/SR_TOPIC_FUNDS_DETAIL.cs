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
    /// 经费明细清单
    /// </summary>
    public class SR_TOPIC_FUNDS_DETAIL : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_FUNDS_DETAIL Instance = new SR_TOPIC_FUNDS_DETAIL();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_FUNDS_DETAIL()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_FUNDS_DETAIL";
            this.ItemName = "经费明细清单";
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
            /// 经费报销ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "经费报销ID")]
            public int TOPIC_FUNDS_ID { get; set; }

            /// <summary>
            /// 报销项目名
            /// </summary>
            [Field(IsNotNull = false, Comment = "报销项目名")]
            public string NAME { get; set; }

            /// <summary>
            /// 报销金额
            /// </summary>
            [Field(IsNotNull = true, Comment = "报销金额")]
            public double FEE { get; set; }

            /// <summary>
            /// 报销说明
            /// </summary>
            [Field(IsNotNull = false, Comment = "报销说明")]
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

        public void SaveFundsDetail(int fundsId, int topicId, string fundsDetails, out int addCount, out int updateCount, out int delCount)
        {
            List<Entity> newFundsDetailList = null;
            if (!string.IsNullOrWhiteSpace(fundsDetails))
            {
                newFundsDetailList = CS.Common.FW.JSON.EncodeToEntity<List<Entity>>(fundsDetails);
            }
            List<Entity> oldFundsDetailList = Instance.GetList<Entity>("TOPIC_FUNDS_ID=?", fundsId).ToList();

            #region 找到增删改的集合
            List<Entity> addFundsDetailList = new List<Entity>();
            List<Entity> deleteFundsDetailList = new List<Entity>();
            List<Entity> updateFundsDetailList = new List<Entity>();

            int count = 0;

            if (newFundsDetailList != null && newFundsDetailList.Count > 0)
            {
                foreach (var newcp in newFundsDetailList)
                {
                    newcp.TOPIC_ID = topicId;
                    newcp.TOPIC_FUNDS_ID = fundsId;
                    newcp.UPDATE_UID = SystemSession.UserID;
                    newcp.UPDATE_TIME = DateTime.Now;
                    count = oldFundsDetailList.Count(p => p.NAME == newcp.NAME);
                    if (count > 0)
                    {
                        updateFundsDetailList.Add(newcp);
                    }
                    else
                    {
                        newcp.CREATE_UID = SystemSession.UserID;
                        newcp.CREATE_TIME = DateTime.Now;
                        addFundsDetailList.Add(newcp);
                    }
                }
            }

            if (oldFundsDetailList != null && oldFundsDetailList.Count > 0)
            {
                if (newFundsDetailList == null || newFundsDetailList.Count == 0)
                {
                    deleteFundsDetailList = oldFundsDetailList;
                }
                else
                {
                    foreach (var oldcp in oldFundsDetailList)
                    {
                        if (newFundsDetailList.Count(p => p.NAME == oldcp.NAME) == 0)
                        {
                            deleteFundsDetailList.Add(oldcp);
                        }
                    }
                }
            }
            #endregion

            #region 操作增删改
            delCount = 0; addCount = 0; updateCount = 0;
            int i = -1;
            if (deleteFundsDetailList != null && deleteFundsDetailList.Count > 0)
            {
                foreach (var item in deleteFundsDetailList)
                {
                    i = Instance.DeleteByKey(item.ID);
                    if (i > 0)
                    {
                        delCount++;
                    }
                }
            }

            if (addFundsDetailList != null && addFundsDetailList.Count > 0)
            {
                foreach (var item in addFundsDetailList)
                {
                    i = Instance.Add(item);
                    if (i > 0)
                    {
                        addCount++;
                    }
                }
            }

            if (updateFundsDetailList != null && updateFundsDetailList.Count > 0)
            {
                foreach (var item in updateFundsDetailList)
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
