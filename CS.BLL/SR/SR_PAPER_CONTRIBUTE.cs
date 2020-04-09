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
    /// 论文投稿
    /// </summary>
    public class SR_PAPER_CONTRIBUTE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_PAPER_CONTRIBUTE Instance = new SR_PAPER_CONTRIBUTE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_PAPER_CONTRIBUTE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_PAPER_CONTRIBUTE";
            this.ItemName = "论文投稿";
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
            /// 论文名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "论文名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 文章类型
            /// </summary>
            [Field(IsNotNull = true, Comment = "文章类型")]
            public int ARTICLE_TYPE_ID { get; set; }

            /// <summary>
            /// 课题ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "课题ID")]
            public int TOPIC_ID { get; set; }

            /// <summary>
            /// 学科
            /// </summary>
            [Field(IsNotNull = true, Comment = "学科")]
            public int SUBJECT_ID { get; set; }

            /// <summary>
            /// 投稿人
            /// </summary>
            [Field(IsNotNull = true, Comment = "投稿人")]
            public string CONTRIBUTOR { get; set; }

            /// <summary>
            /// 联系电话
            /// </summary>
            [Field(IsNotNull = true, Comment = "联系电话")]
            public string PHONE { get; set; }

            /// <summary>
            /// 论文内容
            /// </summary>
            [Field(IsNotNull = true, Comment = "论文内容")]
            public string CONTENT { get; set; }

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

            #region 审批状态
            /// <summary>
            /// 审批状态
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批状态")]
            public int FLOW_STATE { get; set; }

            /// <summary>
            /// 是否审批通过
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否审批通过")]
            public int IS_ADOPT { get; set; }
            #endregion
        }
        #endregion

    }
}
