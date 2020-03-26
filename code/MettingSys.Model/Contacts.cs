using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_Contacts, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class Contacts 
    {
        public Contacts()
        { }

        /// <summary>
        /// 构造函数 MS_Contacts
        /// </summary>
        /// <param name="co_id">co_id</param>
        /// <param name="co_cid">co_cid</param>
        /// <param name="co_flag">co_flag</param>
        /// <param name="co_name">co_name</param>
        /// <param name="co_number">co_number</param>
        public Contacts(int? co_id, int? co_cid, bool? co_flag, string co_name, string co_number)
        {
            this.co_id = co_id;
            this.co_cid = co_cid;
            this.co_flag = co_flag;
            this.co_name = co_name;
            this.co_number = co_number;
        }

        #region 实体属性

        /// <summary>
        /// co_id
        /// </summary>
        public int? co_id { get; set; }

        /// <summary>
        /// co_cid
        /// </summary>
        public int? co_cid { get; set; }

        /// <summary>
        /// co_flag
        /// </summary>
        public bool? co_flag { get; set; }

        /// <summary>
        /// co_name
        /// </summary>
        public string co_name { get; set; }
        
        /// <summary>
        /// co_number
        /// </summary>
        public string co_number { get; set; }
        
        #endregion
    }
}