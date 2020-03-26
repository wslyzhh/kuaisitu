using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_invoices, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class invoices
    {
        public invoices()
        { }

        /// <summary>
        /// 构造函数 invoices
        /// </summary>
        /// <param name="inv_id">inv_id</param>
        /// <param name="inv_oid">inv_oid</param>
        /// <param name="inv_cid">inv_cid</param>
        /// <param name="inv_purchasername">inv_purchaserName</param>
        /// <param name="inv_purchasernum">inv_purchaserNum</param>
        /// <param name="inv_purchaseraddress">inv_purchaserAddress</param>
        /// <param name="inv_purchaserphone">inv_purchaserPhone</param>
        /// <param name="inv_purchaserbank">inv_purchaserBank</param>
        /// <param name="inv_purchaserbanknum">inv_purchaserBankNum</param>
        /// <param name="inv_servicetype">inv_serviceType</param>
        /// <param name="inv_servicename">inv_serviceName</param>
        /// <param name="inv_money">inv_money</param>
        /// <param name="inv_sentway">inv_sentWay</param>
        /// <param name="inv_farea">inv_farea</param>
        /// <param name="inv_darea">inv_darea</param>
        /// <param name="inv_receivename">inv_receiveName</param>
        /// <param name="inv_receivephone">inv_receivePhone</param>
        /// <param name="inv_receiveaddress">inv_receiveAddress</param>
        /// <param name="inv_remark">inv_remark</param>
        /// <param name="inv_personnum">inv_personNum</param>
        /// <param name="inv_personname">inv_personName</param>
        /// <param name="inv_adddate">inv_addDate</param>
        /// <param name="inv_flag1">inv_flag1</param>
        /// <param name="inv_checknum1">inv_checkNum1</param>
        /// <param name="inv_checkname1">inv_checkName1</param>
        /// <param name="inv_checkremark1">inv_checkRemark1</param>
        /// <param name="inv_flag2">inv_flag2</param>
        /// <param name="inv_checknum2">inv_checkNum2</param>
        /// <param name="inv_checkname2">inv_checkName2</param>
        /// <param name="inv_checkremark2">inv_checkRemark2</param>
        /// <param name="inv_flag3">inv_flag3</param>
        /// <param name="inv_checknum3">inv_checkNum3</param>
        /// <param name="inv_checkname3">inv_checkName3</param>
        /// <param name="inv_checkremark3">inv_checkRemark3</param>
        /// <param name="inv_isconfirm">inv_isConfirm</param>
        /// <param name="inv_confirmernum">inv_confirmerNum</param>
        /// <param name="inv_confirmername">inv_confirmerName</param>
        public invoices(int? inv_id, string inv_oid, int? inv_cid,bool? inv_type, string inv_purchasername, string inv_purchasernum, string inv_purchaseraddress, string inv_purchaserphone, string inv_purchaserbank, string inv_purchaserbanknum, string inv_servicetype, string inv_servicename, decimal? inv_money,decimal? inv_overMoney, string inv_sentway, string inv_farea, string inv_darea, string inv_receivename, string inv_receivephone, string inv_receiveaddress, string inv_remark, string inv_personnum, string inv_personname, DateTime? inv_adddate, byte? inv_flag1, string inv_checknum1, string inv_checkname1, string inv_checkremark1, byte? inv_flag2, string inv_checknum2, string inv_checkname2, string inv_checkremark2, byte? inv_flag3, string inv_checknum3, string inv_checkname3, string inv_checkremark3, bool? inv_isconfirm,DateTime? inv_date, string inv_confirmernum, string inv_confirmername)
        {
            this.inv_id = inv_id;
            this.inv_oid = inv_oid;
            this.inv_cid = inv_cid;
            this.inv_type = inv_type;
            this.inv_purchaserName = inv_purchasername;
            this.inv_purchaserNum = inv_purchasernum;
            this.inv_purchaserAddress = inv_purchaseraddress;
            this.inv_purchaserPhone = inv_purchaserphone;
            this.inv_purchaserBank = inv_purchaserbank;
            this.inv_purchaserBankNum = inv_purchaserbanknum;
            this.inv_serviceType = inv_servicetype;
            this.inv_serviceName = inv_servicename;
            this.inv_money = inv_money;
            this.inv_overMoney = inv_overMoney;
            this.inv_sentWay = inv_sentway;
            this.inv_farea = inv_farea;
            this.inv_darea = inv_darea;
            this.inv_receiveName = inv_receivename;
            this.inv_receivePhone = inv_receivephone;
            this.inv_receiveAddress = inv_receiveaddress;
            this.inv_remark = inv_remark;
            this.inv_personNum = inv_personnum;
            this.inv_personName = inv_personname;
            this.inv_addDate = inv_adddate;
            this.inv_flag1 = inv_flag1;
            this.inv_checkNum1 = inv_checknum1;
            this.inv_checkName1 = inv_checkname1;
            this.inv_checkRemark1 = inv_checkremark1;
            this.inv_flag2 = inv_flag2;
            this.inv_checkNum2 = inv_checknum2;
            this.inv_checkName2 = inv_checkname2;
            this.inv_checkRemark2 = inv_checkremark2;
            this.inv_flag3 = inv_flag3;
            this.inv_checkNum3 = inv_checknum3;
            this.inv_checkName3 = inv_checkname3;
            this.inv_checkRemark3 = inv_checkremark3;
            this.inv_isConfirm = inv_isconfirm;
            this.inv_date = inv_date;
            this.inv_confirmerNum = inv_confirmernum;
            this.inv_confirmerName = inv_confirmername;
        }

        #region 实体属性

        /// <summary>
        /// inv_id
        /// </summary>
        public int? inv_id { get; set; }

        /// <summary>
        /// inv_oid
        /// </summary>
        public string inv_oid { get; set; }

        /// <summary>
        /// inv_cid
        /// </summary>
        public int? inv_cid { get; set; }

        /// <summary>
        /// inv_type
        /// </summary>
        public bool? inv_type { get; set; }
        /// <summary>
        /// inv_purchaserName
        /// </summary>
        public string inv_purchaserName { get; set; }

        /// <summary>
        /// inv_purchaserNum
        /// </summary>
        public string inv_purchaserNum { get; set; }

        /// <summary>
        /// inv_purchaserAddress
        /// </summary>
        public string inv_purchaserAddress { get; set; }

        /// <summary>
        /// inv_purchaserPhone
        /// </summary>
        public string inv_purchaserPhone { get; set; }

        /// <summary>
        /// inv_purchaserBank
        /// </summary>
        public string inv_purchaserBank { get; set; }

        /// <summary>
        /// inv_purchaserBankNum
        /// </summary>
        public string inv_purchaserBankNum { get; set; }

        /// <summary>
        /// inv_serviceType
        /// </summary>
        public string inv_serviceType { get; set; }

        /// <summary>
        /// inv_serviceName
        /// </summary>
        public string inv_serviceName { get; set; }

        /// <summary>
        /// inv_money
        /// </summary>
        public decimal? inv_money { get; set; }

        /// <summary>
        /// inv_money
        /// </summary>
        public decimal? inv_overMoney { get; set; }

        /// <summary>
        /// inv_sentWay
        /// </summary>
        public string inv_sentWay { get; set; }

        /// <summary>
        /// inv_farea
        /// </summary>
        public string inv_farea { get; set; }

        /// <summary>
        /// inv_darea
        /// </summary>
        public string inv_darea { get; set; }

        /// <summary>
        /// inv_receiveName
        /// </summary>
        public string inv_receiveName { get; set; }

        /// <summary>
        /// inv_receivePhone
        /// </summary>
        public string inv_receivePhone { get; set; }

        /// <summary>
        /// inv_receiveAddress
        /// </summary>
        public string inv_receiveAddress { get; set; }

        /// <summary>
        /// inv_remark
        /// </summary>
        public string inv_remark { get; set; }

        /// <summary>
        /// inv_personNum
        /// </summary>
        public string inv_personNum { get; set; }

        /// <summary>
        /// inv_personName
        /// </summary>
        public string inv_personName { get; set; }

        /// <summary>
        /// inv_addDate
        /// </summary>
        public DateTime? inv_addDate { get; set; }

        /// <summary>
        /// inv_flag1
        /// </summary>
        public byte? inv_flag1 { get; set; }

        /// <summary>
        /// inv_checkNum1
        /// </summary>
        public string inv_checkNum1 { get; set; }

        /// <summary>
        /// inv_checkName1
        /// </summary>
        public string inv_checkName1 { get; set; }

        /// <summary>
        /// inv_checkRemark1
        /// </summary>
        public string inv_checkRemark1 { get; set; }

        /// <summary>
        /// inv_date
        /// </summary>
        public DateTime? inv_checkTime1 { get; set; }
        
        /// <summary>
        /// inv_flag2
        /// </summary>
        public byte? inv_flag2 { get; set; }

        /// <summary>
        /// inv_checkNum2
        /// </summary>
        public string inv_checkNum2 { get; set; }

        /// <summary>
        /// inv_checkName2
        /// </summary>
        public string inv_checkName2 { get; set; }

        /// <summary>
        /// inv_checkRemark2
        /// </summary>
        public string inv_checkRemark2 { get; set; }

        /// <summary>
        /// inv_date
        /// </summary>
        public DateTime? inv_checkTime2 { get; set; }
        /// <summary>
        /// inv_flag3
        /// </summary>
        public byte? inv_flag3 { get; set; }

        /// <summary>
        /// inv_checkNum3
        /// </summary>
        public string inv_checkNum3 { get; set; }

        /// <summary>
        /// inv_checkName3
        /// </summary>
        public string inv_checkName3 { get; set; }

        /// <summary>
        /// inv_checkRemark3
        /// </summary>
        public string inv_checkRemark3 { get; set; }

        /// <summary>
        /// inv_date
        /// </summary>
        public DateTime? inv_checkTime3 { get; set; }
        /// <summary>
        /// inv_isConfirm
        /// </summary>
        public bool? inv_isConfirm { get; set; }


        /// <summary>
        /// inv_date
        /// </summary>
        public DateTime? inv_date { get; set; }

        /// <summary>
        /// inv_confirmerNum
        /// </summary>
        public string inv_confirmerNum { get; set; }

        /// <summary>
        /// inv_confirmerName
        /// </summary>
        public string inv_confirmerName { get; set; }

        #endregion
    }
}