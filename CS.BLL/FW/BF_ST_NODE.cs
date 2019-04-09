using CS.Base.DBHelper;
using CS.Base.Log;
using CS.Common.FW;
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
    /// 脚本节点
    /// </summary>
    public class BF_ST_NODE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_NODE Instance = new BF_ST_NODE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_NODE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_NODE";
            this.ItemName = "脚本节点";
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
            /// 节点名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, IsIndex = true, IsIndexUnique = true, Comment = "节点名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 脚本类型
            /// </summary>
            [Field(IsNotNull = true, Comment = "脚本类型")]
            public int TYPE_ID { get; set; }

            /// <summary>
            /// 数据库ID（为0时表示默认数据库）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "数据库ID（为0时表示默认数据库）")]
            public int DB_ID { get; set; }

            /// <summary>
            /// 脚本内容
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "脚本内容")]
            public string CONTENT { get; set; }

            /// <summary>
            /// 运行状态（对应枚举：RunStatus)
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "运行状态（对应枚举：RunStatus)")]
            public Int16 RUN_STATUS { get; set; }

            /// <summary>
            /// 最新任务是否成功
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "最新任务是否成功")]
            public Int16 LAST_TASK_IS { get; set; }

            /// <summary>
            /// 最新任务ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "最新任务ID")]
            public int LAST_TASK_ID { get; set; }

            /// <summary>
            /// 最新任务开始时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "最新任务开始时间")]
            public DateTime LAST_TASK_ST { get; set; }

            /// <summary>
            /// 最新任务完成时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "最新任务完成时间")]
            public DateTime LAST_TASK_FT { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注")]
            public string REMARK { get; set; }

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

        /// <summary>
        /// 设置最新任务ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public int SetLastTaskID(int id, int taskID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.运行.GetHashCode());
            dic.Add("LAST_TASK_ID", taskID);
            dic.Add("LAST_TASK_ST", DateTime.Now);
            dic.Add("LAST_TASK_FT", null);
            dic.Add("LAST_TASK_IS", 0);
            return Update(dic, "ID=?", id);
        }

        #region 设置最新任务执行结果
        /// <summary>
        /// 设置最新任务执行结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskID"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public int SetLastTaskResult(int id, int taskID, bool isSuccess)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (taskID > 0)
            {
                dic.Add("LAST_TASK_ID", taskID);
            }
            dic.Add("RUN_STATUS", Enums.RunStatus.结束.GetHashCode());
            dic.Add("LAST_TASK_FT", DateTime.Now);
            dic.Add("LAST_TASK_IS", isSuccess ? 1 : 0);
            return Update(dic, "ID=?", id);
        }

        #endregion

        #region 启动一个节点任务（会自动添加一个新的任务到表BF_ST_TASK）
        /// <summary>
        /// 启动一个节点任务（会自动添加一个新的任务到表BF_ST_TASK）
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="referenceDate">基准日期</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="parameters">手动参数</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否执行成功</returns>
        public bool Start(int nodeID, DateTime referenceDate, DateTime beginDate, DateTime endDate, string parameters, ref string message)
        {
            try
            {
                Entity ne = GetEntityByKey<Entity>(nodeID);
                if (ne == null)
                {
                    message = "未找到节点【" + nodeID + "】";
                    return false;
                }
                if (ne.RUN_STATUS == (short)Enums.RunStatus.运行)
                {
                    message = "在执行任务【" + ne.LAST_TASK_ID + "】，无法启动新任务";
                    return false;
                }
                int i = BF_ST_TASK.Instance.AddNodeTask(nodeID, true, referenceDate, beginDate, endDate, parameters, "手动添加节点任务");
                if (i == 1)
                {
                    message = "已经创建新的节点任务，请稍后刷新查看运行情况";
                    return true;
                }
                message = "创建节点任务失败";
            }
            catch (Exception ex)
            {
                message = "创建节点任务出错，详见日志";
                BLog.Write(BLog.LogLevel.WARN, "创建节点【" + nodeID + "】任务出错：" + ex.ToString());
            }

            return false;
        }
        #endregion

        #region 停止节点任务（停止LAST_TASK_ID字段中值对应的任务）

        /// <summary>
        /// 停止节点任务（停止LAST_TASK_ID字段中值对应的任务）
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否执行成功</returns>
        public bool Stop(int nodeID, ref string message)
        {
            try
            {
                Entity ne = GetEntityByKey<Entity>(nodeID);
                if (ne == null)
                {
                    message = "未找到节点【" + nodeID + "】";
                    return false;
                }
                if (ne.RUN_STATUS == (short)Enums.RunStatus.结束 || ne.LAST_TASK_ID < 1)
                {
                    message = "节点当前未执行任务或当前任务已经停止";
                    return false;
                }

                //节点节点最新任务运行状态
                SetLastTaskResult(nodeID, 0, false);

                //对应的任务
                BF_ST_TASK.Entity te = BF_ST_TASK.Instance.GetEntityByKey<BF_ST_TASK.Entity>(ne.LAST_TASK_ID);
                if (te == null)
                {
                    message = "未找到节点的最新任务【" + ne.LAST_TASK_ID + "】";
                    return false;
                }
                if (te.RUN_STATUS == (short)Enums.RunStatus.结束)
                {
                    message = "节点的最新任务【" + ne.LAST_TASK_ID + "】已经停止";
                    return false;
                }

                //停止任务
                return BF_ST_TASK.Instance.Stop(ne.LAST_TASK_ID, ref message);
            }
            catch (Exception ex)
            {
                message = "停止节点任务出错，详见日志";
                BLog.Write(BLog.LogLevel.WARN, "停止节点【" + nodeID + "】任务出错：" + ex.ToString());
            }

            return false;
        }
        #endregion

        #region 查询列表
        public DataTable GetDataTable(int limit, int page, ref int count, string name, string typeId, string dbId, int self=0, string orderByField = "sn.ID", string orderByType = "ASC")
        {
            string strWhere = "1=1";
            List<object> param = new List<object>();
            #region 添加参数
            if (string.IsNullOrWhiteSpace(name) == false)
                strWhere += " AND SN.NAME LIKE '%" + name.Replace('\'', ' ') + "%'";

            if (string.IsNullOrWhiteSpace(typeId) == false)
            {
                int type = Convert.ToInt32(typeId);
                List<int> types = BF_ST_TYPE.Instance.GetAllChildren(type);
                if (types.Count > 0)
                {
                    strWhere += string.Format(" AND SN.TYPE_ID IN ({0})", string.Join(",", types));
                }
                //param.Add(typeId);
            }
            if (string.IsNullOrWhiteSpace(dbId) == false)
            {
                strWhere += " AND SN.DB_ID = ?";
                param.Add(dbId);
            }
            if (self > 0)
            {
                strWhere += " AND SN.CREATE_UID = ?";
                param.Add(SystemSession.UserID);
            }
            #endregion

            string strSql = "select sn.id, sn.NAME, stp.name typeName,sn.LAST_TASK_ID,SFL.name TaskName,db.name dbName,sn.RUN_STATUS,sn.LAST_TASK_IS,sn.LAST_TASK_ST,sn.LAST_TASK_FT,sn.CREATE_TIME ";
            strSql += ",(select FULL_NAME from BF_USER WHERE ID=SN.CREATE_UID )CREATE_NAME,sn.UPDATE_TIME,(select FULL_NAME from BF_USER WHERE ID=SN.UPDATE_UID )UPDATE_NAME ";
            strSql += " from BF_ST_NODE sn left join BF_ST_flow sfl on sn.LAST_TASK_ID = SFL.ID left join BF_DATABASE db on SN.DB_ID = DB.ID left join BF_ST_TYPE stp on sn.type_id = stp.id where " + strWhere;
            //添加排序
            if (string.IsNullOrWhiteSpace(orderByField) == false)
                strSql += " ORDER BY " + orderByField + " " + (string.IsNullOrWhiteSpace(orderByType) == false ? orderByType : "");

            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0 && page == 0)
                {
                    return dbHelper.ExecuteDataTableParams(strSql, param);//不分页查询所有
                }
                //算总记录
                if (count == 0)
                {
                    string sqlCount = string.Format("SELECT COUNT(*) FROM ({0})", strSql);
                    count = dbHelper.ExecuteScalarIntParams(sqlCount, param);
                }
                return dbHelper.ExecuteDataTablePageParams(strSql, limit, page, param);
            }

        }
        #endregion

        #region 查询日志明细
        public DataTable GetLogDataTable(int limit, int page, int stakId, int nodeId, ref int count, string strMessage = "", string strSql = "", string orderByField = "STFNL.ID", string orderByType = "DESC")
        {
            string sql = @"SELECT STFNL.ID,SFL.NAME FLOWNAME,STFNL.LOG_TIME,STFNL.LOG_LEVEL,STFNL.MESSAGE,STFNL.SQL 
FROM BF_ST_TASK_FLOW_NODE_LOG STFNL LEFT JOIN BF_ST_NODE SNO ON STFNL.NODE_ID = SNO.ID LEFT JOIN BF_ST_FLOW SFL ON STFNL.FLOW_ID = SFL.ID
WHERE STFNL.TASK_ID =? AND STFNL.NODE_ID=? ";

            if (string.IsNullOrWhiteSpace(strMessage) == false)
                sql += " and stfnl.message LIKE '%" + strMessage.Replace('\'', ' ') + "%'";
            if (string.IsNullOrWhiteSpace(strSql) == false)
                sql += " and stfnl.SQL LIKE '%" + strSql.Replace('\'', ' ') + "%'";
            //添加排序
            if (string.IsNullOrWhiteSpace(orderByField) == false)
                sql += " ORDER BY " + orderByField + " " + (string.IsNullOrWhiteSpace(orderByType) == false ? orderByType : "");

            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0)
                {
                    return dbHelper.ExecuteDataTableParams(sql, stakId, nodeId);//不分页查询所有
                }
                //算总记录
                if (count == 0)
                {
                    string sqlCount = string.Format("SELECT COUNT(*) FROM ({0})", sql);
                    count = dbHelper.ExecuteScalarIntParams(sqlCount, stakId, nodeId);
                }
                return dbHelper.ExecuteDataTablePageParams(sql, limit, page, stakId, nodeId);
            }

        }
        #endregion
    }
}