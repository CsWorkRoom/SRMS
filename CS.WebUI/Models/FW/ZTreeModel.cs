using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.FW
{
    /// <summary>
    ///属性结构返回数据定义
    /// </summary>
    public class ZTreeModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int pid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool open { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool chkDisabled { get; set; }
        
    }
}