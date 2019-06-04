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
    /// 预算类型
    /// </summary>
    public class SR_BUDGET_TYPE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_BUDGET_TYPE Instance = new SR_BUDGET_TYPE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_BUDGET_TYPE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_BUDGET_TYPE";
            this.ItemName = "预算类型";
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
            /// 父节点
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "父节点")]
            public int PARENT_ID { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注")]
            public string REAMRK { get; set; }

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
    }
}
