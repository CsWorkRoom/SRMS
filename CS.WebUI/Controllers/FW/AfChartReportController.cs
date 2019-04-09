using CS.Base.Log;
using CS.Library.Export;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace CS.WebUI.Controllers.FW
{
    public class AfChartReportController : ABaseController
    {
        // GET: AfChartReport
        public ActionResult Index()
        {
            return View();
        }

        #region 新增或编辑图表
        /// <summary>
        /// 新增或编辑图表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            
            BF_CHART_REPORT.Entity entity = new BF_CHART_REPORT.Entity();
            entity.IS_SHOW_EXPORT = 1;
            entity.IS_SHOW_DEBUG = 1;

            if (id > 0)
            {
                entity = BF_CHART_REPORT.Instance.GetEntityByKey<BF_CHART_REPORT.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "报表不存在";
                    entity = new BF_CHART_REPORT.Entity();
                    entity.ID = -1;
                }
            }

            ModelState.Clear();
            return View(entity);
        }
        #endregion

        #region 编辑提交
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BF_CHART_REPORT.Entity entity)
        {
            if (!string.IsNullOrEmpty(entity.CHART_CODE))
            {
                string chartCode = Base.Encrypt.BAES.Decrypt(entity.CHART_CODE);
                entity.CHART_CODE = string.IsNullOrEmpty(chartCode) ? " " : chartCode;
            }
            if (!string.IsNullOrEmpty(entity.TOP_CODE))
            {
                string topCode = Base.Encrypt.BAES.Decrypt(entity.TOP_CODE);
                entity.TOP_CODE = string.IsNullOrEmpty(topCode) ? " " : topCode;
            }
            if (!string.IsNullOrEmpty(entity.BOTTOM_CODE))
            {
                string bottomCode = Base.Encrypt.BAES.Decrypt(entity.BOTTOM_CODE);
                entity.BOTTOM_CODE = string.IsNullOrEmpty(bottomCode) ? " " : bottomCode;
            }

            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                CheckInput(entity);

                if (entity.ID < 0)
                {
                    result.Message = "图表不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (entity.ID == 0)
                {
                    entity.IS_ENABLE = 1;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    entity.UPDATE_UID = SystemSession.UserID;
                    entity.UPDATE_TIME = DateTime.Now;
                    i = BF_CHART_REPORT.Instance.Add(entity);
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("NAME", entity.NAME.Trim());
                    dic.Add("DB_ID", entity.DB_ID);
                    dic.Add("IS_SHOW_EXPORT", entity.IS_SHOW_EXPORT);
                    dic.Add("IS_SHOW_DEBUG", entity.IS_SHOW_DEBUG);
                    //dic.Add("IS_SHOW_CHECKBOX", entity.IS_SHOW_CHECKBOX);
                    dic.Add("SQL_CODE", entity.SQL_CODE);
                    dic.Add("REMARK", entity.REMARK);
                    dic.Add("TOP_CODE", entity.TOP_CODE);
                    dic.Add("BOTTOM_CODE", entity.BOTTOM_CODE);
                    dic.Add("UPDATE_UID", SystemSession.UserID);
                    dic.Add("UPDATE_TIME", DateTime.Now);

                    dic.Add("CHART_CODE", entity.CHART_CODE);//CHART代码体
                    i = BF_CHART_REPORT.Instance.UpdateByKey(dic, entity.ID);
                }

                if (i < 1)
                {
                    result.Message = "提交失败！出现了未知错误";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Message = "提交成功！";
                }


            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "编辑图表出错：" + ex.ToString());
            }
            
            ViewBag.Result = result.IsSuccess;
            ViewBag.Message = result.Message;

            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            ModelState.Clear();
            return View(entity);
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
            result.Message = "参数有误！请联系管理员";
            if (id > 0)
            {
                result.IsSuccess = BLL.FW.BF_CHART_REPORT.Instance.DeleteByKey(id) > 0;
                if (result.IsSuccess)
                    result.Message = "删除完成。";
                else
                    result.Message = "程序异常。删除失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 验证输入
        /// <summary>
        /// 验证输入
        /// </summary>
        /// <param name="entity"></param>
        private void CheckInput(BF_CHART_REPORT.Entity entity)
        {
            if (entity == null)
            {
                throw new Exception("对象为空");
            }
            if (string.IsNullOrWhiteSpace(entity.NAME))
            {
                throw new Exception("图表名称不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.SQL_CODE))
            {
                throw new Exception("SQL语句不可为空");
            }
            if (string.IsNullOrWhiteSpace(entity.CHART_CODE))
            {
                throw new Exception("CHART代码不可为空");
            }

            if (BF_CHART_REPORT.Instance.IsDuplicate(entity.ID, "NAME", entity.NAME))
            {
                throw new Exception("图表名称 " + entity.NAME + " 已经存在");
            }
        }
        #endregion

        #region 以json串返回sql查询结果集(含内置参数的替换)
        /// <summary>
        /// 以json串返回sql查询结果集(含内置参数的替换)
        /// </summary>
        /// <param name="sqlE"></param>
        /// <returns></returns>
        public ActionResult GetDataBySql(Models.FW.SqlExcute sqlE)
        {
            BF_CHART_REPORT.Entity entity = new BF_CHART_REPORT.Entity();
            entity.ID = 0;
            entity.DB_ID = sqlE.DB_ID;
            entity.SQL_CODE = sqlE.SQL_CODE;
            
            List<object> paraList = new List<object>();
            
            string sql = BF_CHART_REPORT.GenerateSQL(entity, null, out paraList);
            DataTable dt = BF_CHART_REPORT.QueryTable(entity, sql, null, 0, 1);

            return Content(JSON.DecodeToStr(dt));
        }
        #endregion

        #region 报表展示
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Show(int id)
        {
            if (id < 1)
            {
                return ShowAlert("请选择正确的报表");
            }

            try
            {
                BF_CHART_REPORT.Entity entity = BF_CHART_REPORT.Instance.GetEntityByKey<BF_CHART_REPORT.Entity>(id);
                if (entity == null)
                {
                    return ShowAlert("报表不存在");
                }
                if (entity.IS_ENABLE != 1)
                {
                    return ShowAlert("报表已被禁用");
                }
                ViewBag.DB_ID = entity.DB_ID;
                ViewBag.SQL_CODE = Base.Encrypt.BAES.Encrypt(entity.SQL_CODE);
                ViewBag.CHART_CODE = Base.Encrypt.BAES.Encrypt(entity.CHART_CODE);
                ViewBag.TOP_CODE = entity.TOP_CODE;
                ViewBag.BOTTOM_CODE = entity.BOTTOM_CODE;
                return View();
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "查询报表[" + id + "]出现未知错误：" + ex.Message);
                return ShowAlert("查询报表出现未知错误：" + ex.Message);
            }
        }
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="id"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string orderByField = "CR.ID", string orderByType = "DESC")
        {
            int count = 0;
            DataTable data = BF_CHART_REPORT.Instance.GetDataTable(limit, page, ref count, name, orderByField, orderByType);
            return ToJsonString(data, count);
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
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_图形报表_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");

                #region 添加参数
                string orderByField = "CR.ID";
                string orderByType = "DESC";
                List<object> param = new List<object>();
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(name) == false)
                    where += " and CR.NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                #endregion
                int count = 0;
                DataTable dt = BF_CHART_REPORT.Instance.GetDataTable(0, 0, ref count, name, orderByField, orderByType);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns.Remove("ID");
                dt.Columns["NAME"].Caption = "名称";
                dt.Columns["DBNAME"].Caption = "数据库名称";
                dt.Columns["CHART_TYPE"].Caption = "类别";
                dt.Columns["SHOWEXPORT"].Caption = "导出";
                dt.Columns["SHOWDEBUG"].Caption = "调试";
                dt.Columns["SQL_CODE"].Caption = "SQL";
                dt.Columns["IS_ENABLE"].Caption = "启用";
                dt.Columns["UPDATE_TIME"].Caption = "更新时间";

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
        
    }
}