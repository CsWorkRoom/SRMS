using CS.Base.DBHelper;
using CS.Library.BaseQuery;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 表格报表
    /// </summary>
    public class BF_CHART_REPORT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_CHART_REPORT Instance = new BF_CHART_REPORT();

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_CHART_REPORT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_CHART_REPORT";
            this.ItemName = "图形报表";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
        }
        #endregion

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
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "图形报表名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 数据库ID（为0时表示默认数据库）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "数据库ID（为0时表示默认数据库）")]
            public int DB_ID { get; set; }

            /// <summary>
            /// 图表类型
            /// </summary>
            [Field(Comment = "图表类型")]
            public int CHART_TYPE { get; set; }

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
            /// SQL语句
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "SQL语句")]
            public string SQL_CODE { get; set; }

            /// <summary>
            /// 图表代码体
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "图表代码体")]
            public string CHART_CODE { get; set; }

            /// <summary>
            /// 表格上方的代码（允许HTML、CSS及JS）
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "表格上方的代码（允许HTML、CSS及JS）")]
            public string TOP_CODE { get; set; }

            /// <summary>
            /// 表格下方的代码（允许HTML、CSS及JS）
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "表格下方的代码（允许HTML、CSS及JS）")]
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

        #region 启用
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
        #endregion

        #region 禁用
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
        #endregion

        #region 更新显示字段配置
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
        #endregion

        #region 查询筛选
        /// <summary>
        /// 查询筛选
        /// </summary>
        public class QueryFilter
        {
            /// <summary>
            /// 字段名
            /// </summary>
            public string Field { get; set; }
            /// <summary>
            /// 字段数据类型
            /// </summary>
            public int FieldDataType { get; set; }
            /// <summary>
            /// 运算逻辑
            /// </summary>
            public int FilterOperator { get; set; }
            /// <summary>
            /// 值
            /// </summary>
            public string Value { get; set; }
        }
        #endregion

        #region 生成SQL语句
        /// <summary>
        /// 生成SQL语句
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dicInputValues">输入项的值</param>
        /// <param name="paramList">参数列表</param>
        /// <returns></returns>
        public static string GenerateSQL(Entity entity, Dictionary<string, string> dicInputValues, out List<object> paramList)
        {
            paramList = new List<object>();
            if (entity == null || string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                return string.Empty;
            }

            List<BF_CHART_REPORT_FILTER.Entity> filterList = new List<BF_CHART_REPORT_FILTER.Entity>();
            if (dicInputValues != null && dicInputValues.Count > 0)
            {
                filterList = BF_CHART_REPORT_FILTER.Instance.GetFilterList(entity.ID);
            }

            //执行函数
            string sql = Common.FW.Functions.Instance.ExecuteFunction(entity.SQL_CODE);
            //替换SESSION变量
            sql = SystemSession.TransParams(sql);

            //替换输入项
            Regex regex = new Regex(@"@\((?<name>[a-z_0-9\-]+)\)", RegexOptions.IgnoreCase);
            Match match = regex.Match(sql);
            while (match != null && match.Success)
            {
                string name = match.Result("${name}");
                if (dicInputValues != null && dicInputValues.ContainsKey(name))
                {
                    sql = sql.Replace("@(" + name + ")", dicInputValues[name]);
                }
                else
                {
                    sql = sql.Replace("@(" + name + ")", "0");
                }

                match = match.NextMatch();
            }

            //替换筛选项
            StringBuilder sbWhere = new StringBuilder();
            sbWhere.AppendLine("WHERE 1=1");
            int fc = 0;
            foreach (BF_CHART_REPORT_FILTER.Entity fe in filterList)
            {
                if (dicInputValues.ContainsKey(fe.FIELD_NAME) == false)
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(dicInputValues[fe.FIELD_NAME]))
                {
                    continue;
                }
                fc++;

                string v1 = dicInputValues[fe.FIELD_NAME].Trim();
                string v2 = string.Empty;

                Enums.FilterOperator oper = (Enums.FilterOperator)fe.FILTER_OPERATOR;
                if (oper == Enums.FilterOperator.介于)
                {
                    string[] vs = v1.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    v1 = vs[0].Trim();
                    if (vs.Length > 1)
                    {
                        v2 = vs[1].Trim();
                    }
                    else
                    {
                        v2 = v1;
                    }
                }
                else
                {
                    v2 = v1;
                }

                Enums.FieldDataType dataType = (Enums.FieldDataType)fe.FIELD_DATA_TYPE;
                switch (dataType)
                {
                    case Enums.FieldDataType.数值:
                        if (oper == Enums.FilterOperator.介于)
                        {
                            sbWhere.AppendLine("AND " + fe.FIELD_NAME + ">=?");
                            sbWhere.AppendLine("AND " + fe.FIELD_NAME + "<=?");
                            paramList.Add(Convert.ToDecimal(v1));
                            paramList.Add(Convert.ToDecimal(v2));
                        }
                        else
                        {
                            sbWhere.AppendLine("AND " + fe.FIELD_NAME + "=?");
                            paramList.Add(Convert.ToDecimal(v1));
                        }
                        break;
                    case Enums.FieldDataType.日期:
                        sbWhere.AppendLine("AND " + fe.FIELD_NAME + ">=?");
                        sbWhere.AppendLine("AND " + fe.FIELD_NAME + "<?");
                        paramList.Add(Convert.ToDateTime(v1).Date);
                        paramList.Add(Convert.ToDateTime(v2).AddDays(1).Date);
                        break;
                    case Enums.FieldDataType.文本:
                        if (oper == Enums.FilterOperator.包含)
                        {
                            sbWhere.AppendLine("AND " + fe.FIELD_NAME + " LIKE '%" + v1.Replace('\'', '"') + "%'");
                        }
                        else
                        {
                            sbWhere.AppendLine("AND " + fe.FIELD_NAME + "=?");
                            paramList.Add(v1);
                        }
                        break;
                }
            }

            if (fc > 0)
            {
                sql = "SELECT * FROM (\r\n" + sql + "\r\n)\r\n" + sbWhere.ToString();
            }

            return sql;
        }
        #endregion

        #region 查询报表并返回结果集
        /// <summary>
        /// 查询报表并返回结果集
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="generatedSQL">解析后的SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static DataTable QueryTable(Entity entity, string generatedSQL, List<object> paramList, int pageSize, int pageIndex)
        {
            if (entity == null || string.IsNullOrWhiteSpace(generatedSQL))
            {
                return new DataTable();
            }

            string sql = generatedSQL;

            if (entity.DB_ID <= 0)
            {
                using (BDBHelper dbHelper = new BDBHelper())
                {
                    if (paramList == null || paramList.Count < 1)
                    {
                        if (pageSize < 1)
                        {
                            return dbHelper.ExecuteDataTable(sql);
                        }
                        return dbHelper.ExecuteDataTablePage(sql, pageSize, pageIndex);
                    }
                    if (pageSize < 1)
                    {
                        return dbHelper.ExecuteDataTableParams(sql, paramList);
                    }
                    return dbHelper.ExecuteDataTablePageParams(sql, pageSize, pageIndex, paramList);
                }
            }
            else
            {
                BF_DATABASE.Entity db = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(entity.DB_ID);
                if (db == null)
                {
                    throw new Exception("数据库不存在");
                }

                string dbType = Enums.DBType.Oracle.ToString();
                try
                {
                    dbType = ((Enums.DBType)db.DB_TYPE).ToString();
                }
                catch
                {
                    throw new Exception("未知的数据库类型");
                }

                using (BDBHelper dbHelper = new BDBHelper(dbType, db.IP, db.PORT, db.USER_NAME, db.PASSWORD, db.DB_NAME, db.DB_NAME))
                {
                    if (paramList == null || paramList.Count < 1)
                    {
                        if (pageSize < 1)
                        {
                            return dbHelper.ExecuteDataTable(sql);
                        }

                        return dbHelper.ExecuteDataTablePage(sql, pageSize, pageIndex);
                    }

                    if (pageSize < 1)
                    {
                        return dbHelper.ExecuteDataTableParams(sql, paramList);
                    }

                    return dbHelper.ExecuteDataTablePageParams(sql, pageSize, pageIndex, paramList);
                }
            }
        }
        #endregion

        #region 查询列表
        public DataTable GetDataTable(int limit, int page, ref int count, string name, string orderByField = "CR.ID", string orderByType = "DESC")
        {
            string strWhere = "1=1";
            List<object> param = new List<object>();
            #region 添加参数
            if (string.IsNullOrWhiteSpace(name) == false)
                strWhere += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            #endregion

            string strSql = "SELECT CR.ID,CR.NAME,DB.NAME DBNAME,CHART_TYPE,(CASE IS_SHOW_EXPORT WHEN 1 THEN '是' ELSE '否' END )SHOWEXPORT,(CASE IS_SHOW_DEBUG WHEN 1 THEN '是' ELSE '否' END )SHOWDEBUG,SQL_CODE,(CASE IS_ENABLE WHEN 1 THEN '是' ELSE '否' END )IS_ENABLE,CR.CREATE_TIME,CR.update_time FROM BF_CHART_REPORT CR LEFT JOIN BF_DATABASE DB on CR.DB_ID=DB.ID WHERE " + strWhere;
            //添加排序
            if (string.IsNullOrWhiteSpace(orderByField) == false)
                strSql += " ORDER BY " + orderByField + " " + (string.IsNullOrWhiteSpace(orderByType) == false ? orderByType : "");

            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0 && page == 0)
                {
                    return dbHelper.ExecuteDataTableParams(strSql);//不分页查询所有
                }
                //算总记录
                if (count == 0)
                {
                    string sqlCount = string.Format("SELECT COUNT(*) FROM ({0})", strSql);
                    count = dbHelper.ExecuteScalarIntParams(sqlCount, param);
                }
                return dbHelper.ExecuteDataTablePageParams(strSql, limit, page, param);
            }

        }
        #endregion

    }
}
