using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS.ScriptService
{
    /// <summary>
    /// 任务节点运行
    /// </summary>
    public class TaskNodeRunner
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        private int _taskID = 0;
        /// <summary>
        /// 脚本ID
        /// </summary>
        private int _flowID = 0;
        /// <summary>
        /// 节点ID
        /// </summary>
        private int _nodeID = 0;

        /// <summary>
        /// 任务节点实例ID
        /// </summary>
        private int _taskfnID = 0;
        /// <summary>
        /// 失败次数
        /// </summary>
        private int _failTimes = 0;
        /// <summary>
        /// 最大重试次数
        /// </summary>
        private int _retryTimes = 0;
        /// <summary>
        /// 是否为重启节点实例
        /// </summary>
        private bool _isRestart = false;
        /// <summary>
        /// 任务节点
        /// </summary>
        BF_ST_TASK_FLOW_NODE.Entity _taskNodeEntity;
        /// <summary>
        /// 后台线程
        /// </summary>
        private BackgroundWorker _bw;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="taskfnID">节点实例ID</param>
        /// <param name="isRestart">是否为重启节点实例</param>
        public TaskNodeRunner(int taskfnID, bool isRestart = false)
        {
            _taskfnID = taskfnID;
            _isRestart = isRestart;
        }

        /// <summary>
        /// 开始执行任务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                //读取当前节点
                _taskNodeEntity = BF_ST_TASK_FLOW_NODE.Instance.GetEntityByKey<BF_ST_TASK_FLOW_NODE.Entity>(_taskfnID);
                if (_taskNodeEntity == null)
                {
                    WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, "没有获取任务节点实例对象，无法执行。");
                    return false;
                }
                _taskID = _taskNodeEntity.TASK_ID;
                _flowID = _taskNodeEntity.FLOW_ID;
                _nodeID = _taskNodeEntity.NODE_ID;
                _retryTimes = _taskNodeEntity.RETRY_TIMES;
                _failTimes = _taskNodeEntity.FAIL_TIMES;

                //已经停止的节点，不再执行
                if (_taskNodeEntity.RUN_STATUS == (short)Enums.RunStatus.结束)
                {
                    WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.INFO, "节点实例运行状态为【停止】，本节点将不被执行。");
                    return false;
                }

                _bw = new BackgroundWorker();
                _bw.WorkerSupportsCancellation = true;
                _bw.DoWork += DoWork;
                _bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, string.Format("启动节点实例出现了未知异常，错误信息为：\r\n{0}", ex.ToString()));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 生成代码、编译及运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int waitTimes = 5;
            while (Main.IsRun)
            {
                try
                {
                    if (waitTimes > 500)
                    {
                        WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, "已经等待了太长时间仍然无法执行，本节点实例将放弃执行。");
                        return;
                    }
                    //验证运行节点实例数量
                    if (Main.RunningNodeCount >= Main.MaxExecuteNodeCount)
                    {
                        WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.INFO, string.Format("当前已经有【{0}】个节点实例正在运行，超过系统设定的最大数【{1}】，本节点实例将等待【{2}】秒再尝试执行。", Main.RunningNodeCount, Main.MaxExecuteNodeCount, waitTimes));
                        Thread.Sleep(1000 * waitTimes++);
                        continue;
                    }

                    //检查任务及其运行状态
                    if (_isRestart == false)
                    {
                        BF_ST_TASK.Entity taskEntity = BF_ST_TASK.Instance.GetEntityByKey<BF_ST_TASK.Entity>(_taskID);
                        if (taskEntity == null)
                        {
                            WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, "所属任务不存在，本节点实例将不被执行。");
                            return;
                        }
                        if (taskEntity.RUN_STATUS == Enums.RunStatus.结束.GetHashCode())
                        {
                            WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, "所属任务已经结束，本节点实例将不被执行。");
                            return;
                        }


                        //检查前序节点运行状态
                        IList<BF_ST_TASK_FLOW_NODE.Entity> preList = BF_ST_TASK_FLOW_NODE.Instance.GetPreNodeList(_taskfnID);
                        if (preList != null && preList.Count > 0)
                        {
                            bool isWait = false;
                            foreach (BF_ST_TASK_FLOW_NODE.Entity preNode in preList)
                            {
                                if (preNode.RUN_STATUS != (short)Enums.RunStatus.结束)
                                {
                                    isWait = true;
                                    break;
                                }
                                //检查前节点执行状态
                                if (preNode.IS_SUCCESS != 1)
                                {
                                    WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, string.Format("前节点【{0}】失败，本节点将不会执行", preNode.NODE_ID));
                                    BF_ST_TASK_FLOW_NODE.Instance.SetFinish(_taskfnID, _taskID, _taskNodeEntity.NODE_ID, false);
                                    return;
                                }
                            }

                            if (isWait == true)
                            {
                                Thread.Sleep(1000 * waitTimes++);
                                continue;
                            }
                        }
                    }

                    //读取节点的运行情况
                    BF_ST_NODE.Entity nodeEntity = BF_ST_NODE.Instance.GetEntityByKey<BF_ST_NODE.Entity>(_taskNodeEntity.NODE_ID);
                    if (nodeEntity != null && nodeEntity.RUN_STATUS == (short)Enums.RunStatus.运行)
                    {
                        if (nodeEntity.LAST_TASK_ID != _taskNodeEntity.TASK_ID)
                        {
                            WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.INFO, string.Format("节点实例正在运行另外的任务【{0}】，本节点实例将等待【{1}】秒后再尝试执行。", nodeEntity.LAST_TASK_ID, waitTimes));
                            Thread.Sleep(1000 * waitTimes++);
                            continue;
                        }
                    }

                    //当前运行中的节点数量
                    Main.RunningNodeCount++;

                    //更新当前节点状态
                    int i = BF_ST_TASK_FLOW_NODE.Instance.SetStart(_taskfnID, _taskNodeEntity.TASK_ID, _taskNodeEntity.NODE_ID);
                    if (i < 0)
                    {
                        WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, "更新节点实例的状态为【运行中】失败，无法执行。");
                        Main.RunningNodeCount--;
                        return;
                    }

                    WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.INFO, "更新节点实例的运行状态已经更新为【运行中】，下面将执行节点脚本内容。");
                    ErrorInfo err = new ErrorInfo();
                    Script.Transfer trans = new Script.Transfer();
                    string code = trans.Trans(_taskNodeEntity, ref err);
                    if (err.IsError == true)
                    {
                        WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, string.Format("转换节点实例生成脚本代码失败，错误信息为：\r\n{0}", err.Message));
                        Main.RunningNodeCount--;

                        BF_ST_TASK_FLOW_NODE.Instance.SetFinish(_taskfnID, _taskID, _taskNodeEntity.NODE_ID, false);
                        //BF_ST_TASK.Instance.SetFinish(_taskNodeEntity.TASK_ID, _taskNodeEntity.FLOW_ID, false);
                        return;
                    }

                    //保存源代码到数据库
                    i = BF_ST_TASK_FLOW_NODE.Instance.SetCode(_taskfnID, code);
                    if (i < 1)
                    {
                        WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, "更新节点实例生成脚本代码失败");
                        Main.RunningNodeCount--;

                        BF_ST_TASK_FLOW_NODE.Instance.SetFinish(_taskfnID, _taskID, _taskNodeEntity.NODE_ID, false);
                        //BF_ST_TASK.Instance.SetFinish(_taskNodeEntity.TASK_ID, _taskNodeEntity.FLOW_ID, false);
                        return;
                    }

                    //运行脚本
                    if (Main.IsRun == false)
                    {
                        Main.RunningNodeCount--;
                        return;
                    }

                    do
                    {
                        bool isSuccess = Script.Execute.Run(code, _taskNodeEntity, nodeEntity.DB_ID, ref err);

                        if (isSuccess)
                        {
                            //结束运行状态
                            BF_ST_TASK_FLOW_NODE.Instance.SetFinish(_taskNodeEntity.ID, _taskID, _taskNodeEntity.NODE_ID, true);
                            WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.INFO, "节点实例已经成功执行。");

                            Main.RunningNodeCount--;
                            return;
                        }

                        //记录重试次数
                        _failTimes++;
                        BF_ST_TASK_FLOW_NODE.Instance.RecordTryTimes(_taskNodeEntity.ID, _failTimes);

                        //超过最大尝试次数
                        if (_failTimes >= _retryTimes)
                        {
                            WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, string.Format("作了最后一次尝试，节点实例仍然执行失败，本实例将不再执行，错误信息为：\r\n{0}", err.Message));
                            BF_ST_TASK_FLOW_NODE.Instance.SetFinish(_taskNodeEntity.ID, _taskID, _taskNodeEntity.NODE_ID, false);
                            //BF_ST_TASK.Instance.SetFinish(_taskNodeEntity.TASK_ID, _taskNodeEntity.FLOW_ID, false);

                            //从内存记录中移除
                            Main.RunningNodeCount--;
                            return;
                        }
                        else
                        {
                            WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, string.Format("节点实例 第【{0}】次尝试执行失败，将等待【{1}】秒后再次尝试，错误信息为：\r\n{2}", _retryTimes, waitTimes, err.Message));
                            Thread.Sleep(1000 * waitTimes++);
                        }
                    }
                    while (_failTimes < _retryTimes);
                }
                catch (Exception ex)
                {
                    WriteLog(_taskfnID, _taskID, _flowID, _nodeID, BLog.LogLevel.WARN, string.Format("执行节点实例出现了未知异常，错误信息为：\r\n{0}", ex.ToString()));
                    //从内存记录中移除
                    Main.RunningNodeCount--;
                    return;
                }
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="flowID"></param>
        /// <param name="nodeID"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        protected static void WriteLog(int tfnID, int taskID, int flowID, int nodeID, BLog.LogLevel level, string message, string sql = "")
        {
            //写日志文件
            if (flowID > 0)
            {
                BLog.Write(level, string.Format("脚本任务【{0}】 脚本【{1}】 节点【{2}】 的实例【{3}】 {4}", taskID, flowID, nodeID, tfnID, message));
            }
            else
            {
                BLog.Write(level, string.Format("单节点任务【{0}】 节点【{1}】 的实例【{2}】 {3}", taskID, nodeID, tfnID, message));
            }
            try
            {
                //写数据库表
                if (taskID > 0)
                {
                    BF_ST_TASK_FLOW_NODE_LOG.Instance.Add(tfnID, taskID, flowID, nodeID, level.GetHashCode(), message, sql);
                }
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "写日志到脚本节点日志表出错：" + ex.ToString());
            }
        }
    }
}