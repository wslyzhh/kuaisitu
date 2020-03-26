using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MettingSys.Model
{
    /// <summary>
    /// 业务明细
    /// </summary>
    public partial class businessDetails
    {
        public businessDetails() { }

        private int _de_id = 0;
        private int _de_nid = 0;
        private string _de_name = string.Empty;
        private int _de_sort = 0;
        private bool? _de_isUse = true;
        /// <summary>
        /// 自增ID
        /// </summary>
        public int de_id
        {
            set { _de_id = value; }
            get { return _de_id; }
        }

        /// <summary>
        /// 业务性质表主键
        /// </summary>
        public int de_nid
        {
            set { _de_nid = value; }
            get { return _de_nid; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string de_name
        {
            set { _de_name = value; }
            get { return _de_name; }
        }
        /// <summary>
        /// 排序，越小越靠前
        /// </summary>
        public int de_sort
        {
            set { _de_sort = value; }
            get { return _de_sort; }
        }
        /// <summary>
        /// 启用状态
        /// </summary>
        public bool? de_isUse
        {
            set { _de_isUse = value; }
            get { return _de_isUse; }
        }
    }
}
