using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 角色
    /// </summary>
    public class BF_ROLE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ROLE Instance = new BF_ROLE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ROLE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ROLE";
            this.ItemName = "角色管理";
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
            /// 角色名称
            /// </summary>
            [Field(IsNotNull = true, Length = 64, IsIndex = true, IsIndexUnique = true, Comment = "角色名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 可访问菜单ID（多个ID以逗号分隔，“ALL”表示所有）
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "可访问菜单ID（多个ID以逗号分隔，“ALL”表示所有）")]
            public string MENU_IDS { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

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

        /// <summary>
        /// 默认记录
        /// </summary>
        /// <returns></returns>
        public override int SetDefaultRecords()
        {
            List<Entity> list = new List<Entity>();
            list.Add(new Entity { ID = 1, NAME = "系统管理", IS_ENABLE = 1, MENU_IDS = "ALL", REMARK = "默认角色，不可修改", CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            DefaultRecords = ToDataTable<Entity>(list);
            return DefaultRecords.Rows.Count;
        }

        /// <summary>
        /// 根据角色获取可访问的菜单
        /// </summary>
        /// <param name="roles">逗号分隔的角色ID</param>
        /// <returns></returns>
        public Dictionary<int, BF_MENU.Entity> GetMenusByRoles(string roles)
        {
            if (string.IsNullOrWhiteSpace(roles) == true)
            {
                return new Dictionary<int, BF_MENU.Entity>();
            }

            List<int> roleList = new List<int>();
            char[] separator = new char[] { ',' };
            int roleID = 0;
            foreach (string rid in roles.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(rid, out roleID))
                {
                    roleList.Add(roleID);
                }
            }
            return GetMenusByRoles(roleList);
        }

        /// <summary>
        /// 根据角色获取可访问的菜单
        /// </summary>
        /// <param name="roleList">角色ID列表</param>
        /// <returns></returns>
        public Dictionary<int, BF_MENU.Entity> GetMenusByRoles(List<int> roleList)
        {
            Dictionary<int, BF_MENU.Entity> dic = new Dictionary<int, BF_MENU.Entity>();
            if (roleList == null || roleList.Count < 1)
            {
                return dic;
            }

            Dictionary<int, BF_MENU.Entity> dicAll = BF_MENU.Instance.GetAllMenus();
            if (dicAll == null || dicAll.Count < 1)
            {
                return dic;
            }

            char[] separator = new char[] { ',' };
            foreach (int roleID in roleList)
            {
                Entity roleEntity = GetEntityByKey<Entity>(roleID);
                if (roleEntity == null || roleEntity.IS_ENABLE != 1 || string.IsNullOrWhiteSpace(roleEntity.MENU_IDS))
                {
                    continue;
                }
                if (roleEntity.MENU_IDS.ToUpper() == "ALL")
                {
                    return dicAll;
                }

                string[] menuIDs = roleEntity.MENU_IDS.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                int mid = 0;
                foreach (string menuID in menuIDs)
                {
                    if (int.TryParse(menuID, out mid))
                    {
                        if (dicAll.ContainsKey(mid) == true && dic.ContainsKey(mid) == false)
                        {
                            dic.Add(mid, dicAll[mid]);
                        }
                    }
                }
            }

            return dic;
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
