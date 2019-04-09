using CS.Base.DBHelper;
using CS.Base.Log;
using CS.Common.FW;
using CS.Library.BaseQuery;
using CS.Library.Export;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CS.BLL.FW.BF_FORM.FieldInfo;
using static CS.Common.FW.Enums;

namespace CS.BLL.FW
{
    /// <summary>
    /// 表单编辑
    /// </summary>
    public class BF_FORM : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FORM Instance = new BF_FORM();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FORM()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FORM";
            this.ItemName = "表单编辑";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
        }

        #region 实体

        /// <summary>
        /// 实体
        /// </summary>
        public class Entity
        {
            /// <summary>
            /// ID 
            /// </summary>
            [Field(IsPrimaryKey = true, IsAutoIncrement = true, IsNotNull = true, Comment = "ID ")]
            public int ID { get; set; }

            /// <summary>
            /// 表单名称
            /// </summary>
            [Field(IsNotNull = true, Length = 64, IsIndex = true, IsIndexUnique = true, Comment = "表单名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 数据库ID（为0时表示默认数据库）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "数据库ID（为0时表示默认数据库）")]
            public int DB_ID { get; set; }

            /// <summary>
            /// 数据库表名
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "数据库表名")]
            public string TABLE_NAME { get; set; }

            /// <summary>
            /// 建表模式（对应枚举：CreateTableMode）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "建表模式（对应枚举：CreateTableMode）")]
            public Int16 CREATE_TABLE_MODE { get; set; }

            /// <summary>
            /// 字段信息，存放FieldInfo的JSON串
            /// </summary>
            [Field(IsNotNull = false, Length = 2048, Comment = "字段信息，存放FieldInfo的JSON串")]
            public string FIELDS { get; set; }

            /// <summary>
            /// 是否允许删除数据
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否允许删除数据")]
            public Int16 IS_ALLOW_DELETE { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "备注")]
            public string REMARK { get; set; }

            /// <summary>
            /// JS脚本
            /// </summary>
            [Field(IsNotNull = false, Length = 1000000, Comment = "JS脚本")]
            public string JS_CODE { get; set; }

            /// <summary>
            /// 创建者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "创建者ID")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 修改者者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "修改者者ID")]
            public int UPDATE_UID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 修改时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "修改时间")]
            public DateTime UPDATE_TIME { get; set; }
        }
        #endregion

        #region 字段信息
        /// <summary>
        /// 字段信息
        /// </summary>
        public class FieldInfo
        {
            /// <summary>
            /// 字段英文名 
            /// </summary>
            public string EN_NAME { get; set; }

            /// <summary>
            /// 字段中文名 
            /// </summary>
            public string CN_NAME { get; set; }

            /// <summary>
            /// 数据类型（对应枚举FieldDataType）
            /// </summary>
            public int FIELD_DATA_TYPE { get; set; }

            /// <summary>
            /// 是否为主键（有且仅有一个主键字段，1为是，0为否）
            /// </summary>
            public int IS_KEY_FIELD { get; set; }

            /// <summary>
            /// “新增”是否包含本字段（1为是，0为否）
            /// </summary>
            public int IS_INSERT { get; set; }

            /// <summary>
            /// “更新”是否包含本字段（1为是，0为否）
            /// </summary>
            public int IS_UPDATE { get; set; }

            /// <summary>
            /// 只读字段（1为是，0为否）
            /// </summary>
            public int IS_READONLY { get; set; }

            /// <summary>
            /// 是否不能为空（必须输入，1为是，0为否）
            /// </summary>
            public int IS_NOT_NULL { get; set; }

            /// <summary>
            /// 是否为自增长字段（最多一个自增长字段，1为是，0为否）
            /// </summary>
            public int IS_AUTO_INCREMENT { get; set; }

            /// <summary>
            /// 该字段是否为唯一约束（最多一个唯一约束字段，1为是，0为否）
            /// </summary>
            public int IS_UNIQUE { get; set; }
            /// <summary>
            /// 排序（值越小越靠前）
            /// </summary>
            public int ORDER_NUM { get; set; }
            /// <summary>
            /// 表单输入框类型（对应枚举：FormInputType）
            /// </summary>
            public int INPUT_TYPE { get; set; }

            /// <summary>
            /// 下拉输入框的数据来源方式（当INPUT_TYPE的值为“下拉单选框”时有效，对应枚举：FormInputType）
            /// </summary>
            public int SELECT_INPUT_TYPE { get; set; }
            /// <summary>
            /// 字段默认值（当出现花扩号为函数如：{sysdate},普通死值就不要加花括号了）
            /// </summary>
            public string DEFAULT { get; set; }
            /// <summary>
            /// 下拉框的枚举值（当SELECT_INPUT_TYPE的值为“枚举值”时有效）
            /// </summary>
            public List<SelectEnumOption> SELECT_ENUM_OPTIONS { get; set; }

            /// <summary>
            /// 下拉框的查询定义（当当SELECT_INPUT_TYPE的值为“查询表”时有效）
            /// </summary>
            public SelectQueryInfo SELECT_QUERY_INFO { get; set; }
            /// <summary>
            /// SQL语句
            /// </summary>
            public string SQL { get; set; }
            /// <summary>
            /// 下拉框的选项值定义
            /// </summary>
            public class SelectEnumOption
            {
                /// <summary>
                /// Option的值
                /// </summary>
                public string VALUE { get; set; }
                /// <summary>
                /// Option的显示名
                /// </summary>
                public string NAME { get; set; }
                /// <summary>
                /// Option的父级节点
                /// </summary>
                public string PID { get; set; }
            }

            /// <summary>
            /// 下拉框查询表定义
            /// </summary>
            public class SelectQueryInfo
            {
                /// <summary>
                /// 数据库ID
                /// </summary>
                public int DB_ID { get; set; }
                /// <summary>
                /// 表名
                /// </summary>
                public string TABLE_NAME { get; set; }
                /// <summary>
                /// 值字段
                /// </summary>
                public string VALUE_FIELD { get; set; }
                /// <summary>
                /// 名称字段
                /// </summary>
                public string NAME_FIELD { get; set; }
                /// <summary>
                /// 名称字段
                /// </summary>
                public string PID_FIELD { get; set; }
                /// <summary>
                /// 查询过滤条件
                /// </summary>
                public string WHERE { get; set; }
            }
        }
        #endregion

        #region 生成模板
        /// <summary>
        /// 自动生成模板
        /// </summary>
        /// <param name="op"></param>
        /// <param name="FieldList"></param>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public string GenerateTemplate(int op, List<BF_FORM.FieldInfo> FieldList, DataRow dataRow)
        {
            FieldList.Sort((a, b) => (a.ORDER_NUM).CompareTo(b.ORDER_NUM));//排序
            #region 生成模板
            StringBuilder strHtml = new StringBuilder();
            int i = 0;
            int cotrollerCountNum = 0;//记录是控件总数
            int isReadNum = 0;//记录只读数量
            foreach (BF_FORM.FieldInfo fileItem in FieldList)
            {
                if (fileItem.INPUT_TYPE == (int)Enums.FormInputType.不显示字段)//不显示的字段不加载标签
                    continue;

                #region 判断新增与更新
                if (op == 0 && fileItem.IS_INSERT == 0)
                    continue;
                else if (op == 1 && fileItem.IS_UPDATE == 0 && fileItem.IS_READONLY == 0)
                    continue;
                #endregion

                cotrollerCountNum++;//记录控件总数
                if (fileItem.IS_READONLY == 1)//记录只读总数
                    isReadNum++;

                if (fileItem.INPUT_TYPE == (int)FormInputType.多行文字框 || fileItem.INPUT_TYPE == (int)FormInputType.多个复选框) //多行文本，用
                {
                    if (i % 2 != 0 && i != 0)//是否需要结束上级标签
                        strHtml.Append("    </div>");

                    strHtml.Append("    <div class=\"layui-form-item report_two\">\n");
                    if (fileItem.IS_NOT_NULL == 1)//加入必填标志
                        strHtml.Append("            <label class=\"layui-form-label \">" + fileItem.CN_NAME + "<div class=\"imust\">*</div></label>");
                    else
                        strHtml.Append("            <label class=\"layui-form-label \">" + fileItem.CN_NAME + "</label>");
                    strHtml.Append("            <div class=\"layui-input-block \">");
                    strHtml.Append(GetControllerHtml(fileItem, dataRow));
                    strHtml.Append("            </div> ");
                    strHtml.Append("    </div>");
                    i = 0;//重新起行
                    continue;
                }

                if (i % 2 == 0 || i == 0)
                    strHtml.Append("    <div class=\"layui-form-item\">\n");

                strHtml.Append("        <div>");
                if (fileItem.IS_NOT_NULL == 1)//加入必填标志
                    strHtml.Append("            <label class=\"layui-form-label \">" + fileItem.CN_NAME + "<div class=\"imust\">*</div></label>");
                else
                    strHtml.Append("            <label class=\"layui-form-label \">" + fileItem.CN_NAME + "</label>");
                strHtml.Append("            <div class=\"layui-input-inline\">");
                strHtml.Append(GetControllerHtml(fileItem, dataRow));
                strHtml.Append("            </div> ");
                strHtml.Append("        </div>");

                if (i % 2 != 0 && i != 0)
                    strHtml.Append("    </div>");
                i++;
            }
            #endregion

            #region 是否显示提交按钮
            if (cotrollerCountNum == isReadNum) //是否显示提交按钮
                strHtml.Append("<input type=\"hidden\" value=\"1\" id=\"isShowSubmit\" />");
            else
                strHtml.Append("<input type=\"hidden\" value=\"0\" id=\"isShowSubmit\" />");
            #endregion

            return strHtml.ToString();
        }
        #endregion

        #region 通过类型返回模板控件
        public string GetControllerHtml(BF_FORM.FieldInfo fieldData, DataRow row)
        {
            if (fieldData == null)
                return "";
            #region 控件模板
            string inputHtml = "<input type=\"text\" id=\"{id}\" name=\"{name}\" lay-filter=\"{别名}\" value=\"{value}\" lay-verify=\"{验证}\" placeholder=\"{提示}\" autocomplete=\"off\" class=\"layui-input\" {只读}>";
            string dataHtml = "<input type=\"text\" id=\"{id}\" name=\"{name}\" lay-filter=\"{别名}\" value=\"{value}\" lay-verify=\"datetime\" placeholder=\"yyyy-MM-dd HH:mm:ss\" autocomplete=\"off\" class=\"layui-input\" {只读}>";
            string checkHtml = "<input type=\"checkbox\" id=\"{id}\" name=\"{name}\" lay-filter=\"{别名}\" value=\"{value}\" title=\"{提示}\" {选中} {只读}>";
            string textareaHtml = "<textarea id=\"{id}\" name=\"{name}\" lay-filter=\"{别名}\" placeholder=\"{提示}\" lay-verify=\"{验证}\" class=\"layui-textarea\" {只读}>{内容}</textarea>";
            string selectHtml = "<select lay-search id=\"{id}\"  name=\"{name}\" lay-filter=\"{别名}\" lay-verify=\"{验证}\" {只读}>{选项}</select>";
            string checkboxHtml = "<span {只读}>{选项}</span>";
            string treeHtml = "<input type=\"text\" id=\"{id}\" name=\"{name}\" value=\"{value}\" {只读}>";
            #endregion

            string strHtml = "";
            string value = "";
            string enName = fieldData.EN_NAME;//验证字段的默认值
            if (row != null && row[enName] != null)
                value = row[enName].ToString().Trim();
            else
            {
                //添加默认值                
                if (string.IsNullOrWhiteSpace(fieldData.DEFAULT) == false)
                {
                    value = fieldData.DEFAULT;
                    #region 处理函数
                    int s = value.IndexOf("@{");
                    int e = value.IndexOf("}");
                    if (s == 0 && e == value.Length - 1)
                        value = BF_FORM.Instance.GetReadParam(value); //得到参数值                    
                    #endregion                    
                }
            }

            switch (fieldData.INPUT_TYPE)
            {
                case (int)Enums.FormInputType.下拉单选框:
                    string opHtml = GetSelectOption(fieldData, value);
                    strHtml = selectHtml.Replace("{选项}", opHtml);
                    break;
                case (int)Enums.FormInputType.单个复选框:
                    if (value == "1" || value.ToLower() == "true")
                        checkHtml = checkHtml.Replace("{选中}", "checked");
                    else
                        checkHtml = checkHtml.Replace("{选中}", "");
                    checkHtml = checkHtml.Replace("{提示}", fieldData.CN_NAME).Replace("{value}", (value == "" ? "0" : value));//这个地方设默认值为0可能存在争议
                    strHtml = checkHtml;
                    break;
                case (int)Enums.FormInputType.多个复选框:
                    opHtml = GetCheck(fieldData, value);
                    if (opHtml != null && opHtml != "")
                        strHtml = checkboxHtml.Replace("{选项}", opHtml);
                    break;
                case (int)Enums.FormInputType.多行文字框:
                    strHtml = textareaHtml.Replace("{内容}", value);
                    break;
                case (int)Enums.FormInputType.日期文字框:
                    if (value != null && value != "")
                        value = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                    strHtml = dataHtml.Replace("{value}", value);
                    break;
                case (int)Enums.FormInputType.普通文字框:
                    strHtml = inputHtml.Replace("{value}", value);
                    if (fieldData.FIELD_DATA_TYPE == (int)Enums.FieldDataType.数值)//如果是数值性的就用数值型的文本
                        strHtml = strHtml.Replace("type=\"text\"", "type=\"number\"");
                    break;
                case (int)Enums.FormInputType.下拉树选择:
                    strHtml = treeHtml.Replace("{value}", value);
                    strHtml += GetTrea(fieldData);
                    break;
                default:
                    strHtml = inputHtml.Replace("{value}", value);
                    break;
            }
            strHtml = strHtml.Replace("{id}", enName).Replace("{name}", enName);
            strHtml = strHtml.Replace("{name}", enName).Replace("{别名}", enName);
            strHtml = strHtml.Replace("{提示}", "请输入" + fieldData.CN_NAME).Replace("{别名}", enName);
            strHtml = strHtml.Replace("{验证}", (fieldData.IS_NOT_NULL == 1 ? "required" : ""));
            #region 加入编辑时只读标签
            if (row != null && fieldData.IS_READONLY == 1)//是否为编辑时的只读字段
            {
                strHtml = strHtml.Replace("{只读}", "readonly=\"readonly\"");
            }
            else
            {
                strHtml = strHtml.Replace("{只读}", "");
            }
            #endregion

            return strHtml;
        }
        #endregion

        #region 得到下拉选项
        private string GetSelectOption(BF_FORM.FieldInfo fieldData, string value)
        {
            DataTable opData = SelectItem(fieldData);
            if (opData == null || opData.Rows.Count <= 0)
                return "";
            //生成字符串
            string opHtml = "";
            foreach (DataRow item in opData.Rows)
                opHtml += "<option value=\"" + item["v"] + "\"" + (item["v"].ToString() == value ? " selected=\"selected\"" : "") + ">" + item["k"] + "</option>";
            return opHtml;
        }
        #endregion

        #region 得到多个复选框
        private string GetCheck(BF_FORM.FieldInfo fieldData, string value)
        {
            DataTable opData = SelectItem(fieldData);
            //生成字符串
            string opHtml = "";
            string[] vals = value.Split(',');
            bool isCk = false;//是否选中
            if (opData == null || opData.Rows.Count <= 0)
                return "";
            foreach (DataRow item in opData.Rows)
            {
                isCk = false;//是否选中
                foreach (string val in vals)
                {
                    if (item["v"] != null && val == item["v"].ToString().Trim())
                    {
                        isCk = true;
                        break;
                    }
                }
                opHtml += "<input type =\"checkbox\" value=\"" + item["v"] + "\" name=\"" + fieldData.EN_NAME + "\" lay-skin=\"primary\" title=\"" + item["k"] + "\" " + (isCk ? "checked=\"\"" : "") + ">";
            }

            return opHtml;
        }
        #endregion

        #region 得到树
        private string GetTrea(BF_FORM.FieldInfo fieldData)
        {
            if (fieldData == null)
            {
                return "";
            }
            #region 组装数据集
            DataTable opData = SelectItem(fieldData);
            var obj = new List<object>();
            if (opData != null && opData.Rows.Count > 0)
            {
                foreach (DataRow dr in opData.Rows)
                {
                    obj.Add(
                        new
                        {
                            id = dr["v"] == null || dr["v"].ToString().Trim() == "" ? "" : dr["v"].ToString(),
                            name = dr["k"] == null || dr["k"].ToString().Trim() == "" ? "" : dr["k"].ToString(),
                            pId = dr["p"] == null || dr["p"].ToString().Trim() == "" ? "0" : dr["p"].ToString()
                        });
                }
            }
            JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings();
            string TreeVal = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
            #endregion

            //生成字符串
            string opHtml = "<script>$(function(){var zNodes" + fieldData.EN_NAME + " = JSON.parse('" + TreeVal + "');$.comboztree('{id}',{ztreenode: zNodes" + fieldData.EN_NAME + "});});</script>";//添加支撑JS
            return opHtml;
        }
        #endregion

        #region 得到数据项
        private DataTable SelectItem(BF_FORM.FieldInfo fieldData)
        {
            if (fieldData == null)
                return null;

            DataTable opData = new DataTable();
            switch (fieldData.SELECT_INPUT_TYPE)
            {
                case (int)FormSelectType.枚举值:
                    opData.Columns.Add("k");
                    opData.Columns.Add("v");
                    if (fieldData.INPUT_TYPE == (int)FormInputType.下拉树选择)
                        opData.Columns.Add("p");
                    List<SelectEnumOption> op = fieldData.SELECT_ENUM_OPTIONS;
                    if (op != null && op.Count > 0)
                    {
                        foreach (SelectEnumOption item in op)
                        {
                            DataRow row = opData.NewRow();
                            row["k"] = GetReadParam(item.NAME);
                            row["v"] = GetReadParam(item.VALUE);
                            if (fieldData.INPUT_TYPE == (int)FormInputType.下拉树选择)
                                row["p"] = (string.IsNullOrWhiteSpace(item.PID) ? "0" : item.PID);
                            opData.Rows.Add(row);
                        }
                    }
                    break;
                case (int)FormSelectType.表查询:
                    string pidField = "";
                    if (fieldData.INPUT_TYPE == (int)FormInputType.下拉树选择)
                    {
                        pidField = "," + fieldData.SELECT_QUERY_INFO.PID_FIELD + " p ";
                    }
                    string strField = fieldData.SELECT_QUERY_INFO.NAME_FIELD + " k," + fieldData.SELECT_QUERY_INFO.VALUE_FIELD + " v" + pidField;//需要查询的字段
                    string strWhere = fieldData.SELECT_QUERY_INFO.WHERE;
                    strWhere = GetReadParam(strWhere);//得到SQL
                    opData = GetData(fieldData.SELECT_QUERY_INFO.DB_ID, fieldData.SELECT_QUERY_INFO.TABLE_NAME, strField, strWhere);
                    break;
                case (int)FormSelectType.SQL语句:
                    if (string.IsNullOrWhiteSpace(fieldData.SQL) == false)
                    {
                        string[] info = fieldData.SQL.Split('◎');
                        if (info.Length < 2 || string.IsNullOrWhiteSpace(info[0]) || string.IsNullOrWhiteSpace(info[1]))
                        {
                            return null;
                        }
                        try
                        {
                            string strSql = GetReadParam(info[1]);//得到SQL
                            opData = BF_DATABASE.Instance.ExecuteSelectSQL(Convert.ToInt32(info[0]), strSql, null, 0, 1);
                        }
                        catch (Exception ex)
                        {
                            BLog.Write(BLog.LogLevel.ERROR, "保存SQL模式时,出现未知错误：" + ex.ToString());
                            return null;
                        }
                    }
                    break;
            }
            return opData;
        }
        #endregion

        #region 查询列表
        public DataTable GetDataTable(int limit, int page, string strWhere, List<object> value)
        {
            string strSql = "select f.id,f.name,(case f.db_id when 0 then '本地默认数据库' else to_char(DB.NAME) end ) DBNAME,TABLE_NAME,CREATE_TABLE_MODE,(case IS_ALLOW_DELETE when 1 then '是' else '否' end )IS_ALLOW_DELETE,(case IS_ENABLE when 1 then '是' else '否' end )IS_ENABLE,f.UPDATE_TIME,f.REMARK from BF_FORM f left join BF_DATABASE db on f.db_id=db.id where " + strWhere + " order by f.id desc";
            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0 && page == 0)
                {
                    return dbHelper.ExecuteDataTableParams(strSql, value);//不分页查询所有
                }
                return dbHelper.ExecuteDataTablePageParams(strSql, limit, page, value);
            }
        }
        #endregion

        #region 根据数据库及表来查询数据列表
        /// <summary>
        /// 自动根据主键查询数据集
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="tableName"></param>
        /// <param name="strField"></param>
        /// <param name="FieldList"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public DataTable GetData(int dbId, string tableName, string strField, List<BF_FORM.FieldInfo> FieldList, string rowKey)
        {
            if (dbId < 0 || tableName == null || tableName.Trim() == "" || strField == null || strField.Trim() == "")
                return null;
            tableName = tableName.Trim();
            strField = strField.Trim();
            List<object> param = new List<object>(); //用于存参数
            string strWhere = GetKeyWhere(FieldList, rowKey, ref param, false);//查询条件
            if (string.IsNullOrEmpty(strWhere) == false)
                strWhere = " where " + strWhere;

            string strSql = "select " + strField + " from " + tableName + strWhere;
            return BF_DATABASE.Instance.ExecuteSelectSQL(dbId, strSql, param, 0, 1);
        }
        /// <summary>
        /// 指定条件得数据集
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="tableName"></param>
        /// <param name="strField"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataTable GetData(int dbId, string tableName, string strField, string strWhere)
        {
            if (dbId < 0 || tableName == null || tableName.Trim() == "" || strField == null || strField.Trim() == "")
                return null;
            tableName = tableName.Trim();
            strField = strField.Trim();

            if (string.IsNullOrEmpty(strWhere) == false)
                strWhere = " where " + strWhere;

            string strSql = "select " + strField + " from " + tableName + strWhere;
            List<object> pram = new List<object>();
            DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(dbId, strSql, pram, 0, 1);
            if (dt == null || dt.Rows.Count <= 0)
            {
                dt = new DataTable();
                dt.Columns.Add("k");
                dt.Columns.Add("v");
                dt.Columns.Add("p");
            }
            return dt;
        }
        #endregion

        #region 数据添加
        public bool insertData(int dbId, string tableName, List<string> strFields, List<object> strValues)
        {
            char[] val = new char[strValues.Count];
            for (int i = 0; i < strFields.Count; i++)
            {
                val[i] = '?';
            }
            string sqlInsert = string.Format("insert into {0} ({1}) values ({2})", tableName, string.Join(",", strFields), string.Join(",", val));
            return BF_DATABASE.Instance.ExecuteNonQuery(dbId, sqlInsert, strValues) > 0;
        }
        #endregion

        #region 数据修改
        public bool UpdataData(int dbId, string tableName, List<string> strFields, List<object> strValues, List<BF_FORM.FieldInfo> FieldList, string rowKey)
        {
            string strWhere = GetKeyWhere(FieldList, rowKey, ref strValues, false);//查询条件
            string strSql = "update {0} set {1} where {2}";
            strSql = string.Format(strSql, tableName, string.Join(",", strFields), strWhere);
            return BF_DATABASE.Instance.ExecuteNonQuery(dbId, strSql, strValues) > 0 ? true : false;
        }
        #endregion

        #region 删除数据
        public bool DeleteData(int dbId, string tableName, List<BF_FORM.FieldInfo> FieldList, string rowKey)
        {
            List<object> param = new List<object>(); //用于存参数
            string strWhere = GetKeyWhere(FieldList, rowKey, ref param, false);//查询条件
            string strSql = "delete from " + tableName + " where 1=1 and " + strWhere;
            return BF_DATABASE.Instance.ExecuteNonQuery(dbId, strSql, param) > 0 ? true : false;
        }
        #endregion

        #region 数据主键条件
        /// <summary>
        /// 数据主键条件
        /// </summary>
        /// <param name="FieldList">字段列表</param>
        /// <param name="rowKey">主建数据</param>
        /// <param name="param">返回参数</param>
        /// <param name="isNotIn">是否包含</param>
        /// <returns></returns>
        public string GetKeyWhere(List<BF_FORM.FieldInfo> FieldList, string rowKey, ref List<object> param, bool isNotIn)
        {
            string strWhere = "";
            rowKey = rowKey.Trim();
            foreach (BF_FORM.FieldInfo item in FieldList)
            {
                if (item.IS_KEY_FIELD == 1)//只取第一条，理论上也只会有一条
                {
                    param.Add(GetFieldValue(item.EN_NAME, rowKey, item.FIELD_DATA_TYPE));
                    strWhere = item.EN_NAME + (isNotIn ? "<>" : "=") + " ?";
                    break;
                }
            }
            return strWhere;
        }
        #endregion

        #region 读取数据类型
        public object GetFieldValue(string strField, string strValue, int FIELD_DATA_TYPE)
        {
            switch (FIELD_DATA_TYPE)
            {
                case (int)Enums.FieldDataType.数值:
                    return Convert.ToDecimal(strValue);
                case (int)Enums.FieldDataType.日期:
                    return Convert.ToDateTime(strValue);
                default:
                    return strValue;
            }
        }
        #endregion

        #region 解析默认值
        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public string GetReadParam(string strText)
        {
            //int s = strText.IndexOf("@{");
            //int e = strText.IndexOf("}");
            //if (s != 0 || e != strText.Length - 1)
            //    return strText;
            if (string.IsNullOrWhiteSpace(strText) || strText.Trim()=="")
            {
                return "";
            }
            strText = strText.ToUpper();

            #region URL参数替换
            var parames = System.Web.HttpContext.Current.Request.Params;
            foreach (var item in parames)
            {
                string val = System.Web.HttpContext.Current.Request.Params[item.ToString()];
                strText = strText.Replace("@(" + item.ToString().ToUpper() + ")", val);
            }
            #endregion

            strText = strText.Replace("@{USER_ID}", SystemSession.UserID.ToString());
            strText = strText.Replace("@{USER_NAME}", SystemSession.UserName);
            strText = strText.Replace("@{DEPARTMENT_ID}", SystemSession.UserDepartmentID.ToString());
            strText = strText.Replace("@{DEPARTMENT_CODE}", SystemSession.UserDepartmentCode.ToString());
            strText = strText.Replace("@{DEPARTMENT_NAME}", SystemSession.UserDepartmentName);
            strText = strText.Replace("@{DEPARTMENT_LEVEL}", SystemSession.UserDepartmentLevel.ToString());
            if (strText.IndexOf("@{ALLROLE}") >= 0)//当前用户所属权限
                strText = strText.Replace("@{ALLROLE}", BF_USER.Instance.GetUserRole(SystemSession.UserID));
            strText = strText.Replace("@{DATETIME}", DateTime.Now.ToString());
            strText = strText.Replace("@{DATE}", DateTime.Now.ToString("yyyy-MM-dd"));
            strText = Functions.Instance.ExecuteFunction(strText);//日期参数
            return strText;
        }
        #endregion

        #region 数据有效性验证
        public string ValidateField(BF_FORM.FieldInfo FieldInfo, string val, string rowKey, int DB_ID, string TABLE_NAME, List<BF_FORM.FieldInfo> FieldList)
        {
            string message = "";
            if (string.IsNullOrWhiteSpace(val) && FieldInfo.IS_NOT_NULL == 1)//验证必填字段
            {
                return "对不起！" + FieldInfo.CN_NAME + "字段不能为空。";
            }
            //唯一性约束
            if (string.IsNullOrWhiteSpace(val) == false && FieldInfo.IS_UNIQUE == 1)
            {
                List<object> paramList = new List<object>();
                paramList.Add(val);
                string sql = "SELECT {0} FROM {1} WHERE {0} = ?";
                if (string.IsNullOrWhiteSpace(rowKey) == false)
                {
                    string strWhere = GetKeyWhere(FieldList, rowKey, ref paramList, true);
                    sql += " AND " + strWhere;
                }
                sql = string.Format(sql, FieldInfo.EN_NAME, TABLE_NAME);

                DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(DB_ID, sql, paramList, 0);
                if (dt != null && dt.Rows.Count > 0)
                    return "对不起！" + FieldInfo.CN_NAME + "字段不能重复。因为您违反了唯一性约束！";
            }
            return message;
        }

        #endregion

        #region 建表模式
        public string CreateTable(BF_FORM.Entity entity, List<BF_FORM.FieldInfo> FieldList)
        {
            #region 计算表名
            short tableMode = entity.CREATE_TABLE_MODE;
            string tableName = entity.TABLE_NAME;
            #region 数据库对像
            BDBHelper dbHelper = null;
            if (entity.DB_ID == 0)
            {
                dbHelper = new BDBHelper();
            }
            else
            {
                BF_DATABASE.Entity db = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(entity.DB_ID);
                if (db == null)
                {
                    BLog.Write(BLog.LogLevel.WARN, "查询数据库对像出现异常：数据库ID" + entity.DB_ID);
                    return "";
                }

                string dbType = BF_DATABASE.GetDbTypeName(db.DB_TYPE);
                dbHelper = new BDBHelper(dbType, db.IP, db.PORT, db.USER_NAME, db.PASSWORD, db.DB_NAME, db.DB_NAME);
            }
            #endregion
            //如查指定的表存在就返回
            if (entity.CREATE_TABLE_MODE == (short)Enums.CreateTableMode.指定表 && dbHelper.TableIsExists(tableName))
            {
                return tableName;
            }

            switch (tableMode)
            {
                case (short)Enums.CreateTableMode.年份后缀:
                    tableName += "_" + DateTime.Now.ToString("yyyy");
                    break;
                case (short)Enums.CreateTableMode.年月后缀:
                    tableName += "_" + DateTime.Now.ToString("yyyyMM");
                    break;
                case (short)Enums.CreateTableMode.年月日后缀:
                    tableName += "_" + DateTime.Now.ToString("yyyyMMdd");
                    break;
                case (short)Enums.CreateTableMode.用户ID后缀:
                    tableName += "_" + SystemSession.UserID;
                    break;
            }
            #endregion

            #region 建表
            //自增长主键
            string autoIncrementField = string.Empty;
            //唯一约束字段
            string uniqueField = string.Empty;
            //唯一约束的值
            //  Dictionary<string, bool> dicUniqueValues = new Dictionary<string, bool>();

            foreach (var field in FieldList)
            {
                if (field.IS_AUTO_INCREMENT == 1)
                {
                    autoIncrementField = field.EN_NAME;
                }
                if (field.IS_UNIQUE == 1)
                {
                    uniqueField = field.EN_NAME;
                }
            }

            //表不存在
            if (entity.CREATE_TABLE_MODE != (short)Enums.CreateTableMode.指定表 && dbHelper.TableIsExists(tableName) == false)
            {
                //创建表
                if (dbHelper.CreateTable(tableName, entity.TABLE_NAME) == false)
                {
                    BLog.Write(BLog.LogLevel.WARN, "动态建表出现异常：" + tableName);
                    return "";
                }
                //设置自增长
                if (dbHelper.SetAutoIncrement(tableName, autoIncrementField) == false)
                {
                    BLog.Write(BLog.LogLevel.WARN, "设置表" + tableName + "的字段" + autoIncrementField + "为自增长失败");
                    return "";
                }
            }
            #endregion
            return tableName;
        }
        #endregion

        #region 建表模式
        public string GetTableName(BF_FORM.Entity entity, string yyyy, string yyyymm, string yyyymmdd, ref string strMessage)
        {
            #region 计算表名
            short tableMode = entity.CREATE_TABLE_MODE;
            string tableName = entity.TABLE_NAME;

            switch (tableMode)
            {
                case (short)Enums.CreateTableMode.年份后缀:
                    if (string.IsNullOrEmpty(yyyy))
                    {
                        strMessage = "当前为表后缀为年份，请传入年份格式：yyyy";
                        return "";
                    }
                    tableName += "_" + yyyy;
                    break;
                case (short)Enums.CreateTableMode.年月后缀:
                    if (string.IsNullOrEmpty(yyyymm))
                    {
                        strMessage = "当前为表后缀为年月，请传入年份格式：yyyymm";
                        return "";
                    }
                    tableName += "_" + yyyymm;
                    break;
                case (short)Enums.CreateTableMode.年月日后缀:
                    if (string.IsNullOrEmpty(yyyymmdd))
                    {
                        strMessage = "当前为表后缀为年月日，请传入年份格式：yyyymmdd";
                        return "";
                    }
                    tableName += "_" + yyyymmdd;
                    break;
                case (short)Enums.CreateTableMode.用户ID后缀:
                    tableName += "_" + SystemSession.UserID;
                    break;
            }
            #endregion            
            return tableName;
        }
        #endregion
    }
}
