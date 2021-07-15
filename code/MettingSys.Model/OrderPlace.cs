using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.Model
{
    public partial class OrderPlace
    {
        public OrderPlace()
        { }

        public int? p_id { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string p_oid { get; set; }
        /// <summary>
        /// 区域简写 
        /// </summary>
        public string p_name { get; set; }
        /// <summary>
        /// 区域中文
        /// </summary>
        public string p_chnName { get; set; }
        /// <summary>
        /// 业绩比例
        /// </summary>
        public int? p_ratio { get; set; }
    }
}
