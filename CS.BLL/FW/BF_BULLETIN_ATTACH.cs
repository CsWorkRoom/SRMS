using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 公告附件
    /// </summary>
    public class BF_BULLETIN_ATTACH : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_BULLETIN_ATTACH Instance = new BF_BULLETIN_ATTACH();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_BULLETIN_ATTACH()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_BULLETIN_ATTACH";
            this.ItemName = "公告附件";
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
            /// 附件路径
            /// </summary>
            [Field(IsNotNull = true, Length = 1024, Comment = "附件路径")]
            public string FILE_PATH { get; set; }

        }

        #endregion
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="userList"></param>
        public void AddBulletinAttach(int bullId, List<string> PathList)
        {
            foreach (var item in PathList)
            {
                Entity entity = new Entity();
                entity.BULL_ID = bullId;
                entity.FILE_PATH = item;
                Add(entity);
            }

        }
        /// <summary>
        /// 根据公告ID删除附件
        /// </summary>
        /// <param name="bullId"></param>
        public void DleteBulletinAttach(int bullId)
        {
            Instance.Delete("BULL_ID=?", bullId);
        }
        /// <summary>
        /// 修改附件资料
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="PathList"></param>
        public void UpdateBulletinAttach(int bullId, List<string> PathList)
        {
            //先删除
            DleteBulletinAttach(bullId);
            //新增
            AddBulletinAttach(bullId, PathList);
        }

    }
}
