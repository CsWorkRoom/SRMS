using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.FW
{
    /// <summary>
    /// 枚举定义
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// 系统配置
        /// </summary>
        public enum SystemConfig
        {
            系统名称 = 1,
            会话失效时间 = 2,
            管理员用户名 = 11,
            默认密码 = 12,
            登录失败最大次数 = 13
        }

        /// <summary>
        /// 部门层级
        /// </summary>
        public enum DepartmentLevel
        {
            省公司 = 0,
            地市 = 1,
            区县 = 2,
            分局 = 3,
            渠道 = 4
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum DBType
        {
            Oracle = 1,
            DB2 = 2,
            Vertica = 11,
            GBase = 12
        }

        /// <summary>
        /// 字段类型
        /// </summary>
        public enum FieldDataType
        {
            数值 = 1,
            日期 = 11,
            文本 = 21
        }

        /// <summary>
        /// 报表类型
        /// </summary>
        public enum ReportType
        {
            系统页面 = 0,
            表格报表查询 = 1,
            表格报表导出 = 2,
            图形报表查询 = 3,
            图形报表导出 = 4,
            数据外导 = 5,
            表单添加 = 11,
            表单修改 = 12,
            表单删除 = 13,
            附件上传 = 21,
            附件下载 = 22
        }

        /// <summary>
        /// 筛选类型
        /// </summary>
        public enum FilterType
        {
            默认筛选 = 1,
            高级筛选 = 2
        }

        /// <summary>
        /// 筛选运算
        /// </summary>
        public enum FilterOperator
        {
            等于 = 0,
            小于 = 1,
            小于等于 = 2,
            大于 = 3,
            大于等于 = 4,
            不等于 = 5,
            介于 = 6,
            包含 = 10,
            集合 = 11
        }
        
        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EventType
        {
            表头事件 = 1,
            行事件 = 2
        }

        /// <summary>
        /// 页面显示模式
        /// </summary>
        public enum RequestMode
        {
            当前页弹出框 = 0,
            顶级弹出框 = 1,
            普通提示框 = 11,
            右下提示框 = 12,
            框架新窗口 = 21,
            浏览器新窗口 = 22,
            Ajax异步GET = 31,
            Ajax异步POST = 32,
            JS内部调用 = 41,
            当前页跳转 = 101,
        }

        /// <summary>
        /// 运行状态
        /// </summary>
        public enum RunStatus
        {
            等待 = 0,
            运行 = 1,
            重启 = 2,
            结束 = 10
        }

        /// <summary>
        /// 图表类型（后期或根据图表类型来动态加载js）
        /// </summary>
        public enum ChartType
        {
            饼图 = 1,
            柱状图 = 2,
            曲线图 = 3,
            雷达图 = 4
        }

        #region 数据外导

        /// <summary>
        /// 外导建表模式
        /// </summary>
        public enum CreateTableMode
        {
            指定表 = 1,
            年份后缀 = 2,
            年月后缀 = 3,
            年月日后缀 = 4,
            用户ID后缀 = 5
        }

        #endregion

        #region 表单输入框

        /// <summary>
        /// 表单查询框类型
        /// </summary>
        public enum FormQueryType
        {
            普通文字框 = 1,
            模糊检索框 = 2,
            日期选择框 = 3,
            日期范围框 = 4,
            复选框 = 5,
            下拉单选框 = 6,
            多值查找框 = 7,
            下拉树选择 = 8
        }

        /// <summary>
        /// 表单输入框类型
        /// </summary>
        public enum FormInputType
        {
            普通文字框 = 1,
            日期文字框 = 2,
            多行文字框 = 3,
            下拉单选框 = 4,
            单个复选框 = 5,
            多个复选框 = 6,
            下拉树选择 = 7,
            不显示字段 = 100
        }

        /// <summary>
        /// 表单中Select下拉输入框的数据来源方式
        /// </summary>
        public enum FormSelectType
        {
            不需要此项 = 0,
            枚举值 = 1,
            表查询 = 2,
            SQL语句 = 3
        }

        #endregion

        #region 附件上传

        /// <summary>
        /// 允许上传的文件类型
        /// </summary>
        public enum AcceptUploadFileType
        {
            不限 = 0,
            图片 = 1,
            文本 = 2,
            Word文档 = 11,
            PDF文档 = 12,
            Excel表格 = 13,
            压缩包 = 100
        }

        #endregion

        #region 标签表建表规则
        /// <summary>
        /// 标签表建表方式(后缀)
        /// </summary>
        public enum StorageMode
        {
            无后缀=0,
            日=1,
            月 = 2
        }
        #endregion
        }
}
