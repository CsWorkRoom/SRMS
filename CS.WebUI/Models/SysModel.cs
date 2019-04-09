using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models
{
    #region 附件文件相关类
    /// <summary>
    /// 附件的承载类
    /// </summary>
    public class FilesModel
    {
        /// <summary>
        /// 主键表ID
        /// </summary>
        public int Key_ID { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>
        public string FILE_PATH { get; set; }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string FILE_NAME { get; set; }
        /// <summary>
        /// 附件真实名称
        /// </summary>
        public string REAL_FILE_NAME { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FILE_SIZE { get; set; }
    }
    /// <summary>
    /// 附件控件FILES承载类
    /// </summary>
    public class FileMsg
    {
        /// <summary>
        /// 文件存放目录名
        /// </summary>
        public string PathName { get; set; }
        /// <summary>
        /// 文件集合，以逗号分隔
        /// </summary>
        public string FILES { get; set; }
        /// <summary>
        /// 是否必填项
        /// </summary>
        public bool IsRequire { get; set; }
    }
    #endregion

    #region 树控件相关类
    /// <summary>
    /// 树形控件承载类
    /// </summary>
    public class TreeMsg
    {
        /// <summary>
        /// 控件名
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 控件值
        /// </summary>
        public object ControlValue { get; set; }
        /// <summary>
        /// 下拉项url  "../UpFile/Upload"
        /// </summary>
        public object Url { get; set; }
        /// <summary>
        /// 是否必选项
        /// </summary>
        public bool IsRequire { get; set; }
    }
    #endregion
}