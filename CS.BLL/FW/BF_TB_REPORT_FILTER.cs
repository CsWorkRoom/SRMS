using CS.Common.FW;
using CS.Base.Log;
using CS.Library.BaseQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CS.BLL.FW
{
    /// <summary>
    /// 表格报表筛选配置
    /// </summary>
    public class BF_TB_REPORT_FILTER : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_TB_REPORT_FILTER Instance = new BF_TB_REPORT_FILTER();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_TB_REPORT_FILTER()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_TB_REPORT_FILTER";
            this.ItemName = "表格报表筛选配置";
            this.KeyField = "ID";
            this.OrderbyFields = "ORDER_NUM";
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
            /// 筛选项顺序（数字越小排越前面）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "筛选项顺序（数字越小排越前面）")]
            public int ORDER_NUM { get; set; }

            /// <summary>
            /// 输入框类型 对应枚举值：FormQueryType
            /// </summary>
            [Field(IsNotNull = true, Comment = "输入框类型 对应枚举值：FormQueryType")]
            public Int16 FORM_QUERY_TYPE { get; set; }

            /// <summary>
            /// 下拉框配置详情，当字段FORM_QUERY_TYPE为“下拉单选”时有效，格式有二：
            /// 枚举值：1:值1,文字1\r\n值2,文字2\r\n值3,文字3……
            /// 查询表：2:数据库ID,表名,值字段,文字字段,WHERE子句
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "下拉框配置详情，当字段FORM_QUERY_TYPE为“下拉单选”时有效")]
            public string SELECT_DETAIL { get; set; }

            /// <summary>
            /// 默认值
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "默认值")]
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

        #endregion

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
        /// 获取下拉单元框的选项列表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSelectOptions(Entity entity)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            if (entity == null || entity.FORM_QUERY_TYPE != (short)Enums.FormQueryType.下拉单选框 || string.IsNullOrWhiteSpace(entity.SELECT_DETAIL))
            {
                return options;
            }
            string[] selectDetail = entity.SELECT_DETAIL.Split(new char[] { ':' });
            if (selectDetail.Length < 2)
            {
                return options;
            }
            if (selectDetail[0] == Enums.FormSelectType.枚举值.GetHashCode().ToString())
            {
                string[] ops = selectDetail[1].Split(new char[] { '\r', '\n', ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string op in ops)
                {
                    string[] os = op.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    if (os.Length != 2)
                    {
                        continue;
                    }
                    if (options.ContainsKey(os[0]) == false)
                    {
                        string k = BF_FORM.Instance.GetReadParam(os[0]);
                        string v = BF_FORM.Instance.GetReadParam(os[1]);
                        options.Add(k, v);
                    }
                }
            }
            else if (selectDetail[0] == Enums.FormSelectType.表查询.GetHashCode().ToString())
            {
                string[] infos = selectDetail[1].Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                if (infos.Length < 4)
                {
                    return options;
                }
                int dbid = 0;
                if (int.TryParse(infos[0], out dbid) == false)
                {
                    return options;
                }
                string where = "1=1";
                if (infos.Length > 4 && string.IsNullOrWhiteSpace(infos[4]) == false)
                {
                    where = infos[4];
                }

                try
                {
                    int rowsCount = -1;
                    string sql = string.Format("SELECT {0} KKK,{1} VVV FROM {2} WHERE {3}", infos[2], infos[3], infos[1], where);
                    sql = BF_FORM.Instance.GetReadParam(sql);
                    DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(dbid, sql, null, ref rowsCount, 0);
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr[0] == DBNull.Value || dr[1] == DBNull.Value)
                            {
                                continue;
                            }
                            string key = Convert.ToString(dr[0]);
                            if (options.ContainsKey(key) == false)
                            {
                                options.Add(key, Convert.ToString(dr[1]));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    BLog.Write(BLog.LogLevel.WARN, "加载报表" + entity.REPORT_ID + "，筛选字段" + entity.FIELD_NAME + "的下拉筛选项出错：" + ex.ToString());
                }
            }
            else if (selectDetail[0] == Enums.FormSelectType.SQL语句.GetHashCode().ToString())
            {
                string[] infos = selectDetail[1].Split('◎');
                if (infos.Length < 2)
                {
                    return options;
                }
                int dbid = 0;
                if (int.TryParse(infos[0], out dbid) == false)
                {
                    return options;
                }
                string sql = infos[1];//得到SQL语句
                if (string.IsNullOrWhiteSpace(sql) == false)
                {
                    sql = BF_FORM.Instance.GetReadParam(sql);
                    int rowsCount = -1;
                    DataTable dt = BF_DATABASE.Instance.ExecuteSelectSQL(dbid, sql, null, ref rowsCount, 0);
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr[0] == DBNull.Value || dr[1] == DBNull.Value)
                            {
                                continue;
                            }
                            string key = Convert.ToString(dr[0]);
                            if (options.ContainsKey(key) == false)
                            {
                                options.Add(key, Convert.ToString(dr[1]));
                            }
                        }
                    }
                }
            }
            return options;
        }

        #region 得到树
        public static string GetTrea(BF_TB_REPORT_FILTER.Entity fieldData)
        {
            if (fieldData == null)
            {
                return "";
            }
            #region 组装数据集
            DataTable options = BF_TB_REPORT_FILTER.GetSelectTreaOptions(fieldData);
            var obj = new List<object>();
            if (options != null && options.Rows.Count > 0)
            {
                foreach (DataRow dr in options.Rows)
                {
                    obj.Add(
                        new
                        {
                            id = dr["v"] == null || dr["v"].ToString().Trim() == "" ? "" : dr["v"].ToString(),
                            name = dr["k"] == null || dr["k"].ToString().Trim() == "" ? "" : dr["k"].ToString(),
                            pId = dr["p"] == null || dr["p"].ToString().Trim() == "" ? "0" : dr["p"].ToString(),
                        });
                }
            }
            JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings();
            string TreeVal = JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
            #endregion

            //生成字符串
            string opHtml = "<script>$(function(){var zNodes" + fieldData.ID + " = JSON.parse('" + TreeVal + "');$.comboztree('trea" + fieldData.ID + "',{ztreenode: zNodes" + fieldData.ID + "});});</script>";//添加支撑JS
            return opHtml;
        }
        #endregion

        #region 获取下拉树的选项列表
        /// <summary>
        /// 获取下拉树的选项列表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static DataTable GetSelectTreaOptions(Entity entity)
        {
            DataTable options = new DataTable();

            if (entity == null || entity.FORM_QUERY_TYPE != (short)Enums.FormQueryType.下拉树选择 || string.IsNullOrWhiteSpace(entity.SELECT_DETAIL))
            {
                return options;
            }

            string[] selectDetail = entity.SELECT_DETAIL.Split(new char[] { ':' });
            if (selectDetail.Length < 2)
            {
                return options;
            }

            if (selectDetail[0] == Enums.FormSelectType.枚举值.GetHashCode().ToString())
            {
                options.Columns.Add("k");
                options.Columns.Add("v");
                options.Columns.Add("p");
                string[] ops = selectDetail[1].Split(new char[] { '\r', '\n', ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string op in ops)
                {
                    string[] os = op.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    if (os.Length < 3)
                    {
                        continue;
                    }
                    DataRow row = options.NewRow();
                    row["v"] = (string.IsNullOrWhiteSpace(os[0]) ? "" : BF_FORM.Instance.GetReadParam(os[0])); ;
                    row["k"] = (string.IsNullOrWhiteSpace(os[1]) ? "" : BF_FORM.Instance.GetReadParam(os[1]));
                    row["p"] = (string.IsNullOrWhiteSpace(os[2]) ? "0" : BF_FORM.Instance.GetReadParam(os[2]));
                    options.Rows.Add(row);
                }
            }
            else if (selectDetail[0] == Enums.FormSelectType.表查询.GetHashCode().ToString())
            {
                string[] infos = selectDetail[1].Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                if (infos.Length < 5)
                {
                    return options;
                }
                int dbid = 0;
                if (int.TryParse(infos[0], out dbid) == false)
                {
                    return options;
                }
                string where = "1=1";
                if (infos.Length > 5 && string.IsNullOrWhiteSpace(infos[5]) == false)
                {
                    where = infos[5];
                }

                try
                {
                    int rowsCount = -1;
                    string sql = string.Format("SELECT {0} v,{1} k,{2} p FROM {3} WHERE {4}", infos[2], infos[3], infos[4], infos[1], where);
                    sql = BF_FORM.Instance.GetReadParam(sql);
                    options = BF_DATABASE.Instance.ExecuteSelectSQL(dbid, sql, null, ref rowsCount, 0);
                }
                catch (Exception ex)
                {
                    BLog.Write(BLog.LogLevel.WARN, "加载报表" + entity.REPORT_ID + "，筛选字段" + entity.FIELD_NAME + "的下拉筛选项出错：" + ex.ToString());
                }
            }
            else if (selectDetail[0] == Enums.FormSelectType.SQL语句.GetHashCode().ToString())
            {
                string[] infos = selectDetail[1].Split('◎');
                if (infos.Length < 2)
                {
                    return options;
                }
                int dbid = 0;
                if (int.TryParse(infos[0], out dbid) == false)
                {
                    return options;
                }
                string sql = infos[1];//得到SQL语句
                if (string.IsNullOrWhiteSpace(sql) == false)
                {
                    int rowsCount = -1;
                    sql = BF_FORM.Instance.GetReadParam(sql);
                    options = BF_DATABASE.Instance.ExecuteSelectSQL(dbid, sql, null, ref rowsCount, 0);
                }
            }
            return options;
        }
        #endregion

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
            if (filterDataList != null && filterDataList.Count > 0)
            {
                for (int i = 0; i < filterDataList.Count; i++)
                {
                    Entity entity = filterDataList[i];
                    entity.ORDER_NUM = i;
                    string fn = entity.FIELD_NAME.ToUpper();
                    if (dicNew.ContainsKey(fn) == false)
                    {
                        dicNew.Add(fn, entity);
                    }
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
                    if (string.IsNullOrWhiteSpace(entity.SELECT_DETAIL) == false)
                    {
                        entity.SELECT_DETAIL = entity.SELECT_DETAIL.Trim().Replace('，', ',');
                    }
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
                    if (string.IsNullOrWhiteSpace(entity.SELECT_DETAIL) == false)
                    {
                        entity.SELECT_DETAIL = entity.SELECT_DETAIL.Trim().Replace('，', ',');
                    }
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
