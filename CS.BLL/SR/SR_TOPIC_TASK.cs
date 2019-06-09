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
    /// 课题中期检查任务
    /// </summary>
    public class SR_TOPIC_TASK : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_TASK Instance = new SR_TOPIC_TASK();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_TASK()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_TASK";
            this.ItemName = "课题中期检查任务";
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
            /// 课题ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题ID")]
            public int TOPIC_ID { get; set; }

            /// <summary>
            /// 任务名
            /// </summary>
            [Field(IsNotNull = true, Comment = "任务名")]
            public string NAME { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            [Field(Comment = "开始时间")]
            public DateTime? BEGIN_TIME { get; set; }

            /// <summary>
            /// 结束时间
            /// </summary>
            [Field(Comment = "结束时间")]
            public DateTime? END_TIME { get; set; }

            /// <summary>
            /// 任务详细要求
            /// </summary>
            [Field(IsNotNull = false, Comment = "任务详细要求")]
            public string REMARK { get; set; }

            /// <summary>
            /// 附件IDS(编号以英文逗号分隔)
            /// 使用附件控件自动存储
            /// </summary>
            [Field(IsNotNull = false, Comment = "附件IDS")]
            public string FILES { get; set; }

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
