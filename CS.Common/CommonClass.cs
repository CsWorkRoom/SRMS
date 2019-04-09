using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common
{
    /// <summary>
    /// ztree树控件-中间类
    /// </summary>
    public class ZtreeModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// pId
        /// </summary>
        public string pId { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
    }
    /// <summary>
    /// 过滤字段-中间类
    /// </summary>
    public class FilterField
    {
        /// <summary>
        /// 新增字段 {标签表名}_{标签字段名}
        /// </summary>
        public string NewField { get; set; }
        /// <summary>
        /// 查询字段 {schema}.{标签表名_年月后缀}.{标签字段名}
        /// </summary>
        public string SearchField { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DATA_TYPE { get; set; }
        /// <summary>
        /// 英文字段名
        /// </summary>
        public string EN_FIELD_NAME { get; set; }
        /// <summary>
        /// 标签表名
        /// </summary>
        public string LABEL_TABLE { get; set; }
    }
    /// <summary>
    /// 键值对
    /// </summary>
    public class KV
    {
        /// <summary>
        /// 键
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
    /// <summary>
    /// 返回信息类
    /// </summary>
    public class ResultMessage
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回结果详情（一般可以不使用）
        /// </summary>
        public string Result { get; set; }
    }
    /// <summary>
    /// 用户身份信息
    /// </summary>
    public class UIdentity
    {
        /// <summary>
        /// 身份：省=0,1=地市,2=区县,10=客户经理
        /// </summary>
        //public CS.Common.Enums.Identity Identity { get; set; }
        public int Identity { get; set; }
        /// <summary>
        /// 地市编号
        /// </summary>
        public int CITY { get; set; }
        /// <summary>
        /// 区县编号
        /// </summary>
        public int COUNTY { get; set; }
        /// <summary>
        /// 客户经理工号
        /// </summary>
        public string MANAGER_NO { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string UNIT_ID { get; set; }
    }
    public class Field
    {
        public string EN_FIELD_NAME { get; set; }
        public string CN_FIELD_NAME { get; set; }
    }
    public class TableField
    {
        public string field { get; set; }
        public int width { get; set; }
        public string title { get; set; }
        public bool sort { get; set; }
    }
    public class Fee
    {
        /// <summary>
        /// 酬金明细ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int YYYYMM { get; set; }
        /// <summary>
        /// 系统金额
        /// </summary>
        public double SYS_FEE { get; set; }
        /// <summary>
        /// <summary>
        /// 调整金额
        /// </summary>
        public double MODIFY_FEE { get; set; }
        /// 调整说明
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 区县编号
        /// </summary>
        public int COUNTY { get; set; }
        /// <summary>
        /// 区县
        /// </summary>
        public string COUNTY_NAME { get; set; }
    }
}
