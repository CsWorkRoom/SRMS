using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easyman.Common
{
    public class StringHelper
    {
        /// <summary>
        /// 替换成转义字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LatinReplace(string str)
        {
            List<KV> latinArr = new List<KV>();
            latinArr.Add(new KV { key = ">", value = "&gt;" });
            latinArr.Add(new KV { key = "<", value = "&lt;" });
            latinArr.Add(new KV { key = " ", value = "&nbsp;" });
            latinArr.Add(new KV { key = "\"", value = "&quot;" });
            latinArr.Add(new KV { key = "\'", value = "&#39;" });
            latinArr.Add(new KV { key = "\\", value = "\\\\" });
            latinArr.Add(new KV { key = "\n", value = "\\n" });
            latinArr.Add(new KV { key = "\r", value = "\\r" });
            foreach(var kv in latinArr)
            {
                str = str.Replace(kv.key, kv.value);
            }
            return str;
        }
    }
}
