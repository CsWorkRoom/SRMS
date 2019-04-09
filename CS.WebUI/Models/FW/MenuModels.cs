using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.FW
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuModels
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 上级菜单ID
        /// </summary>
        public int pid { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 图标库
        /// </summary>
        public string font { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool spread = false;
        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuModels> children { get; set; }
    }
}