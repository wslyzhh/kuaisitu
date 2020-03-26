using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_ReceiptPayDetail, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class ReceiptPayDetail 
    {
        public ReceiptPayDetail()
        { }

        /// <summary>
        /// 构造函数 MS_ReceiptPayDetail
        /// </summary>
        /// <param name="rpd_id">rpd_id</param>
        /// <param name="rpd_type">rpd_type</param>
        /// <param name="rpd_oid">rpd_oid</param>
        /// <param name="rpd_rpid">rpd_rpid</param>
        /// <param name="rpd_cid">rpd_cid</param>
        /// <param name="rpd_content">rpd_content</param>
        /// <param name="rpd_money">rpd_money</param>
        /// <param name="rpd_foredate">rpd_foredate</param>
        /// <param name="rpd_method">rpd_method</param>
        /// <param name="rpd_personnum">rpd_personNum</param>
        /// <param name="rpd_personname">rpd_personName</param>
        /// <param name="rpd_date">rpd_date</param>
        /// <param name="rpd_flag1">rpd_flag1</param>
        /// <param name="rpd_checknum1">rpd_checkNum1</param>
        /// <param name="rpd_checkname1">rpd_checkName1</param>
        /// <param name="rpd_checkremark1">rpd_checkRemark1</param>
        /// <param name="rpd_flag2">rpd_flag2</param>
        /// <param name="rpd_checknum2">rpd_checkNum2</param>
        /// <param name="rpd_checkname2">rpd_checkName2</param>
        /// <param name="rpd_checkremark2">rpd_checkRemark2</param>
        /// <param name="rpd_flag3">rpd_flag3</param>
        /// <param name="rpd_checknum3">rpd_checkNum3</param>
        /// <param name="rpd_checkname3">rpd_checkName3</param>
        /// <param name="rpd_checkremark3">rpd_checkRemark3</param>
        /// <param name="rpd_isconfirm">rpd_isConfirm</param>
        /// <param name="rpd_confirmernum">rpd_confirmerNum</param>
        /// <param name="rpd_confirmername">rpd_confirmerName</param>
        /// <param name="rpd_adddate">rpd_adddate</param>
        /// <param name="rpd_area">rpd_area</param>
        public ReceiptPayDetail(int? rpd_id, bool? rpd_type, string rpd_oid, int? rpd_rpid, int? rpd_cid,string rpd_num, string rpd_content, decimal? rpd_money, DateTime? rpd_foredate,int? rpd_method, string rpd_personnum, string rpd_personname, byte? rpd_flag1, string rpd_checknum1, string rpd_checkname1, string rpd_checkremark1,DateTime? rpd_checkTime1, byte? rpd_flag2, string rpd_checknum2, string rpd_checkname2, string rpd_checkremark2, DateTime? rpd_checkTime2, byte? rpd_flag3, string rpd_checknum3, string rpd_checkname3, string rpd_checkremark3, DateTime? rpd_checkTime3, DateTime? rpd_adddate, string rpd_area,int? rpd_cbid)
        {
            this.rpd_id = rpd_id;
            this.rpd_type = rpd_type;
            this.rpd_oid = rpd_oid;
            this.rpd_rpid = rpd_rpid;
            this.rpd_cid = rpd_cid;
            this.rpd_num = rpd_num;
            this.rpd_content = rpd_content;
            this.rpd_money = rpd_money;
            this.rpd_foredate = rpd_foredate;
            this.rpd_method = rpd_method;
            this.rpd_personNum = rpd_personnum;
            this.rpd_personName = rpd_personname;
            this.rpd_flag1 = rpd_flag1;
            this.rpd_checkNum1 = rpd_checknum1;
            this.rpd_checkName1 = rpd_checkname1;
            this.rpd_checkRemark1 = rpd_checkremark1;
            this.rpd_checkTime1 = rpd_checkTime1;
            this.rpd_flag2 = rpd_flag2;
            this.rpd_checkNum2 = rpd_checknum2;
            this.rpd_checkName2 = rpd_checkname2;
            this.rpd_checkRemark2 = rpd_checkremark2;
            this.rpd_checkTime2 = rpd_checkTime2;
            this.rpd_flag3 = rpd_flag3;
            this.rpd_checkNum3 = rpd_checknum3;
            this.rpd_checkName3 = rpd_checkname3;
            this.rpd_checkRemark3 = rpd_checkremark3;
            this.rpd_checkTime3 = rpd_checkTime3;
            this.rpd_adddate = rpd_adddate;
            this.rpd_area = rpd_area;
            this.rpd_cbid = rpd_cbid;
        }

        #region 实体属性

        /// <summary>
        /// rpd_id
        /// </summary>
        public int? rpd_id { get; set; }

        /// <summary>
        /// rpd_type
        /// </summary>
        public bool? rpd_type { get; set; }

        /// <summary>
        /// rpd_oid
        /// </summary>
        public string rpd_oid { get; set; }

        /// <summary>
        /// rpd_rpid
        /// </summary>
        public int? rpd_rpid { get; set; }

        /// <summary>
        /// rpd_cid
        /// </summary>
        public int? rpd_cid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string rpd_num { get; set; }

        /// <summary>
        /// rpd_content
        /// </summary>
        public string rpd_content { get; set; }

        /// <summary>
        /// rpd_money
        /// </summary>
        public decimal? rpd_money { get; set; }

        /// <summary>
        /// rpd_foredate
        /// </summary>
        public DateTime? rpd_foredate { get; set; }

        /// <summary>
        /// rp_method
        /// </summary>
        public int? rpd_method { get; set; }

        /// <summary>
        /// rpd_personNum
        /// </summary>
        public string rpd_personNum { get; set; }

        /// <summary>
        /// rpd_personName
        /// </summary>
        public string rpd_personName { get; set; }
        
        /// <summary>
        /// rpd_flag1
        /// </summary>
        public byte? rpd_flag1 { get; set; }

        /// <summary>
        /// rpd_checkNum1
        /// </summary>
        public string rpd_checkNum1 { get; set; }

        /// <summary>
        /// rpd_checkName1
        /// </summary>
        public string rpd_checkName1 { get; set; }

        /// <summary>
        /// rpd_checkRemark1
        /// </summary>
        public string rpd_checkRemark1 { get; set; }

        /// <summary>
        /// rpd_adddate
        /// </summary>
        public DateTime? rpd_checkTime1 { get; set; }
        /// <summary>
        /// rpd_flag2
        /// </summary>
        public byte? rpd_flag2 { get; set; }

        /// <summary>
        /// rpd_checkNum2
        /// </summary>
        public string rpd_checkNum2 { get; set; }

        /// <summary>
        /// rpd_checkName2
        /// </summary>
        public string rpd_checkName2 { get; set; }

        /// <summary>
        /// rpd_checkRemark2
        /// </summary>
        public string rpd_checkRemark2 { get; set; }

        /// <summary>
        /// rpd_adddate
        /// </summary>
        public DateTime? rpd_checkTime2 { get; set; }
        /// <summary>
        /// rpd_flag3
        /// </summary>
        public byte? rpd_flag3 { get; set; }

        /// <summary>
        /// rpd_checkNum3
        /// </summary>
        public string rpd_checkNum3 { get; set; }

        /// <summary>
        /// rpd_checkName3
        /// </summary>
        public string rpd_checkName3 { get; set; }

        /// <summary>
        /// rpd_checkRemark3
        /// </summary>
        public string rpd_checkRemark3 { get; set; }

        /// <summary>
        /// rpd_adddate
        /// </summary>
        public DateTime? rpd_checkTime3 { get; set; }
        /// <summary>
        /// rpd_adddate
        /// </summary>
        public DateTime? rpd_adddate { get; set; }

        /// <summary>
        /// rpd_area
        /// </summary>
        public string rpd_area { get; set; }

        /// <summary>
        /// rp_method
        /// </summary>
        public int? rpd_cbid { get; set; }

        #endregion
    }
}