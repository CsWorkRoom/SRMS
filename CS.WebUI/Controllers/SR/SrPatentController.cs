using CS.Base.Log;
using CS.BLL.FW;
using CS.WebUI.Controllers.FW;
using CS.WebUI.Models.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CS.BLL.SR;
using Newtonsoft.Json;
using CS.WebUI.Models.SR;

namespace CS.WebUI.Controllers.SR
{
    public class SrPatentController : ABaseController
    {
        public string Modular = "课题管理";
        // GET: SrTopic
        public ActionResult Index()
        {
            return View();
        }

        #region 专利成果管理
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_PATENT.Entity model = new BLL.SR.SR_PATENT.Entity();
            if (id > 0)
            {
                model = BLL.SR.SR_PATENT.Instance.GetEntityByKey<BLL.SR.SR_PATENT.Entity>(id);
                if (model != null)
                {
                }
                else
                {
                    ViewBag.Message = "专利成果不存在！";
                    model.ID = -1;
                   
                }
            }

            #region 加载专利类型树形下拉列表
            DataTable dt = BLL.SR.SR_PATENT_TYPE.Instance.GetTable();
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


            #region 加载学科类型树形下拉列表
            DataTable dtXue = BLL.SR.SR_SUBJECT.Instance.GetTable();
            var xks = new List<object>();
            foreach (DataRow dr in dtXue.Rows)
            {
                xks.Add(
                    new
                    {
                        id = dr["ID"],
                        pId = dr["PARENT_ID"],
                        name = dr["NAME"].ToString(),
                        value = Convert.ToInt32(dr["ID"])
                    });
            }

            ViewBag.SubjectSelect = SerializeObject(xks);
            #endregion

            #region 加载课题
            ViewBag.Topics=SR_TOPIC.Instance.GetDictionary();
            #endregion

            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(SR_PATENT.Entity entity)
        {
            int topicId = entity.ID;
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "成果专利项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (entity.ID == 0)
                {
                    topicId = SR_PATENT.Instance.GetNextValueFromSeqDef();
                    entity.ID = topicId;
                    entity.CREATE_USER_ID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    SR_PATENT.Instance.Add(entity, true);
                }
                else
                {
                    i = SR_PATENT.Instance.UpdateByKey(entity, entity.ID);//修改
                }
                result.IsSuccess = true;
                result.Message = "保存成功";
                result.Result = topicId.ToString();
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的成果专利成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 流程处理页面
        public ActionResult FlowEdit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_PATENT.Entity model =
                BLL.SR.SR_PATENT.Instance.GetEntityByKey<BLL.SR.SR_PATENT.Entity>(id);
            BLL.SR.SR_PATENT_TYPE.Entity patentType = BLL.SR.SR_PATENT_TYPE.Instance.GetEntityByKey<BLL.SR.SR_PATENT_TYPE.Entity>(model.TYPE_ID);
            ViewBag.TypeName = patentType.NAME;
            BLL.SR.SR_TOPIC.Entity topic = BLL.SR.SR_TOPIC.Instance.GetEntityByKey<BLL.SR.SR_TOPIC.Entity>(model.TOPIC_ID);
            ViewBag.TopicName = topic.NAME;
            BLL.SR.SR_SUBJECT.Entity subject = BLL.SR.SR_SUBJECT.Instance.GetEntityByKey<BLL.SR.SR_SUBJECT.Entity>(model.SUBJECT_ID);
            ViewBag.SubjectName = subject.NAME;

            ModelState.Clear();
            return View(model);
        }
        #endregion

    }
}