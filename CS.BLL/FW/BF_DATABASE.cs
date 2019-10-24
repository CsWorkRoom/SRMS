using CS.Base.DBHelper;
using CS.Common.FW;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 数据库管理
    /// </summary>
    public class BF_DATABASE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_DATABASE Instance = new BF_DATABASE();
        /// <summary>
        /// 默认数据库名称
        /// </summary>
        public const string DEFAULT_DB_NAME = "本地默认数据库";

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_DATABASE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_DATABASE";
            this.ItemName = "数据库管理";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
        }

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
            /// 数据库名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, IsIndex = true, IsIndexUnique = true, Comment = "数据库名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 数据库类型，对应枚举变量：DBType
            /// </summary>
            [Field(IsNotNull = true, Comment = "数据库类型，对应枚举变量：DBType")]
            public int DB_TYPE { get; set; }

            /// <summary>
            /// IP地址
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "IP地址")]
            public string IP { get; set; }

            /// <summary>
            /// 端口号
            /// </summary>
            [Field(IsNotNull = true, Comment = "端口号")]
            public int PORT { get; set; }

            /// <summary>
            /// 登录用户名
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "登录用户名")]
            public string USER_NAME { get; set; }

            /// <summary>
            /// 登录密码
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "登录密码")]
            public string PASSWORD { get; set; }

            /// <summary>
            /// 数据库名（或实例名）
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "数据库名（或实例名）")]
            public string DB_NAME { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注")]
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

        /// <summary>
        /// 根据数据库ID获取数据库实例
        /// </summary>
        /// <param name="id">数据库ID（如果为0，则为本地默认数据库）</param>
        /// <returns></returns>
        public Entity GetDbByID(int id)
        {
            if (id == 0)
            {
                Entity entity = new Entity();
                entity.ID = 0;
                entity.NAME = DEFAULT_DB_NAME;
                using (BDBHelper dbHelper = new BDBHelper())
                {
                    switch (dbHelper.DbType.ToLower())
                    {
                        case "oracle":
                            entity.DB_TYPE = Enums.DBType.Oracle.GetHashCode();
                            break;
                        case "db2":
                            entity.DB_TYPE = Enums.DBType.DB2.GetHashCode();
                            break;
                        case "vertica":
                            entity.DB_TYPE = Enums.DBType.Vertica.GetHashCode();
                            break;
                        case "gbase":
                            entity.DB_TYPE = Enums.DBType.GBase.GetHashCode();
                            break;
                    }
                    entity.IP = dbHelper.IP;
                    entity.PORT = dbHelper.Port;
                    entity.USER_NAME = dbHelper.UserName;
                    entity.PASSWORD = dbHelper.Password;
                    entity.DB_NAME = dbHelper.DataBase;

                    Base.Log.BLog.Write(Base.Log.BLog.LogLevel.DEBUG, string.Format("选择本地数据库：{0}:{1},{2},{3},{4}", entity.IP, entity.PORT, entity.USER_NAME, entity.PASSWORD, entity.DB_NAME));
                }
                return entity;
            }

            return GetEntityByKey<Entity>(id);
        }

        /// <summary>
        /// 根据别名获取数据库实例
        /// </summary>
        /// <param name="name">数据库名称（如果为空，则为本地默认数据库）</param>
        /// <returns></returns>
        public Entity GetDbByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name == DEFAULT_DB_NAME)
            {
                return GetDbByID(0);
            }
            return GetEntity<Entity>("NAME=?", name);
        }

        /// <summary>
        /// 返回数据库类型（字符串表示）
        /// </summary>
        /// <param name="typeID">类型ID</param>
        /// <returns></returns>
        public static string GetDbTypeName(int typeID)
        {
            try
            {
                Enums.DBType dbType = (Enums.DBType)typeID;
                return dbType.ToString();
            }
            catch
            {
                throw new Exception("错误的数据库类型：" + typeID);
            }
        }

        #region 返回可用数据库

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <returns>字典（键：数据库ID，值：数据库名称）</returns>
        public Dictionary<int, string> GetDictionary()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, DEFAULT_DB_NAME);
            Dictionary<int, string> d = GetDictionary("ID", "NAME");
            if (d != null)
            {
                foreach (var kvp in d)
                {
                    dic.Add(kvp.Key, kvp.Value);
                }
            }
            return dic;
        }

        #endregion

        #region 通用查询

        /// <summary>
        /// 获取数据库表列表
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="isUsedInImport">是否用于外导表</param>
        /// <returns>表名列表</returns>
        public List<string> GetTableList(int dbID, bool isUsedInImport)
        {
            List<string> list = new List<string>();
            string username = "";
            if (dbID < 0)
            {
                return new List<string>();
            }
            else if (dbID == 0)
            {
                using (BDBHelper dbHelper = new BDBHelper())
                {
                    username = dbHelper.UserName;
                    list = dbHelper.GetTablesList();
                }
            }
            else
            {
                BF_DATABASE.Entity entity = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(dbID);

                if (entity == null)
                {
                    throw new Exception("数据库" + dbID + "不存在");
                }
                string dbType = GetDbTypeName(entity.DB_TYPE);
                using (BDBHelper dbHelper = new BDBHelper(dbType, entity.IP, entity.PORT, entity.USER_NAME, entity.PASSWORD, entity.DB_NAME, entity.DB_NAME))
                {
                    username = dbHelper.UserName;
                    list = dbHelper.GetTablesList();
                }
            }

            if (list == null || list.Count < 1)
            {
                return new List<string>();
            }
            List<string> l = new List<string>();
            int i = 0;
            
            foreach (string t in list)
            {
                string name = t.Replace(username.ToUpper() + ".", "");
                //外导时，排除框架所用的表
                if (isUsedInImport == true && name.StartsWith("BF_"))
                {
                    //continue;
                }
                l.Add(name);
                i++;
                //最多返回200张表
                if (i >= 200 && isUsedInImport == false)
                {
                    break;
                }
            }
            return l;
        }
        /// <summary>
        /// 获取指定库的全量表集合
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
        public List<string> GetAllTableList(int dbID)
        {
            List<string> list = new List<string>();
            string username = "";
            if (dbID < 0)
            {
                return new List<string>();
            }
            else if (dbID == 0)
            {
                using (BDBHelper dbHelper = new BDBHelper())
                {
                    username = dbHelper.UserName;
                    list = dbHelper.GetTablesList();
                }
            }
            else
            {
                BF_DATABASE.Entity entity = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(dbID);

                if (entity == null)
                {
                    throw new Exception("数据库" + dbID + "不存在");
                }
                string dbType = GetDbTypeName(entity.DB_TYPE);
                using (BDBHelper dbHelper = new BDBHelper(dbType, entity.IP, entity.PORT, entity.USER_NAME, entity.PASSWORD, entity.DB_NAME, entity.DB_NAME))
                {
                    username = dbHelper.UserName;
                    list = dbHelper.GetTablesList();
                }
            }

            if (list == null || list.Count < 1)
            {
                return new List<string>();
            }
            else
            {
                return list;
            }
        }

        /// <summary>
        /// 获取数据表字段列表
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="tableName">表名</param>
        /// <returns>字段列表</returns>
        public List<BF_FIELD.Entity> GetFieldsList(int dbID, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new Exception("表名不可为空");
            }
            if (dbID < 0)
            {
                throw new Exception("不存在的数据库");
            }

            List<BF_FIELD.Entity> list = new List<BF_FIELD.Entity>();
            int rows = 0;
            DataTable dt = ExecuteSelectSQL(dbID, "SELECT * FROM " + tableName + " WHERE 1=0", null, ref rows, 0, 1);
            if (dt != null)
            {
                IList<BF_FIELD.Entity> configList = BF_FIELD.Instance.GetList<BF_FIELD.Entity>();
                Dictionary<string, BF_FIELD.Entity> dic = new Dictionary<string, BF_FIELD.Entity>();
                if (configList != null)
                {
                    foreach (BF_FIELD.Entity cf in configList)
                    {
                        if (dic.ContainsKey(cf.EN_NAME.ToUpper()) == false)
                        {
                            dic.Add(cf.EN_NAME.ToUpper(), cf);
                        }
                    }
                }

                foreach (DataColumn col in dt.Columns)
                {
                    string enName = col.ColumnName.ToUpper();
                    if (dic.ContainsKey(enName))
                    {
                        list.Add(dic[enName]);
                    }
                    else
                    {
                        BF_FIELD.Entity field = new BF_FIELD.Entity();
                        field.EN_NAME = enName;
                        field.CN_NAME = enName;
                        if (col.DataType == typeof(string))
                        {
                            field.FIELD_DATA_TYPE = (short)Enums.FieldDataType.文本;
                        }
                        else if (col.DataType == typeof(DateTime))
                        {
                            field.FIELD_DATA_TYPE = (short)Enums.FieldDataType.日期;
                        }
                        else
                        {
                            field.FIELD_DATA_TYPE = (short)Enums.FieldDataType.数值;
                        }
                        field.IS_SHOW = 1;
                        field.SHOW_WIDTH = 80;
                        list.Add(field);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取一个数据库操作对象
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
        private BDBHelper GetBDBHelper(int dbID)
        {
            if (dbID == 0)
            {
                return new BDBHelper();
            }
            else
            {
                Entity db = GetEntityByKey<Entity>(dbID);
                if (db == null)
                {
                    throw new Exception("数据库" + dbID + "不存在");
                }
                string dbType = GetDbTypeName(db.DB_TYPE);
                return new BDBHelper(dbType, db.IP, db.PORT, db.USER_NAME, db.PASSWORD, db.DB_NAME, db.DB_NAME);
            }
        }

        /// <summary>
        /// 在指定数据库上执行SQL语句
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(int dbID, string sql, List<object> paramList)
        {
            using (BDBHelper dbHelper = GetBDBHelper(dbID))
            {
                try
                {
                    if (paramList == null || paramList.Count < 1)
                    {
                        return dbHelper.ExecuteNonQuery(sql);
                    }
                    else
                    {
                        return dbHelper.ExecuteNonQueryParams(sql, paramList);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("在数据库" + dbID + "执行SQL语句出错：" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 在指定数据库执行查询语句
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="pageIndex">页号（从1开始）</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteSelectSQL(int dbID, string sql, List<object> paramList, int pageSize = 10, int pageIndex = 1)
        {
            int rowsCount = -1;
            return ExecuteSelectSQL(dbID, sql, paramList, ref rowsCount, pageSize, pageIndex);
        }

        /// <summary>
        /// 在指定数据库执行查询语句
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="rowsCount">记录数（如果传入值等于0，则会重新计算此值，反之不计算）</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="pageIndex">页号（从1开始）</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteSelectSQL(int dbID, string sql, List<object> paramList, ref int rowsCount, int pageSize = 10, int pageIndex = 1)
        {
            if (dbID < 0)
            {
                throw new Exception("错误的数据库ID");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new Exception("SQL语句不可为空");
            }
            string s = sql.Trim();
            if (s.ToUpper().StartsWith("SELECT ") == false)
            {
                throw new Exception("只能执行SLECT语句！");
            }

            DataTable dt = null;

            using (BDBHelper dbHelper = GetBDBHelper(dbID))
            {
                try
                {
                    if (pageSize <= 0)
                    {
                        if (paramList == null || paramList.Count < 1)
                        {
                            dt = dbHelper.ExecuteDataTable(sql);
                        }
                        else
                        {
                            dt = dbHelper.ExecuteDataTableParams(sql, paramList);
                        }
                        if (dt != null)
                        {
                            rowsCount = dt.Rows.Count;
                        }
                    }
                    else
                    {
                        if (paramList == null || paramList.Count < 1)
                        {
                            dt = dbHelper.ExecuteDataTablePage(sql, pageSize, pageIndex);
                        }
                        else
                        {
                            dt = dbHelper.ExecuteDataTablePageParams(sql, pageSize, pageIndex, paramList);
                        }
                        if (dt != null && dt.Rows.Count < pageSize && pageIndex <= 1)
                        {
                            rowsCount = dt.Rows.Count;
                        }
                        else if (rowsCount == 0)
                        {
                            if (paramList == null || paramList.Count < 1)
                            {
                                rowsCount = dbHelper.ExecuteScalarInt("SELECT COUNT(*) C FROM (" + sql + ")");
                            }
                            else
                            {
                                rowsCount = dbHelper.ExecuteScalarIntParams("SELECT COUNT(*) C FROM (" + sql + ")", paramList);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("在数据库" + dbID + "执行SQL查询出错：" + ex.Message);
                }
            }

            if (pageIndex <= 1 && (dt == null || dt.Rows.Count == 0))
            {
                rowsCount = 0;
            }

            return dt;
        }

        public IDataReader ExecuteDataReader(int dbID, string sql, List<object> paramList, out BDBHelper dbHelper)
        {
            if (dbID < 0)
            {
                throw new Exception("错误的数据库ID");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new Exception("SQL语句不可为空");
            }
            string s = sql.Trim();
            if (s.ToUpper().StartsWith("SELECT ") == false)
            {
                throw new Exception("只能执行SLECT语句！");
            }

            dbHelper = GetBDBHelper(dbID);
            try
            {
                if (paramList == null || paramList.Count < 1)
                {
                    return dbHelper.ExecuteReader(sql);
                }
                else
                {
                    return dbHelper.ExecuteReaderParams(sql, paramList);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在数据库" + dbID + "执行SQL查询出错：" + ex.Message);
            }
        }
        
        /// <summary>
        /// 在指定数据库执行查询语句
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="psd">二次密码验证（执行DDL语句时传入）</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <returns>DataTable</returns>
        public int ExecuteSQL(int dbID, string psd, string sql, List<object> paramList)
        {
            int i = 0;
            if (dbID < 0)
            {
                throw new Exception("错误的数据库ID");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new Exception("SQL语句不可为空");
            }
            string s = sql.Trim();
            if (s.ToUpper().StartsWith("SELECT ") == true)
            {
                throw new Exception("只能执行非SELECT语句！");
            }

            if (psd != "lb@em")
            {
                throw new Exception("二次验证密码不正确，非SELECT语句需要二次验证密码！");
            }

            using (BDBHelper dbHelper = GetBDBHelper(dbID))
            {
                try
                {

                    if (paramList == null || paramList.Count < 1)
                    {
                        i = dbHelper.ExecuteNonQuery(sql);
                    }
                    else
                    {
                        i = dbHelper.ExecuteNonQueryParams(sql, paramList);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("在数据库" + dbID + "执行SQL查询出错：" + ex.Message);
                }
            }

            return i;
        }
        /// <summary>
        /// 执行一条SQL语句，返回第一行第一列object
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(int dbID,string sql)
        {
            object obj = 0;

            if (dbID < 0)
            {
                throw new Exception("错误的数据库ID");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new Exception("SQL语句不可为空");
            }
           

            using (BDBHelper dbHelper = GetBDBHelper(dbID))
            {
                try
                {
                    obj= dbHelper.ExecuteScalar(sql);
                }
                catch (Exception ex)
                {
                    throw new Exception("在数据库" + dbID + "执行SQL查询出错：" + ex.Message);
                }
            }

            return obj;
        }

        public object ExecuteScalar(int dbID, string sql, List<object> paramList)
        {
            object obj = 0;

            if (dbID < 0)
            {
                throw new Exception("错误的数据库ID");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new Exception("SQL语句不可为空");
            }


            using (BDBHelper dbHelper = GetBDBHelper(dbID))
            {
                try
                {
                    obj = dbHelper.ExecuteScalarParams(sql, paramList);
                }
                catch (Exception ex)
                {
                    throw new Exception("在数据库" + dbID + "执行SQL查询出错：" + ex.Message);
                }
            }

            return obj;
        }
        #endregion
    }
}