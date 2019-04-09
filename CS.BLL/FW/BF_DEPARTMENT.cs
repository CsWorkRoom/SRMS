using CS.Base.DBHelper;
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
    /// 部门
    /// </summary>
    public class BF_DEPARTMENT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_DEPARTMENT Instance = new BF_DEPARTMENT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_DEPARTMENT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_DEPARTMENT";
            this.ItemName = "部门管理";
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
            /// 上级部门编码
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "上级部门编码")]
            public int P_CODE { get; set; }

            /// <summary>
            /// 部门编码
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", IsIndex = true, IsIndexUnique = true, Comment = "部门编码")]
            public int DEPT_CODE { get; set; }

            /// <summary>
            /// 部门名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "部门名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 层级
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "层级")]
            public int DEPT_LEVEL { get; set; }

            /// <summary>
            /// 备用标志（结合项目自身情况使用）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "备用标志（结合项目自身情况使用）")]
            public int DEPT_FLAG { get; set; }

            /// <summary>
            /// 备用1
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用1")]
            public string EXTEND_1 { get; set; }

            /// <summary>
            /// 备用2
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用2")]
            public string EXTEND_2 { get; set; }

            /// <summary>
            /// 备用3
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用3")]
            public string EXTEND_3 { get; set; }

            /// <summary>
            /// 备用4
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用4")]
            public string EXTEND_4 { get; set; }

            /// <summary>
            /// 备用5
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用5")]
            public string EXTEND_5 { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注")]
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
            list.Add(new Entity { ID = 1, P_CODE = 0, NAME = "XXX公司", DEPT_LEVEL = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            DefaultRecords = ToDataTable<Entity>(list);
            return DefaultRecords.Rows.Count;
        }

        /// <summary>
        /// 获取某一层级父节点的编码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetParentCode(int code, int level)
        {
            int pcode = 0;
            Entity entity = GetEntity<Entity>("DEPT_CODE=?", code);
            if (entity == null)
            {
                return -1;
            }
            if (entity.DEPT_LEVEL == level)
            {
                return entity.DEPT_CODE;
            }
            int lv = entity.DEPT_LEVEL;

            while (lv > level)
            {
                pcode = entity.P_CODE;
                if (lv - 1 == level)
                {
                    break;
                }
                entity = GetEntity<Entity>("DEPT_CODE=?", pcode);
                lv = entity.DEPT_LEVEL;
            }

            return pcode;
        }

        #region 递归修改子节点(已经由前端指定层级）
        /// <summary>
        /// 更新所有节点的层级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int RefreshNodesLevel(int id, int level)
        {
            int updateCount = 0;
            using (BDBHelper dbHelper = new BDBHelper())
            {
                RefreshSubNodesLevel(id, level, dbHelper, ref updateCount);//进入递归
            }
            return updateCount;
        }

        /// <summary>
        /// 递归更新子节点的层级
        /// </summary>
        /// <param name="pCode"></param>
        /// <param name="level"></param>
        /// <param name="dbHelper"></param>
        /// <param name="updateCount"></param>
        public void RefreshSubNodesLevel(int pCode, int level, BDBHelper dbHelper, ref int updateCount)
        {
            level++;
            string sqlSelect = "SELECT CODE FROM BF_DEPARTMENT WHERE P_CODE=?";
            DataTable dt = dbHelper.ExecuteDataTableParams(sqlSelect, pCode);
            if (dt == null || dt.Rows.Count < 1)
            {
                return;
            }
            string sqlUpdate = "UPDATE BF_DEPARTMENT SET DEPT_LEVEL=" + level + " WHERE P_CODE=?";
            int i = dbHelper.ExecuteNonQueryParams(sqlUpdate, pCode);

            if (i > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int code = Convert.ToInt32(dr["CODE"]);
                    RefreshSubNodesLevel(code, level, dbHelper, ref updateCount);
                }
            }
            updateCount += i;
        }


        #endregion


        #region 获取取子集
        /// <summary>
        /// 递归获取所有子集
        /// </summary>
        /// <param name="nodes">父级节点</param>
        /// <param name="allDepart">所有部门</param>
        /// <returns></returns>
        public List<Entity> GetAllSubsetNode(string[] nodes, IList<BF_DEPARTMENT.Entity> allDepart)
        {
            List<BF_DEPARTMENT.Entity> resutl = new List<BF_DEPARTMENT.Entity>();
            foreach (string nodeId in nodes)
            {
                IList<Entity> resutlList = allDepart.Where(a => a.ID == int.Parse(nodeId)).ToList<Entity>();
                if (resutlList.Count > 0)
                {
                    resutl.Add(resutlList[0]);
                    GetSubsetNode(allDepart, resutlList[0].ID, ref resutl);
                }
            }
            return resutl;
        }

        /// <summary>
        /// 递归获取所有子集
        /// </summary>
        /// <param name="allDepart">所有部门</param>
        /// <param name="pid">父级ID</param>
        /// <param name="resutl"></param>
        private void GetSubsetNode(IList<Entity> allDepart, int pid, ref List<Entity> resutl)
        {
            IList<Entity> resutlList = allDepart.Where(a => a.P_CODE == pid).ToList<Entity>();
            if (resutlList == null || resutlList.Count <= 0)
            {
                return;
            }
            foreach (Entity item in resutlList)
            {
                resutl.Add(item);
                GetSubsetNode(allDepart, item.ID, ref resutl);
            }
        }
        #endregion

    }
}
