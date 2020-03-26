using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 department, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class department
    {
        public department()
        { }

        /// <summary>
        /// 构造函数 department
        /// </summary>
        /// <param name="de_id">de_id</param>
        /// <param name="de_parentid">de_parentid</param>
        /// <param name="de_isgroup">de_isGroup</param>
        /// <param name="de_type">de_type</param>
        /// <param name="de_name">de_name</param>
        /// <param name="de_area">de_area</param>
        /// <param name="de_sort">de_sort</param>
        public department(int? de_id, int? de_parentid, bool? de_isgroup, byte? de_type, string de_name,string de_subname, string de_area, int? de_sort,bool? de_isUse)
        {
            this.de_id = de_id;
            this.de_parentid = de_parentid;
            this.de_isGroup = de_isgroup;
            this.de_type = de_type;
            this.de_name = de_name;
            this.de_subname = de_subname;
            this.de_area = de_area;
            this.de_sort = de_sort;
            this.de_isUse = de_isUse;
        }

        #region 实体属性

        /// <summary>
        /// de_id
        /// </summary>
        public int? de_id { get; set; }

        /// <summary>
        /// de_parentid
        /// </summary>
        public int? de_parentid { get; set; }

        /// <summary>
        /// de_isGroup
        /// </summary>
        public bool? de_isGroup { get; set; }

        /// <summary>
        /// de_type
        /// </summary>
        public byte? de_type { get; set; }

        /// <summary>
        /// de_name
        /// </summary>
        public string de_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string de_subname { get; set; }
        /// <summary>
        /// de_area
        /// </summary>
        public string de_area { get; set; }

        /// <summary>
        /// de_sort
        /// </summary>
        public int? de_sort { get; set; }

        /// <summary>
        /// de_isGroup
        /// </summary>
        public bool? de_isUse { get; set; }


        #endregion

    }
}