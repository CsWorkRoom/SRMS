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
    /// 课题管理
    /// </summary>
    public class SR_PATENT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_PATENT Instance = new SR_PATENT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_PATENT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_PATENT";
            this.ItemName = "成果专利管理";
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
            /// 成果名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "成果名称")]
            public string ACHIEVEMENTS_NAME { get; set; }

            /// <summary>
            /// 专列类型
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "专列类型")]
            public int TYPE_ID { get; set; }

            /// <summary>
            /// 专利号
            /// </summary>
            [Field(IsNotNull = true,  Comment = "专利号")]
            public string  PATENT_NAME { get; set; }

            /// <summary>
            /// 期刊名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "期刊名称")]
            public string PERIODICAL_NAME { get; set; }

            /// <summary>
            /// 所属课题
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "所属课题")]
            public int TOPIC_ID { get; set; }

            /// <summary>
            /// 所属学科
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "所属学科")]
            public int SUBJECT_ID { get; set; }


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
            /// 课题描述
            /// </summary>
            [Field(IsNotNull = false, Comment = "课题描述")]
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

            /// <summary>
            /// 是否审批通过
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否审批通过")]
            public int IS_ADOPT { get; set; }
           
        }

        #endregion

     
    }
}
