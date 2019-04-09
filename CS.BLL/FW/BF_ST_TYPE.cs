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
    /// 脚本流类型
    /// </summary>
    public class BF_ST_TYPE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_ST_TYPE Instance = new BF_ST_TYPE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_ST_TYPE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_ST_TYPE";
            this.ItemName = "脚本类型";
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
            /// 类型名称
            /// </summary>
            [Field(IsNotNull = true, Length = 256, IsIndex = true, IsIndexUnique = true, Comment = "类型名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 上级类型ID
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "上级类型ID")]
            public int PID { get; set; }

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

        #region 查询列表
        public DataTable GetDataTable(int limit, int page, ref int count, int pid, string name, string orderByField, string orderByType)
        {
            string strWhere = "1=1";
            List<object> param = new List<object>();
            if (pid > 0)
            {
                strWhere += " AND st.PID=?";
                param.Add(pid);
            }
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                strWhere += " AND st.NAME LIKE '%" + name.Replace('\'', ' ') + "%'";
            }

            string strSql = "select st.id, st.NAME,(select stp.name from BF_ST_TYPE stp where stp.id=st.PID)PNAME,st.REMARK,(select full_name from BF_USER where st.CREATE_UID=ID)createName,st.CREATE_TIME,(select full_name from BF_USER where st.UPDATE_UID=ID)updateName,st.UPDATE_TIME from BF_ST_TYPE st where " + strWhere;
            //添加排序
            if (string.IsNullOrWhiteSpace(orderByField) == false)
                strSql += " ORDER BY " + orderByField + " " + (string.IsNullOrWhiteSpace(orderByType) == false ? orderByType : "");

            using (BDBHelper dbHelper = new BDBHelper())
            {
                if (limit == 0 && page == 0)
                {
                    return dbHelper.ExecuteDataTableParams(strSql, param);//不分页查询所有
                }
                //算总记录
                if (count == 0)
                {
                    string sqlCount = string.Format("SELECT COUNT(*) FROM ({0})", strSql);
                    count = dbHelper.ExecuteScalarIntParams(sqlCount, param);
                }
                return dbHelper.ExecuteDataTablePageParams(strSql, limit, page, param);
            }
        }
        #endregion

        #region 查询子分类

        /// <summary>
        /// 查询分类及所有子分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> GetAllChildren(int id)
        {
            List<int> list = new List<int>();
            list.Add(id);

            DataTable dt = GetTable();
            if (dt == null || dt.Rows.Count < 1)
            {
                return list;
            }

            GetChildren(dt, id, ref list);
            return list;
        }

        /// <summary>
        /// 递归查询子分类
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pid"></param>
        /// <param name="list"></param>
        public void GetChildren(DataTable dt, int pid, ref List<int> list)
        {
            DataRow[] drs = dt.Select("PID=" + pid);
            if (drs != null && drs.Length > 0)
            {
                foreach (DataRow dr in drs)
                {
                    int id = Convert.ToInt32(dr["ID"]);
                    list.Add(id);

                    //递归
                    GetChildren(dt, id, ref list);
                }
            }
        }

        #endregion
    }
}
