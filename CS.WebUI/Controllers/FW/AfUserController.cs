using CS.Base.Log;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class AfUserController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            GetSelectData();
            return View();
        }

        #region 加载下拉
        /// <summary>
        /// 加载下拉数据
        /// </summary>
        private void GetSelectData()
        {
            var departmentList = BLL.FW.BF_DEPARTMENT.Instance.GetList<BLL.FW.BF_DEPARTMENT.Entity>();
            var obj = new List<object>();
            if (departmentList != null && departmentList.Count > 0)
            {
                foreach (var dep in departmentList)
                {
                    obj.Add(new
                    {
                        id = dep.DEPT_CODE,
                        pId = dep.P_CODE,
                        name = dep.NAME,
                        value = dep.DEPT_CODE
                    });
                }
            }
            ViewBag.DepartmentSelect = SerializeObject(obj);
        }
        #endregion

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="name"></param>
        /// <param name="fullName"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string deptCode = "", string name = "", string fullName = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            int dept = 0;
            int.TryParse(deptCode, out dept);
            if (dept > 0)
            {
                where += " AND DEPT_ID = " + dept;
            }
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(fullName) == false)
            {
                where += " AND FULL_NAME LIKE '%" + fullName.Replace('\'', ' ') + "%'";
            }

            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);
            List<Models.FW.UserModelsForList> umList = new List<Models.FW.UserModelsForList>();
            IList<BLL.FW.BF_USER.Entity> ueList = BLL.FW.BF_USER.Instance.GetListPage(limit, page, order, where);
            int count = BLL.FW.BF_USER.Instance.GetCount(where);
            if (ueList != null && ueList.Count > 0)
            {
                Dictionary<int, string> dicDeptName = BLL.FW.BF_DEPARTMENT.Instance.GetDictionary("ID", "NAME");
                Dictionary<int, string> dicRoleName = BLL.FW.BF_ROLE.Instance.GetDictionary("ID", "NAME");

                foreach (BLL.FW.BF_USER.Entity ue in ueList)
                {
                    Models.FW.UserModelsForList um = new Models.FW.UserModelsForList();
                    um.ID = ue.ID;
                    if (dicDeptName.ContainsKey(ue.DEPT_ID) == true)
                    {
                        um.DEPT_NAME = dicDeptName[ue.DEPT_ID];
                    }
                    um.E_MAIL = ue.E_MAIL;
                    um.FULL_NAME = ue.FULL_NAME;
                    um.IS_ENABLE = ue.IS_ENABLE;
                    um.IS_LOCKED = ue.IS_LOCKED;
                    um.LAST_LOGIN_TIME = ue.LAST_LOGIN_TIME;
                    um.LOGIN_COUNT = ue.LOGIN_COUNT;
                    um.NAME = ue.NAME;
                    um.PHONE_NUMBER = ue.PHONE_NUMBER;
                    um.QQ = ue.QQ;

                    if (dicDeptName.Count > 0 && string.IsNullOrWhiteSpace(ue.ROLE_IDS) == false)
                    {
                        int roleID = 0;
                        foreach (string rid in ue.ROLE_IDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(rid, out roleID) && dicRoleName.ContainsKey(roleID) == true)
                            {
                                um.ROLE_NAMES += dicRoleName[roleID] + ",";
                            }
                        }
                    }
                    umList.Add(um);
                }
            }

            return ToJsonString(umList, count);
        }

        # region 编辑及新增
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IsExpert">0=不是，1=是</param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0,int IsExpert=0 )
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.DIC_DEPTS = BLL.FW.BF_DEPARTMENT.Instance.GetDictionary("DEPT_CODE", "NAME");
            ViewBag.DIC_ROLES = BLL.FW.BF_ROLE.Instance.GetDictionary("ID", "NAME");

            Models.FW.UserModelsForEdit model = new Models.FW.UserModelsForEdit();

            if (id > 0)
            {
                BLL.FW.BF_USER.Entity entity = BLL.FW.BF_USER.Instance.GetEntityByKey<BLL.FW.BF_USER.Entity>(id);
                if (entity != null)
                {
                    model.ID = entity.ID;
                    model.DEPT_ID = entity.DEPT_ID;
                    model.ROLE_IDS = entity.ROLE_IDS;
                    model.NAME = entity.NAME;
                    model.FULL_NAME = entity.FULL_NAME;
                    model.PHONE_NUMBER = entity.PHONE_NUMBER;
                    model.E_MAIL = entity.E_MAIL;
                    model.QQ = entity.QQ;
                    model.FLAG_1 = entity.FLAG_1;
                    model.FLAG_2 = entity.FLAG_2;
                    model.FLAG_3 = entity.FLAG_3;
                    model.EXTEND_1 = entity.EXTEND_1;
                    model.EXTEND_2 = entity.EXTEND_2;
                    model.EXTEND_3 = entity.EXTEND_3;
                    model.TEC_LEVEL = entity.TEC_LEVEL;
                    model.TITLE_ID = entity.TITLE_ID;
                }
                else
                {
                    ViewBag.Message = "账户不存在";
                    model.ID = -1;
                }
            }

            #region 科研管理系统特有
            //职称下拉
            ViewBag.TitleList = CS.BLL.SR.SR_TITLE.Instance.GetList<CS.BLL.SR.SR_TITLE.Entity>();
            #endregion

            #region 组织树下拉
            var departmentList = BLL.FW.BF_DEPARTMENT.Instance.GetList<BLL.FW.BF_DEPARTMENT.Entity>();
            var obj = new List<object>();
            obj.Add(new { id = 0, pId = 0, name = "==请选择部门==", value = 0 });//添加根节点
            if(departmentList!=null&&departmentList.Count>0)
            {
                foreach(var dep in departmentList)
                {
                    obj.Add(new
                    {
                        id = dep.DEPT_CODE,
                        pId = dep.P_CODE,
                        name = dep.NAME,
                        value = dep.ID
                    });
                }
            }
            ViewBag.DepartmentSelect = SerializeObject(obj);

            #endregion

            #region 级别
            var obj2 = new List<object>();
            obj2.Add(new { key="初级" });//添加根节点
            obj2.Add(new { key = "中级" });//添加根节点
            obj2.Add(new { key = "高级" });//添加根节点

            List<string> levelList = new List<string>
            {
                "初级","中级","高级"
            };
            ViewBag.LevelList = levelList;
            #endregion

            #region 加载下拉列表--2018/12/26作废
            //DataTable dt = BLL.FW.BF_DEPARTMENT.Instance.GetTable();
            //var obj = new List<object>();
            //obj.Add(new { id = 0, pId = 0, name = "==请选择部门==", value = 0 });//添加根节点
            //foreach (DataRow dr in dt.Rows)
            //{
            //    obj.Add(
            //        new
            //        {
            //            id = Convert.ToInt32(dr["DEPT_CODE"]),
            //            pId = Convert.ToInt32(dr["P_CODE"]),
            //            name = dr["NAME"].ToString(),
            //            value = Convert.ToInt32(dr["DEPT_CODE"])
            //        });
            //}

            //ViewBag.DepartmentSelect = SerializeObject(obj);
            #endregion

            ViewBag.IsExpert = IsExpert;

            ModelState.Clear();
            return View(model);
        }
        #endregion

        #region 编辑提交
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModel"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Models.FW.UserModelsForEdit userModel, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();
            var roles = collection["ROLES"];
            if (!string.IsNullOrWhiteSpace(roles) && roles.IndexOf(',') > 0)
            {
                roles = "," + roles + ",";
            }
            userModel.ROLE_IDS = roles;
            int i = 0;
            int id = userModel.ID;
            try
            {
                if (id > 0)
                {
                    i = BLL.FW.BF_USER.Instance.Update(id, userModel.FULL_NAME, userModel.DEPT_ID, userModel.ROLE_IDS, userModel.PHONE_NUMBER, userModel.E_MAIL, userModel.QQ, userModel.FLAG_1, userModel.FLAG_2, userModel.FLAG_3, userModel.EXTEND_1, userModel.EXTEND_2, userModel.EXTEND_3,userModel.TEC_LEVEL, userModel.TITLE_ID);
                }
                else
                {
                    i = BLL.FW.BF_USER.Instance.Add(userModel.NAME, userModel.FULL_NAME, userModel.DEPT_ID, userModel.ROLE_IDS, userModel.PHONE_NUMBER, userModel.E_MAIL, userModel.QQ, userModel.FLAG_1, userModel.FLAG_2, userModel.FLAG_3, userModel.EXTEND_1, userModel.EXTEND_2, userModel.EXTEND_3, userModel.TEC_LEVEL, userModel.TITLE_ID);
                }

                if (i > 0)
                {
                    result.IsSuccess = true;
                    result.Message = "保存成功";
                }
                else
                {
                    result.Message = "未知错误";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Unlock(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                int i = 0;
                i = BLL.FW.BF_USER.Instance.Unlock(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "解锁成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "用户解锁程序出现错误：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetEnable(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                int i = 0;
                i = BLL.FW.BF_USER.Instance.SetEnable(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "启用成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "用户启用程序出现错误：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            //bool result = BLL.FW.BF_USER.Instance.SetEnable(id) > 0;
            //return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetUnable(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                int i = 0;
                i = BLL.FW.BF_USER.Instance.SetUnable(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "禁用成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "用户禁用程序出现错误：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            //bool result = BLL.FW.BF_USER.Instance.SetUnable(id) > 0;
            //return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword(int id)
        {
            JsonResultData result = new JsonResultData();
            try
            {
                int i = 0;
                i = BLL.FW.BF_USER.Instance.ResetPassword(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "重置密码成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "用户重置密码程序出现错误：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            //bool result = BLL.FW.BF_USER.Instance.ResetPassword(id) > 0;
            //return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetAll()
        {
            JsonResultData result = new JsonResultData();
            try
            {
                int i = BLL.FW.BF_USER.Instance.ResetPassword();
                result.IsSuccess = true;
                result.Message = "批量重置了" + i + "个用户的密码！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "批量重置用户密码程序出现错误：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldpsd"></param>
        /// <param name="newpsd"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(string oldpsd = "", string newpsd = "")
        {
            JsonResultData result = new JsonResultData();
            string error = string.Empty;
            try
            {
                if (BLL.FW.BF_USER.Instance.ChangePassword(oldpsd, newpsd, out error))
                {
                    result.IsSuccess = true;
                    result.Message = "修改密码成功，请牢记新密码";
                }
                else
                {
                    result.Message = error;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            
            JsonResultData result = new JsonResultData();
            try
            {
                //加一个判断，不能删除admin,免除误删
                int i = 0;
                BLL.FW.BF_USER.Entity entity = BLL.FW.BF_USER.Instance.GetEntityByKey<BLL.FW.BF_USER.Entity>(id);
                if (entity.NAME == "admin")
                {
                    result.IsSuccess = false;
                    result.Message = "不能删除管理员账号：admin";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                i = BLL.FW.BF_USER.Instance.DeleteByKey(id);
                if (i < 1)
                {
                    result.IsSuccess = false;
                    result.Message = "操作数据库出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "删除用户成功！";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "删除用户程序出现错误：" + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            //bool result = BLL.FW.BF_USER.Instance.DeleteByKey(id) > 0;
            //return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string name = "", string fullName = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_字段显示配置_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                if (string.IsNullOrWhiteSpace(fullName) == false)
                {
                    where += " AND FULL_NAME LIKE '%" + fullName.Replace('\'', ' ') + "%'";
                }

                var dt = BLL.FW.BF_USER.Instance.GetTableFields("ROLE_IDS,DEPT_ID,NAME,FULL_NAME,IS_ENABLE,IS_LOCKED,LOGIN_COUNT,LAST_LOGIN_TIME", where);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["NAME"].Caption = "登录名";
                dt.Columns["FULL_NAME"].Caption = "用户姓名";
                dt.Columns["IS_ENABLE"].Caption = "启用状态";
                dt.Columns["IS_LOCKED"].Caption = "锁定状态";
                dt.Columns["LOGIN_COUNT"].Caption = "登录次数";
                dt.Columns["LAST_LOGIN_TIME"].Caption = "最后登录时间";


                dt.Columns.Add(new DataColumn("DEPT_NAME", typeof(string)));
                dt.Columns["DEPT_NAME"].Caption = "所属部门";
                dt.Columns.Add(new DataColumn("ROLE_NAMES", typeof(string)));
                dt.Columns["ROLE_NAMES"].Caption = "拥有角色";

                Dictionary<int, string> dicDeptName = BLL.FW.BF_DEPARTMENT.Instance.GetDictionary("ID", "NAME");
                Dictionary<int, string> dicRoleName = BLL.FW.BF_ROLE.Instance.GetDictionary("ID", "NAME");
                foreach (DataRow dr in dt.Rows)
                {
                    var DEPT_ID = Convert.ToInt32(dr["DEPT_ID"]);
                    var ROLE_IDS = dr["ROLE_IDS"].ToString();
                    if (dicDeptName.ContainsKey(DEPT_ID) == true)
                    {
                        dr["DEPT_NAME"] = dicDeptName[DEPT_ID];
                    }

                    if (dicDeptName.Count > 0 && string.IsNullOrWhiteSpace(ROLE_IDS) == false)
                    {
                        int roleID = 0;
                        foreach (string rid in ROLE_IDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(rid, out roleID) && dicRoleName.ContainsKey(roleID) == true)
                            {
                                dr["ROLE_NAMES"] += dicRoleName[roleID] + ",";
                            }
                        }
                    }
                }
                dt.Columns.Remove("ROLE_IDS");
                dt.Columns.Remove("DEPT_ID");

                Library.Export.ExcelFile export = new Library.Export.ExcelFile(path);
                string fullname = export.ToExcel(dt);
                if (string.IsNullOrWhiteSpace(fullname) == true)
                {
                    return ShowAlert("未生成Excel文件");
                }
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                System.Web.HttpContext.Current.Response.WriteFile(fullname);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();
                //删除文件
                export.Delete(fullName);
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.WARN, "导出默认报表[DB]到Excel出错:" + ex.ToString());
                return ShowAlert("导出数据到Excel出现未知错误：" + ex.Message);
            }
            return ShowAlert("导出成功");
        }

        #region 非框架代码，方便剥离
       
        #endregion
    }
}