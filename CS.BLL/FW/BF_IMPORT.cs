using CS.Base.DBHelper;
using CS.Base.Log;
using CS.Common.FW;
using CS.Library.BaseQuery;
using CS.Library.Export;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 数据外导
    /// </summary>
    public class BF_IMPORT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_IMPORT Instance = new BF_IMPORT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_IMPORT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_IMPORT";
            this.ItemName = "数据外导";
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
            /// 外导名称
            /// </summary>
            [Field(IsNotNull = true, Length = 64, IsIndex = true, IsIndexUnique = true, Comment = "外导名称")]
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
            /// 导入时是否允许更新数据
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "导入时是否允许更新数据")]
            public Int16 IS_ALLOW_UPDATE { get; set; }

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
            /// 是否需要导入该字段
            /// </summary>
            public int IS_IMPORT { get; set; }

            /// <summary>
            /// 是否为自增长字段
            /// </summary>
            public int IS_AUTO_INCREMENT { get; set; }

            /// <summary>
            /// 该字段是否为唯一约束
            /// </summary>
            public int IS_UNIQUE { get; set; }

            /// <summary>
            /// 字段说明
            /// </summary>
            public string COMMENT { get; set; }
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
        /// 获取配置中的字段信息
        /// </summary>
        /// <param name="fields">字段信息（JSON串）</param>
        /// <returns></returns>
        public static Dictionary<string, FieldInfo> GetFieldsInfo(string fields)
        {
            Dictionary<string, FieldInfo> dic = new Dictionary<string, FieldInfo>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                return dic;
            }

            List<FieldInfo> list = JsonConvert.DeserializeObject<List<FieldInfo>>(fields);
            if (list != null)
            {
                foreach (FieldInfo field in list)
                {
                    if (dic.ContainsKey(field.EN_NAME.ToUpper()) == false)
                    {
                        dic.Add(field.EN_NAME.ToUpper(), field);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取样表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="commentList"></param>
        /// <returns></returns>
        public static DataTable GetSampleTable(Entity entity, out List<string> commentList)
        {
            commentList = new List<string>();
            if (entity == null)
            {
                return new DataTable();
            }
            string sql = "SELECT * FROM " + entity.TABLE_NAME + " WHERE 1=0";
            int rc = 0;
            DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(entity.DB_ID, sql, null, ref rc, 0, 1);
            if (dt == null)
            {
                return new DataTable();
            }
            Dictionary<string, FieldInfo> dicFields = GetFieldsInfo(entity.FIELDS);
            List<string> ignores = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                string colName = col.ColumnName.ToUpper();
                if (dicFields.ContainsKey(colName))
                {
                    if (dicFields[colName].IS_IMPORT == 0)
                    {
                        ignores.Add(colName);
                        continue;
                    }
                    col.Caption = dicFields[colName].CN_NAME;
                    commentList.Add(dicFields[colName].COMMENT);
                }
            }

            //移除不用导入的字段
            foreach (string ignore in ignores)
            {
                if (dt.Columns.Contains(ignore))
                {
                    dt.Columns.Remove(ignore);
                }
            }

            return dt;
        }

        /// <summary>
        /// 导入数据文件
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="fileName">文件名</param>
        /// <param name="baseDate">基准日期</param>
        /// <param name="message">返回信息</param>
        /// <param name="errorList">错误信息（键：行号，值：错误信息）</param>
        /// <returns></returns>
        public static int ImportDataFile(Entity entity, string fileName, DateTime baseDate, out string message, out Dictionary<int, string> errorList)
        {
            int n = 0;
            int loadCount = 0;
            int updateCount = 0;
            message = string.Empty;
            errorList = new Dictionary<int, string>();
            try
            {
                if (entity == null)
                {
                    message = "配置不存在";
                    return n;
                }

                if (File.Exists(fileName) == false)
                {
                    message = "文件" + fileName + "不存在";
                    return n;
                }

                string tableName = entity.TABLE_NAME;
                switch (entity.CREATE_TABLE_MODE)
                {
                    case (short)Enums.CreateTableMode.年份后缀:
                        tableName += "_" + baseDate.ToString("yyyy");
                        break;
                    case (short)Enums.CreateTableMode.年月后缀:
                        tableName += "_" + baseDate.ToString("yyyyMM");
                        break;
                    case (short)Enums.CreateTableMode.年月日后缀:
                        tableName += "_" + baseDate.ToString("yyyyMMdd");
                        break;
                    case (short)Enums.CreateTableMode.用户ID后缀:
                        tableName += "_" + SystemSession.UserID;
                        break;
                }

                string sqlQuery = "SELECT * FROM {0} WHERE 1=0";
                string sqlUnique = "SELECT {0} FROM {1}";
                //字段配置
                Dictionary<string, FieldInfo> dicFields = GetFieldsInfo(entity.FIELDS);
                //自增长主键
                string autoIncrementField = string.Empty;
                //忽略的字段
                Dictionary<string, bool> dicIgnore = new Dictionary<string, bool>();
                //唯一约束字段
                string uniqueField = string.Empty;
                //唯一约束的值
                Dictionary<string, bool> dicUniqueValues = new Dictionary<string, bool>();

                foreach (var kvp in dicFields)
                {
                    if (kvp.Value.IS_AUTO_INCREMENT == 1)
                    {
                        autoIncrementField = kvp.Key;
                    }
                    if (kvp.Value.IS_IMPORT == 0)
                    {
                        dicIgnore.Add(kvp.Key, true);
                    }
                    if (kvp.Value.IS_UNIQUE == 1)
                    {
                        uniqueField = kvp.Key;
                    }
                }

                BDBHelper dbHelper = null;
                try
                {
                    if (entity.DB_ID == 0)
                    {
                        dbHelper = new BDBHelper();
                    }
                    else
                    {
                        BF_DATABASE.Entity db = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(entity.DB_ID);
                        if (db == null)
                        {
                            message = "数据库" + entity.DB_ID + "不存在";
                            return n;
                        }

                        string dbType = BF_DATABASE.GetDbTypeName(db.DB_TYPE);
                        dbHelper = new BDBHelper(dbType, db.IP, db.PORT, db.USER_NAME, db.PASSWORD, db.DB_NAME, db.DB_NAME);
                    }
                    //表不存在
                    if (entity.CREATE_TABLE_MODE != (short)Enums.CreateTableMode.指定表 && dbHelper.TableIsExists(tableName) == false)
                    {
                        //创建表
                        if (dbHelper.CreateTable(tableName, entity.TABLE_NAME) == false)
                        {
                            message = "创建表" + tableName + "失败";
                            return n;
                        }
                        //设置自增长
                        if (string.IsNullOrWhiteSpace(autoIncrementField) == false)
                        {
                            if (dbHelper.SetAutoIncrement(tableName, autoIncrementField) == false)
                            {
                                message = "设置表" + tableName + "的字段" + autoIncrementField + "为自增长失败";
                                return n;
                            }
                        }
                    }

                    //查询需要导入的字段
                    DataTable dtQuery = dbHelper.ExecuteDataTable(string.Format(sqlQuery, tableName));

                    if (dtQuery == null)
                    {
                        message = "无法查询导入的目标表" + tableName;
                        return n;
                    }
                    //是否包含导入用户ID
                    bool isContainsUid = dtQuery.Columns.Contains("IMPORT_UID");
                    //是否包含导入时间
                    bool isContainsTime = dtQuery.Columns.Contains("IMPORT_TIME");

                    //读取Excel中的数据
                    DataTable tdExcel = new DataTable();
                    foreach (DataColumn col in dtQuery.Columns)
                    {
                        if (dicIgnore.ContainsKey(col.ColumnName.ToUpper()))
                        {
                            continue;
                        }
                        tdExcel.Columns.Add(col.ColumnName, col.DataType);
                    }
                    //读取数据到DataTable
                    if (LoadFileIntoDataTable(fileName, ref tdExcel, out message) == false)
                    {
                        return 0;
                    }
                    if (tdExcel.Rows.Count < 1)
                    {
                        message = "没有数据";
                        return 0;
                    }

                    //要导入的数据
                    DataTable tdLoad = new DataTable();
                    DataTable tdUpdate = new DataTable();
                    foreach (DataColumn colExcel in tdExcel.Columns)
                    {
                        tdLoad.Columns.Add(colExcel.ColumnName.ToUpper(), colExcel.DataType);
                        tdUpdate.Columns.Add(colExcel.ColumnName.ToUpper(), colExcel.DataType);
                    }
                    if (isContainsUid == true)
                    {
                        tdLoad.Columns.Add("IMPORT_UID", typeof(int));
                        tdUpdate.Columns.Add("IMPORT_UID", typeof(int));
                    }
                    if (isContainsTime == true)
                    {
                        tdLoad.Columns.Add("IMPORT_TIME", typeof(DateTime));
                        tdUpdate.Columns.Add("IMPORT_TIME", typeof(DateTime));
                    }
                    tdUpdate.Columns.Add("EXCEL_ROW_NUM", typeof(int));

                    //唯一约束
                    bool isCheckUnique = false;
                    if (string.IsNullOrWhiteSpace(uniqueField) == false)
                    {
                        sqlUnique = string.Format(sqlUnique, uniqueField, tableName);
                        DataTable tdUnique = dbHelper.ExecuteDataTable(sqlUnique);
                        if (tdUnique != null)
                        {
                            foreach (DataRow rowUnique in tdUnique.Rows)
                            {
                                string key = Convert.ToString(rowUnique[0]);
                                if (dicUniqueValues.ContainsKey(key) == false)
                                {
                                    dicUniqueValues.Add(key, true);
                                    isCheckUnique = true;
                                }
                            }
                        }
                    }

                    //将数据分别写入待导入及待更新
                    int excelRowNumber = 1;
                    //本批次数据重复性验证
                    bool isContainsUniqueField = tdExcel.Columns.Contains(uniqueField);
                    Dictionary<string, bool> dicTemp = new Dictionary<string, bool>();
                    List<int> errorTemp = new List<int>();
                    DateTime importTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    foreach (DataRow dr in tdExcel.Rows)
                    {
                        excelRowNumber++;
                        string key = string.Empty;
                        if (string.IsNullOrWhiteSpace(uniqueField) == false && isContainsUniqueField == true)
                        {
                            key = Convert.ToString(dr[uniqueField]);
                            //不可为空
                            if (string.IsNullOrWhiteSpace(key) == true)
                            {
                                errorTemp.Add(excelRowNumber);
                            }

                            //本批次要导入的数据重复
                            if (dicTemp.ContainsKey(key))
                            {
                                errorTemp.Add(excelRowNumber);
                                continue;
                            }
                            dicTemp.Add(key, true);

                            //和数据库里面原有数据比较
                            if (isCheckUnique == true)
                            {
                                //违反唯一约束
                                if (dicUniqueValues.ContainsKey(key))
                                {
                                    DataRow updateRow = tdUpdate.NewRow();
                                    foreach (DataColumn updateCol in tdExcel.Columns)
                                    {
                                        updateRow[updateCol.ColumnName] = dr[updateCol];
                                    }
                                    if (isContainsUid == true)
                                    {
                                        updateRow["IMPORT_UID"] = SystemSession.UserID;
                                    }
                                    if (isContainsTime == true)
                                    {
                                        updateRow["IMPORT_TIME"] = importTime;
                                    }
                                    updateRow["EXCEL_ROW_NUM"] = excelRowNumber;
                                    tdUpdate.Rows.Add(updateRow);
                                    errorList.Add(excelRowNumber, "数据库中已经存在值：" + key);
                                    continue;
                                }
                            }
                        }

                        //加入待插入
                        DataRow insertRow = tdLoad.NewRow();
                        foreach (DataColumn insertCol in tdExcel.Columns)
                        {
                            insertRow[insertCol.ColumnName] = dr[insertCol];
                        }
                        if (isContainsUid == true)
                        {
                            insertRow["IMPORT_UID"] = SystemSession.UserID;
                        }
                        if (isContainsTime == true)
                        {
                            insertRow["IMPORT_TIME"] = importTime;
                        }
                        tdLoad.Rows.Add(insertRow);
                    }

                    //导入数据
                    if (tdLoad.Rows.Count > 0)
                    {
                        try
                        {

                            loadCount = dbHelper.LoadDataInDataTable(tableName, tdLoad);
                            message = string.Format("共导入{0}条数据", loadCount);
                        }
                        catch (Exception el)
                        {
                            message = string.Format("尝试导入{0}条数据出错：{1}", tdLoad.Rows.Count, el.Message);
                        }
                    }

                    //更新数据
                    if (tdUpdate.Rows.Count > 0)
                    {
                        if (entity.IS_ALLOW_UPDATE == 1)
                        {
                            errorList = new Dictionary<int, string>();
                            string updateFields = string.Empty;
                            int ui = 0;
                            foreach (DataColumn col in tdUpdate.Columns)
                            {
                                if (col.ColumnName == uniqueField || col.ColumnName == "EXCEL_ROW_NUM")
                                {
                                    continue;
                                }
                                if (ui == 0)
                                {
                                    updateFields += col.ColumnName + "=?";
                                }
                                else
                                {
                                    updateFields += " ," + col.ColumnName + "=?";
                                }
                                ui++;
                            }

                            string sqlUpdate = string.Format("UPDATE {0} SET {1} WHERE {2}=?", tableName, updateFields, uniqueField);

                            foreach (DataRow drUpdate in tdUpdate.Rows)
                            {
                                List<object> values = new List<object>();
                                foreach (DataColumn col in tdUpdate.Columns)
                                {
                                    if (col.ColumnName == uniqueField || col.ColumnName == "EXCEL_ROW_NUM")
                                    {
                                        continue;
                                    }
                                    values.Add(drUpdate[col]);
                                }
                                values.Add(drUpdate[uniqueField]);

                                if (dbHelper.ExecuteNonQueryParams(sqlUpdate, values) > 0)
                                {
                                    updateCount++;
                                }
                                else
                                {
                                    errorList.Add(Convert.ToInt32(drUpdate["EXCEL_ROW_NUM"]), "更新数据失败");
                                }
                            }
                        }
                    }

                    foreach (int row in errorTemp)
                    {
                        errorList.Add(row, "表格中数据重复");
                    }

                    if (updateCount > 0)
                    {
                        message += "，更新" + updateCount + "条";
                    }
                    if (errorList.Count > 0)
                    {
                        message += "，失败" + errorList.Count + "条";
                    }
                }
                catch (Exception e)
                {
                    message += "导入数据出错：" + e.Message;
                    BLog.Write(BLog.LogLevel.WARN, "外导数据出错：" + e.ToString());
                    return loadCount + updateCount;
                }
                finally
                {
                    if (dbHelper != null)
                    {
                        dbHelper.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                message += "导入数据出错：" + ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "外导数据出错：" + ex.ToString());
                return loadCount + updateCount;
            }

            return loadCount + updateCount;
        }

        /// <summary>
        /// 加载文件到DataTable
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="errmsg">错误信息</param>
        private static bool LoadFileIntoDataTable(string fileName, ref DataTable dataTable, out string errmsg)
        {
            errmsg = "";
            try
            {
                FileInfo fi = new FileInfo(fileName);
                ExcelFile excel = new ExcelFile(fi.DirectoryName);
                excel.ToDataTable(fi.Name, ref dataTable, 0);
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return false;
        }
    }
}
