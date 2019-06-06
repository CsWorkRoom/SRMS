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
    /// 课题中期任务完成情况
    /// </summary>
    public class SR_TOPIC_TASK_DONE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_TASK_DONE Instance = new SR_TOPIC_TASK_DONE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_TASK_DONE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_TASK_DONE";
            this.ItemName = "课题中期任务完成情况";
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
            /// 课题任务ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题任务ID")]
            public int TOPIC_TASK_ID { get; set; }

            /// <summary>
            /// 完成内容描述
            /// </summary>
            [Field(IsNotNull = true, Comment = "完成内容描述")]
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
