using CS.Base.DBHelper;
using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 数据库管理
    /// </summary>
    public class AfDBController : ABaseController
    {
        #region 编辑配置

        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //数据库类型
            Dictionary<int, string> dicDbType = new Dictionary<int, string>();
            foreach (Enums.DBType source in Enum.GetValues(typeof(Enums.DBType)))
            {
                dicDbType.Add((int)source, source.ToString());
            }
            ViewBag.DIC_DB_TYPE = dicDbType;

            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="type"></param>
        /// <param name="orderByField"></param>
        /// <param name="orderByType"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string ip = "", int type = 0, string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(ip) == false)
            {
                where += " AND IP LIKE '%" + ip.Replace('\'', ' ') + "%'";
            }
            if (type > 0)
            {
                where += " AND DB_TYPE=" + type;
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);
            IList<BF_DATABASE.Entity> list = BF_DATABASE.Instance.GetListPage<BF_DATABASE.Entity>(limit, page, order, where);
            int count = BF_DATABASE.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BF_DATABASE.Entity>();
                count = 0;
            }
            return ToJsonString(list, count);
        }

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            //数据库类型
            Dictionary<int, string> dicDbType = new Dictionary<int, string>();
            foreach (Enums.DBType source in Enum.GetValues(typeof(Enums.DBType)))
            {
                dicDbType.Add((int)source, source.ToString());
            }
            ViewBag.DIC_DB_TYPE = dicDbType;
            BF_DATABASE.Entity entity = new BF_DATABASE.Entity();

            if (id > 0)
            {
                entity = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(id);
                //对密码进行aes解密
                entity.PASSWORD = CS.Base.Encrypt.BAES.Decrypt(entity.PASSWORD);
                if (entity == null)
                {
                    return ShowAlert("配置项不存在");
                }
            }

            return View(entity);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(BF_DATABASE.Entity entity)
        {
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                CheckInput(entity);

                if (entity.ID < 0)
                {
                    result.Message = "数据库不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //对密码进行aes加密
                string password = CS.Base.Encrypt.BAES.Encrypt(entity.PASSWORD);
                entity.PASSWORD = password;
                entity.UPDATE_UID = SystemSession.UserID;
                entity.UPDATE_TIME = DateTime.Now;
                if (entity.ID == 0)
                {
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    i = BF_DATABASE.Instance.Add(entity);
                }
                else
                {
                    BF_DATABASE.Entity oldEntity = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(entity.ID);
                    entity.CREATE_UID = oldEntity.CREATE_UID;
                    entity.CREATE_TIME = oldEntity.CREATE_TIME;
                    i = BF_DATABASE.Instance.UpdateByKey<BF_DATABASE.Entity>(entity, entity.ID);
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
                BLog.Write(BLog.LogLevel.WARN, "编辑数据库配置出错：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <param name="entity"></param>
        private void CheckInput(BF_DATABASE.Entity entity)
        {
            if (entity == null)
            {
                throw new Exception("对象为空");
            }
            if (string.IsNullOrWhiteSpace(entity.NAME))
            {
                throw new Exception("数据库名称不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.IP))
            {
                throw new Exception("数据库IP地址不可为空");
            }
            if (entity.PORT < 1)
            {
                throw new Exception("端口号输入不正确");
            }
            if (string.IsNullOrWhiteSpace(entity.USER_NAME))
            {
                throw new Exception("用户名不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.NAME))
            {
                throw new Exception("密码不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.PASSWORD))
            {
                throw new Exception("数据库密码不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.DB_NAME))
            {
                throw new Exception("数据库实例名不可为空");
            }

            if (BF_DATABASE.Instance.IsDuplicate(entity.ID, "NAME", entity.NAME))
            {
                throw new Exception("数据库名称 " + entity.NAME + " 已经存在");
            }
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
                int i = BF_DATABASE.Instance.DeleteByKey(id);
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

        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string name = "", string ip = "", int type = 0)
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_数据库管理_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                if (string.IsNullOrWhiteSpace(ip) == false)
                {
                    where += " AND IP LIKE '%" + ip.Replace('\'', ' ') + "%'";
                }
                if (type > 0)
                {
                    where += " AND DB_TYPE=" + type;
                }

                var dt = BF_DATABASE.Instance.GetTableFields("NAME,DB_TYPE,IP,PORT,USER_NAME,DB_NAME,REMARK,UPDATE_TIME", where);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }

                dt.Columns["NAME"].Caption = "数据库名称";
                dt.Columns["DB_TYPE"].Caption = "数据库类型";
                dt.Columns["IP"].Caption = "IP地址";
                dt.Columns["PORT"].Caption = "端口号";
                dt.Columns["USER_NAME"].Caption = "用户名";
                dt.Columns["DB_NAME"].Caption = "实例名";
                dt.Columns["REMARK"].Caption = "备注";
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

        #region 通用查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public ActionResult Query()
        {
            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            return View();
        }

        /// <summary>
        /// 查询数据库所有表
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="isImport">是否用于外导</param>
        /// <returns>List(string)的JSON串</returns>
        [HttpPost]
        public string GetTableList(int dbID, bool isImport = false)
        {
            try
            {
                List<string> list = BF_DATABASE.Instance.GetTableList(dbID, isImport);
                return SerializeObject(list);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "获取数据库" + dbID + "中的表名列表出错:" + ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 获取数据表字段列表
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="tableName">表名</param>
        /// <returns>字段列表</returns>
        [HttpPost]
        public string GetFieldsList(int dbID, string tableName)
        {
            try
            {
                List<BF_FIELD.Entity> list = BF_DATABASE.Instance.GetFieldsList(dbID, tableName);
                return SerializeObject(list);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "获取数据库" + dbID + "中表" + tableName + "的字段列表出错:" + ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="dbID">数据库ID</param>
        /// <param name="sql">SQL语句</param>
        /// <param name="pageSize">分页大小（默认为10，0表示不分页）</param>
        /// <param name="pageIndex">页号（从1开始）</param>
        /// <param name="isExecuteRowsCount">是否计算记录条数</param>
        /// <param name="psd">二次密码验证（执行DDL语句时需要）</param>
        /// <returns>适用于Layui的DataTable转JSON串</returns>
        [HttpPost]
        public string ExecuteSelectSQL(int dbID, string sql, int pageSize = 10, int pageIndex = 1, bool isExecuteRowsCount = false, string psd = "")
        {
            if (sql.Trim().ToUpper().StartsWith("SELECT "))
            {
                int rowsCount = isExecuteRowsCount ? 0 : -1;
                try
                {
                    DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(dbID, sql, null, ref rowsCount, pageSize, pageIndex);
                    return ToJsonString(dt, rowsCount);
                }
                catch (Exception ex)
                {
                    BLog.Write(BLog.LogLevel.WARN, "在数据库" + dbID + "中执行SQL查询语句：\r\n" + sql + "\r\n出错：" + ex.ToString());
                    return ToJsonString(new DataTable(), 0, 1, "查询失败：" + ex.Message);
                }
            }
            else
            {
                JsonResultData result = new JsonResultData();
                try
                {
                    int i = BF_DATABASE.Instance.ExecuteSQL(dbID, psd, sql, null);
                    result.IsSuccess = true;
                    result.Message = string.Format("执行SQL语句成功，影响了【{0}】条记录", i);
                }
                catch (Exception ex)
                {
                    BLog.Write(BLog.LogLevel.WARN, "在数据库" + dbID + "中执行SQL语句：\r\n" + sql + "\r\n出错：" + ex.ToString());
                    result.Message = string.Format("执行SQL语句失败：" + ex.Message);
                }

                return JsonConvert.SerializeObject(result);
            }
        }

        #endregion

        #region 导出查询结果

        /// <summary>
        /// 导出查询结果
        /// </summary>
        /// <param name="dbid"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportQuery(int dbid = 0, string sql = "")
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
            string fileName = string.Format("查询结果导出_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
            try
            {
                BLog.Write(BLog.LogLevel.DEBUG, SystemSession.UserName + "将导出查询结果到文件：" + path + fileName + "，SQL为：\r\n" + sql);
                if (string.IsNullOrWhiteSpace(sql))
                {
                    return ShowAlert("请输入查询的SQL语句");
                }
                if (sql.ToUpper().StartsWith("SELECT ") == false)
                {
                    return ShowAlert("只支持SELECT语句查询");
                }
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string sq = System.Web.HttpUtility.UrlDecode(sql);
                DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(dbid, sq, null, 0, 1);
                if (dt == null)
                {
                    ShowAlert("没有查到结果");
                }

                BLog.Write(BLog.LogLevel.DEBUG, "要导的记录数为：" + dt.Rows.Count);

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path + fileName, false, Encoding.UTF8))
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        sw.Write(col.ColumnName + "\t");
                    }
                    sw.WriteLine();
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            sw.Write(Convert.ToString(dr[col]) + "\t");
                        }
                        sw.WriteLine();
                    }
                }

                System.IO.FileInfo fi = new System.IO.FileInfo(path + fileName);
                BLog.Write(BLog.LogLevel.DEBUG, "文件已经生成成功，文件大小为：" + fi.Length / 1024 + "KB，即将开始下载");

                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
                System.Web.HttpContext.Current.Response.WriteFile(path + fileName);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "导出查询结果出错:" + ex.ToString());
                return ShowAlert("导出查询结果出错：" + ex.Message);
            }
            finally
            {
                if (System.IO.File.Exists(path + fileName))
                {
                    try
                    {
                        System.IO.File.Delete(path + fileName);
                        BLog.Write(BLog.LogLevel.DEBUG, "导出数据的临时文件：" + path + fileName + "已经清除");
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return null;
        }
        #endregion
    }
}