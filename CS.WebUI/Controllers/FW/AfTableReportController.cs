using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using CS.Library.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static CS.Common.FW.Enums;


namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 表格报表
    /// </summary>
    public class AfTableReportController : ABaseController
    {
        #region 报表管理页面

        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.DIC_DB_NAME = BF_DATABASE.Instance.GetDictionary();
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BF_TB_REPORT.Entity> list = BF_TB_REPORT.Instance.GetListPage<BF_TB_REPORT.Entity>(limit, page, order, where);
            int count = BF_TB_REPORT.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BF_TB_REPORT.Entity>();
                count = 0;
            }

            return ToJsonString(list, count);
        }

        #endregion

        #region 编辑报表基本信息

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            BF_TB_REPORT.Entity entity = new BF_TB_REPORT.Entity();
            entity.IS_SHOW_EXPORT = 1;
            entity.IS_SHOW_DEBUG = 1;

            if (id > 0)
            {
                entity = BF_TB_REPORT.Instance.GetEntityByKey<BF_TB_REPORT.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "报表不存在";
                    entity = new BF_TB_REPORT.Entity();
                    entity.ID = -1;
                }
            }

            ModelState.Clear();
            return View(entity);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id">ID（验证SQL用）</param>
        /// <param name="dbid">数据库ID（验证SQL用）</param>
        /// <param name="sql">SQL语句，验证用</param>
        /// <param name="inputjson">辅助验证SQL的参数</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(BF_TB_REPORT.Entity entity, int dbid = -1, string sql = "", string inputjson = "")
        {
            JsonResultData result = new JsonResultData();
            //SQL分析
            if (dbid >= 0 && string.IsNullOrWhiteSpace(sql) == false)
            {
                try
                {
                    BF_TB_REPORT.CheckSql(dbid, sql, inputjson);
                    result.IsSuccess = true;
                    result.Message = "SQL分析成功，点击“配置”可进行高级配置";
                }
                catch (Exception e)
                {
                    result.Message = "分析出错：" + e.Message;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //编辑
            int i = 0;
            try
            {
                CheckInput(entity);

                if (entity.ID < 0)
                {
                    result.Message = "报表不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (entity.ID == 0)
                {
                    entity.IS_ENABLE = 1;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    entity.UPDATE_UID = SystemSession.UserID;
                    entity.UPDATE_TIME = DateTime.Now;
                    i = BF_TB_REPORT.Instance.Add(entity);
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("NAME", entity.NAME.Trim());
                    dic.Add("DB_ID", entity.DB_ID);
                    dic.Add("IS_SHOW_EXPORT", entity.IS_SHOW_EXPORT);
                    dic.Add("IS_SHOW_DEBUG", entity.IS_SHOW_DEBUG);
                    dic.Add("IS_SHOW_CHECKBOX", entity.IS_SHOW_CHECKBOX);
                    dic.Add("SQL_CODE", entity.SQL_CODE);
                    dic.Add("DEFAULT_INPUT_VALUES", entity.DEFAULT_INPUT_VALUES);
                    dic.Add("REMARK", entity.REMARK);
                    dic.Add("TOP_CODE", entity.TOP_CODE);
                    dic.Add("BOTTOM_CODE", entity.BOTTOM_CODE);
                    dic.Add("UPDATE_UID", SystemSession.UserID);
                    dic.Add("UPDATE_TIME", DateTime.Now);
                    i = BF_TB_REPORT.Instance.UpdateByKey(dic, entity.ID);
                }

                if (i < 1)
                {
                    result.Message = "出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.IsSuccess = true;
                result.Message = "保存成功";

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "编辑表格报表出错：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <param name="entity"></param>
        private void CheckInput(BF_TB_REPORT.Entity entity)
        {
            if (entity == null)
            {
                throw new Exception("对象为空");
            }
            if (string.IsNullOrWhiteSpace(entity.NAME))
            {
                throw new Exception("报表名称不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                throw new Exception("SQL语句不可为空");
            }

            if (BF_TB_REPORT.Instance.IsDuplicate(entity.ID, "NAME", entity.NAME))
            {
                throw new Exception("报表名称 " + entity.NAME + " 已经存在");
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetEnable(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "启用成功";
            try
            {
                int i = BF_TB_REPORT.Instance.SetEnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "启用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetUnable(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "禁用成功";
            try
            {
                int i = BF_TB_REPORT.Instance.SetUnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "禁用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "删除成功";

            try
            {
                int i = BF_TB_REPORT.Instance.DeleteByKey(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "删除失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 配置报表详细信息

        #region 报表配置
        /// <summary>
        /// 报表配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Configure(int id)
        {
            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            BF_TB_REPORT.Entity entity = BF_TB_REPORT.Instance.GetEntityByKey<BF_TB_REPORT.Entity>(id);
            if (entity == null)
            {
                ViewBag.Result = false;
                ViewBag.Message = "报表不存在";
                return View();
            }
            if (entity.IS_ENABLE != 1)
            {
                ViewBag.Result = false;
                ViewBag.Message = "报表已被禁用";
                return View();
            }

            ViewBag.ID = id;
            ViewBag.xlId = Enums.FormQueryType.下拉单选框.GetHashCode();
            ViewBag.treexl = Enums.FormQueryType.下拉树选择.GetHashCode();
            ViewBag.FieldDataType = GetFieldDataType();
            ViewBag.InputQueryType = GetInputQueryType();
            ViewBag.RequestMode = GetRequestMode();
            ViewBag.FieldData = GetFieldsForConfigure(entity);
            ViewBag.FilterData = GetFilterData(entity);
            ViewBag.EventData = GetEventData(entity);
            ViewBag.LoadIconNames = "var iconNames = " + SerializeObject(new AfMenuController().LoadIconNames()) + ";";//图标列表
            return View();
        }
        #endregion

        #region 字段数据类型
        /// <summary>
        /// 字段数据类型
        /// </summary>
        /// <returns></returns>
        private string GetFieldDataType()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            foreach (Enums.FieldDataType mode in Enum.GetValues(typeof(Enums.FieldDataType)))
            {
                sb.AppendLine("  fieldDataType[" + (int)mode + "] ='" + mode.ToString() + "';");
            }
            sb.AppendLine("</script>");
            return sb.ToString();
        }
        #endregion

        #region 样式类型
        /// <summary>
        /// 样式类型
        /// </summary>
        /// <returns></returns>
        private string GetEventType()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            foreach (Enums.EventType mode in Enum.GetValues(typeof(Enums.EventType)))
            {
                sb.AppendLine("  fieldDataType[" + (int)mode + "] ='" + mode.ToString() + "';");
            }
            sb.AppendLine("</script>");
            return sb.ToString();
        }
        #endregion

        #region 按钮样式

        /// <summary>
        /// 报表配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Style(string value)
        {
            ViewBag.DeValue = value;//默认选中的值
            ViewBag.LoadIconNames = "var iconNames = " + SerializeObject(new AfMenuController().LoadIconNames()) + ";";//图标列表
            return View();
        }
        #endregion

        #region 输入框类型
        /// <summary>
        /// 输入框类型
        /// </summary>
        /// <returns></returns>
        private string GetInputQueryType()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            foreach (Enums.FormQueryType mode in Enum.GetValues(typeof(Enums.FormQueryType)))
            {
                sb.AppendLine("  inputQueryType[" + (int)mode + "] ='" + mode.ToString() + "';");
            }
            sb.AppendLine("</script>");
            return sb.ToString();
        }
        #endregion

        #region 请求模式
        /// <summary>
        /// 请求模式
        /// </summary>
        /// <returns></returns>
        private string GetRequestMode()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (Enums.RequestMode mode in Enum.GetValues(typeof(Enums.RequestMode)))
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Key", (int)mode);
                dic.Add("Value", mode.ToString());
                list.Add(dic);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("  requestMode =");
            sb.AppendLine(SerializeObject(list) + ";");
            sb.AppendLine("</script>");
            return sb.ToString();
        }
        #endregion

        #region 获取字段信息
        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetFieldsForConfigure(BF_TB_REPORT.Entity entity)
        {
            //系统配置的通用字段
            Dictionary<string, BF_FIELD.Entity> dicSystemFields = BF_FIELD.Instance.GetDictionary();
            //报表配置的字段信息（上次已经配置过的）
            Dictionary<string, Models.FW.TbrShowFieldModel> dicReportFields = new Dictionary<string, Models.FW.TbrShowFieldModel>();

            int count = 1;
            string sql = string.Empty;
            List<object> paraList = new List<object>();

            //查询报表
            DataTable dt = BF_TB_REPORT.QueryTable(entity, 1, 1, ref count, out sql, out paraList, "", entity.DEFAULT_INPUT_VALUES, "");
            //原有配置的字段
            if (string.IsNullOrWhiteSpace(entity.SHOW_FIELDS) == false)
            {
                List<Models.FW.TbrShowFieldModel> list = DeserializeObject<List<Models.FW.TbrShowFieldModel>>(entity.SHOW_FIELDS);
                foreach (var model in list)
                {
                    string en = model.EN_NAME.ToUpper();
                    if (dicReportFields.ContainsKey(en) == false && dt.Columns.Contains(en) == true)
                    {
                        Int16 type = (Int16)Enums.FieldDataType.数值.GetHashCode();
                        if (dt.Columns[en].DataType == typeof(DateTime))
                        {
                            type = (Int16)Enums.FieldDataType.日期.GetHashCode();
                        }
                        else if ((dt.Columns[en].DataType == typeof(string)))
                        {
                            type = (Int16)Enums.FieldDataType.文本.GetHashCode();
                        }
                        model.FIELD_DATA_TYPE = type;
                        dicReportFields.Add(model.EN_NAME.ToUpper(), model);
                    }
                }
            }

            //报表字段
            List<Models.FW.TbrShowFieldModel> listReportFields = new List<Models.FW.TbrShowFieldModel>();

            foreach (DataColumn col in dt.Columns)
            {
                string en = col.ColumnName.ToUpper();
                if (dicReportFields.ContainsKey(en) == true)
                {
                    listReportFields.Add(dicReportFields[en]);
                }
                else
                {
                    short width = 80;
                    Enums.FieldDataType dataType = Enums.FieldDataType.数值;
                    if (col.DataType == typeof(string))
                    {
                        dataType = Enums.FieldDataType.文本;
                        width = 120;
                    }
                    else if (col.DataType == typeof(DateTime))
                    {
                        dataType = Enums.FieldDataType.日期;
                        width = 135;
                    }
                    if (en == "ID")
                    {
                        width = 60;
                    }
                    Models.FW.TbrShowFieldModel model = new Models.FW.TbrShowFieldModel();
                    model.EN_NAME = en;
                    model.FIELD_DATA_TYPE = (short)dataType.GetHashCode();
                    if (dicSystemFields.ContainsKey(en))
                    {
                        model.CN_NAME = dicSystemFields[en].CN_NAME;
                        model.IS_SHOW = dicSystemFields[en].IS_SHOW;
                        model.SHOW_WIDTH = dicSystemFields[en].SHOW_WIDTH;
                    }
                    else
                    {
                        model.CN_NAME = en;
                        model.IS_SHOW = 1;
                        model.SHOW_WIDTH = width;
                    }
                    if (model.FIELD_DATA_TYPE != (short)Enums.FieldDataType.文本)
                    {
                        model.IS_ENCRYPT = 0;
                    }
                    listReportFields.Add(model);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("var fieldData =");
            sb.AppendLine(SerializeObject(listReportFields) + ";");
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        #endregion

        #region 筛选
        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetFilterData(BF_TB_REPORT.Entity entity)
        {
            List<BF_TB_REPORT_FILTER.Entity> list = BF_TB_REPORT_FILTER.Instance.GetFilterList(entity.ID);
            if (list == null || list.Count < 1)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine(" filterData =");
            sb.AppendLine(SerializeObject(list) + ";");
            sb.AppendLine("</script>");

            return sb.ToString();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetEventData(BF_TB_REPORT.Entity entity)
        {
            List<BF_TB_REPORT_EVENT.Entity> list = BF_TB_REPORT_EVENT.Instance.GetEventList(entity.ID);
            if (list == null || list.Count < 1)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine(" eventData =");
            sb.AppendLine(SerializeObject(list) + ";");
            sb.AppendLine("</script>");

            return sb.ToString();
        }
        #endregion

        #region 保存配置
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Configure(int id, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                int i = BF_TB_REPORT.Instance.SetShowFields(id, collection["FIELD_CONFIG"]);
                int fc = 0;
                int fca = 0;
                int fcu = 0;
                int fcd = 0;
                int ec = 0;
                if (i > 0)
                {
                    if (string.IsNullOrWhiteSpace(collection["FILTER_CONFIG"]) == false)
                    {
                        List<BF_TB_REPORT_FILTER.Entity> filtetList = DeserializeObject<List<BF_TB_REPORT_FILTER.Entity>>(collection["FILTER_CONFIG"]);
                        fc = BF_TB_REPORT_FILTER.Instance.SetFilteData(id, filtetList, out fca, out fcu, out fcd);
                    }
                    if (string.IsNullOrWhiteSpace(collection["EVENT_CONFIG"]) == false)
                    {
                        List<BF_TB_REPORT_EVENT.Entity> eventList = DeserializeObject<List<BF_TB_REPORT_EVENT.Entity>>(collection["EVENT_CONFIG"]);
                        ec = BF_TB_REPORT_EVENT.Instance.SetEventData(id, eventList, out fca, out fcu, out fcd);
                    }
                }
                result.IsSuccess = true;
                result.Message = string.Format("保存成功，共有{0}个字段，{1}个筛选项，{2}个事件", i, fc, ec);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region 报表展示


        /// <summary>
        /// 展示报表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Show(int id)
        {
            if (id < 1)
            {
                return ShowAlert("请选择正确的报表");
            }
            //GET参数
            string queryString = Request.QueryString.ToString();
            if (string.IsNullOrWhiteSpace(queryString))
            {
                queryString = "id=" + id;
            }
            try
            {
                BF_TB_REPORT.Entity entity = BF_TB_REPORT.Instance.GetEntityByKey<BF_TB_REPORT.Entity>(id);
                if (entity == null)
                {
                    return ShowAlert("报表不存在");
                }
                if (entity.IS_ENABLE != 1)
                {
                    return ShowAlert("报表已被禁用");
                }

                List<BF_TB_REPORT_FILTER.Entity> filterList = BF_TB_REPORT_FILTER.Instance.GetFilterList(entity.ID);
                List<BF_TB_REPORT_EVENT.Entity> eventList = BF_TB_REPORT_EVENT.Instance.GetEventList(entity.ID);
                List<BF_TB_REPORT_FILTER.Entity> defaultFilterList = new List<BF_TB_REPORT_FILTER.Entity>();
                List<BF_TB_REPORT_FILTER.Entity> advancedFilterList = new List<BF_TB_REPORT_FILTER.Entity>();
                List<BF_TB_REPORT_EVENT.Entity> topEventList = null;
                List<BF_TB_REPORT_EVENT.Entity> rowEventList = null;
                if (filterList != null && filterList.Count > 0)
                {
                    defaultFilterList = filterList.Where(f => f.FILTER_TYPE == Enums.FilterType.默认筛选.GetHashCode()).ToList();
                    advancedFilterList = filterList.Where(f => f.FILTER_TYPE == Enums.FilterType.高级筛选.GetHashCode()).ToList();
                }
                if (eventList != null && eventList.Count > 0)
                {
                    topEventList = eventList.Where(f => f.EVENT_TYPE == Enums.EventType.表头事件.GetHashCode()).ToList();
                    rowEventList = eventList.Where(f => f.EVENT_TYPE == Enums.EventType.行事件.GetHashCode()).ToList();
                }
                ViewBag.ID = id;
                ViewBag.QUERY_STRING = System.Web.HttpUtility.UrlDecode(queryString);
                //默认筛选项
                ViewBag.DefaultFilterHtml = GenerateDefaultFilterHtml(entity, defaultFilterList);
                //高级筛选
                ViewBag.AdvancedFilterHtml = GenerateAdvancedFilterHtml(entity, advancedFilterList);
                //日期筛选框
                ViewBag.DatetimeFilterHtml = GenerateDatetimeFilterHtml(entity, filterList);
                //顶部按钮
                ViewBag.TopButtonHtml = GenerateTopButtonHtml(entity, topEventList, advancedFilterList.Count > 0);
                //顶部代码
                ViewBag.TopCodeHtml = GenerateTopCodeHtml(entity);
                //表格渲染
                ViewBag.TableRender = GenerateTableRender(entity, rowEventList, queryString, string.IsNullOrWhiteSpace(entity.BOTTOM_CODE) ? 0 : 50);
                //事件代码
                ViewBag.EventHtml = GenerateEventHtml(entity, rowEventList);
                //底部代码
                ViewBag.BottomCodeHtml = GenerateBottomCodeHtml(entity);

                return View();
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "查询报表[" + id + "]出现未知错误：" + ex.ToString());
                return ShowAlert("查询报表出错，详见运行日志：" + ex.Message);
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="id">报表配置ID</param>
        /// <param name="page">页码</param>
        /// <param name="limit">分页大小</param>
        /// <param name="input">自定义输入项</param>
        /// <param name="count">记录条数</param>
        /// <param name="where">筛选项JSON串</param>
        /// <param name="orderByField">排序字段</param>
        /// <param name="orderByType">排序方式</param>
        /// <returns></returns>
        [HttpPost]
        public string Show(int id, int page, int limit, int count = 0, string input = "", string where = "", string orderByField = "", string orderByType = "")
        {
            string sql = string.Empty;
            List<object> paramList = new List<object>();
            try
            {
                BF_TB_REPORT.Entity entity = BF_TB_REPORT.Instance.GetEntityByKey<BF_TB_REPORT.Entity>(id);

                if (entity == null)
                {
                    throw new Exception("报表" + id + "不存在");
                }

                //GET参数
                string getQueryString = HttpUtility.UrlDecode(Request.QueryString.ToString());
                int rowsCount = count;
                Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);
                DataTable dt = BF_TB_REPORT.QueryTable(id, page, limit, ref rowsCount, out sql, out paramList, getQueryString, input, where, order);

                //加密
                Encrypt(entity, ref dt);

                return ToJsonString(dt, rowsCount, 0, "", sql, string.Join(",\r\n", paramList));
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "查询报表" + id + "出错：\r\n" + ex.ToString());
                return ToJsonString(null, 0, 1, "查询报表出错，详见运行日志", sql, string.Join(",\r\n", paramList));
            }
        }

        /// <summary>
        /// 生成筛选项的HTML标签及设置默认值
        /// </summary>
        /// <param name="filterList">筛选项</param>
        /// <param name="html">HTML标签</param>
        /// <param name="values">默认值</param>
        private void GenerateFilterHtml(List<BF_TB_REPORT_FILTER.Entity> filterList, out string html, out string values)
        {
            html = string.Empty;
            values = string.Empty;

            if (filterList == null || filterList.Count < 1)
            {
                return;
            }
            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            foreach (var filter in filterList)
            {
                Enums.FieldDataType dataType = (Enums.FieldDataType)filter.FIELD_DATA_TYPE;
                Enums.FormQueryType queryType = (Enums.FormQueryType)filter.FORM_QUERY_TYPE;

                string styleWidth = filter.INPUT_WIDTH > 0 ? " style='width:" + filter.INPUT_WIDTH + "px;' " : "";
                string defaultValue = string.IsNullOrWhiteSpace(filter.DEFAULT_VALUE) ? string.Empty : filter.DEFAULT_VALUE.Trim();
                defaultValue = BF_FORM.Instance.GetReadParam(defaultValue);
                if (defaultValue.StartsWith("{") && defaultValue.EndsWith("}"))
                {
                    sbValue.AppendLine("$('#" + filter.FIELD_NAME + "').val(" + defaultValue.Trim(new char[] { '{', '}' }) + ");");
                    defaultValue = string.Empty;
                }
                if (string.IsNullOrWhiteSpace(defaultValue) == false)
                {
                    defaultValue = Functions.Instance.ExecuteFunction(defaultValue);
                    defaultValue = SystemSession.TransParams(defaultValue);
                }
                sbHtml.AppendLine("<div class='layui-input-inline'>");
                sbHtml.AppendLine("<div class='layui-input-inline'>");
                sbHtml.AppendLine("<label class='layui-form-label-query'>" + filter.FILTER_NAME + "</label>");
                sbHtml.AppendLine("</div>");
                sbHtml.AppendLine("<div class='layui-input-inline'>");
                switch (dataType)
                {
                    case Enums.FieldDataType.数值:
                        switch (queryType)
                        {
                            case Enums.FormQueryType.下拉单选框:
                                Dictionary<string, string> options = BF_TB_REPORT_FILTER.GetSelectOptions(filter);
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.等于.GetHashCode() + "'/>");
                                sbHtml.AppendLine("<select lay-search filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "'>");
                                sbHtml.AppendLine("<option value='-999'>请选择</option>");
                                foreach (var kvp in options)
                                {
                                    sbHtml.AppendLine("<option value='" + kvp.Key + "' " + (kvp.Key == defaultValue ? "selected=''" : "") + ">" + kvp.Value + "</option>");
                                }
                                sbHtml.AppendLine("</select>");
                                break;
                            case Enums.FormQueryType.下拉树选择:
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.等于.GetHashCode() + "'/>");
                                sbHtml.AppendLine("<input lay-search type=\"text\" id='trea" + filter.ID + "' filter='" + filter.FIELD_NAME + "'  datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "'>");
                                sbHtml.AppendLine(BF_TB_REPORT_FILTER.GetTrea(filter));
                                break;
                            case Enums.FormQueryType.复选框:
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.等于.GetHashCode() + "'/>");
                                //sbHtml.AppendLine("<input type='checkbox' filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' " + (string.IsNullOrWhiteSpace(defaultValue) ? "" : "checked=''") + " title='是' />");
                                sbHtml.AppendLine("<input type='checkbox' filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' " + (string.IsNullOrWhiteSpace(defaultValue) ? "" : "checked=''") + " lay-text='是|否' lay-skin='switch' />");
                                break;
                            case Enums.FormQueryType.多值查找框:
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.集合.GetHashCode() + "'/>");
                                sbHtml.AppendLine("<textarea filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "' placeholder='支持多个值查询，分隔符为逗号或换行符'" + styleWidth + "></textarea>");
                                break;
                            default:
                                sbHtml.AppendLine("<div style='width:90px;'>");
                                sbHtml.AppendLine("<select lay-search id='OP_" + filter.FIELD_NAME + "'>");
                                foreach (Enums.FilterOperator etype in Enum.GetValues(typeof(Enums.FilterOperator)))
                                {
                                    if (etype.GetHashCode() < 10)
                                    {
                                        sbHtml.AppendLine("<option value='" + etype.GetHashCode() + "'>" + etype.ToString() + "</option>");
                                    }
                                }
                                sbHtml.AppendLine("</select>");
                                sbHtml.AppendLine("</div>");
                                sbHtml.AppendLine("</div>");

                                sbHtml.AppendLine("<div class='layui-input-inline'>");
                                sbHtml.AppendLine("<input type='text' filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "' placeholder='填写数字' class='layui-input search_input'" + styleWidth + "/>");
                                break;
                        }
                        break;
                    case Enums.FieldDataType.日期:
                        sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.等于.GetHashCode() + "'/>");
                        sbHtml.AppendLine("<input type='text' id='" + filter.FIELD_NAME + "' filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "'  placeholder='选择日期' class='layui-input search_input'" + styleWidth + "/>");
                        break;
                    default:
                        switch (queryType)
                        {
                            case Enums.FormQueryType.下拉单选框:
                                Dictionary<string, string> options = BF_TB_REPORT_FILTER.GetSelectOptions(filter);
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.等于.GetHashCode() + "'/>");
                                sbHtml.AppendLine("<select lay-search filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "' " + styleWidth + ">");
                                sbHtml.AppendLine("<option value=''>请选择</option>");
                                foreach (var kvp in options)
                                {
                                    sbHtml.AppendLine("<option value='" + kvp.Key + "' " + (kvp.Key == defaultValue ? "selected='selected'" : "") + ">" + kvp.Value + "</option>");
                                }
                                sbHtml.AppendLine("</select>");
                                break;
                            case Enums.FormQueryType.下拉树选择:
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.等于.GetHashCode() + "'/>");
                                sbHtml.AppendLine("<input lay-search type=\"text\" id='trea" + filter.ID + "' filter='" + filter.FIELD_NAME + "'  datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "' " + styleWidth + ">");
                                sbHtml.AppendLine(BF_TB_REPORT_FILTER.GetTrea(filter));
                                break;
                            case Enums.FormQueryType.多值查找框:
                                sbHtml.AppendLine("<input type='hidden' id='OP_" + filter.FIELD_NAME + "' value='" + Enums.FilterOperator.集合.GetHashCode() + "'/>");
                                sbHtml.AppendLine("<textarea filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' placeholder='支持多个值查询，分隔符为逗号或换行符' " + styleWidth + ">"+ defaultValue + "</textarea>");
                                break;
                            case Enums.FormQueryType.模糊检索框:
                                sbHtml.AppendLine("<div style='width:80px;'>");
                                sbHtml.AppendLine("<select id='OP_" + filter.FIELD_NAME + "'>");
                                sbHtml.AppendLine("<option value='" + Enums.FilterOperator.等于.GetHashCode() + "'>" + Enums.FilterOperator.等于.ToString() + "</option>");
                                sbHtml.AppendLine("<option value='" + Enums.FilterOperator.包含.GetHashCode() + "' selected=''>" + Enums.FilterOperator.包含.ToString() + "</option>");
                                sbHtml.AppendLine("</select>");
                                sbHtml.AppendLine("</div>");
                                sbHtml.AppendLine("</div>");

                                sbHtml.AppendLine("<div class='layui-input-inline'>");
                                sbHtml.AppendLine("<input type='text' filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "' class='layui-input search_input'" + styleWidth + "/>");
                                break;
                            default:
                                sbHtml.AppendLine("<div style='width:80px;'>");
                                sbHtml.AppendLine("<select lay-search id='OP_" + filter.FIELD_NAME + "'>");
                                sbHtml.AppendLine("<option value='" + Enums.FilterOperator.等于.GetHashCode() + "' selected=''>" + Enums.FilterOperator.等于.ToString() + "</option>");
                                sbHtml.AppendLine("<option value='" + Enums.FilterOperator.包含.GetHashCode() + "'>" + Enums.FilterOperator.包含.ToString() + "</option>");
                                sbHtml.AppendLine("</select>");
                                sbHtml.AppendLine("</div>");
                                sbHtml.AppendLine("</div>");

                                sbHtml.AppendLine("<div class='layui-input-inline'>");
                                sbHtml.AppendLine("<input type='text' filter='" + filter.FIELD_NAME + "' datatype='" + filter.FIELD_DATA_TYPE + "' value='" + defaultValue + "' class='layui-input search_input'" + styleWidth + "/>");
                                break;
                        }

                        break;
                }

                sbHtml.AppendLine("</div>");
                sbHtml.AppendLine("</div>");
            }
            html = sbHtml.ToString();
            values = sbValue.ToString();
        }

        /// <summary>
        /// 生成默认筛选项的HTML
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="defaultFilterList"></param>
        /// <returns></returns>
        private string GenerateDefaultFilterHtml(BF_TB_REPORT.Entity entity, List<BF_TB_REPORT_FILTER.Entity> defaultFilterList)
        {
            if (defaultFilterList == null || defaultFilterList.Count < 1)
            {
                return string.Empty;
            }

            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            sbHtml.AppendLine("<blockquote>");
            sbHtml.AppendLine("<div class='layui-inline'>");
            sbValue.AppendLine("<script type='text/javascript'>");
            sbValue.AppendLine("layui.use(['jquery','layer'], function () {");
            sbValue.AppendLine("$ = layui.jquery;");
            string html = string.Empty;
            string values = string.Empty;
            GenerateFilterHtml(defaultFilterList, out html, out values);
            sbHtml.AppendLine(html);
            sbValue.AppendLine(values);

            sbHtml.AppendLine("</div>");
            sbHtml.AppendLine("</blockquote>");
            sbValue.AppendLine("});");
            sbValue.AppendLine("</script>");
            return sbHtml.ToString() + sbValue.ToString();
        }

        /// <summary>
        /// 生成高级筛选的HTML
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="advancedFilterList"></param>
        /// <returns></returns>
        private string GenerateAdvancedFilterHtml(BF_TB_REPORT.Entity entity, List<BF_TB_REPORT_FILTER.Entity> advancedFilterList)
        {
            if (advancedFilterList == null || advancedFilterList.Count < 0)
            {
                return string.Empty;
            }

            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            sbHtml.Append("<blockquote>");

            sbValue.AppendLine("<script type='text/javascript'>");
            sbValue.AppendLine("layui.use(['jquery','layer'], function () {");
            sbValue.AppendLine("$ = layui.jquery;");
            string html = string.Empty;
            string values = string.Empty;
            GenerateFilterHtml(advancedFilterList, out html, out values);
            sbHtml.AppendLine(html);
            sbValue.AppendLine(values);
            sbHtml.Append("</blockquote>");
            sbValue.AppendLine("});");
            sbValue.AppendLine("</script>");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("advancedFilterHtml=\"" + sbHtml.ToString() + "\";");
            sb.AppendLine("</script>");

            return sbHtml.ToString() + sbValue.ToString();
        }

        /// <summary>
        /// 日期类型的筛选项
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string GenerateDatetimeFilterHtml(BF_TB_REPORT.Entity entity, List<BF_TB_REPORT_FILTER.Entity> filterList)
        {
            if (filterList == null || filterList.Count < 1)
            {
                return string.Empty;
            }

            int i = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("layui.use('laydate', function () {");
            sb.AppendLine("var laydate = layui.laydate;");
            foreach (var filter in filterList)
            {
                if (filter.FIELD_DATA_TYPE != Enums.FieldDataType.日期.GetHashCode())
                {
                    continue;
                }
                i++;
                sb.AppendLine("laydate.render({");
                sb.AppendLine("elem: '#" + filter.FIELD_NAME + "'");
                if (filter.FORM_QUERY_TYPE == Enums.FormQueryType.日期范围框.GetHashCode())
                {
                    sb.AppendLine(", range: true");
                }
                sb.AppendLine("});");
            }
            sb.AppendLine("});");
            sb.AppendLine("</script>");
            if (i < 1)
            {
                return string.Empty;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成顶部按钮的HTML
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="topEventList"></param>
        /// <returns></returns>
        private string GenerateTopButtonHtml(BF_TB_REPORT.Entity entity, List<BF_TB_REPORT_EVENT.Entity> topEventList, bool isShowAdvancedFilter)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<blockquote>");
            sb.AppendLine("<div class='layui-inline' style='float:left'>");
            sb.AppendLine("<button class='layui-btn search_btn layui-btn-small' onclick='Query();'><i class='layui-icon  layui-icon-search layui-icon-small'></i>查询</button>");
            if (isShowAdvancedFilter == true)
            {
                //sb.AppendLine("<button class='layui-btn layui-btn-small' onclick='senior_find();'><i class='layui-icon layui-icon-query1 layui-icon-small'></i>高级筛选</button>");
                sb.AppendLine("<button class='layui-btn layui-btn-small' onclick='ShowAdvancedFilter();'><i class='layui-icon layui-icon-query1 layui-icon-small'></i>高级筛选</button>");
            }
            if (entity.IS_SHOW_EXPORT == 1)
            {
                sb.AppendLine("<button class='layui-btn layui-btn-small' onclick='ExportExcel();'><i class='layui-icon layui-icon-exl layui-icon-small'></i>导出</button>");
            }
            if (entity.IS_SHOW_DEBUG == 1)
            {
                sb.AppendLine("<button class='layui-btn layui-btn-warm layui-btn-small' onclick='ShowDebug();'><i class='layui-icon layui-icon-daima layui-icon-small'></i>调试</button>");
            }
            if (topEventList != null && topEventList.Count > 0)
            {
                if (topEventList != null && topEventList.Count > 0)
                {
                    foreach (var te in topEventList)
                    {
                        string fun = "";
                        Enums.RequestMode reqestMode = Enums.RequestMode.普通提示框;
                        try
                        {
                            reqestMode = (Enums.RequestMode)te.REQUEST_MODE;
                        }
                        catch
                        {
                            fun = ("alert('错误的请求类型:" + te.REQUEST_MODE + "');");
                        }

                        #region 继承父级的URL参数                       
                        te.REQUEST_URL = BF_FORM.Instance.GetReadParam(te.REQUEST_URL);
                        #endregion

                        switch (reqestMode)
                        {
                            case Enums.RequestMode.Ajax异步GET:
                                fun = "AjaxGet('" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.Ajax异步POST:
                                fun = "AjaxPost('" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.JS内部调用:
                                fun = te.REQUEST_URL;
                                break;
                            case Enums.RequestMode.右下提示框:
                                fun = "OpenRDPromptWindow('" + te.BUTTON_TEXT + "', " + te.SHOW_WIDTH + ", " + te.SHOW_HEIGHT + ", '" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.当前页弹出框:
                                fun = "OpenWindow('" + te.BUTTON_TEXT + "', " + te.SHOW_WIDTH + ", " + te.SHOW_HEIGHT + ", '" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.当前页跳转:
                                fun = "teditect('" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.普通提示框:
                                fun = "OpenPromptWindow('" + te.BUTTON_TEXT + "', " + te.SHOW_WIDTH + ", " + te.SHOW_HEIGHT + ", '" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.框架新窗口:
                                fun = "OpenFrameWindow('" + te.BUTTON_TEXT + "', '" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.浏览器新窗口:
                                fun = "OpenBrowserWindow('" + te.REQUEST_URL + "');";
                                break;
                            case Enums.RequestMode.顶级弹出框:
                                fun = "OpenTopWindow('" + te.BUTTON_TEXT + "', " + te.SHOW_WIDTH + ", " + te.SHOW_HEIGHT + ", '" + te.REQUEST_URL + "');";
                                break;
                        }
                        sb.AppendLine(GetEventBut(te.BUTTON_STYLE, te.BUTTON_ICON, te.EVENT_NAME, te.BUTTON_TEXT, "onclick =\"" + fun + "\"", "layui-btn"));//顶部按钮
                                                                                                                                                            // sb.AppendLine("<button class='layui-btn linksAdd_btn' onclick=\"" + fun + "\" ><i class='layui-icon layui-icon-help'></i>" + te.BUTTON_TEXT + "</button>");
                    }
                }
            }
            sb.AppendLine("</div>");
            sb.AppendLine("</blockquote>");
            return sb.ToString();
        }

        /// <summary>
        /// 生成顶部代码的HTML
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GenerateTopCodeHtml(BF_TB_REPORT.Entity entity)
        {
            return "<div id='divTopCode'>\r\n" + SystemSession.TransParams(entity.TOP_CODE) + "\r\n</div>";
        }

        /// <summary>
        /// 表格渲染代码
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="rowEventList"></param>
        /// <param name="queryString"></param>
        /// <param name="footHeight"></param>
        /// <returns></returns>
        private string GenerateTableRender(BF_TB_REPORT.Entity entity, List<BF_TB_REPORT_EVENT.Entity> rowEventList, string queryString, int footHeight = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var tableObj = table.render({");
            sb.AppendLine("elem: '#reporttable'");
            sb.AppendLine(", id: 'reporttable'");
            sb.AppendLine(", size: 'sm'");
            //sb.AppendLine(", height: 'full-135'");
            sb.AppendLine(", page:{limit:20, layout:['refresh', 'prev', 'page', 'next', 'skip', 'limit', 'count']}");
            //sb.AppendLine(", limit: 20");
            sb.AppendLine(", url: '" + ApplicationPath + "/AfTableReport/Show?" + queryString + "'");
            sb.AppendLine(", method: 'post'");
            sb.AppendLine(", where: {");
            sb.AppendLine("  count:rowscount");
            sb.AppendLine(", input:inputstring");
            sb.AppendLine(", where:wherestring");
            sb.AppendLine("}");
            sb.AppendLine(", even: true");
            //字段
            sb.AppendLine(", cols: [[");
            int i = 0;
            if (entity.IS_SHOW_CHECKBOX == 1)
            {
                sb.AppendLine("{type:'checkbox', fixed:'left'}");
                i++;
            }
            if (string.IsNullOrWhiteSpace(entity.SHOW_FIELDS) == true)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                List<BF_TB_REPORT.QueryFilterItem> list = new List<BF_TB_REPORT.QueryFilterItem>();
                string sql = string.Empty;
                List<object> paramList = new List<object>();
                int row = 1;
                DataTable dt = BF_TB_REPORT.QueryTable(entity, 1, 1, ref row, out sql, out paramList, queryString, entity.DEFAULT_INPUT_VALUES, "");

                foreach (DataColumn col in dt.Columns)
                {
                    string name = col.ColumnName;
                    int width = 80;
                    if (name == "ID")
                    {
                        width = 60;
                    }
                    else if (col.DataType == typeof(DateTime))
                    {
                        width = 135;
                    }
                    else if (col.DataType == typeof(string))
                    {
                        width = 120;
                    }
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                    i++;
                    if (i == dt.Columns.Count)
                    {
                        sb.AppendLine("{field:'" + name + "', title: '" + name + "', minWidth:" + width + ", sort: true}");
                    }
                    else
                    {
                        sb.AppendLine("{field:'" + name + "', title: '" + name + "', width:" + width + ", sort: true}");
                    }
                }
            }
            else
            {
                List<Models.FW.TbrShowFieldModel> fieldList = DeserializeObject<List<Models.FW.TbrShowFieldModel>>(entity.SHOW_FIELDS);
                foreach (Models.FW.TbrShowFieldModel col in fieldList)
                {
                    if (col.IS_SHOW == 1)
                    {
                        string fix = col.IS_FIXED == 1 ? ", fixed:'left'" : "";
                        string sort = col.IS_SORT == 1 ? ", sort:true" : "";
                        if (i > 0)
                        {
                            sb.Append(",");
                        }
                        i++;
                        if (i == fieldList.Count)
                        {
                            sb.AppendLine("{field:'" + col.EN_NAME + "', title: '" + col.CN_NAME + "', minWidth:" + col.SHOW_WIDTH + fix + sort + "}");
                        }
                        else
                        {
                            sb.AppendLine("{field:'" + col.EN_NAME + "', title: '" + col.CN_NAME + "', width:" + col.SHOW_WIDTH + fix + sort + "}");
                        }
                    }
                }
            }
            if (rowEventList != null && rowEventList.Count > 0)
            {
                sb.AppendLine(",{fixed: 'right', title: '操作', width:" + (rowEventList.Count * 50 + 30) + ", align:'center', toolbar: '#operation'}");
            }

            sb.AppendLine("]]");
            //加载数据完毕后的回调
            sb.AppendLine(", done: function(res, curr, count){");
            sb.AppendLine("pageindex = curr;");
            sb.AppendLine("rowscount = count;");
            sb.AppendLine("executeSQL = res.sql;");
            sb.AppendLine("executeParam = res.sqlparam;");
            sb.AppendLine("$('.layui-table-body').height($(window).height() - $('#divTop').height() - " + (96 + footHeight) + ");");
            sb.AppendLine("layer.close(loading);");
            sb.AppendLine("}");
            sb.AppendLine("});");

            return sb.ToString();
        }

        /// <summary>
        /// 生成表格事件的HTML
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="rowEventList"></param>
        /// <returns></returns>
        private string GenerateEventHtml(BF_TB_REPORT.Entity entity, List<BF_TB_REPORT_EVENT.Entity> rowEventList)
        {
            if (rowEventList == null || rowEventList.Count < 1)
            {
                return string.Empty;
            }

            //正则，匹配URL中的字段
            Regex regField = new Regex(@"\[(?<fn>.+?)\]");

            //模板
            StringBuilder sbTemplate = new StringBuilder();
            sbTemplate.AppendLine("<script type='text/html' id='operation'>");

            //事件处理
            StringBuilder sbEvent = new StringBuilder();
            sbEvent.AppendLine("<script type='text/javascript'>");
            sbEvent.AppendLine("layui.use(['table', 'jquery'], function () {");
            sbEvent.AppendLine("var table = layui.table, $ = layui.jquery;");
            sbEvent.AppendLine("table.on('tool(reporttable)', function (obj) {");
            sbEvent.AppendLine("var data = obj.data, layEvent = obj.event;");
            int i = 0;
            foreach (var re in rowEventList)
            {
                string url = string.IsNullOrWhiteSpace(re.REQUEST_URL) ? string.Empty : re.REQUEST_URL;
                Match match = regField.Match(url);
                while (match.Success == true)
                {
                    string field = match.Result("${fn}");
                    url = url.Replace("[" + field + "]", "' + data." + field + " + '");
                    match = match.NextMatch();
                }

                sbTemplate.AppendLine(GetEventBut(re.BUTTON_STYLE, re.BUTTON_ICON, re.EVENT_NAME, re.BUTTON_TEXT, ""));//行事件按钮样式
                if (i > 0)
                {
                    sbEvent.Append("else ");
                }
                sbEvent.AppendLine("if (layEvent === '" + re.EVENT_NAME + "') {");
                Enums.RequestMode reqestMode = Enums.RequestMode.普通提示框;
                try
                {
                    reqestMode = (Enums.RequestMode)re.REQUEST_MODE;
                }
                catch
                {
                    sbEvent.AppendLine("alert('错误的请求类型:" + re.REQUEST_MODE + "');");
                }
                switch (reqestMode)
                {
                    case Enums.RequestMode.Ajax异步GET:
                        sbEvent.AppendLine("AjaxGet('" + url + "');");
                        break;
                    case Enums.RequestMode.Ajax异步POST:
                        if (re.EVENT_NAME.Contains("删除") || re.EVENT_NAME.ToLower().Contains("del"))
                        {
                            sbEvent.AppendLine("layer.confirm('确定要 " + re.BUTTON_TEXT + " 这条记录吗？', function (index) {");
                            sbEvent.AppendLine("AjaxPost('" + url + "');");
                            sbEvent.AppendLine("});");
                        }
                        else
                        {
                            sbEvent.AppendLine("AjaxPost('" + url + "');");
                        }
                        break;
                    case Enums.RequestMode.JS内部调用:
                        sbEvent.AppendLine(re.REQUEST_URL);
                        break;
                    case Enums.RequestMode.右下提示框:
                        sbEvent.AppendLine("OpenRDPromptWindow('" + re.BUTTON_TEXT + "', " + re.SHOW_WIDTH + ", " + re.SHOW_HEIGHT + ", '" + url + "');");
                        break;
                    case Enums.RequestMode.当前页弹出框:
                        sbEvent.AppendLine("OpenWindow('" + re.BUTTON_TEXT + "', " + re.SHOW_WIDTH + ", " + re.SHOW_HEIGHT + ", '" + url + "');");
                        break;
                    case Enums.RequestMode.当前页跳转:
                        sbEvent.AppendLine("Redirect('" + re.REQUEST_URL + "');");
                        break;
                    case Enums.RequestMode.普通提示框:
                        sbEvent.AppendLine("OpenPromptWindow('" + re.BUTTON_TEXT + "', " + re.SHOW_WIDTH + ", " + re.SHOW_HEIGHT + ", '" + url + "');");
                        break;
                    case Enums.RequestMode.框架新窗口:
                        sbEvent.AppendLine("OpenFrameWindow('" + re.BUTTON_TEXT + "', '" + url + "');");
                        break;
                    case Enums.RequestMode.浏览器新窗口:
                        sbEvent.AppendLine("OpenBrowserWindow('" + url + "');");
                        break;
                    case Enums.RequestMode.顶级弹出框:
                        sbEvent.AppendLine("OpenTopWindow('" + re.BUTTON_TEXT + "', " + re.SHOW_WIDTH + ", " + re.SHOW_HEIGHT + ", '" + url + "');");
                        break;
                }

                sbEvent.AppendLine("}");
                i++;
            }
            sbTemplate.AppendLine("</script>");
            sbEvent.AppendLine("});");
            sbEvent.AppendLine("});");
            sbEvent.AppendLine("</script>");
            return sbTemplate.ToString() + sbEvent.ToString();
        }

        /// <summary>
        /// 生成底部代码的HTML
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GenerateBottomCodeHtml(BF_TB_REPORT.Entity entity)
        {
            return SystemSession.TransParams(entity.BOTTOM_CODE);
        }

        #region 生成事件按钮样式
        /// <summary>
        /// 生成事件按钮样式
        /// </summary>
        /// <param name="buttonStyle">按钮样式</param>
        /// <param name="buttonIcon">图标样式</param>
        /// <param name="enentName">字段名称</param>
        /// <param name="butName">按钮名称</param>
        /// <param name="clickFun">按钮事件</param>
        /// <param name="ButSize">按钮大小</param>
        /// <returns></returns>
        private string GetEventBut(string buttonStyle, string buttonIcon, string enentName, string butName, string clickFun = "", string ButDefaultClass = "layui-badge")
        {
            string reResult = "<span " + clickFun + " class='layui-bg-green " + ButDefaultClass + "' lay-event='" + enentName + "'>" + butName + "</span>";
            if (string.IsNullOrWhiteSpace(buttonStyle) && string.IsNullOrWhiteSpace(buttonIcon))
                return reResult;

            if (string.IsNullOrWhiteSpace(buttonStyle))
                reResult = "<i " + clickFun + " class='layui-icon " + buttonIcon + "' lay-event='" + enentName + "'></i>";
            else
                reResult = "<span " + clickFun + " class='" + buttonStyle + " " + ButDefaultClass + "' lay-event='" + enentName + "'><i class='layui-icon " + buttonIcon + "'></i>" + butName + "</span>";

            return reResult;
        }
        #endregion

        #endregion

        #region 导出数据到Excel

        /// <summary>
        /// 导出到EXCEL文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input">用户自定义输入项</param>
        /// <param name="where">筛选项JSON串</param>
        /// <returns></returns>
        public ActionResult ExportExcel(int id, string input, string where = "")
        {
            string sql = string.Empty;
            List<object> paramList = new List<object>();
            try
            {
                var entity = BF_TB_REPORT.Instance.GetEntityByKey<BF_TB_REPORT.Entity>(id);
                string name = entity == null ? id.ToString() : entity.NAME;
                //string queryString = Request.QueryString.ToString();
                string queryString = HttpUtility.UrlDecode(Request.QueryString.ToString());
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_{0}_{1}.xlsx", name, DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                int rowsCount = -1;
                DataTable dt = BF_TB_REPORT.QueryTable(id, 0, 0, ref rowsCount, out sql, out paramList, queryString, input, where);
                //设置中文字段
                BF_TB_REPORT.Instance.SetTableCaption(id, dt);

                //加密
                Encrypt(entity, ref dt);

                return ExportExcel(filename, dt);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "导出报表[" + id + "]到Excel出错:" + ex.ToString());
                return ShowAlert("导出报表出错，详见运行日志：" + ex.Message);
            }
        }

        #endregion

        #region 数据加密

        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fieldList"></param>
        private void Encrypt(BF_TB_REPORT.Entity entity, ref DataTable dt)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.SHOW_FIELDS) || dt == null || dt.Rows.Count < 1)
            {
                return;
            }

            //字段配置
            List<Models.FW.TbrShowFieldModel> fieldList = DeserializeObject<List<Models.FW.TbrShowFieldModel>>(entity.SHOW_FIELDS);

            if (fieldList == null || fieldList.Count < 1)
            {
                return;
            }

            List<string> cols = new List<string>();
            foreach (var field in fieldList)
            {
                if (field.IS_SHOW == 1 && field.IS_ENCRYPT == 1)
                {
                    cols.Add(field.EN_NAME);
                }
            }
            if (cols.Count < 1)
            {
                return;
            }

            foreach (DataRow dr in dt.Rows)
            {
                foreach (string col in cols)
                {
                    try
                    {
                        dr[col] = EncryptContent(dr[col]);
                    }
                    catch (Exception e)
                    {
                        BLog.Write(BLog.LogLevel.WARN, "查询表格报表加密显示字段" + col + "出错" + e.ToString());
                    }

                }
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        private static string EncryptContent(object content)
        {
            if (content == null || content == DBNull.Value)
            {
                return string.Empty;
            }
            string s = Convert.ToString(content).Trim();
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }
            if (s.Length <= 2)
            {
                s = s.Replace(s[0], '*');
            }
            else if (s.Length <= 5)
            {
                for (int i = 1; i < s.Length - 1; i++)
                {
                    s = s.Replace(s[i], '*');
                }
            }
            else if (s.Length == 11)
            {
                s = s.Replace(s.Substring(3, 4), "****");
            }
            else if (s.Length == 15)
            {
                s = s.Replace(s.Substring(6, 6), "******");
            }
            else if (s.Length == 18)
            {
                s = s.Replace(s.Substring(6, 8), "********");
            }
            else
            {
                for (int i = s.Length / 2; i < s.Length - 1; i++)
                {
                    s = s.Replace(s[i], '*');
                }
            }

            return s;
        }

        #endregion
    }
}