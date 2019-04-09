using CS.Base.Log;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class BF_OP_LOG : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_OP_LOG Instance = new BF_OP_LOG();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_OP_LOG()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_OP_LOG";
            this.ItemName = "操作日志";
            this.KeyField = "ID";
            this.OrderbyFields = "ID DESC";
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
            /// 日志等级
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "3", Comment = "日志等级 ")]
            public Int16 LOG_LEVEL { get; set; }
            /// <summary>
            /// 日志时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", IsIndex = true, Comment = "日志时间")]
            public DateTime LOG_TIME { get; set; }
            /// <summary>
            /// 是否成功
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否成功 ")]
            public Int16 IS_SUCCESS { get; set; }
            /// <summary>
            /// 源IP
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "源IP")]
            public string SRC_IP { get; set; }
            /// <summary>
            /// 源端口
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "源端口")]
            public int SRC_PORT { get; set; }
            /// <summary>
            /// 登录用户ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", IsIndex = true, Comment = "登录用户ID")]
            public int USER_ID { get; set; }
            /// <summary>
            /// 登录用户名
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "-", Length = 64, IsIndex = true, Comment = "登录用户名")]
            public string USER_NAME { get; set; }
            /// <summary>
            /// 控制器名称
            /// </summary>
            [Field(IsNotNull = false, Length = 128, Comment = "控制器名称")]
            public string CONTROLLER { get; set; }
            /// <summary>
            /// ACTION名称
            /// </summary>
            [Field(IsNotNull = false, Length = 128, Comment = "ACTION名称")]
            public string ACTION { get; set; }
            /// <summary>
            /// 请求URL地址
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "请求URL地址")]
            public string REQ_URL { get; set; }
            /// <summary>
            /// 日志内容
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "请求内容")]
            public string CONTENT { get; set; }
            /// <summary>
            /// 日志详情
            /// </summary>
            [Field(IsNotNull = false, Length = 2048, Comment = "源IP")]
            public string DETAIL { get; set; }
        }
        #endregion

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="isSuccess"></param>
        /// <param name="srcIP"></param>
        /// <param name="srcPort"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public int AddLog(BLog.LogLevel logLevel, bool isSuccess, string srcIP, int srcPort, string controller, string action, string url, string content, string detail = "")
        {
            Entity entity = new Entity();
            entity.LOG_LEVEL = (short)logLevel;
            entity.LOG_TIME = DateTime.Now;
            entity.IS_SUCCESS = (short)(isSuccess ? 1 : 0);
            entity.SRC_IP = srcIP;
            entity.SRC_PORT = srcPort;

            entity.USER_ID = SystemSession.UserID;
            if (SystemSession.UserID > 0)
            {
                entity.USER_NAME = SystemSession.UserName;
            }
            else
            {
                entity.USER_NAME = "未知用户";
            }
            entity.CONTROLLER = controller;
            entity.ACTION = action;
            entity.REQ_URL = url;
            entity.CONTENT = content;
            entity.DETAIL = detail;

            return Add(entity);
        }
    }
}
