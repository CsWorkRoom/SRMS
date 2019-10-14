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
    /// 经费报销总表
    /// </summary>
    public class SR_PAGE_RECORD_FUNDS : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_PAGE_RECORD_FUNDS Instance = new SR_PAGE_RECORD_FUNDS();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_PAGE_RECORD_FUNDS()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_PAGE_RECORD_FUNDS";
            this.ItemName = "版面费报销总表";
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
            /// 论文ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "论文ID")]
            public int PAPER_RECORD_ID { get; set; }

            /// <summary>
            /// 报销总金额
            /// </summary>
            [Field(IsNotNull = true, Comment = "报销总金额")]
            public double TOTAL_FEE { get; set; }

            /// <summary>
            /// 报销说明
            /// </summary>
            [Field(IsNotNull = false, Comment = "报销说明")]
            public string REMARK { get; set; }

            /// <summary>
            /// 附件IDS(编号以英文逗号分隔)
            /// 使用附件控件自动存储
            /// </summary>
            [Field(IsNotNull = false, Comment = "附件IDS")]
            public string FILES { get; set; }

            #region 银行卡信息
            /// <summary>
            /// 银行名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "银行名称")]
            public string BANK_NAME { get; set; }
            /// <summary>
            /// 银行账户
            /// </summary>
            [Field(IsNotNull = true, Comment = "银行账户")]
            public string BANK_NO { get; set; }
            /// <summary>
            /// 开户地址
            /// </summary>
            [Field(IsNotNull = false, Comment = "开户地址")]
            public string BANK_ADDRESS { get; set; }
            /// <summary>
            /// 联系人
            /// </summary>
            [Field(IsNotNull = false, Comment = "联系人")]
            public string USER_NAME { get; set; }
            /// <summary>
            /// 联系电话
            /// </summary>
            [Field(IsNotNull = false, Comment = "联系电话")]
            public string USER_PHONE { get; set; }
            #endregion

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }


            /// <summary>
            /// 创建人
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建人")]
            public int CREATE_UID { get; set; }
        }
        #endregion

    }
}
