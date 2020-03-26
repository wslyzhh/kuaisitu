using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_permission, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class permission
    {
        public permission()
        { }

        /// <summary>
        /// 构造函数 MS_permission
        /// </summary>
        /// <param name="pe_id">pe_id</param>
        /// <param name="pe_parentid">pe_parentid</param>
        /// <param name="pe_name">pe_name</param>
        /// <param name="pe_code">pe_code</param>
        /// <param name="pe_remark">pe_remark</param>
        public permission(int? pe_id, int? pe_parentid, string pe_name, string pe_code, string pe_remark)
        {
            this.pe_id = pe_id;
            this.pe_parentid = pe_parentid;
            this.pe_name = pe_name;
            this.pe_code = pe_code;
            this.pe_remark = pe_remark;
        }

        #region 实体属性

        /// <summary>
        /// pe_id
        /// </summary>
        public int? pe_id { get; set; }

        /// <summary>
        /// pe_parentid
        /// </summary>
        public int? pe_parentid { get; set; }

        /// <summary>
        /// pe_name
        /// </summary>
        public string pe_name { get; set; }

        /// <summary>
        /// pe_code
        /// </summary>
        public string pe_code { get; set; }

        /// <summary>
        /// pe_remark
        /// </summary>
        public string pe_remark { get; set; }

        #endregion
    }
}