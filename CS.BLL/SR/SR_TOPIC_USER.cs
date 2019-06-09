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
    /// 课题人员管理
    /// </summary>
    public class SR_TOPIC_USER : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_USER Instance = new SR_TOPIC_USER();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_USER()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_USER";
            this.ItemName = "课题人员";
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
            /// 是否是责任人
            /// </summary>
            [Field(IsNotNull = true, Comment = "是否是责任人")]
            public int IS_PERSON_LIABLE { get; set; }


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
        public int SaveUserListJoins(int topicId, List<Entity> entityList, out int addCount, out int updateCount, out int deleteCount)
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
            List<Entity> listOld = GetTopicUserList(topicId);
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
        /// 获得当前课题的参与人员
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public List<Entity> GetTopicUserList(int topicId)
        {
            return GetList<Entity>("TOPIC_ID=?", topicId).ToList();
        }

    }

    public class TopicUser : SR_TOPIC_USER.Entity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TOPIC_TYPE_NAME { get; set; }
        /// <summary>
        /// 参与人员名称
        /// </summary>

        public string USER_NAME { get; set; }

        /// <summary>
        /// 参与人员名称
        /// </summary>

        public string NAME { get; set; }
    }
}
