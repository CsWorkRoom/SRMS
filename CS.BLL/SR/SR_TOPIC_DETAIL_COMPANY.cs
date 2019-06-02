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
            /// 单位联系电话
            /// </summary>
            [Field(IsNotNull = false, Comment = "单位联系电话")]
            public string PHONE { get; set; }

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
        #endregion
    }
}
