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
    /// 公告管理
    /// </summary>
    public class BF_BULLETIN : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_BULLETIN Instance = new BF_BULLETIN();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_BULLETIN()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_BULLETIN";
            this.ItemName = "公告管理";
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
            /// 标题
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "标题")]
            public string TITLE { get; set; }

            /// <summary>
            /// 摘要
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "摘要")]
            public string SUMMARY { get; set; }

            /// <summary>
            /// 脚本内容
            /// </summary>
            [Field(IsNotNull = true, Length = 4096, Comment = "脚本内容")]
            public string CONTENT { get; set; }


            /// <summary>
            /// 是否启用（未启用的情况下，不可见公告内容）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否启用（未启用的情况下，不可见公告内容）")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 接收公告的部门ID（多个ID逗号分隔，用于编辑时回显,注意：因为公告部门有上下级归属关系，使用递归保存最顶级部门）
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "接收公告的部门ID（多个ID逗号分隔，用于编辑时回显）")]
            public string RECV_DEPT_IDS { get; set; }

            /// <summary>
            /// 接收公告的角色ID（多个ID逗号分隔，用于编辑时回显）
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "接收公告的角色ID（多个ID逗号分隔，用于编辑时回显）")]
            public string RECV_ROLE_IDS { get; set; }

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
        /// 添加一条公告
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="attachmentList"></param>
        /// <returns></returns>
        public int AddBulletin(Entity entity, List<string> attachmentList)
        {
            //1:新增BF_Bulletin表数据
            var bullId = Add(entity, true);
            if (bullId < 1)
            {
                return 0;
            }
            
            //3:根据attachmentList往BF_BULLETIN_ATTACH添加记录
            BLL.FW.BF_BULLETIN_ATTACH.Instance.AddBulletinAttach(bullId, attachmentList);
            return bullId;
        }


        /// <summary>
        /// 修改一条公告
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attachmentList"></param>
        /// <returns></returns>
        public int UpdateBulletin(Entity entity, List<string> attachmentList)
        { 
            //3：修改公告附件表
            BLL.FW.BF_BULLETIN_ATTACH.Instance.UpdateBulletinAttach(entity.ID, attachmentList);
            //1：修改BF_Bulletin表数据
            var bullId = UpdateBullitem(entity);
            if (bullId < 1)
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateBullitem(Entity entity)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("TITLE", entity.TITLE);
            dic.Add("RECV_DEPT_IDS", entity.RECV_DEPT_IDS);
            dic.Add("RECV_ROLE_IDS", entity.RECV_ROLE_IDS);
            dic.Add("SUMMARY", entity.SUMMARY);
            dic.Add("CONTENT", entity.CONTENT);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, entity.ID);
        }

        /// <summary>
        /// 分页查询公告
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="order"></param>
        /// <param name="where"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IList<Entity> GetListPage(int pageSize, int pageIndex, Order order, string where = "", params object[] values)
        {
            IList<Entity> list = base.GetListPage<Entity>(pageSize, pageIndex, order, where, values);

            return list;
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

        /// <summary>
        /// 根据ID删除一条公告，同时删除公告用户表和附件表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteBullIten(int id)
        {
            BLL.FW.BF_BULLETIN_ATTACH.Instance.DleteBulletinAttach(id);
            BLL.FW.BF_BULLETIN_USER.Instance.DeleteBulletinUsers(id);
            return DeleteByKey(id);

        }


        #region 得到有权限阅读的公告集合
        /// <summary>
        /// 得到有权限阅读的公告集合
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="topNum">读取多少条,默认为全部</param>
        /// <returns></returns>
        public List<Entity> GetUserPowerRead(int? userId = null, int topNum = 0)
        {
            List<Entity> returnList = new List<Entity>();
            if (userId == null)
            {
                userId = SystemSession.UserID;
            }

            BF_USER.Entity userData = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(userId);
            int dept = userData.DEPT_ID;//当前用户所在部门ID
            string[] roles = userData.ROLE_IDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//当前用户所属角色

            #region 得到公告集合
            Order order = new Order("UPDATE_TIME", "desc");
            IList<Entity> bulletinList = Instance.GetList<Entity>(order, "IS_ENABLE=1", null);//得到所有可用的公告列表

            order = new Order("id", "DESC");
            IList<BF_DEPARTMENT.Entity> allDepart = BF_DEPARTMENT.Instance.GetList<BF_DEPARTMENT.Entity>(order);//得到所有部门集合

            if (bulletinList == null)
            {
                return null;
            }

            ///开始查找公告集合
            foreach (Entity bulletin in bulletinList)
            {
                if (topNum > 0 && returnList.Count >= topNum)//默认读取所有公告，如果指定了读取条数，就按指定的数目来读
                {
                    return returnList;
                }
                bool isOk = false;//是否找到
                #region 部门查询
                string[] deptList = bulletin.RECV_DEPT_IDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//部门列表ID
                if (deptList.Length > 0)
                {
                    foreach (string deptItem in deptList)
                    {
                        if (dept.ToString() == deptItem)
                        {
                            returnList.Add(bulletin);//记录可看的公告
                            isOk = true;
                            break;
                        }
                    }
                }
                #endregion
                if (isOk)//找到了，就找下一个
                {
                    continue;
                }
                #region 角色查询
                string[] roleList = bulletin.RECV_ROLE_IDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//角色列表
                if (roleList.Length > 0 && roles.Length > 0)
                {
                    foreach (string roleItem in roles)
                    {
                        foreach (var role in roleList)
                        {
                            if (roleItem == role)
                            {
                                returnList.Add(bulletin);//记录可看的公告
                                isOk = true;
                                break;
                            }
                        }
                        if (isOk)//找到了，就找下一个
                        {
                            break;
                        }
                    }
                }
                #endregion
            }
            #endregion

            return returnList;
        }
        #endregion
    }
}
