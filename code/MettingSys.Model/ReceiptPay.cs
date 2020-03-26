using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_ReceiptPay, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class ReceiptPay
    {
        public ReceiptPay()
        { }

        /// <summary>
        /// 构造函数 MS_ReceiptPay
        /// </summary>
        /// <param name="rp_id">rp_id</param>
        /// <param name="rp_type">rp_type</param>
        /// <param name="rp_isExpect">rp_isExpect</param>
        /// <param name="rp_cid">rp_cid</param>
        /// <param name="rp_content">rp_content</param>
        /// <param name="rp_money">rp_money</param>
        /// <param name="rp_foredate">rp_foredate</param>
        /// <param name="rp_method">rp_method</param>
        /// <param name="rp_personnum">rp_personNum</param>
        /// <param name="rp_personname">rp_personName</param>
        /// <param name="rp_date">rp_date</param>
        /// <param name="rp_isconfirm">rp_isConfirm</param>
        /// <param name="rp_confirmernum">rp_confirmerNum</param>
        /// <param name="rp_confirmername">rp_confirmerName</param>
        /// <param name="rp_adddate">rp_adddate</param>
        /// <param name="rp_area">rp_area</param>
        public ReceiptPay(int? rp_id, bool? rp_type,int? rp_ceid,bool? rp_isExpect, int? rp_cid, string rp_content, decimal? rp_money, DateTime? rp_foredate, int? rp_method, string rp_personnum, string rp_personname, DateTime? rp_date, byte? rp_flag, string rp_checknum, string rp_checkname, string rp_checkremark, DateTime? rp_checkTime, byte? rp_flag1, string rp_checknum1, string rp_checkname1, string rp_checkremark1, DateTime? rp_checkTime1, bool? rp_isconfirm, string rp_confirmernum, string rp_confirmername, DateTime? rp_adddate, string rp_area,int? rp_cbid)
        {
            this.rp_id = rp_id;
            this.rp_type = rp_type;
            this.rp_ceid = rp_ceid;
            this.rp_isExpect = rp_isExpect;
            this.rp_cid = rp_cid;
            this.rp_content = rp_content;
            this.rp_money = rp_money;
            this.rp_foredate = rp_foredate;
            this.rp_method = rp_method;
            this.rp_personNum = rp_personnum;
            this.rp_personName = rp_personname;
            this.rp_date = rp_date;
            this.rp_flag = rp_flag;
            this.rp_checkNum = rp_checknum;
            this.rp_checkName = rp_checkname;
            this.rp_checkRemark = rp_checkremark;
            this.rp_checkTime = rp_checkTime;
            this.rp_flag1 = rp_flag1;
            this.rp_checkNum1 = rp_checknum1;
            this.rp_checkName1 = rp_checkname1;
            this.rp_checkRemark1 = rp_checkremark1;
            this.rp_checkTime1 = rp_checkTime1;
            this.rp_isConfirm = rp_isconfirm;
            this.rp_confirmerNum = rp_confirmernum;
            this.rp_confirmerName = rp_confirmername;
            this.rp_adddate = rp_adddate;
            this.rp_area = rp_area;
            this.rp_cbid = rp_cbid;
        }

        #region 实体属性

        /// <summary>
        /// rp_id
        /// </summary>
        public int? rp_id { get; set; }

        /// <summary>
        /// rp_type
        /// </summary>
        public bool? rp_type { get; set; }

        /// <summary>
        /// rp_type
        /// </summary>
        public int? rp_ceid { get; set; }

        /// <summary>
        /// rp_isExpect
        /// </summary>
        public bool? rp_isExpect { get; set; }

        /// <summary>
        /// rp_cid
        /// </summary>
        public int? rp_cid { get; set; }

        /// <summary>
        /// rp_content
        /// </summary>
        public string rp_content { get; set; }

        /// <summary>
        /// rp_money
        /// </summary>
        public decimal? rp_money { get; set; }

        /// <summary>
        /// rp_foredate
        /// </summary>
        public DateTime? rp_foredate { get; set; }

        /// <summary>
        /// rp_method
        /// </summary>
        public int? rp_method { get; set; }

        /// <summary>
        /// rp_personNum
        /// </summary>
        public string rp_personNum { get; set; }

        /// <summary>
        /// rp_personName
        /// </summary>
        public string rp_personName { get; set; }

        /// <summary>
        /// rp_date
        /// </summary>
        public DateTime? rp_date { get; set; }

        /// <summary>
        /// rpd_flag
        /// </summary>
        public byte? rp_flag { get; set; }

        /// <summary>
        /// rpd_checkNum
        /// </summary>
        public string rp_checkNum { get; set; }

        /// <summary>
        /// rpd_checkName
        /// </summary>
        public string rp_checkName { get; set; }

        /// <summary>
        /// rpd_checkRemark
        /// </summary>
        public string rp_checkRemark { get; set; }

        /// <summary>
        /// rp_foredate
        /// </summary>
        public DateTime? rp_checkTime { get; set; }
        /// <summary>
        /// rpd_flag1
        /// </summary>
        public byte? rp_flag1 { get; set; }

        /// <summary>
        /// rpd_checkNum1
        /// </summary>
        public string rp_checkNum1 { get; set; }

        /// <summary>
        /// rpd_checkName1
        /// </summary>
        public string rp_checkName1 { get; set; }

        /// <summary>
        /// rpd_checkRemark1
        /// </summary>
        public string rp_checkRemark1 { get; set; }

        /// <summary>
        /// rp_foredate
        /// </summary>
        public DateTime? rp_checkTime1 { get; set; }
        /// <summary>
        /// rp_isConfirm
        /// </summary>
        public bool? rp_isConfirm { get; set; }

        /// <summary>
        /// rp_confirmerNum
        /// </summary>
        public string rp_confirmerNum { get; set; }

        /// <summary>
        /// rp_confirmerName
        /// </summary>
        public string rp_confirmerName { get; set; }

        /// <summary>
        /// rp_adddate
        /// </summary>
        public DateTime? rp_adddate { get; set; }

        /// <summary>
        /// rp_area
        /// </summary>
        public string rp_area { get; set; }

        /// <summary>
        /// rp_method
        /// </summary>
        public int? rp_cbid { get; set; }

        #endregion

    }
}