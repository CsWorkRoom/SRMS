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
    public class SrSearchController : ABaseController
    {
        public string Modular = "文件管理";
        // GET: SrTopic
        public ActionResult Index()
        {
            return View();
        }

        #region 文件检索
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchInfo()
        {
            return View();
        }


        public string DoSearch(string key)
        {
            try
            {
                Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order("ID", "DESC");
                string whereSql = string.Format(@"DISPLAY_NAME LIKE '%{0}%'", key);
                var FILES = SR_FILES.Instance.GetListPage<SR_FILES.Entity>(20, 1, whereSql, null);
                var fileModels = FILES.Select(e => new FileModel
                {
                    ID = e.ID,
                    FORMAT = e.FORMAT,
                    DISPLAY_NAME = e.DISPLAY_NAME,
                    SOURE = GetSource(e.PATH),
                    SOURE_OBJ = GetSourceObj(e.PATH, e.ID),
                    FILE_PATH = e.PATH.Replace("\\", "\\\\") + "\\" + e.REAL_NAME
                });
                var obj = new { Type = true, Files = fileModels };
                return SerializeObject(obj);
            }
            catch
            {
                var obj = new { Type = false };
                return SerializeObject(obj);
            }
        }

        public string GetSource(string path)
        {
            string type = "";
            if (path.Contains("PaperPath"))
            {
                type = "论文信息";
            }
            else if (path.Contains("PaperRecordFundsPath"))
            {
                type = "论文版面报销信息";
            }
            else if (path.Contains("PatentPath"))
            {
                type = "成果专利";
            }
            else if (path.Contains("TopicDetailPath"))
            {
                type = "课题完善信息";
            }
            else if (path.Contains("TopicPath"))
            {
                type = "课题申请信息";
            }
            else if (path.Contains("TopicEndPath"))
            {
                type = "课题结题信息";
            }
            else if (path.Contains("TopicFundsPath"))
            {
                type = "课题报销信息";
            }
            else if (path.Contains("TopicTaskPath"))
            {
                type = "课题中期信息";
            }
            else if (path.Contains("TopicTaskDonePath"))
            {
                type = "课题中期任务信息";
            }
            else if (path.Contains("SubjectPath"))
            {
                type = "学习资料信息";
            }
            else
            {
                type = "课题相关信息";
            }
            return type;
        }

        public FileSource GetSourceObj(string path, int id)
        {
            FileSource fs = new FileSource();
            string whereSql = string.Format(@"instr(files||',', ',{0},') > 0", id);
            if (path.Contains("PaperPath"))
            {
                var obj = SR_PAPER_RECORD.Instance.GetList<SR_PAPER_RECORD.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = obj.NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("PaperRecordFundsPath"))
            {
                var obj = SR_PAPER_RECORD_FUNDS.Instance.GetList<SR_PAPER_RECORD_FUNDS.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = SR_PAPER_RECORD.Instance.GetEntityByKey<SR_PAPER_RECORD.Entity>(obj.PAPER_RECORD_ID).NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("PatentPath"))
            {
                var obj = SR_PATENT.Instance.GetList<SR_PATENT.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = obj.ACHIEVEMENTS_NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_USER_ID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("TopicDetailPath"))
            {
                var obj = SR_TOPIC_DETAIL.Instance.GetList<SR_TOPIC_DETAIL.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(obj.TOPIC_ID).NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("TopicPath"))
            {
                var obj = SR_TOPIC.Instance.GetList<SR_TOPIC.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = obj.NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_USER_ID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("TopicEndPath"))
            {
                var obj = SR_TOPIC_END.Instance.GetList<SR_TOPIC_END.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(obj.TOPIC_ID).NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("TopicFundsPath"))
            {
                var obj = SR_TOPIC_FUNDS.Instance.GetList<SR_TOPIC_FUNDS.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = SR_TOPIC.Instance.GetEntityByKey<SR_TOPIC.Entity>(obj.TOPIC_ID).NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("TopicTaskPath"))
            {
                var obj = SR_TOPIC_TASK.Instance.GetList<SR_TOPIC_TASK.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = obj.NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("TopicTaskDonePath"))
            {
                var obj = SR_TOPIC_TASK_DONE.Instance.GetList<SR_TOPIC_TASK_DONE.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = SR_TOPIC_TASK.Instance.GetEntityByKey<SR_TOPIC_TASK.Entity>(obj.TOPIC_ID).NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }
            else if (path.Contains("SubjectPath"))
            {
                var obj = SR_SUBJECT_ARTICLE.Instance.GetList<SR_SUBJECT_ARTICLE.Entity>(whereSql).FirstOrDefault();
                if (obj != null)
                {
                    fs.NAME = obj.NAME;
                    fs.CREATE_TIME = obj.CREATE_TIME;
                    fs.USER_NAME = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(obj.CREATE_UID).FULL_NAME;
                }
                else
                {
                    fs = null;
                }
            }

            return fs;
        }
        #endregion

        #region  项目检索
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchItem()
        {
            return View();
        }

        public string DoSearchItem(string key)
        {
            try
            {
                string searchSql = string.Format(
                    @"select * from (
                        select ROWNUM AS rowno, name, content,create_time,full_name, source,url 
                    from (select  pr.name,to_char(substr(pr.content,1,200)) content,pr.create_time,u.full_name,'论文信息' source,'../SrPaperRecord/Edit?id='||pr.id url  from SR_PAPER_RECORD pr,bf_user u where PR.CREATE_UID=u.id
                          union
                        select  P.ACHIEVEMENTS_NAME,to_char(substr(p.remark,1,200)) content, p.create_time,u.full_name,'成果专利信息' source,'../SrPatent/FlowEdit?id='||p.id url  from SR_PATENT p,bf_user u where p.CREATE_User_ID=u.id
                        union
                        select  pr.name,to_char(substr(pr.content,1,200)) content,pr.create_time,u.full_name,'学习资料' source,'../SrSubjectArticle/Show?id='||pr.id url  from SR_SUBJECT_ARTICLE pr,bf_user u where PR.CREATE_UID=u.id
                        union
                        select  P.NAME,to_char(substr(p.remark,1,200)) content, p.create_time,u.full_name,'课题信息' source,'../SrTopic/FlowEdit?id='||p.id url from SR_TOPIC p,bf_user u where p.CREATE_User_ID=u.id
                        union
                        select  pr.name,pr.remark content,pr.create_time,u.full_name,'课题预算' source,'../SrTopicBudget/Edit?id='||pr.id url from SR_TOPIC_BUDGET pr,bf_user u where PR.CREATE_UID=u.id
                        union
                        select  pr.content name,pr.remark content,pr.create_time,u.full_name,'课题结题信息' source ,'../SrTopicEnd/Edit?id='||pr.id url from SR_TOPIC_END pr,bf_user u where PR.CREATE_UID=u.id
                        union
                        select  pr.name,pr.remark content,pr.create_time,u.full_name,'课题中期信息' source,'../SrTopicTask/Edit?id='||pr.id url from SR_TOPIC_TASK pr,bf_user u where PR.CREATE_UID=u.id
                         ) t where t.name like '%{0}%' or t.content like '%{0}%') t  where rowno<10", key );
                DataTable resDt = BF_DATABASE.Instance.ExecuteSelectSQL(0, searchSql, null);
                var obj = new { Type = true, res = resDt };
                return SerializeObject(obj);
            }
            catch
            {
                var obj = new { Type = false };
                return SerializeObject(obj);
            }
        }
        #endregion
    }
}