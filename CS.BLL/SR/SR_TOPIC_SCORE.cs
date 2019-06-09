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
    /// 课题专家单项打分
    /// </summary>
    public class SR_TOPIC_SCORE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_SCORE Instance = new SR_TOPIC_SCORE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_SCORE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_SCORE";
            this.ItemName = "课题专家单项打分";
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
            /// 专家ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "专家ID")]
            public int USER_ID { get; set; }

            /// <summary>
            /// 课题ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题ID")]
            public int TOPIC_ID { get; set; }

            /// <summary>
            /// 课题规则设置ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题规则设置ID")]
            public int TOPIC_SUB_ITEM_ID { get; set; }

            /// <summary>
            /// 评分规则项ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "评分规则项ID")]
            public int SUB_ITEM_ID { get; set; }

            /// <summary>
            /// 单项得分
            /// </summary>
            [Field(IsNotNull = true, Comment = "单项得分")]
            public double SCORE { get; set; }

            /// <summary>
            /// 权重
            /// </summary>
            [Field(IsNotNull = true, Comment = "权重")]
            public double WEIGHT { get; set; }

            /// <summary>
            /// 最终得分
            /// </summary>
            [Field(IsNotNull = true, Comment = "最终得分")]
            public double ACT_SCORE { get; set; }

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
        public int SaveScoreListJoins(int topicId,int userId, List<Entity> entityList, out int addCount, out int updateCount, out int deleteCount)
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
                    //获取规则设置的ID
                    string fn = entity.TOPIC_SUB_ITEM_ID.ToString();
                    if (!dicNew.ContainsKey(fn))
                    {
                        dicNew.Add(fn, entity);
                    }
                }
            }
            List<Entity> listOld = GetTopicScoreList(topicId,userId);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.TOPIC_SUB_ITEM_ID.ToString();
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
                string fn = entity.TOPIC_SUB_ITEM_ID.ToString();
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
                    entity.USER_ID = userId;
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
                    entity.USER_ID = userId;
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
        public List<Entity> GetTopicScoreList(int topicId,int userId)
        {
            return GetList<Entity>("TOPIC_ID=? AND USER_ID=?", topicId, userId).ToList();
        }

        /// <summary>
        /// 获得当前课题单项评分
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public Entity GetTopicSubItemScoreList(int topicSubItemId, int userId)
        {
            return GetList<Entity>("TOPIC_SUB_ITEM_ID=? AND USER_ID=?", topicSubItemId, userId).FirstOrDefault();
        }

    }
    public class TopicScore : SR_TOPIC_SCORE.Entity
    {
        /// <summary>
        /// 学科名称
        /// </summary>
        public string SUBJECT_NAME { get; set; }
        /// <summary>
        /// 评审规则名称
        /// </summary>

        public string SUB_ITEM_NAME { get; set; }
    }
}
