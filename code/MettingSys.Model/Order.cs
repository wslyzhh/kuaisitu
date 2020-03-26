using System;
using System.Collections.Generic;

namespace MettingSys.Model
{
    /// <summary>
    /// 订单表
    /// </summary>
    [Serializable]
    public partial class Order
    {
        public Order()
        { }

        /// <summary>
        /// 构造函数 MS_Order
        /// </summary>
        /// <param name="o_id">o_id</param>
        /// <param name="o_cid">o_cid</param>
        /// <param name="o_coid">o_coid</param>
        /// <param name="o_content">o_content</param>
        /// <param name="o_address">o_address</param>
        /// <param name="o_contractprice">o_contractPrice</param>
        /// <param name="o_contractcontent">o_contractContent</param>
        /// <param name="o_area">o_area</param>
        /// <param name="o_sdate">o_sdate</param>
        /// <param name="o_edate">o_edate</param>
        /// <param name="o_place">o_place</param>
        /// <param name="o_status">o_status</param>
        /// <param name="o_dstatus">o_dstatus</param>
        /// <param name="o_lockstatus">o_lockStatus</param>
        /// <param name="o_remarks">o_remarks</param>
        /// <param name="o_ispush">o_isPush</param>
        /// <param name="o_flag">o_flag</param>
        /// <param name="o_financecust">o_financeCust</param>
        /// <param name="o_adddate">o_addDate</param>
        /// <param name="o_lastupdatedate">o_lastUpdateDate</param>
        public Order(string o_id, int? o_cid, int? o_coid, string o_content, string o_address, string o_contractprice, string o_contractcontent, DateTime? o_sdate, DateTime? o_edate, string o_place, byte? o_status, byte? o_dstatus, byte? o_lockstatus, string o_remarks, bool? o_ispush, byte? o_flag, decimal? o_financecust, DateTime? o_adddate, DateTime? o_lastupdatedate,string o_finremarks)
        {
            this.o_id = o_id;
            this.o_cid = o_cid;
            this.o_coid = o_coid;
            this.o_content = o_content;
            this.o_address = o_address;
            this.o_contractPrice = o_contractprice;
            this.o_contractContent = o_contractcontent;
            this.o_sdate = o_sdate;
            this.o_edate = o_edate;
            this.o_place = o_place;
            this.o_status = o_status;
            this.o_lockStatus = o_lockstatus;
            this.o_remarks = o_remarks;
            this.o_isPush = o_ispush;
            this.o_flag = o_flag;
            this.o_financeCust = o_financecust;
            this.o_addDate = o_adddate;
            this.o_lastUpdateDate = o_lastupdatedate;
            this.o_finRemarks = o_finremarks;
        }

        #region 实体属性

        /// <summary>
        /// o_id
        /// </summary>
        public string o_id { get; set; }
        
        /// <summary>
        /// o_cid
        /// </summary>
        public int? o_cid { get; set; }

        /// <summary>
        /// o_coid
        /// </summary>
        public int? o_coid { get; set; }

        /// <summary>
        /// o_content
        /// </summary>
        public string o_content { get; set; }

        /// <summary>
        /// o_address
        /// </summary>
        public string o_address { get; set; }

        /// <summary>
        /// o_contractPrice
        /// </summary>
        public string o_contractPrice { get; set; }

        /// <summary>
        /// o_contractContent
        /// </summary>
        public string o_contractContent { get; set; }

        /// <summary>
        /// o_sdate
        /// </summary>
        public DateTime? o_sdate { get; set; }

        /// <summary>
        /// o_edate
        /// </summary>
        public DateTime? o_edate { get; set; }

        /// <summary>
        /// o_place
        /// </summary>
        public string o_place { get; set; }

        /// <summary>
        /// o_status
        /// </summary>
        public byte? o_status { get; set; }
        
        /// <summary>
        /// o_lockStatus
        /// </summary>
        public byte? o_lockStatus { get; set; }

        /// <summary>
        /// o_remarks
        /// </summary>
        public string o_remarks { get; set; }

        /// <summary>
        /// o_isPush
        /// </summary>
        public bool? o_isPush { get; set; }

        /// <summary>
        /// o_flag
        /// </summary>
        public byte? o_flag { get; set; }

        /// <summary>
        /// o_financeCust
        /// </summary>
        public decimal? o_financeCust { get; set; }

        /// <summary>
        /// o_addDate
        /// </summary>
        public DateTime? o_addDate { get; set; }

        /// <summary>
        /// o_lastUpdateDate
        /// </summary>
        public DateTime? o_lastUpdateDate { get; set; }

        public string o_finRemarks { get; set; }


        public List<OrderPerson> personlist = new List<OrderPerson>();


        public List<Files> filelist = new List<Files>();

        #endregion

    }
}