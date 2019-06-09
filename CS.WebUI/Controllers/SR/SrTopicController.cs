using CS.Base.Log;
using CS.BLL.FW;
using CS.WebUI.Controllers.FW;
using CS.WebUI.Models.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CS.BLL.SR;
using Newtonsoft.Json;
using CS.WebUI.Models.SR;

namespace CS.WebUI.Controllers.SR
{
    public class SrTopicController : ABaseController
    {
        public string Modular = "课题管理";
        // GET: SrTopic
        public ActionResult Index()
        {
            return View();
        }

        #region 课题管理
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 5)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_TOPIC.Entity model = new BLL.SR.SR_TOPIC.Entity();
            if (id > 0)
            {
                model = BLL.SR.SR_TOPIC.Instance.GetEntityByKey<BLL.SR.SR_TOPIC.Entity>(id);
                if (model != null)
                {
                }
                else
                {
                    ViewBag.Message = "课题不存在！";
                    model.ID = -1;
                }
            }
            #region 加载课题参与人员
            var userList = BLL.SR.SR_TOPIC_USER.Instance.GetList<BLL.SR.SR_TOPIC_USER.Entity>("TOPIC_ID=?", id);
            ViewBag.SelectUser = userList;
            #endregion

            #region 加载下拉列表
            DataTable dt = BLL.SR.SR_TOPIC_TYPE.Instance.GetTable();
            var obj = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = dr["ID"],
                        pId = dr["PARENT_ID"],
                        name = dr["NAME"].ToString(),
                        value = Convert.ToInt32(dr["ID"])
                    });
            }

            ViewBag.TypeSelect = SerializeObject(obj);
            #endregion

            #region 加载附件列表
            string fileIds = "";
            var fileList = BLL.FW.BF_BULLETIN_ATTACH.Instance.GetList<BLL.FW.BF_BULLETIN_ATTACH.Entity>("BULL_ID=?", id);
            List<BulletinFileModel> list = new List<BulletinFileModel>();
            foreach (var item in fileList)
            {
                fileIds += item.FILE_PATH + ",";
                BulletinFileModel fileModel = new Models.FW.BulletinFileModel();
                fileModel.ID = item.ID;
                fileModel.BULL_ID = item.BULL_ID;
                fileModel.FILE_PATH = item.FILE_PATH;
                fileModel.FILE_NAME = Path.GetFileName(item.FILE_PATH);
                list.Add(fileModel);

            }

            ViewBag.FileId = fileIds;
            ViewBag.FileList = list;
            #endregion
            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(SR_TOPIC.Entity entity, FormCollection collection)
        {
            int topicId = entity.ID;
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "课题项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //获取选择的课题参与人员
                string selectUserJson = collection["SelectUser"];
                var userList = DeserializeObject<List<SR_TOPIC_USER.Entity>>(selectUserJson);
                if (entity.ID == 0)
                {
                    topicId = SR_TOPIC.Instance.GetNextValueFromSeqDef();
                    entity.ID = topicId;
                    entity.CREATE_USER_ID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    SR_TOPIC.Instance.Add(entity, true);
                }
                else
                {
                    i = SR_TOPIC.Instance.UpdateByKey(entity, entity.ID);//修改
                }
                //保存参与人员信息
                int addCount = 0;
                int updateCount = 0;
                int deleteCount = 0;
                SR_TOPIC_USER.Instance.SaveUserListJoins(topicId, userList, out addCount, out updateCount, out deleteCount);
                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的课题成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据关键字获取用户列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>

        public string SearchUser(string keyword)
        {
            string where = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where = string.Format(" FULL_NAME LIKE '%{0}%'", keyword);
            }

            IList<BLL.FW.BF_USER.Entity> ueList = BLL.FW.BF_USER.Instance.GetListPage(10, 1, null, where);
            return SerializeObject(ueList);
        }
        /// <summary>
        /// 获取当前课题选择的参与用户
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public string GetTopicUsers(int topicId)
        {
            IList<BLL.SR.SR_TOPIC_USER.Entity> ueList = BLL.SR.SR_TOPIC_USER.Instance.GetTopicUserList(topicId);
            var libaleList = ueList.Select(x => new TopicUser()
            {
                ID = x.USER_ID,
                NAME= BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME"),
                IS_PERSON_LIABLE = x.IS_PERSON_LIABLE
            });
            return SerializeObject(libaleList).ToLower();
        }
        #endregion

        #region 设置规则和选择评审老师

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Set(int topicid = 5)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_TOPIC.Entity model =
            BLL.SR.SR_TOPIC.Instance.GetEntityByKey<BLL.SR.SR_TOPIC.Entity>(topicid);
            BLL.SR.SR_TOPIC_TYPE.Entity topicType = BLL.SR.SR_TOPIC_TYPE.Instance.GetEntityByKey<BLL.SR.SR_TOPIC_TYPE.Entity>(model.TOPIC_TYPE_ID);
            ViewBag.TopicTypeName = topicType.NAME;
            //参加的人员
            List<BLL.SR.SR_TOPIC_USER.Entity> topicUsers = BLL.SR.SR_TOPIC_USER.Instance.GetTopicUserList(topicid);
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
            ViewBag.AllSubject = JsonConvert.SerializeObject(GetSubjects());
            #region 加载课题评审老师
            var expertList = BLL.SR.SR_TOPIC_EXPERT.Instance.GetList<BLL.SR.SR_TOPIC_EXPERT.Entity>("TOPIC_ID=?", topicid);
            ViewBag.SelectExpert = expertList;
            #endregion


            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// 编辑设置评选规则和老师
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Set(SR_TOPIC.Entity entity, FormCollection collection)
        {
            int topicId = entity.ID;
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "课题项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //获取选择的课参与评分专家
                string selectUserJson = collection["SelectExpert"];
                var userList = DeserializeObject<List<SR_TOPIC_EXPERT.Entity>>(selectUserJson);

                //获取选择的课题评分规则
                string selectSubItemJson = collection["SubItems"];
                var subItemList = DeserializeObject<List<SR_TOPIC_SUB_ITEM.Entity>>(selectSubItemJson);

                //保存参与人员信息
                int addCount = 0;
                int updateCount = 0;
                int deleteCount = 0;
                SR_TOPIC_EXPERT.Instance.SaveExpertListJoins(topicId, userList, out addCount, out updateCount, out deleteCount);
                //保存所选评分规则
                int addCountSub = 0;
                int updateCountSub = 0;
                int deleteCountSub = 0;
                SR_TOPIC_SUB_ITEM.Instance.SaveSubItemListJoins(topicId, subItemList, out addCountSub, out updateCountSub, out deleteCountSub);

                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的课题成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前课题选择的评审人员
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public string GetTopicExperts(int topicId)
        {
            IList<BLL.SR.SR_TOPIC_EXPERT.Entity> ueList = BLL.SR.SR_TOPIC_EXPERT.Instance.GetTopicExpertList(topicId);
            var libaleList = ueList.Select(x => new TopicUser()
            {
                ID = x.USER_ID,
                NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME")
            });
            return SerializeObject(libaleList).ToLower();
        }

        /// <summary>
        /// 获取当前课题选择的评分规则
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public string GetTopicSubItems(int topicId)
        {
            IList<BLL.SR.SR_TOPIC_SUB_ITEM.Entity> ueList = BLL.SR.SR_TOPIC_SUB_ITEM.Instance.GetTopicSubItemList(topicId);
            var libaleList = ueList.Select(x => new TopicSubItem()
            {
                ID = x.ID,
                SUBJECT_ID = BLL.SR.SR_SUB_ITEM.Instance.GetStringValueByKey(x.SUB_ITEM_ID, "SUBJECT_ID"),
                SUBJECT_NAME = getSubjuct(x.SUB_ITEM_ID),
                SUB_ITEM_ID = x.SUB_ITEM_ID,
                WEIGHT =x.WEIGHT,
                REMARK = x.REMARK
            });
            return SerializeObject(libaleList).ToLower();
        }
        /// <summary>
        /// 获取所有学科上级接关系
        /// </summary>
        /// <returns></returns>
        public List<SubjectDto> GetSubjects()
        {
            SubjectDto rootRoot = new SubjectDto();
            rootRoot.id = 0;
            rootRoot.name = "系统";
            rootRoot.children = new List<SubjectDto>();
            var fs = BLL.SR.SR_SUBJECT.Instance.GetSubjectList();
            if (fs != null)
            {
                LoopToAppendChildren(fs.ToList(), rootRoot);
            }
            return rootRoot.children;

        }
        /// <summary>
        /// 内部递归循环
        /// </summary>
        /// <param name="all"></param>
        /// <param name="curItem"></param>
        private void LoopToAppendChildren(List<SR_SUBJECT.Entity> all, SubjectDto curItem)
        {
            List<SubjectDto> fsDtos = new List<SubjectDto>();
            var subItems = all.Where(ee => ee.PARENT_ID == curItem.id).ToList();
            if (subItems != null && subItems.Count > 0)
            {
                foreach (SR_SUBJECT.Entity f in subItems)
                {
                    SubjectDto fDto = new SubjectDto();
                    fDto.id = f.ID;
                    fDto.name = f.NAME;
                    fsDtos.Add(fDto);
                }
                curItem.children = new List<SubjectDto>();
                curItem.children.AddRange(fsDtos);
                foreach (var subItem in fsDtos)
                {
                    LoopToAppendChildren(all, subItem);
                }
            }
        }

        /// <summary>
        /// 获取单个学科下的评分规则
        /// </summary>
        /// <param name="subid"></param>
        /// <returns></returns>
        public string GetSubItems(string subid)
        {
            if (!string.IsNullOrWhiteSpace(subid))
            {
                IList<BLL.SR.SR_SUB_ITEM.Entity> ueList = BLL.SR.SR_SUB_ITEM.Instance.GetSubItemList(subid);
                return SerializeObject(ueList);
            }
            else

                return null;

        }

        #region 获取部门用户组织树
        /// <summary>
        /// 角色管理树形单独处理
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBraceUserAndDepTree()
        {
            List<TreeModel> rtlist = new List<TreeModel>();
            List<BLL.FW.BF_DEPARTMENT.Entity> fmlist = BLL.FW.BF_DEPARTMENT.Instance.GetList<BLL.FW.BF_DEPARTMENT.Entity>().ToList();
            foreach (var rootDept in fmlist)
            {
                TreeModel rmodel = new TreeModel();
                rmodel.name = rootDept.NAME;
                rmodel.id = rootDept.DEPT_CODE.ToString();
                rmodel.pId = rootDept.P_CODE.ToString();
                rmodel.icon = "../../Content/Images/organ1.png";
                rtlist.Add(rmodel);
            }

            List<BLL.FW.BF_USER.Entity> ulist = BLL.FW.BF_USER.Instance.GetList<BLL.FW.BF_USER.Entity>().ToList();
            foreach (var item in ulist)
            {
                TreeModel umodel = new TreeModel();
                umodel.id = "user_" + item.ID.ToString();
                umodel.name = item.FULL_NAME;
                umodel.pId = BLL.FW.BF_DEPARTMENT.Instance.GetStringValueByKey(item.DEPT_ID, "DEPT_CODE");
                umodel.levels = "20";
                umodel.icon = "../../Content/Images/person-1.png";
                rtlist.Add(umodel);
            }
            return Json(rtlist);
        }

        /// <summary>
        /// 递归获取下属部门
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="plist"></param>
        /// <returns></returns>
        public List<TreeModel> GetChild(string pid, List<BLL.FW.BF_DEPARTMENT.Entity> plist)
        {
            List<TreeModel> tlist = new List<TreeModel>();
            List<BLL.FW.BF_DEPARTMENT.Entity> flist = new List<BLL.FW.BF_DEPARTMENT.Entity>();
            flist = plist.Where(t => t.DEPT_CODE == Convert.ToInt32(pid)).ToList();
            foreach (var item in flist)
            {
                TreeModel model = new TreeModel();
                model.name = item.NAME;
                model.id = item.DEPT_CODE.ToString();
                model.pId = item.P_CODE.ToString();
                model.levels = item.DEPT_LEVEL.ToString();
                model.icon = "../../Content/Images/organ2.png";
                tlist.Add(model);
                tlist.AddRange(GetChild(item.ID.ToString(), plist));
            }
            return tlist;
        }
        #endregion
        #endregion

        #region 专家单项评分
        public ActionResult ExpertScore(int topicid = 5)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_TOPIC.Entity model =
                BLL.SR.SR_TOPIC.Instance.GetEntityByKey<BLL.SR.SR_TOPIC.Entity>(topicid);
            BLL.SR.SR_TOPIC_TYPE.Entity topicType = BLL.SR.SR_TOPIC_TYPE.Instance.GetEntityByKey<BLL.SR.SR_TOPIC_TYPE.Entity>(model.TOPIC_TYPE_ID);
            ViewBag.TopicTypeName = topicType.NAME;
            //参加的人员
            List<BLL.SR.SR_TOPIC_USER.Entity> topicUsers = BLL.SR.SR_TOPIC_USER.Instance.GetTopicUserList(topicid);
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
            #region 加载课题评审规则
            var subItemList = BLL.SR.SR_TOPIC_SUB_ITEM.Instance.GetList<BLL.SR.SR_TOPIC_SUB_ITEM.Entity>("TOPIC_ID=?", topicid);
            ViewBag.Subitems = SerializeObject(subItemList.ToList());
            #endregion


            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// 专家评分
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ExpertScore(SR_TOPIC.Entity entity, FormCollection collection)
        {
            int topicId = entity.ID;
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "课题项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                //获取选择的课题评分规则
                string selectSubItemJson = collection["ScoreItems"];
                var subItemList = DeserializeObject<List<SR_TOPIC_SCORE.Entity>>(selectSubItemJson);
                double actScore = 0;
                foreach (var item in subItemList)
                {
                    item.ACT_SCORE = item.SCORE * item.WEIGHT / 100;
                    actScore += item.ACT_SCORE;
                }
                //保存所选评分规则
                int addCountSub = 0;
                int updateCountSub = 0;
                int deleteCountSub = 0;
                SR_TOPIC_SCORE.Instance.SaveScoreListJoins(topicId, SystemSession.UserID, subItemList, out addCountSub, out updateCountSub, out deleteCountSub);

                var topicExpert = SR_TOPIC_EXPERT.Instance.GetTopicByTopicAndUser(topicId, SystemSession.UserID);
                topicExpert.IS_SCORE = 1;
                topicExpert.OPER_TIME = DateTime.Now;
                topicExpert.SCORE = actScore;
                SR_TOPIC_EXPERT.Instance.UpdateByKey(topicExpert, topicExpert.ID);//修改
                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的课题成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取当前课题设置的评分规则
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public string GetSubItemsByUserAndTopic(int topicId)
        {
            if (topicId != 0)
            {
                IList<BLL.SR.SR_TOPIC_SUB_ITEM.Entity> ueList = BLL.SR.SR_TOPIC_SUB_ITEM.Instance.GetTopicSubItemList(topicId);
                var libaleList = ueList.Select(x => new TopicSubItem()
                {
                    SUBJECT_NAME = getSubjuct(x.SUB_ITEM_ID),
                    SUB_ITEM_NAME = BLL.SR.SR_SUB_ITEM.Instance.GetStringValueByKey(x.SUB_ITEM_ID, "NAME"),
                    WEIGHT = x.WEIGHT,
                    REMARK =getRuleTips(x.SUB_ITEM_ID, x.REMARK),
                    ID = x.ID,
                    SUB_ITEM_ID = x.SUB_ITEM_ID,
                    SCORE = getSubItemScore(x.ID),
                    SCORE_REMARK = getSubItemRemark(x.ID)
                });
                return SerializeObject(libaleList);
            }
            else

                return null;

        }
         /// <summary>
        /// 获取单项平方规则说明，评分规则
        /// </summary>
        /// <param name="topicSubItemId"></param>
        /// <returns></returns>
        private string getRuleTips(int subItemId,string remark)
        {
            var topicSubItem = BLL.SR.SR_SUB_ITEM.Instance.GetEntityByKey<BLL.SR.SR_SUB_ITEM.Entity>(subItemId);

            if (topicSubItem != null)
            {
                return "评分规则:"+ topicSubItem.RULE + "\n评分说明:"+ topicSubItem.REMARK + "\n设置备注:"+ remark ;
            }
            else
            {

                return "";
            }
        }
        /// <summary>
        /// 获取单项评分
        /// </summary>
        /// <param name="topicSubItemId"></param>
        /// <returns></returns>
        private double getSubItemScore(int  topicSubItemId)
        {
           var topicScore= BLL.SR.SR_TOPIC_SCORE.Instance.GetTopicSubItemScoreList(topicSubItemId, SystemSession.UserID);
            if (topicScore != null)
            {
                return topicScore.SCORE;
            }
            else
            {

                return 0;
            }
        }

        /// <summary>
        /// 获取单项评分备注
        /// </summary>
        /// <param name="topicSubItemId"></param>
        /// <returns></returns>
        private string getSubItemRemark(int topicSubItemId)
        {
            var topicScore = BLL.SR.SR_TOPIC_SCORE.Instance.GetTopicSubItemScoreList(topicSubItemId, SystemSession.UserID);
            if (topicScore != null)
            {
                return topicScore.REMARK;
            }
            else
            {

                return "";
            }
        }

        #endregion


        #region 科教总体评分，立项
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="topicid"></param>
        /// <returns></returns>
        public ActionResult TopicScore(int topicid = 5)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.SR.SR_TOPIC.Entity model =
                BLL.SR.SR_TOPIC.Instance.GetEntityByKey<BLL.SR.SR_TOPIC.Entity>(topicid);
            BLL.SR.SR_TOPIC_TYPE.Entity topicType = BLL.SR.SR_TOPIC_TYPE.Instance.GetEntityByKey<BLL.SR.SR_TOPIC_TYPE.Entity>(model.TOPIC_TYPE_ID);
            ViewBag.TopicTypeName = topicType.NAME;
            //参加的人员
            List<BLL.SR.SR_TOPIC_USER.Entity> topicUsers = BLL.SR.SR_TOPIC_USER.Instance.GetTopicUserList(topicid);
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

            IList<BLL.SR.SR_TOPIC_EXPERT.Entity> allList = BLL.SR.SR_TOPIC_EXPERT.Instance
                .GetTopicExpertList(topicid);
            var opList = allList.Where(x => x.IS_SCORE == 1);
            if (opList != null && opList.Count() > 0)
            {
                ViewBag.AllScore = opList.Sum(x => x.SCORE);
                ViewBag.MaxScore = opList.Max(x => x.SCORE);
                ViewBag.MinScore = opList.Min(x => x.SCORE);
                ViewBag.AvgScore = ViewBag.AllScore / opList.Count();
            }
            else
            {
                ViewBag.AllScore = 0;
                ViewBag.MaxScore = 0;
                ViewBag.MinScore = 0;
                ViewBag.AvgScore = 0;
            }
            ViewBag.IS_APPROVAL = model.IS_APPROVAL;
            ViewBag.APPROVAL_REMARK = model.APPROVAL_REMARK;
            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// 课题立项保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult TopicScore(SR_TOPIC.Entity entity)
        {
            int topicId = entity.ID;
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "课题项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var topic = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(entity.ID);
                topic.IS_APPROVAL = entity.IS_APPROVAL;
                topic.APPROVAL_REMARK = entity.APPROVAL_REMARK;
                topic.TOTAL_SCORE = entity.TOTAL_SCORE;
                topic.MAX_SCORE = entity.MAX_SCORE;
                topic.MIN_SCORE = entity.MIN_SCORE;
                topic.AVG_SCORE = entity.AVG_SCORE;
                topic.APPROVAL_TIME = DateTime.Now;
                SR_TOPIC.Instance.UpdateByKey(topic, topic.ID);//修改
                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的课题成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 评分总情况展示
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public string GetTopicExpertByTopic(int topicId)
        {
            if (topicId != 0)
            {
                IList<BLL.SR.SR_TOPIC_EXPERT.Entity> ueList = BLL.SR.SR_TOPIC_EXPERT.Instance
                    .GetTopicExpertList(topicId).OrderByDescending(x => x.SCORE).ThenBy(x => x.OPER_TIME).ToList();
                var libaleList = ueList.Select(x => new TopicExpert()
                {
                    EXPERT_NAME = BLL.FW.BF_USER.Instance.GetStringValueByKey(x.USER_ID, "FULL_NAME"),
                    ID = x.ID,
                    TOPIC_ID = x.TOPIC_ID,
                    USER_ID = x.USER_ID,
                    IS_SCORE = x.IS_SCORE,
                    SCORE = x.SCORE,
                    OPER_TIME = x.OPER_TIME,
                    CITY_NAME = getCityName(x.USER_ID),
                    ALL_SCORE = getAllScore(x.USER_ID, x.TOPIC_ID)
                });
                return SerializeObject(libaleList);
            }
            else

                return null;

        }

        private string getCityName(int userId)
        {
            string deptId = BLL.FW.BF_USER.Instance.GetStringValueByKey(userId, "DEPT_ID");
            return BLL.FW.BF_DEPARTMENT.Instance.GetEntity<BLL.FW.BF_DEPARTMENT.Entity>("DEPT_CODE=?", deptId).NAME;
        }
        private double getAllScore(int userId, int topicId)
        {
            var topicScores = BLL.SR.SR_TOPIC_SCORE.Instance.GetTopicScoreList(topicId, userId);
            double allScore = 0;
            foreach (var score in topicScores)
            {
                allScore += score.SCORE;
            }

            return allScore;
        }

        /// <summary>
        /// 获取学科名称
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        private string getSubjuct(int subItemId)
        {
            var subitem = BLL.SR.SR_SUB_ITEM.Instance.GetEntityByKey<BLL.SR.SR_SUB_ITEM.Entity>(subItemId);
            return BLL.SR.SR_SUBJECT.Instance.GetEntityByKey<BLL.SR.SR_SUBJECT.Entity>(subitem.SUBJECT_ID).NAME;
        }
        /// <summary>
        /// 单个课题，单个老师评分详情
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public string GetTopicExpertDetailByTopicandUser(int topicId, int userId)
        {
            if (topicId != 0 && userId != 0)
            {
                var topicScores = BLL.SR.SR_TOPIC_SCORE.Instance.GetTopicScoreList(topicId, userId);

                var libaleList = topicScores.Select(x => new TopicScore()
                {
                    SUBJECT_NAME = getSubjuct(x.SUB_ITEM_ID),
                    SUB_ITEM_NAME = BLL.SR.SR_SUB_ITEM.Instance.GetStringValueByKey(x.SUB_ITEM_ID, "NAME"),
                    WEIGHT = x.WEIGHT,
                    REMARK = x.REMARK,
                    SCORE = x.SCORE,
                    ACT_SCORE = x.ACT_SCORE,
                    ID = x.ID,
                    SUB_ITEM_ID = x.SUB_ITEM_ID
                });
                return SerializeObject(libaleList);
            }
            else

                return null;

        }
        #endregion
    }
}