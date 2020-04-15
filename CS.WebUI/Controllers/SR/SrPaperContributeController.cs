
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
    /// 论文投稿
    /// </summary>
    public class SrPaperContributeController : ABaseController
    {
        public string Modular = "论文投稿";

        #region 论文投稿管理
        public ActionResult FlowEdit(int id = 0)
        {
            SR_PAPER_CONTRIBUTE.Entity paper = new SR_PAPER_CONTRIBUTE.Entity();

            #region 01.论文投稿主体
            if (id > 0)
            {
                paper = SR_PAPER_CONTRIBUTE.Instance.GetEntityByKey<SR_PAPER_CONTRIBUTE.Entity>(id);
            }
            #endregion

            #region 02.课题下拉(树状结构) 考虑：显示属于当前人员的课题
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

            #region 03.学科
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

            #region 04.文章类型
            DataTable dtArticleType = SR_ARTICLE_TYPE.Instance.GetTable();
            if (dtArticleType != null && dtArticleType.Rows.Count > 0)
            {
                var obj = new List<object>();
                foreach (DataRow dr in dtArticleType.Rows)
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
                ViewBag.ArticleTypeSelect = SerializeObject(obj);
            }
            #endregion

            return View(paper);
        }
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            SR_PAPER_CONTRIBUTE.Entity paper = new SR_PAPER_CONTRIBUTE.Entity();

            #region 01.论文投稿主体
            if (id > 0)
            {
                paper = SR_PAPER_CONTRIBUTE.Instance.GetEntityByKey<SR_PAPER_CONTRIBUTE.Entity>(id);
            }
            #endregion

            #region 02.课题下拉(树状结构) 考虑：显示属于当前人员的课题
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

            #region 03.学科
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

            #region 04.文章类型
            DataTable dtArticleType = SR_ARTICLE_TYPE.Instance.GetTable();
            if (dtArticleType != null && dtArticleType.Rows.Count > 0)
            {
                var obj = new List<object>();
                foreach (DataRow dr in dtArticleType.Rows)
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
                ViewBag.ArticleTypeSelect = SerializeObject(obj);
            }
            #endregion

            return View(paper);
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(SR_PAPER_CONTRIBUTE.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                //throw new Exception("抛出错误");
                #region 00.数据校验(暂未实现)

                #endregion

                #region 01.保存论文投稿
                int entId = ent.ID;
                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                if (ent.ID == 0)
                {
                    entId = SR_PAPER_CONTRIBUTE.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = entId;
                    SR_PAPER_CONTRIBUTE.Instance.Add(ent);
                }
                else
                {
                    var oldEnt = SR_PAPER_CONTRIBUTE.Instance.GetEntityByKey<SR_PAPER_CONTRIBUTE.Entity>(ent.ID);
                    ent.CREATE_TIME = oldEnt.CREATE_TIME;
                    ent.CREATE_UID = oldEnt.CREATE_UID;
                    SR_PAPER_CONTRIBUTE.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion

                result.IsSuccess = true;
                result.Result = entId.ToString();
                result.FlowCaseName = ent.NAME;
                result.Message = string.Format(@"论文投稿上报成功!");
            }
            catch (Exception ex)
            {
                string err = string.Format(@"论文投稿上报失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "论文投稿上报", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}