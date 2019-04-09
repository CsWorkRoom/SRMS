using CS.Base.DBHelper;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 节点运行日志
    /// </summary>
    public class BF_ST_TASK_FLOW_NODE_LOG : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_TASK_FLOW_NODE_LOG Instance = new BF_ST_TASK_FLOW_NODE_LOG();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_TASK_FLOW_NODE_LOG()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_TASK_FLOW_NODE_LOG";
            this.ItemName = "节点运行日志";
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
            /// BF_ST_TASK_FLOW_NODE表主键
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "BF_ST_TASK_FLOW_NODE表主键")]
            public int TFN_ID { get; set; }

            /// <summary>
            /// 任务ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "任务ID")]
            public int TASK_ID { get; set; }

            /// <summary>
            /// 脚本ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "脚本节点ID")]
            public int FLOW_ID { get; set; }

            /// <summary>
            /// 脚本节点ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "脚本节点ID")]
            public int NODE_ID { get; set; }

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

            /// <summary>
            /// 执行的SQL语句
            /// </summary>
            [Field(IsNotNull = false, Length = 4096, Comment = "执行的SQL语句")]
            public string SQL { get; set; }

        }

        #endregion

        #region 添加一条日志
        /// <summary>
        /// 添加一条日志
        /// </summary>
        /// <param name="tfnID">TFN_ID</param>
        /// <param name="taskID">任务ID</param>
        /// <param name="flowID">脚本ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public int Add(int tfnID, int taskID, int flowID, int nodeID, int level, string message, string sql = "")
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("TFN_ID", tfnID);
            dic.Add("TASK_ID", taskID);
            dic.Add("FLOW_ID", flowID);
            dic.Add("NODE_ID", nodeID);
            dic.Add("LOG_TIME", DateTime.Now);
            dic.Add("LOG_LEVEL", level);
            dic.Add("MESSAGE", message);
            dic.Add("SQL", sql);

            return Add(dic);
        }
        #endregion

        #region 查询日志明细
        public DataTable GetDataTable(int limit, int page,string strWhere, List<object> value)
        {
            string strSql = "select sfl.name flowName,sno.name nodeName,stfnl.log_time,stfnl.log_level,stfnl.message,stfnl.sql from BF_ST_TASK_FLOW_NODE_LOG stfnl left join BF_ST_NODE sno on STFNL.NODE_ID=SNO.ID left join BF_ST_flow sfl on STFNL.FLOW_ID=SFL.ID " + strWhere+" order by id";
            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0 && page == 0)
                {
                    return dbHelper.ExecuteDataTable(strSql);//不分页查询所有
                }
                return dbHelper.ExecuteDataTablePageParams(strSql, limit, page, value);
            }

        } 
        #endregion 
    }
}

