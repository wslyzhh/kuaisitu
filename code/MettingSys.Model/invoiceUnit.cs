using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.Model
{
    [Serializable]
    public partial class invoiceUnit
    {
        public invoiceUnit() { }

        /// <summary>
        /// 构造函数 invoiceUnit
        /// </summary>
        /// <param name="inv_id">inv_id</param>
        /// <param name="inv_flag1">inv_flag1</param>
        public invoiceUnit(int? invU_id, string invU_area,string invU_name,string invU_contact,string invU_contactPhone,bool? invU_flag)
        {
            this.invU_id = invU_id;
            this.invU_area = invU_area;
            this.invU_name = invU_name;
            this.invU_contact = invU_contact;
            this.invU_contactPhone = invU_contactPhone;
            this.invU_flag = invU_flag;
        }

        #region 实体属性

        /// <summary>
        /// invU_id
        /// </summary>
        public int? invU_id { get; set; }

        /// <summary>
        /// invU_area
        /// </summary>
        public string invU_area { get; set; }

        /// <summary>
        /// invU_name
        /// </summary>
        public string invU_name { get; set; }

        /// <summary>
        /// invU_contact
        /// </summary>
        public string invU_contact { get; set; }

        /// <summary>
        /// invU_contact
        /// </summary>
        public string invU_contactPhone { get; set; }

        /// <summary>
        /// invU_flag
        /// </summary>
        public bool? invU_flag { get; set; }
        #endregion
    }
}
