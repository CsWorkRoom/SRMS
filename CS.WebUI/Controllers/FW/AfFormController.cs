using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static CS.BLL.FW.BF_FORM.FieldInfo;
using static CS.Common.FW.Enums;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 表单编辑
    /// </summary>
    public class AfFormController : ABaseController
    {
        // GET: AfForm
        public ActionResult Index()
        {
            GetTableMode();
            return View();
        }

        #region 获取基础信息列表
        /// <summary>
        /// 获取基础信息列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            List<object> param = new List<object>();

            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND f.NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            DataTable data = BLL.FW.BF_FORM.Instance.GetDataTable(limit, page, where, param);
            return ToJsonString(data, data.Rows.Count);
        }
        #endregion

        #region 加载编辑及新增
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            ViewBag.xlId = (int)Enums.FormInputType.下拉单选框;
            ViewBag.duofx = (int)Enums.FormInputType.多个复选框;
            ViewBag.treexl = (int)Enums.FormInputType.下拉树选择;

            GetTableMode();//创建表模式
            GetFieldDataType();//字段类型
            GetInputType();//输入框类型
            GetSelectType();//数据来源
            BF_FORM.Entity entity = new BF_FORM.Entity();
            entity.DB_ID = -1;

            if (id > 0)
            {
                entity = BF_FORM.Instance.GetEntityByKey<BF_FORM.Entity>(id);
                if (entity == null)
                {
                    return ShowAlert("配置项不存在");
                }
            }
            //检查表字段有没有变动
            entity = GetFormFields(entity);
            return View(entity);
        }
        #endregion

        #region 检查表字段有没有变动
        private BF_FORM.Entity GetFormFields(BF_FORM.Entity entity)
        {
            int dbid = entity.DB_ID;//得到数据库ID
            string tableName = entity.TABLE_NAME;
            if (dbid < 0 || string.IsNullOrWhiteSpace(tableName))
            {
                return entity;
            }
            List<BF_FIELD.Entity> list = BF_DATABASE.Instance.GetFieldsList(dbid, tableName);//最新表的情况
            List<BF_FORM.FieldInfo> fieldsList = JsonConvert.DeserializeObject<List<BF_FORM.FieldInfo>>(entity.FIELDS);//当前表的情况
            if (list == null || list.Count <= 0)
                return entity;

            List<BF_FIELD.Entity> isNotList = new List<BF_FIELD.Entity>();//记录变化的字段
            foreach (BF_FIELD.Entity item in list)
            {
                bool isSelect = false;//记录是否查到字段
                if (fieldsList == null || fieldsList.Count <= 0)
                    return entity;

                foreach (BF_FORM.FieldInfo info in fieldsList)
                {
                    if (item.EN_NAME == info.EN_NAME)
                    {
                        isSelect = true;
                        break;
                    }
                }
                if (isSelect == false)
                {
                    isNotList.Add(item);
                }
            }

            //将变化的字段添加到当前字段中
            if (isNotList.Count <= 0)
                return entity;

            //添加变化的字段
            foreach (BF_FIELD.Entity info in isNotList)
            {
                BF_FORM.FieldInfo field = new BF_FORM.FieldInfo();
                field.CN_NAME = info.CN_NAME;
                field.DEFAULT = "";
                field.EN_NAME = info.EN_NAME;
                field.FIELD_DATA_TYPE = info.FIELD_DATA_TYPE;
                field.INPUT_TYPE = (int)FormInputType.普通文字框;
                field.IS_AUTO_INCREMENT = 0;
                field.IS_INSERT = 0;
                field.IS_KEY_FIELD = 0;
                field.IS_NOT_NULL = 0;
                field.IS_READONLY = 0;
                field.IS_UNIQUE = 0;
                field.IS_UPDATE = 0;
                field.ORDER_NUM = 0;
                fieldsList.Add(field);
            }
            entity.FIELDS = JSON.DecodeToStr<List<BF_FORM.FieldInfo>>(fieldsList);
            return entity;
        }
        #endregion

        #region 加载编辑及新增
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DownModel(int index = 0)
        {
            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            ViewBag.xlId = (int)Enums.FormInputType.下拉单选框;
            ViewBag.duofx = (int)Enums.FormInputType.多个复选框;
            ViewBag.treexl = (int)Enums.FormInputType.下拉树选择;

            if (index <= 0)
            {
                ViewBag.Message = "参数有误！请联系管理员。";
                return View();
            }

            GetSelectType();//数据来源
            return View();
        }
        #endregion

        #region 编辑提交
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(BF_FORM.Entity entity)
        {
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "配置项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                entity.UPDATE_UID = SystemSession.UserID;
                entity.UPDATE_TIME = DateTime.Now;
                if (entity.ID == 0)
                {
                    entity.IS_ENABLE = 1;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    i = BF_FORM.Instance.Add(entity);
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("NAME", entity.NAME);
                    dic.Add("DB_ID", entity.DB_ID);
                    dic.Add("TABLE_NAME", entity.TABLE_NAME);
                    dic.Add("CREATE_TABLE_MODE", entity.CREATE_TABLE_MODE);
                    dic.Add("IS_ALLOW_DELETE", entity.IS_ALLOW_DELETE);
                    dic.Add("FIELDS", entity.FIELDS);
                    dic.Add("REMARK", entity.REMARK);
                    dic.Add("JS_CODE", entity.JS_CODE);
                    dic.Add("UPDATE_UID", entity.UPDATE_UID);
                    dic.Add("UPDATE_TIME", entity.UPDATE_TIME);
                    i = BF_FORM.Instance.UpdateByKey(dic, entity.ID);
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
                BLog.Write(BLog.LogLevel.WARN, "编辑导入配置出错：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "删除失败！";
            result.IsSuccess = BLL.FW.BF_FORM.Instance.DeleteByKey(id) > 0;
            if (result.IsSuccess)
                result.Message = "删除完成。";
            else
                result.Message = "程序异常。删除失败！";

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 导出文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string name = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_表单编辑_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                List<object> param = new List<object>();

                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                var dt = BF_FORM.Instance.GetDataTable(0, 0, where, param);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["NAME"].Caption = "类型名称";
                dt.Columns["PNAME"].Caption = "上层类型";
                dt.Columns["REMARK"].Caption = "备注";
                dt.Columns["CREATENAME"].Caption = "创建者";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                dt.Columns["UPDATENAME"].Caption = "最后更新者";
                dt.Columns["UPDATE_TIME"].Caption = "最后更新时间";

                Library.Export.ExcelFile export = new Library.Export.ExcelFile(path);
                string fullName = export.ToExcel(dt);
                if (string.IsNullOrWhiteSpace(fullName) == true)
                {
                    return ShowAlert("未生成Excel文件");
                }
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                System.Web.HttpContext.Current.Response.WriteFile(fullName);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
                //删除文件
                export.Delete(fullName);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "导出默认报表[DB]到Excel出错:" + ex.ToString());
                return ShowAlert("导出数据到Excel出现未知错误：" + ex.Message);
            }
            return ShowAlert("导出成功");
        }
        #endregion

        #region 建表模式
        private void GetTableMode()
        {
            IDictionary dic = new Dictionary<int, string>();
            foreach (Enums.CreateTableMode source in Enum.GetValues(typeof(Common.FW.Enums.CreateTableMode)))
                dic.Add((int)source, source.ToString());
            ViewBag.tmpTMode = dic;
        }
        #endregion

        #region 数据类型
        private void GetFieldDataType()
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();
            foreach (Enums.FieldDataType mode in Enum.GetValues(typeof(Enums.FieldDataType)))
                dic.Add(mode.ToString(), (int)mode);
            ViewBag.tempDataType = dic;
        }
        #endregion

        #region 数据输入框类型
        private void GetInputType()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (Enums.FormInputType mode in Enum.GetValues(typeof(Enums.FormInputType)))
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Key", (int)mode);
                dic.Add("Value", mode.ToString());
                list.Add(dic);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("  tempInputType =");
            sb.AppendLine(SerializeObject(list) + ";");
            sb.AppendLine("</script>");
            ViewBag.tempInputType = sb.ToString();

            //部门层级
            Dictionary<string, object> dataList = new Dictionary<string, object>();
            foreach (Enums.FieldDataType dataType in Enum.GetValues(typeof(Enums.FieldDataType)))
            {
                dataList.Add(dataType.ToString(), (int)dataType);
            }
            ViewBag.DataType = dataList;
        }
        #endregion

        #region 数据输入框类型
        private void GetSelectType()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (Enums.FormSelectType mode in Enum.GetValues(typeof(Enums.FormSelectType)))
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Key", (int)mode);
                dic.Add("Value", mode.ToString());
                list.Add(dic);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("  tempSelectType =");
            sb.AppendLine(SerializeObject(list) + ";");
            sb.AppendLine("</script>");
            ViewBag.tempSelectType = sb.ToString();
        }
        #endregion

        #region 模板操作
        #region  生成表单模板
        /// <summary>
        /// 表单模板（如果选择了修改和删除必须要传rowKey）
        /// </summary>
        /// <param name="formId">表单ID</param>
        /// <param name="op">功能选择（0:添加;1:修改;2:删除）</param>
        /// <param name="rowKey">主键值</param>
        /// <returns></returns>fromId
        public ActionResult Template(int formId = 0, int op = 0, string rowKey = "", string yyyy = "", string yyyymm = "", string yyyymmdd = "")
        {
            #region 验证数据参数
            if (formId <= 0 || op >= 2)
            {
                ViewBag.Message = "对不起！表单没有找到formId或op参数，请联系管理员。";
                //  ShowAlert(ViewBag.Message);//跳转至错误页
                return View();
            }

            if (op != 0)
            {
                if (rowKey == null || rowKey.Trim() == "")
                {
                    ViewBag.Message = "对不起！表单没有找rowKey参数值，请联系管理员。";
                    return View();
                }
                rowKey = rowKey.Trim();
            }
            #endregion

            #region 查询基本信息
            BF_FORM.Entity entity = BF_FORM.Instance.GetEntityByKey<BF_FORM.Entity>(formId);
            string tableName = entity.TABLE_NAME;//表名
            List<BF_FORM.FieldInfo> FieldList = JsonConvert.DeserializeObject<List<BF_FORM.FieldInfo>>(entity.FIELDS);
            if (FieldList.Count <= 0)
            {
                ViewBag.Message = "对不起！表对应的字段有误，请联系管理员。";
                return View();
            }

            DataRow dataRow = null;
            #region 查询要编辑的数据
            if (op == 1)
            {
                string strMessage = "";
                entity.TABLE_NAME = BF_FORM.Instance.GetTableName(entity, yyyy, yyyymm, yyyymmdd, ref strMessage);//得到动态表名
                if (string.IsNullOrEmpty(strMessage) == false || string.IsNullOrEmpty(entity.TABLE_NAME))
                {
                    BLog.Write(BLog.LogLevel.WARN, strMessage);
                    ViewBag.Message = strMessage;
                    // ShowAlert(strMessage);//跳转至错误页
                    return View();
                }
                DataTable data = BF_FORM.Instance.GetData(entity.DB_ID, entity.TABLE_NAME, "*", FieldList, rowKey);
                if (data != null && data.Rows.Count > 0)
                    dataRow = data.Rows[0];
            }
            #endregion
            #endregion
            string strHtml = BF_FORM.Instance.GenerateTemplate(op, FieldList, dataRow);//生成模板
            if (string.IsNullOrEmpty(strHtml))
            {
                ViewBag.Message = "模板生成出现异常！请联系管理";
                return View();
            }
            ViewBag.js_Code = (string.IsNullOrWhiteSpace(entity.JS_CODE) ? "" : "<div class=\"layui-form-item\">" + entity.JS_CODE + "</div>");
            ViewBag.formHtml = strHtml;//生成的表单            
            return View();
        }
        #endregion

        #region 编辑提交
        /// <summary>
        /// 增删改提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Template(FormCollection collection)
        {
            #region 得到参数验证数据
            string formId = Request["formId"];
            string op = Request["op"];
            string rowKey = Request["rowKey"];
            string yyyy = Request["YYYY"];
            string yyyymm = Request["YYYYMM"];
            string yyyymmdd = Request["YYYYMMDD"];

            JsonResultData result = new JsonResultData();
            if (string.IsNullOrEmpty(formId) || string.IsNullOrEmpty(op))
            {
                result.IsSuccess = false;
                result.Message = "对不起！表单没有找到formId或op参数，请联系管理员。";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (op != "0" && string.IsNullOrEmpty(rowKey) == true)
            {
                result.IsSuccess = false;
                result.Message = "对不起！表单没有找rowKey参数值，请联系管理员。";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 提取配置信息
            BF_FORM.Entity entity = BF_FORM.Instance.GetEntityByKey<BF_FORM.Entity>(formId);

            List<BF_FORM.FieldInfo> FieldList = JsonConvert.DeserializeObject<List<BF_FORM.FieldInfo>>(entity.FIELDS);
            if (FieldList.Count <= 0)
            {
                result.IsSuccess = false;
                result.Message = "对不起！表对应的字段有误，请联系管理员。";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion


            try
            {
                List<string> strFields = new List<string>();
                List<object> strValues = new List<object>();
                string strMessage = "";
                bool bitResult = false;
                if (op == "0")//数据添加
                {
                    entity.TABLE_NAME = BF_FORM.Instance.CreateTable(entity, FieldList);//动态创建数据库表
                    if (entity.TABLE_NAME == "")
                    {
                        result.IsSuccess = false;
                        result.Message = "对不起！数据库表有误，请联系管理员。";
                        BLog.Write(BLog.LogLevel.WARN, "动态创建数据库表出现异常：表为空了");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    #region 数据添加
                    foreach (BF_FORM.FieldInfo item in FieldList)
                    {
                        string val = "";
                        if (item.IS_INSERT == 1)
                        {

                            if (item.INPUT_TYPE == (int)Enums.FormInputType.不显示字段)
                            {
                                //添加默认值
                                string strDefault = item.DEFAULT == null ? "" : item.DEFAULT.Trim();
                                if (strDefault != "")
                                    val = BF_FORM.Instance.GetReadParam(strDefault); //将函数转为具体的值                                
                            }
                            else
                            {
                                val = collection[item.EN_NAME] == null ? "" : collection[item.EN_NAME].ToString();
                                if (string.IsNullOrWhiteSpace(val) && item.INPUT_TYPE == (int)Enums.FormInputType.单个复选框)//针对单个复选框由于只存在是或否的关系所以如果不选择时，有默认值，就按默认值走，如果没有就为0
                                {
                                    val = "0";
                                }
                            }

                            #region 数据有效性验证
                            string errorMessage = BF_FORM.Instance.ValidateField(item, val, rowKey, entity.DB_ID, entity.TABLE_NAME, FieldList);
                            if (string.IsNullOrWhiteSpace(errorMessage) == false)
                            {
                                result.IsSuccess = false;
                                result.Message = errorMessage;
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            #endregion

                            if (string.IsNullOrWhiteSpace(val) == false)
                            {
                                strFields.Add(item.EN_NAME);
                                switch (item.FIELD_DATA_TYPE)
                                {
                                    case (int)Enums.FieldDataType.数值:
                                        strValues.Add(Convert.ToDecimal(val));
                                        break;
                                    case (int)Enums.FieldDataType.日期:
                                        strValues.Add(Convert.ToDateTime(val));
                                        break;
                                    default:
                                        strValues.Add(val);
                                        break;
                                }
                            }
                        }
                        else if (item.IS_KEY_FIELD == 1 && item.IS_AUTO_INCREMENT == 0)
                        {
                            strFields.Add(item.EN_NAME);
                            strValues.Add(GetNextValueFromSeqDef(entity.TABLE_NAME));
                        }
                    }
                    //entity.TABLE_NAME = BF_FORM.Instance.CreateTable(entity, FieldList);//动态创建数据库表
                  
                    bitResult = BF_FORM.Instance.insertData(entity.DB_ID, entity.TABLE_NAME, strFields, strValues);
                    #endregion
                }
                else if (op == "1")//数据更新
                {
                    #region 数据更新
                    strFields.Clear();
                    strValues.Clear();
                    foreach (BF_FORM.FieldInfo item in FieldList)
                    {
                        if (item.IS_UPDATE == 1)
                        {
                            strFields.Add(item.EN_NAME + "=?");
                            string val = "";
                            if (item.INPUT_TYPE == (int)Enums.FormInputType.不显示字段)
                            {
                                //添加默认值
                                string strDefault = item.DEFAULT == null ? "" : item.DEFAULT.Trim();
                                if (strDefault != "")
                                    val = BF_FORM.Instance.GetReadParam(strDefault); //将函数转为具体的值
                            }
                            else
                            {
                                val = collection[item.EN_NAME] == null ? "" : collection[item.EN_NAME].ToString();
                                if (string.IsNullOrWhiteSpace(val) && item.INPUT_TYPE == (int)Enums.FormInputType.单个复选框)//针对单个复选框由于只存在是或否的关系所以如果不选择时，有默认值，就按默认值走，如果没有就为0
                                {
                                    val = "0";
                                }
                            }
                            #region 数据有效性验证
                            string errorMessage = BF_FORM.Instance.ValidateField(item, val, rowKey, entity.DB_ID, entity.TABLE_NAME, FieldList);
                            if (string.IsNullOrWhiteSpace(errorMessage) == false)
                            {
                                result.IsSuccess = false;
                                result.Message = errorMessage;
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            #endregion

                            if (string.IsNullOrWhiteSpace(val))//为空时特殊处理
                            {
                                if (item.FIELD_DATA_TYPE == (int)Enums.FieldDataType.数值 || item.FIELD_DATA_TYPE == (int)Enums.FieldDataType.日期)
                                    strValues.Add(null);
                                else
                                    strValues.Add("");
                            }
                            else
                            {
                                //添加值
                                switch (item.FIELD_DATA_TYPE)
                                {
                                    case (int)Enums.FieldDataType.数值:
                                        strValues.Add(Convert.ToDecimal(val));
                                        break;
                                    case (int)Enums.FieldDataType.日期:
                                        strValues.Add(Convert.ToDateTime(val));
                                        break;
                                    default:
                                        strValues.Add(val);
                                        break;
                                }
                            }
                        }
                    }
                    entity.TABLE_NAME = BF_FORM.Instance.GetTableName(entity, yyyy, yyyymm, yyyymmdd, ref strMessage);//得到动态表名
                    if (string.IsNullOrEmpty(strMessage) && string.IsNullOrEmpty(entity.TABLE_NAME) == false)
                        bitResult = BF_FORM.Instance.UpdataData(entity.DB_ID, entity.TABLE_NAME, strFields, strValues, FieldList, rowKey);
                    #endregion
                }
                else if (op == "2")//删除
                {
                    #region 删除
                    entity.TABLE_NAME = BF_FORM.Instance.GetTableName(entity, yyyy, yyyymm, yyyymmdd, ref strMessage);//得到动态表名
                    if (string.IsNullOrEmpty(strMessage) && string.IsNullOrEmpty(entity.TABLE_NAME) == false)
                        bitResult = BF_FORM.Instance.DeleteData(entity.DB_ID, entity.TABLE_NAME, FieldList, rowKey);
                    #endregion
                }

                if (bitResult)
                    result.Message = "数据提交成功！";
                else
                    result.Message = "对不起！数据提交失败，请联系管理员。";

                result.IsSuccess = bitResult;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "模板操作异常：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        /// <summary>
        /// 获得指定表的下一个序列值
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="isBeginPre"></param>
        /// <param name="fix"></param>
        /// <returns></returns>
        private int GetNextValueFromSeqDef(string tableName, bool isBeginPre = true, string fix = "SQ_")
        {
            string seq = "";
            try
            {
                if (isBeginPre)
                {
                    seq = fix + tableName;
                }
                else
                {
                    seq = tableName + fix;
                }
                using (Base.DBHelper.BDBHelper dbHelper = new Base.DBHelper.BDBHelper())
                {
                    return dbHelper.GetNextValueFromSeq(seq);
                }
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, string.Format(@"获得序列[{0}]失败:{1}", seq, ex.Message));
            }
            return 0;
        }
    }
}