using Easyman.Base.Log;
using Easyman.BLL.FW;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Easyman.ScriptService
{
    /// <summary>
    /// 任务节点扫描（每隔n秒扫描一次BF_SCRIPT_TASK_FN，并执行节点）
    /// </summary>
    public static class TaskNodeScanner
    {
        /// <summary>
        /// 后台线程，不断扫描需要执行的节点实例
        /// </summary>
        private static BackgroundWorker _bw;

        /// <summary>
        /// 开始启动
        /// </summary>
        /// <returns></returns>
        public static void Start()
        {
            try
            {
                BLog.Write(BLog.LogLevel.INFO, "节点扫描线程即将启动。");
                _bw = new BackgroundWorker();
                _bw.WorkerSupportsCancellation = true;
                _bw.DoWork += DoWork;
                _bw.RunWorkerAsync();
                BLog.Write(BLog.LogLevel.INFO, "节点扫描线程已经启动。");
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "节点扫描线程启动失败。" + ex.ToString());
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            try
            {
                BLog.Write(BLog.LogLevel.INFO, "节点扫描线程即将停止。");
                _bw.CancelAsync();
                _bw.Dispose();
                BLog.Write(BLog.LogLevel.INFO, "节点扫描线程已经停止。");
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "节点扫描线程停止失败。" + ex.ToString());
            }
        }

        /// <summary>
        /// 不断扫描脚本实例表，对于需要运行的实例，为其添加节点实例并运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            while (Main.IsRun)
            {
                try
                {
                    IList<BF_ST_TASK_FLOW_NODE.Entity> waitList = BF_ST_TASK_FLOW_NODE.Instance.GetWaitingList();
                    if (waitList != null && waitList.Count > 0)
                    {
                        foreach (BF_ST_TASK_FLOW_NODE.Entity fn in waitList)
                        {
                            bool isStart = false;
                            if (string.IsNullOrWhiteSpace(fn.PRE_NODE_IDS))
                            {
                                isStart = true;
                            }

                            if (isStart == true)
                            {
                                BF_ST_TASK_FLOW_NODE.Instance.SetStart(fn.ID);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// 添加一条日志
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        /// <returns></returns>
        public static void WriteLog(int taskID, int nodeID, BLog.LogLevel level, string message)
        {
            //写日志文件
            BLog.Write(level, message);

            try
            {
                //写数据库表
                if (taskID > 0)
                {
                    //BF_ST_TASK_FLOW_NODE_LOG.Instance.Add(taskID, nodeID, level.GetHashCode(), message);
                }
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "写日志到脚本日志表出错：" + ex.ToString());
            }
        }
    }
}
