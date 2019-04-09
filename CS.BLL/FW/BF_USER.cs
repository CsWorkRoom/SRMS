using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS.Library.BaseQuery;
using System.Data;
using CS.Base.Encrypt;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using CS.Base.DBHelper;

namespace CS.BLL.FW
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class BF_USER : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_USER Instance = new BF_USER();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_USER()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_USER";
            this.ItemName = "用户管理";
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
            /// 所属组织机构编码，关联部门表的DEPT_CODE字段
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "所属组织机构ID")]
            public int DEPT_ID { get; set; }

            /// <summary>
            /// 所属角色ID，多个角色逗号分隔
            /// </summary>
            [Field(IsNotNull = true, Length = 512, Comment = "所属角色ID，多个角色逗号分隔")]
            public string ROLE_IDS { get; set; }

            /// <summary>
            /// 登录名，不可重复 
            /// </summary>
            [Field(IsNotNull = true, Length = 128, IsIndex = true, IsIndexUnique = true, Comment = "登录名，不可重复")]
            public string NAME { get; set; }

            /// <summary>
            /// 全名（姓名）
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "全名（姓名）")]
            public string FULL_NAME { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            [Field(IsNotNull = true, Length = 32, Comment = "密码")]
            public string PASSWORD { get; set; }

            /// <summary>
            /// 登录次数
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "登录次数")]
            public int LOGIN_COUNT { get; set; }

            /// <summary>
            /// 登录失败次数
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "登录失败次数")]
            public int LOGIN_FAIL_COUNT { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否启用")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 是否锁定
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否锁定")]
            public Int16 IS_LOCKED { get; set; }

            /// <summary>
            /// 电话号码
            /// </summary>
            [Field(IsNotNull = false, Length = 32, Comment = "电话号码")]
            public string PHONE_NUMBER { get; set; }
            /// <summary>
            /// 邮箱地址
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "邮箱地址")]
            public string E_MAIL { get; set; }

            /// <summary>
            /// QQ号码
            /// </summary>
            [Field(IsNotNull = false, Length = 32, Comment = "QQ号码")]
            public string QQ { get; set; }

            /// <summary>
            /// 备用1
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "备用1")]
            public Int16 FLAG_1 { get; set; }

            /// <summary>
            /// 备用2
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "备用2")]
            public Int16 FLAG_2 { get; set; }

            /// <summary>
            /// 备用3
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "备用3")]
            public Int16 FLAG_3 { get; set; }

            /// <summary>
            /// 备用1
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用1")]
            public string EXTEND_1 { get; set; }

            /// <summary>
            /// 备用2
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用2")]
            public string EXTEND_2 { get; set; }

            /// <summary>
            /// 备用3
            /// </summary>
            [Field(IsNotNull = false, Length = 256, Comment = "备用3")]
            public string EXTEND_3 { get; set; }

            /// <summary>
            /// 最后登录时间
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "NOW", Comment = "最后登录时间")]
            public DateTime LAST_LOGIN_TIME { get; set; }

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

        #region 图形验证码

        /// 生成随机的字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetValidateCode(int length)
        {
            string chars = "0123456789ABCDEabcdefghigklmnopqrFGHIGKLMNOPQRSTUVWXYZstuvwxyz";
            chars = "ABCDEabcdefghgkmnopqrFGHGKMNOPQRSTUVWXYZstuvwxyz";
            int len = chars.Length;
            string code = "";
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < length; i++)
            {
                code += chars[rand.Next(len)];
            }
            return code;
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public static byte[] GetValidateImage(string validateCode)
        {
            using (Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 27))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    Random random = new Random(DateTime.Now.Millisecond);
                    g.Clear(Color.White);
                    //画图片的干扰线
                    for (int i = 0; i < 25; i++)
                    {
                        int x1 = random.Next(image.Width);
                        int x2 = random.Next(image.Width);
                        int y1 = random.Next(image.Height);
                        int y2 = random.Next(image.Height);
                        g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
                    }
                    Font font = new Font("Arial", 14, (FontStyle.Bold | FontStyle.Italic));
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                    g.DrawString(validateCode, font, brush, 5, 3);

                    //画图片的前景干扰线
                    for (int i = 0; i < 100; i++)
                    {
                        int x = random.Next(image.Width);
                        int y = random.Next(image.Height);
                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }
                    //画图片的边框线
                    g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                    //保存图片数据
                    MemoryStream stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Jpeg);

                    //输出图片流
                    return stream.ToArray();
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取账号的默认密码
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private string GetDefaultPassword(string username)
        {
            return BMD5.Encrypt(BF_SYS_CONFIG.DefaultPassword, username);
        }

        /// <summary>
        /// 默认记录
        /// </summary>
        /// <returns></returns>
        public override int SetDefaultRecords()
        {
            string username = BF_SYS_CONFIG.Administrators;
            string password = GetDefaultPassword(username);

            List<Entity> list = new List<Entity>();
            list.Add(new Entity { ID = 1, NAME = username, FULL_NAME = "管理员", ROLE_IDS = "1", PASSWORD = password, IS_ENABLE = 1, DEPT_ID = 1, CREATE_TIME = DateTime.Now, UPDATE_TIME = DateTime.Now });

            DefaultRecords = ToDataTable<Entity>(list);
            return DefaultRecords.Rows.Count;
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fullName"></param>
        /// <param name="deptID"></param>
        /// <param name="roleIds"></param>
        private void CheckInput(int id, string name, string fullName, int deptID, string roleIds)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("登录名不可为空");
            }
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new Exception("用户姓名不可为空");
            }
            if (deptID < 1)
            {
                throw new Exception("请选择所属部门");
            }
            if (string.IsNullOrWhiteSpace(roleIds))
            {
                throw new Exception("请选择所属角色（至少一个角色）");
            }
            if (IsDuplicate(id, "NAME", name))
            {
                throw new Exception("登录名 " + name + " 已经存在");
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullName"></param>
        /// <param name="deptID"></param>
        /// <param name="roleIds"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="qq"></param>
        /// <param name="flag1"></param>
        /// <param name="flag2"></param>
        /// <param name="flag3"></param>
        /// <param name="extend1"></param>
        /// <param name="extend2"></param>
        /// <param name="extend3"></param>
        /// <returns></returns>
        public int Add(string name, string fullName, int deptID, List<int> roleIds, string phoneNumber, string email, string qq, Int16 flag1, Int16 flag2, Int16 flag3, string extend1, string extend2, string extend3)
        {
            return Add(name, fullName, deptID, string.Join(",", roleIds), phoneNumber, email, qq, flag1, flag2, flag3, extend1, extend2, extend3);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullName"></param>
        /// <param name="deptID"></param>
        /// <param name="roleIds"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="qq"></param>
        /// <param name="flag1"></param>
        /// <param name="flag2"></param>
        /// <param name="flag3"></param>
        /// <param name="extend1"></param>
        /// <param name="extend2"></param>
        /// <param name="extend3"></param>
        /// <returns></returns>
        public int Add(string name, string fullName, int deptID, string roleIds, string phoneNumber, string email, string qq, Int16 flag1, Int16 flag2, Int16 flag3, string extend1, string extend2, string extend3)
        {
            CheckInput(0, name, fullName, deptID, roleIds);
            Entity entity = new Entity();
            entity.NAME = name;
            entity.FULL_NAME = fullName;
            entity.PASSWORD = GetDefaultPassword(name);
            entity.DEPT_ID = deptID;
            entity.ROLE_IDS = roleIds;
            entity.IS_ENABLE = 1;
            entity.PHONE_NUMBER = phoneNumber;
            entity.E_MAIL = email;
            entity.QQ = qq;
            entity.CREATE_TIME = DateTime.Now;
            entity.UPDATE_TIME = DateTime.Now;
            entity.CREATE_UID = SystemSession.UserID;
            entity.UPDATE_UID = SystemSession.UserID;
            entity.FLAG_1 = flag1;
            entity.FLAG_2 = flag2;
            entity.FLAG_3 = flag3;
            entity.EXTEND_1 = extend1;
            entity.EXTEND_2 = extend2;
            entity.EXTEND_3 = extend3;

            return Add(entity);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fullName"></param>
        /// <param name="deptID"></param>
        /// <param name="roleIds"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="qq"></param>
        /// <param name="flag1"></param>
        /// <param name="flag2"></param>
        /// <param name="flag3"></param>
        /// <param name="extend1"></param>
        /// <param name="extend2"></param>
        /// <param name="extend3"></param>
        /// <returns></returns>
        public int Update(int id, string fullName, int deptID, List<int> roleIds, string phoneNumber, string email, string qq, Int16 flag1, Int16 flag2, Int16 flag3, string extend1, string extend2, string extend3)
        {
            return Update(id, fullName, deptID, string.Join(",", roleIds), phoneNumber, email, qq, flag1, flag2, flag3, extend1, extend2, extend3);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fullName"></param>
        /// <param name="deptID"></param>
        /// <param name="roleIds"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="qq"></param>
        /// <param name="flag1"></param>
        /// <param name="flag2"></param>
        /// <param name="flag3"></param>
        /// <param name="extend1"></param>
        /// <param name="extend2"></param>
        /// <param name="extend3"></param>
        /// <returns></returns>
        public int Update(int id, string fullName, int deptID, string roleIds, string phoneNumber, string email, string qq, Int16 flag1, Int16 flag2, Int16 flag3, string extend1, string extend2, string extend3)
        {
            CheckInput(id, "-", fullName, deptID, roleIds);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("FULL_NAME", fullName);
            dic.Add("DEPT_ID", deptID);
            dic.Add("ROLE_IDS", roleIds);
            dic.Add("PHONE_NUMBER", phoneNumber);
            dic.Add("E_MAIL", email);
            dic.Add("QQ", qq);
            dic.Add("FLAG_1", flag1);
            dic.Add("FLAG_2", flag2);
            dic.Add("FLAG_3", flag3);
            dic.Add("EXTEND_1", extend1);
            dic.Add("EXTEND_2", extend2);
            dic.Add("EXTEND_3", extend3);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Unlock(int id)
        {
            //IS_LOCKED=1表示锁定
            //LOGIN_FAIL_COUNT登录失败次数修改为0
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_LOCKED", 0);
            dic.Add("LOGIN_FAIL_COUNT", 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);
            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetEnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 1);
            dic.Add("IS_LOCKED", 0);
            dic.Add("LOGIN_FAIL_COUNT", 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetUnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 0);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public int ResetPassword(int id)
        {
            Entity entity = GetEntityByKey<Entity>(id);
            if (entity == null)
            {
                return 0;
            }
            string psd = BMD5.Encrypt(BF_SYS_CONFIG.DefaultPassword, entity.NAME);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PASSWORD", psd);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public int ResetPassword()
        {
            int i = 0;
            IList<Entity> list = GetList<Entity>("PASSWORD=?", "123456");
            if (list.Count > 0)
            {
                foreach (Entity user in list)
                {
                    i += ResetPassword(user.ID);
                }
            }
            return i;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public bool ChangePassword(string oldPassword, string newPassword, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                errorMessage = "原密码不可为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errorMessage = "新密码不可为空";
                return false;
            }
            if (SystemSession.UserID < 1)
            {
                errorMessage = "未登录";
                return false;
            }
            Entity entity = GetEntityByKey<Entity>(SystemSession.UserID);
            if (entity == null)
            {
                errorMessage = "未知用户";
                return false;
            }

            string oldpsd = BMD5.Encrypt(oldPassword, entity.NAME);
            if (oldpsd != entity.PASSWORD)
            {
                errorMessage = "原密码不正确";
                return false;
            }
            string newpsd = BMD5.Encrypt(newPassword, entity.NAME);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PASSWORD", newpsd);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, entity.ID) > 0;
        }

        /// <summary>
        /// 密码解密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string DeCodePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }
            int len = password[0] - 60;
            if (len <= 0)
            {
                return string.Empty;
            }
            char[] ps = new char[len];
            int j = 1;
            for (int i = 0; i < len; i++)
            {
                if (j >= password.Length)
                {
                    return password;
                }

                ps[i] = password[j];
                if (i % 3 == 0)
                {
                    j += 3;
                }
                else
                {
                    j += 2;
                }
            }
            return string.Join("", ps);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="menus">用户可访问菜单</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public bool Login(string userName, string password, out Entity userInfo, out Dictionary<int, BF_MENU.Entity> menus, out string errorMessage)
        {
            userInfo = null;
            menus = new Dictionary<int, BF_MENU.Entity>();
            errorMessage = string.Empty;
            DataRow dr = GetRow("NAME=?", userName);
            if (dr == null)
            {
                errorMessage = "账号不存在";
                return false;
            }
            Entity entity = ToEntity<Entity>(dr);
            if (entity.IS_LOCKED == 1)
            {
                errorMessage = "账号已经被锁定";
                return false;
            }
            if (entity.IS_ENABLE != 1)
            {
                errorMessage = "账号已经停用";
                return false;
            }
            
            //加密
            string psd1 = BMD5.Encrypt(password, userName);
            //解密再加密
            string psd2 = BMD5.Encrypt(DeCodePassword(password), userName);

            if (entity.PASSWORD != psd1 && entity.PASSWORD != psd2)
            {
                int failCount = entity.LOGIN_FAIL_COUNT + 1;
                errorMessage = "密码错误，还有" + (BF_SYS_CONFIG.MaxLoginFailCount - failCount) + "次机会";
                SetLoginFailCount(userName, failCount);
                return false;
            }

            //记录登录情况
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("LOGIN_FAIL_COUNT", 0);
            dic.Add("LOGIN_COUNT", entity.LOGIN_COUNT + 1);
            dic.Add("LAST_LOGIN_TIME", DateTime.Now);

            int i = Update(dic, "NAME=?", userName);
            if (i < 1)
            {
                errorMessage = "出现了未知错误";
                return false;
            }
            //用户信息
            userInfo = entity;
            userInfo.PASSWORD = "******";
            //可访问菜单
            menus = BF_ROLE.Instance.GetMenusByRoles(entity.ROLE_IDS);

            return true;
        }

        #region 4A登录基础验证
        public bool LoginFor4A(string userName, out Entity userInfo, out Dictionary<int, BF_MENU.Entity> menus, out string errorMessage)
        {
            userInfo = null;
            menus = new Dictionary<int, BF_MENU.Entity>();
            errorMessage = string.Empty;
            DataRow dr = GetRow("NAME=?", userName);
            if (dr == null)
            {
                errorMessage = "账号不存在";
                return false;
            }
            Entity entity = ToEntity<Entity>(dr);
            if (entity.IS_LOCKED == 1)
            {
                errorMessage = "账号已经被锁定";
                return false;
            }
            if (entity.IS_ENABLE != 1)
            {
                errorMessage = "账号已经停用";
                return false;
            }

            #region 取消密码验证

            #endregion

            //记录登录情况
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("LOGIN_FAIL_COUNT", 0);
            dic.Add("LOGIN_COUNT", entity.LOGIN_COUNT + 1);
            dic.Add("LAST_LOGIN_TIME", DateTime.Now);

            int i = Update(dic, "NAME=?", userName);
            if (i < 1)
            {
                errorMessage = "出现了未知错误";
                return false;
            }
            //用户信息
            userInfo = entity;
            userInfo.PASSWORD = "******";
            //可访问菜单
            menus = BF_ROLE.Instance.GetMenusByRoles(entity.ROLE_IDS);

            return true;
        }
        #endregion

        /// <summary>
        /// 获取用户可访问菜单
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public Dictionary<int, BF_MENU.Entity> GetMenusByUserID(int userID)
        {
            Entity entity = GetEntityByKey<Entity>(userID);
            if (entity == null)
            {
                return new Dictionary<int, BF_MENU.Entity>();
            }

            return BF_ROLE.Instance.GetMenusByRoles(entity.ROLE_IDS);
        }

        /// <summary>
        /// 记录登录失败最大次数
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="failCount">登录失败次数</param>
        private void SetLoginFailCount(string userName, int failCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("LOGIN_FAIL_COUNT", failCount);
            if (failCount >= BF_SYS_CONFIG.MaxLoginFailCount)
            {
                dic.Add("IS_LOCKED", 1);
            }

            Update(dic, "NAME=?", userName);
        }

        /// <summary>
        /// 获取用户信息，并隐藏密码
        /// </summary>
        /// <param name="where"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IList<Entity> GetList(string where = "", params object[] values)
        {
            IList<Entity> list = base.GetList<Entity>(where, values);
            if (list != null && list.Count > 0)
            {
                foreach (Entity entity in list)
                {
                    entity.PASSWORD = "******";
                }
            }
            return list;
        }

        /// <summary>
        /// 分页查询获取用户信息，并隐藏密码
        /// 
        /// <param name="pageSize"></param>
        /// </summary><param name="pageIndex"></param>
        /// <param name="order">排序</param>
        /// <param name="where"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IList<Entity> GetListPage(int pageSize, int pageIndex, Order order, string where = "", params object[] values)
        {
            IList<Entity> list = base.GetListPage<Entity>(pageSize, pageIndex, order, where, values);
            if (list != null && list.Count > 0)
            {
                foreach (Entity entity in list)
                {
                    entity.PASSWORD = "******";
                }
            }
            return list;
        }

        /// <summary>
        /// 获取用户角色ID集合
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public List<int> GetRoleIds(int userID)
        {
            return GetRoleIds(GetEntityByKey<Entity>(userID));
        }

        /// <summary>
        /// 获取用户角色ID集合
        /// </summary>
        /// <param name="entity">用户信息</param>
        /// <returns></returns>
        public static List<int> GetRoleIds(Entity entity)
        {
            List<int> list = new List<int>();
            if (entity == null || string.IsNullOrWhiteSpace(entity.ROLE_IDS))
            {
                return list;
            }

            int id = 0;
            foreach (string roleid in entity.ROLE_IDS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(roleid, out id) && list.Contains(id) == false)
                {
                    list.Add(id);
                }
            }

            return list;
        }

        #region 获取权限列表
        public string GetUserRole(int userId)
        {
            string strResult = "";
            if (userId <=0)
                return strResult;

            using (BDBHelper dbHelper = new BDBHelper())
            {
                string strSql = "select menu_ids from bf_user us left join BF_ROLE role on US.ROLE_IDS=ROLE.ID where us.id="+userId;
                object objResult = dbHelper.ExecuteReader(strSql);
                if (objResult != null)
                    return objResult.ToString();
            }
            return strResult;
        }
        #endregion

        /// <summary>
        /// 根据用户ID获取用户名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameByUserID(int id)
        {
            return GetStringValueByKey(id, "NAME");
        }

        /// <summary>
        /// 根据用户ID获取用户全名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetFullNameByUserID(int id)
        {
            return GetStringValueByKey(id, "FULL_NAME");
        }

        #region 4A侧发起的增删改
        public int Add4A(string name, string fullName, int deptID,string phoneNumber, string email)
        {
            Entity entity = new Entity();
            entity.NAME = name;
            entity.FULL_NAME = fullName;
            entity.PASSWORD = GetDefaultPassword(name);
            entity.DEPT_ID = deptID;
            entity.ROLE_IDS = "0";//用户角色默认赋值为0
            entity.IS_ENABLE = 1;
            entity.PHONE_NUMBER = phoneNumber;
            entity.E_MAIL = email;
            entity.QQ = "";
            entity.CREATE_TIME = DateTime.Now;
            entity.UPDATE_TIME = DateTime.Now;
            entity.CREATE_UID = SystemSession.UserID;
            entity.UPDATE_UID = SystemSession.UserID;
            entity.FLAG_1 = 0;
            entity.FLAG_2 = 0;
            entity.FLAG_3 = 0;
            entity.EXTEND_1 = "";
            entity.EXTEND_2 = "";
            entity.EXTEND_3 = "";

            return Add(entity,true);
        }

        public int Update4A(int id, string fullName, int deptID, string phoneNumber, string email)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("FULL_NAME", fullName);
            dic.Add("DEPT_ID", deptID);
            dic.Add("PHONE_NUMBER", phoneNumber);
            dic.Add("E_MAIL", email);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            #region 修改不支持的更改：状态、密码。计件自有的其他字段

            #endregion

            return UpdateByKey(dic, id);
        }
        public int ResetPassword4A(int id,string pwd)
        {
            Entity entity = GetEntityByKey<Entity>(id);
            if (entity == null)
            {
                return 0;
            }
            //string psd = BMD5.Encrypt(BF_SYS_CONFIG.DefaultPassword, entity.NAME);
            string psd = BMD5.Encrypt(pwd, entity.NAME);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PASSWORD", psd);
            dic.Add("UPDATE_TIME", DateTime.Now);
            dic.Add("UPDATE_UID", SystemSession.UserID);

            return UpdateByKey(dic, id);
        }

        #endregion
    }
}
