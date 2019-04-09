using CS.Base.Log;
using CS.BLL.FW;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 字段显示配置
    /// </summary>
    public class AfFieldDisplayController : ABaseController
    {
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //字段数据类型
            Dictionary<int, string> dicDataType = new Dictionary<int, string>();
            foreach (Enums.FieldDataType source in Enum.GetValues(typeof(Enums.FieldDataType)))
            {
                dicDataType.Add((int)source, source.ToString());
            }

            ViewBag.DIC_DATA_TYPE = dicDataType;
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="enName"></param>
        /// <param name="cnName"></param>
        /// <returns>JSON数据</returns>
        public string GetList(int page, int limit, string enName = "", string cnName = "", string orderByField = "ID", string orderByType = "ASC")
        {
            string where = "1=1";
            if (string.IsNullOrWhiteSpace(enName) == false)
            {
                where += " AND EN_NAME LIKE '%" + enName.Replace('\'', ' ') + "%'";
            }
            if (string.IsNullOrWhiteSpace(cnName) == false)
            {
                where += " AND CN_NAME LIKE '%" + cnName.Replace('\'', ' ') + "%'";
            }
            Library.BaseQuery.BBaseQuery.Order order = new Library.BaseQuery.BBaseQuery.Order(orderByField, orderByType);

            IList<BF_FIELD.Entity> list = BF_FIELD.Instance.GetListPage<BF_FIELD.Entity>(limit, page, order, where);
            int count = BF_FIELD.Instance.GetCount(where);
            if (list == null)
            {
                list = new List<BF_FIELD.Entity>();
                count = 0;
            }
            return ToJsonString(list, count);
        }

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            Dictionary<int, string> dicFieldDataType = new Dictionary<int, string>();
            foreach (CS.Common.FW.Enums.FieldDataType source in Enum.GetValues(typeof(CS.Common.FW.Enums.FieldDataType)))
            {
                dicFieldDataType.Add((int)source, source.ToString());
            }
            //
            Dictionary<int, string> dicWithobjList = new Dictionary<int, string>();
            dicWithobjList.Add((int)CS.Common.FW.Enums.FieldDataType.数值, "80");
            dicWithobjList.Add((int)CS.Common.FW.Enums.FieldDataType.文本, "120");
            dicWithobjList.Add((int)CS.Common.FW.Enums.FieldDataType.日期, "135");

            ViewBag.Result = false;
            ViewBag.Message = string.Empty;
            ViewBag.DIC_FIELD_DATA_TYPE = dicFieldDataType;
            ViewBag.DIC_FIELD_WITH = dicWithobjList;

            BF_FIELD.Entity entity = new BF_FIELD.Entity();

            if (id > 0)
            {
                entity = BF_FIELD.Instance.GetEntityByKey<BF_FIELD.Entity>(id);
                if (entity == null)
                {
                    ViewBag.Message = "字段不存在";
                    entity = new BF_FIELD.Entity();
                    entity.ID = -1;
                }
            }

            ModelState.Clear();
            return View(entity);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BF_FIELD.Entity entity)
        {
            JsonResultData result = new JsonResultData();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            dic.Add("EN_NAME", entity.EN_NAME.Trim());
            dic.Add("CN_NAME", entity.CN_NAME.Trim());
            dic.Add("FIELD_DATA_TYPE", entity.FIELD_DATA_TYPE);
            dic.Add("IS_REQUISITE", entity.IS_REQUISITE);
            dic.Add("IS_SHOW", entity.IS_SHOW);
            dic.Add("IS_FIXED", entity.IS_FIXED);
            dic.Add("IS_SORT", entity.IS_SORT);
            dic.Add("SHOW_LENGTH", entity.SHOW_LENGTH);
            dic.Add("SHOW_WIDTH", entity.SHOW_WIDTH);
            dic.Add("REMARK", entity.REMARK);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);
            try
            {
                if (entity.ID > 0)
                {
                    //交互
                    int i = BF_FIELD.Instance.UpdateByKey(dic, entity.ID);
                    if (i > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "更新成功";
                    }
                    else
                    {
                        result.Message = "更新失败!";
                    }
                }
                else
                {
                    //默认值
                    dic.Add("IS_DEFAULT", 0);
                    dic.Add("CREATE_TIME", DateTime.Now);
                    dic.Add("CREATE_UID", SystemSession.UserID);
                    //交互
                    int i = BF_FIELD.Instance.Add(dic);
                    if (i > 0)
                    {
                        result.IsSuccess = true;
                        result.Message = "新增成功";
                    }
                    else
                    {
                        result.Message = "新增失败!";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = "编辑出错：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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

            if (id > 0)
            {
                try
                {
                    result.IsSuccess = BF_FIELD.Instance.DeleteByKey(id) > 0;
                    if (result.IsSuccess)
                        result.Message = "删除成功";
                    else
                        result.Message = "删除失败！";
                }
                catch (Exception ex)
                {
                    result.Message = "删除出错：" + ex.Message;
                }
            }
            else
            {
                result.Message = "删除失败:ID必须大于0";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// 导出文件附件
        /// </summary>
        /// <param name="wherestr">搜索条件</param>
        /// <returns>附件信息</returns>
        public ActionResult ExportFile(string enName = "", string cnName = "")
        {
            try
            {
                string filename = HttpUtility.UrlEncode(string.Format("导出报表_字段显示配置_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")), Encoding.UTF8);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/tmp/");
                string where = "1=1";
                if (string.IsNullOrWhiteSpace(enName) == false)
                {
                    where += " AND EN_NAME LIKE '%" + enName.Replace('\'', ' ') + "%'";
                }
                if (string.IsNullOrWhiteSpace(cnName) == false)
                {
                    where += " AND CN_NAME LIKE '%" + cnName.Replace('\'', ' ') + "%'";
                }

                var dt = BF_FIELD.Instance.GetTableFields("EN_NAME,CN_NAME,FIELD_DATA_TYPE,IS_REQUISITE,IS_SHOW,SHOW_WIDTH", where);
                if (dt == null)
                {
                    return ShowAlert("导出数据到Excel出现错误：未查询出数据");
                }
                dt.Columns["EN_NAME"].Caption = "字段英文名";
                dt.Columns["CN_NAME"].Caption = "字段中文名";
                dt.Columns["FIELD_DATA_TYPE"].Caption = "IP字段类型";
                dt.Columns["IS_REQUISITE"].Caption = "是否必须";
                dt.Columns["IS_SHOW"].Caption = "是否显示";
                dt.Columns["SHOW_WIDTH"].Caption = "显示宽度";

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
    }
}