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
    /// 论文备案
    /// </summary>
    public class SR_PAPER_RECORD : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_PAPER_RECORD Instance = new SR_PAPER_RECORD();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_PAPER_RECORD()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_PAPER_RECORD";
            this.ItemName = "论文备案";
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
            /// 期刊名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "期刊名称")]
            public string JOURNAL_NAME { get; set; }

            /// <summary>
            /// 期刊号
            /// </summary>
            [Field(IsNotNull = true, Comment = "期刊号")]
            public string JOURNAL_NO { get; set; }

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
            /// 第一作者
            /// </summary>
            [Field(IsNotNull = true, Comment = "第一作者")]
            public string FIRST_AUTHOR { get; set; }

            /// <summary>
            /// 版面费
            /// </summary>
            [Field(Comment = "版面费")]
            public double PAGE_FEE { get; set; }

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
