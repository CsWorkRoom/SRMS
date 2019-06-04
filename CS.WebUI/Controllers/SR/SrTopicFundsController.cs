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

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 课题经费报销
    /// </summary>
    public class SrTopicFundsController : ABaseController
    {
        public string Modular = "课题经费报销";

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id=0)
        {
            SR_TOPIC_FUNDS.Entity funds = new SR_TOPIC_FUNDS.Entity();
            funds.TOTAL_FEE = 0;//设置默认报销总额为0

            #region 01.经费总表信息
            if (id > 0)
            {
                funds = SR_TOPIC_FUNDS.Instance.GetEntityByKey<SR_TOPIC_FUNDS.Entity>(id);
            }
            #endregion

            #region 02.经费明细清单
            ViewBag.FundsDetails = "[]";
            if (funds!=null&&funds.ID>0)
            {
                var fundsDetailList= SR_TOPIC_FUNDS_DETAIL.Instance.GetList<SR_TOPIC_FUNDS_DETAIL.Entity>("TOPIC_FUNDS_ID=?", id);
                if(fundsDetailList!=null&&fundsDetailList.Count>0)
                {
                    ViewBag.FundsDetails = SerializeObject(fundsDetailList);
                }
            }
            #endregion

            #region 03.课题下拉(树状结构)
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

            #region 04.银行信息(新增时，自动获取默认银行信息)，银行的下拉一并返回
            var bankList = SR_BANK.Instance.GetList<SR_BANK.Entity>("CREATE_UID=? AND IS_DEFAULT=1", SystemSession.UserID);
            if (id == 0)
            {
                if(bankList!=null&&bankList.Count>0)
                {
                    var bank = bankList[0];
                    funds.BANK_ADDRESS = bank.BANK_ADDRESS;
                    funds.BANK_NAME = bank.BANK_NAME;
                    funds.BANK_NO = bank.BANK_NO;
                    funds.USER_NAME = bank.USER_NAME;
                    funds.USER_PHONE = bank.USER_PHONE;
                }
            }
            //用户的银行列表
            ViewBag.Banks = SerializeObject(bankList);
            #endregion

            return View(funds);
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(SR_TOPIC_FUNDS.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                throw new Exception("抛出错误");
                #region 00.数据校验(暂未实现)
                //校验经费总金额与清单经费金额之和是否相等
                #endregion

                int fundsId = ent.ID;
                #region 01.保存经费总表
                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                if (ent.ID == 0)
                {
                    fundsId = SR_TOPIC_FUNDS.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = fundsId;
                    SR_TOPIC_FUNDS.Instance.Add(ent);
                }
                else
                {
                    SR_TOPIC_FUNDS.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion

                #region 01-2.附件银行卡信息自动存储(暂未实现)
                #endregion

                int addCount = 0, updateCount = 0, delCount = 0;
                #region 02.保存经费清单列表
                var fundsDetails = collection["FundsDetails"];
                if (!string.IsNullOrWhiteSpace(fundsDetails) && fundsDetails.Length > 0)
                {
                    SR_TOPIC_FUNDS_DETAIL.Instance.SaveFundsDetail(fundsId, ent.TOPIC_ID, fundsDetails, out addCount, out updateCount, out delCount);
                }
                #endregion

                result.IsSuccess = true;
                result.Message =string.Format(@"填报经费报销信息成功：新增【{0}】,修改【{1}】,删除【{2}】",addCount,updateCount,delCount);
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

    }
}