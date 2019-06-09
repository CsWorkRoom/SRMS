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
    /// 评分项规则
    /// </summary>
    public class SR_SUB_ITEM : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_SUB_ITEM Instance = new SR_SUB_ITEM();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_SUB_ITEM()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_SUB_ITEM";
            this.ItemName = "课题评分项";
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
            [Field(IsNotNull = false, Length = 256, Comment = "所属学科")]
            public int SUBJECT_ID { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 规则
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "规则")]
            public string RULE { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 1024, Comment = "备注")]
            public string REMARK { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }



            /// <summary>
            /// 创建人
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建人")]
            public int CREATE_USER_ID { get; set; }


        }

        #endregion
        #region 方法
        /// <summary>
        /// 获得所有学科列表
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public List<Entity> GetSubItemList(string subid)
        {
            return GetList<Entity>("SUBJECT_ID=?", subid).ToList();
        }
        #endregion
    }
}
