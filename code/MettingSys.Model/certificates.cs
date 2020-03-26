using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_certificates, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class certificates 
    {
        public certificates()
        { }

        /// <summary>
        /// 构造函数 MS_certificates
        /// </summary>
        /// <param name="ce_id">ce_id</param>
        /// <param name="ce_num">ce_num</param>
        /// <param name="ce_date">ce_date</param>
        /// <param name="ce_flag">ce_flag</param>
        /// <param name="ce_personNum">ce_addPerson</param>
        /// <param name="ce_adddate">ce_addDate</param>
        public certificates(int? ce_id, string ce_num, DateTime? ce_date, byte? ce_flag, string ce_checknum, string ce_checkname, string ce_checkremark, string ce_personNum,string ce_personName, DateTime? ce_adddate, string ce_remark)
        {
            this.ce_id = ce_id;
            this.ce_num = ce_num;
            this.ce_date = ce_date;
            this.ce_flag = ce_flag;
            this.ce_checkNum = ce_checknum;
            this.ce_checkName = ce_checkname;
            this.ce_checkRemark = ce_checkremark;
            this.ce_personNum = ce_personNum;
            this.ce_personName = ce_personName;
            this.ce_addDate = ce_adddate;
            this.ce_remark = ce_remark;
        }

        #region 实体属性

        /// <summary>
        /// ce_id
        /// </summary>
        public int? ce_id { get; set; }

        /// <summary>
        /// ce_num
        /// </summary>
        public string ce_num { get; set; }

        /// <summary>
        /// ce_date
        /// </summary>
        public DateTime? ce_date { get; set; }

        /// <summary>
        /// ce_flag
        /// </summary>
        public byte? ce_flag { get; set; }


        /// <summary>
        /// ce_checkNum
        /// </summary>
        public string ce_checkNum { get; set; }

        /// <summary>
        /// ce_checkName
        /// </summary>
        public string ce_checkName { get; set; }

        /// <summary>
        /// ce_checkRemark
        /// </summary>
        public string ce_checkRemark { get; set; }

        /// <summary>
        /// ce_personNum
        /// </summary>
        public string ce_personNum { get; set; }

        /// <summary>
        /// ce_personName
        /// </summary>
        public string ce_personName { get; set; }

        /// <summary>
        /// ce_addDate
        /// </summary>
        public DateTime? ce_addDate { get; set; }


        /// <summary>
        /// ce_remark
        /// </summary>
        public string ce_remark { get; set; }

        #endregion
    }
}