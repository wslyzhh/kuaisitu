using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_customerBank, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class customerBank
    {
        public customerBank()
        { }

        /// <summary>
        /// 构造函数 MS_customerBank
        /// </summary>
        /// <param name="cb_id">cb_id</param>
        /// <param name="cb_cid">cb_cid</param>
        /// <param name="cb_bankname">cb_bankName</param>
        /// <param name="cb_banknum">cb_bankNum</param>
        /// <param name="cb_bank">cb_bank</param>
        /// <param name="cb_bankaddress">cb_bankAddress</param>
        /// <param name="cb_flag">cb_flag</param>
        public customerBank(int? cb_id, int? cb_cid, string cb_bankname, string cb_banknum, string cb_bank, string cb_bankaddress, bool? cb_flag)
        {
            this.cb_id = cb_id;
            this.cb_cid = cb_cid;
            this.cb_bankName = cb_bankname;
            this.cb_bankNum = cb_banknum;
            this.cb_bank = cb_bank;
            this.cb_bankAddress = cb_bankaddress;
            this.cb_flag = cb_flag;
        }

        #region 实体属性

        /// <summary>
        /// cb_id
        /// </summary>
        public int? cb_id { get; set; }

        /// <summary>
        /// cb_cid
        /// </summary>
        public int? cb_cid { get; set; }

        /// <summary>
        /// cb_bankName
        /// </summary>
        public string cb_bankName { get; set; }

        /// <summary>
        /// cb_bankNum
        /// </summary>
        public string cb_bankNum { get; set; }

        /// <summary>
        /// cb_bank
        /// </summary>
        public string cb_bank { get; set; }

        /// <summary>
        /// cb_bankAddress
        /// </summary>
        public string cb_bankAddress { get; set; }

        /// <summary>
        /// cb_flag
        /// </summary>
        public bool? cb_flag { get; set; }
        

        #endregion
    }
}