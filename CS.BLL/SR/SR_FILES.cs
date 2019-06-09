using CS.Base.DBHelper;
using CS.Base.Log;
using CS.Common.FW;
using CS.Library.BaseQuery;
using CS.Library.Export;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CS.BLL.FW.BF_FORM.FieldInfo;
using static CS.Common.FW.Enums;

namespace CS.BLL.SR
{
    /// <summary>
    /// 业务文件管理
    /// </summary>
    public class SR_FILES : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_FILES Instance = new SR_FILES();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_FILES()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_FILES";
            this.ItemName = "业务文件管理";
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
            [Field(IsPrimaryKey = true, IsNotNull = true, Comment = "ID ")]
            public int ID { get; set; }

            /// <summary>
            /// 文件显示名
            /// </summary>
            [Field(IsNotNull = true, Length = 512, Comment = "文件显示名")]
            public string DISPLAY_NAME { get; set; }

            /// <summary>
            /// 文件真实名
            /// </summary>
            [Field(IsNotNull = true, Length = 512, Comment = "文件真实名")]
            public string REAL_NAME { get; set; }

            /// <summary>
            /// 文件路径
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "文件路径")]
            public string PATH { get; set; }

            /// <summary>
            /// 文件格式
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "文件格式")]
            public string FORMAT { get; set; }

            /// <summary>
            /// 文件大小
            /// </summary>
            [Field(IsNotNull = true,Comment = "文件大小")]
            public double FILE_SIZE { get; set; }

            /// <summary>
            /// 文件归属标识
            /// </summary>
            [Field(Length = 512, Comment = "文件归属标识")]
            public string BELONG_TAG { get; set; }

            /// <summary>
            /// 文件说明
            /// </summary>
            [Field(Length = 512, Comment = "文件说明")]
            public string REMARK { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 是否删除
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否删除")]
            public short IS_DELETE { get; set; }

            /// <summary>
            /// 删除人
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "删除人")]
            public int DELETE_UID { get; set; }

            /// <summary>
            /// 删除时间
            /// </summary>
            [Field( Comment = "删除时间")]
            public DateTime? DELETE_TIME { get; set; }

            /// <summary>
            /// 创建者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "创建者ID")]
            public int CREATE_UID { get; set; }
            
        }
        #endregion
        
    }
}
