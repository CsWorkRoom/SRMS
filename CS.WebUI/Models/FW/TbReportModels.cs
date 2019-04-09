using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.FW
{
    /// <summary>
    /// 表格报表字段信息
    /// </summary>
    public class TbrShowFieldModel
    {
        /// <summary>
        /// 字段英文名
        /// </summary>
        public string EN_NAME { get; set; }

        /// <summary>
        /// 字段中文名
        /// </summary>
        public string CN_NAME { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public Int16 FIELD_DATA_TYPE { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public Int16 IS_SHOW { get; set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        public Int16 IS_FIXED { get; set; }

        /// <summary>
        /// 是否排序
        /// </summary>
        public Int16 IS_SORT { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        public Int16 IS_ENCRYPT { get; set; }

        /// <summary>
        /// 显示宽度
        /// </summary>
        public Int16 SHOW_WIDTH { get; set; }

    }
}