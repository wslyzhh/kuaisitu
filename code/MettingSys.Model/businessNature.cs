using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MettingSys.Model
{
    /// <summary>
    /// 业务性质
    /// </summary>
    public partial class businessNature
    {
        public businessNature() { }

        private int _na_id = 0;
        private string _na_name = string.Empty;
        private int _na_sort = 0;
        private bool? _na_isUse = true;
        private bool? _na_flag = false;
        private bool? _na_type = false;

        /// <summary>
        /// 自增ID
        /// </summary>
        public int na_id
        {
            set { _na_id = value; }
            get { return _na_id; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string na_name
        {
            set { _na_name = value; }
            get { return _na_name; }
        }
        /// <summary>
        /// 排序，越小越靠前
        /// </summary>
        public int na_sort
        {
            set { _na_sort = value; }
            get { return _na_sort; }
        }
        /// <summary>
        /// 启用状态
        /// </summary>
        public bool? na_isUse
        {
            set { _na_isUse = value; }
            get { return _na_isUse; }
        }

        /// <summary>
        /// 财务使用0否1是
        /// </summary>
        public bool? na_flag
        {
            set { _na_flag = value; }
            get { return _na_flag; }
        }

        /// <summary>
        /// 明细类别：0业务明细，1员工
        /// </summary>
        public bool? na_type
        {
            set { _na_type = value; }
            get { return _na_type; }
        }
    }
}
