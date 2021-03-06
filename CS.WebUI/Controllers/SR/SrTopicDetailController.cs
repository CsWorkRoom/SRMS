﻿using CS.Base.Log;
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
        public ActionResult Edit(int topicId = 0)
        {
            if (topicId <= 0)
            {
                return ShowAlert(string.Format("课题编号[{0}]应大于0", topicId));
            }
            #region 验证课题是否立项
            var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(topicId);
            if (topic == null)
            {
                return ShowAlert(string.Format("未找到编号为[{0}]的课题", topicId));
            }
            else if (topic.IS_APPROVAL == 0)
            {
                return ShowAlert(string.Format("编号为[{0}]的课题[{1}]未立项，不能进行课题完善操作", topicId, topic.NAME));
            }
            #endregion

            ViewBag.Companys = "[]";//参与单位值初始化
            var ent = SR_TOPIC_DETAIL.Instance.GetEntity<SR_TOPIC_DETAIL.Entity>("TOPIC_ID=?", topicId);
            if (ent == null || ent.ID < 1)
            {
                ent = new SR_TOPIC_DETAIL.Entity();
            }
            else
            {
                //课题完善信息和合作单位集合
                var companyList = SR_TOPIC_DETAIL_COMPANY.Instance.GetList<SR_TOPIC_DETAIL_COMPANY.Entity>("TOPIC_DETAIL_ID=?", ent.ID);
                if (companyList != null && companyList.Count > 0)
                {
                    ViewBag.Companys = SerializeObject(companyList);
                }
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
                    result.IsSuccess = false;
                    result.Message = "未找到课题";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                #region 验证课题是否立项
                var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(entity.TOPIC_ID);
                if (topic == null)
                {
                    result.IsSuccess = false;
                    result.Message= string.Format("未找到编号为[{0}]的课题", entity.TOPIC_ID);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (topic.IS_APPROVAL == 0)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("编号为[{0}]的课题[{1}]未立项，不能进行课题完善操作", entity.TOPIC_ID, topic.NAME);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                #endregion

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

                #region 保存合作单位信息
                string companys = collection["Companys"];
                int delCount = 0, addCount = 0,updateCount = 0;
                SR_TOPIC_DETAIL_COMPANY.Instance.SaveCompanys(entity.ID, companys, out addCount, out updateCount, out delCount);
                #endregion

                #region 修改课题的学科归属
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("SUBJECT_ID",entity.SUBJECT_ID);
                SR_TOPIC.Instance.UpdateByKey(dic, entity.TOPIC_ID);
                #endregion

                result.IsSuccess = true;
                result.Message = string.Format("保存成功！\r\n合作单位：新增【{0}】,修改【{1}】,删除【{2}】", addCount, updateCount, delCount);
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