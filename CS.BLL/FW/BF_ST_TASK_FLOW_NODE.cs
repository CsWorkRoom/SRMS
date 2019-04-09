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
    /// 任务详情（脚本流与节点的绑定的实例）
    /// </summary>
    public class BF_ST_TASK_FLOW_NODE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_TASK_FLOW_NODE Instance = new BF_ST_TASK_FLOW_NODE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_TASK_FLOW_NODE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_TASK_FLOW_NODE";
            this.ItemName = "任务详情（脚本流与节点的绑定的实例）";
            this.KeyField = "ID";
            this.OrderbyFields = "TASK_ID";
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
            /// 前前节点（集合，没有则为空，有则逗号分隔）
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "前前节点（集合，没有则为空，有则逗号分隔）")]
            public string PRE_NODE_IDS { get; set; }

            /// <summary>
            /// 页面展示时DIV位置X坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "页面展示时DIV位置X坐标")]
            public int DIV_X { get; set; }

            /// <summary>
            /// 页面展示时DIV位置Y坐标
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "页面展示时DIV位置Y坐标")]
            public int DIV_Y { get; set; }

            /// <summary>
            /// 脚本内容
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "脚本内容")]
            public string CONTENT { get; set; }

            /// <summary>
            /// 编译前的代码
            /// </summary>
            [Field(IsNotNull = false, Length = 4096, Comment = "编译前的代码")]
            public string CODE { get; set; }

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
            /// 失败重试次数
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "失败重试次数")]
            public int RETRY_TIMES { get; set; }

            /// <summary>
            /// 失败次数
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "失败次数")]
            public Int16 FAIL_TIMES { get; set; }

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
            /// 开始（运行）时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "开始（运行）时间")]
            public DateTime START_TIME { get; set; }

            /// <summary>
            /// 完成时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "完成时间")]
            public DateTime FINISH_TIME { get; set; }
        }

        #endregion

        /// <summary>
        /// 获取前序节点
        /// </summary>
        /// <param name="id">当前任务详情ID</param>
        /// <returns></returns>
        public IList<Entity> GetPreNodeList(int id)
        {
            return GetPreNodeList(GetEntityByKey<Entity>(id));
        }

        /// <summary>
        /// 获取前序节点
        /// </summary>
        /// <param name="entity">当前任务详情</param>
        /// <returns></returns>
        public IList<Entity> GetPreNodeList(Entity entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.PRE_NODE_IDS) == true)
            {
                return new List<Entity>();
            }

            return GetList<Entity>("TASK_ID=? AND NODE_ID IN (" + entity.PRE_NODE_IDS + ")", entity.TASK_ID);
        }

        /// <summary>
        /// 根据任务创建任务节点
        /// </summary>
        /// <param name="taskEntity">任务</param>
        /// <returns></returns>
        public int AddFromTask(BF_ST_TASK.Entity taskEntity)
        {
            if (taskEntity == null)
            {
                throw new Exception("任务不存在");
            }

            DataTable dt = GetTable("TASK_ID=?", taskEntity.ID);
            if (dt != null && dt.Rows.Count > 0)
            {
                SetWait(taskEntity.ID);
                CS.Base.Log.BLog.Write(Base.Log.BLog.LogLevel.WARN, string.Format("任务【{0}】已经有【{1}】个节点实例，将不创建新的实例", taskEntity.ID, dt.Rows.Count));
                return dt.Rows.Count;
            }

            //单节点任务
            if (taskEntity.NODE_ID > 0)
            {
                BF_ST_NODE.Entity ne = BF_ST_NODE.Instance.GetEntityByKey<BF_ST_NODE.Entity>(taskEntity.NODE_ID);
                if (ne == null)
                {
                    throw new Exception(string.Format("任务【{0}】的节点【{1}】不存在", taskEntity.ID, taskEntity.NODE_ID));
                }
                Entity entity = new Entity();
                entity.TASK_ID = taskEntity.ID;
                entity.FLOW_ID = 0;
                entity.NODE_ID = taskEntity.NODE_ID;
                entity.CONTENT = ne.CONTENT;
                entity.RUN_STATUS = (Int16)Enums.RunStatus.等待.GetHashCode();
                entity.RETRY_TIMES = 0;
                entity.REFERENCE_DATE = taskEntity.REFERENCE_DATE;
                entity.BEGIN_DATE = taskEntity.BEGIN_DATE;
                entity.END_DATE = taskEntity.END_DATE;
                entity.PARAMETER = taskEntity.PARAMETER;

                return Add(entity);
            }

            //脚本流任务
            int i = 0;
            BF_ST_FLOW.Entity fe = BF_ST_FLOW.Instance.GetEntityByKey<BF_ST_FLOW.Entity>(taskEntity.FLOW_ID);
            if (fe == null)
            {
                throw new Exception(string.Format("任务【{0}】没有配置有效脚本", taskEntity.ID));
            }

            IList<BF_ST_FLOW_NODE.Entity> fnList = BF_ST_FLOW_NODE.Instance.GetListByFlowID(taskEntity.FLOW_ID);
            if (fnList == null || fnList.Count < 1)
            {
                throw new Exception(string.Format("任务【{0}】没有配置有效节点", taskEntity.ID));
            }

            //添加相关节点
            foreach (BF_ST_FLOW_NODE.Entity fn in fnList)
            {
                BF_ST_NODE.Entity ne = BF_ST_NODE.Instance.GetEntityByKey<BF_ST_NODE.Entity>(fn.NODE_ID);
                if (ne == null)
                {
                    throw new Exception(string.Format("任务【{0}】的相关节点【{1}】不存在", taskEntity.ID, fn.NODE_ID));
                }

                Entity entity = new Entity();
                entity.TASK_ID = taskEntity.ID;
                entity.FLOW_ID = taskEntity.FLOW_ID;
                entity.NODE_ID = fn.NODE_ID;
                if (string.IsNullOrWhiteSpace(fn.PRE_NODE_IDS) == false)
                {
                    entity.PRE_NODE_IDS = fn.PRE_NODE_IDS.Trim(new char[] { ',', ' ' });
                }
                entity.DIV_X = fn.DIV_X;
                entity.DIV_Y = fn.DIV_Y;
                entity.CONTENT = ne.CONTENT;
                entity.RUN_STATUS = (Int16)Enums.RunStatus.等待.GetHashCode();
                entity.RETRY_TIMES = fe.RETRY_TIMES;
                entity.REFERENCE_DATE = taskEntity.REFERENCE_DATE;
                entity.BEGIN_DATE = taskEntity.BEGIN_DATE;
                entity.END_DATE = taskEntity.END_DATE;
                entity.PARAMETER = taskEntity.PARAMETER;

                i += Add(entity);
            }
            return i;
        }

        /// <summary>
        /// 获取任务节点列表
        /// </summary>
        /// <returns></returns>
        public IList<Entity> GetNodeList(int taskID)
        {
            return GetList<Entity>("TASK_ID=?", taskID);
        }

        /// <summary>
        /// 获取等待运行的任务列表
        /// </summary>
        /// <returns></returns>
        public IList<Entity> GetWaitingList(int taskID)
        {
            return GetList<Entity>("TASK_ID=? AND RUN_STATUS=?", taskID, Enums.RunStatus.等待.GetHashCode());
        }

        ///// <summary>
        ///// 获取运行中的任务列表
        ///// </summary>
        ///// <returns></returns>
        //public List<Entity> GetRunningList(int taskID)
        //{
        //    return GetList<Entity>("TASK_ID=? AND RUN_STATUS=?", taskID, Enums.RunStatus.运行.GetHashCode()).ToList();
        //}

        /// <summary>
        /// 获取重启的节点列表
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetReStartList()
        {
            return GetList<Entity>("RUN_STATUS=?", Enums.RunStatus.重启.GetHashCode()).ToList();
        }

        /// <summary>
        /// 将任务的所有节点实例设置为“等待”
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns></returns>
        public int SetWait(int taskID)
        {
            //更新节点实例状态
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.等待.GetHashCode());
            dic.Add("START_TIME", DateTime.Now);
            return Update(dic, "TASK_ID=?", taskID);
        }

        /// <summary>
        /// 重启节点实例
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="content">更新后的脚本（如果不更新，则传入空白字符串）</param>
        /// <returns></returns>
        public int ReStart(int taskID, int nodeID, string content = "")
        {
            //更新节点实例状态
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.重启.GetHashCode());
            dic.Add("START_TIME", DateTime.Now);
            if (string.IsNullOrWhiteSpace(content) == false)
            {
                dic.Add("CONTENT", content);
            }
            return Update(dic, "TASK_ID=? AND NODE_ID=?", taskID, nodeID);
        }

        /// <summary>
        /// 设置状态为“开始”
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="nodeID">任务ID</param>
        /// <param name="taskID">节点ID</param>
        /// <returns></returns>
        public int SetStart(int id, int taskID, int nodeID)
        {
            //设置节点的最新运行节点
            BF_ST_NODE.Instance.SetLastTaskID(nodeID, taskID);
            //更新节点实例状态
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.运行.GetHashCode());
            dic.Add("START_TIME", DateTime.Now);
            return Update(dic, "ID=?", id);
        }

        /// <summary>
        /// 更新错误次数
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="failTimes">错误次数</param>
        /// <returns></returns>
        public int RecordTryTimes(int id, int failTimes)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("FAIL_TIMES", failTimes);
            return Update(dic, "ID=?", id);
        }

        /// <summary>
        /// 设置任务状态为“结束”
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="taskID">任务ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="isSuccess">是否成功</param>
        /// <returns></returns>
        public int SetFinish(int id, int taskID, int nodeID, bool isSuccess)
        {
            //更新节点最新任务情况
            BF_ST_NODE.Instance.SetLastTaskResult(nodeID, taskID, isSuccess);
            //更新节点实例状态
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RUN_STATUS", Enums.RunStatus.结束.GetHashCode());
            dic.Add("IS_SUCCESS", isSuccess ? 1 : 0);
            dic.Add("FINISH_TIME", DateTime.Now);
            return Update(dic, "ID=?", id);
        }

        /// <summary>
        /// 更新编译前的源码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int SetCode(int id, string code)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("CODE", code);
            return UpdateByKey(dic, id);
        }
    }
}