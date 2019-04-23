using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.FW
{
    /// <summary>
    /// 流程实例创建传入的实体类
    /// 为了避免与表单控件的名称一致引起的冲突，流程字段的格式小写，并加入特殊符号区分
    /// </summary>
    public class FlowCaseModel
    {
        /// <summary>
        /// 系统_流程ID
        /// </summary>
        public int SysCsFlowID { get; set; }
        /// <summary>
        /// 系统_流程类型ID
        /// </summary>
        public int SysCsFlowTypeID { get; set; }
        /// <summary>
        /// 系统_流程名称
        /// </summary>
        public string SysCsFlowName { get; set; }
        /// <summary>
        /// 系统_主表单表
        /// </summary>
        public string SysCsMainTable { get; set; }
        /// <summary>
        /// 系统_表单页面
        /// </summary>
        public string SysCsMainPage { get; set; }
        /// <summary>
        /// 系统_主页面提交函数
        /// </summary>
        public string SysCsMainFun { get; set; }
        /// <summary>
        /// 系统_流程说明
        /// </summary>
        public string SysCsRemark { get; set; }
        /// <summary>
        /// 系统_流程是否启用
        /// </summary>
        public short sysCsIsEnable { get; set; }
        /// <summary>
        /// 表单的主键ID值
        /// </summary>
        public int sysCsMainTableKey { get; set; }
    }
}