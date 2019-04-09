using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common
{
    /// <summary>
    /// 公共常量定义
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// 指标统计-聚合函数
        /// </summary>
        public static readonly List<KV> GroupByList = new List<KV>()
        {
            new KV { key="求和(SUM)",value="SUM"},
            new KV { key="计数(COUNT)",value="COUNT"},
            new KV { key="最大值(MAX)",value="MAX"},
            new KV { key="最小值(MIN)",value="MIN"},
            new KV { key="平均值(AVG)",value="AVG"}
        };
    }
}
