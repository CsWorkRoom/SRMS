using CS.Base.Log;
using CS.Common.FW;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 脚本流
    /// </summary>
    public class BF_ST_FLOW : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_FLOW Instance = new BF_ST_FLOW();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_FLOW()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_FLOW";
            this.ItemName = "脚本流";
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
            /// 脚本流名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, IsIndex = true, IsIndexUnique = true, Comment = "脚本流名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 脚本流类型
            /// </summary>
            [Field(IsNotNull = true, Comment = "脚本流类型")]
            public int TYPE_ID { get; set; }

            /// <summary>
            /// 时间表达式
            /// </summary>
            [Field(IsNotNull = true, Comment = "时间表达式")]
            public string CRON { get; set; }

            /// <summary>
            /// 时间偏移量
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "时间偏移量")]
            public int OFFSET { get; set; }

            /// <summary>
            /// 参数值（参数值[,参数值][;参数值[,参数值]]  遇到分号分隔则切分再遍历执行）
            /// </summary>
            [Field(IsNotNull = false, Length = 2048, Comment = "参数值（参数值[,参数值][;参数值[,参数值]]  遇到分号分隔则切分再遍历执行）")]
            public string PARAMETERS { get; set; }

            /// <summary>
            /// 失败重试次数
            /// </summary>

            [Field(IsNotNull = true, DefaultValue = "0", Comment = "失败重试次数")]
            public int RETRY_TIMES { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 最新任务运行状态（对应枚举：RunStatus)
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "最新任务运行状态（对应枚举：RunStatus)")]
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

        /// <summary>
        /// 设置最新任务执行结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public int SetLastTaskResult(int id, bool isSuccess)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.结束.GetHashCode());
            dic.Add("LAST_TASK_FT", DateTime.Now);
            dic.Add("LAST_TASK_IS", isSuccess ? 1 : 0);
            return Update(dic, "ID=?", id);
        }

        /// <summary>
        /// 启动一个脚本任务（会自动添加一个新的任务到表BF_ST_TASK）
        /// </summary>
        /// <param name="flowID">脚本ID</param>
        /// <param name="referenceDate">基准日期</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="parameter">手动参数（多个参数值以逗号分隔）</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否执行成功</returns>
        public bool Start(int flowID, DateTime referenceDate, DateTime beginDate, DateTime endDate, string parameter, ref string message)
        {
            try
            {
                Entity fe = GetEntityByKey<Entity>(flowID);
                if (fe == null)
                {
                    message = "未找到脚本【" + flowID + "】";
                    return false;
                }
                if (fe.RUN_STATUS == (short)Enums.RunStatus.运行)
                {
                    message = "在执行任务【" + fe.LAST_TASK_ID + "】，无法启动新任务";
                    return false;
                }
                int i = BF_ST_TASK.Instance.AddFlowTask(flowID, true, referenceDate, beginDate, endDate, parameter, "手动添加脚本任务");
                if (i == 1)
                {
                    message = "已经创建新的脚本任务，请稍后刷新查看运行情况";
                    return true;
                }
                message = "创建脚本任务失败";
            }
            catch (Exception ex)
            {
                message = "创建脚本任务出错，详见日志";
                BLog.Write(BLog.LogLevel.WARN, "创建脚本【" + flowID + "】任务出错：" + ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 停止脚本任务（停止LAST_TASK_ID字段中值对应的任务）
        /// </summary>
        /// <param name="flowID">脚本ID</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否执行成功</returns>
        public bool Stop(int flowID, ref string message)
        {
            try
            {
                Entity fe = GetEntityByKey<Entity>(flowID);
                if (fe == null)
                {
                    message = "未找到脚本【" + flowID + "】";
                    return false;
                }
                if (fe.RUN_STATUS == (short)Enums.RunStatus.结束 || fe.LAST_TASK_ID < 1)
                {
                    message = "脚本未执行任务或当前任务已经停止";
                    return false;
                }
                //设置脚本最新任务的状态
                SetLastTaskResult(flowID, false);

                //对应的任务
                BF_ST_TASK.Entity te = BF_ST_TASK.Instance.GetEntityByKey<BF_ST_TASK.Entity>(fe.LAST_TASK_ID);
                if (te == null)
                {
                    message = "未找到脚本的最新任务【" + fe.LAST_TASK_ID + "】";
                    return false;
                }
                if (te.RUN_STATUS == (short)Enums.RunStatus.结束)
                {
                    message = "脚本的最新任务【" + fe.LAST_TASK_ID + "】已经停止";
                    return false;
                }

                //停止任务
                return BF_ST_TASK.Instance.Stop(fe.LAST_TASK_ID, ref message);
            }
            catch (Exception ex)
            {
                message = "停止脚本任务出错，详见日志";
                BLog.Write(BLog.LogLevel.WARN, "停止脚本【" + flowID + "】任务出错：" + ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 获取所有启用的脚本
        /// </summary>
        /// <returns>键：脚本ID，值：时间表达式</returns>
        public Dictionary<int, string> GetAllEnables()
        {
            return GetDictionary("ID", "CRON", "IS_ENABLE=?", 1);
        }

        #region 启用
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetEnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 1);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);
            return UpdateByKey(dic, id);
        }
        #endregion

        #region 禁用
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
        #endregion
    }
}
