
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

        #region 任务下达
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

            #region 02.课题下拉(树状结构)//修改为只加载没
            var list = SR_TOPIC.Instance.GetTopicNoEndTree();
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
        #endregion

        #region 任务执行
        public ActionResult FlowEdit(int Id = 0)
        {
            var done = SR_TOPIC_TASK_DONE.Instance.GetEntityByKey<SR_TOPIC_TASK_DONE.Entity>(Id);

            ViewBag.Task = null;

            #region 01.任务主体
            ViewBag.BeginEndData = "";
            if (Id > 0)
            {
                SR_TOPIC_TASK.Entity task = SR_TOPIC_TASK.Instance.GetEntityByKey<SR_TOPIC_TASK.Entity>(done.TOPIC_TASK_ID);
                if (task.BEGIN_TIME != null)
                {
                    //时间范围
                    ViewBag.BeginEndData = task.BEGIN_TIME.Value.ToString("yyyy-MM-dd") + " - " + task.END_TIME.Value.ToString("yyyy-MM-dd");
                }
                done.TOPIC_TASK_ID =done.TOPIC_TASK_ID;
                done.TOPIC_ID = task.TOPIC_ID;
                ViewBag.Task = task;
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

            return View(done);
        }
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="Id">课题中期任务编号</param>
        /// <returns></returns>
        public ActionResult Done(int Id = 0)
        {
            #region 00.校验任务是否已处理
            string msg = "";
            //if (IsDoneTask(Id, out msg))
            //{
            //    //ShowAlert(msg);
            //    return Content("<script>alert('" + msg + "');window.location.href='about:blank';window.close();</script>");
            //}
            #endregion

            SR_TOPIC_TASK_DONE.Entity done = new SR_TOPIC_TASK_DONE.Entity();
            var tdone = SR_TOPIC_TASK_DONE.Instance.GetList<SR_TOPIC_TASK_DONE.Entity>("TOPIC_TASK_ID=?", Id).FirstOrDefault();
            if (tdone != null)
            {
                done = tdone;
            }

            ViewBag.Task = null;

            #region 01.任务主体
            ViewBag.BeginEndData = "";
            if (Id>0)
            {
                SR_TOPIC_TASK.Entity task = SR_TOPIC_TASK.Instance.GetEntityByKey<SR_TOPIC_TASK.Entity>(Id);
                if (task.BEGIN_TIME != null)
                {
                    //时间范围
                    ViewBag.BeginEndData = task.BEGIN_TIME.Value.ToString("yyyy-MM-dd") + " - " + task.END_TIME.Value.ToString("yyyy-MM-dd");
                }
                done.TOPIC_TASK_ID = Id;
                done.TOPIC_ID = task.TOPIC_ID;
                ViewBag.Task = task;
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

            return View(done);
        }

        /// <summary>
        /// 编辑提交(暂时未考虑审核)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Done(SR_TOPIC_TASK_DONE.Entity ent)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                #region 00.校验任务是否已处理
                string msg = "";
                //if (IsDoneTask(ent.TOPIC_TASK_ID, out msg))
                //{
                //    result.IsSuccess = false;
                //    throw new Exception(msg);
                //}
                #endregion

                #region 01.保存任务执行
                ent.UPDATE_TIME = DateTime.Now;
                ent.UPDATE_UID = SystemSession.UserID;
                int entId = ent.ID;
                if (ent.ID == 0)
                {
                    entId = SR_TOPIC_TASK_DONE.Instance.GetNextValueFromSeqDef();
                    ent.CREATE_TIME = DateTime.Now;
                    ent.CREATE_UID = SystemSession.UserID;
                    ent.ID = entId;
                    SR_TOPIC_TASK_DONE.Instance.Add(ent);
                }
                else
                {
                    SR_TOPIC_TASK_DONE.Instance.UpdateByKey(ent, ent.ID);
                }
                #endregion


                result.IsSuccess = true;
                result.Result = entId.ToString();
                result.Message = string.Format(@"中期检查任务处理成功!");
            }
            catch (Exception ex)
            {
                string err = string.Format(@"中期检查任务处理失败：{0}", ex.Message);
                result.IsSuccess = false;
                result.Message = err;
                BLog.Write(BLog.LogLevel.ERROR, err);
                WriteOperationLog(BLog.LogLevel.ERROR, false, Modular, "中期检查任务处理", "", err);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private bool IsDoneTask(int topicTaskId,out string msg)
        {
            bool resBool = false;
            msg = "";
            var doneList = SR_TOPIC_TASK_DONE.Instance.GetList<SR_TOPIC_TASK_DONE.Entity>("TOPIC_TASK_ID=? AND CREATE_UID=?", topicTaskId, SystemSession.UserID);
            if(doneList!=null&&doneList.Count>0)
            {
                msg = "您当前所使用的工号已经提交处理了本任务，请勿重复处理。";
                resBool = true;
            }
            return resBool;
        }
        #endregion
    }
}