using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 任务日志
    /// </summary>
    public class BF_ST_TASK_LOG : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_TASK_LOG Instance = new BF_ST_TASK_LOG();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_TASK_LOG()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_TASK_LOG";
            this.ItemName = "任务日志";
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
            /// 任务ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "任务ID")]
            public int TASK_ID { get; set; }

            /// <summary>
            /// 日志时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "日志时间")]
            public DateTime LOG_TIME { get; set; }

            /// <summary>
            /// 日志等级
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "3", Comment = "日志等级")]
            public Int16 LOG_LEVEL { get; set; }

            /// <summary>
            /// 日志内容
            /// </summary>
            [Field(IsNotNull = true, Length = 2048, Comment = "日志内容")]
            public string MESSAGE { get; set; }

        }

        #endregion

        /// <summary>
        /// 添加一条日志
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        /// <returns></returns>
        public int Add(int taskID, int level, string message)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("TASK_ID", taskID);
            dic.Add("LOG_TIME", DateTime.Now);
            dic.Add("LOG_LEVEL", level);
            dic.Add("MESSAGE", message);

            return Add(dic);
        }
    }
}
