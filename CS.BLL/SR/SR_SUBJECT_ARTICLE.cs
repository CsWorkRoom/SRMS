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
    /// 学科学习资料
    /// </summary>
    public class SR_SUBJECT_ARTICLE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_SUBJECT_ARTICLE Instance = new SR_SUBJECT_ARTICLE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_SUBJECT_ARTICLE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_SUBJECT_ARTICLE";
            this.ItemName = "学科学习资料";
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
            /// 学科学习资料名
            /// </summary>
            [Field(IsNotNull = true, Comment = "论文名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 学科
            /// </summary>
            [Field(IsNotNull = true, Comment = "学科")]
            public int SUBJECT_ID { get; set; }

            /// <summary>
            /// 论文内容
            /// </summary>
            [Field(IsNotNull = true, Comment = "论文内容")]
            public string CONTENT { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, Comment = "是否启用")]
            public short IS_ENABLE { get; set; }

            /// <summary>
            /// 附件IDS(编号以英文逗号分隔)
            /// 使用附件控件自动存储
            /// </summary>
            [Field(IsNotNull = false, Comment = "附件IDS")]
            public string FILES { get; set; }

            #region 操作人
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
            #endregion
        }
        #endregion

    }
}
