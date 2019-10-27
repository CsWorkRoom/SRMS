using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.BLL.FW;
using CS.BLL;
using CS.Base.Log;
using CS.WebUI.Models;
using System.Data;
using System.Text;
using CS.WebUI.Models.SR;
using String = System.String;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 主页
    /// </summary>
    public class HomeController : ABaseController
    {
        /// <summary>
        /// 框架首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //写其它内容
            #region 其它内容写到这里以方便剥离

            #endregion

            return View();
        }
        public ActionResult manager()
        {
            return View();
        }
        /// <summary>
        /// 默认标签页
        /// </summary>
        /// <returns></returns>
        public ActionResult Default()
        {
            int userID = SystemSession.UserID;
            ViewBag.Title = MvcApplication.SystemName + "-" + "首页";

            //写其它内容
            #region 公告
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order("ID", "DESC");
            var bus = BF_BULLETIN.Instance.GetListPage(5, 1, order, "IS_ENABLE=1", null);
            ViewBag.BF_BULLETINS = bus;
            #endregion
            #region 流程代办数量统计
            string flowSql = string.Format(@"select count(1) cnt
                             from 
                            BF_FLOW_NODE_CASE ncase
                            left join BF_FLOW_CASE fcase on ncase.flow_case_id=fcase.id
                            left join BF_FLOW_NODE_CASE_RECORD  cord  on cord.flow_node_case_id=ncase.id and CORD.AUDIT_UID=1 
                            left join BF_USER U ON U.ID=NCASE.CREATE_UID
                            where NCASE.AUDIT_STATUS=0 and cord.audit_status is  null
                            and instr(','||user_ids||',',','||{0}||',')>0", userID);
            DataTable flowDt=BF_DATABASE.Instance.ExecuteSelectSQL(0, flowSql, null);
            ViewBag.FlowCnt = flowDt!=null&& flowDt.Rows.Count>0?flowDt.Rows[0][0].ToString():"0";
            #endregion
            #region 课题设置，设置评分项

            string setSql = string.Format(@"SELECT count(1) cnt FROM (
                                select T.ID,P.NAME TYPE_NAME,T.NAME,T.START_TIME,T.END_TIME,U.FULL_NAME,T.CREATE_TIME,T.FLOW_STATE,T.IS_APPROVAL，
                                (SELECT DECODE(COUNT(1),0,'未设置','已设置')  FROM SR_TOPIC_EXPERT E WHERE E.TOPIC_ID=T.ID) EXPERT_STATUS
                                 from SR_TOPIC T,BF_USER U,SR_TOPIC_TYPE P
                                WHERE U.ID=T.CREATE_USER_ID
                                AND   P.ID=T.TOPIC_TYPE_ID
                                AND   T.IS_ADOPT=1
                                )
                                WHERE   EXPERT_STATUS='未设置'", userID);
            DataTable setDt = BF_DATABASE.Instance.ExecuteSelectSQL(0, setSql, null);
            ViewBag.SetCnt = setDt != null && setDt.Rows.Count > 0 ? setDt.Rows[0][0].ToString() : "0";
            #endregion
            #region 专家评分
            string expertSql = string.Format(@"SELECT count(*) cnt FROM (
                                select E.ID,P.NAME TYPE_NAME,T.NAME,T.START_TIME,T.END_TIME,U.FULL_NAME,T.CREATE_TIME,
                                decode(E.IS_SCORE,0,'未评分','已评分') IS_SCORE,E.SCORE,E.OPER_TIME
                                 from SR_TOPIC_EXPERT E,SR_TOPIC T,BF_USER U,SR_TOPIC_TYPE P
                                WHERE U.ID=T.CREATE_USER_ID AND E.TOPIC_ID=T.ID
                                AND P.ID=T.TOPIC_TYPE_ID AND E.USER_ID={0}
                                )
                                where  IS_SCORE='未评分'", userID);
            DataTable expertDt = BF_DATABASE.Instance.ExecuteSelectSQL(0, expertSql, null);
            ViewBag.ExpertCnt = expertDt != null && expertDt.Rows.Count > 0 ? expertDt.Rows[0][0].ToString() : "0";
            #endregion
            #region 中期任务
            string doSql = string.Format(@"SELECT count(1) cnt FROM (
                                 select T.ID,P.NAME PNAME,R.NAME TOPIC_NAME,T.NAME TNAME,T.BEGIN_TIME,T.END_TIME,T.REMARK,U.FULL_NAME,T.CREATE_TIME,
                                F.FLOW_STATE,N.NAME FLOW_NODE_NAME,DECODE(F.IS_ADOPT,NULL,'未填报',0,'审核中...',1,'审核通过...',2,'审核不通过...') IS_ADOPT,
                                'SR_TOPIC_TASK_DONE' TYPE,case when (F.FLOW_STATE!=0 and F.IS_ADOPT=0) or (F.IS_ADOPT=1) THEN 'auditing' else 'noaudit'   end auidtType
                                 from SR_TOPIC_TASK T
                                 LEFT JOIN SR_TOPIC_TASK_DONE F ON F.TOPIC_TASK_ID=T.ID
                                 left join BF_USER U on U.ID=T.CREATE_UID
                                 left join SR_TOPIC R on  R.ID=T.TOPIC_ID
                                 LEFT JOIN SR_TOPIC_TYPE P ON P.ID=R.TOPIC_TYPE_ID
                                 left join BF_FLOW_NODE N ON F.FLOW_STATE=N.ID
                                where  T.CREATE_UID=1 
                                )
                                where  IS_ADOPT='未填报'");
            DataTable doDt = BF_DATABASE.Instance.ExecuteSelectSQL(0, doSql, null);
            ViewBag.DoCnt = doDt != null && doDt.Rows.Count > 0 ? doDt.Rows[0][0].ToString() : "0";
            #endregion

            return View();
        }

        #region 项目中的个性化内容写到这里，以方便剥离
        /// <summary>
        /// 数量统计
        /// </summary>
        /// <returns></returns>
        public string GetTotal()
        {
            List<string> legend = new List<string>();
            legend.Add("科研课题");
            legend.Add("科研论文");
            legend.Add("科研专利");
            List<string> xAxis = new List<string>();
            var nowDay = DateTime.Now;
            for (int i = 7; i > -1; i--)
            {
                var day = nowDay.AddDays(-i).ToString("yyyy-MM-dd");
                xAxis.Add(day);
            }
            ChartModel chart = new ChartModel();
            chart.legend = legend;
            chart.xAxis = xAxis;
            List<Serie> series = new List<Serie>();
            string d = string.Join(",", xAxis);
            Serie stopic = SetSerie("科研课题", d, "SR_TOPIC", "create_user_id");
            series.Add(stopic);
            Serie paper = SetSerie("科研论文", d, "SR_PAPER_RECORD", "create_uid");
            series.Add(paper);
            Serie patent = SetSerie("科研专利", d, "SR_PATENT", "create_user_id");
            series.Add(patent);
            chart.series = series;
            return SerializeObject(chart).ToLower();
        }

        private Serie SetSerie(string name, string days, string tablename, string cuserid)
        {
            int userID = SystemSession.UserID;
            var currUser = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(userID);
            string cuser = "";
            if (currUser.NAME.ToLower() != "admin")
            {
                cuser = string.Format(@" where {0}={1}", cuserid, userID);
            }

            string sql = string.Format(@"select  d.ctime,nvl(c.cnt,0) cnt from (
                        select regexp_substr('{0}','[^,]+', 1, level, 'i') as ctime from dual connect
                         by level <= length('{0}')-
                         length(regexp_replace('{0}', ',', ''))+1
                         ) d left join 
                         (select ctime,count(1) cnt from (
                        select id, to_char(create_time,'yyyy-MM-dd') ctime from {1} {2} 
                        ) t group by   ctime) c
                        on d.ctime=c.ctime order by ctime", days, tablename, cuser);
            DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(0, sql, null);//series
            Serie se = new Serie();
            se.name = name;
            se.type = "line";
            List<int> da = new List<int>();
            foreach (DataRow dr in dt.Rows)
            {
                da.Add(Convert.ToInt32(dr["cnt"].ToString()));
            }
            se.data = da;
            return se;
        }

        /// <summary>
        /// 费用统计
        /// </summary>
        /// <returns></returns>
        public string GetFee()
        {
            List<string> product = new List<string>();
            product.Add("product");
            var nowDay = DateTime.Now;
            List<string> cols = new List<string>();
            for (int i = 7; i > -1; i--)
            {
                var day = nowDay.AddDays(-i).ToString("yyyy-MM-dd");
                product.Add(day);
                cols.Add(string.Format(@"sum(case t.ctime
                                    when '{0}' then t.fee
                                    else 0
                                end) a{1}", day, i));
            }
            string d = string.Join("','", product);
            int userID = SystemSession.UserID;
            var currUser = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(userID);
            string suser = "";
            string puser = "";
            if (currUser.NAME.ToLower() != "admin")
            {
                suser = string.Format(@" and tf.create_uid={0}", userID);
                puser = string.Format(@" and create_uid={0}", userID);
            }
            string sql = string.Format(@"select t.name,{1} from (select name,ctime,sum(total_fee) fee from(
                        select ty.name,to_char(tf.create_time,'yyyy-MM-dd') ctime,tf.total_fee from sr_topic_funds tf,sr_topic t,sr_topic_type ty
                        where tf.topic_id=t.id and t.topic_type_id=ty.id  
                          and to_char(tf.create_time,'yyyy-MM-dd') in ('{0}') {2}
                        union 
                        select '论文版面' name,to_char(create_time,'yyyy-MM-dd') ctime,total_fee from sr_paper_record_funds 
                           where to_char(create_time,'yyyy-MM-dd') in ('{0}') {3}
                        ) t  group by t.name,t.ctime
                        ) t group by t.name", d, string.Join(",", cols), suser, puser);
            DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(0, sql, null);//series
            int rowCnt = dt.Rows.Count;
            List<dynamic> source = new List<dynamic>();
            source.Add(product);
            //source.Add(new List<dynamic> { "Matcha Latte", 41.1, 30.4, 65.1, 53.3, 83.8, 98.7 });
            //source.Add(new List<dynamic> { "Milk Tea", 86.5, 92.1, 85.7, 83.1, 73.4, 55.1 });
            //source.Add(new List<dynamic> { "Cheese Cocoa", 24.1, 67.2, 79.5, 86.4, 65.2, 82.5 });
            //source.Add(new List<dynamic> { "Walnut Brownie", 55.2, 67.1, 69.2, 72.4, 53.9, 39.1 });

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    List<dynamic> item = new List<dynamic>();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        item.Add(dr[i]);
                    }
                    source.Add(item);
                }
            }
            List<dynamic> series = new List<dynamic>();
            string[] center = { "90%", "25%" };
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                series.Add(new { type = "line", smooth = true, seriesLayoutBy = "row" });
            }
            series.Add(new
            {
                type = "pie",
                id = "pie",
                radius = "40%",
                center = center,
                label = new
                {
                    formatter = "{b}: {@" + nowDay.ToString("yyyy-MM-dd") + "} ({d}%)"
                },
                encode = new
                {
                    itemName = "product",
                    value = nowDay.ToString("yyyy-MM-dd"),
                    tooltip = nowDay.ToString("yyyy-MM-dd")
                }
            });



            var newObj = new { source = source, series = series, day = nowDay.ToString("yyyy-MM-dd") };
            return SerializeObject(newObj);
        }
        #endregion


        #region 框架中的方法，请勿进行个性化修改

        /// <summary>
        /// 根据菜单名称：获取菜单URL
        /// </summary>
        /// <param name="name">菜单名称(名称可以重复，取名复杂点)</param>
        /// <returns></returns>
        public ActionResult ModuleUrl(string name)
        {
            JsonResultData result = new JsonResultData();
            int num = 0;
            try
            {
                if (name == "" || name == null)
                {
                    //获取当前用户未读的数量
                    int userID = SystemSession.UserID;
                    num = BF_BULLETIN_USER.Instance.GetBullReadNum(userID);
                    result.IsSuccess = true;
                    result.Message = num.ToString();
                }
                else
                {
                    var model = BLL.FW.BF_MENU.Instance.GetList<BF_MENU.Entity>("NAME=?", name);
                    if (model.Count() < 1)
                    {
                        result.IsSuccess = false;
                        result.Message = "未找到该菜单!";
                    }
                    else
                    {
                        //只取第一条
                        result.IsSuccess = true;
                        result.Message = model[0].URL;
                    }
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "删除公告程序出现错误：" + ex.ToString());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult SecurityCode()
        {
            string code = BF_USER.GetValidateCode(4);
            TempData["SecurityCode"] = code;
            return File(BF_USER.GetValidateImage(code), "image/Jpeg");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            SystemSession.Clear();
            ViewBag.ErrorMessage = "";
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string password, string captcha)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.ErrorMessage = "用户名不可为空";
                return View();
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "密码不可为空";
                return View();
            }
            if (string.IsNullOrWhiteSpace(captcha))
            {
                ViewBag.ErrorMessage = "验证码不可为空";
                return View();
            }
            if (CheckCaptcha(captcha) == false)
            {
                ViewBag.ErrorMessage = "验证码不正确";
                return View();
            }

            string errorMessage = string.Empty;
            BF_USER.Entity userInfo = null;
            Dictionary<int, BF_MENU.Entity> menus = new Dictionary<int, BF_MENU.Entity>();
            if (BF_USER.Instance.Login(name, password, out userInfo, out menus, out errorMessage) == false)
            {
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }

            if (menus == null || menus.Count < 1)
            {
                ViewBag.ErrorMessage = "没有可访问的菜单，请联系管理员分配角色";
                return View();
            }

            //写SESSION
            WriteSession(userInfo, menus);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        private bool CheckCaptcha(string captcha)
        {
            return true;//开发期间临时关闭验证码
            string code = TempData["SecurityCode"] as string;
            return string.IsNullOrWhiteSpace(captcha) == false && captcha.ToUpper() == code.ToUpper();
        }

        /// <summary>
        /// 写入SESSION
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="menus">可访问菜单</param>
        private void WriteSession(BF_USER.Entity userInfo, Dictionary<int, BF_MENU.Entity> menus)
        {
            SystemSession.WriteSession(userInfo.ID, userInfo.NAME, userInfo.FULL_NAME, userInfo.DEPT_ID, menus);
        }

        #endregion


    }

}