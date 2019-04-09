using CS.Base.Log;
using CS.BLL.FW;
using CS.WebUI.Models.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 公告管理
    /// </summary>
    public class AfBulletinController : ABaseController
    {
        public string Modular = "公告管理";
        /// <summary>
        /// 附件存放的目录
        /// </summary>
        public string AttachmentPath
        {
            get
            {
                return Base.Config.BConfig.GetConfigToString("AttachmentPath");
            }
        }
        // GET: AfBulletin
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="title"></param>
        /// <param name="summary"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string title = "", string summary = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (string.IsNullOrWhiteSpace(title) == false)
            {
                where += " AND TITLE LIKE '%" + title.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(summary) == false)
            {
                where += " AND SUMMARY LIKE '%" + summary.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);
            List<Models.FW.BulletinModelsForList> umList = new List<Models.FW.BulletinModelsForList>();
            IList<BLL.FW.BF_BULLETIN.Entity> ueList = BLL.FW.BF_BULLETIN.Instance.GetListPage(limit, page, order, where);
            int count = BLL.FW.BF_BULLETIN.Instance.GetCount(where);
            if (ueList != null && ueList.Count > 0)
            {

                foreach (BLL.FW.BF_BULLETIN.Entity ue in ueList)
                {
                    Models.FW.BulletinModelsForList um = new Models.FW.BulletinModelsForList();
                    um.ID = ue.ID;
                    um.TITLE = ue.TITLE;
                    um.SUMMARY = ue.SUMMARY;
                    um.IS_ENABLE = ue.IS_ENABLE;
                    um.CONTENT = ue.CONTENT;
                    um.CREATE_UID_NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(ue.CREATE_UID, "NAME");
                    um.CREATE_TIME = ue.CREATE_TIME;
                    umList.Add(um);
                }
            }

            return ToJsonString(umList, count);
        }

        # region 编辑及新增
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.DIC_DEPTS = BLL.FW.BF_DEPARTMENT.Instance.GetDictionary("ID", "NAME");
            ViewBag.DIC_ROLES = BLL.FW.BF_ROLE.Instance.GetDictionary("ID", "NAME");
            BLL.FW.BF_BULLETIN.Entity model = new BLL.FW.BF_BULLETIN.Entity();
            if (id > 0)
            {
                 model = BLL.FW.BF_BULLETIN.Instance.GetEntityByKey<BLL.FW.BF_BULLETIN.Entity>(id);
                if (model != null)
                {
                }
                else
                {
                    ViewBag.Message = "公告不存在！";
                    model.ID = -1;
                }
            }

            #region 加载下拉列表
            DataTable dt = BLL.FW.BF_DEPARTMENT.Instance.GetTable();
            var obj = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = Convert.ToInt32(dr["DEPT_CODE"]),
                        pId = Convert.ToInt32(dr["P_CODE"]),
                        name = dr["NAME"].ToString(),
                        value = Convert.ToInt32(dr["ID"])
                    });
            }

            ViewBag.DepartmentSelect = SerializeObject(obj);
            #endregion

            #region 加载附件列表
            string fileIds = "";
            var fileList = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetList<BLL.FW.BF_BULLETIN_ATTACH.Entity>("BULL_ID=?", id);
            List<BulletinFileModel> list = new List<BulletinFileModel>();
            foreach(var item in fileList)
            {
                fileIds += item.FILE_PATH + ",";
                BulletinFileModel fileModel = new Models.FW.BulletinFileModel();
                fileModel.ID = item.ID;
                fileModel.BULL_ID = item.BULL_ID;
                fileModel.FILE_PATH = item.FILE_PATH;
                fileModel.FILE_NAME = Path.GetFileName(item.FILE_PATH);
                list.Add(fileModel);

            }
            ViewBag.FileId = fileIds;
            ViewBag.FileList = list;
            #endregion
            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BLL.FW.BF_BULLETIN.Entity model, FormCollection collection, HttpPostedFileBase file)
        {
            JsonResultData result = new JsonResultData();
            if (file != null)
            {
                #region  公告附件上传
                //文件上传
                string tableName = "BF_BULLETIN";
                string fileName = string.Empty;
                try
                {
                    fileName = Path.GetFileName(file.FileName);
                    string path = string.Format("{0}/{1}", AttachmentPath, tableName);

                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = new DirectoryInfo(path).FullName;
                    string saveName = string.Format("{0}\\{1}-{2}_{3}", path, DateTime.Now.ToString("yyyyMMdd-HHmmss"), SystemSession.UserName, fileName);
                    file.SaveAs(saveName);
                    result.IsSuccess = true;
                    result.Message = saveName;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                }
                #endregion
            }
            else
            {
                #region 新增或编辑公告

                model.RECV_ROLE_IDS = collection["ROLES"];
                //文件上传地址
                string fileIds= collection["FileId"];
                var fileList = fileIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> path = new List<string>();
                foreach(var item in fileList)
                {
                    path.Add(item);
                }
                int i = 0;
                int id = model.ID;
                if (model.RECV_DEPT_IDS == null)
                {
                    model.RECV_DEPT_IDS = "";
                }
                if (model.RECV_ROLE_IDS == null)
                {
                    model.RECV_ROLE_IDS = "";
                }
                try
                {
                    if (id > 0)
                    {
                        //编辑
                        model.UPDATE_UID = SystemSession.UserID;
                        model.UPDATE_TIME = DateTime.Now;
                        i = BLL.FW.BF_BULLETIN.Instance.UpdateBulletin(model, path);
                    }
                    else
                    {
                        //新增
                        model.CREATE_UID = SystemSession.UserID;
                        model.CREATE_TIME = DateTime.Now;
                        model.UPDATE_UID = SystemSession.UserID;
                        model.UPDATE_TIME = DateTime.Now;
                        model.IS_ENABLE = 1;
                        i = BLL.FW.BF_BULLETIN.Instance.AddBulletin(model, path);
                    }

                    if (i > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "保存成功";
                    }
                    else
                    {
                        result.Message = "出现了未知错误";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                    BLog.Write(BLog.LogLevel.WARN, "编辑公告出错：" + ex.ToString());
                }
                #endregion
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetEnable(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                int i = 0;
                i = BLL.FW.BF_BULLETIN.Instance.SetEnable(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "启用成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "公告启用程序出现错误：" + ex.ToString());
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
            try
            {
                int i = 0;
                i = BLL.FW.BF_BULLETIN.Instance.SetUnable(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "禁用成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "公告禁用程序出现错误：" + ex.ToString());
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
            try
            {
                int i = 0;
                i = BLL.FW.BF_BULLETIN.Instance.DeleteBullIten(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "删除公告成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "删除公告程序出现错误：" + ex.ToString());
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除公告附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult deleteFile(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                //删除文件
                string tableName = "BF_BULLETIN";
                var model = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetEntityByKey<BLL.FW.BF_BULLETIN_ATTACH.Entity>(id);
                string filename = Path.GetFileName(model.FILE_PATH);
                string path = string.Format("{0}/{1}", AttachmentPath, tableName);
                string fullName = path + "/" + filename;
                if (System.IO.File.Exists(fullName) == false)
                {
                    // "文件不存在";
                }
                FileInfo fi = new FileInfo(fullName);
                fi.Delete();
               
                //删除表数据
                int i = 0;
                i = BLL.FW.BF_BULLETIN_ATTACH.Instance.DeleteByKey(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                result.IsSuccess = true;
                result.Message = "删除公告附件成功！";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 查看公告
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aid"></param>
        /// <returns></returns>
        public ActionResult Show(int id, int aid = 0)
        {          
            BLL.FW.BF_BULLETIN.Entity entity = BLL.FW.BF_BULLETIN.Instance.GetEntityByKey<BLL.FW.BF_BULLETIN.Entity>(id);
            if (entity==null)
            {
                return ShowAlert("公告不存在！");
            }
           
            #region 加载附件列表
            var fileList = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetList<BLL.FW.BF_BULLETIN_ATTACH.Entity>("BULL_ID=?", id);
            List<BulletinFileModel> list = new List<BulletinFileModel>();
            foreach (var item in fileList)
            {
                BulletinFileModel fileModel = new Models.FW.BulletinFileModel();
                fileModel.ID = item.ID;
                fileModel.BULL_ID = item.BULL_ID;
                fileModel.FILE_PATH = item.FILE_PATH;
                fileModel.FILE_NAME = Path.GetFileName(item.FILE_PATH);
                list.Add(fileModel);
            }
            ViewBag.FileList = list;
            #endregion
            if (aid < 1)
            {
                return View(entity);
            }
            else
            {
                //下载文件
                string tableName = "BF_BULLETIN";
                var model = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetEntityByKey<BLL.FW.BF_BULLETIN_ATTACH.Entity>(aid);
                if (model == null)
                {
                    return ShowAlert("附件不存在！");
                }
                string filename = Path.GetFileName(model.FILE_PATH);
                string path = string.Format("{0}/{1}", AttachmentPath, tableName);
                string fullName = path + "/" + filename;
                if (System.IO.File.Exists(fullName) == false)
                {
                    return ShowAlert("文件不存在！");
                }
                int index = filename.IndexOf('_');
                string name = filename.Substring(index + 1);
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
                System.Web.HttpContext.Current.Response.WriteFile(fullName);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
                return null;
            }
        }

        /// <summary>
        /// 根据文件名称下载文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ActionResult Download(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                string tableName =  "BF_BULLETIN";
                var model = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetEntityByKey<BLL.FW.BF_BULLETIN_ATTACH.Entity>(id);
                string filename = Path.GetFileName(model.FILE_PATH);

                string path = string.Format("{0}/{1}", AttachmentPath, tableName);

                //下载单个文件
                if (string.IsNullOrWhiteSpace(filename) == false)
                {
                    string fullName = path + "/" + filename;
                    if (System.IO.File.Exists(fullName) == false)
                    {
                        result.IsSuccess = false;
                        result.Message = "文件不存在！";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    int index = filename.IndexOf('_');
                    string name = filename.Substring(index + 1);

                    System.Web.HttpContext.Current.Response.Buffer = true;
                    System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                    System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
                    System.Web.HttpContext.Current.Response.WriteFile(fullName);
                    System.Web.HttpContext.Current.Response.Flush();
                    System.Web.HttpContext.Current.Response.End();

                    result.IsSuccess = true;
                    result.Message = "下载成功！";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "文件不存在！";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
        }

        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string title = "", string summary = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_字段显示配置_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(title) == false)
                {
                    where += " AND title LIKE '%" + title.Replace('\'', ' ') + "%'";
                }
                if (string.IsNullOrWhiteSpace(summary) == false)
                {
                    where += " AND summary LIKE '%" + summary.Replace('\'', ' ') + "%'";
                }

                var dt = BLL.FW.BF_BULLETIN.Instance.GetTableFields("NAME,FULL_NAME,IS_ENABLE,IS_LOCKED,LOGIN_COUNT,LAST_LOGIN_TIME", where);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["ID"].Caption = "ID";
                dt.Columns["TITLE"].Caption = "标题";
                dt.Columns["SUMMARY"].Caption = "摘要";
                dt.Columns["IS_ENABLE"].Caption = "启用状态";
                dt.Columns["CREATE_UID"].Caption = "创建人ID";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                Library.Export.ExcelFile export = new Library.Export.ExcelFile(path);
                string fullname = export.ToExcel(dt);
                if (string.IsNullOrWhiteSpace(fullname) == true)
                {
                    return ShowAlert("未生成Excel文件");
                }
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                System.Web.HttpContext.Current.Response.WriteFile(fullname);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
                //删除文件
                export.Delete(filename);
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