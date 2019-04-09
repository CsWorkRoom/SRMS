using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    public class AfScriptNodeController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int self = 0)
        {
            ViewBag.SHOW_SELF = self;
            GetDatabaseSelectData();
            GetScriptTypeSelectData();//加载下拉
            GetStatus();//运行状态
            return View();
        }

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string typeId = "", string dbId = "", int self=0, string orderByField = "sn.ID", string orderByType = "ASC")
        {
            int count = 0;
            DataTable data = BF_ST_NODE.Instance.GetDataTable(limit, page, ref count, name, typeId, dbId, self, orderByField, orderByType);
            return ToJsonString(data, count);
        }
        #endregion

        #region 查看详细
        public ActionResult Look(int id = 0)
        {
            return Edit(id);
        }
        #endregion

        #region 加载编辑及新增
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.UserName = BLL.FW.BF_USER.Instance.GetDictionary("ID", "NAME");
            BLL.FW.BF_ST_NODE.Entity entity = new BLL.FW.BF_ST_NODE.Entity();

            if (id > 0)
            {
                entity = BF_ST_NODE.Instance.GetEntityByKey<BLL.FW.BF_ST_NODE.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "任务名称不存在";
                    entity = new BLL.FW.BF_ST_NODE.Entity();
                    entity.ID = -1;
                }

            }
            GetStatus();//运行状态
            GetDatabaseSelectData();//数据库下拉
            GetScriptTypeSelectData();//类型加载下拉
            ModelState.Clear();
            return View(entity);
        }
        #endregion

        #region 启动
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="id">节点任务ID</param>
        /// <param name="sdate">基准日期</param>
        /// <param name="bdate">开始日期</param>
        /// <param name="edate">结束日期</param>
        /// <param name="para">可选参数</param>
        /// <returns>启动结果</returns>
        public ActionResult Start(int id = 0, string sdate = "", string bdate = "", string edate = "", string para = "")
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "任务启动失败";
            string message = "";
            try
            {
                if (id <= 0)
                {
                    throw new Exception("任务ID不正确");
                }
                if (string.IsNullOrWhiteSpace(sdate))
                {
                    throw new Exception("基准日期不可为空");
                }
                if (string.IsNullOrWhiteSpace(bdate) || string.IsNullOrWhiteSpace(edate))
                {
                    throw new Exception("起止日期不可为空");
                }

                DateTime referenceDate = DateTime.Parse(sdate);
                DateTime beginDate = DateTime.Parse(bdate);
                DateTime endDate = DateTime.Parse(edate);

                if (beginDate.Year != endDate.Year || beginDate.Month != endDate.Month)
                {
                    throw new Exception("任务不可以跨月");
                }

                int i = 0;
                if (string.IsNullOrWhiteSpace(para))
                {
                    result.IsSuccess = BF_ST_NODE.Instance.Start(id, referenceDate, beginDate, endDate, para, ref message);
                    result.Message = message;
                    i = result.IsSuccess ? 1 : 0;
                }
                else
                {
                    int n = 0;
                    foreach (string p in para.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        n++;
                        i += BF_ST_NODE.Instance.Start(id, referenceDate, beginDate, endDate, p, ref message) ? 1 : 0;
                    }
                    result.IsSuccess = i > 0;
                    result.Message = "根据参数创建了" + i + "个子任务";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.ToString();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Stop(int id = 0)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "任务停止失败";
            string message = "";
            if (id > 0)
            {
                result.IsSuccess = BF_ST_NODE.Instance.Stop(id, ref message);
                result.Message = message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 编辑提交
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModel"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BLL.FW.BF_ST_NODE.Entity entity, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();
            ViewBag.UserName = BF_USER.Instance.GetDictionary("ID", "NAME");
            #region 验证
            if (string.IsNullOrEmpty(entity.NAME))
            {
                result.Message = "任务名称不能为空，提交失败！";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion
                       
            int intResult = 0;
            int intUserId = SystemSession.UserID;//当前登录

            #region 封装数据
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("NAME", entity.NAME);
            dic.Add("TYPE_ID", entity.TYPE_ID);
            dic.Add("DB_ID", entity.DB_ID);
            dic.Add("CONTENT", entity.CONTENT);
            dic.Add("REMARK", entity.REMARK);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", intUserId);

            #endregion

            try
            {
                if (entity.ID > 0) //增加与修改的判断
                {
                    //修改
                    intResult = BF_ST_NODE.Instance.UpdateByKey(dic, entity.ID);
                }
                else
                {
                    //增加
                    dic.Add("CREATE_TIME", DateTime.Now);
                    dic.Add("CREATE_UID", intUserId);
                    intResult = BF_ST_NODE.Instance.Add(dic);
                }
                result.IsSuccess = intResult > 0 ;
                result.Message = intResult > 0 ? "数据提交成功！" : "数据提交失败！";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message; ;
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
            result.Message = "任务中含有子任务，不能删除。删除失败！";
            int FlowCount = BLL.FW.BF_ST_FLOW_NODE.Instance.GetCount(" NODE_ID = ?", id);
            if (FlowCount > 0)
            {
                result.Message = "任务组中包含该任务,请先取消!";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            int intCount = BLL.FW.BF_ST_NODE.Instance.GetCount("CONTENT=?", id);
            if (intCount == 0)
            {
                result.IsSuccess = BLL.FW.BF_ST_NODE.Instance.DeleteByKey(id) > 0;
                if (result.IsSuccess)
                    result.Message = "删除完成。";
                else
                    result.Message = "程序异常。删除失败！";

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 加载脚本类型下拉
        private void GetScriptTypeSelectData()
        {
            DataTable dt = BLL.FW.BF_ST_TYPE.Instance.GetTable();
            if (dt != null)
            {
                var obj = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(
                        new
                        {
                            id = Convert.ToInt32(dr["ID"]),
                            pId = Convert.ToInt32(dr["PID"]),
                            name = dr["NAME"].ToString()
                        });
                }
                ViewBag.ScriptTypeSelect = SerializeObject(obj);
            }
            else
            {
                ViewBag.ScriptTypeSelect = "[]";
            }
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;

        }
        #endregion

        #region 加载数据库下拉
        private void GetDatabaseSelectData()
        {
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
        }
        #endregion

        #region 运行状态
        private void GetStatus()
        {
            #region 状态
            IDictionary dic = new Dictionary<int, string>();
            foreach (Enums.RunStatus source in Enum.GetValues(typeof(Common.FW.Enums.RunStatus)))
                dic.Add((int)source, source.ToString());
            ViewBag.Status = dic;
            #endregion
        }
        #endregion

        #region 导出文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string name = "", string typeId = "", string dbId = "", int self=0)
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_脚本任务_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

                string where = "1=1";
                List<object> param = new List<object>();
                #region 添加参数
                if (string.IsNullOrWhiteSpace(name) == false)
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";

                if (string.IsNullOrWhiteSpace(typeId) == false)
                {
                    where += " AND TYPE_ID = ?";
                    param.Add(typeId);
                }
                if (string.IsNullOrWhiteSpace(dbId) == false)
                {
                    where += " AND DB_ID = ?";
                    param.Add(dbId);
                }
                #endregion
                int count = 0;
                var dt = BF_ST_NODE.Instance.GetDataTable(0, 0, ref count, name, typeId, dbId, self);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns.Remove("ID");
                dt.Columns.Remove("LAST_TASK_ID");
                dt.Columns["NAME"].Caption = "任务名称";
                dt.Columns["TYPENAME"].Caption = "类型";
                dt.Columns["TASKNAME"].Caption = "最新任务";
                dt.Columns["DBNAME"].Caption = "数据库名称";
                dt.Columns["RUN_STATUS"].Caption = "运行状态";
                dt.Columns["LAST_TASK_IS"].Caption = "是否成功";
                dt.Columns["LAST_TASK_ST"].Caption = "开始时间";
                dt.Columns["LAST_TASK_FT"].Caption = "结束时间";
                dt.Columns["CREATE_NAME"].Caption = "创建者";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                dt.Columns["UPDATE_TIME"].Caption = "更新时间";
                dt.Columns["UPDATE_NAME"].Caption = "更新者";

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

        #region 备份

        /// <summary>
        /// 备份
        /// </summary>
        /// <returns></returns>
        public ActionResult Backups()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
            string fileName = string.Format("口径脚本备份_{0}.sql", DateTime.Now.ToString("yyyyMMddHHmmss"));
            try
            {
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                DataTable dt = BF_ST_NODE.Instance.GetTableFields("ID, NAME, DB_ID, CONTENT");

                if (dt == null || dt.Rows.Count < 1)
                {
                    return ShowAlert("没有脚本");
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path + fileName, false, Encoding.UTF8))
                {
                    sw.WriteLine("/// #####################################################################");
                    sw.WriteLine(string.Format("/// {0} 的口径脚本备份", BF_SYS_CONFIG.SystemName));
                    sw.WriteLine(string.Format("/// 本文件于 {0} 由 {1} 备份生成", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SystemSession.UserName));
                    sw.WriteLine(string.Format("/// 本文件共包含 {0} 个脚本，以下为脚本内容：", dt.Rows.Count));
                    sw.WriteLine("/// #####################################################################");
                    sw.WriteLine();
                    sw.WriteLine();
                    foreach (DataRow dr in dt.Rows)
                    {
                        int id = Convert.ToInt32(dr["ID"]);
                        sw.WriteLine(string.Format("/// {0} 开始 >>>>> ************************************************************ >>>>>", id));
                        sw.WriteLine("/// ID:" + Convert.ToInt32(dr["ID"]));
                        sw.WriteLine("/// NAME:" + Convert.ToString(dr["NAME"]));
                        sw.WriteLine("/// DB_ID:" + Convert.ToInt32(dr["DB_ID"]));
                        sw.WriteLine("/// 以下是脚本内容");
                        sw.WriteLine(Convert.ToString(dr["CONTENT"]));
                        sw.WriteLine(string.Format("/// {0} 结束 <<<<< ************************************************************ <<<<<", id));
                        sw.WriteLine();
                        sw.WriteLine();
                    }
                }

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
                BLog.Write(BLog.LogLevel.WARN, "备份脚本代码到文件出错:" + ex.ToString());
                return ShowAlert("备份脚本代码到文件出错：" + ex.Message);
            }
            finally
            {
                if (System.IO.File.Exists(path + fileName))
                {
                    try
                    {
                        System.IO.File.Delete(path + fileName);
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