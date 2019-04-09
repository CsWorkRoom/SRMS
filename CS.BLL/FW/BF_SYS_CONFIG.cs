using CS.Common.FW;
using CS.Library.BaseQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class BF_SYS_CONFIG : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_SYS_CONFIG Instance = new BF_SYS_CONFIG();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_SYS_CONFIG()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_SYS_CONFIG";
            this.ItemName = "系统配置";
            this.KeyField = "ID";
            this.OrderbyFields = "ID";
            this.CacheTimeOut = 10;
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
            [Field(IsPrimaryKey = true, IsNotNull = true, Comment = "ID主键")]
            public int ID { get; set; }

            /// <summary>
            /// 配置项值 
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "配置项值 ")]
            public string VALUE { get; set; }

            /// <summary>
            /// 备注 
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "备注 ")]
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

        #region 内部对象

        /// <summary>
        /// 配置项
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 配置项ID
            /// </summary>
            public Enums.SystemConfig ID { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 配置项值类型
            /// </summary>
            public string ValueType { get; set; }
            /// <summary>
            /// 配置项的值
            /// </summary>
            public object Value { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; }
            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime UpdateTime { get; set; }
        }

        #endregion

        #region 配置项

        #region 系统配置相关
        /// <summary>
        /// 系统名称
        /// </summary>
        public static string SystemName
        {
            get
            {
                return GetDTO(Enums.SystemConfig.系统名称).Value.ToString();
            }
        }

        /// <summary>
        /// 会话失效时间（分钟）
        /// </summary>
        public static int SessionTime
        {
            get
            {
                return Convert.ToInt32(GetDTO(Enums.SystemConfig.会话失效时间).Value);
            }
        }

        #endregion

        #region 登录相关

        /// <summary>
        /// 管理员用户名
        /// </summary>
        public static string Administrators
        {
            get
            {
                return GetDTO(Enums.SystemConfig.管理员用户名).Value.ToString();
            }
        }
        /// <summary>
        /// 默认密码
        /// </summary>
        public static string DefaultPassword
        {
            get
            {
                return GetDTO(Enums.SystemConfig.默认密码).Value.ToString();
            }
        }

        /// <summary>
        /// 登录失败最大次数
        /// </summary>
        public static int MaxLoginFailCount
        {
            get
            {
                return Convert.ToInt32(GetDTO(Enums.SystemConfig.登录失败最大次数).Value);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        public static List<DTO> GetAllConfigs()
        {
            List<DTO> list = new List<DTO>();
            list.Add(GetDTO(Enums.SystemConfig.系统名称));
            list.Add(GetDTO(Enums.SystemConfig.会话失效时间));
            list.Add(GetDTO(Enums.SystemConfig.管理员用户名));
            list.Add(GetDTO(Enums.SystemConfig.默认密码));
            list.Add(GetDTO(Enums.SystemConfig.登录失败最大次数));

            return list;
        }

        /// <summary>
        /// 设置配置项的值
        /// </summary>
        /// <param name="dtojson">配置项的值（JSON串）</param>
        /// <returns></returns>
        public static int SetAllConfigs(string dtojson)
        {
            List<DTO> list = JsonConvert.DeserializeObject<List<DTO>>(dtojson);

            return SetAllConfigs(list);
        }

        /// <summary>
        /// 更新所有配置
        /// </summary>
        /// <param name="configList">配置项的值</param>
        /// <returns></returns>
        public static int SetAllConfigs(List<DTO> configList)
        {
            int n = 0;
            if (configList == null)
            {
                return 0;
            }
            string name = string.Empty;

            foreach (DTO dto in configList)
            {
                try
                {
                    n += SetConfig(dto.ID, dto.Value, dto.Remark);
                }
                catch (Exception ex)
                {
                    throw new Exception("更新配置项" + dto.ID.ToString() + "出错：" + ex.Message);
                }
            }
            return n;
        }

        #region 数据库操作

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="config">配置项</param>
        /// <returns></returns>
        private static DTO GetDTO(Enums.SystemConfig config)
        {
            bool isFound = false;
            string value = "";
            string remark = "";
            DateTime update = new DateTime(2018, 6, 6);

            DataTable dt = Instance.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow[] drs = dt.Select("ID=" + config.GetHashCode());
                if (drs != null)
                {
                    isFound = true;
                    value = Convert.ToString(drs[0]["VALUE"]);
                    remark = Convert.ToString(drs[0]["REMARK"]);
                    update = Convert.ToDateTime(drs[0]["UPDATE_TIME"]);
                }
            }

            DTO dto = new DTO();
            dto.ID = config;
            dto.Name = config.ToString();
            dto.ValueType = "文本";
            dto.UpdateTime = update;
            switch (config)
            {
                case Enums.SystemConfig.系统名称:
                    dto.Value = isFound ? value : "第四代PC框架";
                    dto.Remark = isFound ? remark : "";
                    break;
                case Enums.SystemConfig.会话失效时间:
                    dto.ValueType = "整数";
                    dto.Value = isFound ? Convert.ToInt32(value) : 30;
                    dto.Remark = isFound ? remark : "即SESSION失效时间，单位：分钟";
                    break;
                case Enums.SystemConfig.管理员用户名:
                    dto.Value = isFound ? value : "admin";
                    dto.Remark = isFound ? remark : "系统管理员";
                    break;
                case Enums.SystemConfig.默认密码:
                    dto.Value = isFound ? value : "easyman";
                    dto.Remark = isFound ? remark : "新增及重置用户的默认密码";
                    break;
                case Enums.SystemConfig.登录失败最大次数:
                    dto.ValueType = "整数";
                    dto.Value = isFound ? Convert.ToInt32(value) : 5;
                    dto.Remark = isFound ? remark : "登录连续失败达到该次数，账户将被锁定";
                    break;
            }

            return dto;
        }

        /// <summary>
        /// 设置配置项的值
        /// </summary>
        /// <param name="config"></param>
        /// <param name="value"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        private static int SetConfig(Enums.SystemConfig config, object value, string remark = "")
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                throw new Exception("配置项" + config.ToString() + "不可为空");
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();
            object v = 0;
            switch (config)
            {
                case Enums.SystemConfig.系统名称:
                    v = value.ToString();
                    break;
                case Enums.SystemConfig.会话失效时间:
                    v = Math.Max(1, Convert.ToInt32(value));
                    break;
                case Enums.SystemConfig.管理员用户名:
                    v = value.ToString();
                    break;
                case Enums.SystemConfig.默认密码:
                    v = value.ToString();
                    break;
                case Enums.SystemConfig.登录失败最大次数:
                    v = Math.Max(3, Convert.ToInt32(value));
                    break;
            }

            dic.Add("VALUE", v);
            dic.Add("UPDATE_UID", SystemSession.UserID);
            dic.Add("UPDATE_TIME", DateTime.Now);
            if (string.IsNullOrWhiteSpace(remark) == false)
            {
                dic.Add("REMARK", remark);
            }
            int i = Instance.UpdateByKey(dic, config.GetHashCode());
            if (i > 0)
            {
                return i;
            }
            dic.Add("ID", config.GetHashCode());
            dic.Add("CREATE_UID", SystemSession.UserID);
            dic.Add("CREATE_TIME", DateTime.Now);
            return Instance.Add(dic);
        }

        #endregion
    }
}
