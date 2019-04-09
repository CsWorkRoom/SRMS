using CS.Base.Log;
using CS.BLL.FW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    public class AfScriptTypeController : ABaseController
    {

        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            GetSelectData();//加载下拉
            return View();
        }

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="pid"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, int pid = 0, string name = "", string orderByField = "st.ID", string orderByType = "desc")
        {

            int count = 0;
            DataTable data = BF_ST_TYPE.Instance.GetDataTable(limit, page, ref count, pid, name, orderByField, orderByType);
            return ToJsonString(data, count);
        }
        #endregion

        #region 加载编辑及新增
        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            BLL.FW.BF_ST_TYPE.Entity entity = new BLL.FW.BF_ST_TYPE.Entity();

            if (id > 0)
            {
                entity = BLL.FW.BF_ST_TYPE.Instance.GetEntityByKey<BLL.FW.BF_ST_TYPE.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "脚本类型不存在";
                    entity = new BLL.FW.BF_ST_TYPE.Entity();
                    entity.ID = -1;
                }
            }

            GetSelectData();//加载下拉
            ModelState.Clear();
            return View(entity);
        }
        #endregion

        #region 编辑提交
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BLL.FW.BF_ST_TYPE.Entity entity)
        {
            JsonResultData result = new JsonResultData();
            #region 验证
            if (string.IsNullOrEmpty(entity.NAME))
            {
                result.IsSuccess = false;
                result.Message = "类型名称不能为空，提交失败！";

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            #endregion

            int intResult = 0;
            int intUserId = BLL.FW.SystemSession.UserID;//当前登录

            #region 封装数据
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("NAME", entity.NAME);
            dic.Add("REMARK", entity.REMARK);
            dic.Add("PID", entity.PID);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", intUserId);
            #endregion

            try
            {
                if (entity.ID == entity.PID && entity.PID != 0)
                {
                    result.Message = "父级节点不能与当前目节点为同一级，数据提交失败！";
                }
                else
                {
                    if (entity.ID > 0) //增加与修改的判断
                    {//修改
                        intResult = BF_ST_TYPE.Instance.UpdateByKey(dic, entity.ID);
                    }
                    else
                    {//增加
                        dic.Add("CREATE_TIME", DateTime.Now);
                        dic.Add("CREATE_UID", intUserId);
                        intResult = BF_ST_TYPE.Instance.Add(dic);
                    }
                    result.IsSuccess = intResult > 0;
                    result.Message = intResult > 0 ? "数据提交成功！" : "数据提交失败！";
                }
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
            result.Message = "删除失败，当前分类已包含子分类，请先删除子分类";
            int intCount = BF_ST_TYPE.Instance.GetCount("pid=?", id);
            if (intCount == 0)
            {
                result.IsSuccess = BF_ST_TYPE.Instance.DeleteByKey(id) > 0;
                if (result.IsSuccess)
                    result.Message = "删除完成。";
                else
                    result.Message = "程序异常。删除失败！";

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 加载下拉
        private void GetSelectData()
        {
            DataTable dt = BLL.FW.BF_ST_TYPE.Instance.GetTable();
            var obj = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = Convert.ToInt32(dr["ID"]),
                        pId = Convert.ToInt32(dr["PID"]),
                        name = dr["NAME"].ToString()
                    });
            }

            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.ScriptSelect = SerializeObject(obj);
        }
        #endregion

        #region 导出文件附件         
        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(int pid = 0, string name = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_脚本类型_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                List<object> param = new List<object>();
                if (pid > 0)
                {
                    where += " AND PID=?";
                    param.Add(pid);
                }
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    where += " AND NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
                }
                int count = 0;
                var dt = BF_ST_TYPE.Instance.GetDataTable(0, 0, ref count, pid, name, "st.ID", "asc");
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["NAME"].Caption = "类型名称";
                dt.Columns["PNAME"].Caption = "上层类型";
                dt.Columns["REMARK"].Caption = "备注";
                dt.Columns["CREATENAME"].Caption = "创建者";
                dt.Columns["CREATE_TIME"].Caption = "创建时间";
                dt.Columns["UPDATENAME"].Caption = "最后更新者";
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