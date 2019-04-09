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
    /// 脚本任务
    /// </summary>
    public class BF_ST_TASK : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_TASK Instance = new BF_ST_TASK();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_TASK()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_TASK";
            this.ItemName = "脚本任务";
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
            /// 脚本流ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "脚本流ID")]
            public int FLOW_ID { get; set; }

            /// <summary>
            /// 脚本节点ID
            /// </summary>
            [Field(IsNotNull = true, Comment = "脚本节点ID")]
            public int NODE_ID { get; set; }

            /// <summary>
            /// 是否为手动任务
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否为手动任务")]
            public int IS_MANUAL { get; set; }

            /// <summary>
            /// 基准日期
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "基准日期")]
            public DateTime REFERENCE_DATE { get; set; }

            /// <summary>
            /// 开始日期
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "NOW", Comment = "开始日期")]
            public DateTime BEGIN_DATE { get; set; }

            /// <summary>
            /// 截止日期
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "NOW", Comment = "截止日期")]
            public DateTime END_DATE { get; set; }

            /// <summary>
            /// 参数值（格式：参数值[,参数值]）
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "（格式：参数值[,参数值]）")]
            public string PARAMETER { get; set; }

            /// <summary>
            /// 失败重试次数
            /// </summary>

            [Field(IsNotNull = true, DefaultValue = "0", Comment = "失败重试次数")]
            public int RETRY_TIMES { get; set; }

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
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "更新时间")]
            public DateTime UPDATE_TIME { get; set; }

            /// <summary>
            /// 运行状态（对应枚举：RunStatus)
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "运行状态（对应枚举：RunStatus)")]
            public Int16 RUN_STATUS { get; set; }

            /// <summary>
            /// 是否成功
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否成功")]
            public Int16 IS_SUCCESS { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "开始时间")]
            public DateTime START_TIME { get; set; }

            /// <summary>
            /// 完成时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "完成时间")]
            public DateTime FINISH_TIME { get; set; }
        }
        #endregion

        /// <summary>
        /// 添加一个脚本任务
        /// </summary>
        /// <param name="flowID">脚本组ID</param>
        /// <param name="isManual">是否手动</param>
        /// <param name="referenceDate">基准日期</param>
        /// <param name="beginDate">起始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="parameter">手动参数（手动任务有效）</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public int AddFlowTask(int flowID, bool isManual, DateTime referenceDate, DateTime beginDate, DateTime endDate, string parameter, string remark)
        {
            if (flowID < 1)
            {
                throw new Exception("脚本ID不可小于1");
            }

            BF_ST_FLOW.Entity fe = BF_ST_FLOW.Instance.GetEntityByKey<BF_ST_FLOW.Entity>(flowID);
            if (fe == null)
            {
                throw new Exception("脚本ID【" + flowID + "】不存在");
            }
            if (fe.IS_ENABLE != 1)
            {
                throw new Exception("脚本ID【" + flowID + "】已停用");
            }
            if (fe.RUN_STATUS > (Int16)Enums.RunStatus.等待.GetHashCode() && fe.RUN_STATUS < (Int16)Enums.RunStatus.结束.GetHashCode())
            {
                throw new Exception(string.Format("脚本ID【{0}】的任务【{1}】仍未完成", flowID, fe.LAST_TASK_ID));
            }
            if (beginDate.Year != endDate.Year || beginDate.Month != endDate.Month)
            {
                throw new Exception(remark + " 起始日期" + beginDate + "和截止日期" + endDate + "必须同月");
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("FLOW_ID", flowID);
            dic.Add("NODE_ID", 0);
            dic.Add("IS_MANUAL", isManual ? 1 : 0);
            dic.Add("REFERENCE_DATE", referenceDate.Date);
            dic.Add("BEGIN_DATE", beginDate.Date);
            dic.Add("END_DATE", endDate.Date);
            dic.Add("PARAMETER", parameter);
            dic.Add("RETRY_TIMES", fe.RETRY_TIMES);
            dic.Add("REMARK", remark);
            dic.Add("CREATE_UID", isManual ? SystemSession.UserID : 0);
            dic.Add("UPDATE_UID", isManual ? SystemSession.UserID : 0);
            dic.Add("CREATE_TIME", DateTime.Now);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("RUN_STATUS", Enums.RunStatus.等待.GetHashCode());

            return Add(dic);
        }

        /// <summary>
        /// 添加一个节点任务
        /// </summary>
        /// <param name="nodeID">脚本节点ID</param>
        /// <param name="isManual">是否手动</param>
        /// <param name="referenceDate">基准日期</param>
        /// <param name="beginDate">起始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="parameters">手动参数（手动任务有效）</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public int AddNodeTask(int nodeID, bool isManual, DateTime referenceDate, DateTime beginDate, DateTime endDate, string parameters, string remark)
        {
            if (nodeID < 1)
            {
                throw new Exception("节点ID不可小于1");
            }

            BF_ST_NODE.Entity fe = BF_ST_NODE.Instance.GetEntityByKey<BF_ST_NODE.Entity>(nodeID);
            if (fe == null)
            {
                throw new Exception("节点ID【" + nodeID + "】不存在");
            }
            if (fe.RUN_STATUS > (Int16)Enums.RunStatus.等待.GetHashCode() && fe.RUN_STATUS < (Int16)Enums.RunStatus.结束.GetHashCode())
            {
                throw new Exception(string.Format("节点ID【{0}】的任务【{1}】仍未完成", nodeID, fe.LAST_TASK_ID));
            }
            if (referenceDate.Year != endDate.Year || referenceDate.Month != endDate.Month)
            {
                throw new Exception("基准日期和截止日期必须同月");
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("FLOW_ID", 0);
            dic.Add("NODE_ID", nodeID);
            dic.Add("IS_MANUAL", isManual ? 1 : 0);
            dic.Add("REFERENCE_DATE", referenceDate.Date);
            dic.Add("BEGIN_DATE", beginDate.Date);
            dic.Add("END_DATE", endDate.Date);
            dic.Add("PARAMETER", parameters);
            dic.Add("RETRY_TIMES", 0);
            dic.Add("REMARK", remark);
            dic.Add("CREATE_UID", isManual ? SystemSession.UserID : 0);
            dic.Add("UPDATE_UID", isManual ? SystemSession.UserID : 0);
            dic.Add("CREATE_TIME", DateTime.Now);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("RUN_STATUS", Enums.RunStatus.等待.GetHashCode());

            return Add(dic);
        }

        /// <summary>
        /// 获取等待运行的任务列表
        /// </summary>
        /// <returns></returns>
        public IList<Entity> GetWaitingList()
        {
            return GetList<Entity>("RUN_STATUS=?", Enums.RunStatus.等待.GetHashCode());
        }

        /// <summary>
        /// 获取运行中的任务列表
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetRunningList()
        {
            return GetList<Entity>("RUN_STATUS=?", Enums.RunStatus.运行.GetHashCode()).ToList();
        }

        /// <summary>
        /// 强制启动任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool Start(int taskID, ref string message)
        {
            try
            {
                Entity te = GetEntityByKey<Entity>(taskID);
                if (te == null)
                {
                    message = "没有找到任务【" + taskID + "】";
                    return false;
                }
                if (te.RUN_STATUS == (short)Enums.RunStatus.运行)
                {
                    message = "任务正处于运行中";
                    return false;
                }
                if (te.RUN_STATUS == (short)Enums.RunStatus.等待)
                {
                    message = "任务正处于等待运行中，请稍后再试";
                    return false;
                }
                //单节点任务
                if (te.NODE_ID > 0)
                {
                    BF_ST_NODE.Entity ne = BF_ST_NODE.Instance.GetEntityByKey<BF_ST_NODE.Entity>(te.NODE_ID);
                    if (ne == null)
                    {
                        message = "节点任务，但没有找到节点【" + te.NODE_ID + "】";
                        return false;
                    }
                    if (ne.RUN_STATUS == (short)Enums.RunStatus.运行)
                    {
                        message = "节点任务，但节点【" + ne.NAME + "】正在执行任务【" + ne.LAST_TASK_ID + "】，当前任务无法启动";
                        return false;
                    }
                    BF_ST_NODE.Instance.SetLastTaskID(te.NODE_ID, taskID);
                }
                else
                {
                    BF_ST_FLOW.Entity fe = BF_ST_FLOW.Instance.GetEntityByKey<BF_ST_FLOW.Entity>(te.FLOW_ID);
                    if (fe == null)
                    {
                        message = "无效任务，但没有找到脚本【" + te.FLOW_ID + "】";
                        return false;
                    }
                    if (fe.RUN_STATUS == (short)Enums.RunStatus.运行)
                    {
                        message = "脚本任务，但脚本【" + fe.NAME + "】正在执行任务【" + fe.LAST_TASK_ID + "】，当前任务无法启动";
                        return false;
                    }
                    BF_ST_FLOW.Instance.SetLastTaskID(te.NODE_ID, taskID);
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("RUN_STATUS", Enums.RunStatus.等待.GetHashCode());
                dic.Add("UPDATE_TIME", DateTime.Now);
                dic.Add("START_TIME", null);
                dic.Add("FINISH_TIME", null);
                dic.Add("REMARK", "手动重启任务");
                int i = Update(dic, "ID=?", taskID);
                if (i == 1)
                {
                    message = "已重启任务，请等待执行";
                    return true;
                }
                message = "启动任务失败";
            }
            catch (Exception ex)
            {
                message = "启动任务出错，详见日志";
                BLog.Write(BLog.LogLevel.WARN, "启动任务【" + taskID + "】出错：" + ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 强制停止任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool Stop(int taskID, ref string message)
        {
            try
            {
                Entity te = GetEntityByKey<Entity>(taskID);
                if (te == null)
                {
                    message = "没有找到任务【" + taskID + "】";
                    return false;
                }
                if (te.RUN_STATUS == (short)Enums.RunStatus.运行)
                {
                    message = "任务正处于运行中，无法停止";
                    return false;
                }
                if (te.RUN_STATUS == (short)Enums.RunStatus.结束)
                {
                    message = "任务已经停止了";
                    return false;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("RUN_STATUS", Enums.RunStatus.结束.GetHashCode());
                dic.Add("UPDATE_TIME", DateTime.Now);
                dic.Add("FINISH_TIME", DateTime.Now);
                dic.Add("IS_SUCCESS", 0);
                dic.Add("REMARK", "手动停止任务");
                int i = Update(dic, "ID=?", taskID);
                if (i == 1)
                {
                    message = "已停止任务";
                    return true;
                }
                message = "停止任务失败";
            }
            catch (Exception ex)
            {
                message = "停止任务出错，详见日志";
                BLog.Write(BLog.LogLevel.WARN, "停止任务【" + taskID + "】出错：" + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 设置任务状态为“开始”
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="flowID">脚本ID</param>
        /// <returns></returns>
        public int SetStart(int taskID, int flowID)
        {
            if (flowID > 0)
            {
                BF_ST_FLOW.Instance.SetLastTaskID(flowID, taskID);
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.运行.GetHashCode());
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("START_TIME", DateTime.Now);
            dic.Add("FINISH_TIME", null);
            return Update(dic, "ID=?", taskID);
        }

        /// <summary>
        /// 设置任务状态为“结束”
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="flowID">脚本ID</param>
        /// <param name="isSuccess">是否成功</param>
        /// <returns></returns>
        public int SetFinish(int taskID, int flowID, bool isSuccess)
        {
            if (flowID > 0)
            {
                BF_ST_FLOW.Instance.SetLastTaskResult(flowID, isSuccess);
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.结束.GetHashCode());
            dic.Add("IS_SUCCESS", isSuccess ? 1 : 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("FINISH_TIME", DateTime.Now);
            return Update(dic, "ID=?", taskID);
        }

        #region 查询列表
        public DataTable GetDataTable(int limit, int page, ref int count, string daterange = "", string flowName = "", string nodeName = "", string param = "", string statusId = "", string successId = "", string orderByField = "st.ID", string orderByType = "ASC")
        {
            string strWhere = "1=1";
            List<object> paramList = new List<object>();

            #region 添加参数
            DateTime endDate = DateTime.Today;
            DateTime beginDate = endDate.AddDays(-endDate.Day);
            beginDate = beginDate.AddDays(1 - beginDate.Day);

            if (string.IsNullOrWhiteSpace(daterange) == false)
            {
                Functions.GetIntervalDate(daterange, ref beginDate, ref endDate);
            }
            strWhere += " AND ST.REFERENCE_DATE>=? AND ST.REFERENCE_DATE<?";
            paramList.Add(beginDate.Date);
            paramList.Add(endDate.AddDays(1).Date);
            if (string.IsNullOrWhiteSpace(flowName) == false)
            {
                strWhere += " and SF.NAME like '%" + flowName.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(nodeName) == false)
            {
                strWhere += " AND SN.NAME like '%" + nodeName.Replace('\'', ' ') + "%'";
            }

            if (string.IsNullOrWhiteSpace(param) == false)
            {
                strWhere += " AND ST.PARAMETER like '%" + param.Replace('\'', ' ') + "%'";
            }

            if (string.IsNullOrWhiteSpace(statusId) == false)
            {
                strWhere += " AND st.RUN_STATUS =?";
                paramList.Add(statusId);
            }
            if (string.IsNullOrWhiteSpace(successId) == false)
            {
                strWhere += " AND st.IS_SUCCESS =?";
                paramList.Add(successId);
            }
            #endregion

            string strSql = "select st.id,sf.name FLOWNAME,SN.NAME NODENAME,st.IS_MANUAL,st.REFERENCE_DATE,st.RETRY_TIMES,st.RUN_STATUS,st.IS_SUCCESS ,st.START_TIME,st.FINISH_TIME,st.CREATE_TIME,st.REMARK, st.BEGIN_DATE, st.END_DATE, st.PARAMETER ";
            strSql += "from BF_ST_TASK st left join BF_ST_FLOW sf on ST.FLOW_ID = SF.ID left join BF_ST_NODE sn on ST.NODE_ID = SN.ID where " + strWhere;
            //排序
            if (string.IsNullOrWhiteSpace(orderByField) == false)
                strSql += " ORDER BY " + orderByField + " " + (string.IsNullOrWhiteSpace(orderByType) == false ? orderByType : "");
            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0 && page == 0)
                {
                    return dbHelper.ExecuteDataTableParams(strSql, paramList);//不分页查询所有
                }
                //算总记录
                if (count == 0)
                {
                    string sqlCount = string.Format("SELECT COUNT(*) FROM ({0})", strSql);
                    count = dbHelper.ExecuteScalarIntParams(sqlCount, paramList);
                }
                return dbHelper.ExecuteDataTablePageParams(strSql, limit, page, paramList);
            }
        }
        #endregion

    }
}
