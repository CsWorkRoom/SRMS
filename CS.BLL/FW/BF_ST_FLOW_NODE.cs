using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 脚本流与节点的绑定
    /// </summary>
    public class BF_ST_FLOW_NODE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_FLOW_NODE Instance = new BF_ST_FLOW_NODE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_FLOW_NODE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_FLOW_NODE";
            this.ItemName = "脚本流与节点的绑定";
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
            /// 脚本流ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "脚本流ID")]
            public int FLOW_ID { get; set; }

            /// <summary>
            /// 脚本节点ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "脚本节点ID")]
            public int NODE_ID { get; set; }

            /// <summary>
            /// 前序节点（集合，没有则为空，有则逗号分隔）
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "前序节点（集合，没有则为空，有则逗号分隔）")]
            public string PRE_NODE_IDS { get; set; }

            /// <summary>
            /// 页面展示时DIV位置X坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "页面展示时DIV位置X坐标")]
            public int DIV_X { get; set; }

            /// <summary>
            /// 页面展示时DIV位置Y坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "页面展示时DIV位置Y坐标")]
            public int DIV_Y { get; set; }
        }
        #endregion

        /// <summary>
        /// 获取脚本的相关节点
        /// </summary>
        /// <param name="flowID">脚本ID</param>
        /// <returns></returns>
        public IList<Entity> GetListByFlowID(int flowID)
        {
            return GetList<Entity>("FLOW_ID=?", flowID);
        }

        /// <summary>
        /// 根据ID取名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameByID(int id)
        {
            Dictionary<int, string> dic = GetDictionary("ID", "NAME");
            if (dic.ContainsKey(id))
            {
                return dic[id];
            }
            return "未知";
        }
    }
}
