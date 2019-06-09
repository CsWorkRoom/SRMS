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
    /// 课题评审规则
    /// </summary>
    public class SR_TOPIC_SUB_ITEM : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_SUB_ITEM Instance = new SR_TOPIC_SUB_ITEM();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_SUB_ITEM()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_SUB_ITEM";
            this.ItemName = "课题评审规则";
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
            [Field(IsPrimaryKey = true, IsAutoIncrement = true, IsNotNull = true, Comment = "ID")]
            public int ID { get; set; }


            /// <summary>
            /// 课题ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题ID")]
            public int TOPIC_ID { get; set; }

            /// <summary>
            /// 评分规则项ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "评分规则项ID")]
            public int SUB_ITEM_ID { get; set; }

            /// <summary>
            /// 权重值
            /// </summary>
            [Field(IsNotNull = true, Comment = "权重值")]
            public double WEIGHT { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field( Comment = "备注")]
            public string REMARK { get; set; }

      
        }

        #endregion

        /// <summary>
        /// 批量保存课题的评审规则
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="entityList"></param>
        /// <param name="addCount"></param>
        /// <param name="updateCount"></param>
        /// <param name="deleteCount"></param>
        /// <returns></returns>
        public int SaveSubItemListJoins(int topicId, List<Entity> entityList, out int addCount, out int updateCount, out int deleteCount)
        {
            addCount = 0;
            updateCount = 0;
            deleteCount = 0;
            if (topicId < 1)
            {
                return 0;
            }

            Dictionary<string, Entity> dicNew = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicOld = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicInsert = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicUpdate = new Dictionary<string, Entity>();
            List<int> listDelete = new List<int>();
            if (entityList != null && entityList.Count > 0)
            {
                for (int i = 0; i < entityList.Count; i++)
                {
                    Entity entity = entityList[i];
                    //获取用户的ID
                    string fn = entity.SUB_ITEM_ID.ToString();
                    if (!dicNew.ContainsKey(fn))
                    {
                        dicNew.Add(fn, entity);
                    }
                }
            }
            List<Entity> listOld = GetTopicSubItemList(topicId);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.SUB_ITEM_ID.ToString();
                    if (!dicOld.ContainsKey(fn))
                    {
                        dicOld.Add(fn, entity);
                    }
                    //将删除
                    if (!dicNew.ContainsKey(fn))
                    {
                        listDelete.Add(entity.ID);
                    }
                }
            }

            foreach (Entity entity in entityList)
            {
                string fn = entity.SUB_ITEM_ID.ToString();
                if (dicOld.ContainsKey(fn))
                {
                    if (!dicUpdate.ContainsKey(fn))
                    {
                        entity.ID = dicOld[fn].ID;//赋值
                        dicUpdate.Add(fn, entity);
                    }
                }
                else
                {
                    if (!dicInsert.ContainsKey(fn))
                    {
                        dicInsert.Add(fn, entity);
                    }
                }
            }

            //删除
            if (listDelete.Count > 0)
            {
                deleteCount = Delete("ID IN (" + string.Join(",", listDelete) + ")");
            }
            //添加
            if (dicInsert.Count > 0)
            {
                List<Entity> listInsert = dicInsert.Values.ToList<Entity>();
                for (int i = 0; i < dicInsert.Count; i++)
                {
                    Entity entity = listInsert[i];
                    entity.TOPIC_ID = topicId;
                    addCount += Add<Entity>(entity);
                }
            }
            //更新
            if (dicUpdate.Count > 0)
            {
                List<Entity> listUpdate = dicUpdate.Values.ToList<Entity>();
                for (int i = 0; i < listUpdate.Count; i++)
                {
                    Entity entity = listUpdate[i];
                    entity.TOPIC_ID = topicId;
                    updateCount += UpdateByKey<Entity>(entity, entity.ID);
                }
            }

            return addCount + updateCount + deleteCount;
        }
        /// <summary>
        /// 获得当前课题评审规则
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public List<Entity> GetTopicSubItemList(int topicId)
        {
            return GetList<Entity>("TOPIC_ID=?", topicId).ToList();
        }

    }

    public class TopicSubItem : SR_TOPIC_SUB_ITEM.Entity
    {
        /// <summary>
        /// 学科名称
        /// </summary>
        public string SUBJECT_NAME { get; set; }
        /// <summary>
        /// 评审规则名称
        /// </summary>

        public string SUB_ITEM_NAME { get; set; }

        /// <summary>
        /// 学科ID
        /// </summary>

        public string SUBJECT_ID { get; set; }

        /// <summary>
        ///专家评分
        /// </summary>

        public double SCORE { get; set; }
        /// <summary>
        ///专家评分说明
        /// </summary>

        public string SCORE_REMARK { get; set; }
    }
}
