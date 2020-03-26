using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_finance, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class finance
    {
        public finance()
        { }

        /// <summary>
        /// 构造函数 MS_finance
        /// </summary>
        /// <param name="fin_id">fin_id</param>
        /// <param name="fin_oid">fin_oid</param>
        /// <param name="fin_type">fin_type</param>
        /// <param name="fin_cid">fin_cid</param>
        /// <param name="fin_marknum">fin_markNum</param>
        /// <param name="fin_nature">fin_nature</param>
        /// <param name="fin_detail">fin_detail</param>
        /// <param name="fin_sdate">fin_sdate</param>
        /// <param name="fin_edate">fin_edate</param>
        /// <param name="fin_illustration">fin_illustration</param>
        /// <param name="fin_expression">fin_expression</param>
        /// <param name="fin_money">fin_money</param>
        /// <param name="fin_month">fin_month</param>
        /// <param name="fin_flag">fin_flag</param>
        /// <param name="fin_checknum">fin_checkNum</param>
        /// <param name="fin_checkname">fin_checkName</param>
        /// <param name="fin_checkremark">fin_checkRemark</param>
        /// <param name="fin_area">fin_area</param>
        /// <param name="fin_personnum">fin_personNum</param>
        /// <param name="fin_personname">fin_personName</param>
        /// <param name="fin_adddate">fin_adddate</param>
        /// <param name="fin_remark">fin_remark</param>
        public finance(int? fin_id, string fin_oid, bool? fin_type, int? fin_cid, string fin_marknum, int? fin_nature, string fin_detail, DateTime? fin_sdate, DateTime? fin_edate, string fin_illustration, string fin_expression, decimal? fin_money, string fin_month, byte? fin_flag, string fin_checknum, string fin_checkname, string fin_checkremark, string fin_area, string fin_personnum, string fin_personname, DateTime? fin_adddate, string fin_remark)
        {
            this.fin_id = fin_id;
            this.fin_oid = fin_oid;
            this.fin_type = fin_type;
            this.fin_cid = fin_cid;
            this.fin_markNum = fin_marknum;
            this.fin_nature = fin_nature;
            this.fin_detail = fin_detail;
            this.fin_sdate = fin_sdate;
            this.fin_edate = fin_edate;
            this.fin_illustration = fin_illustration;
            this.fin_expression = fin_expression;
            this.fin_money = fin_money;
            this.fin_month = fin_month;
            this.fin_flag = fin_flag;
            this.fin_checkNum = fin_checknum;
            this.fin_checkName = fin_checkname;
            this.fin_checkRemark = fin_checkremark;
            this.fin_area = fin_area;
            this.fin_personNum = fin_personnum;
            this.fin_personName = fin_personname;
            this.fin_adddate = fin_adddate;
            this.fin_remark = fin_remark;
        }

        #region 实体属性

        /// <summary>
        /// fin_id
        /// </summary>
        public int? fin_id { get; set; }

        /// <summary>
        /// fin_oid
        /// </summary>
        public string fin_oid { get; set; }

        /// <summary>
        /// fin_type
        /// </summary>
        public bool? fin_type { get; set; }

        /// <summary>
        /// fin_cid
        /// </summary>
        public int? fin_cid { get; set; }

        /// <summary>
        /// fin_markNum
        /// </summary>
        public string fin_markNum { get; set; }

        /// <summary>
        /// fin_nature
        /// </summary>
        public int? fin_nature { get; set; }

        /// <summary>
        /// fin_detail
        /// </summary>
        public string fin_detail { get; set; }

        /// <summary>
        /// fin_sdate
        /// </summary>
        public DateTime? fin_sdate { get; set; }

        /// <summary>
        /// fin_edate
        /// </summary>
        public DateTime? fin_edate { get; set; }

        /// <summary>
        /// fin_illustration
        /// </summary>
        public string fin_illustration { get; set; }

        /// <summary>
        /// fin_expression
        /// </summary>
        public string fin_expression { get; set; }

        /// <summary>
        /// fin_money
        /// </summary>
        public decimal? fin_money { get; set; }

        /// <summary>
        /// fin_month
        /// </summary>
        public string fin_month { get; set; }

        /// <summary>
        /// fin_flag
        /// </summary>
        public byte? fin_flag { get; set; }

        /// <summary>
        /// fin_checkNum
        /// </summary>
        public string fin_checkNum { get; set; }

        /// <summary>
        /// fin_checkName
        /// </summary>
        public string fin_checkName { get; set; }

        /// <summary>
        /// fin_checkRemark
        /// </summary>
        public string fin_checkRemark { get; set; }

        /// <summary>
        /// fin_area
        /// </summary>
        public string fin_area { get; set; }

        /// <summary>
        /// fin_personNum
        /// </summary>
        public string fin_personNum { get; set; }

        /// <summary>
        /// fin_personName
        /// </summary>
        public string fin_personName { get; set; }

        /// <summary>
        /// fin_adddate
        /// </summary>
        public DateTime? fin_adddate { get; set; }

        /// <summary>
        /// fin_remark
        /// </summary>
        public string fin_remark { get; set; }

        #endregion
    }
}