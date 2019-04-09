using CS.BLL.FW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS.ScriptService.Script
{
    /// <summary>
    /// 脚本转换类，将脚本中的变量、函数等作替换处理
    /// </summary>
    public class Transfer
    {
        /// <summary>
        /// 基准时间，默认为当前时间
        /// </summary>
        private DateTime _referenceDateTime = DateTime.Now;
        /// <summary>
        /// 基准时间
        /// </summary>
        public DateTime ReferenceDateTime
        {
            get { return _referenceDateTime; }
        }

        /// <summary>
        /// 转换脚本
        /// </summary>
        /// <param name="taskNodeEntity">节点实例</param>
        /// <param name="err">错误信息</param>
        /// <returns>转换后的脚本</returns>
        public string Trans(BF_ST_TASK_FLOW_NODE.Entity taskNodeEntity, ref ErrorInfo err)
        {
            if (taskNodeEntity == null)
            {
                return string.Empty;
            }

            _referenceDateTime = taskNodeEntity.REFERENCE_DATE;
            string code = taskNodeEntity.CONTENT;
            string functions = string.Empty;
            try
            {
                //替换自定义函数
                code = ReplaceFunctions('@', code);
                //替换参数
                code = code.Replace("@BEGIN_DATE", taskNodeEntity.BEGIN_DATE.ToString("yyyy -MM-dd"));
                code = code.Replace("@END_DATE", taskNodeEntity.END_DATE.ToString("yyyy -MM-dd"));
                code = code.Replace("@PARAM", taskNodeEntity.PARAMETER);
                //拼凑节点执行代码块
                return GenerateCode(code, functions);
            }
            catch (Exception ex)
            {
                err.IsError = true;
                err.Message = string.Format("脚本【{0}】的实例【{1}】中的节点【{2}】转换脚本，错误信息为：\r\n{3}\r\n原始脚本代码为：\r\n{4}", taskNodeEntity.FLOW_ID, taskNodeEntity.ID, taskNodeEntity.NODE_ID, ex.ToString(), code);
                return string.Empty;
            }
        }

        /// <summary>
        /// 替换脚本中的自定义函数
        /// </summary>
        /// <param name="prechar">前缀字符（@表示可变，$表示固定）</param>
        /// <param name="code">脚本内容</param>
        /// <returns>替换之后的脚本</returns>
        public string ReplaceFunctions(char prechar, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return string.Empty;
            }

            Regex regFunction = new Regex("\\" + prechar + @"{(?<fun>\w+)\((?<par>.*?)\)}");
            Match match = regFunction.Match(code);
            while (match != null && match.Success)
            {
                string fun = match.Result("${fun}");
                string par = match.Result("${par}");
                string result = GetFunctionResult(fun, par, _referenceDateTime);
                //替换
                if (string.IsNullOrWhiteSpace(result) == false)
                {
                    code = code.Replace(prechar + "{" + fun + "(" + par + ")}", result);
                }

                match = match.NextMatch();
            }

            return code;
        }

        /// <summary>
        /// 获取日期函数执行结果
        /// </summary>
        /// <param name="function">函数名</param>
        /// <param name="para">参数</param>
        /// <param name="referenceDateTime">基准日期</param>
        /// <returns></returns>
        private static string GetFunctionResult(string function, string para, DateTime referenceDateTime)
        {
            int p = 0;
            int.TryParse(para, out p);
            Base b = new Base();
            b.SetReferenceDateTime(referenceDateTime);

            switch (function)
            {
                case "day":
                    return b.day(p);
                case "day_of_month":
                    return b.day_of_month(p).ToString();
                case "day_of_month2":
                    return b.day_of_month2(p);
                case "last_day":
                    return b.last_day(p);
                case "month":
                    return b.month(p);
                case "month_of_year":
                    return b.month_of_year(p).ToString();
                case "month_of_year2":
                    return b.month_of_year2(p);
                case "year":
                    return b.year(p).ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// 拼凑节点执行代码块
        /// </summary>
        /// <param name="csharpCode"></param>
        /// <param name="csharpFun"></param>
        /// <returns></returns>
        private static string GenerateCode(string csharpCode, string csharpFun)
        {
            csharpCode = ReplaceDataTime(csharpCode, DateTime.Now);

            string code =
@"using System;
namespace CS.ScriptService.Script
{
    public class ScripRunner : Base
    {
        public bool Run()
        {
            try
            {
                //载入脚本内容>>>>>>>>
@(csharpCode)
                //结束脚本内容<<<<<<<<
                return true;
            }
            catch (Exception err)
            {
                //脚本执行失败处理
                WriteErrorMessage(err.ToString(), 3);
                return false;
            }
        }
        //加载自定义函数>>>>>>>>
@(csharpFun)
        //结束自定义函数<<<<<<<<
    }
}";
            code = code.Replace("@(csharpCode)", csharpCode);
            code = code.Replace("@(csharpFun)", csharpFun);
            return code;
        }

        /// <summary>
        /// 替换@{day(0)}、@{month(0)}、@{years(0)},@{last_day()}
        /// </summary>
        /// <param name="inString"></param>
        /// <param name="nowDatetime"></param>
        /// <returns></returns>
        private static string ReplaceDataTime(string inString, DateTime nowDatetime)
        {
            inString = inString.Replace("@{day}", "@{day(0)}");
            inString = inString.Replace("@{month}", "@{month(0)}");
            inString = inString.Replace("@{years}", "@{year(0)}");
            inString = inString.Replace("@{years", "@{year");
            inString = inString.Replace("@{last_day}", "@{last_day(0)}");

            var sql = inString;
            int nowPlace = 0;
            {
                int s = sql.IndexOf("@{day(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 6;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }
                        sql = sql.Replace("@{day(" + per + ")}", nowDatetime.AddDays(per).ToString("yyyyMMdd"));
                        if (per == 0)
                        {
                            sql = sql.Replace("@{day()}", nowDatetime.AddDays(per).ToString("yyyyMMdd"));
                        }

                        s = sql.IndexOf("@{day(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            {
                int s = sql.IndexOf("@{month(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 8;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }
                        sql = sql.Replace("@{month(" + per + ")}", nowDatetime.AddMonths(per).ToString("yyyyMM"));
                        if (per == 0)
                        {
                            sql = sql.Replace("@{month()}", nowDatetime.AddMonths(per).ToString("yyyyMM"));
                        }

                        s = sql.IndexOf("@{month(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }
            {
                int s = sql.IndexOf("@{year(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 7;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }
                        sql = sql.Replace("@{year(" + per + ")}", nowDatetime.AddYears(per).ToString("yyyy"));
                        if (per == 0)
                        {
                            sql = sql.Replace("@{year()}", nowDatetime.AddYears(per).ToString("yyyy"));
                        }

                        s = sql.IndexOf("@{year(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            {
                int s = sql.IndexOf("@{last_day(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 11;
                        int per = 0;
                        if (e > s)
                        {
                            per = Convert.ToInt32(sql.Substring(s, e - s));
                        }

                        DateTime temp = new DateTime(nowDatetime.Year, nowDatetime.Month, 1);
                        var tmpV = temp.AddMonths(per + 1).AddDays(-1).ToString("yyyyMMdd");

                        sql = sql.Replace("@{last_day(" + per + ")}", tmpV);
                        if (per == 0)
                        {
                            sql = sql.Replace("@{last_day()}", tmpV);
                        }

                        s = sql.IndexOf("@{last_day(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            return sql;
        }
    }
}
