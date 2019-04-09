using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 附件管理
    /// </summary>
    public class AfFileController : ABaseController
    {
        public string Modular = "附件管理";

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

            IList<BF_FILE.Entity> list = BF_FILE.Instance.GetListPage<BF_FILE.Entity>(limit, page, order, where);
            int count = BF_FILE.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BF_FILE.Entity>();
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
            //文件类型
            Dictionary<int, string> dicFileType = new Dictionary<int, string>();
            Dictionary<int, string> dicFileTypeSelect = new Dictionary<int, string>();
            foreach (Enums.AcceptUploadFileType source in Enum.GetValues(typeof(Enums.AcceptUploadFileType)))
            {
                dicFileType.Add((int)source, source.ToString());
            }
            ViewBag.DIC_FILE_TYPE = dicFileType;
            ViewBag.DIC_FILE_TYPE_SELECT = dicFileTypeSelect;

            //数据库
            ViewBag.DIC_DBS = BF_DATABASE.Instance.GetDictionary();
            //创建表模式
            Dictionary<int, string> dicCreateTableMode = new Dictionary<int, string>();
            foreach (Enums.CreateTableMode item in Enum.GetValues(typeof(Enums.CreateTableMode)))
            {
                dicCreateTableMode.Add((int)item, item.ToString());
            }
            ViewBag.DIC_CREATE_TABLE_MODE = dicCreateTableMode;
            BF_FILE.Entity entity = new BF_FILE.Entity();
            entity.DB_ID = -1;
            entity.IS_ALLOW_DELETE = 1;

            if (id > 0)
            {
                entity = BF_FILE.Instance.GetEntityByKey<BF_FILE.Entity>(id);
                if (entity == null)
                {
                    return ShowAlert("配置项不存在");
                }
                if (string.IsNullOrWhiteSpace(entity.ACCEPT_FILE_TYPES) == false)
                {
                    string[] types = entity.ACCEPT_FILE_TYPES.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string type in types)
                    {
                        int t = 0;
                        if (int.TryParse(type, out t) && dicFileTypeSelect.ContainsKey(t) == false)
                        {
                            if (t == 0)
                            {
                                dicFileTypeSelect = new Dictionary<int, string>();
                                dicFileTypeSelect.Add(0, "");
                                break;
                            }
                            dicFileTypeSelect.Add(t, "");
                        }
                    }
                }
            }

            ViewBag.DIC_FILE_TYPE_SELECT = dicFileTypeSelect;
            return View(entity);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(BF_FILE.Entity entity, FormCollection collection)
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
                if (string.IsNullOrWhiteSpace(collection["ACCEPT_FILE_TYPES"]) == true)
                {
                    entity.ACCEPT_FILE_TYPES = Enums.AcceptUploadFileType.不限.GetHashCode().ToString();
                }
                else
                {
                    entity.ACCEPT_FILE_TYPES = collection["ACCEPT_FILE_TYPES"];
                }

                if (entity.ID == 0)
                {
                    entity.IS_ENABLE = 1;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    i = BF_FILE.Instance.Add(entity);
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("NAME", entity.NAME);
                    dic.Add("DB_ID", entity.DB_ID);
                    dic.Add("TABLE_NAME", entity.TABLE_NAME);
                    dic.Add("CREATE_TABLE_MODE", entity.CREATE_TABLE_MODE);
                    dic.Add("KEY_FIELD", entity.KEY_FIELD);
                    dic.Add("IS_ALLOW_DELETE", entity.IS_ALLOW_DELETE);
                    dic.Add("ACCEPT_FILE_TYPES", entity.ACCEPT_FILE_TYPES);
                    dic.Add("REMARK", entity.REMARK);
                    dic.Add("UPDATE_UID", entity.UPDATE_UID);
                    dic.Add("UPDATE_TIME", entity.UPDATE_TIME);
                    i = BF_FILE.Instance.UpdateByKey(dic, entity.ID);
                }

                if (i < 1)
                {
                    result.Message = "出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.WARN, false, Modular, "编辑", "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "编辑附件配置出错：" + ex.ToString());

                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, "编辑", "", (entity.ID > 0 ? "修改" : "添加") + "了ID为" + entity.ID + "的配置");
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
                int i = BF_FILE.Instance.SetEnable(id);
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
                int i = BF_FILE.Instance.SetUnable(id);
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
                int i = BF_FILE.Instance.DeleteByKey(id);
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

        #region 附件上传

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="rowkey">记录主键值</param>
        /// <param name="yyyymmdd">如果是按日期为后缀的动态表，则需要传入该表日期</param>
        /// <returns></returns>
        public ActionResult Upload(int id, string rowkey, string yyyymmdd = "")
        {
            ViewBag.ID = id;
            ViewBag.ROW_KEY = rowkey;
            ViewBag.YYYYMMDD = yyyymmdd;
            string mimes = string.Empty;
            string exts = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(AttachmentPath))
                {
                    return ShowAlert("请通知管理员在配置文件中配置附件存放路径：AttachmentPath");
                }

                string tableName = string.Empty;
                BF_FILE.Entity entity = BF_FILE.Instance.CheckUpload(id, rowkey, yyyymmdd, out tableName);

                BF_FILE.GetAcceptMimeTypes(entity, out mimes, out exts);
            }
            catch (Exception ex)
            {
                return ShowAlert(ex.Message);
            }

            ViewBag.MIME_TYPES = mimes;
            ViewBag.EXT_NAMES = exts;

            return View();
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="rowkey">记录主键值</param>
        /// <param name="file">上传的文件</param>
        /// <param name="yyyymmdd">如果是按日期为后缀的动态表，则需要传入该表日期</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(int id, string rowkey, HttpPostedFileBase file, string yyyymmdd = "")
        {
            JsonResultData result = new JsonResultData();
            string tableName = string.Empty;
            string fileName = string.Empty;
            try
            {
                //验证
                BF_FILE.Entity entity = BF_FILE.Instance.CheckUpload(id, rowkey, yyyymmdd, out tableName);
                fileName = Path.GetFileName(file.FileName);

                //string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/up/");
                string path = string.Format("{0}/{1}/{2}", AttachmentPath, tableName, rowkey);

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                path = new DirectoryInfo(path).FullName;
                string saveName = string.Format("{0}\\{1}-{2}_{3}", path, DateTime.Now.ToString("yyyy-MM-dd HH=mm=ss"), SystemSession.UserName, fileName);

                file.SaveAs(saveName);

                result.IsSuccess = true;
                result.Message = "上传附件成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, "上传附件", "", "为表：" + tableName + " 上传了附件", "文件名:" + fileName);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "上传附件【" + id + "】出错：" + ex.ToString());
                WriteOperationLog(BLog.LogLevel.WARN, false, Modular, "上传附件", "", "为表：" + tableName + " 上传附件出错", "文件名:" + fileName);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 附件查看及下载、删除

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="rowkey">记录主键值</param>
        /// <param name="yyyymmdd">如果是按日期为后缀的动态表，则需要传入该表日期</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public ActionResult Download(int id, string rowkey, string yyyymmdd = "", string filename = "")
        {
            ViewBag.ID = id;
            ViewBag.ROW_KEY = rowkey;
            ViewBag.YYYYMMDD = yyyymmdd;
            ViewBag.FILE_JSON = string.Empty;
            List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();

            try
            {
                string tableName = string.Empty;
                //验证
                BF_FILE.Entity entity = BF_FILE.Instance.CheckUpload(id, rowkey, yyyymmdd, out tableName);

                string path = string.Format("{0}/{1}/{2}", AttachmentPath, tableName, rowkey);

                //下载单个文件
                if (string.IsNullOrWhiteSpace(filename) == false)
                {
                    string fullName = path + "/" + filename;
                    if (System.IO.File.Exists(fullName) == false)
                    {
                        return ShowAlert("文件不存在");
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

                    WriteOperationLog(BLog.LogLevel.INFO, true, Modular, "下载附件", "", "下载表：" + tableName + " 的附件", "文件名:" + filename);
                }
                else
                {
                    if (Directory.Exists(path) == true)
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            Dictionary<string, object> file = new Dictionary<string, object>();
                            string name = fi.Name;
                            string time = name.Length > 19 ? name.Substring(0, 19).Replace('=', ':') : "";
                            int index = name.IndexOf('_');
                            string user = index > 20 ? name.Substring(20, (index - 20)) : "";
                            file.Add("上传时间", time);
                            file.Add("上传者", user);
                            file.Add("文件名", name.Substring(index + 1));
                            file.Add("原文件名", name);
                            file.Add("是否本人", entity.IS_ALLOW_DELETE == 1 && user == SystemSession.UserName);

                            files.Add(file);
                        }

                        //JSON序列化
                        ViewBag.FILE_JSON = SerializeObject(files);
                    }
                }
            }
            catch (Exception ex)
            {
                return ShowAlert(ex.Message);
            }
            return View();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="rowkey">记录主键值</param>
        /// <param name="yyyymmdd">如果是按日期为后缀的动态表，则需要传入该表日期</param>
        /// <param name="mode">请求模式（down/del）</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFile(int id, string rowkey, string yyyymmdd = "", string filename = "")
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return ShowAlert("文件名不可为空");
            }
            string tableName = string.Empty;

            JsonResultData result = new JsonResultData();
            try
            {
                //验证
                BF_FILE.Entity entity = BF_FILE.Instance.CheckUpload(id, rowkey, yyyymmdd, out tableName);
                if (entity.IS_ALLOW_DELETE != 1)
                {
                    result.Message = entity.NAME + "的附件不可删除，若要删除，请联系管理员修改相应配置。";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string path = string.Format("{0}/{1}/{2}", AttachmentPath, tableName, rowkey);
                string fullName = path + "/" + filename;
                if (System.IO.File.Exists(fullName) == false)
                {
                    result.Message = "文件不存在";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                FileInfo fi = new FileInfo(fullName);
                string name = fi.Name;
                int index = name.IndexOf('_');
                string user = index > 20 ? name.Substring(20, (index - 20)) : "";
                if (user != SystemSession.UserName)
                {
                    result.Message = "不可以删除他人上传的附件，若要强制删除，请联系管理员。";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                fi.Delete();
                result.IsSuccess = true;
                result.Message = "删除文件成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, "删除附件", "", "删除表：" + tableName + " 的附件", "文件名:" + filename);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "删除附件【" + id + "】的文件【" + filename + "】出错：" + ex.ToString());
                WriteOperationLog(BLog.LogLevel.WARN, false, Modular, "删除附件", "", "删除表：" + tableName + " 的附件失败", "文件名:" + filename);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}