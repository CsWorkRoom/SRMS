
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
    /// 课题中期检查任务
    /// </summary>
    public class SrTopicTaskController : ABaseController
    {
        public string Modular = "课题中期检查任务";

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            SR_TOPIC_TASK.Entity task = new SR_TOPIC_TASK.Entity();
            ViewBag.BeginEndData = "";

            #region 01.任务主体
            if (id > 0)
            {
                task = SR_TOPIC_TASK.Instance.GetEntityByKey<SR_TOPIC_TASK.Entity>(id);
                if (task.BEGIN_TIME != null)
                {
                    //时间范围
                    ViewBag.BeginEndData = task.BEGIN_TIME.Value.ToString("yyyy-MM-dd") + " - " + task.END_TIME.Value.ToString("yyyy-MM-dd");
                }
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

            return View(task);
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(SR_TOPIC_TASK.Entity ent, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                //throw new Exception("抛出错误");
                #region 00.数据校验(暂未实现)

                #endregion

                #region 01.保存任务
                #region 时间范围赋值
                var beginEndData = collection["beginEndData"];//时间范围
                if (!string.IsNullOrWhiteSpace(beginEndData))//日期区间
                {
                    string[] dataArr = Regex.Split(beginEndData.Trim(), " - ", RegexOptions.IgnoreCase);
                    if (string.IsNullOrWhiteSpace(dataArr[0]) == false)//开始日期
                        ent.BEGIN_TIME = Convert.ToDateTime(dataArr[0]);
                    if (string.IsNullOrWhiteSpace(dataArr[1]) == false)//结束日期
                        ent.END_TIME = Convert.ToDateTime(dataArr[1]);
                }
                #endregion

                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                if (ent.ID == 0)
                {
                    var entId = SR_TOPIC_TASK.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = entId;
                    SR_TOPIC_TASK.Instance.Add(ent);
                }
                else
                {
                    SR_TOPIC_TASK.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion


                result.IsSuccess = true;
                result.Message = string.Format(@"中期检查任务下达成功!");
            }
            catch (Exception ex)
            {
                string err = string.Format(@"中期检查任务下达失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "中期检查任务下达", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}