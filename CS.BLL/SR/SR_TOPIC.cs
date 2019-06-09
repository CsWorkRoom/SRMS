using CS.Common;
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
    /// 课题管理
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
            public string FILES { get; set; }

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
            /// 立项说明
            /// </summary>
            [Field(IsNotNull = false, DefaultValue = "", Comment = "立项说明")]
            public string APPROVAL_REMARK { get; set; }

            /// <summary>
            /// 立项时间
            /// </summary>
            [Field(IsNotNull = false,  Comment = "立项时间")]
            public DateTime APPROVAL_TIME { get; set; }

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

        //获取全量课题信息(含类型树)
        public List<ZtreeModel> GetTopicTree()
        {
            var obj = new List<ZtreeModel>();
            #region 拼课题类
            DataTable typeDt = SR_TOPIC_TYPE.Instance.GetTable();
            if (typeDt != null && typeDt.Rows.Count > 0)
            {
                foreach (DataRow dr in typeDt.Rows)
                {
                    if (string.IsNullOrWhiteSpace(dr["PARENT_ID"].ToString()))
                    {
                        obj.Add(new ZtreeModel
                        {
                            id = "type_" + dr["ID"],
                            pId = "type_" + (string.IsNullOrWhiteSpace(dr["PARENT_ID"].ToString()) ? "" : dr["PARENT_ID"]),
                            name = dr["NAME"].ToString(),
                            icon = "/Content/zTree/img/1_open.png"
                            //value = Convert.ToInt32(dr["ID"])
                        });
                    }
                    else
                    {
                        obj.Add(new ZtreeModel
                        {
                            id = "type_" + dr["ID"],
                            pId = "type_" + (string.IsNullOrWhiteSpace(dr["PARENT_ID"].ToString()) ? "" : dr["PARENT_ID"]),
                            name = dr["NAME"].ToString(),
                            icon = "/Content/zTree/img/7.png"
                            //value = Convert.ToInt32(dr["ID"])
                        });
                    }
                }
            }
            #endregion

            #region 拼课题
            DataTable dt = Instance.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(
                        new ZtreeModel
                        {
                            id = dr["ID"].ToString().Trim(),
                            pId = (string.IsNullOrWhiteSpace(dr["TOPIC_TYPE_ID"].ToString()) ? "" : "type_" + dr["TOPIC_TYPE_ID"]),
                            name = dr["NAME"].ToString(),
                            icon = "/Content/zTree/img/3.png"//图标
                        });
                }
            }
            #endregion
            return obj;
        }
        //根据用户获得与之相关的课题信息
        public void GetTopicTreeByUser()
        {

        }

    }
}
