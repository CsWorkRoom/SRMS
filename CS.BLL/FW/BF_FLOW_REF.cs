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
    /// 流程间关联
    /// </summary>
    public class BF_FLOW_REF : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FLOW_REF Instance = new BF_FLOW_REF();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW_REF()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW_REF";
            this.ItemName = "流程间关联";
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
            /// 上级流程ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "上级流程ID")]
            public int PARENT_ID { get; set; }

            /// <summary>
            /// 当前流程ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "当前流程ID")]
            public int FLOW_ID { get; set; }
        }
        #endregion
        /// <summary>
        /// 获取下级流程集合
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public List<Entity> GetNextFlow(int flowId)
        {
            return Instance.GetList<Entity>("PARENT_ID=?", flowId).ToList();
        }
    }
}
