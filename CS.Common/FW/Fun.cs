using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Configuration;
using System.IO;

namespace CS.Common
{
    public class Fun
    {
        public static string GetExceptionMessage(Exception e)
        {
            IList<string> message = new List<string>();
            message.Add(e.Message);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                message.Add(e.Message);
            }
            return message[message.Count - 1];
        }


        /// <summary>
        /// 将DataTable数据转换成实体类
        /// 本功能主要用于外导EXCEL
        /// </summary>
        /// <typeparam name="T">MVC的实体类</typeparam>
        /// <param name="dt">输入的DataTable</param>
        /// <returns>实体类的LIST</returns>
        public static IList<T> TableToClass<T>(DataTable dt) where T : new()
        {
            IList<T> outList = new List<T>();
            T tmpClass = new T();
            if (dt.Rows.Count == 0) return outList;
            PropertyInfo[] proInfoArr = tmpClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, string> dic_all = new Dictionary<string, string>();
            foreach (var t in proInfoArr)
            {
                var attrsPro = t.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrsPro.Length > 0)
                {
                    DisplayAttribute pro = (DisplayAttribute)attrsPro[0];
                    dic_all.Add(pro.Name, t.Name);
                }
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dic_all.Where(x => x.Key == dt.Columns[i].Caption).Count() != 0)
                {
                    dic.Add(dt.Columns[i].Caption, dic_all[dt.Columns[i].Caption]);
                }
                else if (dic_all.Where(x => x.Value == dt.Columns[i].Caption).Count() != 0)
                {
                    dic.Add(dt.Columns[i].Caption, dt.Columns[i].Caption);
                }
            }

            var rowTmp = dt.Rows[0];

            for (int a = 0; a < dt.Rows.Count; a++)
            {
                var row = dt.Rows[a];
                tmpClass = new T();
                proInfoArr = tmpClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
                foreach (var t in dic)
                {
                    PropertyInfo outproInfo = tmpClass.GetType().GetProperty(t.Value);
                    if (outproInfo != null)
                    {
                        if (row[t.Key] == null || string.IsNullOrEmpty(row[t.Key].ToString()))
                        {
                            row[t.Key] = rowTmp[t.Key];
                        }
                        outproInfo.SetValue(tmpClass, Convert.ChangeType(row[t.Key], outproInfo.PropertyType, CultureInfo.CurrentCulture), null);
                    }
                }
                outList.Add(tmpClass);
                rowTmp = dt.Rows[a];
            }
            return outList;
        }

        /// <summary>
        /// 复制一个类里所有属性到别一个类
        /// </summary>
        /// <typeparam name="inT">传入的类型</typeparam>
        /// <typeparam name="outT">输出类型</typeparam>
        /// <param name="inClass">传入的类</param>
        /// <param name="outClass">输入的类</param>
        /// <param name="allPar">排除的赋值属性</param>
        /// <returns>复制结果的类</returns>
        public static outT ClassToCopy<inT, outT>(inT inClass, outT outClass, IList<string> allPar = null)
        {
            if (inClass == null) return outClass;
            PropertyInfo[] proInfoArr = inClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);//得到该类的所有公共属性
            for (int a = 0; a < proInfoArr.Length; a++)
            {
                if (allPar != null && allPar.Contains(proInfoArr[a].Name)) continue;
                PropertyInfo outproInfo = outClass.GetType().GetProperty(proInfoArr[a].Name);
                if (outproInfo != null)
                {
                    var type = outproInfo.PropertyType;
                    object objValue = proInfoArr[a].GetValue(inClass, null);
                    if (null != objValue)
                    {
                        if (!outproInfo.PropertyType.IsGenericType)
                        {
                            objValue = Convert.ChangeType(objValue, outproInfo.PropertyType);
                        }
                        else
                        {
                            Type genericTypeDefinition = outproInfo.PropertyType.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Nullable<>))
                            {
                                objValue = Convert.ChangeType(objValue, Nullable.GetUnderlyingType(outproInfo.PropertyType));
                            }
                        }
                    }
                    outproInfo.SetValue(outClass, objValue, null);
                }
            }
            return outClass;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="inT"></typeparam>
        /// <typeparam name="outT"></typeparam>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static outT ClassToCopy<inT, outT>(inT inClass) where outT : new()
        {
            if (inClass == null) return default(outT);
            outT outClass = new outT();
            return ClassToCopy(inClass, outClass);
        }
        /// <summary>
        /// 转换IList内的所有属性
        /// </summary>
        /// <typeparam name="inT"></typeparam>
        /// <typeparam name="outT"></typeparam>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static IList<outT> ClassListToCopy<inT, outT>(IList<inT> inClass) where outT : new()
        {
            if (inClass == null) return default(IList<outT>);
            IList<outT> outClass = new List<outT>();
            for (int a = 0; a < inClass.Count; a++)
            {
                outClass.Add(ClassToCopy<inT, outT>(inClass[a]));
            }
            return outClass;
        }

        /// <summary>
        /// 把枚举转换成下接列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IList<SelectItem> GetSelectListFromEnum<T>()
        {
            IList<SelectItem> selectItemList = new List<SelectItem>();
            foreach (T mode in Enum.GetValues(typeof(T)))
            {
                selectItemList.Add(new SelectItem { Text = mode.ToString(), Value = (mode.GetHashCode()).ToString() });
            }

            return selectItemList;
        }
        /// <summary>
        /// 计算MD5
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static string FilesMakeMd5(byte[] fileContent)
        {
            if (fileContent == null) return null;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fileContent);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));
            return sb.ToString();
        }


        public static string GetUpMoth(string yyyyMM, int numMoth)
        {
            DateTime dt = new DateTime(Convert.ToInt32(yyyyMM.Substring(0, 4)), Convert.ToInt32(yyyyMM.Substring(4, 2)), 1);
            return dt.AddMonths(numMoth).ToString("yyyyMM");
        }


        public static DataTable JsonToDataTable(string strJson)
        {
            //取出表名  
            Regex rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名  
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));

            //获取数据  
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');

                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        DataColumn dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        dc.ColumnName = strCell[0].ToString().Replace("\"", "");
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split(':')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            return tb;
        }
        public static string EvalExpression(string formula)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Result").Expression = formula;
            dt.Rows.Add(dt.NewRow());

            var result = dt.Rows[0]["Result"];
            return result.ToString();
        }
        /// <summary>
        /// 获取类的备注信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClassDescription<T>()
        {

            object[] peroperties = typeof(T).GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (peroperties.Length > 0)
            {
                return ((DescriptionAttribute)peroperties[0]).Description;
            }
            return "";
        }
        /// <summary>
        /// 获取类的属性说明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClassProperDescription<T>(string properName)
        {
            PropertyInfo[] peroperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in peroperties)
            {
                if (property.Name == properName)
                {
                    object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (objs.Length > 0)
                    {
                        return ((DescriptionAttribute)objs[0]).Description;
                    }
                }
            }
            return "";
        }

        public static string GetClassProperType<T>(string properName)
        {
            PropertyInfo[] peroperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in peroperties)
            {
                if (property.Name == properName)
                {
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    return propertyType.Name;
                }
            }
            return "";
        }

        /// <summary>
        /// 产生一组不重复的随机数
        /// </summary>
        public static IList<int> RandomIntList(int MinValue, int MaxValue, int Length)
        {
            if (MaxValue - MinValue + 1 < Length)
            {
                return null;
            }
            Random R = new Random();
            Int32 SuiJi = 0;
            IList<int> suijisuzu = new List<int>();
            int min = MinValue - 1;
            int max = MaxValue + 1;
            for (int i = 0; i < Length; i++)
            {
                suijisuzu.Add(min);
            }
            for (int i = 0; i < Length; i++)
            {
                while (true)
                {
                    SuiJi = R.Next(min, max);
                    if (!suijisuzu.Contains(SuiJi))
                    {
                        suijisuzu[i] = SuiJi;
                        break;
                    }
                }
            }
            return suijisuzu;
        }

        #region 通过两个点的经纬度计算距离

        private const double EARTH_RADIUS = 6378.137; //地球半径
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 通过两个点的经纬度计算距离(千米)
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        /// <summary>
        /// 通过两个点的经纬度计算距离(千米)
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(string lat1, string lng1, string lat2, string lng2)
        {
            return Fun.GetDistance(Convert.ToDouble(lat1), Convert.ToDouble(lng1), Convert.ToDouble(lat2), Convert.ToDouble(lng2));
        }

        #endregion


        public static string ReplaceDataTable(string inStr, DataTable table)
        {
            var sql = inStr;
            if (string.IsNullOrEmpty(sql))
            {
                return "";
            }
            int iRow = 0;
            string sColumns = "";
            int nowPlace = 0;
            {
                int s = sql.IndexOf("@{DataTable(");
                nowPlace = s;
                if (s > -1)
                {
                    int e = sql.IndexOf(")}", s);
                    while (e > s && s > -1)
                    {
                        s = s + 12;
                        string str = "";
                        if (e > s)
                        {
                            str = sql.Substring(s, e - s);
                            iRow = Convert.ToInt32(str.Split(',')[0].Trim());
                            sColumns = str.Split(',')[1].Trim();
                        }
                        sql = sql.Replace("@{DataTable(" + str + ")}", table.Rows[iRow][sColumns].ToString());

                        s = sql.IndexOf("@{DataTable(");
                        if (nowPlace == s)
                        {
                            return "";
                        }
                        nowPlace = s;
                        if (s > -1)
                        {
                            e = sql.IndexOf(")}", s);
                        }
                    }
                }
            }

            return sql;
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHTML(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return Htmlstring;
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring=HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;

        }

        /// <summary>  
        /// dataTable转换成Json格式(包含表名)
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2Json(DataTable dt, bool hasTbName = false)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            if (hasTbName)
            {
                jsonBuilder.Append("{\"");
                jsonBuilder.Append(dt.TableName);
                jsonBuilder.Append("\":");
            }

            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");

            if (hasTbName)
            {
                jsonBuilder.Append("}");
            }
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// datatable转list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> DatatableToList<T>(DataTable dt) where T : new()
        {
            IList<T> list = new List<T>();
            Type type = typeof(T);
            string propertysName = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo temp in propertys)
                {
                    propertysName = temp.Name;
                    if (dt.Columns.Contains(propertysName))
                    {
                        if (!temp.CanWrite) { continue; }//是否能设置属性值
                        var value = dr[propertysName];//table列名
                        if (value != DBNull.Value)
                        {
                            temp.SetValue(t, value);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        #region 得到随机数
        /// <summary>
        /// 得到随机数
        /// </summary>
        /// <param name="minValue">起始值</param>
        /// <param name="maxValue">结束值</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            if (minValue > maxValue)
                return null;

            if (minValue == maxValue)
            {
                int[] intResult = { minValue };
                return intResult;
            }
            int intTepm = maxValue - minValue;
            if (intTepm <= count) //如果提取数大于起止值差，就提前结果反回
            {
                int[] intResult = new int[intTepm];
                for (int i = 0; i < intTepm; i++)
                {
                    intResult[i] = minValue;
                    minValue += 1;
                }
                return intResult;
            }
            Random rnd = new Random();
            int length = maxValue - minValue + 1;
            byte[] keys = new byte[length];
            rnd.NextBytes(keys);
            int[] items = new int[length];
            for (int i = 0; i < length; i++)
            {
                items[i] = i + minValue;
            }
            Array.Sort(keys, items);
            int[] result = new int[count];
            Array.Copy(items, result, count);
            return result;
        }
        #endregion

        #region 获取中英文混排字符串的实际长度
        /// <summary>
        /// 获取中英文混排字符串的实际长度(字节数)
        /// </summary>
        /// <param name="str">要获取长度的字符串</param>
        /// <returns>字符串的实际长度值（字节数）</returns>
        public static int GetStringLength(string str)
        {
            if (str.Equals(string.Empty))
                return 0;
            int strlen = 0;
            ASCIIEncoding strData = new ASCIIEncoding();
            //将字符串转换为ASCII编码的字节数字
            byte[] strBytes = strData.GetBytes(str);
            for (int i = 0; i <= strBytes.Length - 1; i++)
            {
                if (strBytes[i] == 63)  //中文都将编码为ASCII编码63,即"?"号
                    strlen++;
                strlen++;
            }
            return strlen;
        }
        #endregion
    }
    //
    // 摘要:
    //     表示 System.Web.Mvc.SelectList 类的实例中的选定项。
    public class SelectItem
    {
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}