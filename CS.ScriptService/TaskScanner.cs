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
    /// 任务扫描（每隔n秒扫描一次表：BF_ST_TASK,并将节点添加到表：BF_ST_TASK_FLOW_NODE，然后再实例化TaskNodeRunner，启动各个节点)
    /// </summary>
    public static class TaskScanner
    {
        /// <summary>
        /// 后台线程，不断扫描需要执行的节点实例
        /// </summary>
        private static BackgroundWorker _bwRunNode;

        /// <summary>
        /// 后台线程，不断扫描任务是否已经执行完成
        /// </summary>
        private static BackgroundWorker _bwScanTask;

        /// <summary>
        /// 开始启动
        /// </summary>
        /// <returns></returns>
        public static void Start()
        {
            try
            {
                BLog.Write(BLog.LogLevel.INFO, "任务扫描线程即将启动。");
                _bwRunNode = new BackgroundWorker();
                _bwRunNode.WorkerSupportsCancellation = true;
                _bwRunNode.DoWork += DoRunWork;
                _bwRunNode.RunWorkerAsync();

                _bwScanTask = new BackgroundWorker();
                _bwScanTask.WorkerSupportsCancellation = true;
                _bwScanTask.DoWork += DoScanWork;
                _bwScanTask.RunWorkerAsync();
                BLog.Write(BLog.LogLevel.INFO, "任务扫描线程已经启动。");
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "任务扫描线程启动失败。" + ex.ToString());
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            try
            {
                BLog.Write(BLog.LogLevel.INFO, "任务扫描线程即将停止。");
                _bwRunNode.CancelAsync();
                _bwRunNode.Dispose();

                _bwScanTask.CancelAsync();
                _bwScanTask.Dispose();
                BLog.Write(BLog.LogLevel.INFO, "任务扫描线程已经停止。");
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "任务扫描线程停止失败。" + ex.ToString());
            }
        }

        /// <summary>
        /// 不断扫描脚本实例表，对于需要运行的实例，为其添加节点实例并运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DoRunWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //上次未完成的任务
                IList<BF_ST_TASK.Entity> runList = BF_ST_TASK.Instance.GetRunningList();
                if (runList != null && runList.Count > 0)
                {
                    foreach (BF_ST_TASK.Entity te in runList)
                    {
                        try
                        {
                            WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("即将执行上次未完成的任务【{0}】", te.ID));
                            IList<BF_ST_TASK_FLOW_NODE.Entity> tfnList = BF_ST_TASK_FLOW_NODE.Instance.GetNodeList(te.ID);
                            if (tfnList != null && tfnList.Count > 0)
                            {
                                foreach (BF_ST_TASK_FLOW_NODE.Entity tfnEntity in tfnList)
                                {
                                    string stopmsg = string.Empty;
                                    BF_ST_NODE.Instance.Stop(tfnEntity.NODE_ID, ref stopmsg);
                                    TaskNodeRunner runner = new TaskNodeRunner(tfnEntity.ID);
                                    if (runner.Start() == true)
                                    {
                                        WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("任务【{0}】的节点将开始执行", te.ID));
                                    }
                                    else
                                    {
                                        WriteLog(te.ID, BLog.LogLevel.WARN, string.Format("任务【{0}】的节点无法开始执行", te.ID));
                                        //BF_ST_TASK.Instance.SetFinish(te.ID, te.FLOW_ID, false);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //修改状态
                            BF_ST_TASK.Instance.SetFinish(te.ID, te.FLOW_ID, false);
                            WriteLog(te.ID, BLog.LogLevel.ERROR, "为任务添加节点详情出错，本次任务" + te.ID + "无法执行，错误信息为：" + ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(0, BLog.LogLevel.ERROR, "扫描上次未完成的脚本任务出错，错误信息为：" + ex.ToString());
            }

            while (Main.IsRun)
            {
                int i = 0;
                //扫描重启的节点实例
                try
                {
                    List<BF_ST_TASK_FLOW_NODE.Entity> restartList = BF_ST_TASK_FLOW_NODE.Instance.GetReStartList();
                    if (restartList != null && restartList.Count > 0)
                    {
                        foreach (BF_ST_TASK_FLOW_NODE.Entity ne in restartList)
                        {
                            TaskNodeRunner runner = new TaskNodeRunner(ne.ID, true);
                            if (runner.Start() == true)
                            {
                                WriteLog(ne.TASK_ID, BLog.LogLevel.INFO, string.Format("任务【{0}】的节点【{1}】将重新执行", ne.TASK_ID, ne.NODE_ID));
                            }
                            else
                            {
                                WriteLog(ne.TASK_ID, BLog.LogLevel.WARN, string.Format("任务【{0}】的节点【{1}】无法重新执行", ne.TASK_ID, ne.NODE_ID));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(0, BLog.LogLevel.ERROR, "扫描待重新执行的脚本节点出错，错误信息为：" + ex.ToString());
                }

                //扫描等待执行的任务
                try
                {
                    IList<BF_ST_TASK.Entity> waitList = BF_ST_TASK.Instance.GetWaitingList();
                    if (waitList != null && waitList.Count > 0)
                    {
                        foreach (BF_ST_TASK.Entity te in waitList)
                        {
                            try
                            {
                                //修改状态
                                BF_ST_TASK.Instance.SetStart(te.ID, te.FLOW_ID);
                                WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("任务【{0}】已经修改任务状态为“开始”", te.ID));

                                //添加任务节点
                                int n = BF_ST_TASK_FLOW_NODE.Instance.AddFromTask(te);
                                WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("任务【{0}】已经添加了【{1}】个节点实例，等待执行", te.ID, n));

                                IList<BF_ST_TASK_FLOW_NODE.Entity> tfnList = BF_ST_TASK_FLOW_NODE.Instance.GetWaitingList(te.ID);
                                if (tfnList != null && tfnList.Count > 0)
                                {
                                    foreach (BF_ST_TASK_FLOW_NODE.Entity tfnEntity in tfnList)
                                    {
                                        TaskNodeRunner runner = new TaskNodeRunner(tfnEntity.ID);
                                        if (runner.Start() == true)
                                        {
                                            WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("任务【{0}】的节点【{1}】将开始执行", te.ID, tfnEntity.NODE_ID));
                                        }
                                        else
                                        {
                                            WriteLog(te.ID, BLog.LogLevel.WARN, string.Format("任务【{0}】的节点【{1}】无法开始执行", te.ID, tfnEntity.NODE_ID));
                                            //BF_ST_TASK.Instance.SetFinish(te.ID, te.FLOW_ID, false);
                                        }
                                    }
                                }
                                i++;
                            }
                            catch (Exception ex)
                            {
                                //修改状态
                                BF_ST_TASK.Instance.SetFinish(te.ID, te.FLOW_ID, false);
                                WriteLog(te.ID, BLog.LogLevel.ERROR, string.Format("任务【{0}】添加节点实例出错，任务无法执行，错误信息为：\r\n【{1}】", te.ID, ex.ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(0, BLog.LogLevel.ERROR, "扫描待执行的脚本任务出错，错误信息为：" + ex.ToString());
                }

                Thread.Sleep(1000 * 5);
            }
        }

        /// <summary>
        /// 扫描任务是否执行完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DoScanWork(object sender, DoWorkEventArgs e)
        {
            while (Main.IsRun)
            {
                try
                {
                    //扫描未完成的任务
                    IList<BF_ST_TASK.Entity> runList = BF_ST_TASK.Instance.GetRunningList();
                    if (runList != null && runList.Count > 0)
                    {
                        foreach (BF_ST_TASK.Entity te in runList)
                        {
                            try
                            {
                                IList<BF_ST_TASK_FLOW_NODE.Entity> tfnList = BF_ST_TASK_FLOW_NODE.Instance.GetNodeList(te.ID);
                                if (tfnList == null || tfnList.Count < 1)
                                {
                                    //还没有节点实例（有可能是还没来得及添加）
                                    continue;
                                }
                                bool isFinish = true;
                                bool isSueccess = true;
                                foreach (BF_ST_TASK_FLOW_NODE.Entity node in tfnList)
                                {
                                    if (node.RUN_STATUS == (short)Enums.RunStatus.结束)
                                    {
                                        if (node.IS_SUCCESS != 1)
                                        {
                                            isSueccess = false;
                                            //WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("任务【{0}】的节点【{1}】执行失败，将提前使任务状态为“结束”", te.ID, node.NODE_ID));
                                            //BF_ST_TASK.Instance.SetFinish(te.ID, te.FLOW_ID, false);
                                            //isFinish = false;
                                            //break;
                                        }
                                    }
                                    else
                                    {
                                        isFinish = false;
                                    }
                                }

                                if (isFinish == true)
                                {
                                    WriteLog(te.ID, BLog.LogLevel.INFO, string.Format("任务【{0}】共【{1}】个节点已经全部成功执行完成，将修改任务状态为“结束”", te.ID, tfnList.Count));
                                    BF_ST_TASK.Instance.SetFinish(te.ID, te.FLOW_ID, isSueccess);
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteLog(te.ID, BLog.LogLevel.ERROR, "扫描任务节点详情的执行情况出错，本次任务" + te.ID + "错误信息为：" + ex.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(0, BLog.LogLevel.ERROR, "扫描执行中的脚本任务出错，错误信息为：" + ex.ToString());
                }

                Thread.Sleep(1000 * 5);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        private static void WriteLog(int taskID, BLog.LogLevel level, string message)
        {
            //写日志文件
            BLog.Write(level, message);
            try
            {
                //写数据库表
                if (taskID > 0)
                {
                    BF_ST_TASK_LOG.Instance.Add(taskID, level.GetHashCode(), message);
                }
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "写日志到脚本日志表出错：" + ex.ToString());
            }
        }
    }
}