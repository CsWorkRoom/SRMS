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
    /// 流程节点
    /// </summary>
    public class BF_FLOW_NODE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FLOW_NODE Instance = new BF_FLOW_NODE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW_NODE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW_NODE";
            this.ItemName = "流程节点";
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
            /// 所属流程ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程ID")]
            public int FLOW_ID { get; set; }

            /// <summary>
            /// 流程节点名
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "流程名")]
            public string NAME { get; set; }

            /// <summary>
            /// 是否主节点
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否主节点")]
            public Int16 IS_MAIN { get; set; }

            /// <summary>
            /// 横坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "横坐标")]
            public int DIV_X { get; set; }

            /// <summary>
            /// 纵坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "纵坐标")]
            public int DIV_Y { get; set; }

            /// <summary>
            /// 节点说明
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "流程说明")]
            public string REMARK { get; set; }

            /// <summary>
            /// 用户权限IDS
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "用户权限IDS")]
            public string USER_IDS { get; set; }

            /// <summary>
            /// 角色权限IDS
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "用户权限IDS")]
            public string ROLE_IDS { get; set; }

            /// <summary>
            /// 组织权限IDS
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "用户权限IDS")]
            public string DPT_IDS { get; set; }

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

        public int SaveFlowNodes(int flowId, List<Entity> flowNodeList, out int addCount, out int updateCount, out int deleteCount)
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
            if (flowNodeList != null && flowNodeList.Count > 0)
            {
                for (int i = 0; i < flowNodeList.Count; i++)
                {
                    Entity entity = flowNodeList[i];
                    string fn = entity.NAME.ToUpper();
                    if (!dicNew.ContainsKey(fn))
                    {
                        dicNew.Add(fn, entity);
                    }
                }
            }
            List<Entity> listOld = GetFlowNodeList(flowId);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.NAME.ToUpper();
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

            foreach (Entity entity in flowNodeList)
            {
                string fn = entity.NAME.ToUpper();
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
        /// 根据流程ID获得节点集合
        /// </summary>
        /// <param name="flowId">流程ID</param>
        /// <returns></returns>
        public List<Entity> GetFlowNodeList(int flowId)
        {
            return GetList<Entity>("FLOW_ID=?", flowId).ToList();
        }
        /// <summary>
        /// 根据流程ID获得节点集合(Dictionary<string, Entity>)
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public Dictionary<string, Entity> GetDicFlowNodeList(int flowId)
        {
            var flowNodeList = GetFlowNodeList(flowId);
            Dictionary<string, Entity> dicNodeList = new Dictionary<string, Entity>();
            if (flowNodeList != null && flowNodeList.Count > 0)
            {
                for (int i = 0; i < flowNodeList.Count; i++)
                {
                    Entity entity = flowNodeList[i];
                    string fk = entity.NAME.ToUpper();
                    if (!dicNodeList.ContainsKey(fk))
                    {
                        dicNodeList.Add(fk, entity);
                    }
                }
            }
            return dicNodeList;
        }
    }
}
