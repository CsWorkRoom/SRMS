
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
using System.Text.RegularExpressions;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 学科学习资料
    /// </summary>
    public class SrSubjectArticleController : ABaseController
    {
        public string Modular = "学科学习资料";

        #region 学科学习资料管理
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            SR_SUBJECT_ARTICLE.Entity paper = new SR_SUBJECT_ARTICLE.Entity();

            #region 01.论文备案主体
            if (id > 0)
            {
                paper = SR_SUBJECT_ARTICLE.Instance.GetEntityByKey<SR_SUBJECT_ARTICLE.Entity>(id);
            }
            #endregion

            #region 01.学科
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

            return View(paper);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(SR_SUBJECT_ARTICLE.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                #region 00.数据校验(暂未实现)

                #endregion

                #region 01.保存学科学习资料
                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                if (ent.ID == 0)
                {
                    var entId = SR_SUBJECT_ARTICLE.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = entId;
                    SR_SUBJECT_ARTICLE.Instance.Add(ent);
                }
                else
                {
                    var oldEnt = SR_SUBJECT_ARTICLE.Instance.GetEntityByKey<SR_SUBJECT_ARTICLE.Entity>(ent.ID);
                    ent.CREATE_TIME = oldEnt.CREATE_TIME;
                    ent.CREATE_UID = oldEnt.CREATE_UID;
                    SR_SUBJECT_ARTICLE.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion

                result.IsSuccess = true;
                result.Message = string.Format(@"学科学习资料保存成功!");
            }
            catch (Exception ex)
            {
                string err = string.Format(@"学科学习资料保存失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "学科学习资料保存", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 学科学习资料展示
        public ActionResult Show(int id = 0)
        {
            var paper = SR_SUBJECT_ARTICLE.Instance.GetEntityByKey<SR_SUBJECT_ARTICLE.Entity>(id);
            var subject = SR_SUBJECT.Instance.GetEntityByKey<SR_SUBJECT.Entity>(paper.SUBJECT_ID);
            ViewBag.SubjectName = subject != null ? subject.NAME : "无";
            var creater = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(paper.CREATE_UID);
            ViewBag.CreateUser = creater != null ? creater.FULL_NAME : "未知";
            return View(paper);
        }
        #endregion
    }
}