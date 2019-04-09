using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 数据外导
    /// </summary>
    public class AfImportController : ABaseController
    {
        #region 配置

        // GET: AfImport
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            //创建表模式
            Dictionary<int, string> dicCreateTableMode = new Dictionary<int, string>();
            foreach (Enums.CreateTableMode item in Enum.GetValues(typeof(Enums.CreateTableMode)))
            {
                dicCreateTableMode.Add((int)item, item.ToString());
            }
            ViewBag.DIC_CREATE_TABLE_MODE = dicCreateTableMode;

            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string name = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BF_IMPORT.Entity> list = BF_IMPORT.Instance.GetListPage<BF_IMPORT.Entity>(limit, page, order, where);
            int count = BF_IMPORT.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BF_IMPORT.Entity>();
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
            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            //创建表模式
            Dictionary<int, string> dicCreateTableMode = new Dictionary<int, string>();
            foreach (Enums.CreateTableMode item in Enum.GetValues(typeof(Enums.CreateTableMode)))
            {
                dicCreateTableMode.Add((int)item, item.ToString());
            }
            ViewBag.DIC_CREATE_TABLE_MODE = dicCreateTableMode;
            BF_IMPORT.Entity entity = new BF_IMPORT.Entity();
            entity.DB_ID = -1;

            if (id > 0)
            {
                entity = BF_IMPORT.Instance.GetEntityByKey<BF_IMPORT.Entity>(id);
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
        public JsonResult Edit(BF_IMPORT.Entity entity)
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
                    i = BF_IMPORT.Instance.Add(entity);
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("NAME", entity.NAME);
                    dic.Add("DB_ID", entity.DB_ID);
                    dic.Add("TABLE_NAME", entity.TABLE_NAME);
                    dic.Add("CREATE_TABLE_MODE", entity.CREATE_TABLE_MODE);
                    dic.Add("FIELDS", entity.FIELDS);
                    dic.Add("IS_ALLOW_UPDATE", entity.IS_ALLOW_UPDATE);
                    dic.Add("REMARK", entity.REMARK);
                    dic.Add("UPDATE_UID", entity.UPDATE_UID);
                    dic.Add("UPDATE_TIME", entity.UPDATE_TIME);
                    i = BF_IMPORT.Instance.UpdateByKey(dic, entity.ID);
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

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetEnable(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "启用成功";
            try
            {
                int i = BF_IMPORT.Instance.SetEnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "启用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetUnable(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = true;
            result.Message = "禁用成功";
            try
            {
                int i = BF_IMPORT.Instance.SetUnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "禁用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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
                int i = BF_IMPORT.Instance.DeleteByKey(id);
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

        #endregion

        #region 使用

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="isdownload">是否为下载样本</param>
        /// <returns></returns>
        public ActionResult Import(int id, bool isdownload = false)
        {
            ViewBag.ID = id;
            BF_IMPORT.Entity entity = BF_IMPORT.Instance.GetEntityByKey<BF_IMPORT.Entity>(id);
            if (entity == null)
            {
                return ShowAlert("未找到配置" + id);
            }
            Enums.CreateTableMode ctMode = (Enums.CreateTableMode)entity.CREATE_TABLE_MODE;
            ViewBag.CREATE_TABLE_MODE = ctMode.GetHashCode();
            ViewBag.IS_SHOW_BASE_DATE = !(ctMode == Enums.CreateTableMode.指定表 || ctMode == Enums.CreateTableMode.用户ID后缀);
            ViewBag.TABLE_NAME = entity.TABLE_NAME + "(" + ctMode.ToString() + ")";
            if (isdownload == true)
            {
                return ImportDownload(entity);
            }

            return View();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="date">基准日期（默认为当天）</param>
        /// <param name="file">文件</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Import(int id, string date, HttpPostedFileBase file)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                string fileName = Path.GetFileName(file.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();
                DateTime baseDate = DateTime.Today;
                if (string.IsNullOrWhiteSpace(date) == false)
                {
                    DateTime.TryParse(date, out baseDate);
                }

                if (fileExtension != ".xlsx")
                {
                    result.Message = "上传的文件格式错误，只能上传后缀为.xlsx的Excel文件";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                BF_IMPORT.Entity entity = BF_IMPORT.Instance.GetEntityByKey<BF_IMPORT.Entity>(id);
                if (entity == null)
                {
                    result.Message = "未找到配置" + id;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/up/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string saveName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + SystemSession.UserID + "_" + fileName;

                file.SaveAs(path + saveName);
                string message = string.Empty;
                Dictionary<int, string> errorList = new Dictionary<int, string>();
                int n = BF_IMPORT.ImportDataFile(entity, path + saveName, baseDate, out message, out errorList);
                if (n < 1)
                {
                    BLog.Write(BLog.LogLevel.WARN, "外导数据【" + id + "】失败：" + message);
                }
                result.IsSuccess = n > 0;
                result.Message = message;
                result.Result = "";
                if (errorList != null && errorList.Count > 0)
                {
                    result.Result = string.Join("\r\n", errorList);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "外导数据【" + id + "】出错：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 样表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private ActionResult ImportDownload(BF_IMPORT.Entity entity)
        {
            try
            {
                List<string> commentList = new List<string>();
                DataTable dt = BF_IMPORT.GetSampleTable(entity, out commentList);
                string filename = HttpUtility.UrlEncode(string.Format("{0}_{1}.xlsx", entity.NAME, DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                Library.Export.ExcelFile export = new Library.Export.ExcelFile(path);
                string fullName = export.ToExcel(dt);
                if (string.IsNullOrWhiteSpace(fullName) == true || System.IO.File.Exists(fullName) == false)
                {
                    return ShowAlert("未生成Excel文件");
                }

                IWorkbook workBook;
                using (FileStream fs = System.IO.File.OpenRead(fullName))
                {
                    workBook = new XSSFWorkbook(fs);
                    ISheet sheet = workBook.GetSheetAt(0);
                    IRow row = sheet.GetRow(0);

                    XSSFDrawing patriarch = (XSSFDrawing)sheet.CreateDrawingPatriarch();
                    for (int c = 0; c < row.Cells.Count; c++)
                    {
                        if (string.IsNullOrWhiteSpace(commentList[c]) == false)
                        {
                            IComment comment = patriarch.CreateCellComment(new XSSFClientAnchor(0, 50, 0, 50, c, 0, c + 3, 5));
                            comment.Author = "编辑提示";
                            comment.String = new XSSFRichTextString("【编辑提示】\r\n" + commentList[c]);
                            row.Cells[c].CellComment = comment;
                        }
                    }
                }

                //重写文件
                using (FileStream fs = System.IO.File.Create(fullName))
                {
                    workBook.Write(fs);
                    fs.Close();
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
                ShowAlert("生成样表出错：" + ex.Message);
            }
            return View();
        }

        #endregion

        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string name = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_数据外导配置_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }

                var dt = BF_IMPORT.Instance.GetTable(where);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }

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
    }
}