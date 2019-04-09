using CS.Base.Cron;
using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.ScriptService
{
    /// <summary>
    /// 脚本(按照时间表达式，定时执行，生成任务并写入表：BF_ST_TASK）
    /// </summary>
    public class ScriptFlow : BCron
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">任务流ID</param>
        /// <param name="expression">时间表达式</param>
        public ScriptFlow(int id, string expression) : base(id, expression)
        {
            WriteLog(0, BLog.LogLevel.INFO, string.Format("脚本【{0}】已经初始化，将按照时间表达式【{1}】定时执行。如在运行中修改时间表达式，需要至少提前1分钟。", id, expression));
        }

        /// <summary>
        /// 到达时间点之后，执行具体任务（本方法由基类自动调用，不需要在外部来调用）
        /// </summary>
        /// <param name="runTime">当前时间点</param>
        /// <returns></returns>
        protected override bool Execute(DateTime runTime)
        {
            if (Main.IsRun == false)
            {
                WriteLog(0, BLog.LogLevel.INFO, string.Format("服务已经停止，脚本【{0}】将不会执行任务。", ID));
                return false;
            }

            WriteLog(0, BLog.LogLevel.DEBUG, string.Format("脚本【{0}】即将获取执行中的实例，如果有实例处于运行中，将不会创建新实例。", ID));

            try
            {
                //获取当前脚本
                BF_ST_FLOW.Entity entity = BF_ST_FLOW.Instance.GetEntityByKey<BF_ST_FLOW.Entity>(ID);
                if (entity == null)
                {
                    WriteLog(0, BLog.LogLevel.WARN, "脚本【" + ID + "】对象为空");
                    return false;
                }

                if (entity.RUN_STATUS == (short)Enums.RunStatus.运行)
                {
                    WriteLog(0, BLog.LogLevel.WARN, "脚本【" + ID + "】正在运行任务【" + entity.LAST_TASK_ID + "】，将暂不创新新任务");
                    return false;
                }

                //分月以及拆分参数
                DateTime referenceDate = DateTime.Today;
                DateTime beginDate = DateTime.Today;
                DateTime endDate = DateTime.Today;
                string parameters = entity.PARAMETERS;

                //下一次运行的时间
                DateTime nextRunTime = GetNextRunTime();

                //按月的口径
                if (nextRunTime.Month != runTime.Month && nextRunTime.Day == runTime.Day)
                {
                    referenceDate = DateTime.Today;
                    endDate = referenceDate.AddMonths(1 + entity.OFFSET);   //偏移月数
                    endDate = endDate.AddDays(-endDate.Day);                //月末最后一天
                    beginDate = endDate.AddDays(1 - endDate.Day);           //月初一号
                    if (string.IsNullOrWhiteSpace(parameters) == true)
                    {
                        BF_ST_TASK.Instance.AddFlowTask(ID, false, referenceDate, beginDate, endDate, "", "程序自动创建");
                    }
                    else
                    {
                        foreach (string para in parameters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            BF_ST_TASK.Instance.AddFlowTask(ID, false, referenceDate, beginDate, endDate, para, "程序自动拆分参数创建");
                        }
                    }
                }
                else
                {
                    //间隔天数
                    int tsDays = (int)(nextRunTime - runTime).TotalDays;
                    referenceDate = DateTime.Today;
                    endDate = referenceDate.AddDays(entity.OFFSET);     //根据偏移天数得到结束日期
                    beginDate = endDate.AddDays(1 - tsDays);            //起始日期
                    //可能拆分成两个月的（不考虑三个月及以上的情况）
                    DateTime end1 = endDate;
                    DateTime beg2 = beginDate;
                    DateTime ref1 = referenceDate;
                    //跨月
                    if (beginDate.Month != endDate.Month)
                    {
                        end1 = endDate.AddDays(-endDate.Day);   //上一月最后一天
                        beg2 = end1.AddDays(1);                 //当月第一天
                        ref1 = referenceDate.AddDays(-endDate.Day);
                    }

                    if (string.IsNullOrWhiteSpace(parameters) == true)
                    {
                        if (beginDate.Month == endDate.Month)
                        {
                            BF_ST_TASK.Instance.AddFlowTask(ID, false, referenceDate, beginDate, endDate, "", "程序自动创建");
                        }
                        else
                        {
                            //上一月
                            BF_ST_TASK.Instance.AddFlowTask(ID, false, ref1, beginDate, end1, "", "程序自动分月创建");
                            //本月
                            BF_ST_TASK.Instance.AddFlowTask(ID, false, referenceDate, beg2, endDate, "", "程序自动分月创建");
                        }
                    }
                    else
                    {
                        if (beginDate.Month == endDate.Month)
                        {
                            foreach (string para in parameters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                BF_ST_TASK.Instance.AddFlowTask(ID, false, referenceDate, beginDate, endDate, para, "程序自动拆分参数创建");
                            }
                        }
                        else
                        {
                            //上一月
                            foreach (string para in parameters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                BF_ST_TASK.Instance.AddFlowTask(ID, false, ref1, beginDate, end1, para, "程序自动分月并拆分参数创建");
                            }
                            //本月
                            foreach (string para in parameters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                BF_ST_TASK.Instance.AddFlowTask(ID, false, referenceDate, beg2, endDate, para, "程序自动分月并拆分参数创建");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(0, BLog.LogLevel.WARN, "创建脚本【" + ID + "】任务失败" + ex.ToString());
                return false;
            }

            return true;
        }


        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        protected static void WriteLog(int taskID, BLog.LogLevel level, string message)
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
                BLog.Write(BLog.LogLevel.ERROR, "写日志到任务日志表出错：" + ex.ToString());
            }
        }
    }
}
