using CS.Base.DBHelper;
using CS.Base.Log;
using CS.BLL.FW;
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
    /// 部门管理
    /// </summary>
    public class AfDepartmentController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //加载下拉
            GetSelectData();
            //部门名称
            ViewBag.DIC_DEPT_NAME = BF_DEPARTMENT.Instance.GetDictionary("DEPT_CODE", "NAME");
            return View();
        }

        /// <summary>
        /// 获取一个层级的区域列表
        /// </summary>
        /// <param name="level">层级（1-4层，即地市-渠道）</param>
        /// <param name="key">检索关键词（模糊查询）</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAreaListByLevel(int level = 1, string key = "")
        {
            List<Dictionary<string, string>> jsonlist = new List<Dictionary<string, string>>();

            IList<BF_DEPARTMENT.Entity> list = new List<BF_DEPARTMENT.Entity>();
            if (string.IsNullOrWhiteSpace(key) == true)
            {
                list = BF_DEPARTMENT.Instance.GetList<BF_DEPARTMENT.Entity>("DEPT_CODE>0 AND DEPT_LEVEL=?", level);
            }
            else
            {
                list = BF_DEPARTMENT.Instance.GetList<BF_DEPARTMENT.Entity>("DEPT_CODE>0 AND DEPT_LEVEL=? AND NAME LIKE '%" + key.Replace('\'', '"') + "%'", 1, level);
            }
            if (list != null && list.Count > 0)
            {
                foreach (var entity in list)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    if (dic.ContainsKey(entity.DEPT_CODE.ToString()) == false)
                    {
                        dic.Add("code", entity.DEPT_CODE.ToString());
                        dic.Add("name", entity.NAME);

                        jsonlist.Add(dic);
                    }
                }
            }

            return Json(jsonlist);
        }

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="pcode"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, int pcode = 0, string name = "", string code = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            List<object> param = new List<object>();
            if (pcode > 0)
            {
                where += " AND P_CODE=?";
                param.Add(pcode);
            }
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(code) == false)
            {
                where += " AND DEPT_CODE=? ";
                param.Add(code);
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BF_DEPARTMENT.Entity> list = BF_DEPARTMENT.Instance.GetListPage<BF_DEPARTMENT.Entity>(limit, page, order, where, param);
            int count = BF_DEPARTMENT.Instance.GetCount(where, param);
            if (list == null)
            {
                list = new List<BF_DEPARTMENT.Entity>();
                count = 0;
            }
            return ToJsonString(list, count);
        }
        #endregion

        #region 编辑及新增
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0, int pcode = 0)
        {
            BF_DEPARTMENT.Entity entity = new BF_DEPARTMENT.Entity();
            //加载下拉
            GetSelectData();
            if (id > 0)
            {
                entity = BF_DEPARTMENT.Instance.GetEntityByKey<BF_DEPARTMENT.Entity>(id);
                if (entity == null)
                {
                    return ShowAlert("部门不存在");
                }
            }
            else
            {
                entity.P_CODE = pcode;
                if (pcode > 0)
                {
                    BF_DEPARTMENT.Entity pe = BF_DEPARTMENT.Instance.GetEntityByKey<BF_DEPARTMENT.Entity>(pcode);
                    if (pe != null)
                    {
                        entity.DEPT_LEVEL = Math.Min(4, pe.DEPT_LEVEL + 1);
                    }
                }
            }
            return View(entity);
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
        public ActionResult Edit(BF_DEPARTMENT.Entity entity)
        {
            JsonResultData result = new JsonResultData();
            #region 验证
            if (string.IsNullOrEmpty(entity.NAME))
            {
                result.Message = "部门名称不可为空！";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (entity.P_CODE != 0 && entity.DEPT_CODE == entity.P_CODE)
            {
                result.IsSuccess = false;
                result.Message = "父级节点不能与当前目节点为同一级！";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            int i = 0;
            try
            {
                #region 验证重复性
                if (entity.DEPT_CODE >= 0)
                {
                    if (BF_DEPARTMENT.Instance.IsDuplicate(entity.ID, "DEPT_CODE", entity.DEPT_CODE.ToString()))
                    {
                        result.IsSuccess = false;
                        result.Message = "部门编码" + entity.DEPT_CODE + "已存在";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("NAME", entity.NAME);
                dic.Add("REMARK", entity.REMARK);
                dic.Add("P_CODE", entity.P_CODE);
                dic.Add("DEPT_CODE", entity.DEPT_CODE);
                dic.Add("DEPT_LEVEL", entity.DEPT_LEVEL);
                dic.Add("DEPT_FLAG", entity.DEPT_FLAG);
                dic.Add("UPDATE_TIME", DateTime.Now);
                dic.Add("UPDATE_UID", SystemSession.UserID);

                //修改
                if (entity.ID > 0) //增加与修改的判断
                {
                    i = BF_DEPARTMENT.Instance.UpdateByKey(dic, entity.ID);
                }
                else
                {
                    //增加
                    dic.Add("CREATE_TIME", DateTime.Now);
                    dic.Add("CREATE_UID", SystemSession.UserID);
                        i = BF_DEPARTMENT.Instance.Add(dic);
                }

                result.IsSuccess = i > 0 ? true : false;
                result.Message = i > 0 ? "数据提交成功！" : "数据提交失败！";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            JsonResultData result = new JsonResultData();
            result.IsSuccess = false;
            result.Message = "节点含有子节点，不能删除。删除失败！";

            int intCount = BF_DEPARTMENT.Instance.GetCount("pid=?", id);
            if (intCount == 0)
            {
                result.IsSuccess = BF_DEPARTMENT.Instance.DeleteByKey(id) > 0;
                if (result.IsSuccess)
                {
                    result.Message = "删除完成。";
                }
                else
                    result.Message = "程序异常。删除失败！";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 加载下拉
        /// <summary>
        /// 加载下拉数据
        /// </summary>
        private void GetSelectData()
        {
            #region 新2018/12/26
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
            #endregion

            #region 作废2018/12/26
            //DataTable dt = BF_DEPARTMENT.Instance.GetTable();
            //var obj = new List<object>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    obj.Add(
            //        new
            //        {
            //            id = Convert.ToInt32(dr["DEPT_CODE"]),
            //            pId = Convert.ToInt32(dr["P_CODE"]),
            //            name = dr["NAME"].ToString(),
            //            value = Convert.ToInt32(dr["ID"])
            //        });
            //}
            //ViewBag.DepartmentSelect = SerializeObject(obj);
            #endregion

            //部门层级
            Dictionary<int, string> dicDeptLevel = new Dictionary<int, string>();
            foreach (Enums.DepartmentLevel level in Enum.GetValues(typeof(Enums.DepartmentLevel)))
            {
                dicDeptLevel.Add((int)level, level.ToString());
            }
            ViewBag.DIC_DEPT_LEVEL = dicDeptLevel;
        }
        /// <summary>
        /// 获取省/地市json
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCitys()
        {
            DataTable dt = BF_DEPARTMENT.Instance.GetTable("(DEPT_LEVEL=? OR DEPT_LEVEL=?) AND DEPT_FLAG=?"
                , CS.Common.FW.Enums.DepartmentLevel.省公司.GetHashCode()
                , CS.Common.FW.Enums.DepartmentLevel.地市.GetHashCode()
                , 1);//获取地市(计件特殊约定：DEPT_FLAG=1代表组织，=0代表部门)
            var obj = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = Convert.ToInt32(dr["DEPT_CODE"]),
                        pId = Convert.ToInt32(dr["P_CODE"]),
                        name = dr["NAME"].ToString()
                    });
            }
            return Json(obj);
        }
        #endregion

        #region 导出文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(int pcode = 0, string name = "", string code = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_组织机构管理_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                List<object> param = new List<object>();
                if (pcode > 0)
                {
                    where += " AND P_CODE=?";
                    param.Add(pcode);
                }
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                if (string.IsNullOrWhiteSpace(code) == false)
                {
                    where += " AND DEPT_CODE=? ";
                    param.Add(code);
                }
                var dt = BF_DEPARTMENT.Instance.GetTableFields("NAME,DEPT_CODE,DEPT_LEVEL,P_CODE,DEPT_FLAG,REMARK,CREATE_TIME,UPDATE_TIME", where, param);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns.Remove("ID");
                dt.Columns["NAME"].Caption = "部门名称";
                dt.Columns["DEPT_CODE"].Caption = "部门编码";
                dt.Columns["DEPT_LEVEL"].Caption = "部门层级";
                dt.Columns["P_CODE"].Caption = "上级部门编码";
                dt.Columns["DEPT_FLAG"].Caption = "部门标志";
                dt.Columns["REMARK"].Caption = "部门说明";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                dt.Columns["UPDATE_TIME"].Caption = "最后更新时间";

                Library.Export.ExcelFile export = new Library.Export.ExcelFile(path);
                string fullName = export.ToExcel(dt);
                if (string.IsNullOrWhiteSpace(fullName) == true)
                {
                    return ShowAlert("未生成Excel文件");
                }
                System.Web.HttpContext.Current.Response.Buffer = true;
                System.Web.HttpContext.Current.Response.Clear();//清除缓冲区所有内容
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                System.Web.HttpContext.Current.Response.WriteFile(fullName);
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
        #endregion

    }
}