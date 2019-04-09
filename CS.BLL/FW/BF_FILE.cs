using CS.Base.DBHelper;
using CS.Base.Log;
using CS.Common.FW;
using CS.Library.BaseQuery;
using CS.Library.Export;
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
    /// 附件
    /// </summary>
    public class BF_FILE : BBaseQuery
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static BF_FILE Instance = new BF_FILE();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BF_FILE()
        {
            this.IsAddIntoCache = true;
            this.TableName = "BF_FILE";
            this.ItemName = "附件";
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
            /// 名称
            /// </summary>
            [Field(IsNotNull = true, Length = 64, IsIndex = true, IsIndexUnique = true, Comment = "名称")]
            public string NAME { get; set; }

            /// <summary>
            /// 数据库ID（为0时表示默认数据库）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "数据库ID（为0时表示默认数据库）")]
            public int DB_ID { get; set; }

            /// <summary>
            /// 数据库表名
            /// </summary>
            [Field(IsNotNull = true, Length = 64, Comment = "数据库表名")]
            public string TABLE_NAME { get; set; }

            /// <summary>
            /// 建表模式（对应枚举：CreateTableMode）
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "建表模式（对应枚举：CreateTableMode）")]
            public Int16 CREATE_TABLE_MODE { get; set; }

            /// <summary>
            /// 主键字段名
            /// </summary>
            [Field(IsNotNull = false, Length = 64, Comment = "主键字段名")]
            public string KEY_FIELD { get; set; }

            /// <summary>
            /// 是否允许删除自己上传的附件
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "0", Comment = "是否允许删除自己上传的附件")]
            public Int16 IS_ALLOW_DELETE { get; set; }

            /// <summary>
            /// 允许的文件类型（多种文件类型用“,”分隔）
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "允许的文件类型（多种文件类型用“,”分隔）")]
            public string ACCEPT_FILE_TYPES { get; set; }

            /// <summary>
            /// 是否启用
            /// </summary>
            [Field(IsNotNull = true, DefaultValue = "1", Comment = "是否有效")]
            public Int16 IS_ENABLE { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            [Field(IsNotNull = false, Length = 512, Comment = "备注")]
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

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetEnable(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IS_ENABLE", 1);
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

        #region 可接受的文件类型

        /// <summary>
        /// 获取接受的文件类型
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static List<Enums.AcceptUploadFileType> GetAcceptFileTypes(Entity entity)
        {
            List<Enums.AcceptUploadFileType> list = new List<Enums.AcceptUploadFileType>();
            if (entity==null|| string.IsNullOrWhiteSpace(entity.ACCEPT_FILE_TYPES))
            {
                return list;
            }

            string[] ts = entity.ACCEPT_FILE_TYPES.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Enums.AcceptUploadFileType type = Enums.AcceptUploadFileType.不限;
            foreach (string t in ts)
            {
                if (Enum.TryParse<Enums.AcceptUploadFileType>(t, out type))
                {
                    if (type == Enums.AcceptUploadFileType.不限)
                    {
                        list = new List<Enums.AcceptUploadFileType>();
                        list.Add(type);
                        return list;
                    }

                    list.Add(type);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取上传支持的MIME文件类型
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="mimeTypes">MIME文件类型，逗号分隔</param>
        /// <param name="extensionNames">扩展名，｜分隔</param>
        /// <returns></returns>
        public static void GetAcceptMimeTypes(Entity entity, out string mimeTypes, out string extensionNames)
        {
            mimeTypes = string.Empty;
            extensionNames = string.Empty;
            List<string> ml = new List<string>();
            List<string> el = new List<string>();
            List<Enums.AcceptUploadFileType> typeList = GetAcceptFileTypes(entity);
            foreach (Enums.AcceptUploadFileType type in typeList)
            {
                switch (type)
                {
                    case Enums.AcceptUploadFileType.不限:
                        mimeTypes = "*.*";
                        extensionNames = "";
                        return;
                    case Enums.AcceptUploadFileType.图片:
                        ml.Add("image/*");
                        el.Add("jpg");
                        el.Add("jpeg");
                        el.Add("png");
                        el.Add("gif");
                        el.Add("bmp");
                        break;
                    case Enums.AcceptUploadFileType.文本:
                        ml.Add("text/plain");
                        el.Add("txt");
                        break;
                    case Enums.AcceptUploadFileType.Word文档:
                        ml.Add("application/msword");
                        ml.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                        el.Add("doc");
                        el.Add("docx");
                        break;
                    case Enums.AcceptUploadFileType.PDF文档:
                        ml.Add("application/pdf");
                        el.Add("pdf");
                        break;
                    case Enums.AcceptUploadFileType.Excel表格:
                        ml.Add("application/vnd.ms-excel");
                        ml.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        el.Add("xls");
                        el.Add("xlsx");
                        break;
                    case Enums.AcceptUploadFileType.压缩包:
                        ml.Add("application/octet-stream");
                        ml.Add("application/x-zip-compressed");
                        ml.Add("application/x-gzip");
                        el.Add("rar");
                        el.Add("zip");
                        el.Add("gz");
                        el.Add("7z");
                        break;
                }
            }

            mimeTypes = string.Join(", ", ml);
            extensionNames = string.Join("|", el);
        }

        #endregion

        #region 上传
        /// <summary>
        /// 检测是否可上传，并将异常抛出
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="rowkey">主键值</param>
        /// <param name="yyyymmdd">如果是按日期为后缀的动态表，则需要传入该表日期</param>
        /// <param name="tableName">表名</param>
        public Entity CheckUpload(int id, string rowkey, string yyyymmdd, out string tableName)
        {
            if (id < 1)
            {
                throw new Exception("请通过参数id传入正确的配置ID");
            }
            if (string.IsNullOrWhiteSpace(rowkey))
            {
                throw new Exception("请通过参数rowkey传入附件所属记录的主键值");
            }
            Entity entity = GetEntityByKey<Entity>(id);
            if (entity == null)
            {
                throw new Exception("未找到配置，请通过参数id传入正确的配置ID");
            }
            tableName = entity.TABLE_NAME;
            int ymd = 0;
            switch (entity.CREATE_TABLE_MODE)
            {
                case (short)Enums.CreateTableMode.年份后缀:
                    if (string.IsNullOrWhiteSpace(yyyymmdd) || yyyymmdd.Length < 4 || int.TryParse(yyyymmdd, out ymd) == false)
                    {
                        throw new Exception("由于目标表以年份为后缀，请通过yyyymmdd传入相应日期");
                    }
                    tableName += "_" + yyyymmdd.Substring(0, 4);
                    break;
                case (short)Enums.CreateTableMode.年月后缀:
                    if (string.IsNullOrWhiteSpace(yyyymmdd) || yyyymmdd.Length < 6 || int.TryParse(yyyymmdd, out ymd) == false)
                    {
                        throw new Exception("由于目标表以年月为后缀，请通过yyyymmdd传入相应日期");
                    }
                    tableName += "_" + yyyymmdd.Substring(0, 6);
                    break;
                case (short)Enums.CreateTableMode.年月日后缀:
                    if (string.IsNullOrWhiteSpace(yyyymmdd) || yyyymmdd.Length != 8 || int.TryParse(yyyymmdd, out ymd) == false)
                    {
                        throw new Exception("由于目标表以日期为后缀，请通过yyyymmdd传入相应日期");
                    }
                    tableName += "_" + yyyymmdd;
                    break;
                case (short)Enums.CreateTableMode.用户ID后缀:
                    tableName += "_" + SystemSession.UserID;
                    break;
            }

            BDBHelper dbHelper = null;
            try
            {
                if (entity.DB_ID == 0)
                {
                    dbHelper = new BDBHelper();
                }
                else
                {
                    BF_DATABASE.Entity db = BF_DATABASE.Instance.GetEntityByKey<BF_DATABASE.Entity>(entity.DB_ID);
                    if (db == null)
                    {
                        throw new Exception("数据库配置错误");
                    }
                    string dbType = BF_DATABASE.GetDbTypeName(db.DB_TYPE);
                    dbHelper = new BDBHelper(dbType, db.IP, db.PORT, db.USER_NAME, db.PASSWORD, db.DB_NAME, db.DB_NAME);
                }
                if (dbHelper.TableIsExists(tableName) == false)
                {
                    throw new Exception("目标表" + tableName + "不存在");
                }
                string sql = string.Format("SELECT * FROM {0} WHERE {1}=?", tableName, entity.KEY_FIELD);
                DataRow dr = dbHelper.ExecuteDataRowParams(sql, rowkey);
                if (dr == null)
                {
                    throw new Exception(string.Format("在目标表{0}中，没有找到主键{1}值为{2}的记录", tableName, entity.KEY_FIELD, rowkey));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("未知错误：" + ex.Message);
            }
            finally
            {
                if (dbHelper != null)
                {
                    dbHelper.Dispose();
                }
            }

            return entity;
        }

        #endregion
    }
}
