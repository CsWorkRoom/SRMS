using CS.Base.DBHelper;
using CS.Common.FW;
using CS.Library.BaseQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CS.BLL.FW
{
    /// <summary>
    /// 表格报表
    /// </summary>
    public class BF_TB_REPORT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_TB_REPORT Instance = new BF_TB_REPORT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_TB_REPORT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_TB_REPORT";
            this.ItemName = "表格报表";
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
            /// 报表名称
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "报表名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 数据库ID（为0时表示默认数据库）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "数据库ID（为0时表示默认数据库）")]
            public int DB_ID { get; set; }

            /// <summary>
            /// 是否显示“导出”按钮
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否显示“导出”按钮")]
            public Int16 IS_SHOW_EXPORT { get; set; }

            /// <summary>
            /// 是否显示“调试”按钮
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否显示“调试”按钮")]
            public Int16 IS_SHOW_DEBUG { get; set; }

            /// <summary>
            /// 是否显示复选框
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否显示复选框")]
            public Int16 IS_SHOW_CHECKBOX { get; set; }

            /// <summary>
            /// SQL语句
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "SQL语句")]
            public string SQL_CODE { get; set; }

            /// <summary>
            /// 默认输入参数（用于后台分析SQL语句提取字段信息）
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "默认输入参数（用于后台分析SQL语句提取字段信息）")]
            public string DEFAULT_INPUT_VALUES { get; set; }

            /// <summary>
            /// 字段显示设置（使用layui的table样式存储）
            /// </summary>
            [Field(IsNotNull = false, Length = 2048, Comment = "字段显示设置（使用layui的table样式存储）")]
            public string SHOW_FIELDS { get; set; }

            /// <summary>
            /// 表格上方的代码（允许HTML、CSS及JS）
            /// </summary>
            [Field(IsNotNull = false, Length = 4096, Comment = "表格上方的代码（允许HTML、CSS及JS）")]
            public string TOP_CODE { get; set; }

            /// <summary>
            /// 表格下方的代码（允许HTML、CSS及JS）
            /// </summary>
            [Field(IsNotNull = false, Length = 4096, Comment = "表格下方的代码（允许HTML、CSS及JS）")]
            public string BOTTOM_CODE { get; set; }

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

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetEnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 1);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetUnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 更新显示字段配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="showFields"></param>
        /// <returns></returns>
        public int SetShowFields(int id, string showFields)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SHOW_FIELDS", showFields);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 用户自定义输入项
        /// </summary>
        public class InputValueItem
        {
            /// <summary>
            /// 输入框名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 值
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// 筛选项
        /// </summary>
        public class QueryFilterItem
        {
            /// <summary>
            /// 字段名称
            /// </summary>
            public string Field { get; set; }
            /// <summary>
            /// 字段数据类型
            /// </summary>
            public int DataType { get; set; }
            /// <summary>
            /// 运算逻辑
            /// </summary>
            public int Operator { get; set; }
            /// <summary>
            /// 输入项值
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// 验证SQL
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="sqlCode">SQL语句</param>
        /// <param name="inputJson">输入项及值（JSON串）</param>
        public static void CheckSql(int dbID, string sqlCode, string inputJson)
        {
            string sql = string.Empty;
            List<object> paramList = new List<object>();
            List<InputValueItem> inputList = null;
            if (string.IsNullOrWhiteSpace(inputJson) == false)
            {
                try
                {
                    inputList = JsonConvert.DeserializeObject<List<InputValueItem>>(inputJson);
                }
                catch
                {
                    throw new Exception("输入项错误");
                }
            }

            try
            {
                TransSQL(sqlCode, "", inputList, null, out sql, out paramList);
            }
            catch (Exception ex)
            {
                throw new Exception("解析SQL语句出错：" + ex.Message);
            }
            int rows = 1;
            try
            {
                DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(dbID,  sql, paramList, ref rows, 1, 1);
            }
            catch (Exception ex)
            {
                throw new Exception("尝试执行SQL语句出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 查询报表
        /// </summary>
        /// <param name="reportID">报表ID</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="count">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="inputJson">自定义输入项的值</param>
        /// <param name="whereJson">过滤字段</param>
        /// <returns></returns>
        public static DataTable QueryTable(int reportID, int pageIndex, int pageSize, ref int count, out string sql, out List<object> paramList, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            Entity entity = Instance.GetEntityByKey<Entity>(reportID);
            if (entity == null)
            {
                throw new Exception("报表" + reportID + "不存在");
            }
            return QueryTable(entity, pageIndex, pageSize, ref count, out sql, out paramList, getQueryString, inputJson, whereJson, order);
        }

        /// <summary>
        /// 查询报表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="count">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="getQueryString">URL中GET参数</param>
        /// <param name="inputJson">自定义输入项的值</param>
        /// <param name="whereJson">过滤字段</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public static DataTable QueryTable(Entity entity, int pageIndex, int pageSize, ref int count, out string sql, out List<object> paramList, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            sql = string.Empty;
            paramList = new List<object>();

            //DataTable dt = new DataTable();
            if (entity == null)
            {
                throw new Exception("报表不存在");
            }

            if (entity.IS_ENABLE != 1)
            {
                throw new Exception("报表" + entity.NAME + "已停用");
            }
            if (string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                throw new Exception("报表" + entity.NAME + "的SQL语句未配置");
            }

            Dictionary<string, string> dicInputValues = new Dictionary<string, string>();

            List<InputValueItem> inputList = null;
            if (string.IsNullOrWhiteSpace(inputJson) == false)
            {
                try
                {
                    inputList = JsonConvert.DeserializeObject<List<InputValueItem>>(inputJson);
                }
                catch
                {
                    throw new Exception("输入项错误");
                }
            }

            List<QueryFilterItem> filterList = null;
            if (string.IsNullOrWhiteSpace(whereJson) == false)
            {
                try
                {
                    filterList = JsonConvert.DeserializeObject<List<QueryFilterItem>>(whereJson);
                }
                catch
                {
                    throw new Exception("筛选项错误");
                }
            }

            try
            {
                TransSQL(entity.SQL_CODE, getQueryString, inputList, filterList, out sql, out paramList, order);
            }
            catch (Exception ex)
            {
                throw new Exception("转换SQL语句出错:" + ex);
            }

            return BF_DATABASE.Instance.ExecuteSelectSQL(entity.DB_ID, sql, paramList, ref count, pageSize, pageIndex);

        }

        /// <summary>
        /// 转换SQL语句
        /// </summary>
        /// <param name="sqlCode">原始SQL语句</param>
        /// <param name="queryString">URL中GET参数</param>
        /// <param name="inputList">自定义输入项(JSON串）</param>
        /// <param name="filterList">筛选项</param>
        /// <param name="paramList">参数列表（输出）</param>
        /// <returns>SQL语句</returns>
        private static void TransSQL(string sqlCode,string queryString, List<InputValueItem> inputList, List<QueryFilterItem> filterList, out string sql, out List<object> paramList, Order order = null)
        {
            sql = string.Empty;
            paramList = new List<object>();
            if (string.IsNullOrWhiteSpace(sqlCode))
            {
                return;
            }

            //执行函数
            sql = Functions.Instance.ExecuteFunction(sqlCode);

            //替换SESSION变量
            sql = SystemSession.TransParams(sql);

            //自定义输入参数
            Dictionary<string, string> dicInputValues = new Dictionary<string, string>();
            if (inputList != null)
            {
                foreach (InputValueItem input in inputList)
                {
                    if (dicInputValues.ContainsKey(input.Name) == false)
                    {
                        dicInputValues.Add(input.Name, input.Value);
                    }
                }
            }

            //替换输入项
            Regex regex = new Regex(@"@\((?<name>[a-z_0-9\-]+)\)", RegexOptions.IgnoreCase);
            Match match = regex.Match(sql);
            while (match != null && match.Success)
            {
                string name = match.Result("${name}");

                if (dicInputValues.ContainsKey(name) == true)
                {
                    if (dicInputValues[name] == null)
                    {
                        sql = sql.Replace("@(" + name + ")", "");
                    }
                    else
                    {
                        sql = sql.Replace("@(" + name + ")", dicInputValues[name]);
                    }
                }
                else
                {
                    Regex reg = new Regex("&" + name + @"=(?<value>[^\?&=]*)", RegexOptions.IgnoreCase);
                    Match m = reg.Match("&" + queryString);
                    if (m.Success == true)
                    {
                        sql = sql.Replace("@(" + name + ")", m.Result("${value}"));
                    }
                    else
                    {
                        throw new Exception("SQL语句中，@(" + name + ") 未找到定义");
                    }
                }

                match = match.NextMatch();
            }

            //替换筛选项
            if (filterList != null)
            {
                StringBuilder sbWhere = new StringBuilder();
                sbWhere.AppendLine("WHERE 1=1");
                int filterCount = 0;
                string fieldName = string.Empty;
                try
                {
                    foreach (QueryFilterItem filter in filterList)
                    {
                        fieldName = filter.Field;
                        if (string.IsNullOrWhiteSpace(filter.Value))
                        {
                            continue;
                        }
                        filterCount++;
                        Enums.FieldDataType dataType = (Enums.FieldDataType)filter.DataType;
                        Enums.FilterOperator oper = (Enums.FilterOperator)filter.Operator;
                        switch (dataType)
                        {
                            case Enums.FieldDataType.数值:
                                switch (oper)
                                {
                                    case Enums.FilterOperator.小于:
                                        sbWhere.AppendLine("AND " + filter.Field + "<?");
                                        paramList.Add(Convert.ToDecimal(filter.Value));
                                        break;
                                    case Enums.FilterOperator.小于等于:
                                        sbWhere.AppendLine("AND " + filter.Field + "<=?");
                                        paramList.Add(Convert.ToDecimal(filter.Value));
                                        break;
                                    case Enums.FilterOperator.大于:
                                        sbWhere.AppendLine("AND " + filter.Field + ">?");
                                        paramList.Add(Convert.ToDecimal(filter.Value));
                                        break;
                                    case Enums.FilterOperator.大于等于:
                                        sbWhere.AppendLine("AND " + filter.Field + ">=?");
                                        paramList.Add(Convert.ToDecimal(filter.Value));
                                        break;
                                    case Enums.FilterOperator.不等于:
                                        sbWhere.AppendLine("AND " + filter.Field + ">=?");
                                        paramList.Add(Convert.ToDecimal(filter.Value));
                                        break;
                                    case Enums.FilterOperator.介于:
                                        sbWhere.AppendLine("AND " + filter.Field + ">? AND " + filter.Field + "<?");
                                        string[] vdecs = filter.Value.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                                        paramList.Add(Convert.ToDecimal(vdecs[0]));
                                        paramList.Add(Convert.ToDecimal(vdecs[1]));
                                        break;
                                    case Enums.FilterOperator.集合:
                                        string[] vdecl = filter.Value.Split(new char[] { ',', '，', '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (string v in vdecl)
                                        {
                                            Convert.ToDecimal(v);
                                        }
                                        sbWhere.AppendLine("AND " + filter.Field + " IN (" + string.Join(",", vdecl) + ")");
                                        break;
                                    default:
                                        sbWhere.AppendLine("AND " + filter.Field + "=?");
                                        paramList.Add(Convert.ToDecimal(filter.Value));
                                        break;
                                }
                                break;
                            case Enums.FieldDataType.日期:
                                string[] vdates = filter.Value.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                                DateTime begin = Convert.ToDateTime(vdates[0]);
                                DateTime end = begin.AddDays(1);
                                if (vdates.Length > 1)
                                {
                                    end = Convert.ToDateTime(vdates[1]).AddDays(1);
                                }
                                sbWhere.AppendLine("AND " + filter.Field + ">? AND " + filter.Field + "<?");
                                paramList.Add(begin);
                                paramList.Add(end);
                                break;
                            case Enums.FieldDataType.文本:
                                switch (oper)
                                {
                                    case Enums.FilterOperator.包含:
                                        sbWhere.AppendLine("AND " + filter.Field + " LIKE '%" + filter.Value.Replace('\'', '"') + "%'");
                                        break;
                                    case Enums.FilterOperator.集合:
                                        string[] vstrs = filter.Value.Split(new char[] { ',', '，', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                        List<string> vstrl = new List<string>();
                                        foreach (var s in vstrs)
                                        {
                                            vstrl.Add("'" + s.Replace('\'', '"') + "'");
                                        }
                                        sbWhere.AppendLine("AND " + filter.Field + " IN (" + string.Join(",", vstrl) + ")");
                                        break;
                                    default:
                                        sbWhere.AppendLine("AND " + filter.Field + "=?");
                                        paramList.Add(Convert.ToString(filter.Value));
                                        break;
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("筛选项：" + fieldName + "出错：" + ex.ToString());
                }
                //if (filterCount > 0)  //为了性能，在没有筛选项的情况下直接添加排序子句，但原SQL中有ORDER BY的话会有BUG
                //{
                    sql = "SELECT * FROM (\r\n" + sql + "\r\n)\r\n" + sbWhere.ToString();
                    if (order != null && string.IsNullOrWhiteSpace(order.Field) == false)
                    {
                        sql += " ORDER BY " + order.Field + (order.IsDesc ? " DESC" : " ASC");
                    }
                //}
                //else
                //{
                //    if (order != null && string.IsNullOrWhiteSpace(order.Field) == false)
                //    {
                //        sql += "\r\nORDER BY " + order.Field + (order.IsDesc ? " DESC" : " ASC");
                //    }
                //}
            }
        }

        #region 返回sql查询的数据流
        /// <summary>
        /// 查询报表
        /// </summary>
        /// <param name="reportID">报表ID</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="count">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="inputJson">自定义输入项的值</param>
        /// <param name="whereJson">过滤字段</param>
        /// <returns></returns>
        public static IDataReader QueryDataReader(int reportID, out string sql, out List<object> paramList, out BDBHelper dbHelper, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            Entity entity = Instance.GetEntityByKey<Entity>(reportID);
            if (entity == null)
            {
                throw new Exception("报表" + reportID + "不存在");
            }
            return QueryDataReader(entity, out sql, out paramList, out dbHelper, getQueryString, inputJson, whereJson, order);
        }

        /// <summary>
        /// 查询报表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="count">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="getQueryString">URL中GET参数</param>
        /// <param name="inputJson">自定义输入项的值</param>
        /// <param name="whereJson">过滤字段</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public static IDataReader QueryDataReader(Entity entity, out string sql, out List<object> paramList, out BDBHelper dbHelper, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            sql = string.Empty;
            paramList = new List<object>();

            //DataTable dt = new DataTable();
            if (entity == null)
            {
                throw new Exception("报表不存在");
            }

            if (entity.IS_ENABLE != 1)
            {
                throw new Exception("报表" + entity.NAME + "已停用");
            }
            if (string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                throw new Exception("报表" + entity.NAME + "的SQL语句未配置");
            }

            Dictionary<string, string> dicInputValues = new Dictionary<string, string>();

            List<InputValueItem> inputList = null;
            if (string.IsNullOrWhiteSpace(inputJson) == false)
            {
                try
                {
                    inputList = JsonConvert.DeserializeObject<List<InputValueItem>>(inputJson);
                }
                catch
                {
                    throw new Exception("输入项错误");
                }
            }

            List<QueryFilterItem> filterList = null;
            if (string.IsNullOrWhiteSpace(whereJson) == false)
            {
                try
                {
                    filterList = JsonConvert.DeserializeObject<List<QueryFilterItem>>(whereJson);
                }
                catch
                {
                    throw new Exception("筛选项错误");
                }
            }

            try
            {
                TransSQL(entity.SQL_CODE, getQueryString, inputList, filterList, out sql, out paramList, order);
            }
            catch (Exception ex)
            {
                throw new Exception("转换SQL语句出错:" + ex);
            }

            return BF_DATABASE.Instance.ExecuteDataReader(entity.DB_ID, sql, paramList, out dbHelper);
        }

        #endregion

        #region 返回sql查询第一行第一列
        public static object QueryScalar(int reportID, out string sql, out List<object> paramList, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            Entity entity = Instance.GetEntityByKey<Entity>(reportID);
            if (entity == null)
            {
                throw new Exception("报表" + reportID + "不存在");
            }
            return QueryScalar(entity, out sql, out paramList, getQueryString, inputJson, whereJson, order);
        }

        /// <summary>
        /// 查询报表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="count">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="getQueryString">URL中GET参数</param>
        /// <param name="inputJson">自定义输入项的值</param>
        /// <param name="whereJson">过滤字段</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public static object QueryScalar(Entity entity, out string sql, out List<object> paramList, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            sql = string.Empty;
            paramList = new List<object>();

            //DataTable dt = new DataTable();
            if (entity == null)
            {
                throw new Exception("报表不存在");
            }

            if (entity.IS_ENABLE != 1)
            {
                throw new Exception("报表" + entity.NAME + "已停用");
            }
            if (string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                throw new Exception("报表" + entity.NAME + "的SQL语句未配置");
            }

            Dictionary<string, string> dicInputValues = new Dictionary<string, string>();

            List<InputValueItem> inputList = null;
            if (string.IsNullOrWhiteSpace(inputJson) == false)
            {
                try
                {
                    inputList = JsonConvert.DeserializeObject<List<InputValueItem>>(inputJson);
                }
                catch
                {
                    throw new Exception("输入项错误");
                }
            }

            List<QueryFilterItem> filterList = null;
            if (string.IsNullOrWhiteSpace(whereJson) == false)
            {
                try
                {
                    filterList = JsonConvert.DeserializeObject<List<QueryFilterItem>>(whereJson);
                }
                catch
                {
                    throw new Exception("筛选项错误");
                }
            }

            try
            {
                TransSQL(entity.SQL_CODE, getQueryString, inputList, filterList, out sql, out paramList, order);
            }
            catch (Exception ex)
            {
                throw new Exception("转换SQL语句出错:" + ex);
            }
            return BF_DATABASE.Instance.ExecuteScalar(entity.DB_ID, sql, paramList);
        }
        #endregion

        #region 返回sql查询的总量
        public static object QueryCount(int reportID, out string sql, out List<object> paramList, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            Entity entity = Instance.GetEntityByKey<Entity>(reportID);
            if (entity == null)
            {
                throw new Exception("报表" + reportID + "不存在");
            }
            return QueryCount(entity, out sql, out paramList, getQueryString, inputJson, whereJson, order);
        }

        /// <summary>
        /// 查询报表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="count">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="getQueryString">URL中GET参数</param>
        /// <param name="inputJson">自定义输入项的值</param>
        /// <param name="whereJson">过滤字段</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public static object QueryCount(Entity entity, out string sql, out List<object> paramList, string getQueryString, string inputJson = "", string whereJson = "", Order order = null)
        {
            sql = string.Empty;
            paramList = new List<object>();

            //DataTable dt = new DataTable();
            if (entity == null)
            {
                throw new Exception("报表不存在");
            }

            if (entity.IS_ENABLE != 1)
            {
                throw new Exception("报表" + entity.NAME + "已停用");
            }
            if (string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                throw new Exception("报表" + entity.NAME + "的SQL语句未配置");
            }

            Dictionary<string, string> dicInputValues = new Dictionary<string, string>();

            List<InputValueItem> inputList = null;
            if (string.IsNullOrWhiteSpace(inputJson) == false)
            {
                try
                {
                    inputList = JsonConvert.DeserializeObject<List<InputValueItem>>(inputJson);
                }
                catch
                {
                    throw new Exception("输入项错误");
                }
            }

            List<QueryFilterItem> filterList = null;
            if (string.IsNullOrWhiteSpace(whereJson) == false)
            {
                try
                {
                    filterList = JsonConvert.DeserializeObject<List<QueryFilterItem>>(whereJson);
                }
                catch
                {
                    throw new Exception("筛选项错误");
                }
            }

            try
            {
                TransSQL(entity.SQL_CODE, getQueryString, inputList, filterList, out sql, out paramList, order);
            }
            catch (Exception ex)
            {
                throw new Exception("转换SQL语句出错:" + ex);
            }
            sql = string.Format("SELECT COUNT(*) FROM ({0}) TTT",sql);
            return BF_DATABASE.Instance.ExecuteScalar(entity.DB_ID, sql, paramList);
        }
        #endregion

        #region 导出表格设置中文字段名

        /// <summary>
        /// 导出表格设置中文字段名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="table"></param>
        public void SetTableCaption(int id, DataTable table)
        {
            Entity entity = GetEntityByKey<Entity>(id);
            if (entity == null)
            {
                return;
            }
            List<BF_FIELD.Entity> listField = JSON.EncodeToEntity<List<BF_FIELD.Entity>>(entity.SHOW_FIELDS);
            Dictionary<string, BF_FIELD.Entity> dic = new Dictionary<string, BF_FIELD.Entity>();
            foreach (BF_FIELD.Entity field in listField)
            {
                if (field.IS_SHOW != 1)
                {
                    if (table.Columns.Contains(field.EN_NAME))
                    {
                        table.Columns.Remove(field.EN_NAME);
                    }
                }

                if (dic.ContainsKey(field.EN_NAME) == false)
                {
                    dic.Add(field.EN_NAME, field);
                }
            }

            foreach (DataColumn col in table.Columns)
            {
                if (dic.ContainsKey(col.ColumnName))
                {
                    col.Caption = dic[col.ColumnName].CN_NAME;
                }
            }
        }

        #endregion
    }
}