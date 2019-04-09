using CS.Base.DBHelper;
using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using CS.Library.Compress;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS.ScriptService.Script
{
    /// <summary>
    /// 脚本执行基类，包含了对界面自定义函数的定义与实现。
    /// 生成的脚本继承自本类
    /// </summary>
    public class Base
    {
        #region 私有变量定义

        /// <summary>
        /// 任务详情ID
        /// </summary>
        private int _tfnID = 0;
        /// <summary>
        /// 任务ID
        /// </summary>
        private int _taskID = 0;
        /// <summary>
        /// 脚本ID
        /// </summary>
        private int _flowID = 0;
        /// <summary>
        /// 节点ID
        /// </summary>
        private int _nodeID = 0;
        /// <summary>
        /// 执行基准时间
        /// </summary>
        private DateTime _referenceDateTime = DateTime.Now;
        /// <summary>
        /// 脚本执行中是否有错（如果有错，则提前中止）
        /// </summary>
        private bool _isError = false;
        /// <summary>
        /// 错误信息
        /// </summary>
        private string _errorMessage = string.Empty;
        /// <summary>
        /// 数据库访问实体
        /// </summary>
        private BF_DATABASE.Entity _dbServer;
        /// <summary>
        /// 下载文件的临时文件夹
        /// </summary>
        protected string _downDbPath = AppDomain.CurrentDomain.BaseDirectory + "\\UpFiles\\DownDB\\";
        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tfnID">TFNID</param>
        /// <param name="taskID">任务ID</param>
        /// <param name="flowID">脚本ID</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="defaultDbID">默认数据库ID</param>
        public void Init(int tfnID, int taskID, int flowID, int nodeID)
        {
            _tfnID = tfnID;
            _taskID = taskID;
            _flowID = flowID;
            _nodeID = nodeID;
        }

        /// <summary>
        /// 设置基准时间
        /// </summary>
        /// <param name="referenceDateTime">基准时间</param>
        public void SetReferenceDateTime(DateTime referenceDateTime)
        {
            _referenceDateTime = referenceDateTime;
        }

        #endregion

        #region 支持的方法

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrorMessage()
        {
            return _errorMessage;
        }

        /// <summary>
        /// 写错误信息
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="logLevel">日志等级</param>
        /// <param name="sql">SQL语句或脚本</param>
        protected void WriteErrorMessage(string errorMessage, int logLevel = 3, string sql = "")
        {
            _errorMessage = errorMessage;// + "\r\n本脚本中的后续代码将不再执行。";
            log(errorMessage, logLevel, sql);

            _isError = true;
        }

        /// <summary>
        /// 选定当前要运行的数据库
        /// </summary>
        /// <param name="dbName">数据库名称（如果为空，则为本地默认数据库）</param>
        public bool setnowdb(string dbName)
        {
            if (_isError == true)
            {
                return false;
            }

            _dbServer = BF_DATABASE.Instance.GetDbByName(dbName);

            if (_dbServer == null)
            {
                WriteErrorMessage("未找到数据库【" + dbName + "】", 2);
                return false;
            }

            if (string.IsNullOrWhiteSpace(dbName))
            {
                log(string.Format("数据库已经成功切换为：【{0}】", BF_DATABASE.DEFAULT_DB_NAME));
            }
            else
            {
                log(string.Format("数据库已经成功切换为：【{0}，{1}，{2}】", _dbServer.ID, _dbServer.NAME, _dbServer.IP));
            }

            return true;
        }

        /// <summary>
        /// 设置当前数据库byID
        /// </summary>
        /// <param name="dbid">数据库服务器ID</param>
        public bool setnowdbid(int? dbid)
        {
            if (_isError == true)
            {
                return false;
            }

            if (dbid == null || dbid < 0)
            {
                WriteErrorMessage("指定的数据库ID不能为空或小于0", 2);
                return false;
            }

            _dbServer = BF_DATABASE.Instance.GetDbByID(dbid.Value);
            if (_dbServer == null)
            {
                WriteErrorMessage("未找到数据库【" + dbid + "】", 2);
                return false;
            }
            log(string.Format("数据库已经成功切换为：【{0}，{1}，{2}】", dbid, _dbServer.NAME, _dbServer.IP));
            return true;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="logLevel">日志等级</param>
        /// <param name="sql">SQL脚本</param>
        public bool log(string message, int logLevel = 4, string sql = "")
        {
            if (_isError)
            {
                return false;
            }
            try
            {
                return BF_ST_TASK_FLOW_NODE_LOG.Instance.Add(_tfnID, _taskID, _flowID, _nodeID, logLevel, message, sql) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 写日志（默认日志等级为4级）
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="sql">SQL脚本</param>
        public bool log(string message, string sql)
        {
            return log(message, BLog.LogLevel.INFO.GetHashCode(), sql);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isSplit">是否根据分号分割SQL语句</param>
        /// <returns>执行成功返回true，执行失败返回false</returns>
        public bool exec(string sql, bool isSplit = true)
        {
            if (_isError == true)
            {
                return false;
            }

            return execute(sql, isSplit);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isSplit">是否根据分号分割SQL语句</param>
        /// <returns>执行成功返回true，执行失败返回false</returns>
        public bool execute(string sql, bool isSplit = true)
        {
            if (_isError == true)
            {
                return false;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(string.Format(@"未设置执行数据库，请在语句【{0}】之前指定数据库setnowdb(""数据库名"");", sql), 3);
                return false;
            }

            //直接无视
            if (sql.ToUpper() == "COMMIT")
            {
                return true;
            }

            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                //log(string.Format("将在【{0}】执行SQL语句", _dbServer.NAME), sql);
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    if (isSplit == false)
                    {
                        dbHelper.ExecuteNonQuery(sql);
                        log(string.Format("在【{0}】执行SQL语句成功", _dbServer.NAME), sql);
                    }
                    else
                    {
                        //支持多条SQL，每条之间使用分号分隔，如果是多条则自动使用事务
                        string[] sqls = sql.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sqls.Length == 1)
                        {
                            dbHelper.ExecuteNonQuery(sqls[0]);
                            log(string.Format("在【{0}】执行SQL语句成功", _dbServer.NAME), sqls[0]);
                        }
                        else
                        {
                            dbHelper.BeginTrans();
                            bool isIntrans = false;
                            foreach (string s in sqls)
                            {
                                if (string.IsNullOrWhiteSpace(s) || s.Length < 5)
                                {
                                    //log(string.Format("事务中含有空SQL语句【{0}】", s));
                                    continue;
                                }

                                try
                                {
                                    if (s.Trim().ToUpper() == "COMMIT")
                                    {
                                        log(string.Format("显式调用了COMMIT强制提交事务"));
                                        dbHelper.CommitTrans();
                                        dbHelper.BeginTrans();
                                        isIntrans = false;
                                    }
                                    else
                                    {
                                        dbHelper.ExecuteNonQuery(s);
                                        isIntrans = true;
                                        log(string.Format("在【{0}】使用事务方式执行SQL语句成功", _dbServer.NAME), s);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log(string.Format("在【{0}】使用事务方式执行SQL语句失败", _dbServer.NAME), s);
                                    try
                                    {
                                        dbHelper.RollbackTrans();
                                    }
                                    catch (Exception erb)
                                    {
                                        log(string.Format("在【{0}】使用回滚事务失败\r\n", _dbServer.NAME, erb.Message), s);
                                    }

                                    throw ex;
                                }
                            }
                            if (isIntrans == true)
                            {
                                log(string.Format("事务未显式调用COMMIT，将自动提交"));
                                dbHelper.CommitTrans();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                WriteErrorMessage(string.Format("在【{0}】执行语句失败\r\n错误原因【{1}】", _dbServer.NAME, e.ToString()), 3, sql);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判断数据库中表是否存在
        /// </summary>
        /// <param name="tableNames">表名，可以为多张表，表名以逗号分隔</param>
        /// <returns>所有表均存在则返回为true，反之返回false</returns>
        public bool is_table_exists(string tableNames)
        {
            if (_isError == true)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(tableNames))
            {
                WriteErrorMessage("表名为空，【is_table_exists】语句将不会查找任何表。", 3);
                return false;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(@"未设置执行数据库，请在调用函数【is_table_exists】之前指定数据库setnowdb(""数据库名"");", 3);
                return false;
            }

            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    foreach (string tableName in tableNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string t = tableName;
                        if (t.Contains(".") == false)
                        {
                            t = _dbServer.USER_NAME + "." + tableName;
                        }

                        if (dbHelper.TableIsExists(t) == false)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【is_table_exists】检查表【{1}】时出错，错误信息为：\r\n{2}", _dbServer.NAME, tableNames, ex.ToString()), 3);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 清空表
        /// </summary>
        /// <param name="tableNames">表名，可以为多张表，表名以逗号分隔</param>
        public bool truncate_table(string tableNames)
        {
            if (_isError == true)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(tableNames))
            {
                WriteErrorMessage("表名为空，【truncate_table】语句将不会删除任何表记录。", 3);
                return false;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(@"未设置执行数据库，请在调用函数【truncate_table】之前指定数据库setnowdb(""数据库名"");", 3);
                return false;
            }

            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    foreach (string tableName in tableNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string t = tableName.Trim();
                        if (t.Contains(".") == false)
                        {
                            t = _dbServer.USER_NAME + "." + tableName;
                        }

                        if (dbHelper.TableIsExists(t))
                        {
                            if (dbHelper.Truncate(t) == false)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【truncate_table】清空表【{1}】所有记录时出错，错误信息为：\r\n【{2}】", _dbServer.NAME, tableNames, ex.ToString()), 3);
                return false;
            }

            log(string.Format(@"在【{0}】成功调用函数【truncate_table】清空表【{1}】所有记录", _dbServer.NAME, tableNames));
            return true;
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableNames">表名，可以为多张表，表名以逗号分隔</param>
        public bool drop_table(string tableNames)
        {
            if (_isError == true)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(tableNames))
            {
                WriteErrorMessage("表名为空，【drop_table】语句将不会删除任何表。", 3);
                return false;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(@"未设置执行数据库，请在调用函数【drop_table】之前指定数据库setnowdb(""数据库名"");", 3);
                return false;
            }

            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    foreach (string tableName in tableNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string t = tableName.Trim();
                        if (t.Contains(".") == false)
                        {
                            t = _dbServer.USER_NAME + "." + tableName;
                        }

                        if (dbHelper.TableIsExists(t))
                        {
                            if (dbHelper.Drop(t) == false)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【drop_table】删除表【{1}】时出错，错误信息为：\r\n【{2}】", _dbServer.NAME, tableNames, ex.ToString()), 3);
                return false;
            }

            log(string.Format(@"在【{0}】成功调用函数【drop_table】删除表【{1}】", _dbServer.NAME, tableNames));

            return true;
        }

        #region 执行SQL语句返回结果集

        /// <summary>
        /// 执行SQL语句，返回执行语句结果集
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>DataTable结果集</returns>
        public DataTable exec_table(string sql)
        {
            return execute_datatable(sql);
        }

        /// <summary>
        /// 执行SQL语句，返回执行语句结果集
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>DataTable结果集</returns>
        public DataTable execute_datatable(string sql)
        {
            if (_isError == true)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                WriteErrorMessage("SQL语句为空，【execute_datatable】语句将不会执行任何内容。", 3);
                return null;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                log(@"未设置执行数据库，请在调用函数【execute_datatable】之前指定数据库setnowdb(""数据库名"");", 3, sql);
                return null;
            }
            DataTable dt;
            int n = 0;
            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    dt = dbHelper.ExecuteDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【execute_datatable】时出错，错误信息为：\r\n【{1}】", _dbServer.NAME, ex.ToString()), 3, sql);
                return null;
            }
            if (dt != null)
            {
                n = dt.Rows.Count;
            }
            log(string.Format(@"在【{0}】成功调用函数【execute_datatable】，查询了【{1}】行数据", _dbServer.NAME, n), 4, sql);
            return dt;
        }

        #endregion

        #region 执行SQL语句返回第一行第一列

        /// <summary>
        /// 执行SQL语句返回第一行第一列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回执行语句结果</returns>
        public object exec_scalar(string sql)
        {
            return execute_scalar(sql);
        }

        /// <summary>
        /// 执行SQL语句返回第一行第一列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回执行语句结果</returns>
        public object exec_obj(string sql)
        {
            return execute_scalar(sql);
        }

        /// <summary>
        /// 执行SQL语句，返回执行语句结果
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回执行语句结果</returns>
        public object execute_scalar(string sql)
        {
            if (_isError == true)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                WriteErrorMessage("SQL语句为空，【execute_scalar】语句将不会执行任何内容。", 3);
                return null;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                log(@"未设置执行数据库，请在调用函数【execute_scalar】之前指定数据库setnowdb(""数据库名"");", 3, sql);
                return null;
            }
            object obj;
            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    obj = dbHelper.ExecuteScalar(sql);
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【execute_scalar】时出错，错误信息为：\r\n【{1}】", _dbServer.NAME, ex.ToString()), 3, sql);
                return null;
            }

            log(string.Format(@"在【{0}】成功调用函数【execute_scalar】", _dbServer.NAME), 4, sql);
            return obj;
        }

        #endregion

        /// <summary>
        /// 查询数据并写到本地文件
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="fileName">文件名</param>
        /// <param name="pageSize">分页大小，每一页写一个文件</param>
        /// <returns>输出文件所在的路径</returns>
        public string down_db(string sql, string fileName, int pageSize = 1000000)
        {
            if (_isError == true)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                WriteErrorMessage("SQL语句为空，【down_db】语句将不会执行任何内容。", 3);
                return string.Empty;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                log(@"未设置执行数据库，请在调用函数【down_db】之前指定数据库setnowdb(""数据库名"");", 3, sql);
                return string.Empty;
            }

            string path = _downDbPath + fileName + "\\";

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (var file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】导出数据到文件，在创建及清空目录【{1}】时出错，错误信息为：\r\n{2}", _dbServer.NAME, path, ex.ToString()), 3, sql);
                return string.Empty;
            }

            DataTable dt;
            int pageIndex = 0;
            int i = 0;
            DateTime begin = DateTime.Now;
            try
            {
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper adbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    //分页查询
                    while (true)
                    {
                        pageIndex++;
                        dt = adbHelper.ExecuteDataTablePage(sql, pageSize, pageIndex);

                        if (dt == null)
                        {
                            break;
                        }

                        if (pageIndex == 1)
                        {
                            //写建表脚本
                            string createFileName = path + fileName + "_Create.txt";
                            string createSql = adbHelper.MakeCreateTableSql(fileName, dt);
                            File.WriteAllText(createFileName, createSql, Encoding.UTF8);
                        }

                        if (dt.Rows.Count < 1)
                        {
                            break;
                        }

                        //写文件
                        string dataFileName = path + fileName + "_" + pageIndex + ".txt";
                        adbHelper.WriteDataTableIntoFile(dt, dataFileName);
                        i += dt.Rows.Count;
                        if (dt.Rows.Count < pageSize)
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【down_db】时出错，错误信息为：\r\n【{1}】", _dbServer.NAME, ex.ToString()), 3, sql);
                return string.Empty;
            }

            TimeSpan ts = DateTime.Now - begin;
            log(string.Format(@"在【{0}】成功调用函数【down_db】,将查询结果写入【{1}】条记录到文件【{2}】，共用时【{3}】毫秒（包含查询时间）", _dbServer.NAME, i, path, ts.TotalMilliseconds), 4, sql);
            return path;
        }


        /// <summary>
        /// 导出表写入文件并将文件压缩打包，生成的文件路径：~/UpFiles/DownDB/{tableName}.zip
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="zipFileName">压缩文件名（不包含后缀名）</param>
        /// <returns>文件全名,含路径</returns>
        public string down_db_to_file(string sql, string zipFileName)
        {
            if (_isError == true)
            {
                return string.Empty;
            }

            var path = down_db(sql, zipFileName);
            if (string.IsNullOrWhiteSpace(path))
            {
                WriteErrorMessage(string.Format(@"从数据库【{0}】查询数据，并将结果写入本地路径【{1}】失败，将不会生成zip压缩包。", _dbServer.NAME, path), 3, sql);
                return string.Empty;
            }

            var zipFileFullName = _downDbPath + zipFileName + ".zip";
            try
            {
                BZipFile.ZipDirectory(path, zipFileFullName);
                log(string.Format("从数据库【{0}】查询数据，生成本地ZIP文件成功，文件名为【{1}】", _dbServer.NAME, zipFileFullName));
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"压缩路径【{0}】下的文件失败，错误信息为：\r\n{1}", path, ex.ToString()), 3, sql);
                return string.Empty;
            }

            return zipFileFullName;
        }

        /// <summary>
        /// 数据库里面表到表的数据复制（当pageSize达到10000时，会自动选择异步方式；达到100000时，会自动选择流的方式）
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="destTableName">生成的目标表名</param>
        /// <param name="destDBName">导入的目的数据库</param>
        /// <param name="isCreatTable">0表示不创建表;1(默认值)表示自动创建表(在导数据之前，要删除已经存在表</param>
        /// <param name="pageSize">页面大小（分批查询导入，每批次记录条数，超过10万则会自动使用先写文件再导文件的方式）</param>
        /// <returns>复制记录条数</returns>
        public int down_db_to_db(string sql, string destTableName, string destDBName, int isCreatTable = 1, int pageSize = 50000)
        {
            if (pageSize < 10000)
            {
                //普通方式
                return down_db_to_db_general(sql, destTableName, destDBName, isCreatTable, pageSize);
            }
            //else if (pageSize < 100000)
            //{
            //    //异步方式
            //    return down_db_to_db_async(sql, destTableName, destDBName, isCreatTable, pageSize);
            //}
            else
            {
                //流式
                return down_db_to_db_flow(sql, destTableName, destDBName, isCreatTable, pageSize);
            }
        }

        /// <summary>
        /// 数据库里面表到表的数据复制（适用于十万级别及以下的中小规模表）
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="destTableName">生成的目标表名</param>
        /// <param name="destDBName">导入的目的数据库</param>
        /// <param name="isCreatTable">0表示不创建表;1(默认值)表示自动创建表(在导数据之前，要删除已经存在表</param>
        /// <param name="pageSize">页面大小（分批查询导入，每批次记录条数，超过10万则会自动使用先写文件再导文件的方式）</param>
        /// <returns>复制记录条数</returns>
        public int down_db_to_db_general(string sql, string destTableName, string destDBName, int isCreatTable = 1, int pageSize = 50000)
        {
            if (_isError == true)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                WriteErrorMessage("SQL语句为空，【down_db_to_db】语句将不会执行任何内容。", 3);
                return 0;
            }

            if (string.IsNullOrWhiteSpace(destTableName))
            {
                WriteErrorMessage("目标表名为空，【down_db_to_db】语句将不会执行任何内容。", 3, sql);
                return 0;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(@"未设置执行数据库，请在调用函数【down_db_to_db】之前指定数据库setnowdb(""数据库名"");");
                return 0;
            }
            //获取目的数据库
            BF_DATABASE.Entity destDBServer = BF_DATABASE.Instance.GetDbByName(destDBName);
            //验证当前是否打开数据库
            if (destDBServer == null)
            {
                WriteErrorMessage(string.Format(@"目的数据库【{0}】不存在，【down_db_to_db】将不会进行任何操作;", destDBName));
                return 0;
            }

            DataTable dt;
            int pageIndex = 0;
            int i = 0;
            DateTime begin = DateTime.Now;
            DateTime bg = DateTime.Now;
            TimeSpan tq = bg - begin;
            TimeSpan ti = bg - begin;
            try
            {
                if (destTableName.IndexOf('.') <= 0)
                {
                    destTableName = destDBServer.USER_NAME + "." + destTableName;
                }
                Enums.DBType dbTypeDest = (Enums.DBType)destDBServer.DB_TYPE;
                using (BDBHelper dbHelperDest = new BDBHelper(dbTypeDest.ToString(), destDBServer.IP, destDBServer.PORT, destDBServer.USER_NAME, destDBServer.PASSWORD, destDBServer.DB_NAME, destDBServer.DB_NAME))
                {
                    bool isNeedCreateTable = false;
                    if (dbHelperDest.TableIsExists(destTableName) == true)
                    {
                        //创建新表
                        if (isCreatTable == 1)
                        {
                            isNeedCreateTable = true;
                            log(string.Format(@"将删除目的数据库【{0}】的表【{1}】", destDBName, destTableName), 4);
                            try
                            {
                                dbHelperDest.Drop(destTableName);
                            }
                            catch (Exception e)
                            {
                                WriteErrorMessage(string.Format(@"删除目的数据库【{0}】的表【{1}】出错，错误信息为：\r\n【{2}】", destDBName, destTableName, e.ToString()), 3);
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        isNeedCreateTable = true;
                    }

                    Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                    //查询
                    using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                    {
                        while (Main.IsRun)
                        {
                            pageIndex++;
                            bg = DateTime.Now;
                            dt = dbHelper.ExecuteDataTablePage(sql, pageSize, pageIndex);
                            tq = DateTime.Now - bg;

                            if (dt == null || dt.Rows.Count < 1)
                            {
                                break;
                            }

                            //移除分页查询中的序号列
                            if (dt.Columns.Contains("BROW_NUM"))
                            {
                                dt.Columns.Remove("BROW_NUM");
                            }

                            //创建表
                            if (isNeedCreateTable == true)
                            {
                                isNeedCreateTable = false;
                                bool isCreatedTable = false;
                                try
                                {
                                    isCreatedTable = dbHelperDest.CreateTableFromDataTable(destTableName, dt);
                                }
                                catch (Exception ex)
                                {
                                    WriteErrorMessage(string.Format(@"在目的数据库【{0}】根据查询结果创建表【{1}】出错，错误信息为：\r\n{2}", destDBName, destTableName, ex.Message), 3);
                                    return 0;
                                }

                                if (isCreatedTable == true)
                                {
                                    WriteErrorMessage(string.Format(@"已在目的数据库【{0}】根据查询结果创建表【{1}】", destDBName, destTableName), 3);
                                }
                                else
                                {
                                    WriteErrorMessage(string.Format(@"在目的数据库【{0}】根据查询结果创建表【{1}】失败，错误信息：【{2}】\r\n", destDBName, destTableName, _errorMessage), 3);
                                    return 0;
                                }
                            }

                            try
                            {
                                //导入
                                bg = DateTime.Now;
                                int n = 0;
                                int rc = dt.Rows.Count;
                                if (rc < Main.UseFileModeRows)
                                {
                                    n = dbHelperDest.LoadDataInDataTable(destTableName, dt);
                                }
                                else
                                {
                                    n = dbHelperDest.LoadDataInDataTableWithFile(destTableName, dt);
                                }

                                ti = DateTime.Now - bg;
                                i += n;
                                if (Main.IsCheckLoadCount && n < rc)
                                {
                                    WriteErrorMessage(string.Format(@"将第【{0}】页查询结果【{1}】条记录导入到目的数据库【{2}】表【{3}】成功导入【{4}】条，其中，查询用时【{5}】毫秒，导入用时【{6}】毫秒，已经累计导入【{7}】条，由于本页数据未完全导入，将提前中止。", pageIndex, dt.Rows.Count, destDBName, destTableName, n, tq.TotalMilliseconds, ti.TotalMilliseconds, i), 3);
                                    return i;
                                }
                                log(string.Format(@"将第【{0}】页查询结果【{1}】条记录导入到目的数据库【{2}】表【{3}】成功导入【{4}】条，其中，查询用时【{5}】毫秒，导入用时【{6}】毫秒，已经累计导入【{7}】条", pageIndex, dt.Rows.Count, destDBName, destTableName, n, tq.TotalMilliseconds, ti.TotalMilliseconds, i), 4);
                            }
                            catch (Exception ee)
                            {
                                WriteErrorMessage(string.Format(@"将第【{0}】页查询结果导入到目的数据库【{1}】表【{2}】失败，错误信息：\r\n【{3}】", pageIndex, destDBName, destTableName, ee.ToString()), 3, sql);
                                return i;
                            }

                            if (dt.Rows.Count < pageSize)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【down_db_to_db】时出错，错误信息为：\r\n【{1}】", _dbServer.NAME, ex.ToString()), 3, sql);
                return 0;
            }
            TimeSpan ts = DateTime.Now - begin;
            log(string.Format(@"在【{0}】成功调用函数【down_db_to_db】,将查询结果导入【{1}】条记录到表【{2}】，总共用时【{3}】毫秒（包含查询时间）", _dbServer.NAME, i, destTableName, ts.TotalMilliseconds), 4, sql);

            return i;
        }

        /// <summary>
        /// 使用异步方式将查询结果写入指定表（适用于百万级别的中等规模表）
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="destTableName">生成的目标表名</param>
        /// <param name="destDBName">导入的目的数据库</param>
        /// <param name="isCreatTable">0表示不创建表;1(默认值)表示自动创建表(在导数据之前，要删除已经存在表</param>
        /// <param name="pageSize">页面大小（分批查询导入，每批次记录条数，超过10万则会自动使用先写文件再导文件的方式）</param>
        /// <returns>复制记录条数</returns>
        public int down_db_to_db_async(string sql, string destTableName, string destDBName, int isCreatTable = 1, int pageSize = 50000)
        {
            if (_isError == true)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                WriteErrorMessage("SQL语句为空，【down_db_to_db】语句将不会执行任何内容。", 3);
                return 0;
            }

            if (string.IsNullOrWhiteSpace(destTableName))
            {
                WriteErrorMessage("目标表名为空，【down_db_to_db】语句将不会执行任何内容。", 3, sql);
                return 0;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(@"未设置执行数据库，请在调用函数【down_db_to_db】之前指定数据库setnowdb(""数据库名"");");
                return 0;
            }
            //获取目的数据库
            BF_DATABASE.Entity destDBServer = BF_DATABASE.Instance.GetDbByName(destDBName);
            //验证当前是否打开数据库
            if (destDBServer == null)
            {
                WriteErrorMessage(string.Format(@"目的数据库【{0}】不存在，【down_db_to_db】将不会进行任何操作;", destDBName));
                return 0;
            }

            int pageIndex = 0;
            _isAsyncLoadQueryEnd = false;
            _isAsyncLoadWriteEnd = false;
            _asyncLoadRowsCount = 0;
            bool isNeedCreateTable = false;
            int readRowsCount = 0;

            DateTime begin = DateTime.Now;
            DateTime bg = DateTime.Now;
            TimeSpan tq = bg - begin;
            TimeSpan ti = bg - begin;
            try
            {
                if (destTableName.IndexOf('.') <= 0)
                {
                    destTableName = destDBServer.USER_NAME + "." + destTableName;
                }

                _bwAsyncLoadTask = new BackgroundWorker();
                _bwAsyncLoadTask.WorkerSupportsCancellation = true;
                _bwAsyncLoadTask.DoWork += AsyncLoadTask_DoWork;
                _bwAsyncLoadTask.RunWorkerAsync();

                //查询
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    while (Main.IsRun)
                    {
                        if (_isError == true)
                        {
                            BLog.Write(BLog.LogLevel.WARN, "异步查询过程中程序出现错误而停止导入，已经查询" + pageIndex + "页，" + readRowsCount + "条记录");
                            break;
                        }

                        pageIndex++;
                        //BLog.Write(BLog.LogLevel.DEBUG, "将查询第" + pageIndex + "页，并异步将结果导入到表：" + destTableName);
                        bg = DateTime.Now;
                        DataTable dt = dbHelper.ExecuteDataTablePage(sql, pageSize, pageIndex);
                        tq = DateTime.Now - bg;
                        //BLog.Write(BLog.LogLevel.DEBUG, "已经查询第" + pageIndex + "页，并异步将结果导入到表：" + destTableName);

                        if (dt == null || dt.Rows.Count < 1)
                        {
                            break;
                        }

                        readRowsCount += dt.Rows.Count;
                        //移除分页查询中的序号列
                        if (dt.Columns.Contains("BROW_NUM"))
                        {
                            dt.Columns.Remove("BROW_NUM");
                        }

                        log(string.Format(@"查询第【{0}】页，有【{1}】条记录，查询用时【{2}】毫秒，稍后将异步导入到数据库【{3}】表【{4}】中。", pageIndex, dt.Rows.Count, tq.TotalMilliseconds, destDBName, destTableName), 4);

                        //第一页将有可能需要建表
                        if (pageIndex == 1)
                        {
                            Enums.DBType dbTypeDest = (Enums.DBType)_dbServer.DB_TYPE;
                            using (BDBHelper dbHelperDest = new BDBHelper(dbTypeDest.ToString(), destDBServer.IP, destDBServer.PORT, destDBServer.USER_NAME, destDBServer.PASSWORD, destDBServer.DB_NAME, destDBServer.DB_NAME))
                            {
                                if (dbHelperDest.TableIsExists(destTableName) == true)
                                {
                                    //创建新表
                                    if (isCreatTable == 1)
                                    {
                                        isNeedCreateTable = true;
                                        log(string.Format(@"将删除目的数据库【{0}】的表【{1}】", destDBName, destTableName), 4);
                                        try
                                        {
                                            dbHelperDest.Drop(destTableName);
                                        }
                                        catch (Exception e)
                                        {
                                            WriteErrorMessage(string.Format(@"删除目的数据库【{0}】的表【{1}】出错，错误信息为：\r\n【{2}】", destDBName, destTableName, e.ToString()), 3);
                                            return 0;
                                        }
                                    }
                                }
                                else
                                {
                                    isNeedCreateTable = true;
                                }
                                //创建表
                                if (isNeedCreateTable == true)
                                {
                                    isNeedCreateTable = false;
                                    bool isCreatedTable = false;
                                    try
                                    {
                                        isCreatedTable = dbHelperDest.CreateTableFromDataTable(destTableName, dt);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteErrorMessage(string.Format(@"在目的数据库【{0}】根据查询结果创建表【{1}】出错，错误信息为：\r\n{2}", destDBName, destTableName, ex.Message), 3);
                                        return 0;
                                    }

                                    if (isCreatedTable == true)
                                    {
                                        log(string.Format(@"已在目的数据库【{0}】根据查询结果创建表【{1}】", destDBName, destTableName), 4);
                                    }
                                    else
                                    {
                                        WriteErrorMessage(string.Format(@"在目的数据库【{0}】根据查询结果创建表【{1}】失败，错误信息：【{2}】\r\n", destDBName, destTableName, _errorMessage), 3);
                                        return 0;
                                    }
                                }
                            }
                        }

                        //异步导入
                        LoadDataAsync(destDBServer, destTableName, dt, pageIndex);

                        //避免过多数据压到内存
                        while (_queueAsyncLoadTask.Count > 3)
                        {
                            if (_isError == true)
                            {
                                BLog.Write(BLog.LogLevel.WARN, "异步查询及导入过程中程序出现错误而停止导入，已经查询" + pageIndex + "页，" + readRowsCount + "条记录");
                                break;
                            }
                            //BLog.Write(BLog.LogLevel.DEBUG, "异步导入：已经有【" + _queueAsyncLoadTask.Count + "】页数据在等待导入中，导入速度慢于查询速度，将暂停查询。");
                            Thread.Sleep(1000);
                            continue;
                        }

                        if (dt.Rows.Count < pageSize)
                        {
                            break;
                        }
                    }
                }
                //查询完毕
                _isAsyncLoadQueryEnd = true;

                //避免永远等待
                DateTime waiteEnd = DateTime.Now.AddMinutes(10);
                //等待执行完成
                while (_queueAsyncLoadTask.Count > 0 || (_isAsyncLoadWriteEnd == false && DateTime.Now < waiteEnd))
                {
                    if (_isError)
                    {
                        return _asyncLoadRowsCount;
                    }
                    Thread.Sleep(100);
                    continue;
                }
                _bwAsyncLoadTask.CancelAsync();
                _bwAsyncLoadTask.Dispose();

                TimeSpan ts = DateTime.Now - begin;
                log(string.Format(@"在【{0}】成功调用函数【down_db_to_db】,将查询结果异步导入【{1}】条记录到表【{2}】，总共用时【{3}】毫秒（包含查询时间）", _dbServer.NAME, _asyncLoadRowsCount, destTableName, ts.TotalMilliseconds), 4, sql);

                return _asyncLoadRowsCount;
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【down_db_to_db】时出错，错误信息为：\r\n【{1}】", _dbServer.NAME, ex.ToString()), 3, sql);
                return 0;
            }
        }

        /// <summary>
        /// 使用流的方式查询并将结果异步写入指定表（对于超过千万条记录的大表，这是最高效的方式）
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="destTableName">生成的目标表名</param>
        /// <param name="destDBName">导入的目的数据库</param>
        /// <param name="isCreatTable">0表示不创建表;1(默认值)表示自动创建表(在导数据之前，要删除已经存在表</param>
        /// <param name="pageSize">页面大小（分批查询导入，每批次记录条数，超过10万则会自动使用先写文件再导文件的方式）</param>
        /// <returns>复制记录条数</returns>
        public int down_db_to_db_flow(string sql, string destTableName, string destDBName, int isCreatTable = 1, int pageSize = 50000)
        {
            if (_isError == true)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                WriteErrorMessage("SQL语句为空，【down_db_to_db】语句将不会执行任何内容。", 3);
                return 0;
            }

            if (string.IsNullOrWhiteSpace(destTableName))
            {
                WriteErrorMessage("目标表名为空，【down_db_to_db】语句将不会执行任何内容。", 3, sql);
                return 0;
            }

            //验证当前是否打开数据库
            if (_dbServer == null)
            {
                WriteErrorMessage(@"未设置执行数据库，请在调用函数【down_db_to_db】之前指定数据库setnowdb(""数据库名"");");
                return 0;
            }
            //获取目的数据库
            BF_DATABASE.Entity destDBServer = BF_DATABASE.Instance.GetDbByName(destDBName);
            //验证当前是否打开数据库
            if (destDBServer == null)
            {
                WriteErrorMessage(string.Format(@"目的数据库【{0}】不存在，【down_db_to_db】将不会进行任何操作;", destDBName));
                return 0;
            }

            DataTable dt = new DataTable();
            //表结构
            DataTable dtSchema = new DataTable();
            int pageIndex = 0;
            int readRowsCount = 0;
            _isAsyncLoadQueryEnd = false;
            _isAsyncLoadWriteEnd = false;
            _asyncLoadRowsCount = 0;
            bool isNeedCreateTable = false;

            DateTime begin = DateTime.Now;
            DateTime bg = DateTime.Now;
            TimeSpan tq = bg - begin;
            TimeSpan ti = bg - begin;
            try
            {
                if (destTableName.IndexOf('.') <= 0)
                {
                    destTableName = destDBServer.USER_NAME + "." + destTableName;
                }

                _bwAsyncLoadTask = new BackgroundWorker();
                _bwAsyncLoadTask.WorkerSupportsCancellation = true;
                _bwAsyncLoadTask.DoWork += AsyncLoadTask_DoWork;
                _bwAsyncLoadTask.RunWorkerAsync();

                //查询
                Enums.DBType dbType = (Enums.DBType)_dbServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbType.ToString(), _dbServer.IP, _dbServer.PORT, _dbServer.USER_NAME, _dbServer.PASSWORD, _dbServer.DB_NAME, _dbServer.DB_NAME))
                {
                    using (IDataReader reader = dbHelper.ExecuteReader(sql))
                    {
                        //设置表结构
                        for (int c = 0; c < reader.FieldCount; c++)
                        {
                            dt.Columns.Add(reader.GetName(c), reader.GetFieldType(c));
                        }

                        bool isCanRead = reader.IsClosed == false && reader.Read();

                        int i = 0;
                        bg = DateTime.Now;
                        //遍历记录
                        while (Main.IsRun && reader.IsClosed == false && isCanRead)
                        {
                            if (_isError == true)
                            {
                                BLog.Write(BLog.LogLevel.WARN, "流式查询过程中程序出现错误而停止导入，已经查询" + pageIndex + "页，" + readRowsCount + "条记录");
                                break;
                            }
                            //赋值
                            DataRow dr = dt.NewRow();
                            for (int c = 0; c < reader.FieldCount; c++)
                            {
                                dr[c] = reader.GetValue(c);
                            }
                            dt.Rows.Add(dr);

                            i++;
                            readRowsCount++;
                            isCanRead = reader.Read();

                            if (i >= pageSize || isCanRead == false)
                            {
                                tq = DateTime.Now - bg;
                                pageIndex++;
                                log(string.Format(@"使用流的方式查询第【{0}】页，有【{1}】条记录，查询用时【{2}】毫秒，稍后将异步导入到数据库【{3}】表【{4}】中。", pageIndex, dt.Rows.Count, tq.TotalMilliseconds, destDBName, destTableName), 4);

                                //第一页将有可能需要建表
                                if (pageIndex == 1)
                                {
                                    Enums.DBType dbTypeDest = (Enums.DBType)destDBServer.DB_TYPE;
                                    using (BDBHelper dbHelperDest = new BDBHelper(dbTypeDest.ToString(), destDBServer.IP, destDBServer.PORT, destDBServer.USER_NAME, destDBServer.PASSWORD, destDBServer.DB_NAME, destDBServer.DB_NAME))
                                    {
                                        if (dbHelperDest.TableIsExists(destTableName) == true)
                                        {
                                            //创建新表
                                            if (isCreatTable == 1)
                                            {
                                                isNeedCreateTable = true;
                                                log(string.Format(@"将删除目的数据库【{0}】的表【{1}】", destDBName, destTableName), 4);
                                                try
                                                {
                                                    dbHelperDest.Drop(destTableName);
                                                }
                                                catch (Exception e)
                                                {
                                                    WriteErrorMessage(string.Format(@"删除目的数据库【{0}】的表【{1}】出错，错误信息为：\r\n【{2}】", destDBName, destTableName, e.ToString()), 3);
                                                    return 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            isNeedCreateTable = true;
                                        }
                                        //创建表
                                        if (isNeedCreateTable == true)
                                        {
                                            isNeedCreateTable = false;
                                            bool isCreatedTable = false;
                                            try
                                            {
                                                isCreatedTable = dbHelperDest.CreateTableFromDataTable(destTableName, dt);
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteErrorMessage(string.Format(@"在目的数据库【{0}】根据查询结果创建表【{1}】出错，错误信息为：\r\n{2}", destDBName, destTableName, ex.Message), 3);
                                                return 0;
                                            }

                                            if (isCreatedTable == true)
                                            {
                                                log(string.Format(@"已在目的数据库【{0}】根据查询结果创建表【{1}】", destDBName, destTableName), 4);
                                            }
                                            else
                                            {
                                                WriteErrorMessage(string.Format(@"在目的数据库【{0}】根据查询结果创建表【{1}】失败，错误信息：【{2}】\r\n", destDBName, destTableName, _errorMessage), 3);
                                                return 0;
                                            }
                                        }
                                    }
                                }

                                //异步导入
                                LoadDataAsync(destDBServer, destTableName, dt, pageIndex);

                                //避免过多数据压到内存
                                while (_queueAsyncLoadTask.Count > 3)
                                {
                                    if (_isError == true)
                                    {
                                        BLog.Write(BLog.LogLevel.WARN, "流式查询及导入过程中程序出现错误而停止导入，已经查询" + pageIndex + "页，" + readRowsCount + "条记录");
                                        break;
                                    }
                                    //BLog.Write(BLog.LogLevel.DEBUG, "异步导入：已经有【" + _queueAsyncLoadTask.Count + "】页数据在等待导入中，导入速度慢于查询速度，将暂停查询。");
                                    Thread.Sleep(1000);
                                    continue;
                                }

                                dt.Rows.Clear();
                                i = 0;

                                bg = DateTime.Now;
                            }

                            if (isCanRead == false)
                            {
                                break;
                            }
                        }
                    }
                }
                //查询完毕
                _isAsyncLoadQueryEnd = true;
                //避免永远等待
                DateTime waiteEnd = DateTime.Now.AddMinutes(10);
                //等待执行完成
                while (_queueAsyncLoadTask.Count > 0 || (_isAsyncLoadWriteEnd == false && DateTime.Now < waiteEnd))
                {
                    if (_isError)
                    {
                        return _asyncLoadRowsCount;
                    }
                    Thread.Sleep(100);
                    continue;
                }
                _bwAsyncLoadTask.CancelAsync();
                _bwAsyncLoadTask.Dispose();

                TimeSpan ts = DateTime.Now - begin;
                log(string.Format(@"在【{0}】成功调用函数【down_db_to_db】,将查询结果异步导入【{1}】条记录到表【{2}】，总共用时【{3}】毫秒（包含查询时间）", _dbServer.NAME, _asyncLoadRowsCount, destTableName, ts.TotalMilliseconds), 4, sql);

                return _asyncLoadRowsCount;
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"在【{0}】调用函数【down_db_to_db】时出错，错误信息为：\r\n【{1}】", _dbServer.NAME, ex.ToString()), 3, sql);
                return 0;
            }
        }

        /// <summary>
        /// 异步导入数据任务
        /// </summary>
        protected class AsyncLoadTask
        {
            public string DestTableName;
            public BF_DATABASE.Entity DestDBServer;
            public DataTable Table;
            public int PageIndex;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="destDBServer"></param>
            /// <param name="destTableName"></param>
            /// <param name="dt"></param>
            /// <param name="pageIndex"></param>
            public AsyncLoadTask(BF_DATABASE.Entity destDBServer, string destTableName, DataTable dt, int pageIndex)
            {
                DestTableName = destTableName;
                DestDBServer = destDBServer;
                //需要复制一份
                Table = dt.Clone();
                foreach (DataRow dr in dt.Rows)
                {
                    Table.ImportRow(dr);
                }
                PageIndex = pageIndex;
            }
        }

        #region 异步导入变量定义
        private bool _isAsyncLoadQueryEnd = false;
        private bool _isAsyncLoadWriteEnd = false;
        private int _asyncLoadRowsCount = 0;
        private ConcurrentQueue<AsyncLoadTask> _queueAsyncLoadTask = new ConcurrentQueue<AsyncLoadTask>();
        private BackgroundWorker _bwAsyncLoadTask;
        #endregion

        /// <summary>
        /// 后台线程不断处理队列中的数据，将其导入到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AsyncLoadTask_DoWork(object sender, DoWorkEventArgs e)
        {
            AsyncLoadTask asyncTask;
            int pageCount = 0;
            DateTime lastLoadTime = DateTime.Now;
            while (Main.IsRun)
            {
                if (_isError == true)
                {
                    BLog.Write(BLog.LogLevel.WARN, "异步导入数据过程中程序出现错误而停止导入，已经导入" + pageCount + "页，" + _asyncLoadRowsCount + "条记录");
                    return;
                }

                if (_queueAsyncLoadTask.Count < 1)
                {
                    if (_isAsyncLoadQueryEnd == true)
                    {
                        BLog.Write(BLog.LogLevel.DEBUG, "异步导入数据结束，共有" + pageCount + "页，" + _asyncLoadRowsCount + "条记录");
                        Thread.Sleep(100);
                        _isAsyncLoadWriteEnd = true;
                        return;
                    }

                    //超时
                    if ((DateTime.Now - lastLoadTime).TotalMinutes > 15)
                    {
                        _isError = true;
                        WriteErrorMessage("超过15分钟没有新的数据，猜测是查询卡死，提前中止导入。当前共有" + pageCount + "页，" + _asyncLoadRowsCount + "条记录", 3);
                        Thread.Sleep(100);
                        _isAsyncLoadWriteEnd = true;
                        return;
                    }

                    Thread.Sleep(100);
                    continue;
                }

                if (_queueAsyncLoadTask.TryDequeue(out asyncTask) == false)
                {
                    Thread.Sleep(100);
                    continue;
                }

                lastLoadTime = DateTime.Now;
                pageCount = asyncTask.PageIndex;
                BLog.Write(BLog.LogLevel.DEBUG, string.Format("准备将异步导入第【{0}】页数据，到表【{1}】，还有【{2}】页等待导入。", asyncTask.PageIndex, asyncTask.DestTableName, _queueAsyncLoadTask.Count));
                try
                {
                    Enums.DBType dbTypeDest = (Enums.DBType)asyncTask.DestDBServer.DB_TYPE;
                    using (BDBHelper dbHelperDest = new BDBHelper(dbTypeDest.ToString(), asyncTask.DestDBServer.IP, asyncTask.DestDBServer.PORT, asyncTask.DestDBServer.USER_NAME, asyncTask.DestDBServer.PASSWORD, asyncTask.DestDBServer.DB_NAME, asyncTask.DestDBServer.DB_NAME))
                    {
                        //导入
                        DateTime bg = DateTime.Now;
                        int rc = asyncTask.Table.Rows.Count;
                        int n = 0;
                        if (rc < Main.UseFileModeRows)
                        {
                            n = dbHelperDest.LoadDataInDataTable(asyncTask.DestTableName, asyncTask.Table);
                        }
                        else
                        {
                            n = dbHelperDest.LoadDataInDataTableWithFile(asyncTask.DestTableName, asyncTask.Table, Main.IsDeleteLoadDataFiles);
                        }

                        TimeSpan ti = DateTime.Now - bg;
                        _asyncLoadRowsCount += n;
                        if (Main.IsCheckLoadCount && n < rc)
                        {
                            WriteErrorMessage(string.Format(@"第【{0}】页查询结果【{1}】条记录，已经异步导入到目的数据库【{2}】表【{3}】成功导入【{4}】条，导入用时【{5}】毫秒，已经累计导入【{6}】条，还有【{7}】页等待导入中，由于本页数据未完全导入，将提前中止。", asyncTask.PageIndex, asyncTask.Table.Rows.Count, asyncTask.DestDBServer.NAME, asyncTask.DestTableName, n, ti.TotalMilliseconds, _asyncLoadRowsCount, _queueAsyncLoadTask.Count), 3);
                            _isError = true;
                            return;
                        }
                        log(string.Format(@"第【{0}】页查询结果【{1}】条记录，已经异步导入到目的数据库【{2}】表【{3}】成功导入【{4}】条，导入用时【{5}】毫秒，已经累计导入【{6}】条，还有【{7}】页等待导入中", asyncTask.PageIndex, asyncTask.Table.Rows.Count, asyncTask.DestDBServer.NAME, asyncTask.DestTableName, n, ti.TotalMilliseconds, _asyncLoadRowsCount, _queueAsyncLoadTask.Count), 4);
                    }
                }
                catch (Exception ee)
                {
                    WriteErrorMessage(string.Format(@"将第【{0}】页查询结果异步导入到目的数据库【{1}】表【{2}】失败，内存中还有【{3}】页未导入，错误信息：\r\n【{4}】", asyncTask.PageIndex, asyncTask.DestDBServer.NAME, asyncTask.DestTableName, _queueAsyncLoadTask.Count, ee.ToString()), 3);
                    _isError = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 添加待处理数据到队列，如果等待数据过多，则返回false提示需要等待
        /// </summary>
        /// <param name="destDBServer"></param>
        /// <param name="destTableName"></param>
        /// <param name="dt"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private void LoadDataAsync(BF_DATABASE.Entity destDBServer, string destTableName, DataTable dt, int pageIndex)
        {
            _queueAsyncLoadTask.Enqueue(new AsyncLoadTask(destDBServer, destTableName, dt, pageIndex));
        }

        /// <summary>
        /// 将txt文件数据导入到表中
        /// 本方法当前支持将计件esop推送的txt文件数据导入到指定库的表中(不一定适用于其他txt文件)
        /// </summary>
        /// <param name="destTableName"></param>
        /// <param name="destDBName"></param>
        /// <param name="fieldsTerminated"></param>
        public int load_txt_to_db(string destTableName, string destDBName, string txtPath, string fieldsTerminated)
        {
            #region 基础校验
            if (_isError == true)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(destTableName))
            {
                WriteErrorMessage("目标表名为空，【load_txt_to_db】语句将不会执行任何内容。", 3, "");
                return 0;
            }

            //获取目的数据库
            BF_DATABASE.Entity destDBServer = BF_DATABASE.Instance.GetDbByName(destDBName);
            //验证当前是否打开数据库
            if (destDBServer == null)
            {
                WriteErrorMessage(string.Format(@"目的数据库【{0}】不存在，【load_txt_to_db】将不会进行任何操作;", destDBName));
                return 0;
            }
            #endregion

            try
            {
                Enums.DBType dbTypeDest = (Enums.DBType)destDBServer.DB_TYPE;
                using (BDBHelper dbHelper = new BDBHelper(dbTypeDest.ToString(), destDBServer.IP, destDBServer.PORT, destDBServer.USER_NAME, destDBServer.PASSWORD, destDBServer.DB_NAME, destDBServer.DB_NAME))
                {
                    return dbHelper.LoadTxtToTable(destTableName, txtPath, fieldsTerminated);
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(string.Format(@"导入文件到库【{0}】失败，错误信息为：\r\n【{1}】", destDBServer.NAME, ex.ToString()), 3, "");
                return 0;
            }

        }
        #endregion

        #region 常用函数定义

        /// <summary>
        /// 获取基准时间的日期，精确到天
        /// </summary>
        /// <param name="days">偏移时间天数，默认为0，即当天日期</param>
        /// <returns>格式yyyyMMdd</returns>
        public string day(int days = 0)
        {
            return _referenceDateTime.AddDays(days).ToString("yyyyMMdd");
        }

        /// <summary>
        /// 获取日期为当月的第几天
        /// </summary>
        /// <param name="days">偏移时间天数，默认为0，即当天日期的号数</param>
        /// <returns>即当天日期的号数</returns>
        public int day_of_month(int days = 0)
        {
            return _referenceDateTime.AddDays(days).Day;
        }

        /// <summary>
        /// 获取日期为当月的第几天
        /// </summary>
        /// <param name="days">偏移时间天数，默认为0，即当天日期的号数</param>
        /// <returns>即当天日期的号数，两位的字符串</returns>
        public string day_of_month2(int days = 0)
        {
            return _referenceDateTime.AddDays(days).Day.ToString("00");
        }

        /// <summary>
        /// 获取基准时间当月的最后一天的日期
        /// </summary>
        /// <param name="months">
        /// 输入参数有三种情况：
        /// 1.参数为空时，获得当前月最后一天的值
        /// 2.参数为N时，获得的时间是当前月向后偏移N月的最后一天的值
        /// 3.参数为-N时，获得的时间是当前月向后偏移N月的最后一天的值
        /// </param>
        /// <returns>yyyyMMdd格式的字符串</returns>
        public string last_day(int months = 0)
        {
            DateTime date = _referenceDateTime.AddMonths(months + 1);
            return date.AddDays(-date.Day).ToString("yyyyMMdd");
        }

        /// <summary>
        /// 获取基准时间的日期，精确到月
        /// </summary>
        /// <param name="months">
        /// 输入参数有三种情况：
        /// 1.参数为空时，获得当前月的值
        /// 2.参数为N时，获得的时间是当前月向后偏移N月后的值
        /// 3.参数为-N时，获得的时间是当前月向后偏移N月后的值
        /// </param>
        /// <returns>yyyyMM格式的年月值</returns>
        public string month(int months = 0)
        {
            return _referenceDateTime.AddMonths(months).ToString("yyyyMM");
        }

        /// <summary>
        /// 获取基准时间中当年的第几个月
        /// </summary>
        /// <param name="months">
        /// 输入参数有三种情况：
        /// 1.参数为空时，获得当前月的值
        /// 2.参数为N时，获得的时间是当前月向后偏移N月后的值
        /// 3.参数为-N时，获得的时间是当前月向后偏移N月后的值
        /// </param>
        /// <returns>月</returns>
        public int month_of_year(int months = 0)
        {
            return _referenceDateTime.AddMonths(months).Month;
        }

        /// <summary>
        /// 获取基准时间中当年的第几个月
        /// </summary>
        /// <param name="months">
        /// 输入参数有三种情况：
        /// 1.参数为空时，获得当前月的值
        /// 2.参数为N时，获得的时间是当前月向后偏移N月后的值
        /// 3.参数为-N时，获得的时间是当前月向后偏移N月后的值
        /// </param>
        /// <returns>月（使用两位数字返回）</returns>
        public string month_of_year2(int months = 0)
        {
            return _referenceDateTime.AddMonths(months).Month.ToString("00");
        }

        /// <summary>
        /// 获取基准时间的日期，精确到年
        /// </summary>
        /// <param name="years">
        /// 输入参数有三种情况：
        /// 1.参数为空时，获得当前时间的年份
        /// 2.参数为N时，获得的时间是当前年份向后偏移N年后的值
        /// 3.参数为-N时，获得的时间是当前年份向前偏移N年后的值
        /// </param>
        /// <returns>年份（四位数字）</returns>
        public int year(int years)
        {
            return _referenceDateTime.Year + years;
        }

        #endregion

        #region 自定义函数

        /// <summary>
        /// 获取地市ID
        /// </summary>
        /// <param name="cityDbUserName">地市数据库用户名</param>
        /// <returns></returns>
        public int get_city_id(string cityDbUserName)
        {
            switch (cityDbUserName)
            {
                case "CD_USER":
                    return 2; // 成都
                case "MY_USER":
                    return 3; // 绵阳
                case "ZG_USER":
                    return 4; // 自贡
                case "PZH_USER":
                    return 5; // 攀枝花
                case "GY_USER":
                    return 6; // 广元
                case "DZ_USER":
                    return 7; // 达州
                case "LZ_USER":
                    return 8; // 泸州
                case "GA_USER":
                    return 9; // 广安
                case "BZ_USER":
                    return 10; // 巴中
                case "SN_USER":
                    return 11; // 遂宁
                case "YB_USER":
                    return 12; // 宜宾
                case "NJ_USER":
                    return 13; // 内江
                case "ZY_USER":
                    return 14; // 资阳
                case "LS_USER":
                    return 15; // 乐山
                case "YA_USER":
                    return 16; // 雅安
                case "DY_USER":
                    return 17; // 德阳
                case "NC_USER":
                    return 18; // 南充
                case "MS_USER":
                    return 19; // 眉山
                case "AB_USER":
                    return 20; // 阿坝
                case "GZ_USER":
                    return 21; // 甘孜
                case "XC_USER":
                    return 22; // 凉山
                case "TFXQ_USER":
                    return 87; // 天府新区
            }
            return 0;
        }

        /// <summary>
        /// 获取地市在BOSS的数据库名
        /// </summary>
        /// <param name="cityDbUserName">地市数据库用户名</param>
        /// <returns></returns>
        public string get_boss_dbname(string cityDbUserName)
        {
            switch (cityDbUserName)
            {
                case "CD_USER":
                    return "68";    //成都	a
                case "MY_USER":
                    return "CSCDM07";   //绵阳	b
                case "ZG_USER":
                    return "CSCDM07";   //自贡	c
                case "PZH_USER":
                    return "CSCDM06";   //攀枝花	d
                case "GY_USER":
                    return "CSCDM07";   //广元	e
                case "DZ_USER":
                    return "CSCDM06";   //达州	f
                case "LZ_USER":
                    return "CSCDM06";   //泸州	g
                case "GA_USER":
                    return "CSCDM06";   //广安	h
                case "BZ_USER":
                    return "CSCDM06";   //巴中	i
                case "SN_USER":
                    return "CSCDM07";   //遂宁	j
                case "YB_USER":
                    return "CSCDM06";   //宜宾	k
                case "NJ_USER":
                    return "CSCDM07";   //内江	l
                case "ZY_USER":
                    return "CSCDM06";   //资阳	m
                case "LS_USER":
                    return "CSCDM07";   //乐山	n
                case "YA_USER":
                    return "CSCDM07";   //雅安	p
                case "DY_USER":
                    return "CSCDM07";   //德阳	q
                case "NC_USER":
                    return "CSCDM07";   //南充	r
                case "MS_USER":
                    return "CSCDM06";   //眉山	s
                case "AB_USER":
                    return "CSCDM07";   //阿坝	u
                case "GZ_USER":
                    return "CSCDM06";   //甘孜	v
                case "XC_USER":
                    return "CSCDM06";	//凉山	w
            }
            log("注意！获取到的数据库名为空，如果继续使用setnowdb()选择数据库的话，会选中本地默认数据库");
            return string.Empty;
        }


        /// <summary>
        /// 获取地市在ESOP的数据库名
        /// </summary>
        /// <param name="cityDbUserName">地市数据库用户名</param>
        /// <returns></returns>
        public string get_esop_dbname(string cityDbUserName)
        {
            switch (cityDbUserName)
            {
                case "PZH_USER":
                    return "CJJXC72";   //攀枝花	5
                case "GZ_USER":
                    return "CJJXC72";   //甘孜	21
                case "YB_USER":
                    return "CJJXC72";   //宜宾	12
                case "XC_USER":
                    return "CJJXC72";   //凉山	22
                case "LZ_USER":
                    return "CJJXC72";   //泸州	8
                case "BZ_USER":
                    return "CJJXC72";   //巴中	10
                case "MS_USER":
                    return "CJJXC72";   //眉山	19
                case "GA_USER":
                    return "CJJXC72";   //广安	9
                case "DZ_USER":
                    return "CJJXC72";   //达州	7
                case "ZY_USER":
                    return "CJJXC72";   //资阳	14
                case "MY_USER":
                    return "CJJXC73";   //绵阳	3
                case "NJ_USER":
                    return "CJJXC73";   //内江	13
                case "DY_USER":
                    return "CJJXC73";   //德阳	17
                case "AB_USER":
                    return "CJJXC73";   //阿坝	20
                case "SN_USER":
                    return "CJJXC73";   //遂宁	11
                case "ZG_USER":
                    return "CJJXC73";   //自贡	4
                case "GY_USER":
                    return "CJJXC73";   //广元	6
                case "LS_USER":
                    return "CJJXC73";   //乐山	15
                case "YA_USER":
                    return "CJJXC73";   //雅安	16
                case "NC_USER":
                    return "CJJXC73";	//南充	18
            }

            log("注意！获取到的数据库名为空，如果继续使用setnowdb()选择数据库的话，会选中本地默认数据库");
            return string.Empty;
        }

        /// <summary>
        /// 根据地市数据库USER获取地市编码（简写，同车牌号首字母）
        /// 函数名实在难取，暂时用这个。
        /// </summary>
        /// <param name="cityDbUserName">地市数据库用户名</param>
        /// <returns></returns>
        public string get_atoz(string cityDbUserName)
        {
            switch (cityDbUserName)
            {
                case "CD_USER":
                    return "a"; //成都	2
                case "MY_USER":
                    return "b"; //绵阳	3
                case "ZG_USER":
                    return "c"; //自贡	4
                case "PZH_USER":
                    return "d"; //攀枝花	5
                case "GY_USER":
                    return "e"; //广元	6
                case "DZ_USER":
                    return "f"; //达州	7
                case "LZ_USER":
                    return "g"; //泸州	8
                case "GA_USER":
                    return "h"; //广安	9
                case "BZ_USER":
                    return "i"; //巴中	10
                case "SN_USER":
                    return "j"; //遂宁	11
                case "YB_USER":
                    return "k"; //宜宾	12
                case "NJ_USER":
                    return "l"; //内江	13
                case "ZY_USER":
                    return "m"; //资阳	14
                case "LS_USER":
                    return "n"; //乐山	15
                case "YA_USER":
                    return "p"; //雅安	16
                case "DY_USER":
                    return "q"; //德阳	17
                case "NC_USER":
                    return "r"; //南充	18
                case "MS_USER":
                    return "s"; //眉山	19
                case "AB_USER":
                    return "u"; //阿坝	20
                case "GZ_USER":
                    return "v"; //甘孜	21
                case "XC_USER":
                    return "w";	//凉山	22
            }
            return string.Empty;
        }

        #endregion
    }
}