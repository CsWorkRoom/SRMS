using CS.Base.Log;
using CS.BLL.FW;
using CS.WebUI.Models.FW;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace CS.WebUI.Controllers
{
    /// <summary>
    /// JSON返回对象
    /// </summary>
    public class JsonResultData
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回结果详情（一般可以不使用）
        /// </summary>
        public string Result { get; set; }
    }

    /// <summary>
    /// 文件上传管理
    /// </summary>
    public class UpFileController : Controller
    {
        public string Modular = "文件上传管理（服务于文件上传分布视图）";

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(string pathName, HttpPostedFileBase file)
        {
            JsonResultData result = new JsonResultData();
            if (file != null)
            {
                #region  附件上传
                //文件上传
                string fileName = string.Empty;
                try
                {
                    #region 保存文件到物理路径
                    string fileDisplayName = Path.GetFileName(file.FileName);
                    string path = Base.Config.BConfig.GetConfigToString(pathName);//获取文件目录

                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = new DirectoryInfo(path).FullName;
                    //格式说明：{时间}-{用户工号}-{文件字节大小}_{文件原名}
                    fileName = string.Format("{0}-{1}-{2}_&&_{3}", DateTime.Now.ToString("yyyyMMdd-HHmmss"), SystemSession.UserName, file.ContentLength, fileDisplayName);
                    string saveName = string.Format("{0}\\{1}", path, fileName);
                    file.SaveAs(saveName);
                    #endregion

                    #region 保存信息到文件表 fileEntity
                    int fileId = CS.BLL.SR.SR_FILES.Instance.GetNextValueFromSeqDef();//主键ID
                    CS.BLL.SR.SR_FILES.Entity fileEntity = new BLL.SR.SR_FILES.Entity()
                    {
                        ID = fileId,
                        CREATE_TIME = DateTime.Now,
                        CREATE_UID = SystemSession.UserID,
                        IS_DELETE = 0,
                        DISPLAY_NAME = fileDisplayName,
                        REAL_NAME = fileName,
                        FILE_SIZE = file.ContentLength,
                        FORMAT = Path.GetExtension(file.FileName),
                        PATH = path
                    };
                    CS.BLL.SR.SR_FILES.Instance.Add(fileEntity);//保存
                    #endregion

                    result.IsSuccess = true;
                    result.Message = "保存文件成功";
                    result.Result = SerializeObject(fileEntity);
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
                result.IsSuccess = false;
                result.Message = "客户端未上载文件！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 序列化格式设置
        /// </summary>
        private static JsonSerializerSettings _jsonSerializerSettings = null;
        /// <summary>
        /// 序列化为JSON字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string SerializeObject(object data)
        {
            if (_jsonSerializerSettings == null)
            {
                _jsonSerializerSettings = new JsonSerializerSettings();
                _jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }

            //转为JSON字符串
            return JsonConvert.SerializeObject(data, _jsonSerializerSettings);
        }
    }
}