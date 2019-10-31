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
    /// 课题预算信息
    /// </summary>
    public class SrTopicBudgetController : ABaseController
    {
        public string Modular = "课题预算信息";
        public ActionResult FlowEdit(int Id = 0)
        {
            var main = SR_TOPIC_BUDGET_MAIN.Instance.GetEntityByKey<SR_TOPIC_BUDGET_MAIN.Entity>(Id);
            #region 01.课题基础信息（暂未实现）
            var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(main.TOPIC_ID);
            IEnumerable<BLL.SR.SR_TOPIC.Entity> topics = new List<BLL.SR.SR_TOPIC.Entity>() { topic };
            ViewBag.TOPIC_NAME = topic.NAME;
            BLL.SR.SR_TOPIC.Entity model = topic;
            BLL.SR.SR_TOPIC_TYPE.Entity topicType = BLL.SR.SR_TOPIC_TYPE.Instance.GetEntityByKey<BLL.SR.SR_TOPIC_TYPE.Entity>(model.TOPIC_TYPE_ID);
            ViewBag.TopicTypeName = topicType.NAME;
            ViewBag.TOPIC = topics.ToList();
            //参加的人员
            List<BLL.SR.SR_TOPIC_USER.Entity> topicUsers = BLL.SR.SR_TOPIC_USER.Instance.GetTopicUserList(topic.ID);
            var libaleList = topicUsers.Where(x => x.IS_PERSON_LIABLE == 1).Select(x => new TopicUser()
            {
                USER_NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME")
            });
            var libaleNoList = topicUsers.Where(x => x.IS_PERSON_LIABLE == 0).Select(x => new TopicUser()
            {
                USER_NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME")
            });
            ViewBag.SelectLibaleUsers = libaleList.ToList();
            ViewBag.SelectNoLibaleUsers = libaleNoList.ToList();
            #endregion

            #region 02.预算列表信息
            var budgetList = SR_TOPIC_BUDGET.Instance.GetList<SR_TOPIC_BUDGET.Entity>("TOPIC_ID=?", main.TOPIC_ID);
            ViewBag.Budgets = SerializeObject(budgetList);//预算列表
            #endregion

            #region 03.预算类型(非html标记)
            ViewBag.BudgetTypeArr = GetBudgetTypeArr();
            #endregion

            ViewBag.TOPIC_ID = main.TOPIC_ID;//课题编号


            return View(new CS.BLL.SR.SR_TOPIC_BUDGET.Entity());
        }
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="topicId">课题编号</param>
        /// <returns></returns>
        public ActionResult Edit(int Id=0)
        {
            #region 01.课题基础信息（暂未实现）
            var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(Id);
            IEnumerable<BLL.SR.SR_TOPIC.Entity> topics =new List<BLL.SR.SR_TOPIC.Entity>(){ topic };
            ViewBag.TOPIC_NAME = topic.NAME;
            BLL.SR.SR_TOPIC.Entity model = topic;
            BLL.SR.SR_TOPIC_TYPE.Entity topicType = BLL.SR.SR_TOPIC_TYPE.Instance.GetEntityByKey<BLL.SR.SR_TOPIC_TYPE.Entity>(model.TOPIC_TYPE_ID);
            ViewBag.TopicTypeName = topicType.NAME;
            ViewBag.TOPIC = topics.ToList();
            //参加的人员
            List<BLL.SR.SR_TOPIC_USER.Entity> topicUsers = BLL.SR.SR_TOPIC_USER.Instance.GetTopicUserList(Id);
            var libaleList = topicUsers.Where(x => x.IS_PERSON_LIABLE == 1).Select(x => new TopicUser()
            {
                USER_NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME")
            });
            var libaleNoList = topicUsers.Where(x => x.IS_PERSON_LIABLE == 0).Select(x => new TopicUser()
            {
                USER_NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME")
            });
            ViewBag.SelectLibaleUsers = libaleList.ToList();
            ViewBag.SelectNoLibaleUsers = libaleNoList.ToList();
            #endregion

            #region 02.预算列表信息
            var budgetList = SR_TOPIC_BUDGET.Instance.GetList<SR_TOPIC_BUDGET.Entity>("TOPIC_ID=?", Id);
            ViewBag.Budgets = SerializeObject(budgetList);//预算列表
            #endregion

            #region 03.预算类型(非html标记)
            ViewBag.BudgetTypeArr = GetBudgetTypeArr();
            #endregion

            ViewBag.TOPIC_ID = Id;//课题编号
          

            return View(new CS.BLL.SR.SR_TOPIC_BUDGET.Entity());
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                int addCount = 0, updateCount = 0, delCount = 0;
                double totalFee = 0;//总预算金额
                #region 01.遍历保存预算明细
                var budgets = collection["Budgets"];
                int topicId = Convert.ToInt32(collection["TOPIC_ID"].Trim());
                if (!string.IsNullOrWhiteSpace(budgets) && budgets.Length > 0)
                {
                    var budgetList = DeserializeObject<List<SR_TOPIC_BUDGET.Entity>>(budgets);
                    SR_TOPIC_BUDGET.Instance.SaveBudget(topicId, budgets, out addCount, out updateCount, out delCount);
                    totalFee = budgetList.Sum(p => p.FEE);
                }
                #endregion
                #region 保存预算流程信息表
                int entId=SR_TOPIC_BUDGET_MAIN.Instance.SaveBudgetMain(topicId);
                #endregion
                #region 02.修改课题表的预算总金额
                var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(topicId);
                if (topic != null && topic.ID > 0)
                {
                    topic.TOTAL_FEE = totalFee;
                    SR_TOPIC.Instance.UpdateByKey(topic, topicId);
                }
                result.IsSuccess = true;
                result.Result = entId.ToString();
                result.Message = string.Format(@"保存预算清单成功：新增【{0}】,修改【{1}】,删除【{2}】", addCount, updateCount, delCount);
                #endregion
            }
            catch(Exception ex)
            {
                string err = string.Format(@"保存预算清单失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "保存预算清单","", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得预算类型(暂不支持树状结构)
        /// </summary>
        /// <returns></returns>
        private string GetBudgetTypeArr()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            var typeList = SR_BUDGET_TYPE.Instance.GetList<SR_BUDGET_TYPE.Entity>();
            if (typeList != null && typeList.Count > 0)
            {
                foreach (var type in typeList)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Key", type.ID);
                    dic.Add("Value", type.NAME);
                    list.Add(dic);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("  budgetTypeArr =");
            sb.AppendLine(SerializeObject(list) + ";");
            sb.AppendLine("</script>");
            return sb.ToString();
        }

    }
}