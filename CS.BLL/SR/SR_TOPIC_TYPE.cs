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
    /// 公告管理
    /// </summary>
    public class SR_TOPIC_TYPE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC_TYPE Instance = new SR_TOPIC_TYPE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC_TYPE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC_TYPE";
            this.ItemName = "课题类型";
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

        /// <summary>
        /// 添加一条课题信息
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
        /// 分页查询课题信息
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

      

    }
}
