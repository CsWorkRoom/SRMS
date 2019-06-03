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
    /// 课题完善信息参与单位
    /// </summary>
    public class SR_TOPIC_DETAIL_COMPANY : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_DETAIL_COMPANY Instance = new SR_TOPIC_DETAIL_COMPANY();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_DETAIL_COMPANY()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_DETAIL_COMPANY";
            this.ItemName = "课题完善信息参与单位";
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
            /// 课题信息ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题信息ID")]
            public int TOPIC_DETAIL_ID { get; set; }

            /// <summary>
            /// 单位名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "单位名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 单位联系人
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "单位联系人")]
            public string LINK_NAME { get; set; }

            /// <summary>
            /// 单位联系电话
            /// </summary>
            [Field(IsNotNull = false, Comment = "单位联系电话")]
            public string PHONE { get; set; }

            /// <summary>
            /// 是否有合作协议
            /// </summary>
            [Field(IsNotNull = true, Comment = "是否有合作协议")]
            public short IS_CONTRACT { get; set; }

            /// <summary>
            /// 单位地址
            /// </summary>
            [Field(IsNotNull = false, Comment = "单位地址")]
            public string ADDRESS { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Comment = "备注")]
            public string REMARK { get; set; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 保存课题完善的合作单位信息
        /// </summary>
        /// <param name="topicDetailId"></param>
        /// <param name="companys"></param>
        public void SaveCompanys(int topicDetailId, string companys, out int addCount, out int updateCount, out int delCount)
        {
            List<Entity> newCompanyList = null;
            if (!string.IsNullOrWhiteSpace(companys))
            {
                newCompanyList = CS.Common.FW.JSON.EncodeToEntity<List<Entity>>(companys);
            }
            List<Entity> oldCompanyList = Instance.GetList<Entity>("TOPIC_DETAIL_ID=?", topicDetailId).ToList();

            #region 找到增删改的集合
            List<Entity> addCompanyList = new List<Entity>();
            List<Entity> deleteCompanyList = new List<Entity>();
            List<Entity> updateCompanyList = new List<Entity>();

            int count = 0;

            if (newCompanyList != null && newCompanyList.Count > 0)
            {
                foreach (var newcp in newCompanyList)
                {
                    newcp.TOPIC_DETAIL_ID = topicDetailId;
                    count = oldCompanyList.Count(p => p.NAME == newcp.NAME);
                    if (count > 0)
                    {
                        updateCompanyList.Add(newcp);
                    }
                    else
                    {
                        addCompanyList.Add(newcp);
                    }
                }
            }

            if (oldCompanyList != null && oldCompanyList.Count > 0)
            {
                if (newCompanyList == null || newCompanyList.Count == 0)
                {
                    deleteCompanyList = oldCompanyList;
                }
                else
                {
                    foreach (var oldcp in oldCompanyList)
                    {
                        if (newCompanyList.Count(p => p.NAME == oldcp.NAME) == 0)
                        {
                            deleteCompanyList.Add(oldcp);
                        }
                    }
                }
            }
            #endregion

            #region 操作增删改
            delCount = 0; addCount = 0; updateCount = 0;
            int i = -1;
            if (deleteCompanyList != null && deleteCompanyList.Count > 0)
            {
                foreach (var item in deleteCompanyList)
                {
                    i = Instance.DeleteByKey(item.ID);
                    if (i > 0)
                    {
                        delCount++;
                    }
                }
            }

            if (addCompanyList != null && addCompanyList.Count > 0)
            {
                foreach (var item in addCompanyList)
                {
                    i = Instance.Add(item);
                    if (i > 0)
                    {
                        addCount++;
                    }
                }
            }

            if (updateCompanyList != null && updateCompanyList.Count > 0)
            {
                foreach (var item in updateCompanyList)
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
        #endregion
    }
}
