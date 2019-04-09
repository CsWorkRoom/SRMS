using CS.Library.BaseQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.BLL.FW
{
    /// <summary>
    /// 表格报表事件配置
    /// </summary>
    public class BF_TB_REPORT_EVENT : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_TB_REPORT_EVENT Instance = new BF_TB_REPORT_EVENT();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_TB_REPORT_EVENT()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_TB_REPORT_EVENT";
            this.ItemName = "表格报表事件配置";
            this.KeyField = "ID";
            this.OrderbyFields = "ORDER_NUM";
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
            /// 所属报表ID
            /// </summary>
            [Field(IsNotNull = true, IsIndex = true, Comment = "报表ID ")]
            public int REPORT_ID { get; set; }

            /// <summary>
            /// 事件类型（对应枚举变量：ButtonType）
            /// </summary>
            [Field(IsNotNull = true, Comment = "事件类型（对应枚举变量：EventType）")]
            public Int16 EVENT_TYPE { get; set; }

            /// <summary>
            /// 事件名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "事件名称 ")]
            public string EVENT_NAME { get; set; }

            /// <summary>
            /// 按钮顺序（数字越小排越前面）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "按钮顺序（数字越小排越前面）")]
            public int ORDER_NUM { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 按钮显示名称
            /// </summary>
            [Field(IsNotNull = true, Comment = "按钮显示名称 ")]
            public string BUTTON_TEXT { get; set; }

            /// <summary>
            /// 按钮背景色，允许CSS规范中的写法（为空则使用默认颜色）
            /// </summary>
            [Field(IsNotNull = false, Comment = "按钮背景色，允许CSS规范中的写法（为空则使用默认颜色）")]
            public string BUTTON_BG_COLOR { get; set; }

            /// <summary>
            /// 请求模式（对应枚举变量：RequestMode）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "请求模式（对应枚举变量：RequestMode）")]
            public Int16 REQUEST_MODE { get; set; }

            /// <summary>
            /// 按钮请求的URL地址（可以带参数，参数可以根据规则使用变量）
            /// </summary>
            [Field(IsNotNull = false, Length =1024, Comment = "按钮请求的URL地址（可以带参数，参数可以根据规则使用变量）")]
            public string REQUEST_URL { get; set; }

            /// <summary>
            /// 弹出显示时，弹出层宽度
            /// </summary>
            [Field(IsNotNull = true, Comment = "弹出显示时，弹出层宽度")]
            public Int16 SHOW_WIDTH { get; set; }

            /// <summary>
            /// 弹出显示时，弹出层高度
            /// </summary>
            [Field(IsNotNull = true, Comment = "弹出显示时，弹出层高度")]
            public Int16 SHOW_HEIGHT { get; set; }

            /// <summary>
            /// 事件图标
            /// </summary>
            [Field(IsNotNull = true, Length =200, Comment = "事件图标样式")]
            public string BUTTON_ICON { get; set; }
            /// <summary>
            /// 事件按钮
            /// </summary>
            [Field(IsNotNull = true,Length =200, Comment = "事件按钮样式")]
            public string BUTTON_STYLE { get; set; }

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

        /// <summary>
        /// 获取报表的事件列表
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns></returns>
        public List<Entity> GetEventList(int reportID)
        {
            return GetList<Entity>("REPORT_ID=?", reportID).ToList();
        }

        /// <summary>
        /// 设置事件列表
        /// </summary>
        /// <param name="reportID">报表ID</param>
        /// <param name="eventDataList">事件列表</param>
        /// <returns></returns>
        public int SetEventData(int reportID, List<Entity> eventDataList, out int addCount, out int updateCount, out int deleteCount)
        {
            addCount = 0;
            updateCount = 0;
            deleteCount = 0;
            if (reportID < 1)
            {
                return 0;
            }

            Dictionary<string, Entity> dicNew = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicOld = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicInsert = new Dictionary<string, Entity>();
            Dictionary<string, Entity> dicUpdate = new Dictionary<string, Entity>();
            List<int> listDelete = new List<int>();
            if (eventDataList != null && eventDataList.Count > 0)
            {
                for (int i = 0; i < eventDataList.Count; i++)
                {
                    Entity entity = eventDataList[i];
                    entity.ORDER_NUM = i;
                    string fn = entity.EVENT_NAME.ToUpper();
                    if (dicNew.ContainsKey(fn) == false)
                    {
                        dicNew.Add(fn, entity);
                    }
                }
            }

            List<Entity> listOld = GetEventList(reportID);
            if (listOld != null && listOld.Count > 0)
            {
                foreach (Entity entity in listOld)
                {
                    string fn = entity.EVENT_NAME.ToUpper();
                    if (dicOld.ContainsKey(fn) == false)
                    {
                        dicOld.Add(fn, entity);
                    }
                    //将删除
                    if (dicNew.ContainsKey(fn) == false)
                    {
                        listDelete.Add(entity.ID);
                    }
                }
            }

            foreach (Entity entity in eventDataList)
            {
                string fn = entity.EVENT_NAME.ToUpper();
                if (dicOld.ContainsKey(fn) == true)
                {
                    if (dicUpdate.ContainsKey(fn) == false)
                    {
                        dicUpdate.Add(fn, entity);
                    }
                }
                else
                {
                    if (dicInsert.ContainsKey(fn) == false)
                    {
                        dicInsert.Add(fn, entity);
                    }
                }

            }

            //删除
            if (listDelete.Count > 0)
            {
                deleteCount = Delete("ID IN (" + string.Join(",", listDelete) + ")");
            }
            //添加
            if (dicInsert.Count > 0)
            {
                List<Entity> listInsert = dicInsert.Values.ToList<Entity>();
                for (int i = 0; i < dicInsert.Count; i++)
                {
                    Entity entity = listInsert[i];
                    entity.REPORT_ID = reportID;
                    entity.IS_ENABLE = 1;
                    entity.CREATE_TIME = DateTime.Now;
                    entity.UPDATE_TIME = DateTime.Now;
                    entity.CREATE_UID = SystemSession.UserID;
                    entity.UPDATE_UID = SystemSession.UserID;
                    
                    addCount += Add<Entity>(entity);
                }
            }
            //更新
            if (dicUpdate.Count > 0)
            {
                List<Entity> listUpdate = dicUpdate.Values.ToList<Entity>();
                for (int i = 0; i < listUpdate.Count; i++)
                {
                    Entity entity = listUpdate[i];
                    entity.REPORT_ID = reportID;
                    entity.IS_ENABLE = 1;
                    entity.UPDATE_TIME = DateTime.Now;
                    entity.UPDATE_UID = SystemSession.UserID;

                    updateCount += UpdateByKey<Entity>(entity, entity.ID);
                }
            }

            return addCount + updateCount + deleteCount;
        }
    }
}
