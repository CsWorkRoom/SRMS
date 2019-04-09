using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 图形报表筛选配置
    /// </summary>
    public class BF_CHART_REPORT_FILTER : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_CHART_REPORT_FILTER Instance = new BF_CHART_REPORT_FILTER();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_CHART_REPORT_FILTER()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_CHART_REPORT_FILTER";
            this.ItemName = "图形报表筛选配置";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
        }

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
            /// 报表ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "报表ID ")]
            public int REPORT_ID { get; set; }

            /// <summary>
            /// 筛选类型（位置） 对应枚举值：FilterType
            /// </summary>
            [Field(IsNotNull = true, Comment = "筛选类型（位置） 对应枚举值：FilterType ")]
            public Int16 FILTER_TYPE { get; set; }

            /// <summary>
            /// 筛选项名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "筛选项名称")]
            public string FILTER_NAME { get; set; }

            /// <summary>
            /// 筛选字段名
            /// </summary>
            [Field(IsNotNull = true, Comment = "筛选字段名")]
            public string FIELD_NAME { get; set; }

            /// <summary>
            /// 字段类型 对应枚举类型：FieldDataType
            /// </summary>
            [Field(IsNotNull = true, Comment = "字段类型 对应枚举类型：FieldDataType")]
            public Int16 FIELD_DATA_TYPE { get; set; }

            /// <summary>
            /// 输入框名称（默认等于字段名）
            /// </summary>
            [Field(IsNotNull = true, Comment = "输入框名称（默认等于字段名）")]
            public string INPUT_NAME { get; set; }

            /// <summary>
            /// 筛选逻辑 对应枚举值：FilterOperator
            /// </summary>
            [Field(IsNotNull = true, Comment = "筛选逻辑 对应枚举值：FilterOperator")]
            public Int16 FILTER_OPERATOR { get; set; }

            /// <summary>
            /// 默认值
            /// </summary>
            [Field(IsNotNull = false, Comment = "默认值")]
            public string DEFAULT_VALUE { get; set; }

            /// <summary>
            /// 输入框宽度
            /// </summary>
            [Field(IsNotNull = true, Comment = "输入框宽度")]
            public int INPUT_WIDTH { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

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

        /// <summary>
        /// 获取报表的筛选字段列表
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns></returns>
        public List<Entity> GetFilterList(int reportID)
        {
            return GetList<Entity>("REPORT_ID=?", reportID).ToList();
        }

        /// <summary>
        /// 设置筛选字段列表
        /// </summary>
        /// <param name="reportID">报表ID</param>
        /// <param name="filterData">筛选字段列表</param>
        /// <returns></returns>
        public int SetFilteData(int reportID, List<Entity> filterDataList, out int addCount, out int updateCount, out int deleteCount)
        {
            addCount = 0;
            updateCount = 0;
            deleteCount = 0;
            if (reportID < 1 || filterDataList == null || filterDataList.Count < 1)
            {
                return 0;
            }

            Dictionary<string, Entity> dicNew = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicOld = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicInsert = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicUpdate = new Dictionary<string, Entity>();
            List<int> listDelete = new List<int>();
            foreach (Entity entity in filterDataList)
            {
                string fn = entity.FIELD_NAME.ToUpper();
                if (dicNew.ContainsKey(fn) == false)
                {
                    dicNew.Add(fn, entity);
                }
            }
            List<Entity> listOld = GetFilterList(reportID);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.FIELD_NAME.ToUpper();
                    if (dicOld.ContainsKey(fn) == false)
                    {
                        dicOld.Add(fn, entity);
                    }
                    //将删除
                    if (dicNew.ContainsKey(fn) == false)
                    {
                        listDelete.Add(entity.ID);
                    }
                }
            }

            foreach (Entity entity in filterDataList)
            {
                string fn = entity.FIELD_NAME.ToUpper();
                if (dicOld.ContainsKey(fn) == true)
                {
                    if (dicUpdate.ContainsKey(fn) == false)
                    {
                        dicUpdate.Add(fn, entity);
                    }
                }
                else
                {
                    if (dicInsert.ContainsKey(fn) == false)
                    {
                        dicInsert.Add(fn, entity);
                    }
                }

            }

            //删除
            if (listDelete.Count > 0)
            {
                deleteCount = Delete("ID IN (" + string.Join(",", listDelete) + ")");
            }
            //添加
            if (dicInsert.Count > 0)
            {
                List<Entity> listInsert = dicInsert.Values.ToList<Entity>();
                for (int i = 0; i < dicInsert.Count; i++)
                {
                    Entity entity = listInsert[i];
                    entity.REPORT_ID = reportID;
                    entity.IS_ENABLE = 1;
                    entity.CREATE_TIME = DateTime.Now;
                    entity.UPDATE_TIME = DateTime.Now;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.UPDATE_UID = SystemSession.UserID;

                    addCount += Add<Entity>(entity);
                }
            }
            //更新
            if (dicUpdate.Count > 0)
            {
                List<Entity> listUpdate = dicUpdate.Values.ToList<Entity>();
                for (int i = 0; i < listUpdate.Count; i++)
                {
                    Entity entity = listUpdate[i];
                    entity.REPORT_ID = reportID;
                    entity.IS_ENABLE = 1;
                    entity.UPDATE_TIME = DateTime.Now;
                    entity.UPDATE_UID = SystemSession.UserID;

                    updateCount += UpdateByKey<Entity>(entity, entity.ID);
                }
            }

            return addCount + updateCount + deleteCount;
        }
    }
}
