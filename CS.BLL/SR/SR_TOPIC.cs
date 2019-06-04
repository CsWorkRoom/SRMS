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
    public class SR_TOPIC : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SR_TOPIC Instance = new SR_TOPIC();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SR_TOPIC()
        {
            this.IsAddIntoCache = true;
            this.TableName = "SR_TOPIC";
            this.ItemName = "课题申请";
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
            /// 课题类型
            /// </summary>
            [Field(IsNotNull = true, Length = 256, Comment = "课题类型")]
            public int TOPIC_TYPE_ID { get; set; }

            /// <summary>
            /// 课题名称
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "课题名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            [Field(IsNotNull = true,  Comment = "开始时间")]
            public DateTime START_TIME { get; set; }


            /// <summary>
            /// 结束时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "结束时间")]
            public DateTime END_TIME { get; set; }

            /// <summary>
            /// 创建人
            /// </summary>
            [Field(IsNotNull = true, Comment = "创建人")]
            public int CREATE_USER_ID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW",Comment = "创建时间")]
            public DateTime CREATE_TIME { get; set; }

            /// <summary>
            /// 课题描述
            /// </summary>
            [Field(IsNotNull = false, Comment = "课题描述")]
            public string REMARK { get; set; }

            /// <summary>
            /// 附件IDS
            /// </summary>
            [Field(IsNotNull = false, Comment = "附件IDS")]
            public string ATTACH_IDS { get; set; }

            /// <summary>
            /// 审批状态
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "审批状态")]
            public int FLOW_STATE { get; set; }

            /// <summary>
            /// 是否立项
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "0", Comment = "是否立项")]
            public int IS_APPROVAL { get; set; }
            /// <summary>
            /// 课题总得分
            /// </summary>
            [Field(IsNotNull = false, DefaultValue ="0", Comment = "课题总得分")]
            public double TOTAL_SCORE { get; set; }
            /// <summary>
            /// 课题最高分
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "0", Comment = "课题最高分")]
            public double MAX_SCORE { get; set; }
            /// <summary>
            /// 课题最低分
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "0", Comment = "课题最低分")]
            public double MIN_SCORE { get; set; }
            /// <summary>
            /// 平均得分
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "0", Comment = "平均得分")]
            public double AVG_SCORE { get; set; }
            /// <summary>
            /// 课题状态
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "0", Comment = "课题状态")]
            public int STATE { get; set; }

            #region CS增加字段
            /// <summary>
            /// 课题总预算
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "0", Comment = "课题总预算")]
            public double TOTAL_FEE { get; set; }
            #endregion
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
