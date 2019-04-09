using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.FW
{
    /// <summary>
    /// 用户对象（用于编辑）
    /// </summary>
    public class UserModelsForEdit
    {

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 所属组织机构ID
        /// </summary>
        public int DEPT_ID { get; set; }

        /// <summary>
        /// 所属角色ID，多个角色逗号分隔
        /// </summary>
        public string ROLE_IDS { get; set; }

        /// <summary>
        /// 登录名，不可重复 
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 全名（姓名）
        /// </summary>
        public string FULL_NAME { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PHONE_NUMBER { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string E_MAIL { get; set; }

        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 备用1
        /// </summary>
        public Int16 FLAG_1 { get; set; }

        /// <summary>
        /// 备用2
        /// </summary>
        public Int16 FLAG_2 { get; set; }

        /// <summary>
        /// 备用3
        /// </summary>
        public Int16 FLAG_3 { get; set; }

        /// <summary>
        /// 备用1
        /// </summary>
        public string EXTEND_1 { get; set; }

        /// <summary>
        /// 备用2
        /// </summary>
        public string EXTEND_2 { get; set; }

        /// <summary>
        /// 备用3
        /// </summary>
        public string EXTEND_3 { get; set; }
    }

    /// <summary>
    /// 用户对象（用于列表展示）
    /// </summary>
    public class UserModelsForList
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 所属组织机构名称
        /// </summary>
        public string DEPT_NAME { get; set; }

        /// <summary>
        /// 所属角色ID，多个角色逗号分隔
        /// </summary>
        public string ROLE_NAMES { get; set; }

        /// <summary>
        /// 登录名，不可重复 
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 全名（姓名）
        /// </summary>
        public string FULL_NAME { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LOGIN_COUNT { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IS_ENABLE { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public int IS_LOCKED { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PHONE_NUMBER { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string E_MAIL { get; set; }

        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 备用1
        /// </summary>
        public Int16 FLAG_1 { get; set; }

        /// <summary>
        /// 备用2
        /// </summary>
        public Int16 FLAG_2 { get; set; }

        /// <summary>
        /// 备用3
        /// </summary>
        public Int16 FLAG_3 { get; set; }

        /// <summary>
        /// 备用1
        /// </summary>
        public string EXTEND_1 { get; set; }

        /// <summary>
        /// 备用2
        /// </summary>
        public string EXTEND_2 { get; set; }

        /// <summary>
        /// 备用3
        /// </summary>
        public string EXTEND_3 { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LAST_LOGIN_TIME { get; set; }
    }
}