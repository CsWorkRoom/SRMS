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
    /// 银行卡信息
    /// </summary>
    public class SR_BANK : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_BANK Instance = new SR_BANK();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_BANK()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_BANK";
            this.ItemName = "银行卡信息";
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

            /// <summary>
            /// 是否默认
            /// </summary>
            [Field(IsNotNull = true, Comment = "是否默认")]
            public short IS_DEFAULT { get; set; }

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
