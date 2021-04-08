using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.Model
{
    [Serializable]
    public partial class unBusinessMoneyLog
    {
        public unBusinessMoneyLog() 
        {
            
        }
        public unBusinessMoneyLog(int? ubml_id,int? ubml_ubaid, byte? ubml_type, decimal? ubml_newMoney, decimal? ubml_oldMoney, string ubml_username, string ubml_realname, DateTime? ubml_date,string ubml_remark)
        {
            this.ubml_id = ubml_id;
            this.ubml_ubaid = ubml_ubaid;
            this.ubml_type = ubml_type;
            this.ubml_newMoney = ubml_newMoney;
            this.ubml_oldMoney = ubml_oldMoney;
            this.ubml_username = ubml_username;
            this.ubml_realname = ubml_realname;
            this.ubml_date = ubml_date;
            this.ubml_remark = ubml_remark;
        }

        #region 实体属性

        /// <summary>
        /// ubml_id
        /// </summary>
        public int? ubml_id { get; set; }

        /// <summary>
        /// ubml_ubaid
        /// </summary>
        public int? ubml_ubaid { get; set; }

        /// <summary>
        /// ubml_type
        /// </summary>
        public byte? ubml_type { get; set; }

        /// <summary>
        /// ubml_newMoney
        /// </summary>
        public decimal? ubml_newMoney { get; set; }

        /// <summary>
        /// ubml_oldMoney
        /// </summary>
        public decimal? ubml_oldMoney { get; set; }

        /// <summary>
        /// ubml_username
        /// </summary>
        public string ubml_username { get; set; }

        /// <summary>
        /// ubml_realname
        /// </summary>
        public string ubml_realname { get; set; }

        /// <summary>
        /// ubml_date
        /// </summary>
        public DateTime? ubml_date { get; set; }

        public string ubml_remark { get; set; }

        #endregion
    }
}
