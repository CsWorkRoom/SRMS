using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CS.WebUI.Controllers
{
    /// <summary>
    /// 外导数据审批
    /// </summary>
    public class ImportApprovalController : FW.ABaseController
    {

        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="tableName"></param>
        /// <param name="importUID"></param>
        /// <param name="importTime"></param>
        /// <returns></returns>
        public ActionResult Approval(int id, int level, string tableName, int importUID, string importTime)
        {
            DateTime time = DateTime.Parse(importTime);
            ViewBag.ID = id;
            ViewBag.LEVEL = level;
            ViewBag.TABLE_NAME = tableName;
            ViewBag.IMPORT_UID = importUID;
            ViewBag.IMPORT_TIME = time.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.COUNT = 0;
            BLL.FW.BF_IMPORT.Entity entity = BLL.FW.BF_IMPORT.Instance.GetEntityByKey<BLL.FW.BF_IMPORT.Entity>(id);
            if (entity == null)
            {
                return ShowAlert("相关导入配置不存在，请联系管理员");
            }
            if (tableName.ToUpper().StartsWith(entity.TABLE_NAME.ToUpper()) == false)
            {
                return ShowAlert("表名错误，请联系管理员");
            }

            try
            {
                string sql = "SELECT COUNT(*) C FROM " + tableName + " WHERE 1=1 AND IMPORT_UID=? AND IMPORT_TIME=?";
                List<Object> param = new List<object>();
                param.Add(importUID);
                param.Add(time);

                if (level > 1)
                {
                    sql += " AND APPROVAL_" + (level - 1) + "=1";
                }

                DataTable dt = BLL.FW.BF_DATABASE.Instance.ExecuteSelectSQL(entity.DB_ID, sql, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.COUNT = Convert.ToInt32(dt.Rows[0][0]);
                }
                return View();
            }
            catch (Exception ex)
            {
                return ShowAlert(ex.Message);
            }
        }

        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="tableName"></param>
        /// <param name="importUID"></param>
        /// <param name="importTime"></param>
        /// <param name="isApproval"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Approval(int id, int level, string tableName, int importUID, DateTime importTime, int isApproval, string message)
        {
            JsonResultData result = new JsonResultData();

            try
            {
                BLL.FW.BF_IMPORT.Entity entity = BLL.FW.BF_IMPORT.Instance.GetEntityByKey<BLL.FW.BF_IMPORT.Entity>(id);
                if (entity == null)
                {
                    throw new Exception("相关导入配置不存在，请联系管理员");
                }
                if (tableName.ToUpper().StartsWith(entity.TABLE_NAME.ToUpper()) == false)
                {
                    throw new Exception("表名错误，请联系管理员");
                }

                string sql = string.Format(
@"UPDATE {0} 
    SET APPROVAL_{1}=?, APPROVAL_{1}_UID=?, APPROVAL_{1}_TIME=?, APPROVAL_{1}_MSG=?
WHERE 1=1 
    AND IMPORT_UID=? 
    AND IMPORT_TIME=?", tableName, level);
                List<Object> param = new List<object>();
                param.Add(isApproval == 1 ? 1 : 0);
                param.Add(BLL.FW.SystemSession.UserID);
                param.Add(DateTime.Now);
                param.Add(message);
                param.Add(importUID);
                param.Add(importTime);

                if (level > 1)
                {
                    sql += " AND APPROVAL_" + (level - 1) + "=1";
                }

                int i = BLL.FW.BF_DATABASE.Instance.ExecuteNonQuery(entity.DB_ID, sql, param);

                result.IsSuccess = true;
                result.Message = "共有" + i + "条记录审批" + (isApproval == 1 ? "通过" : "不通过");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "审批出错：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}