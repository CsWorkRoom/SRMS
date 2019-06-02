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

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 课题完善信息
    /// </summary>
    public class SrTopicDetailController : ABaseController
    {
        public string Modular = "课题完善信息";

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="topicId">课题编号</param>
        /// <returns></returns>
        public ActionResult Edit(int topicId=0)
        {
            if (topicId <= 0)
            {
                ShowAlert(string.Format("课题编号[{0}]应大于0", topicId));
            }

            var ent = SR_TOPIC_DETAIL.Instance.GetEntity<SR_TOPIC_DETAIL.Entity>("TOPIC_ID=?", topicId);
            if(ent==null||ent.ID<1)
            {
                ent = new SR_TOPIC_DETAIL.Entity();
            }

            #region 初始赋值
            ent.TOPIC_ID = topicId;
            #endregion

            #region 三个树
            #region 学科下拉项:SubjectSelect
            DataTable dt = SR_SUBJECT.Instance.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                var obj = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(
                        new
                        {
                            id = Convert.ToInt32(dr["ID"]),
                            pId = string.IsNullOrWhiteSpace(dr["PARENT_ID"].ToString()) ? "" : dr["PARENT_ID"],
                            name = dr["NAME"].ToString(),
                            value = Convert.ToInt32(dr["ID"])
                        });
                }
                ViewBag.SubjectSelect = SerializeObject(obj);
            }
            #endregion

            #region 组织下拉项:DepartmentSelect
            DataTable depDt = BF_DEPARTMENT.Instance.GetTable();
            if (depDt != null && depDt.Rows.Count > 0)
            {
                var obj = new List<object>();
                foreach (DataRow dr in depDt.Rows)
                {
                    obj.Add(
                        new
                        {
                            id = Convert.ToInt32(dr["DEPT_CODE"]),
                            pId = string.IsNullOrWhiteSpace(dr["P_CODE"].ToString()) ? "" : dr["P_CODE"],
                            name = dr["NAME"].ToString(),
                            value = Convert.ToInt32(dr["DEPT_CODE"])
                        });
                }
                ViewBag.DepartmentSelect = SerializeObject(obj);
            }
            #endregion

            #region 会计类型下拉项:DepartmentSelect
            DataTable accountTypeDt = SR_ACCOUNTING_TYPE.Instance.GetTable();
            if (accountTypeDt != null && accountTypeDt.Rows.Count > 0)
            {
                var obj = new List<object>();
                foreach (DataRow dr in accountTypeDt.Rows)
                {
                    obj.Add(
                        new
                        {
                            id = Convert.ToInt32(dr["ID"]),
                            pId = string.IsNullOrWhiteSpace(dr["PARENT_ID"].ToString()) ? "" : dr["PARENT_ID"],
                            name = dr["NAME"].ToString(),
                            value = Convert.ToInt32(dr["ID"])
                        });
                }
                ViewBag.AccountingTypeSelect = SerializeObject(obj);
            }
            #endregion
            #endregion

            return View(ent);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(SR_TOPIC_DETAIL.Entity entity, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.TOPIC_ID <= 0)
                {
                    result.Message = "未找到课题";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                entity.UPDATE_UID = SystemSession.UserID;
                entity.UPDATE_TIME = DateTime.Now;

                if (entity.ID == 0)
                {
                    int entityId = SR_TOPIC_DETAIL.Instance.GetNextValueFromSeqDef();
                    entity.ID = entityId;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    i = SR_TOPIC_DETAIL.Instance.Add(entity, true);
                }
                else
                {
                    i = SR_TOPIC_DETAIL.Instance.UpdateByKey(entity, entity.ID);//修改
                }

                if (i < 1)
                {
                    result.Message = "出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的信息成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "保存出错：" + ex.ToString());

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}