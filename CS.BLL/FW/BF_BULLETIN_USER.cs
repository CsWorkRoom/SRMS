using CS.Base.DBHelper;
using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 公告阅读情况
    /// 创建公告时，根据选择的部门或者角色，提取指定范围内的用户ID集合（记得去重），逐一添加到本表
    /// </summary>
    public class BF_BULLETIN_USER : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_BULLETIN_USER Instance = new BF_BULLETIN_USER();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_BULLETIN_USER()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_BULLETIN_USER";
            this.ItemName = "构造函数";
            this.KeyField = "ID";
            this.OrderbyFields = "BULL_ID";
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
            /// 公告ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "公告ID")]
            public int BULL_ID { get; set; }

            /// <summary>
            /// 用户ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "用户ID")]
            public int USER_ID { get; set; }

            /// <summary>
            /// 是否已读
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, DefaultValue = "0", Comment = "是否已读")]
            public Int16 IS_READ { get; set; }

            /// <summary>
            /// 阅读时间
            /// </summary>
            [Field(IsNotNull = false, Comment = "阅读时间")]
            public DateTime READ_TIME { get; set; }
        }

        #endregion

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="userList"></param>
        public void AddBulletinUser(int bullId, List<int> userList)
        {
            foreach (var item in userList)
            {
                Entity entity = new Entity();
                entity.BULL_ID = bullId;
                entity.USER_ID = item;
                entity.IS_READ = 0;
                Add(entity);
            }

        }
        /// <summary>
        /// 根据公告ID，用户ID删除数据
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="Uid"></param>
        public void DeleteBulletinUser(int bullId, int Uid)
        {
            Instance.Delete("BULL_ID=? and USER_ID=?", bullId, Uid);
        }
        /// <summary>
        /// 根据公告ID删除数据
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="Uid"></param>
        public void DeleteBulletinUsers(int bullId)
        {
            Instance.Delete("BULL_ID=? ", bullId);
        }
        /// <summary>
        /// 根据公告ID修改用户数据
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="userList"></param>
        public void UpdateBulletinUser(int bullId, List<int> userList)
        {
            //1:查询公告用户集合
            List<int> list = new List<int>();
            var list1 = Instance.GetList<Entity>("BULL_ID=?", bullId);
            foreach (var item in list1)
            {
                list.Add(item.USER_ID);
            }

            //取出需要新增的用户ID集合
            List<int> listAdd = new List<int>();
            foreach (var item in userList)
            {
                if (!list.Contains(item))
                {
                    listAdd.Add(item);
                }
            }
            AddBulletinUser(bullId, listAdd);

            //删除数据
            foreach (var item in list)
            {
                if (!userList.Contains(item))
                {
                    Instance.DeleteBulletinUser(bullId, item);
                }
            }


        }

        /// <summary>
        /// 已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateRead(int id)
        {
            int uCount = Instance.GetCount("BULL_ID=" + id + " AND USER_ID=" + SystemSession.UserID);
            if (uCount <= 0)
            {
                Entity entity = new FW.BF_BULLETIN_USER.Entity();
                entity.BULL_ID = id;
                entity.IS_READ = 1;
                entity.READ_TIME = DateTime.Now;
                entity.USER_ID = SystemSession.UserID;
                return Instance.Add(entity);
            }
            return 1;
        }

        /// <summary>
        /// 获取当前用户未读公告数量
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int GetBullReadNum(int uid)
        {
            var list = Instance.GetList<Entity>("USER_ID=? AND IS_READ=?", uid, 1);//已读数量
            List<BF_BULLETIN.Entity> dt = BF_BULLETIN.Instance.GetUserPowerRead(uid);//读取当前用户应读的文档;
            return (dt.Count() - list.Count());
        }

    }
}