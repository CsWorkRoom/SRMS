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

namespace CS.WebUI.Controllers.FW
{
    /// <summary>
    /// 流程管理
    /// </summary>
    public class AfFlowController : ABaseController
    {
        public string Modular = "流程管理";

        #region 配置

        /// <summary>
        /// 编辑及新增
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            BLL.FW.BF_FLOW.Entity flow = new BF_FLOW.Entity();//初始化
            flow.IS_ENABLE = 1;
            if (id > 0)//编辑
            {
                flow = BLL.FW.BF_FLOW.Instance.GetEntityByKey<BLL.FW.BF_FLOW.Entity>(id);
                if (flow == null)
                {
                    return ShowAlert("流程配置项不存在");
                }
                //流程节点信息
                var flowNodeList = BLL.FW.BF_FLOW_NODE.Instance.GetList<BLL.FW.BF_FLOW_NODE.Entity>("FLOW_ID=?", id);
                //流程节点关系
                var flowNodeJoinList = BLL.FW.BF_FLOW_NODE_JOIN.Instance.GetList<BLL.FW.BF_FLOW_NODE_JOIN.Entity>("FLOW_ID=?", id);

                ViewBag.FlowNodes = SerializeObject(flowNodeList);
                ViewBag.FlowNodeJoins = SerializeObject(flowNodeJoinList);
            }

            #region 流程类型下拉项
            DataTable dt = BLL.FW.BF_FLOW_TYPE.Instance.GetTable();
            var obj = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                obj.Add(
                    new
                    {
                        id = Convert.ToInt32(dr["ID"]),
                        pId = string.IsNullOrWhiteSpace(dr["PARENT_ID"].ToString()) ? "" : dr["PARENT_ID"],
                        name = dr["NAME"].ToString(),
                        value = Convert.ToInt32(dr["ID"])
                    });
            }

            ViewBag.FlowTypes = SerializeObject(obj);
            #endregion

            #region 表单表下拉
            var tbList = BF_DATABASE.Instance.GetAllTableList(0);
            ViewBag.TbList = tbList;
            #endregion

            return View(flow);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(BF_FLOW.Entity entity, FormCollection collection)
        {
            JsonResultData result = new JsonResultData();
            int i = 0;
            try
            {
                if (entity.ID < 0)
                {
                    result.Message = "流程配置项不存在，不可编辑";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                entity.UPDATE_UID = SystemSession.UserID;
                entity.UPDATE_TIME = DateTime.Now;

                #region 获得流程节点+流程节点关系
                string flowNodeJson = collection["FlowNodes"];
                string flowNodeJoinJson = collection["FlowNodeJoins"];

                var flowNodeList = DeserializeObject<List<BF_FLOW_NODE.Entity>>(flowNodeJson);
                var flowNodeJoinList = DeserializeObject<List<FlowNodeJoin>>(flowNodeJoinJson);
                #endregion

                if (entity.ID == 0)
                {
                    //entity.IS_ENABLE = 1;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.CREATE_TIME = DateTime.Now;
                    i = BF_FLOW.Instance.Add(entity, true);
                    entity.ID = i;
                }
                else
                {
                    #region 校验是否有正在运行中的流程实例（暂未实现）
                    #endregion

                    i = BF_FLOW.Instance.UpdateByKey(entity, entity.ID);//修改
                }
                #region 保存流程节点
                int addCountNode = 0;
                int updateCountNode = 0;
                int deleteCountNode = 0;
                //保存节点信息
                BF_FLOW_NODE.Instance.SaveFlowNodes(entity.ID, flowNodeList, out addCountNode, out updateCountNode, out deleteCountNode);
                #endregion

                #region 保存流程关系
                //获得最新的流程节点信息
                var NowFlowNodeList = BF_FLOW_NODE.Instance.GetDicFlowNodeList(entity.ID);

                #region 给FROM_NODE_ID、TO_NODE_ID赋值
                if (flowNodeJoinList != null && flowNodeJoinList.Count > 0)
                {
                    foreach (var join in flowNodeJoinList)
                    {
                        if (NowFlowNodeList.ContainsKey(join.FROM_NODE_NAME))
                        {
                            join.FROM_NODE_ID = NowFlowNodeList[join.FROM_NODE_NAME].ID;
                        }
                        if (NowFlowNodeList.ContainsKey(join.TO_NODE_NAME))
                        {
                            join.TO_NODE_ID = NowFlowNodeList[join.TO_NODE_NAME].ID;
                        }
                    }
                }
                #endregion
                List<BF_FLOW_NODE_JOIN.Entity> NodeJoinList = Common.Fun.ClassListToCopy<FlowNodeJoin, BF_FLOW_NODE_JOIN.Entity>(flowNodeJoinList).ToList();

                int addCountJoin = 0;
                int updateCountJoin = 0;
                int deleteCountJoin = 0;
                //保存节点关系信息
                BF_FLOW_NODE_JOIN.Instance.SaveFlowNodeJoins(entity.ID, NodeJoinList, out addCountJoin, out updateCountJoin, out deleteCountJoin);
                #endregion

                if (i < 1)
                {
                    result.Message = "出现了未知错误";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.IsSuccess = true;
                result.Message = "保存成功";
                WriteOperationLog(BLog.LogLevel.INFO, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的流程成功！");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                BLog.Write(BLog.LogLevel.WARN, "流程保存出错：" + ex.ToString());

                WriteOperationLog(BLog.LogLevel.ERROR, true, Modular, (entity.ID > 0 ? "修改" : "添加"), "", (entity.ID > 0 ? "修改" : "添加") + "ID为" + entity.ID + "的配置失败：" + ex.Message);
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
            result.IsSuccess = true;
            result.Message = "启用成功";
            try
            {
                int i = BF_FLOW.Instance.SetEnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "启用失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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
            result.IsSuccess = true;
            result.Message = "禁用成功";
            try
            {
                int i = BF_FLOW.Instance.SetUnable(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "禁用失败：" + ex.Message;
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
            result.IsSuccess = true;
            result.Message = "删除成功";

            try
            {
                #region 删除节点及其节点关系（暂未实现）

                #endregion

                int i = BF_FLOW.Instance.DeleteByKey(id);
                if (i < 1)
                {
                    throw new Exception("未知原因");
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "删除失败：" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 校验是否有正在运行中的流程实例（暂未实现）
        #endregion


        #region 获取所有的 角色/人员/部门信息
        public ActionResult GetEntityTree(string type)
        {
            var entitys = new List<ZTreeModel>();
            if (type == "user")
            {
                var entityUsers = BLL.FW.BF_USER.Instance.GetList<BLL.FW.BF_USER.Entity>("IS_ENABLE=1 AND IS_LOCKED=0").OrderBy(e => e.FULL_NAME);
                foreach (var user in entityUsers)
                {
                    ZTreeModel model = new ZTreeModel();
                    model.id = user.ID;
                    model.pid = 0;
                    model.name = user.NAME;
                    model.open = false;
                    model.chkDisabled = false;
                    entitys.Add(model);
                }
            }
            else if (type == "role")
            {
                var entityRoles = BLL.FW.BF_ROLE.Instance.GetList<BLL.FW.BF_ROLE.Entity>("IS_ENABLE=1").OrderBy(e => e.NAME);
                foreach (var role in entityRoles)
                {
                    ZTreeModel model = new ZTreeModel();
                    model.id = role.ID;
                    model.pid = 0;
                    model.name = role.NAME;
                    model.open = false;
                    model.chkDisabled = false;
                    entitys.Add(model);
                }
            }
            else
            {
                var entityRoles = BLL.FW.BF_DEPARTMENT.Instance.GetList<BLL.FW.BF_DEPARTMENT.Entity>().OrderBy(e => e.NAME);
                foreach (var role in entityRoles)
                {
                    ZTreeModel model = new ZTreeModel();
                    model.id = role.ID;
                    model.pid = 0;
                    model.name = role.NAME;
                    model.open = false;
                    model.chkDisabled = false;
                    entitys.Add(model);
                }
            }

            var obj = new { code = 0, data = entitys };
            return Json(obj);
        }
        #endregion
    }
}