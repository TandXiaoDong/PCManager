using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace ShikuIM.Model
{
    /// <summary>
	/// tb_areas:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
    [Table("tb_areas")]
    public partial class Areas
    {
        public Areas()
        {
            //DBContext = new UserDBContext();
        }

        //UserDBContext DBContext;
        #region Model
        private int _id;
        private int _parent_id;
        private int _type;
        private string _name;
        private string _zip;

        /// <summary>
        /// 
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int parent_id
        {
            set { _parent_id = value; }
            get { return _parent_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int type
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string zip
        {
            set { _zip = value; }
            get { return _zip; }
        }
        #endregion Model
        public override string ToString()
        {
            return name;
        }

        #region 获取子项位置列表
        /// <summary>
        /// 获取子项位置列表
        /// </summary>
        /// <returns></returns>
        public List<Areas> GetChildrenList()
        {
            try
            {
                SystemDBContext.DBAutoConnect();
                var result = (from area in Applicate.SystemDbContext.tbareas
                              where area.parent_id == this.parent_id
                             && area.type == this.type
                              select area
                              ).ToList();
                if (result == null)
                {
                    return new List<Areas>();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Areas GetModel()
        {
            SystemDBContext.DBAutoConnect();
            var result = (from area in Applicate.SystemDbContext.tbareas
                          where area.id == this.id
                          select area
                          ).FirstOrDefault();
            if (result == null)
            {
                result = new Areas();
            }

            return result;
        }
    }
}
