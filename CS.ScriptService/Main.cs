using CS.Base.Log;
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
    /// 主程序
    /// </summary>
    public static class Main
    {
        /// <summary>
        /// 运行标志
        /// </summary>
        public static bool IsRun = false;
        /// <summary>
        /// 重新加载策略的时间间隔（秒钟）
        /// </summary>
        public const int RELOAD_RULE_SECONDS = 10;
        /// <summary>
        /// 允许最大同时执行的节点数量（默认为10）
        /// </summary>
        public static int MaxExecuteNodeCount = 10;
        /// <summary>
        /// 当前运行中的节点数量
        /// </summary>
        public static int RunningNodeCount = 0;
        /// <summary>
        /// 启用文件导入模式的最低记录量（达到这个数量则自动使用文件导入方式，默认10万）
        /// </summary>
        public static int UseFileModeRows = 100000;
        /// <summary>
        /// 是否验证导入成功数量，默认为true
        /// </summary>
        public static bool IsCheckLoadCount = true;
        /// <summary>
        /// 从文件导入数据到数据库后，是否删除产生的文件（调试时使用，默认为true）
        /// </summary>
        public static bool IsDeleteLoadDataFiles = true;
        /// <summary>
        /// 后台线程，不断扫描新的脚本
        /// </summary>
        private static BackgroundWorker _bw;
        /// <summary>
        /// 上次加载脚本的时间
        /// </summary>
        private static DateTime _lastLoadScriptFlowTime = DateTime.MinValue;
        /// <summary>
        /// 已经启动的脚本对象
        /// </summary>
        private static Dictionary<int, ScriptFlow> _dicTaskers = new Dictionary<int, ScriptFlow>();
        /// <summary>
        /// 错误的时间表达式
        /// </summary>
        private static Dictionary<long, bool> _dicErrorTimeExpression = new Dictionary<long, bool>();

        /// <summary>
        /// 启动
        /// </summary>
        public static void Start()
        {
            IsRun = true;

            try
            {
                BLog.Write(BLog.LogLevel.INFO, "程序即将启动。");
                _bw = new BackgroundWorker();
                _bw.WorkerSupportsCancellation = true;
                _bw.DoWork += bw_DoWork;
                _bw.RunWorkerAsync();

                TaskScanner.Start();

                BLog.Write(BLog.LogLevel.INFO, "程序已经启动。");
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.FATAL, "程序启动失败。" + ex.ToString());
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            IsRun = false;

            try
            {
                BLog.Write(BLog.LogLevel.INFO, "程序即将停止。");
                TaskScanner.Stop();

                _bw.CancelAsync();
                _bw.Dispose();

                foreach (var kvp in _dicTaskers)
                {
                    kvp.Value.Stop();
                }
                _dicTaskers.Clear();
                BLog.Write(BLog.LogLevel.INFO, "程序已经停止。");
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "程序停止失败。" + ex.ToString());
            }
        }

        /// <summary>
        /// 后台线程，定时加载新的任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Main.IsRun)
            {
                DateTime now = DateTime.Now;
                if ((now - _lastLoadScriptFlowTime).TotalSeconds >= RELOAD_RULE_SECONDS)
                {
                    ReLoadScriptFlow();
                    _lastLoadScriptFlowTime = now;
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 重新加载脚本配置
        /// </summary>
        private static void ReLoadScriptFlow()
        {
            //BLog.Write(BLog.LogLevel.DEBUG, "即将重新加载脚本。");
            Dictionary<int, string> dicScripts = BLL.FW.BF_ST_FLOW.Instance.GetAllEnables();
            //BLog.Write(BLog.LogLevel.DEBUG, "当前共有【" + dicScripts.Count + "】个脚本。");
            int addCount = 0;
            int delCount = 0;
            int errCount = 0;
            int updCount = 0;
            //停止已经删除及停用的脚本
            List<int> delTemp = new List<int>();
            foreach (var kvp in _dicTaskers)
            {
                if (dicScripts.ContainsKey(kvp.Key) == false)
                {
                    kvp.Value.Stop();
                    delTemp.Add(kvp.Key);
                    delCount++;
                    BLog.Write(BLog.LogLevel.DEBUG, "脚本ID【" + kvp.Key + "】已经停止。");
                }
            }
            //添加新的脚本，并启动它
            foreach (var kvp in dicScripts)
            {
                if (_dicTaskers.ContainsKey(kvp.Key) == true)
                {
                    //更新时间表达式
                    if (_dicTaskers[kvp.Key].TimeExpression != kvp.Value)
                    {
                        _dicTaskers[kvp.Key].SetTimeExpression(kvp.Value);
                        BLog.Write(BLog.LogLevel.INFO, string.Format("脚本【{0}】的时间表达式已经更新为【{1}】，1分钟后生效。", kvp.Key, kvp.Value));
                        updCount++;
                    }
                }
                else
                {
                    try
                    {
                        ScriptFlow sf = new ScriptFlow(kvp.Key, kvp.Value);
                        _dicTaskers.Add(kvp.Key, sf);
                        sf.Start();
                        addCount++;
                    }
                    catch (Exception ex)
                    {
                        //只输出一次错误日志，避免日志太多
                        if (_dicErrorTimeExpression.ContainsKey(kvp.Key) == false)
                        {
                            BLog.Write(BLog.LogLevel.WARN, "脚本【" + kvp.Key + "】初始化失败：" + ex.Message);
                            _dicErrorTimeExpression.Add(kvp.Key, false);
                        }
                        errCount++;
                    }
                }
            }
            //移除已经删除及停用的脚本
            if (delTemp.Count > 0)
            {
                foreach (int id in delTemp)
                {
                    _dicTaskers.Remove(id);
                }
            }

            if (delCount > 0 || addCount > 0 || updCount > 0)
            {
                BLog.Write(BLog.LogLevel.INFO, string.Format("已经成功重新加载脚本，删除了{0}个，成功添加了{1}个，有{2}个添加失败，更新了{3}个任务的时间表达式，当前共有{4}个计划任务，将定时执行。", delCount, addCount, errCount, updCount, _dicTaskers.Count));
            }
        }

    }
}
