using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.Model
{
    [Serializable]
    public partial class publicSetting
    {
        public publicSetting() { }

        /// <summary>
        /// 构造函数 MS_Files
        /// </summary>
        /// <param name="ps_id">ps_id</param>
        /// <param name="ps_type">ps_type</param>
        /// <param name="ps_isuse">ps_isuse</param>
        /// <param name="ps_sdate">ps_sdate</param>
        /// <param name="ps_edate">ps_edate</param>
        /// <param name="ps_ratio">ps_ratio</param>
        public publicSetting(int? ps_id, byte? ps_type, bool ps_isuse, DateTime? ps_sdate, DateTime? ps_edate, int? ps_ratio)
        {
            this.ps_id = ps_id;
            this.ps_type = ps_type;
            this.ps_isuse = ps_isuse;
            this.ps_sdate = ps_sdate;
            this.ps_edate = ps_edate;
            this.ps_ratio = ps_ratio;
        }

        #region 实体属性

        /// <summary>
        /// ps_id
        /// </summary>
        public int? ps_id { get; set; }

        /// <summary>
        /// ps_type
        /// </summary>
        public byte? ps_type { get; set; }

        /// <summary>
        /// ps_isuse
        /// </summary>
        public bool ps_isuse { get; set; }

        /// <summary>
        /// ps_sdate
        /// </summary>
        public DateTime? ps_sdate { get; set; }

        /// <summary>
        /// ps_edate
        /// </summary>
        public DateTime? ps_edate { get; set; }

        /// <summary>
        /// ps_ratio
        /// </summary>
        public int? ps_ratio { get; set; }

        #endregion
    }
}
