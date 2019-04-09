using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS.Common.FW
{
    /// <summary>
    /// 框架内置函数
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// 任务开始日期
        /// </summary>
        private DateTime _beginDate = DateTime.Now;
        /// <summary>
        /// 任务结束日期
        /// </summary>
        private DateTime _endDate = DateTime.Now;

        /// <summary>
        /// 自定义函数（忽略大小写）
        /// </summary>
        private static Regex _regexFunction = new Regex(@"@{(?<fun>[a-z_]+)\((?<par>[\d\-]*?)\)}", RegexOptions.IgnoreCase);

        /// <summary>
        /// 单例
        /// </summary>
        public static Functions Instance
        {
            get
            {
                return new Functions(DateTime.Today, DateTime.Today);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        public Functions(DateTime beginDate, DateTime endDate)
        {
            _beginDate = beginDate;
            _endDate = endDate;
        }

        /// <summary>
        /// 计算两个日期相差的月数
        /// </summary>
        /// <param name="begin">起始日期</param>
        /// <param name="end">结束日期</param>
        /// <returns></returns>
        public static int TotalMonths(DateTime begin, DateTime end)
        {
            return (end.Year - begin.Year) * 12 + (end.Month - begin.Month);
        }

        /// <summary>
        /// 执行自定义函数并替换里面的内容
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <returns></returns>
        public string ExecuteFunction(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            Match match = _regexFunction.Match(content);
            while (match != null && match.Success)
            {
                string fun = match.Result("${fun}");
                string par = match.Result("${par}");
                bool isMatch = false;
                string result = GetFunctionResult(fun, par, out isMatch);
                //替换
                if (isMatch == true)
                {
                    content = content.Replace("@{" + fun + "(" + par + ")}", result);
                }

                match = match.NextMatch();
            }
            return content;
        }

        /// <summary>
        /// 获取日期函数执行结果
        /// </summary>
        /// <param name="function">函数名</param>
        /// <param name="para">参数</param>
        /// <param name="isMatch">是否是自定义函数</param>
        /// <returns></returns>
        private string GetFunctionResult(string function, string para, out bool isMatch)
        {
            int p = 0;
            if (string.IsNullOrWhiteSpace(para) == false && int.TryParse(para, out p) == false)
            {
                isMatch = false;
                return "错误参数：" + para + "，参数必须是整数";
            }

            isMatch = true;
            switch (function)
            {
                case "yyyy":
                    return yyyy(p);
                case "yyyymm":
                    return yyyymm(p);
                case "yyyymmdd":
                    return yyyymmdd(p);
                case "datetime_now":
                    return "to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + "','yyyy-mm-dd')";
                case "datetime_begin":
                    return "to_date('" + _beginDate.ToString("yyyy-MM-dd") + "','yyyy-mm-dd')";
                case "datetime_end":
                    return "to_date('" + _endDate.ToString("yyyy-MM-dd") + " 23:59:59" + "','yyyy-mm-dd hh24:mi:ss')";
            }

            isMatch = false;
            return string.Empty;
        }

        #region 函数实现


        /// <summary>
        /// 获取年份
        /// </summary>
        /// <param name="y">年份偏移量</param>
        /// <returns></returns>
        public string yyyy(int y = 0)
        {
            return _beginDate.AddYears(y).ToString("yyyy");
        }

        /// <summary>
        /// 获取年月
        /// </summary>
        /// <param name="m">月份偏移量</param>
        /// <returns></returns>
        public string yyyymm(int m = 0)
        {
            return _beginDate.AddMonths(m).ToString("yyyyMM");
        }

        /// <summary>
        /// 获取日期
        /// </summary>
        /// <param name="d">偏移天数</param>
        /// <returns></returns>
        public string yyyymmdd(int d = 0)
        {
            return _beginDate.AddDays(d).ToString("yyyyMMdd");
        }

        #endregion

        #region 解析区间日期
        /// <summary>
        /// 解析区间日期
        /// </summary>
        /// <param name="strIntervalDate">原日期字符串</param>
        /// <param name="beginDate">返回开始时间</param>
        /// <param name="endDate">返回结束时间</param>
        public static void GetIntervalDate(string strIntervalDate, ref DateTime beginDate, ref DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(strIntervalDate) == false)
            {
                string[] ss = strIntervalDate.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length == 2)
                {
                    DateTime.TryParse(ss[0], out beginDate);
                    DateTime.TryParse(ss[1], out endDate);
                }
            }
        }
        #endregion

        #region 将UNIX时间转成C#时间格式
        /// <summary>
        /// 将UNIX时间戳转成C#时间格式
        /// </summary>
        /// <param name="time">UNIX时间时间戳</param>
        /// <returns>C#时间格式</returns>
        public static DateTime ConvertUnixDateToDateTime(long time)
        {            
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(time);
            return dt;
        }
        #endregion

        #region 将C#时间格式转成Unix时间戳
        /// <summary>
        /// 将C#时间格式转成Unix时间戳
        /// </summary>
        /// <param name="time">C#时间</param>
        /// <returns>Unix时间戳</returns>
        public static long ConvertDateTimeToUnixDate(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)(time - startTime).TotalSeconds; // 相差秒数 
        }
        #endregion
    }
}
