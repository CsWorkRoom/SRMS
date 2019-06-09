using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.SR
{
    /// <summary>
    /// 科教学科
    /// </summary>
    public class SR_SUBJECT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_SUBJECT Instance = new SR_SUBJECT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_SUBJECT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_SUBJECT";
            this.ItemName = "学科项目";
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
            /// 父节点
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "父节点")]
            public int PARENT_ID { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注")]
            public string REAMRK { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }


            /// <summary>
            /// 更新时间
            /// </summary>
            [Field(IsNotNull = true, Comment = "更新时间")]
            public DateTime UPDATE_TIME { get; set; }

            /// <summary>
            /// 创建人
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建人")]
            public int CREATE_UID { get; set; }

            /// <summary>
            /// 修改人
            /// </summary>
            [Field(IsNotNull = true, Comment = "修改人")]
            public int UPDATE_UID { get; set; }

        }

        #endregion
        #region 方法
        /// <summary>
        /// 获得所有学科列表
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public List<Entity> GetSubjectList()
        {
            return GetList<Entity>().ToList();
        }
        #endregion
    }

    public class SubjectDto 
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<SubjectDto> children { get; set; }
    }
}
