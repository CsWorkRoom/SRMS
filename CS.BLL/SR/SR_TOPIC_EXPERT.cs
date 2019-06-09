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
    /// 课题评审老师
    /// </summary>
    public class SR_TOPIC_EXPERT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_EXPERT Instance = new SR_TOPIC_EXPERT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_EXPERT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_EXPERT";
            this.ItemName = "课题评审老师";
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
            /// 用户ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "用户ID")]
            public int USER_ID { get; set; }

            /// <summary>
            /// 是否已经评分
            /// </summary>
            [Field(IsNotNull = true, Comment = "是否评分")]
            public int IS_SCORE { get; set; }

            /// <summary>
            /// 最终评分
            /// </summary>
            [Field(DefaultValue = "0", Comment = "最终得分")]
            public double SCORE { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW",Comment = "当前评分时间")]
            public DateTime OPER_TIME { get; set; }
        }

        #endregion

        /// <summary>
        /// 批量保存课题的参与人员
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="entityList"></param>
        /// <param name="addCount"></param>
        /// <param name="updateCount"></param>
        /// <param name="deleteCount"></param>
        /// <returns></returns>
        public int SaveExpertListJoins(int topicId, List<Entity> entityList, out int addCount, out int updateCount, out int deleteCount)
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
                    string fn = entity.USER_ID.ToString();
                    if (!dicNew.ContainsKey(fn))
                    {
                        dicNew.Add(fn, entity);
                    }
                }
            }
            List<Entity> listOld = GetTopicExpertList(topicId);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.USER_ID.ToString();
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
                string fn = entity.USER_ID.ToString();
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
                    entity.IS_SCORE = 0;
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
        /// 获得当前课题的评审老师
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public List<Entity> GetTopicExpertList(int topicId)
        {
            return GetList<Entity>("TOPIC_ID=?", topicId).ToList();
        }

        /// <summary>
        /// 返回用户当前课题
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Entity GetTopicByTopicAndUser(int topicId,int userId)
        {
            return GetList<Entity>("TOPIC_ID=? and USER_ID=?", topicId, userId).FirstOrDefault();
        }

    }

    public class TopicExpert : SR_TOPIC_EXPERT.Entity
    {
        /// <summary>
        /// 课题名称
        /// </summary>
        public string TOPIC_NAME { get; set; }
        /// <summary>
        /// 评审老师名称
        /// </summary>

        public string EXPERT_NAME { get; set; }
        /// <summary>
        /// 评审老师归属部门
        /// </summary>

        public string CITY_NAME { get; set; }

        /// <summary>
        /// 每个评审的总打分
        /// </summary>
        public double ALL_SCORE { get; set; }
    }
}
