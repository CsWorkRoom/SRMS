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

namespace CS.BLL.FW
{
    /// <summary>
    /// 流程定义
    /// </summary>
    public class BF_FLOW : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FLOW Instance = new BF_FLOW();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FLOW()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FLOW";
            this.ItemName = "流程定义";
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
            /// 流程名
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "流程名")]
            public string NAME { get; set; }

            /// <summary>
            /// 流程类型
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "流程类型")]
            public int FLOW_TYPE_ID { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否启用")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 流程说明
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "流程说明")]
            public string REMARK { get; set; }

            /// <summary>
            /// 主表单表
            /// </summary>
            [Field(IsNotNull = true, Length = 128, Comment = "主表单表")]
            public string MAIN_TABLE { get; set; }

            /// <summary>
            /// 主页面
            /// </summary>
            [Field(IsNotNull = true, Length = 512, Comment = "主页面")]
            public string MAIN_PAGE { get; set; }

            /// <summary>
            /// 主页面提交函数
            /// </summary>
            [Field(IsNotNull = true, Length = 128, Comment = "主表单表")]
            public string MAIN_FUN { get; set; }

            /// <summary>
            /// 创建者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "创建者ID")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 修改者者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "修改者者ID")]
            public int UPDATE_UID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 修改时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "修改时间")]
            public DateTime UPDATE_TIME { get; set; }
        }
        #endregion

        public int SetEnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 1);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetUnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }
    }
}
