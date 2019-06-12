using CS.Common;
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
    /// 论文管理
    /// </summary>
    public class SR_PAPER : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_PAPER Instance = new SR_PAPER();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_PAPER()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_PAPER";
            this.ItemName = "论文管理";
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
            [Field(IsNotNull = true, Length = 1024, Comment = "论文名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 论文类型
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "论文类型")]
            public int PAPER_TYPE_ID { get; set; }

            /// <summary>
            /// 所属课题
            /// </summary>
            [Field( Length = 256, Comment = "所属课题")]
            public int TOPIC_ID { get; set; }


            /// <summary>
            /// 所属学科
            /// </summary>
            [Field(Length = 256, Comment = "所属学科")]
            public int SUBJECT_ID { get; set; }

            /// <summary>
            /// 第一作者
            /// </summary>
            [Field(Length = 256, Comment = "第一作者")]
            public string FIRST_AUTHOR { get; set; }

            /// <summary>
            /// 其他人员
            /// </summary>
            [Field(Length = 256, Comment = "其他人员")]
            public string OTHER_AUTHOR { get; set; }

            /// <summary>
            /// 版面费用
            /// </summary>
            [Field(Length = 256, Comment = "版面费用")]
            public double PAGE_AMOUNT { get; set; }

            /// <summary>
            /// 创建人
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建人")]
            public int CREATE_USER_ID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW",Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 论文描述
            /// </summary>
            [Field(IsNotNull = false, Comment = "论文描述")]
            public string REMARK { get; set; }

            /// <summary>
            /// 附件IDS
            /// </summary>
            [Field(IsNotNull = false, Comment = "附件IDS")]
            public string FILES { get; set; }

            /// <summary>
            /// 审批状态
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批状态")]
            public int FLOW_STATE { get; set; }
        }

        #endregion

   

    }
}
