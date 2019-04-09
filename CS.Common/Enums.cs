using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common
{
    /// <summary>
    /// 公共枚举定义
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// 任务模式
        /// </summary>
        public enum TaskMode
        {
            自动 = 0,
            手动 = 1
        }

        /// <summary>
        /// 任务运行状态
        /// </summary>
        public enum TaskStatus
        {
            等待 = 0,
            运行 = 1,
            结束 = 2
        }

        /// <summary>
        /// 任务执行结果
        /// </summary>
        public enum TaskResult
        {
            //默认值
            未完成 = 0,
            成功 = 1,
            失败 = 2
        }
        /// <summary>
        /// 人员身份
        /// </summary>
        public enum Identity
        {
            省 = 0,
            地市 = 1,
            区县 = 2,
            //中间可扩展片区等层级
            客户经理 = 10
        }

        /// <summary>
        /// 维护操作状态
        /// 新增 = 1,修改 = 2,删除 = 3,启用 = 4,禁用 = 5
        /// </summary>
        public enum OpStatus
        {
            新增 = 1,
            修改 = 2,
            删除 = 3,
            启用 = 4,
            禁用 = 5
        }
        /// <summary>
        /// 审批状态
        /// </summary>
        public enum AuditStatus
        {
            未审批 = 0,
            通过 = 1,
            退回 = 2
        }
        /// <summary>
        /// 酬金种类
        /// </summary>
        public enum FeeType
        {
            计件 = 1,
            抢盘 = 2,
            获取分享 = 3
        }
    }
}
