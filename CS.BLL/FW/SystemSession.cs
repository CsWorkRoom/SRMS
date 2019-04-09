using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CS.BLL.FW
{
    public class SystemSession
    {
        /// <summary>
        /// 是否为后台服务程序（后台服务启动时，请将“IsWindowsServiceApplication”置为“true”）
        /// </summary>
        public static bool IsWindowsServiceApplication = false;
        private static int _userID = 0;
        private static string _userName = "System";
        private static string _fullUserName = "未知";
        private static int _departmentID = 0;
        private static int _departmentCode = 0;
        private static string _departmentName = string.Empty;
        private static int _departmentLevel = 0;
        #region 备用字段
        private static int _deptFlag = 0;
        private static string _deptExtend1 = "";
        private static string _deptExtend2 = "";
        private static string _deptExtend3 = "";
        private static string _deptExtend4 = "";
        private static string _deptExtend5 = "";
        private static Int16 _userFlag1 = 0;
        private static Int16 _userFlag2 = 0;
        private static Int16 _userFlag3 = 0;
        private static string _userExtend1 = "";
        private static string _userExtend2 = "";
        private static string _userExtend3 = "";
        #endregion
        private static Dictionary<int, FW.BF_MENU.Entity> _dicMenus = new Dictionary<int, BF_MENU.Entity>();
        private static Dictionary<string, int> _dicMenuURLs = new Dictionary<string, int>();

        /// <summary>
        /// 清空
        /// </summary>
        public static void Clear()
        {
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// 写入SESSION
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName">用户登录名</param>
        /// <param name="fullUserName">用户中文名</param>
        /// <param name="departmentID"></param>
        /// <param name="dicMenus"></param>
        public static void WriteSession(int userID, string userName,string fullUserName, int departmentID, Dictionary<int, BF_MENU.Entity> dicMenus)
        {
            Dictionary<string, int> dicMenuURLs = new Dictionary<string, int>();
            BF_USER.Entity userEntity = BF_USER.Instance.GetEntityByKey<BF_USER.Entity>(userID);
            //BF_DEPARTMENT.Entity deptEntity = BF_DEPARTMENT.Instance.GetEntityByKey<BF_DEPARTMENT.Entity>(departmentCode);
            BF_DEPARTMENT.Entity deptEntity = BF_DEPARTMENT.Instance.GetEntityByKey<BF_DEPARTMENT.Entity>(departmentID);

            foreach (var kvp in dicMenus)
            {
                if (string.IsNullOrWhiteSpace(kvp.Value.URL) == true)
                {
                    continue;
                }
                string url = kvp.Value.URL.ToLower();
                if (dicMenuURLs.ContainsKey(url) == false)
                {
                    dicMenuURLs.Add(url, kvp.Key);
                }
            }

            if (IsWindowsServiceApplication == true)
            {
                _userID = userID;
                _userName = userName;
                _fullUserName = fullUserName;
                _departmentID = departmentID;
                _dicMenus = dicMenus;
                _dicMenuURLs = dicMenuURLs;
                if (deptEntity != null)
                {
                    _departmentID = deptEntity.ID;
                    _departmentCode = deptEntity.DEPT_CODE;
                    _departmentName = deptEntity.NAME;
                    _departmentLevel = deptEntity.DEPT_LEVEL;
                    _deptFlag = deptEntity.DEPT_FLAG;
                    _deptExtend1 = deptEntity.EXTEND_1;
                    _deptExtend2 = deptEntity.EXTEND_2;
                    _deptExtend3 = deptEntity.EXTEND_3;
                    _deptExtend4 = deptEntity.EXTEND_4;
                    _deptExtend5 = deptEntity.EXTEND_5;
                }
                else
                {
                    _departmentID = -1;
                    _departmentLevel = -1;
                }

                _userFlag1 = userEntity.FLAG_1;
                _userFlag2 = userEntity.FLAG_2;
                _userFlag3 = userEntity.FLAG_3;
                _userExtend1 = userEntity.EXTEND_1;
                _userExtend2 = userEntity.EXTEND_2;
                _userExtend3 = userEntity.EXTEND_3;
            }
            else
            {
                HttpContext.Current.Session["BEF_USER_ID"] = userID;
                HttpContext.Current.Session["BEF_USER_NAME"] = userName;
                HttpContext.Current.Session["BEF_FULL_USER_NAME"] = fullUserName;
                HttpContext.Current.Session["BEF_DEPARTMENT_ID"] = departmentID;

                HttpContext.Current.Session["BEF_MENUS"] = dicMenus;
                HttpContext.Current.Session["BEF_MENU_URLS"] = dicMenuURLs;
                //扩展
                HttpContext.Current.Session["BEF_USER_FLAG_1"] = userEntity.FLAG_1;
                HttpContext.Current.Session["BEF_USER_FLAG_2"] = userEntity.FLAG_2;
                HttpContext.Current.Session["BEF_USER_FLAG_3"] = userEntity.FLAG_3;
                HttpContext.Current.Session["BEF_USER_EXTEND_1"] = userEntity.EXTEND_1;
                HttpContext.Current.Session["BEF_USER_EXTEND_2"] = userEntity.EXTEND_2;
                HttpContext.Current.Session["BEF_USER_EXTEND_3"] = userEntity.EXTEND_3;

                if (deptEntity != null)
                {
                    HttpContext.Current.Session["BEF_DEPARTMENT_ID"] = deptEntity.ID;
                    HttpContext.Current.Session["BEF_DEPARTMENT_CODE"] = deptEntity.DEPT_CODE;
                    HttpContext.Current.Session["BEF_DEPARTMENT_NAME"] = deptEntity.NAME;
                    HttpContext.Current.Session["BEF_DEPARTMENT_LEVEL"] = deptEntity.DEPT_LEVEL;

                    HttpContext.Current.Session["BEF_DEPT_FLAG"] = deptEntity.DEPT_FLAG;
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_1"] = deptEntity.EXTEND_1;
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_2"] = deptEntity.EXTEND_2;
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_3"] = deptEntity.EXTEND_3;
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_4"] = deptEntity.EXTEND_4;
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_5"] = deptEntity.EXTEND_5;
                }
                else
                {
                    HttpContext.Current.Session["BEF_DEPARTMENT_ID"] = -1;
                    HttpContext.Current.Session["BEF_DEPARTMENT_CODE"] = 0;
                    HttpContext.Current.Session["BEF_DEPARTMENT_NAME"] = "";
                    HttpContext.Current.Session["BEF_DEPARTMENT_LEVEL"] = -1;

                    HttpContext.Current.Session["BEF_DEPT_FLAG"] = 0;
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_1"] = "";
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_2"] = "";
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_3"] = "";
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_4"] = "";
                    HttpContext.Current.Session["BEF_DEPT_EXTEND_5"] = "";
                }

                //SESSION失效时间
                HttpContext.Current.Session.Timeout = BF_SYS_CONFIG.SessionTime;
            }
        }

        /// <summary>
        /// 获取当前登录用户的ID（未登录及后台服务则默认为0）
        /// </summary>
        public static int UserID
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userID;
                }

                object s = HttpContext.Current.Session["BEF_USER_ID"];
                if (s == null)
                {
                    return -1;
                    throw new Exception("未登录");
                }

                int userID = 0;
                if (int.TryParse(s.ToString(), out userID))
                {
                    return userID;
                }

                throw new Exception("未正确登录");
            }
        }

        /// <summary>
        /// 当前登录用户工号
        /// </summary>
        public static string UserName
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userName;
                }

                object s = HttpContext.Current.Session["BEF_USER_NAME"];
                if (s == null)
                {
                    throw new Exception("未登录");
                }

                return s.ToString();
            }
        }

        /// <summary>
        /// 当前登录用户名
        /// </summary>
        public static string FullUserName
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _fullUserName;
                }

                object s = HttpContext.Current.Session["BEF_FULL_USER_NAME"];
                if (s == null)
                {
                    throw new Exception("未登录");
                }

                return s.ToString();
            }
        }

        /// <summary>
        /// 用户所属部门ID
        /// </summary>
        public static int UserDepartmentID
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _departmentID;
                }

                object s = HttpContext.Current.Session["BEF_DEPARTMENT_ID"];
                if (s == null)
                {
                    throw new Exception("未登录");
                }

                int departmentID = 0;
                if (int.TryParse(s.ToString(), out departmentID))
                {
                    return departmentID;
                }

                throw new Exception("未正确登录");
            }
        }

        /// <summary>
        /// 组织机构编码
        /// </summary>
        public static int UserDepartmentCode
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _departmentCode;
                }

                object s = HttpContext.Current.Session["BEF_DEPARTMENT_CODE"];
                if (s == null)
                {
                    throw new Exception("未登录");
                }

                int departmentCode = 0;
                if (int.TryParse(s.ToString(), out departmentCode))
                {
                    return departmentCode;
                }

                throw new Exception("未正确登录");
            }
        }

        /// <summary>
        /// 组织机构名称
        /// </summary>
        public static string UserDepartmentName
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _departmentName;
                }

                object s = HttpContext.Current.Session["BEF_DEPARTMENT_NAME"];
                return s.ToString();
            }
        }

        /// <summary>
        /// 用户所属部门层级
        /// </summary>
        public static int UserDepartmentLevel
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _departmentLevel;
                }

                object s = HttpContext.Current.Session["BEF_DEPARTMENT_LEVEL"];
                if (s == null)
                {
                    throw new Exception("未登录");
                }

                int departmentLevel = 0;
                if (int.TryParse(s.ToString(), out departmentLevel))
                {
                    return departmentLevel;
                }

                throw new Exception("未正确登录");
            }
        }

        #region 备用（扩展）字段
        /// <summary>
        /// 用户标志1
        /// </summary>
        public static Int16 UserFlag1
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userFlag1;
                }

                object s = HttpContext.Current.Session["BEF_USER_FLAG_1"];
                if (s == null)
                {
                    return -1;
                }
                return Convert.ToInt16(s);
            }
        }

        /// <summary>
        /// 用户标志2
        /// </summary>
        public static Int16 UserFlag2
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userFlag2;
                }

                object s = HttpContext.Current.Session["BEF_USER_FLAG_2"];
                if (s == null)
                {
                    return -1;
                }
                return Convert.ToInt16(s);
            }
        }
        /// <summary>
        /// 用户标志3
        /// </summary>
        public static Int16 UserFlag3
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userFlag3;
                }

                object s = HttpContext.Current.Session["BEF_USER_FLAG_3"];
                if (s == null)
                {
                    return -1;
                }
                return Convert.ToInt16(s);
            }
        }
        /// <summary>
        /// 用户扩展1
        /// </summary>
        public static string UserExtend1
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userExtend1;
                }

                object s = HttpContext.Current.Session["BEF_USER_EXTEND_1"];
                return Convert.ToString(s);
            }
        }

        /// <summary>
        /// 用户扩展2
        /// </summary>
        public static string UserExtend2
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userExtend2;
                }

                object s = HttpContext.Current.Session["BEF_USER_EXTEND_2"];
                return Convert.ToString(s);
            }
        }
        /// <summary>
        /// 用户扩展3
        /// </summary>
        public static string UserExtend3
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _userExtend3;
                }

                object s = HttpContext.Current.Session["BEF_USER_EXTEND_3"];
                return Convert.ToString(s);
            }
        }
        /// <summary>
        /// 部门标志
        /// </summary>
        public static int DeptFlag
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _deptFlag;
                }

                object s = HttpContext.Current.Session["BEF_DEPT_FLAG"];
                if (s == null)
                {
                    return -1;
                }
                return Convert.ToInt32(s);
            }
        }

        /// <summary>
        /// 部门扩展1
        /// </summary>
        public static string DeptExtend1
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _deptExtend1;
                }

                object s = HttpContext.Current.Session["BEF_DEPT_EXTEND_1"];
                return Convert.ToString(s);
            }
        }
        /// <summary>
        /// 部门扩展2
        /// </summary>
        public static string DeptExtend2
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _deptExtend2;
                }

                object s = HttpContext.Current.Session["BEF_DEPT_EXTEND_2"];
                return Convert.ToString(s);
            }
        }
        /// <summary>
        /// 部门扩展3
        /// </summary>
        public static string DeptExtend3
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _deptExtend3;
                }

                object s = HttpContext.Current.Session["BEF_DEPT_EXTEND_3"];
                return Convert.ToString(s);
            }
        }
        /// <summary>
        /// 部门扩展4
        /// </summary>
        public static string DeptExtend4
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _deptExtend5;
                }

                object s = HttpContext.Current.Session["BEF_DEPT_EXTEND_5"];
                return Convert.ToString(s);
            }
        }
        /// <summary>
        /// 部门扩展5
        /// </summary>
        public static string DeptExtend5
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _deptExtend5;
                }

                object s = HttpContext.Current.Session["BEF_DEPT_EXTEND_5"];
                return Convert.ToString(s);
            }
        }

        #endregion

        #region 个性化内容

        /// <summary>
        /// 获取用户所属地市编码
        /// </summary>
        public static int City
        {
            get
            {
                int code = UserDepartmentCode;
                return BF_DEPARTMENT.Instance.GetParentCode(code, Common.FW.Enums.DepartmentLevel.地市.GetHashCode());
            }
        }

        /// <summary>
        /// 获取用户所属区县编码
        /// </summary>
        public static int County
        {
            get
            {
                int code = UserDepartmentCode;
                return BF_DEPARTMENT.Instance.GetParentCode(code, Common.FW.Enums.DepartmentLevel.区县.GetHashCode());
            }
        }

        #endregion

        /// <summary>
        /// 用户可访问菜单（键：菜单ID，值：菜单对象实体）
        /// </summary>
        public static Dictionary<int, FW.BF_MENU.Entity> UserMenus
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _dicMenus;
                }

                object obj = HttpContext.Current.Session["BEF_MENUS"];
                if (obj == null)
                {
                    throw new Exception("未登录");
                }

                try
                {
                    return (Dictionary<int, FW.BF_MENU.Entity>)obj;
                }
                catch
                {
                    throw new Exception("未正确登录");
                }
            }
        }

        /// <summary>
        /// 用户可访问菜单URL（键：URL，值：菜单ID）
        /// </summary>
        public static Dictionary<string, int> UserMenuURLs
        {
            get
            {
                if (IsWindowsServiceApplication == true)
                {
                    return _dicMenuURLs;
                }

                object obj = HttpContext.Current.Session["BEF_MENU_URLS"];
                if (obj == null)
                {
                    throw new Exception("未登录");
                }

                try
                {
                    return (Dictionary<string, int>)obj;
                }
                catch
                {
                    throw new Exception("未正确登录");
                }
            }
        }

        /// <summary>
        /// 替换参数
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string TransParams(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            if (content.IndexOf("@{") < 0)
            {
                return content;
            }

            string s = content.Replace("@{USER_ID}", SystemSession.UserID.ToString());
            s = s.Replace("@{USER_NAME}", SystemSession.UserName);
            s = s.Replace("@{DEPARTMENT_ID}", SystemSession.UserDepartmentID.ToString());
            s = s.Replace("@{DEPARTMENT_CODE}", SystemSession.UserDepartmentCode.ToString());
            s = s.Replace("@{DEPARTMENT_NAME}", SystemSession.UserDepartmentName);
            s = s.Replace("@{DEPARTMENT_LEVEL}", SystemSession.UserDepartmentLevel.ToString());
            s = s.Replace("@{ALLROLE}", BF_USER.Instance.GetUserRole(SystemSession.UserID));
            s = s.Replace("@{DATETIME}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            s = s.Replace("@{DATE}", DateTime.Now.ToString("yyyy-MM-dd"));
            //扩展参数
            s = s.Replace("@{BEF_USER_FLAG_1}", UserFlag1.ToString());
            s = s.Replace("@{BEF_USER_FLAG_2}", UserFlag2.ToString());
            s = s.Replace("@{BEF_USER_FLAG_3}", UserFlag3.ToString());
            s = s.Replace("@{BEF_USER_EXTEND_1}", UserExtend1);
            s = s.Replace("@{BEF_USER_EXTEND_2}", UserExtend2);
            s = s.Replace("@{BEF_USER_EXTEND_3}", UserExtend3);
            s = s.Replace("@{BEF_DEPT_FLAG}", DeptFlag.ToString());
            s = s.Replace("@{BEF_DEPT_EXTEND_1}", DeptExtend1);
            s = s.Replace("@{BEF_DEPT_EXTEND_2}", DeptExtend2);
            s = s.Replace("@{BEF_DEPT_EXTEND_3}", DeptExtend3);
            s = s.Replace("@{BEF_DEPT_EXTEND_4}", DeptExtend4);
            s = s.Replace("@{BEF_DEPT_EXTEND_5}", DeptExtend5);
            #region 计件个性参数
            s = s.Replace("@{CITY}", SystemSession.City.ToString());
            s = s.Replace("@{COUNTY}", SystemSession.County.ToString());
            #endregion
            return s;
        }
    }
}