using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_finance_chk, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class finance_chk
    {
        public finance_chk()
        { }

        /// <summary>
        /// 构造函数 MS_finance_chk
        /// </summary>
        /// <param name="fc_id">fc_id</param>
        /// <param name="fc_oid">fc_oid</param>
        /// <param name="fc_finid">fc_finid</param>
        /// <param name="fc_num">fc_num</param>
        /// <param name="fc_money">fc_money</param>
        /// <param name="fc_number">fc_number</param>
        /// <param name="fc_name">fc_name</param>
        /// <param name="fc_adddate">fc_addDate</param>
        public finance_chk(int? fc_id, string fc_oid, int? fc_finid, string fc_num, decimal? fc_money, string fc_number, string fc_name, DateTime? fc_adddate)
        {
            this.fc_id = fc_id;
            this.fc_oid = fc_oid;
            this.fc_finid = fc_finid;
            this.fc_num = fc_num;
            this.fc_money = fc_money;
            this.fc_number = fc_number;
            this.fc_name = fc_name;
            this.fc_addDate = fc_adddate;
        }

        #region 实体属性

        /// <summary>
        /// fc_id
        /// </summary>
        public int? fc_id { get; set; }

        /// <summary>
        /// fc_oid
        /// </summary>
        public string fc_oid { get; set; }

        /// <summary>
        /// fc_finid
        /// </summary>
        public int? fc_finid { get; set; }

        /// <summary>
        /// fc_num
        /// </summary>
        public string fc_num { get; set; }

        /// <summary>
        /// fc_money
        /// </summary>
        public decimal? fc_money { get; set; }

        /// <summary>
        /// fc_number
        /// </summary>
        public string fc_number { get; set; }

        /// <summary>
        /// fc_name
        /// </summary>
        public string fc_name { get; set; }

        /// <summary>
        /// fc_addDate
        /// </summary>
        public DateTime? fc_addDate { get; set; }

        #endregion
    }
}