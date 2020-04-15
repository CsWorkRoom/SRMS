
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
    /// 论文备案
    /// </summary>
    public class SrPaperRecordController : ABaseController
    {
        public string Modular = "论文备案";

        #region 论文备案管理
        public ActionResult FlowEdit(int id = 0)
        {
            SR_PAPER_RECORD.Entity paper = new SR_PAPER_RECORD.Entity();

            #region 01.论文备案主体
            if (id > 0)
            {
                paper = SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(id);
            }
            #endregion

            #region 02.课题下拉(树状结构) 考虑：显示属于当前人员的课题
            //课题下拉树
            var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(paper.TOPIC_ID);
            ViewBag.TopicName = topic.NAME;
            #endregion

            #region 03.学科
            var sub = SR_SUBJECT.Instance.GetEntityByKey<SR_SUBJECT.Entity>(paper.SUBJECT_ID);
            ViewBag.SubjectName = sub.NAME;
            #endregion

            #region 04.文章类型
            var artype = SR_ARTICLE_TYPE.Instance.GetEntityByKey<SR_ARTICLE_TYPE.Entity>(paper.ARTICLE_TYPE_ID);
            ViewBag.ArticleName = artype.NAME;
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
            SR_PAPER_RECORD.Entity paper = new SR_PAPER_RECORD.Entity();

            #region 01.论文备案主体
            if (id > 0)
            {
                paper = SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(id);
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
        public JsonResult Edit(SR_PAPER_RECORD.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                //throw new Exception("抛出错误");
                #region 00.数据校验(暂未实现)

                #endregion

                #region 01.保存论文备案
                int entId = ent.ID;
                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                if (ent.ID == 0)
                {
                    entId = SR_PAPER_RECORD.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = entId;
                    SR_PAPER_RECORD.Instance.Add(ent);
                }
                else
                {
                    var oldEnt = SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(ent.ID);
                    ent.CREATE_TIME = oldEnt.CREATE_TIME;
                    ent.CREATE_UID = oldEnt.CREATE_UID;
                    SR_PAPER_RECORD.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion

                result.IsSuccess = true;
                result.Result = entId.ToString();
                result.FlowCaseName = ent.NAME;
                result.Message = string.Format(@"论文备案上报成功!");
            }
            catch (Exception ex)
            {
                string err = string.Format(@"论文备案上报失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "论文备案上报", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 版面费报销
        public ActionResult FeeFlowEdit(int Id = 0)
        {
            var pageRecord = SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(Id);
            SR_PAPER_RECORD_FUNDS.Entity funds = new SR_PAPER_RECORD_FUNDS.Entity();
            funds.TOTAL_FEE = 0;//设置默认报销总额为0
            funds.PAPER_RECORD_ID = Id;
            var paperRecordFund = SR_PAPER_RECORD_FUNDS.Instance.GetList<SR_PAPER_RECORD_FUNDS.Entity>("PAPER_RECORD_ID=?", Id);
            #region 01.经费总表信息
            if (paperRecordFund != null && paperRecordFund.Count > 0)
            {
                funds = paperRecordFund.FirstOrDefault();
            }
            #endregion
         
            #region 论文信息
            ViewBag.PAPER_RECORD_NAME = pageRecord.NAME;
            #endregion
            return View(funds);
        }
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FeeEdit(int Id = 0)
        {
            var pageRecord= SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(Id);
            SR_PAPER_RECORD_FUNDS.Entity funds = new SR_PAPER_RECORD_FUNDS.Entity();
            funds.TOTAL_FEE = 0;//设置默认报销总额为0
            funds.PAPER_RECORD_ID = Id;
            var paperRecordFund = SR_PAPER_RECORD_FUNDS.Instance.GetList<SR_PAPER_RECORD_FUNDS.Entity>("PAPER_RECORD_ID=?", Id);
            #region 01.经费总表信息
            if (paperRecordFund!=null && paperRecordFund.Count>0)
            {
                funds = paperRecordFund.FirstOrDefault();
            }
            #endregion

            #region 04.银行信息(新增时，自动获取默认银行信息)，银行的下拉一并返回
            var bankList = SR_BANK.Instance.GetList<SR_BANK.Entity>("CREATE_UID=? AND IS_DEFAULT=1", SystemSession.UserID);
                if (bankList != null && bankList.Count > 0)
                {
                    var bank = bankList[0];
                    funds.BANK_ADDRESS = bank.BANK_ADDRESS;
                    funds.BANK_NAME = bank.BANK_NAME;
                    funds.BANK_NO = bank.BANK_NO;
                    funds.USER_NAME = bank.USER_NAME;
                    funds.USER_PHONE = bank.USER_PHONE;
                }
            //用户的银行列表
            ViewBag.Banks = SerializeObject(bankList);
            #endregion
            #region 论文信息
            ViewBag.PAPER_RECORD_NAME = pageRecord.NAME;
            #endregion
            return View(funds);
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FeeEdit(SR_PAPER_RECORD_FUNDS.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                //throw new Exception("抛出错误");
                #region 00.数据校验(暂未实现)
                //校验经费总金额与清单经费金额之和是否相等
                #endregion

                int fundsId = ent.ID;
                #region 01.保存经费总表
                if (ent.ID == 0)
                {
                    fundsId = SR_PAPER_RECORD_FUNDS.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = fundsId;
                    SR_PAPER_RECORD_FUNDS.Instance.Add(ent);
                }
                else
                {
                    SR_PAPER_RECORD_FUNDS.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion

                #region 01-2.附件银行卡信息自动存储
                short isDefault = Convert.ToInt16(collection["IS_DEFAULT_BANK"]);
                var bankList = SR_BANK.Instance.GetList<SR_BANK.Entity>("CREATE_UID=?", SystemSession.UserID);
                if (bankList != null && bankList.Count > 0)
                {
                    var bank = bankList.FirstOrDefault(p => p.BANK_NO == ent.BANK_NO);
                    if (bank != null && bank.ID > 0)
                    {
                        bank.BANK_NAME = ent.BANK_NAME;
                        bank.BANK_ADDRESS = ent.BANK_ADDRESS;
                        bank.USER_NAME = ent.USER_NAME;
                        bank.USER_PHONE = ent.USER_PHONE;
                        bank.IS_DEFAULT = isDefault;
                        if (isDefault == 1)
                        {
                            foreach (var item in bankList)
                            {
                                item.IS_DEFAULT = 0;
                                SR_BANK.Instance.UpdateByKey(item, item.ID);
                            }
                        }
                        SR_BANK.Instance.UpdateByKey(bank, bank.ID);
                    }
                    else
                    {
                        if (isDefault == 1)
                        {
                            foreach (var item in bankList)
                            {
                                item.IS_DEFAULT = 0;
                                SR_BANK.Instance.UpdateByKey(item, item.ID);
                            }
                        }
                        SaveBank(ent, isDefault);
                    }
                }
                else
                {
                    SaveBank(ent, isDefault);
                }
                #endregion

                result.IsSuccess = true;
                result.Message = string.Format(@"填报经费报销信息成功");
                result.Result = fundsId.ToString();
                var paperRecord = SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(ent.PAPER_RECORD_ID);
                result.FlowCaseName = paperRecord.NAME;
            }
            catch (Exception ex)
            {
                string err = string.Format(@"填报经费报销信息失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "经费报销填报", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void SaveBank(SR_PAPER_RECORD_FUNDS.Entity ent, short isDefault)
        {
            SR_BANK.Entity newBank = new SR_BANK.Entity();
            newBank.ID = SR_BANK.Instance.GetNextValueFromSeqDef();
            newBank.BANK_NO = ent.BANK_NO;
            newBank.BANK_NAME = ent.BANK_NAME;
            newBank.BANK_ADDRESS = ent.BANK_ADDRESS;
            newBank.USER_NAME = ent.USER_NAME;
            newBank.USER_PHONE = ent.USER_PHONE;
            newBank.IS_DEFAULT = isDefault;
            newBank.CREATE_TIME = DateTime.Now;
            newBank.CREATE_UID = SystemSession.UserID;
            SR_BANK.Instance.Add(newBank);
        }
        #endregion

    }
}