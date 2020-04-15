using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.WebUI.Models.FW;
using CS.BLL.SR;
using System.Text;
using CS.Base.DBHelper;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 课题结题申请
    /// </summary>
    public class SrTopicEndController : ABaseController
    {
        public string Modular = "课题结题申请";

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            SR_TOPIC_END.Entity ent = new SR_TOPIC_END.Entity();
            ent.END_STATUS = -1;//默认未选择

            #region 01.结题申请表
            if (id > 0)
            {
                ent = SR_TOPIC_END.Instance.GetEntityByKey<SR_TOPIC_END.Entity>(id);
            }
            #endregion

            #region 02.课题下拉(树状结构)
            var list = SR_TOPIC.Instance.GetTopicTree();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.icon = ApplicationPath + item.icon;
                }
            }
            //课题下拉树
            ViewBag.TopicSelect = SerializeObject(list);
            #endregion

            #region 03.科研成功状态下拉
            var endStatusSelectList = Common.Fun.GetSelectListFromEnum<Common.Enums.TopicEndStatus>();
            ViewBag.EndStatusSelectList = endStatusSelectList;
            #endregion

            return View(ent);
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(SR_TOPIC_END.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                //throw new Exception("抛出错误");
                #region 00.数据校验(暂未实现)
                //校验经费总金额与清单经费金额之和是否相等
                #endregion

                int entId = ent.ID;
                #region 01.保存结题申请
                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                if (ent.ID == 0)
                {
                    entId = SR_TOPIC_END.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = entId;
                    SR_TOPIC_END.Instance.Add(ent);
                }
                else
                {
                    SR_TOPIC_END.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion

                result.IsSuccess = true;
                result.Result = entId.ToString();
                var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(ent.TOPIC_ID);
                result.FlowCaseName = topic.NAME + "结题申请-"+ entId.ToString();
                result.Message = "填报结题申请信息成功！";
            }
            catch (Exception ex)
            {
                string err = string.Format(@"填报结题申请信息失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "填报结题申请", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}