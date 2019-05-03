using CS.Base.Log;
using CS.BLL.FW;
using CS.WebUI.Controllers.FW;
using CS.WebUI.Models.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.SR
{
    public class SrTopicController : ABaseController
    {
        // GET: SrTopic
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_TOPIC.Entity model = new BLL.SR.SR_TOPIC.Entity();
            if (id > 0)
            {
                model = BLL.SR.SR_TOPIC.Instance.GetEntityByKey<BLL.SR.SR_TOPIC.Entity>(id);
                if (model != null)
                {
                }
                else
                {
                    ViewBag.Message = "课题不存在！";
                    model.ID = -1;
                }
            }

            #region 加载下拉列表
            DataTable dt = BLL.SR.SR_TOPIC_TYPE.Instance.GetTable();
            var obj = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = dr["ID"],
                        pId = dr["PARENT_ID"],
                        name = dr["NAME"].ToString(),
                        value = Convert.ToInt32(dr["ID"])
                    });
            }

            ViewBag.TypeSelect = SerializeObject(obj);
            #endregion

            #region 加载附件列表
            string fileIds = "";
            var fileList = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetList<BLL.FW.BF_BULLETIN_ATTACH.Entity>("BULL_ID=?", id);
            List<BulletinFileModel> list = new List<BulletinFileModel>();
            foreach (var item in fileList)
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
        /// 根据关键字获取用户列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
     
        public string SearchUser(string keyword)
        {
            string where="";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where = string.Format(" FULL_NAME LIKE '%{0}%'", keyword);
            }
           
            IList<BLL.FW.BF_USER.Entity> ueList = BLL.FW.BF_USER.Instance.GetListPage(10, 1,null, where);
            return SerializeObject(ueList);
        }
    }
}