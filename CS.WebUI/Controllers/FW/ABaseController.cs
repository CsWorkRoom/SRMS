using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Base.Log;
using Newtonsoft.Json;
using System.Data;
using CS.Library.Export;
using CS.BLL.FW;
using System.Text.RegularExpressions;
using System.IO;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 框架控制器基类
    /// </summary>
    public abstract class ABaseController : Controller
    {
        /// <summary>
        /// JSON返回对象
        /// </summary>
        public class JsonResultData
        {
            /// <summary>
            /// 是否执行成功
            /// </summary>
            public bool IsSuccess { get; set; }
            /// <summary>
            /// 返回信息
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// 返回结果详情（一般可以不使用）
            /// </summary>
            public string Result { get; set; }
        }

        /// <summary>
        /// 序列化格式设置
        /// </summary>

        private static JsonSerializerSettings _jsonSerializerSettings = null;

        /// <summary>
        /// 发布网站的根目录（URL中的应用程序目录）
        /// </summary>
        public static string ApplicationPath = "";


        /// <summary>
        /// 在调用Action方法前，验证访问权限
        /// </summary>
        /// <param name="context"></param>
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.Title = MvcApplication.SystemName + context.Controller.GetType().ToString();

            string controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            string actionName = context.ActionDescriptor.ActionName.ToLower();
            //不带参数的路径
            string path = path = "/" + context.RequestContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath.ToLower().Trim(new char[] { '~', '/' });
            //BLog.Write(BLog.LogLevel.DEBUG, "path:" + path);
            string rawUrl = context.HttpContext.Request.RawUrl;
            if (Request.ApplicationPath != "/" && ApplicationPath != Request.ApplicationPath)
            {
                ApplicationPath = Request.ApplicationPath;
            }

            ///  /home/login4a  提供给4A的‘单点登录地址’
            ///  /home/login  登录地址
            ///  /AfUser/UpdateAppAcctSoap  提供给4A的‘帐号变更接口’
            if (path == "/aferror/index"
                || path == "/home/login"
                || path == "/home/securitycode"
                || path.ToLower() == "/home/login4a"
                || path.ToLower() == "/AfUser/UpdateAppAcctSoap".ToLower())
            {
                return;
            }

            if (SystemSession.UserID < 1)
            {
                context.Result = Redirect("~/Home/Login");
                return;
            }

            try
            {
                //验证访问权限
                CheckPermission(path, rawUrl.ToLower());
            }
            catch (Exception ex)
            {
                context.Result = Content("<script>alert('没有权限访问页面：" + rawUrl + "');</script>");// Redirect("~/AfError/Index?message=" + ex.Message + "&url=" + Server.UrlEncode(rawUrl));
                BLog.Write(BLog.LogLevel.WARN, "用户：" + SystemSession.UserName + "访问了无权限的URL：" + HttpUtility.HtmlEncode(rawUrl));
                return;
            }

            //记录通用日志
            try
            {
                string srcIP = context.HttpContext.Request.UserHostAddress;
                int srcPort = context.HttpContext.Request.Url.Port;

                string content = "请求方式：" + context.HttpContext.Request.RequestType;
                BF_OP_LOG.Instance.AddLog(BLog.LogLevel.INFO, true, srcIP, srcPort, controllerName, actionName, rawUrl, content);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "记录操作日志出错：" + ex.ToString());
            }
        }

        /// <summary>
        /// 提取URL中参数
        /// </summary>
        private static Regex _regexParam = new Regex(@"(?<param>[^\?&]+?=[^\?&]*)");

        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="path">不带参数的路径</param>
        /// <param name="rawUrl">带参数的URL</param>
        /// <returns></returns>
        private void CheckPermission(string path, string rawUrl)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/" || path == "/home/index")
            {
                return;
            }
            if (SystemSession.UserMenuURLs.ContainsKey(path))
            {
                return;
            }

            Match matchUrl = _regexParam.Match(rawUrl);
            if (matchUrl.Success == false)
            {
                throw new Exception("没有权限访问页面");
            }
            Dictionary<string, byte> dicParams = new Dictionary<string, byte>();
            while (matchUrl.Success)
            {
                string para = matchUrl.Result("${param}");
                if (dicParams.ContainsKey(para) == false)
                {
                    dicParams.Add(para, 1);
                }
                matchUrl = matchUrl.NextMatch();
            }

            foreach (var kvp in SystemSession.UserMenuURLs)
            {
                if (kvp.Key.StartsWith(path))
                {
                    Match matchMenu = _regexParam.Match(kvp.Key);
                    bool isMatched = true;
                    while (matchMenu.Success == true)
                    {
                        string para = matchMenu.Result("${param}");
                        if (dicParams.ContainsKey(para) == false)
                        {
                            isMatched = false;
                            break;
                        }
                        matchMenu = matchMenu.NextMatch();
                    }
                    if (isMatched == true)
                    {
                        return;
                    }
                }
            }

            throw new Exception("没有权限访问页面");
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext context)
        {
            try
            {
                string filepath = context.RequestContext.HttpContext.Request.FilePath;
                string controllerName = "未知";
                string actionName = "未知";
                if (string.IsNullOrWhiteSpace(filepath) == false)
                {
                    string[] ss = filepath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ss.Length > 0)
                    {
                        controllerName = ss[0];
                        if (ss.Length > 1)
                        {
                            actionName = ss[1];
                        }
                    }
                }
                string rawUrl = context.HttpContext.Request.RawUrl;
                string srcIP = context.HttpContext.Request.UserHostAddress;
                int srcPort = context.HttpContext.Request.Url.Port;
                string content = "出现未知错误：" + context.Exception.Message;
                if (content.Length > 512)
                {
                    content = content.Substring(0, 512);
                }

                //BF_OP_LOG.Instance.AddLog(BLog.LogLevel.WARN, false, srcIP, srcPort, controllerName, actionName, rawUrl, content, context.Exception.ToString());


                BF_OP_LOG.Instance.AddLog(BLog.LogLevel.WARN, false, srcIP, srcPort, controllerName, actionName, rawUrl, content, context.Exception.ToString());
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "记录错误日志出错：" + ex.ToString());
            }

            BLog.Write(BLog.LogLevel.ERROR, context.Exception.ToString());
            Redirect("~/AfError/Index");
        }

        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="modular">功能模块</param>
        /// <param name="operation">操作</param>
        /// <param name="url">请求的URL</param>
        /// <param name="content">日志内容</param>
        /// <param name="detail">操作详情</param>
        public void WriteOperationLog(BLog.LogLevel level, bool isSuccess, string modular, string operation, string url, string content, string detail = "")
        {
            try
            {
                string srcIP = GetClientIP();
                int srcPort = 0;
                //BF_OP_LOG.Instance.AddLog(BLog.LogLevel.WARN, isSuccess, srcIP, srcPort, modular, operation, url, content, detail);

                BF_OP_LOG.Instance.AddLog(BLog.LogLevel.WARN, isSuccess, srcIP, srcPort, modular, operation, url, content, detail);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "记录操作日志出错：" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        private string GetClientIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
            {
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            }
            return ip;
        }

        /// <summary>
        /// 将对象转换为JSON字符串并外加一层LAYUI所需信息
        /// </summary>
        /// <param name="data">返回数据对象（一般为一个List）</param>
        /// <param name="count">查询数量</param>
        /// <param name="code">返回代码（0表示成功）</param>
        /// <param name="message">错误信息</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="sqlParams">SQL语句参数</param>
        /// <returns></returns>
        protected string ToJsonString(object data, int count, int code = 0, string message = "查询成功", string sql = "", string sqlParams = "")
        {
            if (_jsonSerializerSettings == null)
            {
                _jsonSerializerSettings = new JsonSerializerSettings();
                _jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("code", code);
            dic.Add("msg", message);
            dic.Add("count", count);
            dic.Add("data", data);
            if (string.IsNullOrWhiteSpace(sql) == false)
            {
                dic.Add("sql", sql);
                dic.Add("sqlparam", sqlParams);
            }

            //转为JSON字符串
            return JsonConvert.SerializeObject(dic, _jsonSerializerSettings);
        }

        /// <summary>
        /// 序列化为JSON字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string SerializeObject(object data)
        {
            if (_jsonSerializerSettings == null)
            {
                _jsonSerializerSettings = new JsonSerializerSettings();
                _jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }

            //转为JSON字符串
            return JsonConvert.SerializeObject(data, _jsonSerializerSettings);
        }

        /// <summary>
        /// 将JSON字符串转换为.NET对象
        /// </summary>
        /// <typeparam name="T">.NET对象类型</typeparam>
        /// <param name="jsonstring">将JSON字符串</param>
        /// <returns></returns>
        protected T DeserializeObject<T>(string jsonstring)
        {
            return JsonConvert.DeserializeObject<T>(jsonstring);
        }

        /// <summary>
        /// 显示JS Alert提示框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ActionResult ShowAlert(string message)
        {
            return Content("<script>alert('" + message.Replace("'", "‘").Replace("\r\n", " ") + "');</script>");
        }

        #region 导出文件

        /// <summary>
        /// 将数据导出到EXCEL文件
        /// </summary>
        /// <param name="fileName">显示文件名</param>
        /// <param name="table">数据</param>
        /// <returns></returns>
        protected ActionResult ExportExcel(string fileName, DataTable table)
        {
            if (table == null)
            {
                return ShowAlert("未查到数据");
            }

            if (table.Rows.Count < 1)
            {
                return ShowAlert("报表查无数据，将不生成文件");
            }

            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

            string fullName = string.Empty;

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            if (table.Rows.Count > 100000)
            {
                BLog.Write(BLog.LogLevel.DEBUG, "由于数据超过10万行，报表将导出为.csv");
                fileName = fileName.Replace(".xlsx", ".csv");
                fullName = path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                using (FileStream fs = new FileStream(fullName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (StreamWriter sw = new StreamWriter(fs, new System.Text.UTF8Encoding(true)))
                    {
                        foreach (DataColumn col in table.Columns)
                        {
                            sw.Write("\"" + col.Caption + "\",");
                        }
                        sw.WriteLine();

                        foreach (DataRow dr in table.Rows)
                        {
                            foreach (DataColumn col in table.Columns)
                            {
                                sw.Write("\"" + Convert.ToString(dr[col]) + "\",");
                            }
                            sw.WriteLine();
                            sw.Flush();
                        }
                    }
                }
            }
            else
            {
                ExcelFile export = new ExcelFile(path);
                fullName = export.ToExcel(table);
                if (string.IsNullOrWhiteSpace(fullName) == true)
                {
                    return ShowAlert("未生成Excel文件");
                }
            }

            return ExportFile(fullName, fileName, true);
        }

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="fullName">文件全名</param>
        /// <param name="fileName">导出显示的文件名</param>
        /// <param name="isDeleteTempFile">导出后是否删除文件</param>
        /// <returns></returns>
        protected ActionResult ExportFile(string fullName, string fileName, bool isDeleteTempFile = false)
        {
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            System.Web.HttpContext.Current.Response.WriteFile(fullName);
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
            //删除文件
            if (isDeleteTempFile == true)
            {
                System.IO.File.Delete(fullName);
            }

            return ShowAlert("导出成功");
        }

        #endregion

        /// <summary>
        /// 是否接入4A
        /// </summary>
        /// <returns></returns>
        protected bool IsConnectTo4A()
        {
            bool resbool = false;
            try
            {
                resbool = Convert.ToBoolean(Base.Config.BConfig.GetConfigToString("IsConnectTo4A"));
            }
            catch
            { }
            return resbool;
        }
    }
}