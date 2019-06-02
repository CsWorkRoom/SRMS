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
    /// 课题完善信息
    /// </summary>
    public class SR_TOPIC_DETAIL : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_DETAIL Instance = new SR_TOPIC_DETAIL();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_DETAIL()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_DETAIL";
            this.ItemName = "课题完善信息";
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
            /// 学科ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "学科ID")]
            public int SUBJECT_ID { get; set; }

            /// <summary>
            /// 归属单位(组织ID)
            /// </summary>
            [Field(IsNotNull = true, Comment = "归属单位(组织ID)")]
            public int DEPARTMENT_ID { get; set; }

            /// <summary>
            /// 会计类型ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "会计类型ID")]
            public int ACCOUNTING_TYPE_ID { get; set; }

            /// <summary>
            /// 项目摘要
            /// </summary>
            [Field(IsNotNull = false, Comment = "项目摘要")]
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

        #region 支持的底层方法(一般为对表的增删改操作)
        /// <summary>
        /// 添加一个学科完善信息
        /// </summary>
        /// <param name="ent"></param>
        public void Add(Entity ent)
        {

        }

        public void Update(Entity ent)
        { }
        #endregion

    }
}
