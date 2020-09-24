using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_Customer, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class Customer 
    {
        public Customer()
        { }

        /// <summary>
        /// 构造函数 MS_Customer
        /// </summary>
        /// <param name="c_id">c_id</param>
        /// <param name="c_name">c_name</param>
        /// <param name="c_type">c_type</param>
        /// <param name="c_num">c_num</param>
        /// <param name="c_remarks">c_remarks</param>
        /// <param name="c_flag">c_flag</param>
        /// <param name="c_isuse">c_isUse</param>
        /// <param name="c_owner">c_owner</param>
        /// <param name="c_ownername">c_ownerName</param>
        /// <param name="c_adddate">c_addDate</param>
        public Customer(int? c_id, string c_name, byte? c_type, string c_num, string c_remarks, byte? c_flag, bool? c_isuse, string c_owner, string c_ownername, DateTime? c_adddate,string c_business)
        {
            this.c_id = c_id;
            this.c_name = c_name;
            this.c_type = c_type;
            this.c_num = c_num;
            this.c_remarks = c_remarks;
            this.c_flag = c_flag;
            this.c_isUse = c_isuse;
            this.c_owner = c_owner;
            this.c_ownerName = c_ownername;
            this.c_addDate = c_adddate;
            this.c_business = c_business;
        }

        #region 实体属性

        /// <summary>
        /// c_id
        /// </summary>
        public int? c_id { get; set; }

        /// <summary>
        /// c_name
        /// </summary>
        public string c_name { get; set; }

        /// <summary>
        /// c_type
        /// </summary>
        public byte? c_type { get; set; }

        /// <summary>
        /// c_num
        /// </summary>
        public string c_num { get; set; }

        /// <summary>
        /// c_remarks
        /// </summary>
        public string c_remarks { get; set; }

        /// <summary>
        /// c_flag
        /// </summary>
        public byte? c_flag { get; set; }

        /// <summary>
        /// c_isUse
        /// </summary>
        public bool? c_isUse { get; set; }

        /// <summary>
        /// c_owner
        /// </summary>
        public string c_owner { get; set; }

        /// <summary>
        /// c_ownerName
        /// </summary>
        public string c_ownerName { get; set; }

        /// <summary>
        /// c_addDate
        /// </summary>
        public DateTime? c_addDate { get; set; }

        /// <summary>
        /// c_remarks
        /// </summary>
        public string c_business { get; set; }
        #endregion

    }
}