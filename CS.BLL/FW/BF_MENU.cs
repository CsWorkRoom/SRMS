using CS.Base.Log;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    public class BF_MENU : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_MENU Instance = new BF_MENU();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_MENU()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_MENU";
            this.ItemName = "系统菜单";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
        }

        #region 实体

        /// <summary>
        /// 实体
        /// </summary>
        public class Entity
        {
            /// <summary>
            /// ID 
            /// </summary>
            [Field(IsPrimaryKey = true, IsAutoIncrement = true, IsNotNull = true, Comment = "ID ")]
            public int ID { get; set; }

            /// <summary>
            /// 上级菜单
            /// </summary>
            [Field(IsNotNull = true, Comment = "上级菜单")]
            public int PID { get; set; }

            /// <summary>
            /// 菜单名称
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "菜单名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 菜单图标库
            /// </summary>
            [Field(IsNotNull = false, Length = 128, Comment = "菜单图标库")]
            public string FONT { get; set; }

            /// <summary>
            /// 菜单图标
            /// </summary>
            [Field(IsNotNull = false, Length = 128, Comment = "菜单图标库")]
            public string ICON { get; set; }

            /// <summary>
            /// 菜单顺序（数字越小排越前面）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "菜单顺序（数字越小排越前面）")]
            public int ORDER_NUM { get; set; }

            /// <summary>
            /// 菜单URL地址（可带必要参数）
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "菜单URL地址（可带必要参数）")]
            public string URL { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 是否为内置菜单
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否为内置菜单")]
            public Int16 IS_DEFAULT { get; set; }

            /// <summary>
            /// 是否显示在导航（菜单）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否显示在导航（菜单）")]
            public Int16 IS_SHOW_NAV { get; set; }

            /// <summary>
            /// 报表类型（对应枚举变量：ReportType）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "报表类型（对应枚举变量：ReportType）")]
            public Int16 REPORT_TYPE { get; set; }

            /// <summary>
            /// 报表ID（为0时表示现有页面）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "报表ID（为0时表示现有页面）")]
            public int REPORT_ID { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "备注")]
            public string REMARK { get; set; }

            /// <summary>
            /// 创建者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "创建者ID")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 修改者者ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "修改者者ID")]
            public int UPDATE_UID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 修改时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "修改时间")]
            public DateTime UPDATE_TIME { get; set; }
        }

        #endregion

        #region 默认记录

        /// <summary>
        /// 默认记录
        /// </summary>
        /// <returns></returns>
        public override int SetDefaultRecords()
        {
            List<Entity> list = new List<Entity>();
            int id = 1;
            int pid = 0;
            //1
            list.Add(new Entity { ID = id++, PID = 0, NAME = "系统管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-yuancheng01-copy", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now, ORDER_NUM = 100 });
            //2
            list.Add(new Entity { ID = id++, PID = 1, NAME = "公共功能", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, REMARK = "多个页面的异步调用会用到这些功能，建议为管理员开启", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //3
            list.Add(new Entity { ID = id++, PID = 1, NAME = "菜单及报表", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-pingtaijicaidan", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //4
            list.Add(new Entity { ID = id++, PID = 1, NAME = "用户及权限", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-yonghuming", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //5
            list.Add(new Entity { ID = id++, PID = 1, NAME = "公告管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-horn", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //6
            list.Add(new Entity { ID = id++, PID = 1, NAME = "口径管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-daima", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //7
            list.Add(new Entity { ID = id++, PID = 1, NAME = "系统配置", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-shezhi1", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //8
            list.Add(new Entity { ID = id++, PID = 1, NAME = "系统日志", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-chaxun4", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //9 测试
            list.Add(new Entity { ID = id++, PID = 0, NAME = "测试功能", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, FONT = "layui-icon", ICON = "layui-icon-help", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now, ORDER_NUM = 200 });

            //公共功能-------2
            pid = 2;
            list.Add(new Entity { ID = id++, PID = pid, NAME = "首页", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/Home/Default", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "加载菜单", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/GetMenus", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "读取数据库表名", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/GetTableList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "读取表字段", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/GetFieldsList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "执行SQL语句", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/ExecuteSelectSQL", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //系统管理-------3
            pid = id++; //菜单管理
            list.Add(new Entity { ID = pid, PID = 3, NAME = "菜单管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfMenu/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "菜单列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出菜单列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑菜单", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "加载报表列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/GetReport", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除菜单", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用菜单", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "禁用菜单", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfMenu/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //表格报表
            list.Add(new Entity { ID = pid, PID = 3, NAME = "表格报表", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfTableReport/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "表格报表列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfTableReport/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出表格报表列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfTableReport/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑表格报表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfTableReport/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "表格报表配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfTableReport/Configure", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除表格报表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfTableReport/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //图形报表
            list.Add(new Entity { ID = pid, PID = 3, NAME = "图形报表", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfChartReport/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "图形报表列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfChartReport/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出图形报表列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfChartReport/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑图形报表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfChartReport/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "验证图形报表SQL", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfChartReport/GetDataBySql", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除图形报表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfChartReport/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //数据外导
            list.Add(new Entity { ID = pid, PID = 3, NAME = "数据外导", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfImport/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "外导列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出外导列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑外导", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除外导", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用外导", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "停用外导", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //表单编辑
            list.Add(new Entity { ID = pid, PID = 3, NAME = "表单编辑", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfForm/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "表单编辑配置列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfForm/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出表单编辑配置列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfImport/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑表单配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfForm/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑表单下拉选项", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfForm/DownModel", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除表单配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfForm/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用表单配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfForm/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "停用表单配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfForm/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //附件管理
            list.Add(new Entity { ID = pid, PID = 3, NAME = "附件管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfFile/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "附件管理配置列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFile/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出附件管理配置列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFile/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑附件管理配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFile/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除附件管理配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFile/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用附件管理配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFile/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "停用附件管理配置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFile/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //用户及权限-----------4
            pid = id++; //组织机构管理
            list.Add(new Entity { ID = pid, PID = 4, NAME = "组织机构管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfDepartment/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "组织机构列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDepartment/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出组织机构列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDepartment/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑组织机构", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDepartment/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除组织机构", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDepartment/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //用户管理
            list.Add(new Entity { ID = pid, PID = 4, NAME = "用户管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfUser/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "用户列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出用户列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑用户", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除用户", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用用户", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "禁用用户", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "重置密码", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/ResetPassword", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "修改密码", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/ChangePassword", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "解锁用户", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfUser/Unlock", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //角色管理
            list.Add(new Entity { ID = pid, PID = 4, NAME = "角色管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfRole/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "角色列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfRole/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出角色列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfRole/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑角色", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfRole/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除角色", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfRole/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用角色", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfRole/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "禁用角色", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfRole/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //公告管理-------5
            pid = id++; //公告管理
            list.Add(new Entity { ID = pid, PID = 5, NAME = "公告管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfBulletin/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "公告列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出公告列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑公告", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除公告附件", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/deleteFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除公告", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用公告", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "禁用公告", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "查看公告", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfBulletin/Show", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            //口径管理-------6
            pid = id++; //脚本类型
            list.Add(new Entity { ID = pid, PID = 6, NAME = "脚本类型", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfScriptType/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "脚本类型列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptType/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出脚本类型列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptType/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑脚本类型", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptType/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除脚本类型", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptType/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //任务
            list.Add(new Entity { ID = pid, PID = 6, NAME = "任务管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfScriptNode/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "任务列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出任务列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "任务详情", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/look", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑任务", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除任务", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启动任务", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/Start", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "停止任务", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptNode/Stop", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //任务组
            list.Add(new Entity { ID = pid, PID = 6, NAME = "任务组管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfScriptFlow/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "任务组列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出任务组列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "加载任务组类型树", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/SciptFlowZtree", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "查看任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/Look", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启用任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/SetEnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "禁用任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/SetUnable", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启动任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/Start", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "停止任务组", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptFlow/Stop", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //日志任务
            list.Add(new Entity { ID = pid, PID = 6, NAME = "运行日志", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfScriptTask/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "日志列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出日志列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "查看任务执行情况", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/look", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "查看任务代码", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/TaskNodeShow", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑并重启任务", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/TaskNodeRestart", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "查看任务组日志", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTaskLog/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "任务组日志列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTaskLog/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "任务日志", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTaskFlowNodeLog/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "任务日志列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTaskFlowNodeLog/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "启动任务实例", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/Start", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "停止任务实例", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfScriptTask/Stop", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            //系统配置-------7
            pid = id++; //数据库管理
            list.Add(new Entity { ID = pid, PID = 7, NAME = "数据库管理", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfDB/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "数据库列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出数据库列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑数据库", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除数据库", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfDB/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //数据库查询
            list.Add(new Entity { ID = pid, PID = 7, NAME = "数据库查询", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfDB/Query", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //字段显示配置
            list.Add(new Entity { ID = pid, PID = 7, NAME = "字段显示配置", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfFieldDisplay/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "字段显示列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFieldDisplay/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出字段显示列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFieldDisplay/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑字段", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFieldDisplay/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "删除字段", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfFieldDisplay/Delete", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //全局设置
            list.Add(new Entity { ID = pid, PID = 7, NAME = "全局设置", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfGlobal/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "编辑全局设置", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfGlobal/Edit", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            //系统日志-------8
            pid = id++; //操作日志
            list.Add(new Entity { ID = pid, PID = 8, NAME = "操作日志", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfOperationLog/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "操作日志列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfOperationLog/GetList", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = pid, NAME = "导出操作日志列表", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfOperationLog/ExportFile", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            pid = id++; //运行日志
            list.Add(new Entity { ID = pid, PID = 8, NAME = "运行日志", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfFileLog/Index", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            //测试功能--------9
            list.Add(new Entity { ID = id++, PID = 9, NAME = "测试-表格报表展示", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfTableReport/Show?id=1", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = 9, NAME = "测试-表格报表导出", IS_ENABLE = 1, IS_SHOW_NAV = 0, IS_DEFAULT = 1, URL = "/AfTableReport/ExportExcel?id=1", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = 9, NAME = "测试-图形报表展示", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfChartReport/Show?id=3", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, PID = 9, NAME = "测试-外导测试", IS_ENABLE = 1, IS_SHOW_NAV = 1, IS_DEFAULT = 1, URL = "/AfImport/Import?id=1", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            DefaultRecords = ToDataTable<Entity>(list);
            return DefaultRecords.Rows.Count;
        }

        #endregion


        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="isActiveOnly">是否仅查询有效的菜单</param>
        /// <returns></returns>
        public Dictionary<int, Entity> GetAllMenus(bool isActiveOnly = true)
        {
            Dictionary<int, Entity> dic = new Dictionary<int, Entity>();
            IList<Entity> list = null;
            if (isActiveOnly == true)
            {
                list = GetList<Entity>("IS_ENABLE=?", 1);
            }
            else
            {
                list = GetList<Entity>();
            }

            if (list != null)
            {
                foreach (Entity entity in list)
                {
                    dic.Add(entity.ID, entity);
                }
            }
            return dic;
        }

        /// <summary>
        /// 判断菜单层级是否小于3级
        /// </summary>
        /// <param name="isActiveOnly">是否仅查询有效的菜单</param>
        /// <returns></returns>
        public bool IsSetMenus(int pid)
        {
            if (pid <= 0)
                return true;

            try
            {
                string where = "id=(select pid from BF_MENU where id=?)";
                DataRow row = Instance.GetRowFields("pid", where, pid);
                if (row == null || row["pid"] == null || string.IsNullOrWhiteSpace(row["pid"].ToString()) || Convert.ToInt32(row["pid"]) <= 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                BLog.Write(BLog.LogLevel.ERROR, "判断菜单层级时程序出现异常，请联系管理员。异常：" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetEnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 1);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetUnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
            
        }
        
    }
}