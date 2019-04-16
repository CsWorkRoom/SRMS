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
    /// 流程节点关系
    /// </summary>
    public class BF_FLOW_NODE_JOIN : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FLOW_NODE_JOIN Instance = new BF_FLOW_NODE_JOIN();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW_NODE_JOIN()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW_NODE_JOIN";
            this.ItemName = "流程节点关系";
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
            /// 源节点ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "源节点ID")]
            public int FROM_NODE_ID { get; set; }

            /// <summary>
            /// 目标节点ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "目标节点ID")]
            public int TO_NODE_ID { get; set; }

            /// <summary>
            /// 条件模式
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "条件模式")]
            public short CONDITION_MODE { get; set; }

            /// <summary>
            /// 条件内容
            /// </summary>
            [Field(Length = 1024, IsIndex = true, IsIndexUnique = true, Comment = "条件内容")]
            public string CONDITION_CONTENT { get; set; }

            /// <summary>
            /// 创建者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "创建者ID")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 修改者者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "修改者者ID")]
            public int UPDATE_UID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 修改时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "修改时间")]
            public DateTime UPDATE_TIME { get; set; }
        }
        #endregion

        /// <summary>
        /// 更新节点关系信息
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="entityList"></param>
        /// <param name="addCount"></param>
        /// <param name="updateCount"></param>
        /// <param name="deleteCount"></param>
        /// <returns></returns>
        public int SaveFlowNodeJoins(int flowId, List<Entity> entityList, out int addCount, out int updateCount, out int deleteCount)
        {
            addCount = 0;
            updateCount = 0;
            deleteCount = 0;
            if (flowId < 1)
            {
                return 0;
            }
            //if (filterId < 1 || processList == null || processList.Count < 1)
            //{
            //    return 0;
            //}

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
                    //源节点+目标节点共同构成主键,格式：[from]_[to]
                    string fn = entity.FROM_NODE_ID.ToString() + "_" + entity.TO_NODE_ID.ToString();
                    if (!dicNew.ContainsKey(fn))
                    {
                        dicNew.Add(fn, entity);
                    }
                }
            }
            List<Entity> listOld = GetFlowNodeJoinList(flowId);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.FROM_NODE_ID.ToString() + "_" + entity.TO_NODE_ID.ToString();
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
                string fn = entity.FROM_NODE_ID.ToString() + "_" + entity.TO_NODE_ID.ToString();
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
                    entity.CREATE_TIME = DateTime.Now;
                    entity.CREATE_UID = FW.SystemSession.UserID;
                    entity.UPDATE_TIME = DateTime.Now;
                    entity.UPDATE_UID = FW.SystemSession.UserID;
                    entity.FLOW_ID = flowId;

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
                    entity.UPDATE_TIME = DateTime.Now;
                    entity.UPDATE_UID = FW.SystemSession.UserID;
                    entity.FLOW_ID = flowId;

                    updateCount += UpdateByKey<Entity>(entity, entity.ID);
                }
            }

            return addCount + updateCount + deleteCount;
        }
        /// <summary>
        /// 获得节点关系信息集合
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public List<Entity> GetFlowNodeJoinList(int flowId)
        {
            return GetList<Entity>("FLOW_ID=?", flowId).ToList();
        }
    }

    public class FlowNodeJoin:BF_FLOW_NODE_JOIN.Entity
    {
        /// <summary>
        /// 源节点名
        /// </summary>
        public string FROM_NODE_NAME { get; set; }
        /// <summary>
        /// 目标节点名
        /// </summary>
        public string TO_NODE_NAME { get; set; }
    }
}
