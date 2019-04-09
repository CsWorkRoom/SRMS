using CS.Library.BaseQuery;
using CS.Library.Cache;
using CS.Common.FW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 通用字段显示配置，主要用来在查询管理中能够快捷地进行字段的中文命名、显示宽度设置等
    /// </summary>
    public class BF_FIELD : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FIELD Instance = new BF_FIELD();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FIELD()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FIELD";
            this.ItemName = "通用字段显示配置，主要用来在查询管理中能够快捷地进行字段的中文命名、显示宽度设置等";
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
            /// ID 自增长，主键
            /// </summary>
            [Field(IsPrimaryKey = true, IsAutoIncrement = true, IsNotNull = true, Comment = "ID 自增长，主键")]
            public int ID { get; set; }

            /// <summary>
            /// 字段英文名 
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "字段英文名 ")]
            public string EN_NAME { get; set; }

            /// <summary>
            /// 字段中文名 
            /// </summary>
            [Field(IsNotNull = true, Length = 128, Comment = "字段中文名 ")]
            public string CN_NAME { get; set; }

            /// <summary>
            /// 字段类型 对应枚举类型：FieldDataType
            /// </summary>
            [Field(IsNotNull = true, Comment = "字段类型 对应枚举类型：FieldDataType")]
            public Int16 FIELD_DATA_TYPE { get; set; }

            /// <summary>
            /// 是否必须 对于业务数据源表，该字段是否为必须（1:必须；0:不必须）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否必须 对于业务数据源表，该字段是否为必须（1:必须；0:不必须）")]
            public Int16 IS_REQUISITE { get; set; }

            /// <summary>
            /// 是否显示 是否在列表中显示（1显示；0不显示）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否显示 是否在列表中显示（1显示；0不显示）")]
            public Int16 IS_SHOW { get; set; }

            /// <summary>
            /// 是否冻结在左侧（1冻结；0不冻结）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否冻结在左侧（1冻结；0不冻结）")]
            public Int16 IS_FIXED { get; set; }

            /// <summary>
            /// 是否排序 是否允许排序（1允许；0不允许）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否排序 是否允许排序（1允许；0不允许）")]
            public Int16 IS_SORT { get; set; }

            /// <summary>
            /// 字段显示长度 在列表中显示的字符数（0表示全显示）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "字段显示长度 在列表中显示的字符数（0表示全显示）")]
            public int SHOW_LENGTH { get; set; }

            /// <summary>
            /// 字段显示宽度 在列表中显示的宽度（0表示自动）,单位：像素
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "80", Comment = "字段显示宽度 在列表中显示的宽度（0表示自动）,单位：像素")]
            public Int16 SHOW_WIDTH { get; set; }

            /// <summary>
            /// 是否为默认记录（默认记录禁止编辑及修改）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否为默认记录 ")]
            public Int16 IS_DEFAULT { get; set; }

            /// <summary>
            /// 备注 
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注 ")]
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

        #region 默认记录
        
        /// <summary>
        /// 设置默认记录
        /// </summary>
        /// <returns></returns>
        public override int SetDefaultRecords()
        {
            List<Entity> list = new List<Entity>();
            int id = 1;
            list.Add(new Entity { ID = id++, EN_NAME = "ID", CN_NAME = "ID", FIELD_DATA_TYPE = (short)Enums.FieldDataType.数值.GetHashCode(), IS_SHOW = 1, SHOW_WIDTH = 60, IS_REQUISITE = 1, IS_DEFAULT = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, EN_NAME = "OP_TIME", CN_NAME = "办理时间", FIELD_DATA_TYPE = (short)Enums.FieldDataType.日期.GetHashCode(), IS_SHOW = 1, SHOW_WIDTH = 120, IS_REQUISITE = 1, IS_DEFAULT = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, EN_NAME = "OP_USER", CN_NAME = "办理工号", FIELD_DATA_TYPE = (short)Enums.FieldDataType.文本.GetHashCode(), IS_SHOW = 1, SHOW_WIDTH = 80, IS_REQUISITE = 1, IS_DEFAULT = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, EN_NAME = "SR_NUM", CN_NAME = "流水号", FIELD_DATA_TYPE = (short)Enums.FieldDataType.文本.GetHashCode(), IS_SHOW = 1, SHOW_WIDTH = 120, IS_REQUISITE = 1, IS_DEFAULT = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });
            list.Add(new Entity { ID = id++, EN_NAME = "PHONE_NUM", CN_NAME = "电话号码", FIELD_DATA_TYPE = (short)Enums.FieldDataType.文本.GetHashCode(), IS_SHOW = 1, SHOW_WIDTH = 80, IS_REQUISITE = 1, IS_DEFAULT = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            DefaultRecords = ToDataTable<Entity>(list);
            return DefaultRecords.Rows.Count;
        }
        #endregion

        /// <summary>
        /// 获取一个默认字段显示信息
        /// </summary>
        /// <param name="enName">字段英文名</param>
        /// <param name="type">数据类型</param>
        /// <param name="reportID">菜单ID</param>
        /// <param name="width">显示宽度</param>
        /// <returns></returns>
        public static Entity Default(string enName, Type type, int reportType = 0, int reportID = 0, int width = 0)
        {
            Entity e = new Entity();
            e.EN_NAME = enName;
            e.CN_NAME = enName;
            e.IS_REQUISITE = 0;
            e.IS_SHOW = 1;
            e.IS_DEFAULT = 0;
            if (type == typeof(int))
            {
                e.FIELD_DATA_TYPE = (Int16)Enums.FieldDataType.数值.GetHashCode();
                e.SHOW_LENGTH = 0;
                e.SHOW_WIDTH = (short)(width > 0 ? width : 80);
            }
            else if (type == typeof(DateTime))
            {
                e.FIELD_DATA_TYPE = (Int16)Enums.FieldDataType.日期.GetHashCode();
                e.SHOW_LENGTH = 0;
                e.SHOW_WIDTH = (short)(width > 0 ? width : 136);
            }
            else
            {
                e.FIELD_DATA_TYPE = (Int16)Enums.FieldDataType.文本.GetHashCode();
                e.SHOW_LENGTH = 10;
                e.SHOW_WIDTH = (short)(width > 0 ? width : 120);
            }

            return e;
        }

        /// <summary>
        /// 获取必须的字段
        /// </summary>
        /// <returns>字典，键：字段英文名，值：字段信息</returns>
        public Dictionary<string, Entity> GetRequisiteFields()
        {
            Dictionary<string, Entity> dic = new Dictionary<string, Entity>();
            IList<Entity> list = GetList<Entity>("IS_REQUISITE=?", 1);
            if (list != null)
            {
                foreach (Entity entity in list)
                {
                    if (dic.ContainsKey(entity.EN_NAME) == false)
                    {
                        dic.Add(entity.EN_NAME, entity);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="reportType">报表类型（通用类型则为0）</param>
        /// <param name="reportID">报表ID（通用字段则为0）</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>如果未配置，则返回默认</returns>
        public Entity GetByFieldName(string fieldName)
        {
            Dictionary<string, Entity> dic = GetDictionary();
            string field = fieldName.ToUpper();
            //通用字段
            if (dic.ContainsKey(field))
            {
                return dic[field];
            }

            //未匹配到
            return Default(fieldName, typeof(string));
        }

        /// <summary>
        /// 从缓存中加载字段信息到字典
        /// </summary>
        /// <returns>字典，键：字段名_菜单ID</returns>
        public Dictionary<string, Entity> GetDictionary()
        {
            string cacheKey = this.TableName + "_GetDictionary";
            object obj = BCache.Get(cacheKey);
            if (obj != null)
            {
                return (Dictionary<string, Entity>)obj;
            }

            Dictionary<string, Entity> dic = new Dictionary<string, Entity>();
            IList<Entity> list = GetList<Entity>();
            if (list == null)
            {
                return dic;
            }
            foreach (Entity entity in list)
            {
                string key = entity.EN_NAME.ToUpper();                
                if (dic.ContainsKey(key) == false)
                {
                    dic.Add(key, entity);
                }
            }

            //加到缓存
            BCache.Add(cacheKey, dic, 10);

            return dic;
        }
    }

}
