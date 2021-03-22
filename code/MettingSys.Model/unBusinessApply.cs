using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_unBusinessApply, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class unBusinessApply
    {
        public unBusinessApply()
        { }

        /// <summary>
        /// 构造函数 MS_unBusinessApply
        /// </summary>
        /// <param name="uba_id">uba_id</param>
        /// <param name="uba_type">uba_type</param>
        /// <param name="uba_function">uba_function</param>
        /// <param name="uba_oid">uba_oid</param>
        /// <param name="uba_instruction">uba_instruction</param>
        /// <param name="uba_receivebank">uba_receiveBank</param>
        /// <param name="uba_receivebanknum">uba_receiveBankNum</param>
        /// <param name="uba_receivebankname">uba_receiveBankName</param>
        /// <param name="uba_money">uba_money</param>
        /// <param name="uba_foredate">uba_foreDate</param>
        /// <param name="uba_date">uba_date</param>
        /// <param name="uba_paymethod">uba_payMethod</param>
        /// <param name="uba_flag1">uba_flag1</param>
        /// <param name="uba_checknum1">uba_checkNum1</param>
        /// <param name="uba_checkname1">uba_checkName1</param>
        /// <param name="uba_checkremark1">uba_checkRemark1</param>
        /// <param name="uba_flag2">uba_flag2</param>
        /// <param name="uba_checknum2">uba_checkNum2</param>
        /// <param name="uba_checkname2">uba_checkName2</param>
        /// <param name="uba_checkremark2">uba_checkRemark2</param>
        /// <param name="uba_flag3">uba_flag3</param>
        /// <param name="uba_checknum3">uba_checkNum3</param>
        /// <param name="uba_checkname3">uba_checkName3</param>
        /// <param name="uba_checkremark3">uba_checkRemark3</param>
        /// <param name="uba_confirmer">uba_confirmer</param>
        /// <param name="uba_personnum">uba_PersonNum</param>
        /// <param name="uba_personname">uba_personName</param>
        /// <param name="uba_adddate">uba_addDate</param>
        /// <param name="uba_area">uba_area</param>
        public unBusinessApply(int? uba_id, byte? uba_type, string uba_function, string uba_oid, string uba_instruction, string uba_receivebank, string uba_receivebanknum, string uba_receivebankname, decimal? uba_money, decimal? uba_checkMoney, DateTime? uba_foredate, DateTime? uba_date, int? uba_paymethod, byte? uba_flag1, string uba_checknum1, string uba_checkname1, string uba_checkremark1,DateTime? uba_checkTime1, byte? uba_flag2, string uba_checknum2, string uba_checkname2, string uba_checkremark2, DateTime? uba_checkTime2, byte? uba_flag3, string uba_checknum3, string uba_checkname3, string uba_checkremark3, DateTime? uba_checkTime3, bool? uba_isconfirm, string uba_confirmernum, string uba_confirmername, string uba_personnum, string uba_personname, DateTime? uba_adddate, string uba_area, string uba_remark)
        {
            this.uba_id = uba_id;
            this.uba_type = uba_type;
            this.uba_function = uba_function;
            this.uba_oid = uba_oid;
            this.uba_instruction = uba_instruction;
            this.uba_receiveBank = uba_receivebank;
            this.uba_receiveBankNum = uba_receivebanknum;
            this.uba_receiveBankName = uba_receivebankname;
            this.uba_money = uba_money;
            this.uba_checkMoney = uba_checkMoney;
            this.uba_foreDate = uba_foredate;
            this.uba_date = uba_date;
            this.uba_payMethod = uba_paymethod;
            this.uba_flag1 = uba_flag1;
            this.uba_checkNum1 = uba_checknum1;
            this.uba_checkName1 = uba_checkname1;
            this.uba_checkRemark1 = uba_checkremark1;
            this.uba_checkTime1 = uba_checkTime1;
            this.uba_flag2 = uba_flag2;
            this.uba_checkNum2 = uba_checknum2;
            this.uba_checkName2 = uba_checkname2;
            this.uba_checkRemark2 = uba_checkremark2;
            this.uba_checkTime2 = uba_checkTime2;
            this.uba_flag3 = uba_flag3;
            this.uba_checkNum3 = uba_checknum3;
            this.uba_checkName3 = uba_checkname3;
            this.uba_checkRemark3 = uba_checkremark3;
            this.uba_checkTime3 = uba_checkTime3;
            this.uba_isConfirm = uba_isconfirm;
            this.uba_confirmerNum = uba_confirmernum;
            this.uba_confirmerName = uba_confirmername;
            this.uba_PersonNum = uba_personnum;
            this.uba_personName = uba_personname;
            this.uba_addDate = uba_adddate;
            this.uba_area = uba_area;
            this.uba_remark = uba_remark;
        }

        #region 实体属性

        /// <summary>
        /// uba_id
        /// </summary>
        public int? uba_id { get; set; }

        /// <summary>
        /// uba_type
        /// </summary>
        public byte? uba_type { get; set; }

        /// <summary>
        /// uba_function
        /// </summary>
        public string uba_function { get; set; }

        /// <summary>
        /// uba_oid
        /// </summary>
        public string uba_oid { get; set; }

        /// <summary>
        /// uba_instruction
        /// </summary>
        public string uba_instruction { get; set; }

        /// <summary>
        /// uba_receiveBank
        /// </summary>
        public string uba_receiveBank { get; set; }

        /// <summary>
        /// uba_receiveBankNum
        /// </summary>
        public string uba_receiveBankNum { get; set; }

        /// <summary>
        /// uba_receiveBankName
        /// </summary>
        public string uba_receiveBankName { get; set; }

        /// <summary>
        /// uba_money
        /// </summary>
        public decimal? uba_money { get; set; }

        /// <summary>
        /// uba_checkMoney
        /// </summary>
        public decimal? uba_checkMoney { get; set; }

        /// <summary>
        /// uba_foreDate
        /// </summary>
        public DateTime? uba_foreDate { get; set; }

        /// <summary>
        /// uba_date
        /// </summary>
        public DateTime? uba_date { get; set; }

        /// <summary>
        /// uba_payMethod
        /// </summary>
        public int? uba_payMethod { get; set; }

        /// <summary>
        /// uba_flag1
        /// </summary>
        public byte? uba_flag1 { get; set; }

        /// <summary>
        /// uba_checkNum1
        /// </summary>
        public string uba_checkNum1 { get; set; }

        /// <summary>
        /// uba_checkName1
        /// </summary>
        public string uba_checkName1 { get; set; }

        /// <summary>
        /// uba_checkRemark1
        /// </summary>
        public string uba_checkRemark1 { get; set; }

        /// <summary>
        /// uba_foreDate
        /// </summary>
        public DateTime? uba_checkTime1 { get; set; }

        /// <summary>
        /// uba_flag2
        /// </summary>
        public byte? uba_flag2 { get; set; }

        /// <summary>
        /// uba_checkNum2
        /// </summary>
        public string uba_checkNum2 { get; set; }

        /// <summary>
        /// uba_checkName2
        /// </summary>
        public string uba_checkName2 { get; set; }

        /// <summary>
        /// uba_checkRemark2
        /// </summary>
        public string uba_checkRemark2 { get; set; }

        /// <summary>
        /// uba_foreDate
        /// </summary>
        public DateTime? uba_checkTime2 { get; set; }
        /// <summary>
        /// uba_flag3
        /// </summary>
        public byte? uba_flag3 { get; set; }

        /// <summary>
        /// uba_checkNum3
        /// </summary>
        public string uba_checkNum3 { get; set; }

        /// <summary>
        /// uba_checkName3
        /// </summary>
        public string uba_checkName3 { get; set; }

        /// <summary>
        /// uba_checkRemark3
        /// </summary>
        public string uba_checkRemark3 { get; set; }

        /// <summary>
        /// uba_foreDate
        /// </summary>
        public DateTime? uba_checkTime3 { get; set; }

        /// <summary>
        /// uba_isConfirm
        /// </summary>
        public bool? uba_isConfirm { get; set; }

        /// <summary>
        /// uba_confirmerNum
        /// </summary>
        public string uba_confirmerNum { get; set; }

        /// <summary>
        /// uba_confirmerName
        /// </summary>
        public string uba_confirmerName { get; set; }

        /// <summary>
        /// uba_PersonNum
        /// </summary>
        public string uba_PersonNum { get; set; }

        /// <summary>
        /// uba_personName
        /// </summary>
        public string uba_personName { get; set; }

        /// <summary>
        /// uba_addDate
        /// </summary>
        public DateTime? uba_addDate { get; set; }

        /// <summary>
        /// uba_area
        /// </summary>
        public string uba_area { get; set; }

        /// <summary>
        /// uba_remark
        /// </summary>
        public string uba_remark { get; set; }
        #endregion
    }
}