using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.FW
{
    /// <summary>
    /// 公告对象（用于编辑）
    /// </summary>
    public class BulletinModelsForEdit
    {

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 接收公告的部门ID（多个ID逗号分隔，用于编辑时回显,注意：因为公告部门有上下级归属关系，使用递归保存最顶级部门）
        /// </summary>
        public string RECV_DEPT_IDS { get; set; }

        /// <summary>
        /// 所属角色ID，多个角色逗号分隔
        /// </summary>
        public string RECV_ROLE_IDS { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string TITLE { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string SUMMARY { get; set; }

        /// <summary>
        /// 脚本内容
        /// </summary>
        public string CONTENT { get; set; }
        /// <summary>
        /// 脚本内容
        /// </summary>
        public string FileId { get; set; }
    }

    /// <summary>
    /// 用户对象（用于列表展示）
    /// </summary>
    public class BulletinModelsForList
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 接收公告的部门ID（多个ID逗号分隔，用于编辑时回显,注意：因为公告部门有上下级归属关系，使用递归保存最顶级部门）
        /// </summary>
        public string RECV_DEPT_IDS { get; set; }

        /// <summary>
        /// 所属角色ID，多个角色逗号分隔
        /// </summary>
        public string RECV_ROLE_IDS { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string TITLE { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string SUMMARY { get; set; }

        /// <summary>
        /// 脚本内容
        /// </summary>
        public string CONTENT { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IS_ENABLE { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CREATE_UID { get; set; }
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CREATE_UID_NAME { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CREATE_TIME { get; set; }
    }


    public class BulletinFileModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 公告ID
        /// </summary>
        public int BULL_ID { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>
        public string FILE_PATH { get; set; }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string FILE_NAME { get; set; }

    }
}